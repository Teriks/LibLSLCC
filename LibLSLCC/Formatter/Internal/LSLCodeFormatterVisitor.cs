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
using LibLSLCC.CodeValidator.Visitor;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.Formatter.Internal
{
    /// <summary>
    ///     An LSL Syntax tree visitor that formats code.
    /// </summary>
    internal sealed class LSLCodeFormatterVisitor : LSLValidatorNodeVisitor<bool>
    {
        private readonly LinkedList<LSLComment> _comments = new LinkedList<LSLComment>();

        private readonly Stack<Tuple<bool, ExpressionWrappingContext>> _expressionContextStack =
            new Stack<Tuple<bool, ExpressionWrappingContext>>();

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


        private Tuple<bool, ExpressionWrappingContext> ExpressionWrappingPop()
        {
            return _expressionContextStack.Pop();
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

            StringBuilder strBuilder = new StringBuilder();

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
            return GetComments(left.StopIndex, right.StartIndex).ToList();
        }


        private bool WriteCommentsBetweenRange(LSLSourceCodeRange left, LSLSourceCodeRange right,
            int existingNewLinesBetweenNextNode = 0)
        {
            var comments = CommentsBetweenRange(left, right).ToList();
            return WriteCommentsBetweenRange(comments, left, right, existingNewLinesBetweenNextNode);
        }


        private bool WriteCommentsBetweenRange(
            IList<LSLComment> comments,
            LSLSourceCodeRange left,
            LSLSourceCodeRange right,
            int existingNewLinesBetweenNextNode = 0)
        {
            if (comments.Count <= 0) return false;

            var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart - left.LineEnd);

            if (linesBetweenNodeAndFirstComment == 0)
            {
                var spacesBetweenNodeAndFirstComment = ((comments[0].SourceRange.StartIndex - left.StopIndex) -
                                                        1);

                spacesBetweenNodeAndFirstComment = spacesBetweenNodeAndFirstComment > 0
                    ? spacesBetweenNodeAndFirstComment
                    : 1;


                Write(LSLFormatTools.CreateTabCorrectSpaceString(spacesBetweenNodeAndFirstComment));
            }

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));

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
                    var linesBetweenCommentAndNextNode = (right.LineStart - comment.SourceRange.LineEnd);

                    if (linesBetweenCommentAndNextNode == 0)
                    {
                        var spacesBetweenCommentAndNextNode = (right.StartIndex - comment.SourceRange.StopIndex);

                        spacesBetweenCommentAndNextNode = spacesBetweenCommentAndNextNode > 0
                            ? spacesBetweenCommentAndNextNode
                            : 1;

                        Write(LSLFormatTools.CreateTabCorrectSpaceString(spacesBetweenCommentAndNextNode));
                    }
                    else
                    {
                        var newLinesCnt = linesBetweenCommentAndNextNode - existingNewLinesBetweenNextNode;

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
            if (node.Parent is ILSLExpressionStatementNode && Settings.StatementExpressionWrapping)
            {
                Visit(node.LeftExpression);


                if (!WriteCommentsBetweenRange(node.LeftExpression.SourceRange, node.SourceRangeOperation))
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


                if (!WriteCommentsBetweenRange(node.SourceRangeOperation, node.RightExpression.SourceRange))
                {
                    Write(" ");
                }


                ExpressionWrappingPush(true, wrappingContext);
                Visit(node.RightExpression);
                ExpressionWrappingPop();
                return true;
            }


            Visit(node.LeftExpression);


            if (!WriteCommentsBetweenRange(node.LeftExpression.SourceRange, node.SourceRangeOperation))
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


            if (!WriteCommentsBetweenRange(node.SourceRangeOperation, node.RightExpression.SourceRange))
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


            int sourceStart, sourceLen;

            if (nodeCount > 0)
            {
                sourceStart = node.Parent.SourceRange.StartIndex + 1;
                sourceLen = node.Expressions[0].SourceRange.StartIndex - sourceStart;

                Write(_sourceReference.Substring(sourceStart, sourceLen));
            }
            else
            {
                sourceStart = node.Parent.SourceRange.StartIndex + 1;
                sourceLen = node.Parent.SourceRange.StopIndex - sourceStart;

                Write(_sourceReference.Substring(sourceStart, sourceLen));

                return true;
            }

            for (var nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
            {
                var nodeAheadIndex = nodeIndex + 1;

                Visit(node.Expressions[nodeIndex]);

                if ((nodeIndex + 1) < nodeCount)
                {
                    sourceStart = node.Expressions[nodeIndex].SourceRange.StopIndex + 1;

                    sourceLen = node.Expressions[nodeAheadIndex].SourceRange.StartIndex - sourceStart;

                    Write(_sourceReference.Substring(sourceStart, sourceLen));
                }
            }

            if (nodeCount > 0)
            {
                sourceStart = node.Expressions.Last().SourceRange.StopIndex + 1;
                sourceLen = node.Parent.SourceRange.StopIndex - sourceStart;

                Write(_sourceReference.Substring(sourceStart, sourceLen));
            }

            ExpressionWrappingPop();

            return true;
        }


        public override bool VisitFunctionCallParameters(ILSLExpressionListNode node)
        {
            var nodeCount = node.Expressions.Count;


            ExpressionWrappingPush(false, null);

            var parent = (ILSLFunctionCallNode) node.Parent;
            int sourceStart, sourceLen;

            if (nodeCount > 0)
            {
                sourceStart = parent.SourceRangeOpenParenth.StartIndex + 1;
                sourceLen = node.Expressions[0].SourceRange.StartIndex - sourceStart;

                Write(_sourceReference.Substring(sourceStart, sourceLen));
            }
            else
            {
                sourceStart = parent.SourceRangeOpenParenth.StartIndex + 1;
                sourceLen = parent.SourceRangeCloseParenth.StopIndex - sourceStart;

                Write(_sourceReference.Substring(sourceStart, sourceLen));

                return true;
            }


            for (var nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
            {
                var nodeAheadIndex = nodeIndex + 1;

                Visit(node.Expressions[nodeIndex]);

                if (nodeAheadIndex < nodeCount)
                {
                    sourceStart = node.Expressions[nodeIndex].SourceRange.StopIndex + 1;

                    sourceLen = node.Expressions[nodeAheadIndex].SourceRange.StartIndex - sourceStart;
                    Write(_sourceReference.Substring(sourceStart, sourceLen));
                }
            }


            if (nodeCount > 0)
            {
                sourceStart = node.Expressions.Last().SourceRange.StopIndex + 1;
                sourceLen = parent.SourceRangeCloseParenth.StopIndex - sourceStart;

                Write(_sourceReference.Substring(sourceStart, sourceLen));
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

            WriteCommentsBetweenRange(node.SourceRange.FirstCharRange, node.InnerExpression.SourceRange);


            Visit(node.InnerExpression);


            WriteCommentsBetweenRange(node.InnerExpression.SourceRange, node.SourceRange.LastCharRange);

            Write(")");
            ExpressionWrappingPop();

            return true;
        }


        public override bool VisitPostfixOperation(ILSLPostfixOperationNode node)
        {
            Visit(node.LeftExpression);


            WriteCommentsBetweenRange(node.LeftExpression.SourceRange, node.SourceRangeOperation);

            Write(node.OperationString);

            return true;
        }


        public override bool VisitPrefixOperation(ILSLPrefixOperationNode node)
        {
            Write(node.OperationString);

            WriteCommentsBetweenRange(node.SourceRangeOperation, node.RightExpression.SourceRange);

            Visit(node.RightExpression);
            return true;
        }


        public override bool VisitRotationLiteral(ILSLRotationLiteralNode node)
        {
            ExpressionWrappingPush(false, null);

            Write("<");

            WriteCommentsBetweenRange(node.SourceRange.FirstCharRange, node.XExpression.SourceRange);

            Visit(node.XExpression);

            WriteCommentsBetweenRange(node.XExpression.SourceRange, node.SourceRangeCommaOne);

            Write(",");

            var commentsBetween = WriteCommentsBetweenRange(node.SourceRangeCommaOne,
                node.YExpression.SourceRange);
            if (!commentsBetween)
            {
                Write(" ");
            }


            Visit(node.YExpression);

            WriteCommentsBetweenRange(node.YExpression.SourceRange, node.SourceRangeCommaTwo);

            Write(",");

            commentsBetween = WriteCommentsBetweenRange(node.SourceRangeCommaTwo, node.ZExpression.SourceRange);
            if (!commentsBetween)
            {
                Write(" ");
            }


            Visit(node.ZExpression);

            WriteCommentsBetweenRange(node.ZExpression.SourceRange, node.SourceRangeCommaThree);

            Write(",");

            commentsBetween = WriteCommentsBetweenRange(node.SourceRangeCommaThree, node.SExpression.SourceRange);
            if (!commentsBetween)
            {
                Write(" ");
            }

            Visit(node.SExpression);

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

            WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.SourceRangeCastToType);

            Write(node.CastToTypeName);

            WriteCommentsBetweenRange(node.SourceRangeCastToType.LastCharRange, node.SourceRangeCloseParenth);

            Writer.Write(")");

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
            Write("." + node.AccessedComponentString);

            return true;
        }


        public override bool VisitVectorLiteral(ILSLVectorLiteralNode node)
        {
            ExpressionWrappingPush(false, null);

            Write("<");

            WriteCommentsBetweenRange(node.SourceRange.FirstCharRange, node.XExpression.SourceRange);

            Visit(node.XExpression);

            WriteCommentsBetweenRange(node.XExpression.SourceRange, node.SourceRangeCommaOne);

            Write(",");

            var commentsBetween = WriteCommentsBetweenRange(node.SourceRangeCommaOne,
                node.YExpression.SourceRange);
            if (!commentsBetween)
            {
                Write(" ");
            }


            Visit(node.YExpression);

            WriteCommentsBetweenRange(node.YExpression.SourceRange, node.SourceRangeCommaTwo);

            Write(",");

            commentsBetween = WriteCommentsBetweenRange(node.SourceRangeCommaTwo, node.ZExpression.SourceRange);
            if (!commentsBetween)
            {
                Write(" ");
            }

            Visit(node.ZExpression);

            WriteCommentsBetweenRange(node.ZExpression.SourceRange, node.SourceRange.LastCharRange);

            Write(">");

            ExpressionWrappingPop();

            return true;
        }


        public override bool VisitDoLoop(ILSLDoLoopNode node)
        {
            Write("do");


            _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeDoKeyword,
                node.Code.SourceRange, 1);

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            var comments = WriteCommentsBetweenRange(node.Code.SourceRange, node.SourceRangeWhileKeyword);

            if (!comments)
            {
                Write("\n" + GenIndent());
            }

            Write("while");

            WriteCommentsBetweenRange(node.SourceRangeWhileKeyword, node.SourceRangeOpenParenth);

            Write("(");

            WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.ConditionExpression.SourceRange);


            var wrappingContext = new ExpressionWrappingContext(node, this)
            {
                ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeDoWhileExpressionWrap,
                MinimumExpressionsToWrap = Settings.MinimumExpressionsInDoWhileToWrap
            };

            ExpressionWrappingPush(Settings.DoWhileExpressionWrapping, wrappingContext);
            Visit(node.ConditionExpression);
            ExpressionWrappingPop();


            WriteCommentsBetweenRange(node.ConditionExpression.SourceRange, node.SourceRangeCloseParenth);

            Write(")");

            WriteCommentsBetweenRange(node.SourceRangeCloseParenth, node.SourceRangeSemicolon);

            Write(";");

            return true;
        }


        public override bool VisitForLoop(ILSLForLoopNode node)
        {
            Write("for");


            WriteCommentsBetweenRange(node.SourceRangeForKeyword, node.SourceRangeOpenParenth);

            Write("(");


            if (node.HasInitExpressions)
            {
                WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.InitExpressionList.SourceRange);

                ExpressionWrappingPush(false, null);
                Visit(node.InitExpressionList);
                ExpressionWrappingPop();

                WriteCommentsBetweenRange(node.InitExpressionList.SourceRange, node.SourceRangeFirstSemicolon);
            }
            else
            {
                WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.SourceRangeFirstSemicolon);
            }


            if (node.HasConditionExpression)
            {
                var commentsBetween = CommentsBetweenRange(node.SourceRangeFirstSemicolon,
                    node.ConditionExpression.SourceRange);

                Write(commentsBetween.Count > 0 ? ";" : "; ");

                WriteCommentsBetweenRange(commentsBetween, node.SourceRangeFirstSemicolon,
                    node.ConditionExpression.SourceRange);


                ExpressionWrappingPush(false, null);
                Visit(node.ConditionExpression);
                ExpressionWrappingPop();

                WriteCommentsBetweenRange(node.ConditionExpression.SourceRange, node.SourceRangeSecondSemicolon);
            }
            else
            {
                Write(";");
                WriteCommentsBetweenRange(node.SourceRangeFirstSemicolon, node.SourceRangeSecondSemicolon);
            }


            if (node.HasAfterthoughtExpressions)
            {
                var commentsBetween =
                    CommentsBetweenRange(
                        node.SourceRangeSecondSemicolon,
                        node.AfterthoughExpressionList.SourceRange);

                Write(commentsBetween.Count > 0 ? ";" : "; ");


                WriteCommentsBetweenRange(commentsBetween, node.SourceRangeSecondSemicolon,
                    node.AfterthoughExpressionList.SourceRange);

                ExpressionWrappingPush(false, null);
                Visit(node.AfterthoughExpressionList);
                ExpressionWrappingPop();

                WriteCommentsBetweenRange(node.AfterthoughExpressionList.SourceRange, node.SourceRangeCloseParenth);
            }
            else
            {
                Write(";");
                WriteCommentsBetweenRange(node.SourceRangeSecondSemicolon, node.SourceRangeCloseParenth);
            }


            Write(")");

            _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeCloseParenth,
                node.Code.SourceRange, 1);

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            return true;
        }


        public override bool VisitWhileLoop(ILSLWhileLoopNode node)
        {
            Write("while");

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


            WriteCommentsBetweenRange(node.ConditionExpression.SourceRange, node.SourceRangeCloseParenth);

            Write(")");

            _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.SourceRangeCloseParenth,
                node.Code.SourceRange, 1);

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


        public override bool VisitCompilationUnit(ILSLCompilationUnitNode unode)
        {
            var nodes = unode.GlobalVariableDeclarations.Concat<ILSLReadOnlySyntaxTreeNode>(unode.FunctionDeclarations)
                .Concat(new[] {unode.DefaultStateNode})
                .Concat(unode.StateDeclarations).ToList();

            nodes.Sort((a, b) => a.SourceRange.StartIndex.CompareTo(b.SourceRange.StartIndex));

            IList<LSLComment> comments;

            if (nodes.Count > 0)
            {
                CompilationUnit_WriteCommentsAtTopOfSource_WithNodes(nodes, unode);


                for (var nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
                {
                    var nodeAheadIndex = nodeIndex + 1;

                    var node = nodes[nodeIndex];

                    Visit(node);


                    if (nodeAheadIndex < nodes.Count)
                    {
                        var nextNode = nodes[nodeAheadIndex];

                        comments =
                            GetComments(node.SourceRange.StopIndex, nextNode.SourceRange.StartIndex).ToList();

                        if (comments.Count > 0)
                        {
                            CompilationUnit_BetweenTopLevelNodes_WithCommentsBetween(node, comments, nextNode);
                        }
                        else
                        {
                            CompilationUnit_BetweenTopLevelNodes_NoCommentsBetween(node, nextNode);
                        }
                    }
                    else
                    {
                        comments = GetComments(node.SourceRange.StopIndex, _sourceReference.Length).ToList();

                        if (comments.Count > 0)
                        {
                            CompilationUnit_AfterLastNode_WithComments(node, comments, unode);
                        }
                        else
                        {
                            CompilationUnit_AfterLastNode_NoComments(node, unode);
                        }
                    }
                }
            }
            else
            {
                comments = GetComments(unode.SourceRange.StartIndex, unode.SourceRange.StopIndex).ToList();

                if (comments.Count > 0)
                {
                    CompilationUnit_NoTreeNodes_WithComments(unode, comments);
                }
                else
                {
                    CompilationUnit_NoTreeNodes_WithoutComments(unode);
                }
            }


            return true;
        }


        private void CompilationUnit_NoTreeNodes_WithoutComments(ILSLCompilationUnitNode unode)
        {
            var linesBetweenNodeAndEndOfScope = (unode.SourceRange.LineEnd - unode.SourceRange.LineStart);

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndEndOfScope - 1));
        }


        private void CompilationUnit_NoTreeNodes_WithComments(ILSLCompilationUnitNode unode, IList<LSLComment> comments)
        {
            var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                   unode.SourceRange.LineStart);

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
                    var linesBetweenCommentAndEndOfScope = (unode.SourceRange.LineEnd -
                                                            comment.SourceRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndEndOfScope));
                }
            }
        }


        private void CompilationUnit_AfterLastNode_NoComments(ILSLReadOnlySyntaxTreeNode node,
            ILSLCompilationUnitNode unode)
        {
            var linesBetweenNodeAndEndOfScope = (unode.SourceRange.LineEnd -
                                                 node.SourceRange.LineEnd);

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndEndOfScope));
        }


        private void CompilationUnit_AfterLastNode_WithComments(ILSLReadOnlySyntaxTreeNode node,
            IList<LSLComment> comments, ILSLCompilationUnitNode unode)
        {
            var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                   node.SourceRange.LineEnd);


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
                    var linesBetweenCommentAndEndOfScope = (unode.SourceRange.LineEnd -
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

            if (linesBetweenNodeAndFirstComment <= 2 && linesBetweenNodeAndFirstComment > 0)
            {
                if ((nextNode is ILSLFunctionDeclarationNode || nextNode is ILSLStateScopeNode)
                    && (node is ILSLFunctionDeclarationNode || node is ILSLVariableDeclarationNode))
                {
                    linesBetweenNodeAndFirstComment = 3;
                }
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

            if ((nextNode is ILSLFunctionDeclarationNode && node is ILSLFunctionDeclarationNode) ||
                (nextNode is ILSLFunctionDeclarationNode && node is ILSLVariableDeclarationNode) ||
                (nextNode is ILSLVariableDeclarationNode && node is ILSLFunctionDeclarationNode) ||
                (nextNode is ILSLStateScopeNode && node is ILSLFunctionDeclarationNode) ||
                (nextNode is ILSLStateScopeNode && node is ILSLVariableDeclarationNode) ||
                (nextNode is ILSLStateScopeNode && node is ILSLStateScopeNode))
            {
                if (linesBetweenTwoNodes < Settings.MinimumNewLinesBetweenDistinctGlobalStatements)
                {
                    linesBetweenTwoNodes = Settings.MinimumNewLinesBetweenDistinctGlobalStatements;
                }
            }

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes));
        }


        private void CompilationUnit_WriteCommentsAtTopOfSource_WithNodes(
            IList<ILSLReadOnlySyntaxTreeNode> nodes,
            ILSLCompilationUnitNode unode)
        {
            var comments = GetComments(0, nodes[0].SourceRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                       unode.SourceRange.LineStart);


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
                var linesBetweenTwoNodes = (nodes[0].SourceRange.LineStart - unode.SourceRange.LineStart);

                Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes - 1));
            }
        }


        public override bool VisitEventHandler(ILSLEventHandlerNode node)
        {
            Write(node.Name + "(");
            Visit(node.ParameterList);
            Write(")");

            var comments =
                GetComments(node.ParameterList.SourceRange.StopIndex,
                    node.Code.SourceRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                _wroteCommentAfterEventParameterList = true;

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


            var comments =
                GetComments(node.ParameterList.SourceRange.StopIndex,
                    node.FunctionBodyNode.SourceRange.StartIndex).ToList();

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
                        var linesBetweenCommentAndNextNode = (node.FunctionBodyNode.SourceRange.LineStart -
                                                              comment.SourceRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode - 1));
                    }
                }
            }


            Visit(node.FunctionBodyNode);

            return true;
        }


        public override bool VisitState(ILSLStateScopeNode snode)
        {
            if (snode.IsDefaultState)
            {
                Write("default");
            }
            else
            {
                Write("state");

                if (!WriteCommentsBetweenRange(snode.SourceRangeStateKeyword, snode.SourceRangeStateName))
                {
                    Write(" ");
                }

                Write(snode.StateName);
            }

            var wroteCommentBetweenStateNameAndOpenBrace = WriteCommentsBetweenRange(snode.SourceRangeStateName,
                snode.SourceRangeOpenBrace);

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

            var nodes = snode.EventHandlers;

            var indent = GenIndent();


            var comments =
                GetComments(snode.SourceRangeOpenBrace.StartIndex, nodes[0].SourceRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = (comments[0].SourceRange.LineStart -
                                                       snode.SourceRangeOpenBrace.LineStart);

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
                        var linesBetweenCommentAndNextNode = (nodes[0].SourceRange.LineStart -
                                                              comment.SourceRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode));
                    }
                }
            }
            else
            {
                var linesBetweenTwoNodes = (nodes[0].SourceRange.LineStart -
                                            snode.SourceRangeOpenBrace.LineStart);

                linesBetweenTwoNodes = linesBetweenTwoNodes > Settings.MaximumNewLinesAtBeginingOfStateScope
                    ? Settings.MaximumNewLinesAtBeginingOfStateScope
                    : linesBetweenTwoNodes;

                Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes - 1));
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

                    if (node.SourceRange.LineStart == nextNode.SourceRange.LineStart)
                    {
                        singleLineBroken = 1;
                        Write("\n");
                    }


                    comments =
                        GetComments(node.SourceRange.StopIndex, nextNode.SourceRange.StartIndex).ToList();

                    if (comments.Count > 0)
                    {
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
                    else
                    {
                        var linesBetweenTwoNodes = (nextNode.SourceRange.LineStart - node.SourceRange.LineEnd);

                        if (linesBetweenTwoNodes < Settings.MinimumNewLinesBetweenEventHandlers)
                        {
                            linesBetweenTwoNodes = (Settings.MinimumNewLinesBetweenEventHandlers - singleLineBroken);
                        }


                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes));
                    }
                }
                else
                {
                    comments = GetComments(node.SourceRange.StopIndex, snode.SourceRange.StopIndex).ToList();

                    if (comments.Count > 0)
                    {
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
                                var linesBetweenCommentAndEndOfScope = (snode.SourceRangeCloseBrace.LineEnd -
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
                    else
                    {
                        var linesBetweenNodeAndEndOfScope = (snode.SourceRangeCloseBrace.LineEnd -
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
                }
            }


            _indentLevel--;

            Write(LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingStateBrace) + "}");

            return true;
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

                if (!WriteCommentsBetweenRange(node.SourceRangeReturnKeyword, node.ReturnExpression.SourceRange))
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

                WriteCommentsBetweenRange(node.ReturnExpression.SourceRange, node.SourceRangeSemicolon);

                Write(";");
            }
            else
            {
                Write("return");
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

            if (!WriteCommentsBetweenRange(node.SourceRangeStateKeyword, node.SourceRangeStateName))
            {
                Write(" ");
            }

            Write(node.StateTargetName);

            WriteCommentsBetweenRange(node.SourceRangeStateName, node.SourceRangeSemicolon);

            Write(";");

            return true;
        }


        public override bool VisitJumpStatement(ILSLJumpStatementNode node)
        {
            Write("jump");

            if (!WriteCommentsBetweenRange(node.SourceRangeJumpKeyword, node.SourceRangeLabelName))
            {
                Write(" ");
            }

            Write(node.LabelName);

            WriteCommentsBetweenRange(node.SourceRangeLabelName, node.SourceRangeSemicolon);

            Write(";");

            return true;
        }


        public override bool VisitLabelStatement(ILSLLabelStatementNode node)
        {
            Write("@");

            WriteCommentsBetweenRange(node.SourceRangeLabelPrefix, node.SourceRangeLabelName);

            Write(node.LabelName);

            WriteCommentsBetweenRange(node.SourceRangeLabelName, node.SourceRangeSemicolon);

            Write(";");

            return true;
        }


        public override bool VisitControlStatement(ILSLControlStatementNode snode)
        {
            IEnumerable<ILSLReadOnlySyntaxTreeNode> nodese = (new ILSLReadOnlySyntaxTreeNode[] {snode.IfStatement});

            if (snode.HasElseIfStatements)
            {
                nodese = nodese.Concat(snode.ElseIfStatement);
            }

            if (snode.HasElseStatement)
            {
                nodese = nodese.Concat(new ILSLReadOnlySyntaxTreeNode[] {snode.ElseStatement}).ToList();
            }


            var nodes = nodese.ToList();


            GenIndent();

            for (var nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
            {
                var nodeAheadIndex = nodeIndex + 1;

                var node = nodes[nodeIndex];


                Visit(node);

                if (nodeAheadIndex >= nodes.Count) continue;

                var nextNode = nodes[nodeAheadIndex];

                _wroteCommentAfterControlChainMember = WriteCommentsBetweenRange(node.SourceRange, nextNode.SourceRange,
                    1);
            }

            _wroteCommentAfterControlChainMember = false;

            return true;
        }


        public override bool VisitIfStatement(ILSLIfStatementNode node)
        {
            Write("if");

            WriteCommentsBetweenRange(node.SourceRangeIfKeyword, node.SourceRangeOpenParenth);

            Write("(");

            WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.ConditionExpression.SourceRange);


            var wrappingContext = new ExpressionWrappingContext(node, this)
            {
                ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeIfExpressionWrap,
                MinimumExpressionsToWrap = Settings.MinimumExpressionsInIfToWrap
            };

            ExpressionWrappingPush(Settings.IfExpressionWrapping, wrappingContext);
            Visit(node.ConditionExpression);
            ExpressionWrappingPop();


            WriteCommentsBetweenRange(node.ConditionExpression.SourceRange, node.SourceRangeCloseParenth);

            Write(")");

            if (((ILSLReadOnlyCodeStatement) node.Code).InsideSingleStatementScope)
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


            if (!WriteCommentsBetweenRange(node.SourceRangeElseKeyword, node.SourceRangeIfKeyword))
            {
                Write(" ");
            }

            Write("if");

            WriteCommentsBetweenRange(node.SourceRangeIfKeyword, node.SourceRangeOpenParenth);

            Write("(");

            WriteCommentsBetweenRange(node.SourceRangeOpenParenth, node.ConditionExpression.SourceRange);


            var wrappingContext = new ExpressionWrappingContext(node, this)
            {
                ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeElseIfExpressionWrap,
                MinimumExpressionsToWrap = Settings.MinimumExpressionsInElseIfToWrap
            };


            ExpressionWrappingPush(Settings.ElseIfExpressionWrapping, wrappingContext);
            Visit(node.ConditionExpression);
            ExpressionWrappingPop();


            WriteCommentsBetweenRange(node.ConditionExpression.SourceRange, node.SourceRangeCloseParenth);

            Write(")");


            if (((ILSLReadOnlyCodeStatement) node.Code).InsideSingleStatementScope)
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


            if (((ILSLReadOnlyCodeStatement) node.Code).InsideSingleStatementScope)
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

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            return true;
        }


        public override bool VisitExpressionStatement(ILSLExpressionStatementNode node)
        {
            Visit(node.Expression);


            WriteCommentsBetweenRange(node.Expression.SourceRange, node.SourceRangeSemicolon);


            Write(";");

            return true;
        }


        public override bool VisitVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            Write(node.TypeName);

            if (!WriteCommentsBetweenRange(node.SourceRangeType, node.SourceRangeName))
            {
                Write(" ");
            }

            Write(node.Name);

            if (node.HasDeclarationExpression)
            {
                if (!WriteCommentsBetweenRange(node.SourceRangeName, node.SourceRangeOperator))
                {
                    Write(" ");
                }

                Write("=");

                var wrappingContext = new ExpressionWrappingContext(node, this)
                {
                    ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeDeclarationExpressionWrap,
                    MinimumExpressionsToWrap = Settings.MinimumExpressionsInDeclarationToWrap
                };

                if (!WriteCommentsBetweenRange(node.SourceRangeOperator, node.DeclarationExpression.SourceRange))
                {
                    Write(" ");
                }


                ExpressionWrappingPush(Settings.DeclarationExpressionWrapping, wrappingContext);
                Visit(node.DeclarationExpression);
                ExpressionWrappingPop();


                WriteCommentsBetweenRange(node.DeclarationExpression.SourceRange,
                    node.SourceRange.LastCharRange);

                Write(";");
            }
            else
            {
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


        public override bool VisitMultiStatementCodeScope(ILSLCodeScopeNode snode)
        {
            var spaceBeforeClosingBrace = "";
            if (snode.Parent is ILSLCodeScopeNode)
            {
                Write("{\n");
            }
            else
            {
                var newLine = false;
                var spaceBeforeOpeningBrace = "";

                if (snode.CodeScopeType == LSLCodeScopeType.If)
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
                else if (snode.CodeScopeType == LSLCodeScopeType.ElseIf)
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
                else if (snode.CodeScopeType == LSLCodeScopeType.Else)
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
                else if (snode.CodeScopeType == LSLCodeScopeType.ForLoop)
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
                else if (snode.CodeScopeType == LSLCodeScopeType.DoLoop)
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
                else if (snode.CodeScopeType == LSLCodeScopeType.WhileLoop)
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
                else if (snode.CodeScopeType == LSLCodeScopeType.EventHandler)
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
                else if (snode.CodeScopeType == LSLCodeScopeType.Function)
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

            var nodes = snode.CodeStatements.ToList();

            var indent = GenIndent();


            if (nodes.Count > 0)
            {
                var comments =
                    GetComments(snode.SourceRange.StartIndex, nodes[0].SourceRange.StartIndex).ToGenericArray();

                if (comments.Count > 0)
                {
                    MultiStatement_BeforeFirstNode_CommentsBefore(comments, nodes[0], snode);
                }
                else
                {
                    MultiStatement_BeforeFirstNode_NoPrecedingComments(nodes[0], snode);
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

                        if (node.SourceRange.LineEnd == nextNode.SourceRange.LineStart)
                        {
                            singleLineBroken = 1;
                        }


                        comments =
                            GetComments(node.SourceRange.StopIndex, nextNode.SourceRange.StartIndex)
                                .ToGenericArray();

                        if (comments.Count > 0)
                        {
                            MultiStatement_BetweenTwoNodes_WithComments(singleLineBroken, node, comments, nextNode);
                        }
                        else //no comments
                        {
                            MultiStatement_BetweenTwoNodes_NoComments(singleLineBroken, node, nextNode);
                        }
                    }
                    else // last node in scope
                    {
                        MultiStatementLastNodeInScope(node, snode);
                    }
                }
            }
            else // no nodes
            {
                MultiStatement_NoNodesInCodeScope(snode);
            }


            _indentLevel--;

            Write(GenIndent() + spaceBeforeClosingBrace + "}");

            return true;
        }


        private void MultiStatement_BetweenTwoNodes_WithComments(
            int singleLineBroken,
            ILSLReadOnlyCodeStatement node,
            IReadOnlyGenericArray<LSLComment> comments,
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


        private void MultiStatement_NoNodesInCodeScope(ILSLCodeScopeNode snode)
        {
            var indent = GenIndent();
            var comments = GetComments(snode.SourceRange.StartIndex, snode.SourceRange.StopIndex).ToList();

            if (comments.Count > 0)
            {
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
            else
            {
                var linesBetweenNodeAndEndOfScope = (snode.SourceRange.LineEnd - snode.SourceRange.LineStart);

                linesBetweenNodeAndEndOfScope = linesBetweenNodeAndEndOfScope > Settings.MaximumNewLinesAtEndOfCodeScope
                    ? Settings.MaximumNewLinesAtEndOfCodeScope
                    : linesBetweenNodeAndEndOfScope;


                Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndEndOfScope - 1));
            }
        }


        private void MultiStatement_BetweenTwoNodes_NoComments(int singleLineBroken, ILSLReadOnlyCodeStatement node,
            ILSLReadOnlyCodeStatement nextNode)
        {
            var linesBetweenTwoNodes = ((nextNode.SourceRange.LineStart -
                                         node.SourceRange.LineEnd) + singleLineBroken);

            if ((nextNode is ILSLVariableDeclarationNode ||
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
                 node is ILSLCodeScopeNode))
            {
                if (linesBetweenTwoNodes < Settings.MinimumNewLinesBetweenDistinctLocalStatements && !(
                    (nextNode is ILSLVariableDeclarationNode && node is ILSLVariableDeclarationNode) ||
                    (nextNode is ILSLExpressionStatementNode && node is ILSLExpressionStatementNode)
                    ))
                {
                    linesBetweenTwoNodes = Settings.MinimumNewLinesBetweenDistinctLocalStatements;
                }
            }

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes));
        }


        private void MultiStatement_BeforeFirstNode_NoPrecedingComments(ILSLReadOnlyCodeStatement firstNode,
            ILSLCodeScopeNode snode)
        {
            var linesBetweenTwoNodes = (firstNode.SourceRange.LineStart - snode.SourceRange.LineStart);

            linesBetweenTwoNodes = linesBetweenTwoNodes > Settings.MaximumNewLinesAtBeginingOfCodeScope
                ? Settings.MaximumNewLinesAtBeginingOfCodeScope
                : linesBetweenTwoNodes;

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes - 1));
        }


        private void MultiStatement_BeforeFirstNode_CommentsBefore(IList<LSLComment> commentsBetweenOpenBraceAndNode,
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


        private void MultiStatementLastNodeInScope(ILSLReadOnlyCodeStatement node, ILSLCodeScopeNode snode)
        {
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
            var expressionCount = node.Expressions.Count;

            for (var expressionIndex = 0; expressionIndex < expressionCount; expressionIndex++)
            {
                var expressionAheadIndex = expressionIndex + 1;

                Visit(node.Expressions[expressionIndex]);

                if (expressionAheadIndex < expressionCount)
                {
                    var start = node.Expressions[expressionIndex].SourceRange.StopIndex + 1;

                    var len = node.Expressions[expressionAheadIndex].SourceRange.StartIndex - start;

                    Write(_sourceReference.Substring(start, len));
                }
            }
            return true;
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