#region FileInfo

// 
// File: LSLCodeFormatterVisitor.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 

#endregion

#region Imports

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibLSLCC.CodeValidator;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeFormatter
{
    /// <summary>
    ///     An LSL Syntax tree visitor that formats code.
    /// </summary>
    internal sealed class LSLCodeFormatterVisitor : LSLValidatorNodeVisitor<bool>
    {
        private readonly LinkedList<LSLComment> _comments = new LinkedList<LSLComment>();

        private readonly Stack<Tuple<bool, ExpressionWrappingContext>> _expressionContextStack =
            new Stack<Tuple<bool, ExpressionWrappingContext>>();


        private readonly Stack<Tuple<bool, ExpressionListWrappingContext>> _expressionListContextStack =
            new Stack<Tuple<bool, ExpressionListWrappingContext>>();

        private int _binaryExpressionsSinceNewLine;
        private int _indentLevel;
        private int _nonTabsWrittenSinceLastLine;
        private string _sourceReference;
        private int _tabsWrittenSinceLastLine;
        private int _writeColumn;
        private int _writeLine;
        private bool _wroteCommentAfterControlChainMember;
        private bool _wroteCommentAfterEventParameterList;
        private bool _wroteCommentAfterFunctionDeclarationParameterList;
        private bool _wroteCommentBeforeControlStatementCode;


        private int GetCharacterColumnsSinceLastLine()
        {
            return (Settings.TabString.Length*_tabsWrittenSinceLastLine) + _nonTabsWrittenSinceLastLine;
        }

        /// <summary>
        ///     Default constructor for the formatting visitor.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="settings" /> is <c>null</c>.</exception>
        public LSLCodeFormatterVisitor(LSLCodeFormatterSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            Settings = settings;
        }


        public LSLCodeFormatterSettings Settings { get; private set; }

        /// <summary>
        ///     The TextWriter that was passed to the WriteAndFlush function.
        /// </summary>
        public TextWriter Writer { get; private set; }

        private ExpressionWrappingContext CurrentExpressionWrappingContext
        {
            get { return _expressionContextStack.Count > 0 ? _expressionContextStack.Peek().Item2 : null; }
        }

        private bool ExpressionWrappingCurrentlyEnabled
        {
            get { return _expressionContextStack.Count > 0 && _expressionContextStack.Peek().Item1; }
        }




        private ExpressionListWrappingContext CurrentExpressionListWrappingContext
        {
            get { return _expressionListContextStack.Count > 0 ? _expressionListContextStack.Peek().Item2 : null; }
        }

        private bool ExpressionWrappingListCurrentlyEnabled
        {
            get { return _expressionListContextStack.Count > 0 && _expressionListContextStack.Peek().Item1; }
        }

        private void Reset()
        {
            _expressionContextStack.Clear();
            _comments.Clear();


            _indentLevel = 0;
            _sourceReference = "";
            _writeColumn = 0;
            _writeLine = 0;
            _tabsWrittenSinceLastLine = 0;
            _nonTabsWrittenSinceLastLine = 0;
            _binaryExpressionsSinceNewLine = 0;
            _wroteCommentBeforeControlStatementCode = false;
        }


        private void ExpressionWrappingPush(bool enabled, ExpressionWrappingContext context)
        {
            if (enabled && context == null)
            {
                throw new ArgumentNullException("context",
                    "ExpressionWrappingContext cannot be null if 'enabled' is true!.");
            }

            _expressionContextStack.Push(Tuple.Create(enabled, context));
        }


        private void ExpressionListWrappingPush(bool enabled, ExpressionListWrappingContext context)
        {
            if (enabled && context == null)
            {
                throw new ArgumentNullException("context",
                    "ExpressionListWrappingContext cannot be null if 'enabled' is true!.");
            }

            _expressionListContextStack.Push(Tuple.Create(enabled, context));
        }


        private Tuple<bool, ExpressionWrappingContext> ExpressionWrappingPop()
        {
            return _expressionContextStack.Pop();
        }
        private Tuple<bool, ExpressionListWrappingContext> ExpressionListWrappingPop()
        {
            return _expressionListContextStack.Pop();
        }

        private string GenIndent(int add = 0)
        {
            return LSLFormatTools.CreateTabsString(_indentLevel + add);
        }


        private void Write(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            var strBuilder = new StringBuilder();

            foreach (var c in str)
            {
                if (c == '\n')
                {
                    _writeLine++;
                    _writeColumn = 0;
                    _tabsWrittenSinceLastLine = 0;
                    _nonTabsWrittenSinceLastLine = 0;
                    OnNewLineWritten();

                    strBuilder.Append(Settings.NewlineSequence);
                }
                else if (c == '\t')
                {
                    _tabsWrittenSinceLastLine++;
                    //OnTabWritten();

                    strBuilder.Append(Settings.TabString);
                }
                else
                {
                    _nonTabsWrittenSinceLastLine++;
                    //OnNonTabWritten();

                    strBuilder.Append(c);
                }

                if (c == '\n') continue;

                _writeColumn++;
                //OnColumnCharacterWritten();
            }

            Writer.Write(strBuilder.ToString());
        }


        /*private void OnColumnCharacterWritten()
        {
            //
        }*/

        /*private void OnNonTabWritten()
        {
            //
        }*/

        /*private void OnTabWritten()
        {
            //
        }*/


        private void OnNewLineWritten()
        {
            _binaryExpressionsSinceNewLine = 0;
        }


        /// <summary>
        ///     Formats an <see cref="ILSLCompilationUnitNode" /> to an output writer, <paramref name="sourceReference" /> is only
        ///     required if you want to keep comments.
        /// </summary>
        /// <param name="sourceReference">
        ///     The source code of the script, only necessary if comments exist.  Passing <c>null</c>
        ///     will cause all comments to be stripped, regardless of formatter settings.
        /// </param>
        /// <param name="compilationUnit">The top level <see cref="ILSLCompilationUnitNode" /> syntax tree node to format.</param>
        /// <param name="writer">The writer to write the formated source code to.</param>
        /// <param name="closeStream">
        ///     <c>true</c> if this method should close <paramref name="writer" /> when finished.  The
        ///     default value is <c>false</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.HasErrors" /> is <c>true</c> in
        ///     <paramref name="compilationUnit" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="compilationUnit" /> or <paramref name="writer" /> is
        ///     <c>null</c>.
        /// </exception>
        public void WriteAndFlush(string sourceReference, ILSLCompilationUnitNode compilationUnit, TextWriter writer,
            bool closeStream = false)
        {
            if (compilationUnit == null)
            {
                throw new ArgumentNullException("compilationUnit");
            }

            if (compilationUnit.HasErrors)
            {
                throw new ArgumentException(typeof (ILSLCompilationUnitNode).Name +
                                            ".HasErrors is true, cannot format a tree with syntax errors.");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            try
            {
                _sourceReference = sourceReference;

                if (_sourceReference != null && !Settings.RemoveComments)
                {
                    foreach (var comment in compilationUnit.Comments)
                    {
                        _comments.AddLast(comment);
                    }
                }

                Writer = writer;

                Visit(compilationUnit);

                Writer.Flush();
            }
            finally
            {
                if (closeStream)
                {
                    Writer.Close();
                }

                Reset();
            }
        }


        private IList<LSLComment> CommentsBetweenRange(LSLSourceCodeRange left, LSLSourceCodeRange right)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");

            return GetComments(left.StopIndex, right.StartIndex).ToList();
        }


        private bool WriteCommentsBetweenRange(LSLSourceCodeRange left, LSLSourceCodeRange right,
            int existingNewLinesBetweenNextNode = 0)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");

            var comments = CommentsBetweenRange(left, right).ToList();
            return WriteCommentsBetweenRange(comments, left, right, existingNewLinesBetweenNextNode);
        }


        private bool WriteCommentsBetweenRange(
            IList<LSLComment> comments,
            LSLSourceCodeRange left,
            LSLSourceCodeRange right,
            int existingNewLinesBetweenNextNode = 0)
        {

            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");


            if (comments.Count == 0) return false;


            var linesBetweenCommentAndLeftRange = (comments[0].SourceRange.LineStart - left.LineEnd);

            if (linesBetweenCommentAndLeftRange == 0)
            {
                var spacesBetweenCommentAndLeftRange = ((comments[0].SourceRange.StartIndex - left.StopIndex) -
                                                        1);

                spacesBetweenCommentAndLeftRange = spacesBetweenCommentAndLeftRange > 0
                    ? spacesBetweenCommentAndLeftRange
                    : 1;


                Write(LSLFormatTools.CreateTabCorrectSpaceString(spacesBetweenCommentAndLeftRange));
            }

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndLeftRange));

            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];

                if (comment.SourceRange.LineStart != left.LineEnd)
                {
                    Write(GenIndent() + comment.Text);
                }
                else
                {
                    Write(comment.Text);
                }

                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];

                    var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                comment.SourceRange.LineEnd);

                    if (linesBetweenComments == 0)
                    {
                        var spacesBetweenComments = (nextComment.SourceRange.StartIndex -
                                                     comment.SourceRange.StopIndex);

                        Write(LSLFormatTools.CreateTabCorrectSpaceString(spacesBetweenComments));
                    }

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndRightRange = (right.LineStart - comment.SourceRange.LineEnd);

                    if (linesBetweenCommentAndRightRange == 0)
                    {
                        var spacesBetweenCommentAndNextNode = (right.StartIndex - comment.SourceRange.StopIndex);

                        spacesBetweenCommentAndNextNode = spacesBetweenCommentAndNextNode > 0
                            ? spacesBetweenCommentAndNextNode
                            : 1;

                        Write(LSLFormatTools.CreateTabCorrectSpaceString(spacesBetweenCommentAndNextNode));
                    }
                    else
                    {
                        var newLinesCnt = linesBetweenCommentAndRightRange - existingNewLinesBetweenNextNode;

                        var newLines = LSLFormatTools.CreateNewLinesString(newLinesCnt);

                        if (newLinesCnt > 0)
                        {
                            newLines += GenIndent();
                        }

                        Write(newLines);
                    }
                }
            }

            return true;
        }


        public override bool VisitBinaryExpression(ILSLBinaryExpressionNode node)
        {
            bool cannotWriteComments;


            if (node.Parent is ILSLExpressionStatementNode && Settings.StatementExpressionWrapping)
            {
                Visit(node.LeftExpression);

                cannotWriteComments = !node.LeftExpression.SourceRangesAvailable || !node.SourceRangesAvailable;

                if (cannotWriteComments || 
                    !WriteCommentsBetweenRange(node.LeftExpression.SourceRange, node.SourceRangeOperation))
                {
                    Write(" ");
                }

                Write(node.OperationString);

                _binaryExpressionsSinceNewLine++;

                var wrappingContext = new ExpressionWrappingContext(node, this)
                {
                    ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeStatementExpressionWrap,
                    MinimumExpressionsToWrap = Settings.MinimumExpressionsInStatementToWrap
                };

                cannotWriteComments = !node.RightExpression.SourceRangesAvailable || !node.SourceRangesAvailable;


                if (cannotWriteComments || 
                    !WriteCommentsBetweenRange(node.SourceRangeOperation, node.RightExpression.SourceRange))
                {
                    Write(" ");
                }


                ExpressionWrappingPush(true, wrappingContext);
                Visit(node.RightExpression);

                ExpressionWrappingPop();

                return true;
            }


            Visit(node.LeftExpression);


            cannotWriteComments = !node.LeftExpression.SourceRangesAvailable || !node.SourceRangesAvailable;


            if (cannotWriteComments || 
                !WriteCommentsBetweenRange(node.LeftExpression.SourceRange, node.SourceRangeOperation))
            {
                Write(" ");
            }


            Write(node.OperationString);


            _binaryExpressionsSinceNewLine++;


            if (ExpressionWrappingCurrentlyEnabled &&
                ((_writeColumn - CurrentExpressionWrappingContext.WriteColumn) >
                 CurrentExpressionWrappingContext.ColumnsBeforeExpressionWrap
                 && _binaryExpressionsSinceNewLine >= CurrentExpressionWrappingContext.MinimumExpressionsToWrap))
            {
                Write("\n" +
                      LSLFormatTools.CreateTabsString(CurrentExpressionWrappingContext.TabsWrittenSinceLastLine) +
                      LSLFormatTools.CreateTabCorrectSpaceString(
                          CurrentExpressionWrappingContext.NonTabsWrittenSinceLastLine));
            }


            cannotWriteComments = !node.RightExpression.SourceRangesAvailable || !node.SourceRangesAvailable;

            if (cannotWriteComments || 
                !WriteCommentsBetweenRange(node.SourceRangeOperation, node.RightExpression.SourceRange))
            {
                Write(" ");
            }


            Visit(node.RightExpression);


            return true;
        }


        public override bool VisitListLiteralInitializerList(ILSLExpressionListNode node)
        {
            ExpressionWrappingPush(false, null);


            var nodeCount = node.Expressions.Count;


            ExpressionWrappingPush(false, null);


            if (nodeCount > 0)
            {
                var first = node.Expressions[0];

                ExpressionListWrappingPush(true, new ExpressionListWrappingContext(first, this)
                {
                    MaximumCharactersBeforeWrap = Settings.MaximumCharactersBeforeListLiteralWrap,
                    ForceWrapping = true
                });
            }



            VisitExpressionList(node);


            if (nodeCount > 0)
            {
                ExpressionListWrappingPop();
            }

            ExpressionWrappingPop();

            return true;
        }


        public override bool VisitFunctionCallArguments(ILSLExpressionListNode node)
        {
            var nodeCount = node.Expressions.Count;


            ExpressionWrappingPush(false, null);


            if (nodeCount > 0)
            {
                var first = node.Expressions[0];

                ExpressionListWrappingPush(true, new ExpressionListWrappingContext(first, this)
                {
                    MaximumCharactersBeforeWrap = Settings.MaximumCharactersBeforeArgumentListWrap,
                    ForceWrapping = true
                });
            }



            VisitExpressionList(node);


            if (nodeCount > 0)
            {
                ExpressionListWrappingPop();
            }

            ExpressionWrappingPop();

            return true;
        }


        public override bool VisitFloatLiteral(ILSLFloatLiteralNode node)
        {
            Write(node.RawText);

            return true;
        }


        public override bool VisitFunctionCall(ILSLFunctionCallNode node)
        {
            Write(node.Name + "(");
            Visit(node.ArgumentExpressionList);
            Write(")");

            return true;
        }


        public override bool VisitIntegerLiteral(ILSLIntegerLiteralNode node)
        {
            Write(node.RawText);

            return true;
        }


        public override bool VisitListLiteral(ILSLListLiteralNode node)
        {
            Write("[");

            Visit(node.ExpressionList);

            Write("]");

            return true;
        }


        public override bool VisitParenthesizedExpression(ILSLParenthesizedExpressionNode node)
        {
            ExpressionWrappingPush(false, null);

            Write("(");

            var canWriteComments = node.SourceRangesAvailable && node.InnerExpression.SourceRangesAvailable;

            if(canWriteComments)
                WriteCommentsBetweenRange(node.SourceRange.FirstCharRange, node.InnerExpression.SourceRange);


            Visit(node.InnerExpression);

            if (canWriteComments)
                WriteCommentsBetweenRange(node.InnerExpression.SourceRange, node.SourceRange.LastCharRange);


            Write(")");

            ExpressionWrappingPop();

            return true;
        }


        public override bool VisitPostfixOperation(ILSLPostfixOperationNode node)
        {
            Visit(node.LeftExpression);

            if(node.LeftExpression.SourceRangesAvailable && node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.LeftExpression.SourceRange, node.SourceRangeOperation);

            Write(node.OperationString);

            return true;
        }


        public override bool VisitPrefixOperation(ILSLPrefixOperationNode node)
        {
            Write(node.OperationString);

            if (node.SourceRangesAvailable && node.RightExpression.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeOperation, node.RightExpression.SourceRange);

            Visit(node.RightExpression);
            return true;
        }


        public override bool VisitRotationLiteral(ILSLRotationLiteralNode node)
        {
            ExpressionWrappingPush(false, null);

            Write("<");



            bool commentsBetween = false;
            var componentRangesAvailable = node.XExpression.SourceRangesAvailable && node.SourceRangesAvailable;


            if(componentRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRange.FirstCharRange, node.XExpression.SourceRange);

            Visit(node.XExpression);

            if (componentRangesAvailable)
                WriteCommentsBetweenRange(node.XExpression.SourceRange, node.SourceRangeCommaOne);



            Write(",");




            componentRangesAvailable = node.YExpression.SourceRangesAvailable && node.SourceRangesAvailable;

            
            if(componentRangesAvailable)
                commentsBetween = WriteCommentsBetweenRange(node.SourceRangeCommaOne,
                    node.YExpression.SourceRange);

            if (!commentsBetween)
            {
                Write(" ");
            }


            Visit(node.YExpression);

            if (componentRangesAvailable)
                WriteCommentsBetweenRange(node.YExpression.SourceRange, node.SourceRangeCommaTwo);



            Write(",");




            componentRangesAvailable = node.ZExpression.SourceRangesAvailable && node.SourceRangesAvailable;

            if (componentRangesAvailable)
                commentsBetween = WriteCommentsBetweenRange(node.SourceRangeCommaTwo, node.ZExpression.SourceRange);

            if (!commentsBetween)
            {
                Write(" ");
            }


            Visit(node.ZExpression);

            if (componentRangesAvailable)
                WriteCommentsBetweenRange(node.ZExpression.SourceRange, node.SourceRangeCommaThree);




            Write(",");



            componentRangesAvailable = node.ZExpression.SourceRangesAvailable && node.SourceRangesAvailable;

            if (componentRangesAvailable)
                commentsBetween = WriteCommentsBetweenRange(node.SourceRangeCommaThree, node.SExpression.SourceRange);

            if (!commentsBetween)
            {
                Write(" ");
            }

            Visit(node.SExpression);

            if (componentRangesAvailable)
                WriteCommentsBetweenRange(node.SExpression.SourceRange, node.SourceRange.LastCharRange);


            Write(">");

            ExpressionWrappingPop();

            return true;
        }


        public override bool VisitStringLiteral(ILSLStringLiteralNode node)
        {
            Write(node.RawText);
            return true;
        }


        public override bool VisitTypecastExpression(ILSLTypecastExprNode node)
        {
            Write("(");

            if(node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.SourceRangeCastToType);


            Write(node.CastToTypeName);


            if (node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeCastToType.LastCharRange, node.SourceRangeCloseParenth);


            Writer.Write(")");


            if (node.SourceRangesAvailable && node.CastedExpression.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeCloseParenth, node.CastedExpression.SourceRange);

            Visit(node.CastedExpression);

            return true;
        }


        public override bool VisitVariableReference(ILSLVariableNode node)
        {
            Write(node.Name);

            return true;
        }


        public override bool VisitVecRotAccessor(ILSLTupleAccessorNode node)
        {
            Visit(node.AccessedExpression);
            Write("." + node.AccessedComponent);

            return true;
        }


        public override bool VisitVectorLiteral(ILSLVectorLiteralNode node)
        {
            ExpressionWrappingPush(false, null);

            Write("<");


            bool commentsBetween = false;

            var sourceRangesavailable = node.XExpression.SourceRangesAvailable && node.SourceRangesAvailable;


            if (sourceRangesavailable)
                WriteCommentsBetweenRange(node.SourceRange.FirstCharRange, node.XExpression.SourceRange);

            Visit(node.XExpression);

            if (sourceRangesavailable)
                WriteCommentsBetweenRange(node.XExpression.SourceRange, node.SourceRangeCommaOne);

            Write(",");



            sourceRangesavailable = node.YExpression.SourceRangesAvailable && node.SourceRangesAvailable;


            if (sourceRangesavailable)
                commentsBetween = WriteCommentsBetweenRange(node.SourceRangeCommaOne,
                    node.YExpression.SourceRange);

            if (!commentsBetween)
            {
                Write(" ");
            }


            Visit(node.YExpression);

            if (sourceRangesavailable)
                WriteCommentsBetweenRange(node.YExpression.SourceRange, node.SourceRangeCommaTwo);



            Write(",");



            sourceRangesavailable = node.ZExpression.SourceRangesAvailable && node.SourceRangesAvailable;

            if (sourceRangesavailable)
                commentsBetween = WriteCommentsBetweenRange(node.SourceRangeCommaTwo, node.ZExpression.SourceRange);

            if (!commentsBetween)
            {
                Write(" ");
            }


            Visit(node.ZExpression);

            if (sourceRangesavailable)
                WriteCommentsBetweenRange(node.ZExpression.SourceRange, node.SourceRange.LastCharRange);


            Write(">");

            ExpressionWrappingPop();

            return true;
        }


        public override bool VisitDoLoop(ILSLDoLoopNode node)
        {
            Write("do");

            bool canWriteComments = node.SourceRangesAvailable && node.Code.SourceRangesAvailable;

            if (canWriteComments)
            {
                if (node.Code.IsSingleStatementScope)
                {
                    _indentLevel++;


                    _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeDoKeyword,
                        node.Code.SourceRange, 1);

                    _indentLevel--;
                }
                else
                {
                    _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeDoKeyword,
                        node.Code.SourceRange, 1);
                }
            }


            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            bool wroteComments = false;

            if(canWriteComments)
                wroteComments = WriteCommentsBetweenRange(node.Code.SourceRange, node.SourceRangeWhileKeyword);

            if (!wroteComments)
            {
                Write("\n" + GenIndent());
            }

            Write("while");

            
            if(node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeWhileKeyword, node.SourceRangeOpenParenth);


            Write("(");


            canWriteComments = node.SourceRangesAvailable && node.ConditionExpression.SourceRangesAvailable;

            if (canWriteComments)
                WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.ConditionExpression.SourceRange);


            var wrappingContext = new ExpressionWrappingContext(node, this)
            {
                ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeDoWhileExpressionWrap,
                MinimumExpressionsToWrap = Settings.MinimumExpressionsInDoWhileToWrap
            };

            ExpressionWrappingPush(Settings.DoWhileExpressionWrapping, wrappingContext);
            Visit(node.ConditionExpression);
            ExpressionWrappingPop();


            if (canWriteComments)
                WriteCommentsBetweenRange(node.ConditionExpression.SourceRange, node.SourceRangeCloseParenth);

            Write(")");

            if(node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeCloseParenth, node.SourceRangeSemicolon);

            Write(";");

            return true;
        }


        public override bool VisitForLoop(ILSLForLoopNode node)
        {
            Write("for");

            if(node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeForKeyword, node.SourceRangeOpenParenth);

            Write("(");


            bool canWriteComments = node.SourceRangesAvailable && node.InitExpressionList.SourceRangesAvailable;

            
            if (node.HasInitExpressions)
            {
                if (canWriteComments)
                    WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.InitExpressionList.SourceRange);

                ExpressionWrappingPush(false, null);



                ExpressionListWrappingPush(true,
                    new ExpressionListWrappingContext(node.InitExpressionList.Expressions.First(), this));

                VisitExpressionList(node.InitExpressionList);

                ExpressionListWrappingPop();


                ExpressionWrappingPop();

                if (canWriteComments)
                    WriteCommentsBetweenRange(node.InitExpressionList.SourceRange, node.SourceRangeFirstSemicolon);
            }
            else
            {
                if (canWriteComments)
                    WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.SourceRangeFirstSemicolon);
            }
            


            if (node.HasConditionExpression)
            {
                canWriteComments = node.SourceRangesAvailable && node.ConditionExpression.SourceRangesAvailable;


                IList<LSLComment> commentsBetween = null;
                if (canWriteComments)
                {
                    commentsBetween = CommentsBetweenRange(node.SourceRangeFirstSemicolon,
                        node.ConditionExpression.SourceRange);

                    Write(commentsBetween.Count > 0 ? ";" : "; ");
                }
                else
                {
                    Write("; ");
                }

                
                if(canWriteComments)
                    WriteCommentsBetweenRange(commentsBetween, node.SourceRangeFirstSemicolon,
                    node.ConditionExpression.SourceRange);


                ExpressionWrappingPush(false, null);
                Visit(node.ConditionExpression);
                ExpressionWrappingPop();

                if(canWriteComments)
                    WriteCommentsBetweenRange(node.ConditionExpression.SourceRange, node.SourceRangeSecondSemicolon);
            }
            else
            {
                Write(";");

                if(node.SourceRangesAvailable)
                    WriteCommentsBetweenRange(node.SourceRangeFirstSemicolon, node.SourceRangeSecondSemicolon);
            }


            if (node.HasAfterthoughtExpressions)
            {
                canWriteComments = node.SourceRangesAvailable && node.AfterthoughtExpressionList.SourceRangesAvailable;

                IList<LSLComment> commentsBetween = null;

                if (canWriteComments)
                {
                    commentsBetween =
                        CommentsBetweenRange(
                            node.SourceRangeSecondSemicolon,
                            node.AfterthoughtExpressionList.SourceRange);

                    Write(commentsBetween.Count > 0 ? ";" : "; ");
                }
                else
                {
                    Write("; ");
                }

                if(canWriteComments)
                    WriteCommentsBetweenRange(commentsBetween, node.SourceRangeSecondSemicolon,
                        node.AfterthoughtExpressionList.SourceRange);

                ExpressionWrappingPush(false, null);

                ExpressionListWrappingPush(true,
                    new ExpressionListWrappingContext(node.InitExpressionList.Expressions.First(), this));

                VisitExpressionList(node.AfterthoughtExpressionList);

                ExpressionListWrappingPop();

                ExpressionWrappingPop();

                if(canWriteComments)
                    WriteCommentsBetweenRange(node.AfterthoughtExpressionList.SourceRange, node.SourceRangeCloseParenth);
            }
            else
            {
                Write(";");

                if(node.SourceRangesAvailable)
                    WriteCommentsBetweenRange(node.SourceRangeSecondSemicolon, node.SourceRangeCloseParenth);
            }


            Write(")");


            canWriteComments = node.SourceRangesAvailable && node.Code.SourceRangesAvailable;

            if (canWriteComments)
            {
                if (node.Code.IsSingleStatementScope)
                {
                    _indentLevel++;

                    _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeCloseParenth,
                        node.Code.SourceRange, 1);

                    _indentLevel--;
                }
                else
                {
                    _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeCloseParenth,
                        node.Code.SourceRange, 1);
                }
            }

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            return true;
        }


        public override bool VisitWhileLoop(ILSLWhileLoopNode node)
        {
            Write("while");

            var canWriteComments = node.SourceRangesAvailable;

            if(canWriteComments)
                WriteCommentsBetweenRange(node.SourceRangeWhileKeyword, node.SourceRangeOpenParenth);

            Write("(");


            var wrappingContext = new ExpressionWrappingContext(node, this)
            {
                ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeWhileExpressionWrap,
                MinimumExpressionsToWrap = Settings.MinimumExpressionsInWhileToWrap
            };

            ExpressionWrappingPush(Settings.WhileExpressionWrapping, wrappingContext);
            Visit(node.ConditionExpression);
            ExpressionWrappingPop();

            canWriteComments = node.SourceRangesAvailable && node.ConditionExpression.SourceRangesAvailable;

            if(canWriteComments)
                WriteCommentsBetweenRange(node.ConditionExpression.SourceRange, node.SourceRangeCloseParenth);

            Write(")");

            canWriteComments = node.SourceRangesAvailable && node.Code.SourceRangesAvailable;

            if (canWriteComments)
            {
                if (node.Code.IsSingleStatementScope)
                {
                    _indentLevel++;

                    _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeCloseParenth,
                        node.Code.SourceRange, 1);

                    _indentLevel--;
                }
                else
                {
                    _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeCloseParenth,
                        node.Code.SourceRange, 1);
                }
            }

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            return true;
        }


        private IEnumerable<LSLComment> GetComments(int sourceRangeStart, int sourceRangeEnd)
        {
            var first = _comments.First;

            while (first != null)
            {
                var next = first.Next;
                var comment = first.Value;

                if (comment.SourceRange.StartIndex >= sourceRangeStart &&
                    comment.SourceRange.StopIndex <= sourceRangeEnd)
                {
                    yield return comment;

                    _comments.Remove(first);
                }
                first = next;
            }
        }


        public override bool VisitCompilationUnit(ILSLCompilationUnitNode node)
        {
            var nodes = node.GlobalVariableDeclarations.Concat<ILSLReadOnlySyntaxTreeNode>(node.FunctionDeclarations)
                .Concat(new[] {node.DefaultStateNode})
                .Concat(node.StateDeclarations).ToList();

            if (nodes.All(x => x.SourceRangesAvailable))
            {
                nodes.Sort((x,y)=>x.SourceRange.StartIndex.CompareTo(y.SourceRange.StartIndex));
            }

            if (nodes.Count > 0)
            {
                CompilationUnit_WriteCommentsAtTopOfSource_WithNodes(nodes, node);


                for (var nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
                {
                    var nodeAheadIndex = nodeIndex + 1;

                    var curNode = nodes[nodeIndex];

                    Visit(curNode);


                    if (nodeAheadIndex < nodes.Count)
                    {
                        var nextNode = nodes[nodeAheadIndex];

                        CompilationUnit_BetweenTopLevelNodes(curNode, nextNode);
                    }
                    else
                    {
                        CompilationUnit_AfterLastNode(node, curNode);
                    }
                }
            }
            else
            {
                CompilationUnit_NoTreeNodes(node);
            }

            return true;
        }


        private void CompilationUnit_NoTreeNodes(ILSLCompilationUnitNode node)
        {
            if (!node.SourceRangesAvailable) return;

            IList<LSLComment> comments = GetComments(node.SourceRange.StartIndex, node.SourceRange.StopIndex).ToList();

            if (comments.Count > 0)
            {
                CompilationUnit_NoTreeNodes_WithComments(node, comments);
            }
            else
            {
                CompilationUnit_NoTreeNodes_WithoutComments(node);
            }
        }


        private void CompilationUnit_AfterLastNode(ILSLCompilationUnitNode node, ILSLReadOnlySyntaxTreeNode curNode)
        {
            if (!node.SourceRangesAvailable || !curNode.SourceRangesAvailable)
            {
                return;
            }

            IList<LSLComment> comments =
                GetComments(curNode.SourceRange.StopIndex, int.MaxValue).ToList();

            if (comments.Count > 0)
            {
                CompilationUnit_AfterLastNode_WithComments(curNode, comments, node);
            }
            else
            {
                CompilationUnit_AfterLastNode_NoComments(curNode, node);
            }
        }


        private void CompilationUnit_BetweenTopLevelNodes(ILSLReadOnlySyntaxTreeNode node, ILSLReadOnlySyntaxTreeNode nextNode)
        {
            if (!node.SourceRangesAvailable || !nextNode.SourceRangesAvailable)
            {

                if (TopLevelCompilationUnitNodesAreDistinct(node, nextNode))
                {
                    Write(LSLFormatTools.CreateNewLinesString(Settings.MinimumNewLinesBetweenDistinctGlobalStatements));
                }
                else
                {
                    Write("\n");
                }

                return;
            }

            IList<LSLComment> comments = GetComments(node.SourceRange.StopIndex, nextNode.SourceRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                CompilationUnit_BetweenTopLevelNodes_WithCommentsBetween(node, comments, nextNode);
            }
            else
            {
                CompilationUnit_BetweenTopLevelNodes_NoCommentsBetween(node, nextNode);
            }
        }


        private void CompilationUnit_NoTreeNodes_WithoutComments(ILSLCompilationUnitNode node)
        {
            var linesBetweenNodeAndEndOfScope = (node.SourceRange.LineEnd - node.SourceRange.LineStart);

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndEndOfScope - 1));
        }


        private void CompilationUnit_NoTreeNodes_WithComments(ILSLCompilationUnitNode node, IList<LSLComment> comments)
        {
            var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                   node.SourceRange.LineStart);

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment - 1));

            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];

                Write(FormatComment("", comment));

                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];

                    var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndEndOfScope = (node.SourceRange.LineEnd -
                                                            comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndEndOfScope));
                }
            }
        }


        private void CompilationUnit_AfterLastNode_NoComments(ILSLReadOnlySyntaxTreeNode curNode,
            ILSLCompilationUnitNode node)
        {

            var linesBetweenNodeAndEndOfScope = (node.SourceRange.LineEnd -
                                                 curNode.SourceRange.LineEnd);

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndEndOfScope));
        }


        private void CompilationUnit_AfterLastNode_WithComments(ILSLReadOnlySyntaxTreeNode curNode,
            IList<LSLComment> comments, ILSLCompilationUnitNode node)
        { 
            var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                   curNode.SourceRange.LineEnd);


            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));


            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];

                if (comment.SourceRange.LineStart != curNode.SourceRange.LineEnd)
                {
                    Write(FormatComment("", comment));
                }
                else
                {
                    var space = _sourceReference.Substring(curNode.SourceRange.StopIndex + 1,
                        (comment.SourceRange.StartIndex - curNode.SourceRange.StopIndex) - 1);

                    Write(space + comment.Text);
                }

                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];

                    var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndEndOfScope = (node.SourceRange.LineEnd -
                                                            comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndEndOfScope));
                }
            }
        }


        private void CompilationUnit_BetweenTopLevelNodes_WithCommentsBetween(
            ILSLReadOnlySyntaxTreeNode node,
            IList<LSLComment> comments,
            ILSLReadOnlySyntaxTreeNode nextNode)
        { 

            var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                    node.SourceRange.LineEnd);

            if (linesBetweenNodeAndFirstComment < Settings.MinimumNewLinesBetweenGlobalStatementAndNextComment)
            {
                if ((nextNode is ILSLFunctionDeclarationNode || nextNode is ILSLStateScopeNode)
                    && (node is ILSLFunctionDeclarationNode || node is ILSLVariableDeclarationNode))
                {
                    linesBetweenNodeAndFirstComment = Settings.MinimumNewLinesBetweenGlobalStatementAndNextComment;
                }
            }

            if (linesBetweenNodeAndFirstComment > Settings.MaximumNewLinesBetweenGlobalStatementAndNextComment)
            {
                linesBetweenNodeAndFirstComment = Settings.MaximumNewLinesBetweenGlobalStatementAndNextComment;
            }


            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));

            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];

                if (comment.SourceRange.LineStart != node.SourceRange.LineEnd)
                {
                    Write(FormatComment("", comment));
                }
                else
                {
                    var space = _sourceReference.Substring(node.SourceRange.StopIndex + 1,
                        (comment.SourceRange.StartIndex - node.SourceRange.StopIndex) - 1);

                    Write(space + comment.Text);
                }


                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];
                    var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndNextNode = (nextNode.SourceRange.LineStart -
                                                          comment.SourceRange.LineEnd);


                    linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                       node.SourceRange.LineEnd);

                    if (linesBetweenNodeAndFirstComment == 0 &&
                        linesBetweenCommentAndNextNode < Settings.MinimumNewLinesBetweenDistinctGlobalStatements)
                    {
                        if ((nextNode is ILSLFunctionDeclarationNode || nextNode is ILSLStateScopeNode) &&
                            (node is ILSLFunctionDeclarationNode || node is ILSLVariableDeclarationNode))
                        {
                            linesBetweenCommentAndNextNode = Settings.MinimumNewLinesBetweenDistinctGlobalStatements;
                        }
                    }

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode));
                }
            }
        }


        private void CompilationUnit_BetweenTopLevelNodes_NoCommentsBetween(ILSLReadOnlySyntaxTreeNode node,
            ILSLReadOnlySyntaxTreeNode nextNode)
        {
            var linesBetweenTwoNodes = (nextNode.SourceRange.LineStart - node.SourceRange.LineEnd);

            if (TopLevelCompilationUnitNodesAreDistinct(node, nextNode))
            {
                if (linesBetweenTwoNodes < Settings.MinimumNewLinesBetweenDistinctGlobalStatements)
                {
                    linesBetweenTwoNodes = Settings.MinimumNewLinesBetweenDistinctGlobalStatements;
                }
            }

            if (linesBetweenTwoNodes > Settings.MaximumNewLinesBetweenGlobalStatements)
            {
                linesBetweenTwoNodes = Settings.MaximumNewLinesBetweenGlobalStatements;
            }

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes));
        }


        private static bool TopLevelCompilationUnitNodesAreDistinct(ILSLReadOnlySyntaxTreeNode node, ILSLReadOnlySyntaxTreeNode nextNode)
        {
            return (nextNode is ILSLFunctionDeclarationNode && node is ILSLFunctionDeclarationNode) ||
                   (nextNode is ILSLFunctionDeclarationNode && node is ILSLVariableDeclarationNode) ||
                   (nextNode is ILSLVariableDeclarationNode && node is ILSLFunctionDeclarationNode) ||
                   (nextNode is ILSLStateScopeNode && node is ILSLFunctionDeclarationNode) ||
                   (nextNode is ILSLStateScopeNode && node is ILSLVariableDeclarationNode) ||
                   (nextNode is ILSLStateScopeNode && node is ILSLStateScopeNode);
        }


        private void CompilationUnit_WriteCommentsAtTopOfSource_WithNodes(
            IList<ILSLReadOnlySyntaxTreeNode> nodes,
            ILSLCompilationUnitNode unitNode)
        {

            if(!unitNode.SourceRangesAvailable || !nodes[0].SourceRangesAvailable) return;

            var comments = GetComments(0, nodes[0].SourceRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                       unitNode.SourceRange.LineStart);




                Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment - 1));


                for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
                {
                    var commentAheadIndex = commentIndex + 1;

                    var comment = comments[commentIndex];

                    Write(FormatComment("", comment));

                    if (commentAheadIndex < comments.Count)
                    {
                        var nextComment = comments[commentAheadIndex];

                        var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                    comment.SourceRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = (nodes[0].SourceRange.LineStart -
                                                              comment.SourceRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode));
                    }
                }
            }
            else
            {
                var linesBetweenTwoNodes = (nodes[0].SourceRange.LineStart - unitNode.SourceRange.LineStart);

                Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes - 1));
            }
        }


        public override bool VisitEventHandler(ILSLEventHandlerNode node)
        {
            Write(node.Name + "(");
            Visit(node.ParameterList);
            Write(")");


            if (node.Code.SourceRangesAvailable && node.ParameterList.SourceRangesAvailable)
            { 

                var comments =
                GetComments(node.ParameterList.SourceRange.StopIndex,
                    node.Code.SourceRange.StartIndex).ToList();

                if (comments.Count > 0)
                {
                    _wroteCommentAfterEventParameterList = true;

                    int linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                           node.ParameterList.SourceRange.LineEnd);


                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));


                    for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
                    {
                        var commentAheadIndex = commentIndex + 1;

                        var comment = comments[commentIndex];

                        if (comment.SourceRange.LineStart != node.ParameterList.SourceRange.LineEnd)
                        {
                            Write(GenIndent() + comment.Text);
                        }
                        else
                        {
                            Write(" " + comment.Text);
                        }

                        if (commentAheadIndex < comments.Count)
                        {
                            var nextComment = comments[commentAheadIndex];

                            var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                        comment.SourceRange.LineEnd);

                            Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                        }
                        else
                        {
                            var linesBetweenCommentAndNextNode = (node.Code.SourceRange.LineStart -
                                                                  comment.SourceRange.LineEnd);

                            Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode - 1));
                        }
                    }
                }
            }

            Visit(node.Code);

            return true;
        }


        public override bool VisitFunctionDeclaration(ILSLFunctionDeclarationNode node)
        {
            if (node.ReturnType != LSLType.Void)
            {
                Write(node.ReturnTypeName + " " + node.Name + "(");
            }
            else
            {
                Write(node.Name + "(");
            }

            Visit(node.ParameterList);
            Write(")");


            if (node.Code.SourceRangesAvailable && node.ParameterList.SourceRangesAvailable)
            {
                var comments =
                    GetComments(node.ParameterList.SourceRange.StopIndex,
                        node.Code.SourceRange.StartIndex).ToList();

                if (comments.Count > 0)
                {
                    _wroteCommentAfterFunctionDeclarationParameterList = true;

                    var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                           node.ParameterList.SourceRange.LineEnd);


                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));


                    for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
                    {
                        var commentAheadIndex = commentIndex + 1;

                        var comment = comments[commentIndex];

                        if (comment.SourceRange.LineStart != node.ParameterList.SourceRange.LineEnd)
                        {
                            Write(GenIndent() + comment.Text);
                        }
                        else
                        {
                            Write(" " + comment.Text);
                        }

                        if (commentAheadIndex < comments.Count)
                        {
                            var nextComment = comments[commentAheadIndex];

                            var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                        comment.SourceRange.LineEnd);

                            Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                        }
                        else
                        {
                            var linesBetweenCommentAndNextNode = (node.Code.SourceRange.LineStart -
                                                                  comment.SourceRange.LineEnd);

                            Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode - 1));
                        }
                    }
                }
            }


            Visit(node.Code);

            return true;
        }


        public override bool VisitState(ILSLStateScopeNode stateNode)
        {
            if (stateNode.IsDefaultState)
            {
                Write("default");
            }
            else
            {
                Write("state");

                if (!stateNode.SourceRangesAvailable || 
                    !WriteCommentsBetweenRange(stateNode.SourceRangeStateKeyword, stateNode.SourceRangeStateName))
                {
                    Write(" ");
                }

                Write(stateNode.StateName);
            }

            var wroteCommentBetweenStateNameAndOpenBrace = false;

            if(stateNode.SourceRangesAvailable)
                wroteCommentBetweenStateNameAndOpenBrace = 
                    WriteCommentsBetweenRange(stateNode.SourceRangeStateName, stateNode.SourceRangeOpenBrace);


            string spaceBeforeOpeningBrace = "";

            if (wroteCommentBetweenStateNameAndOpenBrace)
            {
                if (Settings.AddSpacesBeforeOpeningStateBraceAfterCommentBreak)
                {
                    spaceBeforeOpeningBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningStateBrace);
                }
            }
            else
            {
                spaceBeforeOpeningBrace =
                    LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningStateBrace);
            }


            if (Settings.StateBracesOnNewLine)
            {
                Write("\n" + spaceBeforeOpeningBrace + "{\n");
            }
            else
            {
                Write(spaceBeforeOpeningBrace + "{\n");
            }

            _indentLevel++;

            var nodes = stateNode.EventHandlers;

            var indent = GenIndent();

            IList<LSLComment> comments;

            if (stateNode.SourceRangesAvailable && nodes[0].SourceRangesAvailable)
            {
                comments =
                    GetComments(stateNode.SourceRangeOpenBrace.StartIndex, nodes[0].SourceRange.StartIndex).ToList();

                if (comments.Count > 0)
                {
                    State_CommentsBeforeFirstNode(stateNode, comments, nodes[0]);
                }
                else
                {
                    State_NoCommentsBeforeFirstNode(stateNode, nodes);
                }
            }


            for (var nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
            {
                var nodeAheadIndex = nodeIndex + 1;

                var node = nodes[nodeIndex];

                var singleLineBroken = 0;

                Write(indent);
                Visit(node);

                if (nodeAheadIndex < nodes.Count)
                {
                    var nextNode = nodes[nodeAheadIndex];


                    if (!node.SourceRangesAvailable || !nextNode.SourceRangesAvailable)
                    {
                        Write(LSLFormatTools.CreateNewLinesString(Settings.MinimumNewLinesBetweenEventHandlers));
                        continue;
                    }

                    if (node.SourceRange.LineStart == nextNode.SourceRange.LineStart)
                    {
                        singleLineBroken = 1;
                        Write("\n");
                    }

                    comments =
                        GetComments(node.SourceRange.StopIndex, nextNode.SourceRange.StartIndex).ToList();

                    if (comments.Count > 0)
                    {
                        State_WriteCommentsBetweenNodes(comments, node, nextNode);
                    }
                    else
                    {
                        State_WriteNoCommentsBetweenNodes(nextNode, node, singleLineBroken);
                    }
                }
                else
                {
                    if (!node.SourceRangesAvailable)
                    {
                        Write("\n");
                        continue;
                    }

                    comments = GetComments(node.SourceRange.StopIndex, stateNode.SourceRange.StopIndex).ToList();

                    if (comments.Count > 0)
                    {
                        State_WriteCommentsAfterLastNode(stateNode, comments, node);
                    }
                    else
                    {
                        State_WriteNoCommentsAfterLastNode(stateNode, node);
                    }
                }
            }


            _indentLevel--;

            Write(LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingStateBrace) + "}");

            return true;
        }


        private void State_WriteNoCommentsAfterLastNode(ILSLStateScopeNode stateNode, ILSLEventHandlerNode node)
        {
            var linesBetweenNodeAndEndOfScope = (stateNode.SourceRangeCloseBrace.LineEnd -
                                                 node.SourceRange.LineEnd);


            linesBetweenNodeAndEndOfScope = linesBetweenNodeAndEndOfScope >
                                            Settings.MaximumNewLinesAtEndOfStateScope
                ? Settings.MaximumNewLinesAtEndOfStateScope
                : linesBetweenNodeAndEndOfScope;

            if (linesBetweenNodeAndEndOfScope == 0)
            {
                linesBetweenNodeAndEndOfScope = 1;
            }

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndEndOfScope));
        }


        private void State_WriteCommentsAfterLastNode(ILSLStateScopeNode stateNode, IList<LSLComment> comments, ILSLEventHandlerNode node)
        {
            string indent = GenIndent();

            var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                   node.SourceRange.LineEnd);

            if (linesBetweenNodeAndFirstComment == 0) linesBetweenNodeAndFirstComment = 1;


            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));


            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];

                Write(FormatComment(indent, comment));

                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];

                    var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndEndOfScope = (stateNode.SourceRangeCloseBrace.LineEnd -
                                                            comment.SourceRange.LineEnd);

                    linesBetweenCommentAndEndOfScope =
                        linesBetweenCommentAndEndOfScope > Settings.MaximumNewLinesAtEndOfStateScope
                            ? Settings.MaximumNewLinesAtEndOfStateScope
                            : linesBetweenCommentAndEndOfScope;

                    if (linesBetweenCommentAndEndOfScope == 0)
                    {
                        linesBetweenCommentAndEndOfScope = 1;
                    }


                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndEndOfScope));
                }
            }
        }


        private void State_WriteNoCommentsBetweenNodes(ILSLEventHandlerNode nextNode, ILSLEventHandlerNode node, int singleLineBroken)
        {
            var linesBetweenTwoNodes = (nextNode.SourceRange.LineStart - node.SourceRange.LineEnd);

            if (linesBetweenTwoNodes < Settings.MinimumNewLinesBetweenEventHandlers)
            {
                linesBetweenTwoNodes = (Settings.MinimumNewLinesBetweenEventHandlers - singleLineBroken);
            }


            Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes));
        }


        private void State_WriteCommentsBetweenNodes(IList<LSLComment> comments, ILSLEventHandlerNode node, ILSLEventHandlerNode nextNode)
        {
            string indent = GenIndent();

            var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                   node.SourceRange.LineEnd);

            if (linesBetweenNodeAndFirstComment < Settings.MinimumNewLinesBetweenEventHandlers)
            {
                linesBetweenNodeAndFirstComment = Settings.MinimumNewLinesBetweenEventHandlers;
            }

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));


            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];

                Write(FormatComment(indent, comment));

                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];

                    var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndNextNode = (nextNode.SourceRange.LineStart -
                                                          comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode));
                }
            }
        }


        private void State_NoCommentsBeforeFirstNode(ILSLStateScopeNode stateNode, IReadOnlyGenericArray<ILSLEventHandlerNode> nodes)
        {
            var linesBetweenTwoNodes = (nodes[0].SourceRange.LineStart -
                                        stateNode.SourceRangeOpenBrace.LineStart);

            linesBetweenTwoNodes = linesBetweenTwoNodes > Settings.MaximumNewLinesAtBeginingOfStateScope
                ? Settings.MaximumNewLinesAtBeginingOfStateScope
                : linesBetweenTwoNodes;

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes - 1));
        }


        private void State_CommentsBeforeFirstNode(ILSLStateScopeNode stateNode, IList<LSLComment> comments, ILSLEventHandlerNode firstNode)
        {
            string indent = GenIndent();

            var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                   stateNode.SourceRangeOpenBrace.LineStart);

            linesBetweenNodeAndFirstComment = linesBetweenNodeAndFirstComment >
                                              Settings.MaximumNewLinesAtBeginingOfStateScope
                ? Settings.MaximumNewLinesAtBeginingOfStateScope
                : linesBetweenNodeAndFirstComment;

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment - 1));

            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];

                Write(FormatComment(indent, comment));

                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];

                    var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndNextNode = (firstNode.SourceRange.LineStart -
                                                          comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode));
                }
            }
        }


        public override bool VisitParameterDefinition(ILSLParameterNode node)
        {
            Write(node.TypeName + " " + node.Name);

            return true;
        }


        public override bool VisitParameterDefinitionList(ILSLParameterListNode node)
        {
            if (!node.HasParameters)
            {
                return true;
            }

            var parameterCount = node.Parameters.Count;


            for (var parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
            {
                Visit(node.Parameters[parameterIndex]);

                if (parameterIndex != (parameterCount - 1))
                {
                    Write(", ");
                }
            }

            return true;
        }


        public override bool VisitReturnStatement(ILSLReturnStatementNode node)
        {
            if (node.HasReturnExpression)
            {
                Write("return");

                bool cannotWriteComments = !node.SourceRangesAvailable || !node.ReturnExpression.SourceRangesAvailable;

                if (cannotWriteComments ||
                    !WriteCommentsBetweenRange(node.SourceRangeReturnKeyword, node.ReturnExpression.SourceRange))
                {
                    Write(" ");
                }

                var wrappingContext = new ExpressionWrappingContext(node, this)
                {
                    ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeReturnExpressionWrap,
                    MinimumExpressionsToWrap = Settings.MinimumExpressionsInReturnToWrap
                };

                ExpressionWrappingPush(Settings.ReturnExpressionWrapping, wrappingContext);
                Visit(node.ReturnExpression);
                ExpressionWrappingPop();

                if(!cannotWriteComments)
                    WriteCommentsBetweenRange(node.ReturnExpression.SourceRange, node.SourceRangeSemicolon);

                Write(";");
            }
            else
            {
                Write("return");

                if(node.SourceRangesAvailable)
                    WriteCommentsBetweenRange(node.SourceRangeReturnKeyword, node.SourceRangeSemicolon);

                Write(";");
            }
            return true;
        }


        public override bool VisitSemicolonStatement(ILSLSemicolonStatement node)
        {
            Write(";");

            return true;
        }


        public override bool VisitStateChangeStatement(ILSLStateChangeStatementNode node)
        {
            Write("state");

            if (!node.SourceRangesAvailable || 
                !WriteCommentsBetweenRange(node.SourceRangeStateKeyword, node.SourceRangeStateName))
            {
                Write(" ");
            }

            Write(node.StateTargetName);

            if(node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeStateName, node.SourceRangeSemicolon);

            Write(";");

            return true;
        }


        public override bool VisitJumpStatement(ILSLJumpStatementNode node)
        {
            Write("jump");

            if (!node.SourceRangesAvailable || 
                !WriteCommentsBetweenRange(node.SourceRangeJumpKeyword, node.SourceRangeLabelName))
            {
                Write(" ");
            }

            Write(node.LabelName);

            if(node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeLabelName, node.SourceRangeSemicolon);

            Write(";");

            return true;
        }


        public override bool VisitLabelStatement(ILSLLabelStatementNode node)
        {
            Write("@");

            if(node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeLabelPrefix, node.SourceRangeLabelName);

            Write(node.LabelName);


            if (node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeLabelName, node.SourceRangeSemicolon);

            Write(";");

            return true;
        }


        public override bool VisitControlStatement(ILSLControlStatementNode snode)
        {
            IEnumerable<ILSLReadOnlySyntaxTreeNode> chainNodes = (new ILSLReadOnlySyntaxTreeNode[] {snode.IfStatement});

            if (snode.HasElseIfStatements)
            {
                chainNodes = chainNodes.Concat(snode.ElseIfStatement);
            }

            if (snode.HasElseStatement)
            {
                chainNodes = chainNodes.Concat(new ILSLReadOnlySyntaxTreeNode[] {snode.ElseStatement}).ToList();
            }


            var chainNodeList = chainNodes.ToList();


            GenIndent();

            for (var nodeIndex = 0; nodeIndex < chainNodeList.Count; nodeIndex++)
            {
                var nodeAheadIndex = nodeIndex + 1;

                var node = chainNodeList[nodeIndex];


                Visit(node);

                if (nodeAheadIndex >= chainNodeList.Count) continue;

                var nextNode = chainNodeList[nodeAheadIndex];

                if(node.SourceRangesAvailable && nextNode.SourceRangesAvailable)
                    _wroteCommentAfterControlChainMember = WriteCommentsBetweenRange(node.SourceRange, nextNode.SourceRange, 1);
            }

            _wroteCommentAfterControlChainMember = false;

            return true;
        }


        public override bool VisitIfStatement(ILSLIfStatementNode node)
        {
            Write("if");

            if(node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeIfKeyword, node.SourceRangeOpenParenth);

            Write("(");

            bool canWriteComments = node.SourceRangesAvailable && node.ConditionExpression.SourceRangesAvailable;

            if (canWriteComments)
                WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.ConditionExpression.SourceRange);


            var wrappingContext = new ExpressionWrappingContext(node, this)
            {
                ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeIfExpressionWrap,
                MinimumExpressionsToWrap = Settings.MinimumExpressionsInIfToWrap
            };

            ExpressionWrappingPush(Settings.IfExpressionWrapping, wrappingContext);
            Visit(node.ConditionExpression);
            ExpressionWrappingPop();

            if(canWriteComments)
                WriteCommentsBetweenRange(node.ConditionExpression.SourceRange, node.SourceRangeCloseParenth);

            Write(")");


            canWriteComments = node.SourceRangesAvailable && node.Code.SourceRangesAvailable;

            if (canWriteComments)
            {
                if (node.Code.IsSingleStatementScope)
                {
                    _indentLevel++;

                    _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeCloseParenth,
                        node.Code.SourceRange, 1);

                    _indentLevel--;
                }
                else
                {
                    _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeCloseParenth,
                        node.Code.SourceRange, 1);
                }
            }

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            return true;
        }


        public override bool VisitElseIfStatement(ILSLElseIfStatementNode node)
        {
            if (Settings.ElseIfStatementOnNewLine || _wroteCommentAfterControlChainMember)
            {
                Write("\n" + GenIndent() + "else");
            }
            else
            {
                Write(LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeUnbrokenElseIfStatement) + "else");
            }


            if (!node.SourceRangesAvailable || 
                !WriteCommentsBetweenRange(node.SourceRangeElseKeyword, node.SourceRangeIfKeyword))
            {
                Write(" ");
            }

            Write("if");

            if(node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.SourceRangeIfKeyword, node.SourceRangeOpenParenth);

            Write("(");


            bool canWriteComments = node.SourceRangesAvailable && node.ConditionExpression.SourceRangesAvailable;

            if(canWriteComments)
                WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.ConditionExpression.SourceRange);


            var wrappingContext = new ExpressionWrappingContext(node, this)
            {
                ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeElseIfExpressionWrap,
                MinimumExpressionsToWrap = Settings.MinimumExpressionsInElseIfToWrap
            };


            ExpressionWrappingPush(Settings.ElseIfExpressionWrapping, wrappingContext);
            Visit(node.ConditionExpression);
            ExpressionWrappingPop();

            if(canWriteComments)
                WriteCommentsBetweenRange(node.ConditionExpression.SourceRange, node.SourceRangeCloseParenth);

            Write(")");


            canWriteComments = node.SourceRangesAvailable && node.Code.SourceRangesAvailable;


            if (canWriteComments)
            {
                if (node.Code.IsSingleStatementScope)
                {
                    _indentLevel++;

                    _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeCloseParenth,
                        node.Code.SourceRange, 1);

                    _indentLevel--;
                }
                else
                {
                    _wroteCommentBeforeControlStatementCode =
                        WriteCommentsBetweenRange(node.SourceRangeCloseParenth, node.Code.SourceRange, 1);
                }
            }

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            return true;
        }


        public override bool VisitElseStatement(ILSLElseStatementNode node)
        {
            if (Settings.ElseStatementOnNewLine || _wroteCommentAfterControlChainMember)
            {
                Write("\n" + GenIndent() + "else");
            }
            else
            {
                Write(LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeUnbrokenElseStatement) + "else");
            }

            if (node.SourceRangesAvailable && node.Code.SourceRangesAvailable)
            {
                if (node.Code.IsSingleStatementScope)
                {
                    _indentLevel++;

                    _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeElseKeyword,
                        node.Code.SourceRange, 1);

                    _indentLevel--;
                }
                else
                {
                    _wroteCommentBeforeControlStatementCode =
                        WriteCommentsBetweenRange(node.SourceRangeElseKeyword, node.Code.SourceRange, 1);
                }
            }

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            return true;
        }


        public override bool VisitExpressionStatement(ILSLExpressionStatementNode node)
        {
            Visit(node.Expression);


            if(node.Expression.SourceRangesAvailable && node.SourceRangesAvailable)
                WriteCommentsBetweenRange(node.Expression.SourceRange, node.SourceRangeSemicolon);


            Write(";");

            return true;
        }


        public override bool VisitVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            Write(node.TypeName);

            if (!node.SourceRangesAvailable || 
                !WriteCommentsBetweenRange(node.SourceRangeType, node.SourceRangeName))
            {
                Write(" ");
            }

            Write(node.Name);

            if (node.HasDeclarationExpression)
            {
                if (!node.SourceRangesAvailable ||
                    !WriteCommentsBetweenRange(node.SourceRangeName, node.SourceRangeOperator))
                {
                    Write(" ");
                }

                Write("=");

                var wrappingContext = new ExpressionWrappingContext(node, this)
                {
                    ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeDeclarationExpressionWrap,
                    MinimumExpressionsToWrap = Settings.MinimumExpressionsInDeclarationToWrap
                };

                bool cannotWriteComments = !node.SourceRangesAvailable ||
                                           !node.DeclarationExpression.SourceRangesAvailable;

                if (cannotWriteComments ||
                    !WriteCommentsBetweenRange(node.SourceRangeOperator, node.DeclarationExpression.SourceRange))
                {
                    Write(" ");
                }


                ExpressionWrappingPush(Settings.DeclarationExpressionWrapping, wrappingContext);
                Visit(node.DeclarationExpression);
                ExpressionWrappingPop();

                if(!cannotWriteComments)
                    WriteCommentsBetweenRange(node.DeclarationExpression.SourceRange,
                        node.SourceRange.LastCharRange);

                Write(";");
            }
            else
            {
                if(node.SourceRangesAvailable)
                    WriteCommentsBetweenRange(node.SourceRangeName, node.SourceRange.LastCharRange);

                Write(";");
            }


            return true;
        }


        public override bool VisitSingleStatementCodeScope(ILSLCodeScopeNode node)
        {
            var statement = node.CodeStatements.First();

            if (statement is ILSLSemicolonStatement)
            {
                Visit(statement);
            }
            else
            {
                if (Settings.ConvertBracelessControlStatements)
                {
                    var newLine = false;
                    var spaceBeforeOpeningBrace = "";
                    var spaceBeforeClosingBrace = "";
                    if (node.CodeScopeType == LSLCodeScopeType.If)
                    {
                        newLine = Settings.IfStatementBracesOnNewLine;

                        spaceBeforeClosingBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingIfBrace);

                        if (_wroteCommentAfterControlChainMember)
                        {
                            if (Settings.AddSpacesBeforeOpeningIfBraceAfterCommentBreak)
                            {
                                spaceBeforeOpeningBrace =
                                    LSLFormatTools.CreateTabCorrectSpaceString(
                                        Settings.SpacesBeforeOpeningIfBrace);
                            }
                        }
                        else
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningIfBrace);
                        }
                    }
                    else if (node.CodeScopeType == LSLCodeScopeType.ElseIf)
                    {
                        newLine = Settings.ElseIfStatementBracesOnNewLine;

                        spaceBeforeClosingBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingElseIfBrace);

                        if (_wroteCommentAfterControlChainMember)
                        {
                            if (Settings.AddSpacesBeforeOpeningElseIfBraceAfterCommentBreak)
                            {
                                spaceBeforeOpeningBrace =
                                    LSLFormatTools.CreateTabCorrectSpaceString(
                                        Settings.SpacesBeforeOpeningElseIfBrace);
                            }
                        }
                        else
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningElseIfBrace);
                        }
                    }
                    else if (node.CodeScopeType == LSLCodeScopeType.Else)
                    {
                        newLine = Settings.ElseStatementBracesOnNewLine;

                        spaceBeforeClosingBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingElseBrace);

                        if (_wroteCommentAfterControlChainMember)
                        {
                            if (Settings.AddSpacesBeforeOpeningElseBraceAfterCommentBreak)
                            {
                                spaceBeforeOpeningBrace =
                                    LSLFormatTools.CreateTabCorrectSpaceString(
                                        Settings.SpacesBeforeOpeningElseBrace);
                            }
                        }
                        else
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningElseBrace);
                        }
                    }
                    else if (node.CodeScopeType == LSLCodeScopeType.ForLoop)
                    {
                        newLine = Settings.ForLoopBracesOnNewLine;

                        spaceBeforeClosingBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingForLoopBrace);

                        if (_wroteCommentAfterControlChainMember)
                        {
                            if (Settings.AddSpacesBeforeOpeningForLoopBraceAfterCommentBreak)
                            {
                                spaceBeforeOpeningBrace =
                                    LSLFormatTools.CreateTabCorrectSpaceString(
                                        Settings.SpacesBeforeOpeningForLoopBrace);
                            }
                        }
                        else
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningForLoopBrace);
                        }
                    }
                    else if (node.CodeScopeType == LSLCodeScopeType.DoLoop)
                    {
                        newLine = Settings.DoLoopBracesOnNewLine;

                        spaceBeforeClosingBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingDoLoopBrace);

                        if (_wroteCommentAfterControlChainMember)
                        {
                            if (Settings.AddSpacesBeforeOpeningDoLoopBraceAfterCommentBreak)
                            {
                                spaceBeforeOpeningBrace =
                                    LSLFormatTools.CreateTabCorrectSpaceString(
                                        Settings.SpacesBeforeOpeningDoLoopBrace);
                            }
                        }
                        else
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningDoLoopBrace);
                        }
                    }
                    else if (node.CodeScopeType == LSLCodeScopeType.WhileLoop)
                    {
                        newLine = Settings.WhileLoopBracesOnNewLine;

                        spaceBeforeClosingBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingWhileLoopBrace);

                        if (_wroteCommentAfterControlChainMember)
                        {
                            if (Settings.AddSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak)
                            {
                                spaceBeforeOpeningBrace =
                                    LSLFormatTools.CreateTabCorrectSpaceString(
                                        Settings.SpacesBeforeOpeningWhileLoopBrace);
                            }
                        }
                        else
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningWhileLoopBrace);
                        }
                    }

                    if (newLine || _wroteCommentBeforeControlStatementCode)
                    {
                        Write("\n" + GenIndent() + spaceBeforeOpeningBrace + "{");
                    }
                    else
                    {
                        Write(spaceBeforeOpeningBrace + "{");
                    }

                    _indentLevel++;
                    Write("\n" + GenIndent());
                    Visit(statement);
                    _indentLevel--;

                    Write("\n" + GenIndent() + spaceBeforeClosingBrace + "}");
                }
                else
                {
                    _indentLevel++;

                    if (Settings.IndentBracelessControlStatements || _wroteCommentBeforeControlStatementCode)
                    {
                        Write("\n" + GenIndent());
                        _wroteCommentBeforeControlStatementCode = false;
                    }
                    else
                    {
                        Write(" ");
                    }

                    Visit(statement);

                    _indentLevel--;
                }
            }

            return true;
        }


        private static string FormatComment(string indent, LSLComment comment)
        {
            if (comment.Type == LSLCommentType.SingleLine)
            {
                return indent + comment.Text;
            }


            var parts = comment.Text.Split('\n').ToList();

            if (parts.Count == 0)
            {
                return indent + comment.Text;
            }


            var firstLine = parts[0];

            firstLine = firstLine.Substring(2, firstLine.Length - 2);

            if (parts.Count == 1)
            {
                return indent + comment.Text;
            }


            var indentSpaces = indent.GetStringSpacesIndented();

            var indentString = LSLFormatTools.CreateTabCorrectSpaceString(indentSpaces == 0 ? 0 : indentSpaces - 1);

            firstLine = indentString + "/*" + firstLine + "\n";

            for (var partIndex = 1; partIndex < parts.Count; partIndex++)
            {
                var part = parts[partIndex];

                var userIndent = part.GetStringSpacesIndented();

                if (indentSpaces != userIndent || (indentSpaces == 0 && userIndent == 0))
                {
                    part = indentString + " " + part.Trim();
                }

                firstLine += part + (partIndex == parts.Count - 1 ? "" : "\n");
            }

            return firstLine;
        }


        public override bool VisitMultiStatementCodeScope(ILSLCodeScopeNode codeScopeNode)
        {
            var spaceBeforeClosingBrace = "";
            if (codeScopeNode.Parent is ILSLCodeScopeNode)
            {
                Write("{\n");
            }
            else
            {
                var newLine = false;
                var spaceBeforeOpeningBrace = "";

                if (codeScopeNode.CodeScopeType == LSLCodeScopeType.If)
                {
                    newLine = Settings.IfStatementBracesOnNewLine;

                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingIfBrace);

                    if (_wroteCommentAfterControlChainMember)
                    {
                        if (Settings.AddSpacesBeforeOpeningIfBraceAfterCommentBreak)
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(
                                    Settings.SpacesBeforeOpeningIfBrace);
                        }
                    }
                    else
                    {
                        spaceBeforeOpeningBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningIfBrace);
                    }
                }
                else if (codeScopeNode.CodeScopeType == LSLCodeScopeType.ElseIf)
                {
                    newLine = Settings.ElseIfStatementBracesOnNewLine;

                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingElseIfBrace);

                    if (_wroteCommentAfterControlChainMember)
                    {
                        if (Settings.AddSpacesBeforeOpeningElseIfBraceAfterCommentBreak)
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(
                                    Settings.SpacesBeforeOpeningElseIfBrace);
                        }
                    }
                    else
                    {
                        spaceBeforeOpeningBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningElseIfBrace);
                    }
                }
                else if (codeScopeNode.CodeScopeType == LSLCodeScopeType.Else)
                {
                    newLine = Settings.ElseStatementBracesOnNewLine;

                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingElseBrace);

                    if (_wroteCommentAfterControlChainMember)
                    {
                        if (Settings.AddSpacesBeforeOpeningElseBraceAfterCommentBreak)
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(
                                    Settings.SpacesBeforeOpeningElseBrace);
                        }
                    }
                    else
                    {
                        spaceBeforeOpeningBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningElseBrace);
                    }
                }
                else if (codeScopeNode.CodeScopeType == LSLCodeScopeType.ForLoop)
                {
                    newLine = Settings.ForLoopBracesOnNewLine;

                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingForLoopBrace);

                    if (_wroteCommentAfterControlChainMember)
                    {
                        if (Settings.AddSpacesBeforeOpeningForLoopBraceAfterCommentBreak)
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(
                                    Settings.SpacesBeforeOpeningForLoopBrace);
                        }
                    }
                    else
                    {
                        spaceBeforeOpeningBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningForLoopBrace);
                    }
                }
                else if (codeScopeNode.CodeScopeType == LSLCodeScopeType.DoLoop)
                {
                    newLine = Settings.DoLoopBracesOnNewLine;

                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingDoLoopBrace);

                    if (_wroteCommentAfterControlChainMember)
                    {
                        if (Settings.AddSpacesBeforeOpeningDoLoopBraceAfterCommentBreak)
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(
                                    Settings.SpacesBeforeOpeningDoLoopBrace);
                        }
                    }
                    else
                    {
                        spaceBeforeOpeningBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningDoLoopBrace);
                    }
                }
                else if (codeScopeNode.CodeScopeType == LSLCodeScopeType.WhileLoop)
                {
                    newLine = Settings.WhileLoopBracesOnNewLine;

                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingWhileLoopBrace);

                    if (_wroteCommentAfterControlChainMember)
                    {
                        if (Settings.AddSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak)
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(
                                    Settings.SpacesBeforeOpeningWhileLoopBrace);
                        }
                    }
                    else
                    {
                        spaceBeforeOpeningBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningWhileLoopBrace);
                    }
                }
                else if (codeScopeNode.CodeScopeType == LSLCodeScopeType.EventHandler)
                {
                    newLine = Settings.EventBracesOnNewLine;

                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingEventBrace);


                    if (_wroteCommentAfterEventParameterList)
                    {
                        if (Settings.AddSpacesBeforeOpeningEventBraceAfterCommentBreak)
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(
                                    Settings.SpacesBeforeOpeningEventBrace);
                        }
                    }
                    else
                    {
                        spaceBeforeOpeningBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningEventBrace);
                    }
                }
                else if (codeScopeNode.CodeScopeType == LSLCodeScopeType.Function)
                {
                    newLine = Settings.FunctionBracesOnNewLine;

                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingFunctionBrace);

                    if (_wroteCommentAfterEventParameterList)
                    {
                        if (Settings.AddSpacesBeforeOpeningFunctionBraceAfterCommentBreak)
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(
                                    Settings.SpacesBeforeOpeningFunctionBrace);
                        }
                    }
                    else
                    {
                        spaceBeforeOpeningBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningFunctionBrace);
                    }
                }

                if (newLine ||
                    _wroteCommentBeforeControlStatementCode ||
                    _wroteCommentAfterEventParameterList ||
                    _wroteCommentAfterFunctionDeclarationParameterList)
                {
                    Write("\n" + GenIndent() + spaceBeforeOpeningBrace + "{\n");
                    _wroteCommentAfterEventParameterList = false;
                    _wroteCommentAfterFunctionDeclarationParameterList = false;
                }
                else
                {
                    Write(spaceBeforeOpeningBrace + "{\n");
                }
            }

            _indentLevel++;

            var nodes = codeScopeNode.CodeStatements.ToList();

            var indent = GenIndent();

            if (nodes.Count > 0)
            {
                IList<LSLComment> comments;

                if (codeScopeNode.SourceRangesAvailable && nodes[0].SourceRangesAvailable)
                {
                    comments =
                        GetComments(codeScopeNode.SourceRange.StartIndex, nodes[0].SourceRange.StartIndex).ToList();

                    if (comments.Count > 0)
                    {
                        MultiStatement_WriteCommentsBeforeFirstNode(comments, nodes[0], codeScopeNode);
                    }
                    else
                    {
                        MultiStatement_WriteNoCommentsBeforeFirstNode(nodes[0], codeScopeNode);
                    }
                }

                /////////////////////
                /////////////////////
                /////////////////////                 

                for (var nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
                {
                    var nodeAheadIndex = nodeIndex + 1;
                    var node = nodes[nodeIndex];

                    var singleLineBroken = 0;


                    Write(indent);
                    Visit(node);

                    if (nodeAheadIndex < nodes.Count)
                    {
                        var nextNode = nodes[nodeAheadIndex];

                        if (!node.SourceRangesAvailable || !nextNode.SourceRangesAvailable)
                        {
                            if (TwoCodeStatementsAreDistinct(node, nextNode))
                            {
                                Write(
                                    LSLFormatTools.CreateNewLinesString(
                                        Settings.MinimumNewLinesBetweenDistinctLocalStatements));
                            }
                            else
                            {
                                Write("\n");
                            }

                            continue;
                        }

                        if (node.SourceRange.LineEnd == nextNode.SourceRange.LineStart)
                        {
                            singleLineBroken = 1;
                        }

                        comments =
                            GetComments(node.SourceRange.StopIndex, nextNode.SourceRange.StartIndex)
                                .ToGenericArray();

                        if (comments.Count > 0)
                        {
                            MultiStatement_WriteCommentsBetweenTwoNodes(singleLineBroken, node, comments, nextNode);
                        }
                        else //no comments
                        {
                            MultiStatement_WriteNoCommentsBetweenTwoNodes(singleLineBroken, node, nextNode);
                        }
                    }
                    else // last node in scope
                    {

                        if (node.SourceRangesAvailable && codeScopeNode.SourceRangesAvailable)
                        {
                            MultiStatement_WriteCommentsAfterLastNode(node, codeScopeNode);
                        }
                        else
                        {
                            Write("\n");
                        }
                    }
                }
            }
            else // no nodes
            {
                if (codeScopeNode.SourceRangesAvailable)
                {
                    MultiStatement_WriteCommentsNoNodesInScope(codeScopeNode);
                }
                else
                {
                    Write("\n");
                }
            }


            _indentLevel--;

            Write(GenIndent() + spaceBeforeClosingBrace + "}");

            return true;
        }


        private void MultiStatement_WriteCommentsBetweenTwoNodes(
            int singleLineBroken,
            ILSLReadOnlyCodeStatement node,
            IList<LSLComment> comments,
            ILSLReadOnlySyntaxTreeNode nextNode)
        {
            var indent = GenIndent();

            var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                   node.SourceRange.LineEnd);


            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));


            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];

                if (comment.SourceRange.LineStart != node.SourceRange.LineEnd)
                {
                    Write(FormatComment(indent, comment));
                }
                else
                {
                    var space = _sourceReference.Substring(node.SourceRange.StopIndex + 1,
                        (comment.SourceRange.StartIndex - node.SourceRange.StopIndex) - 1);

                    Write(space + comment.Text);
                }

                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];

                    var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else //last comment
                {
                    var linesBetweenCommentAndNextNode = (nextNode.SourceRange.LineStart -
                                                          comment.SourceRange.LineEnd +
                                                          singleLineBroken);
                    var linesBetweenCommentAndLastNode = (node.SourceRange.LineEnd -
                                                          comment.SourceRange.LineStart);

                    if (linesBetweenCommentAndLastNode == 0 && linesBetweenCommentAndNextNode == 1 &&
                        node is ILSLControlStatementNode)
                    {
                        linesBetweenCommentAndNextNode++;
                    }

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode));
                }
            }
        }


        private void MultiStatement_WriteCommentsNoNodesInScope(ILSLCodeScopeNode statementNode)
        {
            if (!statementNode.SourceRangesAvailable) return;

            var indent = GenIndent();

            var comments = GetComments(statementNode.SourceRange.StartIndex, statementNode.SourceRange.StopIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                       statementNode.SourceRange.LineStart);

                linesBetweenNodeAndFirstComment = linesBetweenNodeAndFirstComment >
                                                  Settings.MaximumNewLinesAtBeginingOfCodeScope
                    ? Settings.MaximumNewLinesAtBeginingOfCodeScope
                    : linesBetweenNodeAndFirstComment;


                Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment - 1));

                for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
                {
                    var commentAheadIndex = commentIndex + 1;

                    var comment = comments[commentIndex];


                    Write(FormatComment(indent, comment));

                    if (commentAheadIndex < comments.Count)
                    {
                        var nextComment = comments[commentAheadIndex];

                        var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                    comment.SourceRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                    }
                    else
                    {
                        var linesBetweenCommentAndEndOfScope = (statementNode.SourceRange.LineEnd -
                                                                comment.SourceRange.LineEnd);

                        linesBetweenCommentAndEndOfScope = linesBetweenCommentAndEndOfScope >
                                                           Settings.MaximumNewLinesAtEndOfCodeScope
                            ? Settings.MaximumNewLinesAtEndOfCodeScope
                            : linesBetweenCommentAndEndOfScope;

                        if (linesBetweenCommentAndEndOfScope == 0)
                        {
                            linesBetweenCommentAndEndOfScope = 1;
                        }

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndEndOfScope));
                    }
                }
            }
            else
            {
                var linesBetweenNodeAndEndOfScope = (statementNode.SourceRange.LineEnd - statementNode.SourceRange.LineStart);

                linesBetweenNodeAndEndOfScope = linesBetweenNodeAndEndOfScope > Settings.MaximumNewLinesAtEndOfCodeScope
                    ? Settings.MaximumNewLinesAtEndOfCodeScope
                    : linesBetweenNodeAndEndOfScope;


                Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndEndOfScope - 1));
            }
        }


        private void MultiStatement_WriteNoCommentsBetweenTwoNodes(int singleLineBroken, ILSLReadOnlyCodeStatement node,
            ILSLReadOnlyCodeStatement nextNode)
        {
            var linesBetweenTwoNodes = ((nextNode.SourceRange.LineStart -
                                         node.SourceRange.LineEnd) + singleLineBroken);

            if (TwoCodeStatementsAreDistinct(node, nextNode))
            {
                if (linesBetweenTwoNodes < Settings.MinimumNewLinesBetweenDistinctLocalStatements )
                {
                    linesBetweenTwoNodes = Settings.MinimumNewLinesBetweenDistinctLocalStatements;
                }
            }

            if (linesBetweenTwoNodes > Settings.MaximumNewLinesBetweenLocalStatements)
            {
                linesBetweenTwoNodes = Settings.MaximumNewLinesBetweenLocalStatements;
            }

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes));
        }


        private static bool TwoCodeStatementsAreDistinct(ILSLReadOnlyCodeStatement node, ILSLReadOnlyCodeStatement nextNode)
        {
            return (nextNode is ILSLVariableDeclarationNode ||
                    nextNode is ILSLControlStatementNode ||
                    nextNode is ILSLExpressionStatementNode ||
                    nextNode is ILSLLoopNode ||
                    nextNode is ILSLReturnStatementNode ||
                    nextNode is ILSLStateChangeStatementNode ||
                    nextNode is ILSLJumpStatementNode ||
                    nextNode is ILSLLabelStatementNode ||
                    nextNode is ILSLCodeScopeNode)
                   &&
                   (node is ILSLVariableDeclarationNode ||
                    node is ILSLControlStatementNode ||
                    node is ILSLExpressionStatementNode ||
                    node is ILSLLoopNode ||
                    node is ILSLReturnStatementNode ||
                    node is ILSLStateChangeStatementNode ||
                    node is ILSLJumpStatementNode ||
                    node is ILSLLabelStatementNode ||
                    node is ILSLCodeScopeNode)
                   && !(
                       (nextNode is ILSLVariableDeclarationNode && node is ILSLVariableDeclarationNode) ||
                       (nextNode is ILSLExpressionStatementNode && node is ILSLExpressionStatementNode)
                       );
        }


        private void MultiStatement_WriteNoCommentsBeforeFirstNode(ILSLReadOnlyCodeStatement firstNode,
            ILSLCodeScopeNode snode)
        {
            var linesBetweenTwoNodes = (firstNode.SourceRange.LineStart - snode.SourceRange.LineStart);

            linesBetweenTwoNodes = linesBetweenTwoNodes > Settings.MaximumNewLinesAtBeginingOfCodeScope
                ? Settings.MaximumNewLinesAtBeginingOfCodeScope
                : linesBetweenTwoNodes;

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes - 1));
        }


        private void MultiStatement_WriteCommentsBeforeFirstNode(IList<LSLComment> commentsBetweenOpenBraceAndNode,
            ILSLReadOnlyCodeStatement firstNode, ILSLCodeScopeNode snode)
        {
            var comments = commentsBetweenOpenBraceAndNode;

            var indent = GenIndent();

            var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                   snode.SourceRange.LineStart);

            linesBetweenNodeAndFirstComment = linesBetweenNodeAndFirstComment >
                                              Settings.MaximumNewLinesAtBeginingOfCodeScope
                ? Settings.MaximumNewLinesAtBeginingOfCodeScope
                : linesBetweenNodeAndFirstComment;


            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment - 1));

            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];


                Write(FormatComment(indent, comment));

                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];

                    var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndNextNode = (firstNode.SourceRange.LineStart -
                                                          comment.SourceRange.LineEnd);

                    if (linesBetweenCommentAndNextNode == 0)
                    {
                        linesBetweenCommentAndNextNode = 1;
                    }


                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode));
                }
            }
        }


        private void MultiStatement_WriteCommentsAfterLastNode(ILSLReadOnlyCodeStatement node, ILSLCodeScopeNode snode)
        {
            if (!node.SourceRangesAvailable || !snode.SourceRangesAvailable) return;

            var comments = GetComments(node.SourceRange.StopIndex, snode.SourceRange.StopIndex).ToList();
            var indent = GenIndent();

            if (comments.Count > 0)
            {
                var linesBetweenLastNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                           node.SourceRange.LineEnd);


                Write(LSLFormatTools.CreateNewLinesString(linesBetweenLastNodeAndFirstComment));


                for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
                {
                    var commentAheadIndex = commentIndex + 1;

                    var comment = comments[commentIndex];

                    if (comment.SourceRange.LineStart != node.SourceRange.LineEnd)
                    {
                        Write(FormatComment(indent, comment));
                    }
                    else
                    {
                        var space = _sourceReference.Substring(node.SourceRange.StopIndex + 1,
                            (comment.SourceRange.StartIndex - node.SourceRange.StopIndex) - 1);

                        Write(space + comment.Text);
                    }

                    if (commentAheadIndex < comments.Count)
                    {
                        var nextComment = comments[commentAheadIndex];

                        var linesBetweenComments = (nextComment.SourceRange.LineStart -
                                                    comment.SourceRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                    }
                    else // last comment
                    {
                        var linesBetweenCommentAndEndOfScope = (snode.SourceRange.LineEnd -
                                                                comment.SourceRange.LineEnd);

                        linesBetweenCommentAndEndOfScope = linesBetweenCommentAndEndOfScope >
                                                           Settings.MaximumNewLinesAtEndOfCodeScope
                            ? Settings.MaximumNewLinesAtEndOfCodeScope
                            : linesBetweenCommentAndEndOfScope;

                        if (linesBetweenCommentAndEndOfScope == 0)
                        {
                            linesBetweenCommentAndEndOfScope = 1;
                        }

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndEndOfScope));
                    }
                }
            }
            else //no comments
            {
                var linesBetweenNodeAndEndOfScope = (snode.SourceRange.LineEnd -
                                                     node.SourceRange.LineEnd);

                linesBetweenNodeAndEndOfScope = linesBetweenNodeAndEndOfScope > Settings.MaximumNewLinesAtEndOfCodeScope
                    ? Settings.MaximumNewLinesAtEndOfCodeScope
                    : linesBetweenNodeAndEndOfScope;


                if (linesBetweenNodeAndEndOfScope == 0)
                {
                    linesBetweenNodeAndEndOfScope = 1;
                }

                Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndEndOfScope));
            }
        }


        public override bool VisitHexLiteral(ILSLHexLiteralNode lslHexLiteralNode)
        {
            Write(lslHexLiteralNode.RawText);

            return true;
        }


        public override bool VisitExpressionList(ILSLExpressionListNode node)
        {
            var nodeCount = node.Expressions.Count;

            for (var nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
            {
                var nodeAheadIndex = nodeIndex + 1;

                Visit(node.Expressions[nodeIndex]);

                if (nodeAheadIndex >= nodeCount) continue;

                var me = node.Expressions[nodeIndex];
                var next = node.Expressions[nodeAheadIndex];


                bool wrap = 
                    ExpressionWrappingListCurrentlyEnabled && 
                    CurrentExpressionListWrappingContext.ForceWrapping && 
                    GetCharacterColumnsSinceLastLine() > CurrentExpressionListWrappingContext.MaximumCharactersBeforeWrap;

                if (!me.SourceRangesAvailable || !next.SourceRangesAvailable)
                {
                    if (wrap)
                    {
                        string indent =
                            LSLFormatTools.CreateTabsString(
                                CurrentExpressionListWrappingContext.TabsWrittenSinceLastLine) +
                            LSLFormatTools.CreateSpacesString(
                                CurrentExpressionListWrappingContext.NonTabsWrittenSinceLastLine);

                        Write(",\n" + indent);
                    }
                    else
                    {
                        Write(", ");
                    }
                    continue;
                }

                WriteCommentsBetweenRange(me.SourceRange.LastCharRange, node.SourceRangeCommaList[nodeIndex]);

                Write(",");

                //honor any wrapping the user has done by hand, even if ForceWrapping is false.
                wrap = wrap || me.SourceRange.LineEnd != next.SourceRange.LineStart;


                if (!WriteCommentsBetweenRange(node.SourceRangeCommaList[nodeIndex], next.SourceRange.FirstCharRange, wrap ? 1 : 0) && !wrap)
                {
                    Write(" ");
                }

                if (wrap)
                {
                    string indent =
                        LSLFormatTools.CreateTabsString(
                            CurrentExpressionListWrappingContext.TabsWrittenSinceLastLine) +
                        LSLFormatTools.CreateSpacesString(
                            CurrentExpressionListWrappingContext.NonTabsWrittenSinceLastLine);

                    Write("\n" + indent);
                }
            }

            return true;
        }


        private class ExpressionListWrappingContext
        {
            public ExpressionListWrappingContext(ILSLReadOnlySyntaxTreeNode firstExpression, LSLCodeFormatterVisitor parent)
            {
                FirstExpression = firstExpression;
                WriteColumn = parent._writeColumn;
                WriteLine = parent._writeLine;
                TabsWrittenSinceLastLine = parent._tabsWrittenSinceLastLine;
                NonTabsWrittenSinceLastLine = parent._nonTabsWrittenSinceLastLine;
            }

            public ILSLReadOnlySyntaxTreeNode FirstExpression { get; private set; }
            public int WriteColumn { get; private set; }
            public int WriteLine { get; private set; }
            public int TabsWrittenSinceLastLine { get; private set; }
            public int NonTabsWrittenSinceLastLine { get; private set; }
            public int MaximumCharactersBeforeWrap { get; set; }
            public bool ForceWrapping { get; set; }
        }

        private class ExpressionWrappingContext
        {
            public ExpressionWrappingContext(ILSLReadOnlySyntaxTreeNode statement, LSLCodeFormatterVisitor parent)
            {
                Statement = statement;
                WriteColumn = parent._writeColumn;
                WriteLine = parent._writeLine;
                TabsWrittenSinceLastLine = parent._tabsWrittenSinceLastLine;
                NonTabsWrittenSinceLastLine = parent._nonTabsWrittenSinceLastLine;
            }


            public ILSLReadOnlySyntaxTreeNode Statement { get; private set; }
            public int WriteColumn { get; private set; }
            public int WriteLine { get; private set; }
            public int TabsWrittenSinceLastLine { get; private set; }
            public int NonTabsWrittenSinceLastLine { get; private set; }
            public int ColumnsBeforeExpressionWrap { get; set; }
            public int MinimumExpressionsToWrap { get; set; }
        }
    }
}