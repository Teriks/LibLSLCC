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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.Formatter.Visitor
{
    /// <summary>
    /// An LSL Syntax tree visitor that formats code.
    /// </summary>
    internal class LSLCodeFormatterVisitor : LSLValidatorNodeVisitor<bool>
    {
        public LSLCodeFormatterSettings Settings { get; private set; }

        private readonly Stack<Tuple<bool, ExpressionWrappingContext>> _expressionContextStack =
            new Stack<Tuple<bool, ExpressionWrappingContext>>();

        private readonly LinkedList<LSLComment> _comments = new LinkedList<LSLComment>();
        private int _indentLevel;
        private string _sourceReference;
        private int _writeColumn;
        private int _writeLine;
        private int _tabsWrittenSinceLastLine;
        private int _nonTabsWrittenSinceLastLine;
        private int _binaryExpressionsSinceNewLine;
        private bool _wroteCommentBeforeControlStatementCode;
        private bool _wroteCommentAfterControlChainMember;
        private bool _wroteCommentAfterEventParameterList;
        private bool _wroteCommentAfterFunctionDeclarationParameterList;


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


        /// <summary>
        /// Default constructor for the formatting visitor.
        /// </summary>
        public LSLCodeFormatterVisitor(LSLCodeFormatterSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// The TextWriter that was passed to the WriteAndFlush function.
        /// </summary>
        public TextWriter Writer { get; private set; }


        private ExpressionWrappingContext CurrentExpressionWrappingContext
        {
            get
            {
                return _expressionContextStack.Count > 0 ? _expressionContextStack.Peek().Item2 : null;
            }
        }

        private bool ExpressionWrappingCurrentlyEnabled
        {
            get { return _expressionContextStack.Count > 0 && _expressionContextStack.Peek().Item1; }
        }


        private void ExpressionWrappingPush(bool enabled, ExpressionWrappingContext context)
        {
            if (enabled && context == null)
            {
                throw new ArgumentNullException("context", "ExpressionWrappingContext cannot be null if 'enabled' is true!.");
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
        /// Writes formated code to a TextWriter as a given <see cref="ILSLCompilationUnitNode"/> is visited.
        /// A reference to the original source code text is required.
        /// </summary>
        /// <param name="sourceReference">A reference to the original source code text.</param>
        /// <param name="node">The <see cref="ILSLCompilationUnitNode"/> that was created from the source code text.  Most likely by <see cref="LSLCodeValidator"/>.</param>
        /// <param name="writer">The TextWriter to write the formatted code to.</param>
        /// <param name="closeStream">Whether or not to call .Close() on the TextWriter when done writing to it.</param>
        public void WriteAndFlush(string sourceReference, ILSLCompilationUnitNode node, TextWriter writer,
            bool closeStream = true)
        {
            try
            {
                _sourceReference = sourceReference;

                foreach (var comment in node.Comments)
                {
                    _comments.AddLast(comment);
                }

                Writer = writer;

                Visit(node);
                Writer.Flush();

                if (closeStream)
                {
                    Writer.Close();
                }
            }
            finally
            {
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

            var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart - left.LineEnd);

            if (linesBetweenNodeAndFirstComment == 0)
            {
                var spacesBetweenNodeAndFirstComment = ((comments[0].SourceCodeRange.StartIndex - left.StopIndex) -
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

                if (comment.SourceCodeRange.LineStart != left.LineEnd)
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

                    var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                comment.SourceCodeRange.LineEnd);

                    if (linesBetweenComments == 0)
                    {
                        var spacesBetweenComments = (nextComment.SourceCodeRange.StartIndex -
                                                     comment.SourceCodeRange.StopIndex);

                        Write(LSLFormatTools.CreateTabCorrectSpaceString(spacesBetweenComments));
                    }

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndNextNode = (right.LineStart - comment.SourceCodeRange.LineEnd);

                    if (linesBetweenCommentAndNextNode == 0)
                    {
                        var spacesBetweenCommentAndNextNode = (right.StartIndex - comment.SourceCodeRange.StopIndex);

                        spacesBetweenCommentAndNextNode = spacesBetweenCommentAndNextNode > 0
                            ? spacesBetweenCommentAndNextNode
                            : 1;

                        Write(LSLFormatTools.CreateTabCorrectSpaceString(spacesBetweenCommentAndNextNode));
                    }
                    else
                    {
                        var newLinesCnt = linesBetweenCommentAndNextNode - existingNewLinesBetweenNextNode;

                        var newLines = LSLFormatTools.CreateNewLinesString(newLinesCnt);

                        if (newLinesCnt > 0){
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


                if (!WriteCommentsBetweenRange(node.LeftExpression.SourceCodeRange, node.OperationSourceCodeRange))
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


                if (!WriteCommentsBetweenRange(node.OperationSourceCodeRange, node.RightExpression.SourceCodeRange))
                {
                    Write(" ");
                }


                ExpressionWrappingPush(true, wrappingContext);
                Visit(node.RightExpression);
                ExpressionWrappingPop();
                return true;
            }


            Visit(node.LeftExpression);


            if (!WriteCommentsBetweenRange(node.LeftExpression.SourceCodeRange, node.OperationSourceCodeRange))
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


            if (!WriteCommentsBetweenRange(node.OperationSourceCodeRange, node.RightExpression.SourceCodeRange))
            {
                Write(" ");
            }


            Visit(node.RightExpression);


            return true;
        }

        public override bool VisitListLiteralInitializerList(ILSLExpressionListNode node)
        {
            ExpressionWrappingPush(false, null);

            var nodeCount = node.ExpressionNodes.Count;


            int sourceStart, sourceLen;

            if (nodeCount > 0)
            {
                sourceStart = node.Parent.SourceCodeRange.StartIndex + 1;
                sourceLen = node.ExpressionNodes[0].SourceCodeRange.StartIndex - sourceStart;

                Write(_sourceReference.Substring(sourceStart, sourceLen));
            }
            else
            {
                sourceStart = node.Parent.SourceCodeRange.StartIndex + 1;
                sourceLen = node.Parent.SourceCodeRange.StopIndex - sourceStart;

                Write(_sourceReference.Substring(sourceStart, sourceLen));

                return true;
            }

            for (var nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
            {
                var nodeAheadIndex = nodeIndex + 1;

                Visit(node.ExpressionNodes[nodeIndex]);

                if ((nodeIndex + 1) < nodeCount)
                {
                    sourceStart = node.ExpressionNodes[nodeIndex].SourceCodeRange.StopIndex + 1;

                    sourceLen = node.ExpressionNodes[nodeAheadIndex].SourceCodeRange.StartIndex - sourceStart;

                    Write(_sourceReference.Substring(sourceStart, sourceLen));
                }

            }

            if (nodeCount > 0)
            {
                sourceStart = node.ExpressionNodes.Last().SourceCodeRange.StopIndex + 1;
                sourceLen = node.Parent.SourceCodeRange.StopIndex - sourceStart;

                Write(_sourceReference.Substring(sourceStart, sourceLen));
            }

            ExpressionWrappingPop();

            return true;
        }


        public override bool VisitFunctionCallParameters(ILSLExpressionListNode node)
        {
            var nodeCount = node.ExpressionNodes.Count;


            ExpressionWrappingPush(false, null);

            var parent = (ILSLFunctionCallNode) node.Parent;
            int sourceStart, sourceLen;

            if (nodeCount > 0)
            {
                sourceStart = parent.OpenParenthSourceCodeRange.StartIndex + 1;
                sourceLen = node.ExpressionNodes[0].SourceCodeRange.StartIndex - sourceStart;

                Write(_sourceReference.Substring(sourceStart, sourceLen));
            }
            else
            {
                sourceStart = parent.OpenParenthSourceCodeRange.StartIndex + 1;
                sourceLen = parent.CloseParenthSourceCodeRange.StopIndex - sourceStart;

                Write(_sourceReference.Substring(sourceStart, sourceLen));

                return true;
            }


            for (var nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
            {
                var nodeAheadIndex = nodeIndex + 1;

                Visit(node.ExpressionNodes[nodeIndex]);

                if (nodeAheadIndex < nodeCount)
                {
                    sourceStart = node.ExpressionNodes[nodeIndex].SourceCodeRange.StopIndex + 1;

                    sourceLen = node.ExpressionNodes[nodeAheadIndex].SourceCodeRange.StartIndex - sourceStart;
                    Write(_sourceReference.Substring(sourceStart, sourceLen));
                }

            }


            if (nodeCount > 0)
            {
                sourceStart = node.ExpressionNodes.Last().SourceCodeRange.StopIndex + 1;
                sourceLen = parent.CloseParenthSourceCodeRange.StopIndex - sourceStart;

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
            Visit(node.ParameterListNode);
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

            Visit(node.ExpressionListNode);

            Write("]");

            return true;
        }

        public override bool VisitParenthesizedExpression(ILSLParenthesizedExpressionNode node)
        {
            ExpressionWrappingPush(false, null);
            Write("(");

            WriteCommentsBetweenRange(node.SourceCodeRange.FirstCharRange, node.InnerExpression.SourceCodeRange);


            Visit(node.InnerExpression);


            WriteCommentsBetweenRange(node.InnerExpression.SourceCodeRange, node.SourceCodeRange.LastCharRange);

            Write(")");
            ExpressionWrappingPop();

            return true;
        }

        public override bool VisitPostfixOperation(ILSLPostfixOperationNode node)
        {
            Visit(node.LeftExpression);


            WriteCommentsBetweenRange(node.LeftExpression.SourceCodeRange, node.OperationSourceCodeRange);

            Write(node.OperationString);

            return true;
        }

        public override bool VisitPrefixOperation(ILSLPrefixOperationNode node)
        {
            Write(node.OperationString);

            WriteCommentsBetweenRange(node.OperationSourceCodeRange, node.RightExpression.SourceCodeRange);

            Visit(node.RightExpression);
            return true;
        }

        public override bool VisitRotationLiteral(ILSLRotationLiteralNode node)
        {
            ExpressionWrappingPush(false, null);

            Write("<");

            WriteCommentsBetweenRange(node.SourceCodeRange.FirstCharRange, node.XExpression.SourceCodeRange);

            Visit(node.XExpression);

            WriteCommentsBetweenRange(node.XExpression.SourceCodeRange, node.CommaOneSourceCodeRange);

            Write(",");

            var commentsBetween = WriteCommentsBetweenRange(node.CommaOneSourceCodeRange,
                node.YExpression.SourceCodeRange);
            if (!commentsBetween)
            {
                Write(" ");
            }


            Visit(node.YExpression);

            WriteCommentsBetweenRange(node.YExpression.SourceCodeRange, node.CommaTwoSourceCodeRange);

            Write(",");

            commentsBetween = WriteCommentsBetweenRange(node.CommaTwoSourceCodeRange, node.ZExpression.SourceCodeRange);
            if (!commentsBetween)
            {
                Write(" ");
            }


            Visit(node.ZExpression);

            WriteCommentsBetweenRange(node.ZExpression.SourceCodeRange, node.CommaThreeSourceCodeRange);

            Write(",");

            commentsBetween = WriteCommentsBetweenRange(node.CommaThreeSourceCodeRange, node.SExpression.SourceCodeRange);
            if (!commentsBetween)
            {
                Write(" ");
            }

            Visit(node.SExpression);

            WriteCommentsBetweenRange(node.SExpression.SourceCodeRange, node.SourceCodeRange.LastCharRange);

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
            Write("(" + node.CastToTypeString + ")");

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

            WriteCommentsBetweenRange(node.SourceCodeRange.FirstCharRange, node.XExpression.SourceCodeRange);

            Visit(node.XExpression);

            WriteCommentsBetweenRange(node.XExpression.SourceCodeRange, node.CommaOneSourceCodeRange);

            Write(",");

            var commentsBetween = WriteCommentsBetweenRange(node.CommaOneSourceCodeRange,
                node.YExpression.SourceCodeRange);
            if (!commentsBetween)
            {
                Write(" ");
            }


            Visit(node.YExpression);

            WriteCommentsBetweenRange(node.YExpression.SourceCodeRange, node.CommaTwoSourceCodeRange);

            Write(",");

            commentsBetween = WriteCommentsBetweenRange(node.CommaTwoSourceCodeRange, node.ZExpression.SourceCodeRange);
            if (!commentsBetween)
            {
                Write(" ");
            }

            Visit(node.ZExpression);

            WriteCommentsBetweenRange(node.ZExpression.SourceCodeRange, node.SourceCodeRange.LastCharRange);

            Write(">");

            ExpressionWrappingPop();

            return true;
        }

        public override bool VisitDoLoop(ILSLDoLoopNode node)
        {
            Write("do");


            _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.DoKeywordSourceCodeRange,
                node.Code.SourceCodeRange, 1);

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            var comments = WriteCommentsBetweenRange(node.Code.SourceCodeRange, node.WhileKeywordSourceCodeRange);

            if (!comments)
            {
                Write("\n" + GenIndent());
            }

            Write("while");

            WriteCommentsBetweenRange(node.WhileKeywordSourceCodeRange, node.OpenParenthSourceCodeRange);

            Write("(");

            WriteCommentsBetweenRange(node.OpenParenthSourceCodeRange, node.ConditionExpression.SourceCodeRange);


            var wrappingContext = new ExpressionWrappingContext(node, this)
            {
                ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeDoWhileExpressionWrap,
                MinimumExpressionsToWrap = Settings.MinimumExpressionsInDoWhileToWrap
            };

            ExpressionWrappingPush(Settings.DoWhileExpressionWrapping, wrappingContext);
            Visit(node.ConditionExpression);
            ExpressionWrappingPop();


            WriteCommentsBetweenRange(node.ConditionExpression.SourceCodeRange, node.CloseParenthSourceCodeRange);

            Write(")");

            WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange, node.SemiColonSourceCodeRange);

            Write(";");

            return true;
        }

        public override bool VisitForLoop(ILSLForLoopNode node)
        {
            Write("for");


            WriteCommentsBetweenRange(node.ForKeywordSourceCodeRange, node.OpenParenthSourceCodeRange);

            Write("(");


            if (node.HasInitExpressions)
            {
                WriteCommentsBetweenRange(node.OpenParenthSourceCodeRange, node.InitExpressionsList.SourceCodeRange);

                ExpressionWrappingPush(false, null);
                Visit(node.InitExpressionsList);
                ExpressionWrappingPop();

                WriteCommentsBetweenRange(node.InitExpressionsList.SourceCodeRange, node.FirstSemiColonSourceCodeRange);
            }
            else
            {
                WriteCommentsBetweenRange(node.OpenParenthSourceCodeRange, node.FirstSemiColonSourceCodeRange);
            }


            if (node.HasConditionExpression)
            {
                var commentsBetween = CommentsBetweenRange(node.FirstSemiColonSourceCodeRange,
                    node.ConditionExpression.SourceCodeRange);

                Write(commentsBetween.Count > 0 ? ";" : "; ");

                WriteCommentsBetweenRange(commentsBetween, node.FirstSemiColonSourceCodeRange,
                    node.ConditionExpression.SourceCodeRange);


                ExpressionWrappingPush(false, null);
                Visit(node.ConditionExpression);
                ExpressionWrappingPop();

                WriteCommentsBetweenRange(node.ConditionExpression.SourceCodeRange, node.SecondSemiColonSourceCodeRange);
            }
            else
            {
                Write(";");
                WriteCommentsBetweenRange(node.FirstSemiColonSourceCodeRange, node.SecondSemiColonSourceCodeRange);
            }


            if (node.HasAfterthoughtExpressions)
            {
                var commentsBetween =
                    CommentsBetweenRange(
                        node.SecondSemiColonSourceCodeRange,
                        node.AfterthoughExpressions.SourceCodeRange);

                Write(commentsBetween.Count > 0 ? ";" : "; ");


                WriteCommentsBetweenRange(commentsBetween, node.SecondSemiColonSourceCodeRange,
                    node.AfterthoughExpressions.SourceCodeRange);

                ExpressionWrappingPush(false, null);
                Visit(node.AfterthoughExpressions);
                ExpressionWrappingPop();

                WriteCommentsBetweenRange(node.AfterthoughExpressions.SourceCodeRange, node.CloseParenthSourceCodeRange);
            }
            else
            {
                Write(";");
                WriteCommentsBetweenRange(node.SecondSemiColonSourceCodeRange, node.CloseParenthSourceCodeRange);
            }


            Write(")");

            _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange,
                node.Code.SourceCodeRange, 1);

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            return true;
        }

        public override bool VisitWhileLoop(ILSLWhileLoopNode node)
        {
            Write("while");

            WriteCommentsBetweenRange(node.WhileKeywordSourceCodeRange, node.OpenParenthSourceCodeRange);

            Write("(");


            var wrappingContext = new ExpressionWrappingContext(node, this)
            {
                ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeWhileExpressionWrap,
                MinimumExpressionsToWrap = Settings.MinimumExpressionsInWhileToWrap
            };

            ExpressionWrappingPush(Settings.WhileExpressionWrapping, wrappingContext);
            Visit(node.ConditionExpression);
            ExpressionWrappingPop();


            WriteCommentsBetweenRange(node.ConditionExpression.SourceCodeRange, node.CloseParenthSourceCodeRange);

            Write(")");

            _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange,
                node.Code.SourceCodeRange, 1);

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            return true;
        }

        private IEnumerable<LSLComment> GetComments(int sourceRangeStart, int sourceRangeEnd)
        {
            if(Settings.RemoveComments) yield break;


            var first = _comments.First;

            while (first != null)
            {
                var next = first.Next;
                var comment = first.Value;

                if (comment.SourceCodeRange.StartIndex >= sourceRangeStart &&
                    comment.SourceCodeRange.StopIndex <= sourceRangeEnd)
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
                .Concat(new[] {unode.DefaultState})
                .Concat(unode.StateDeclarations).ToList();

            nodes.Sort((a, b) => a.SourceCodeRange.StartIndex.CompareTo(b.SourceCodeRange.StartIndex));

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
                            GetComments(node.SourceCodeRange.StopIndex, nextNode.SourceCodeRange.StartIndex).ToList();

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
                        comments = GetComments(node.SourceCodeRange.StopIndex, _sourceReference.Length).ToList();

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
                comments = GetComments(unode.SourceCodeRange.StartIndex, unode.SourceCodeRange.StopIndex).ToList();

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
            var linesBetweenNodeAndEndOfScope = (unode.SourceCodeRange.LineEnd - unode.SourceCodeRange.LineStart);

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndEndOfScope - 1));
        }

        private void CompilationUnit_NoTreeNodes_WithComments(ILSLCompilationUnitNode unode, IList<LSLComment> comments)
        {
            var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                   unode.SourceCodeRange.LineStart);

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment - 1));

            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];

                Write(FormatComment("", comment));

                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];

                    var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                comment.SourceCodeRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndEndOfScope = (unode.SourceCodeRange.LineEnd -
                                                            comment.SourceCodeRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndEndOfScope));
                }
            }
        }

        private void CompilationUnit_AfterLastNode_NoComments(ILSLReadOnlySyntaxTreeNode node,
            ILSLCompilationUnitNode unode)
        {
            var linesBetweenNodeAndEndOfScope = (unode.SourceCodeRange.LineEnd -
                                                 node.SourceCodeRange.LineEnd);

            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndEndOfScope));
        }

        private void CompilationUnit_AfterLastNode_WithComments(ILSLReadOnlySyntaxTreeNode node,
            IList<LSLComment> comments, ILSLCompilationUnitNode unode)
        {
            var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                   node.SourceCodeRange.LineEnd);


            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));


            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];

                if (comment.SourceCodeRange.LineStart != node.SourceCodeRange.LineEnd)
                {
                    Write(FormatComment("", comment));
                }
                else
                {
                    var space = _sourceReference.Substring(node.SourceCodeRange.StopIndex + 1,
                        (comment.SourceCodeRange.StartIndex - node.SourceCodeRange.StopIndex) - 1);

                    Write(space + comment.Text);
                }

                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];

                    var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                comment.SourceCodeRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndEndOfScope = (unode.SourceCodeRange.LineEnd -
                                                            comment.SourceCodeRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndEndOfScope));
                }
            }
        }

        private void CompilationUnit_BetweenTopLevelNodes_WithCommentsBetween(
            ILSLReadOnlySyntaxTreeNode node,
            IList<LSLComment> comments,
            ILSLReadOnlySyntaxTreeNode nextNode)
        {
            var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                   node.SourceCodeRange.LineEnd);

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

                if (comment.SourceCodeRange.LineStart != node.SourceCodeRange.LineEnd)
                {
                    Write(FormatComment("", comment));
                }
                else
                {
                    var space = _sourceReference.Substring(node.SourceCodeRange.StopIndex + 1,
                        (comment.SourceCodeRange.StartIndex - node.SourceCodeRange.StopIndex) - 1);

                    Write(space + comment.Text);
                }


                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];
                    var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                comment.SourceCodeRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndNextNode = (nextNode.SourceCodeRange.LineStart -
                                                          comment.SourceCodeRange.LineEnd);


                    linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                       node.SourceCodeRange.LineEnd);

                    if (linesBetweenNodeAndFirstComment == 0 && linesBetweenCommentAndNextNode < Settings.MinimumNewLinesBetweenDistinctGlobalStatements)
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
            var linesBetweenTwoNodes = (nextNode.SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd);

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
            var comments = GetComments(0, nodes[0].SourceCodeRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                       unode.SourceCodeRange.LineStart);


                Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment - 1));


                for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
                {
                    var commentAheadIndex = commentIndex + 1;

                    var comment = comments[commentIndex];

                    Write(FormatComment("", comment));

                    if (commentAheadIndex < comments.Count)
                    {
                        var nextComment = comments[commentAheadIndex];

                        var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                    comment.SourceCodeRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = (nodes[0].SourceCodeRange.LineStart -
                                                              comment.SourceCodeRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode));
                    }
                }
            }
            else
            {
                var linesBetweenTwoNodes = (nodes[0].SourceCodeRange.LineStart - unode.SourceCodeRange.LineStart);

                Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes - 1));
            }
        }

        public override bool VisitEventHandler(ILSLEventHandlerNode node)
        {
            Write(node.Name + "(");
            Visit(node.ParameterListNode);
            Write(")");

            var comments =
                GetComments(node.ParameterListNode.SourceCodeRange.StopIndex,
                    node.EventBodyNode.SourceCodeRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                _wroteCommentAfterEventParameterList = true;

                var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                       node.ParameterListNode.SourceCodeRange.LineEnd);


                Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));

                for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
                {
                    var commentAheadIndex = commentIndex + 1;

                    var comment = comments[commentIndex];

                    if (comment.SourceCodeRange.LineStart != node.ParameterListNode.SourceCodeRange.LineEnd)
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

                        var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                    comment.SourceCodeRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = (node.EventBodyNode.SourceCodeRange.LineStart -
                                                              comment.SourceCodeRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode - 1));
                    }
                }
            }

            Visit(node.EventBodyNode);

            return true;
        }

        public override bool VisitFunctionDeclaration(ILSLFunctionDeclarationNode node)
        {
            if (node.ReturnType != LSLType.Void)
            {
                Write(node.ReturnTypeString + " " + node.Name + "(");
            }
            else
            {
                Write(node.Name + "(");
            }

            Visit(node.ParameterListNode);
            Write(")");


            var comments =
                GetComments(node.ParameterListNode.SourceCodeRange.StopIndex,
                    node.FunctionBodyNode.SourceCodeRange.StartIndex).ToList();

            if (comments.Count > 0)
            {

                _wroteCommentAfterFunctionDeclarationParameterList = true;

                var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                       node.ParameterListNode.SourceCodeRange.LineEnd);


                Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));


                for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
                {
                    var commentAheadIndex = commentIndex + 1;

                    var comment = comments[commentIndex];

                    if (comment.SourceCodeRange.LineStart != node.ParameterListNode.SourceCodeRange.LineEnd)
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

                        var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                    comment.SourceCodeRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = (node.FunctionBodyNode.SourceCodeRange.LineStart -
                                                              comment.SourceCodeRange.LineEnd);

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

                if (!WriteCommentsBetweenRange(snode.StateKeywordSourceCodeRange, snode.StateNameSourceCodeRange))
                {
                    Write(" ");
                }

                Write(snode.StateName);
            }

            var wroteCommentBetweenStateNameAndOpenBrace = WriteCommentsBetweenRange(snode.StateNameSourceCodeRange, snode.OpenBraceSourceCodeRange);

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
                GetComments(snode.OpenBraceSourceCodeRange.StartIndex, nodes[0].SourceCodeRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                       snode.OpenBraceSourceCodeRange.LineStart);

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

                        var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                    comment.SourceCodeRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = (nodes[0].SourceCodeRange.LineStart -
                                                              comment.SourceCodeRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode));
                    }
                }
            }
            else
            {
                var linesBetweenTwoNodes = (nodes[0].SourceCodeRange.LineStart -
                                            snode.OpenBraceSourceCodeRange.LineStart);

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

                    if (node.SourceCodeRange.LineStart == nextNode.SourceCodeRange.LineStart)
                    {
                        singleLineBroken = 1;
                        Write("\n");
                    }


                    comments =
                        GetComments(node.SourceCodeRange.StopIndex, nextNode.SourceCodeRange.StartIndex).ToList();

                    if (comments.Count > 0)
                    {
                        var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                               node.SourceCodeRange.LineEnd);

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

                                var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                            comment.SourceCodeRange.LineEnd);

                                Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                            }
                            else
                            {
                                var linesBetweenCommentAndNextNode = (nextNode.SourceCodeRange.LineStart -
                                                                      comment.SourceCodeRange.LineEnd);

                                Write(LSLFormatTools.CreateNewLinesString(linesBetweenCommentAndNextNode));
                            }
                        }
                    }
                    else
                    {
                        var linesBetweenTwoNodes = (nextNode.SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd);

                        if (linesBetweenTwoNodes < Settings.MinimumNewLinesBetweenEventHandlers)
                        {
                            linesBetweenTwoNodes = (Settings.MinimumNewLinesBetweenEventHandlers - singleLineBroken);
                        }


                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenTwoNodes));
                    }
                }
                else
                {
                    comments = GetComments(node.SourceCodeRange.StopIndex, snode.SourceCodeRange.StopIndex).ToList();

                    if (comments.Count > 0)
                    {
                        var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                               node.SourceCodeRange.LineEnd);

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

                                var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                            comment.SourceCodeRange.LineEnd);

                                Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                            }
                            else
                            {
                                var linesBetweenCommentAndEndOfScope = (snode.CloseBraceSourceCodeRange.LineEnd -
                                                                        comment.SourceCodeRange.LineEnd);

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
                        var linesBetweenNodeAndEndOfScope = (snode.CloseBraceSourceCodeRange.LineEnd -
                                                             node.SourceCodeRange.LineEnd);


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
            Write(node.TypeString + " " + node.Name);

            return true;
        }

        public override bool VisitParameterDefinitionList(ILSLParameterListNode node)
        {
            if (!node.HasParameterNodes)
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

                if (!WriteCommentsBetweenRange(node.ReturnKeywordSourceCodeRange, node.ReturnExpression.SourceCodeRange))
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

                WriteCommentsBetweenRange(node.ReturnExpression.SourceCodeRange, node.SemiColonSourceCodeRange);

                Write(";");
            }
            else
            {
                Write("return");
                WriteCommentsBetweenRange(node.ReturnKeywordSourceCodeRange, node.SemiColonSourceCodeRange);
                Write(";");
            }
            return true;
        }

        public override bool VisitSemiColonStatement(ILSLSemiColonStatement node)
        {
            Write(";");

            return true;
        }

        public override bool VisitStateChangeStatement(ILSLStateChangeStatementNode node)
        {
            Write("state");

            if (!WriteCommentsBetweenRange(node.StateKeywordSourceCodeRange, node.StateNameSourceCodeRange))
            {
                Write(" ");
            }

            Write(node.StateTargetName);

            WriteCommentsBetweenRange(node.StateNameSourceCodeRange, node.SemiColonSourceCodeRange);

            Write(";");

            return true;
        }

        public override bool VisitJumpStatement(ILSLJumpStatementNode node)
        {
            Write("jump");

            if (!WriteCommentsBetweenRange(node.JumpKeywordSourceCodeRange, node.LabelNameSourceCodeRange))
            {
                Write(" ");
            }

            Write(node.LabelName);

            WriteCommentsBetweenRange(node.LabelNameSourceCodeRange, node.SemiColonSourceCodeRange);

            Write(";");

            return true;
        }

        public override bool VisitLabelStatement(ILSLLabelStatementNode node)
        {
            Write("@");

            WriteCommentsBetweenRange(node.LabelPrefixSourceCodeRange, node.LabelNameSourceCodeRange);

            Write(node.LabelName);

            WriteCommentsBetweenRange(node.LabelNameSourceCodeRange, node.SemiColonSourceCodeRange);

            Write(";");

            return true;
        }

        public override bool VisitControlStatement(ILSLControlStatementNode snode)
        {
            IEnumerable<ILSLReadOnlySyntaxTreeNode> nodese = (new ILSLReadOnlySyntaxTreeNode[] {snode.IfStatement});

            if (snode.HasElseIfStatements)
            {
                nodese = nodese.Concat(snode.ElseIfStatements);
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

                _wroteCommentAfterControlChainMember = WriteCommentsBetweenRange(node.SourceCodeRange, nextNode.SourceCodeRange, 1);
            }

            _wroteCommentAfterControlChainMember = false;

            return true;
        }

        public override bool VisitIfStatement(ILSLIfStatementNode node)
        {
            Write("if");

            WriteCommentsBetweenRange(node.IfKeywordSourceCodeRange, node.OpenParenthSourceCodeRange);

            Write("(");

            WriteCommentsBetweenRange(node.OpenParenthSourceCodeRange, node.ConditionExpression.SourceCodeRange);


            var wrappingContext = new ExpressionWrappingContext(node, this)
            {
                ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeIfExpressionWrap,
                MinimumExpressionsToWrap = Settings.MinimumExpressionsInIfToWrap
            };

            ExpressionWrappingPush(Settings.IfExpressionWrapping, wrappingContext);
            Visit(node.ConditionExpression);
            ExpressionWrappingPop();


            WriteCommentsBetweenRange(node.ConditionExpression.SourceCodeRange, node.CloseParenthSourceCodeRange);

            Write(")");

            if (node.Code.IsSingleBlockStatement)
            {
                _indentLevel++;

                _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange,
                    node.Code.SourceCodeRange, 1);

                _indentLevel--;
            }
            else
            {
                _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange, node.Code.SourceCodeRange, 1);
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


            if (!WriteCommentsBetweenRange(node.ElseKeywordSourceCodeRange, node.IfKeywordSourceCodeRange))
            {
                Write(" ");
            }

            Write("if");

            WriteCommentsBetweenRange(node.IfKeywordSourceCodeRange, node.OpenParenthSourceCodeRange);

            Write("(");

            WriteCommentsBetweenRange(node.OpenParenthSourceCodeRange, node.ConditionExpression.SourceCodeRange);


            var wrappingContext = new ExpressionWrappingContext(node, this)
            {
                ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeElseIfExpressionWrap,
                MinimumExpressionsToWrap = Settings.MinimumExpressionsInElseIfToWrap
            };


            ExpressionWrappingPush(Settings.ElseIfExpressionWrapping, wrappingContext);
            Visit(node.ConditionExpression);
            ExpressionWrappingPop();


            WriteCommentsBetweenRange(node.ConditionExpression.SourceCodeRange, node.CloseParenthSourceCodeRange);

            Write(")");


            if (node.Code.IsSingleBlockStatement)
            {
                _indentLevel++;

                _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange,
                    node.Code.SourceCodeRange, 1);

                _indentLevel--;
            }
            else
            {
                _wroteCommentBeforeControlStatementCode = 
                    WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange, node.Code.SourceCodeRange, 1);
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


            if (node.Code.IsSingleBlockStatement)
            {
                _indentLevel++;

                _wroteCommentBeforeControlStatementCode = WriteCommentsBetweenRange(node.ElseKeywordSourceCodeRange,
                    node.Code.SourceCodeRange, 1);

                _indentLevel--;
            }
            else
            {
                _wroteCommentBeforeControlStatementCode = 
                    WriteCommentsBetweenRange(node.ElseKeywordSourceCodeRange, node.Code.SourceCodeRange, 1);
            }

            Visit(node.Code);

            _wroteCommentBeforeControlStatementCode = false;

            return true;
        }

        public override bool VisitExpressionStatement(ILSLExpressionStatementNode node)
        {
            Visit(node.Expression);


            WriteCommentsBetweenRange(node.Expression.SourceCodeRange, node.SourceCodeRange);


            Write(";");

            return true;
        }



        public override bool VisitVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            Write(node.TypeString);

            if (!WriteCommentsBetweenRange(node.TypeSourceCodeRange, node.NameSourceCodeRange))
            {
                Write(" ");
            }

            Write(node.Name);

            if (node.HasDeclarationExpression)
            {
                if (!WriteCommentsBetweenRange(node.NameSourceCodeRange, node.OperatorSourceCodeRange))
                {
                    Write(" ");
                }

                Write("=");

                var wrappingContext = new ExpressionWrappingContext(node, this)
                {
                    ColumnsBeforeExpressionWrap = Settings.ColumnsBeforeDeclarationExpressionWrap,
                    MinimumExpressionsToWrap = Settings.MinimumExpressionsInDeclarationToWrap
                };

                if (!WriteCommentsBetweenRange(node.OperatorSourceCodeRange, node.DeclarationExpression.SourceCodeRange))
                {
                    Write(" ");
                }


                ExpressionWrappingPush(Settings.DeclarationExpressionWrapping, wrappingContext);
                Visit(node.DeclarationExpression);
                ExpressionWrappingPop();


                WriteCommentsBetweenRange(node.DeclarationExpression.SourceCodeRange,
                    node.SourceCodeRange.LastCharRange);

                Write(";");
            }
            else
            {
                WriteCommentsBetweenRange(node.NameSourceCodeRange, node.SourceCodeRange.LastCharRange);

                Write(";");
            }


            return true;
        }

        public override bool VisitSingleStatementCodeScope(ILSLCodeScopeNode node)
        {
            var statement = node.CodeStatements.First();

            if (statement is ILSLSemiColonStatement)
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
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingForBrace);

                        if (_wroteCommentAfterControlChainMember)
                        {
                            if (Settings.AddSpacesBeforeOpeningForLoopBraceAfterCommentBreak)
                            {
                                spaceBeforeOpeningBrace =
                                    LSLFormatTools.CreateTabCorrectSpaceString(
                                        Settings.SpacesBeforeOpeningForBrace);
                            }
                        }
                        else
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningForBrace);
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
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingForBrace);

                    if (_wroteCommentAfterControlChainMember)
                    {
                        if (Settings.AddSpacesBeforeOpeningForLoopBraceAfterCommentBreak)
                        {
                            spaceBeforeOpeningBrace =
                                LSLFormatTools.CreateTabCorrectSpaceString(
                                    Settings.SpacesBeforeOpeningForBrace);
                        }
                    }
                    else
                    {
                        spaceBeforeOpeningBrace =
                            LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningForBrace);
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
                    GetComments(snode.SourceCodeRange.StartIndex, nodes[0].SourceCodeRange.StartIndex).ToGenericArray();

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

                        if (node.SourceCodeRange.LineEnd == nextNode.SourceCodeRange.LineStart)
                        {
                            singleLineBroken = 1;
                        }


                        comments =
                            GetComments(node.SourceCodeRange.StopIndex, nextNode.SourceCodeRange.StartIndex)
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
            var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                   node.SourceCodeRange.LineEnd);


            Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));


            for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
            {
                var commentAheadIndex = commentIndex + 1;

                var comment = comments[commentIndex];

                if (comment.SourceCodeRange.LineStart != node.SourceCodeRange.LineEnd)
                {
                    Write(FormatComment(indent, comment));
                }
                else
                {
                    var space = _sourceReference.Substring(node.SourceCodeRange.StopIndex + 1,
                        (comment.SourceCodeRange.StartIndex - node.SourceCodeRange.StopIndex) - 1);

                    Write(space + comment.Text);
                }

                if (commentAheadIndex < comments.Count)
                {
                    var nextComment = comments[commentAheadIndex];

                    var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                comment.SourceCodeRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else //last comment
                {
                    var linesBetweenCommentAndNextNode = (nextNode.SourceCodeRange.LineStart -
                                                          comment.SourceCodeRange.LineEnd +
                                                          singleLineBroken);
                    var linesBetweenCommentAndLastNode = (node.SourceCodeRange.LineEnd -
                                                          comment.SourceCodeRange.LineStart);

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
            var comments = GetComments(snode.SourceCodeRange.StartIndex, snode.SourceCodeRange.StopIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                       snode.SourceCodeRange.LineStart);

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

                        var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                    comment.SourceCodeRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                    }
                    else
                    {
                        var linesBetweenCommentAndEndOfScope = (snode.SourceCodeRange.LineEnd -
                                                                comment.SourceCodeRange.LineEnd);

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
                var linesBetweenNodeAndEndOfScope = (snode.SourceCodeRange.LineEnd - snode.SourceCodeRange.LineStart);

                linesBetweenNodeAndEndOfScope = linesBetweenNodeAndEndOfScope > Settings.MaximumNewLinesAtEndOfCodeScope
                    ? Settings.MaximumNewLinesAtEndOfCodeScope
                    : linesBetweenNodeAndEndOfScope;


                Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndEndOfScope - 1));
            }
        }



        private void MultiStatement_BetweenTwoNodes_NoComments(int singleLineBroken, ILSLReadOnlyCodeStatement node,
            ILSLReadOnlyCodeStatement nextNode)
        {
            var linesBetweenTwoNodes = ((nextNode.SourceCodeRange.LineStart -
                                         node.SourceCodeRange.LineEnd) + singleLineBroken);

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
            var linesBetweenTwoNodes = (firstNode.SourceCodeRange.LineStart - snode.SourceCodeRange.LineStart);

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

            var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                   snode.SourceCodeRange.LineStart);

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

                    var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                comment.SourceCodeRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndNextNode = (firstNode.SourceCodeRange.LineStart -
                                                          comment.SourceCodeRange.LineEnd);

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
            var comments = GetComments(node.SourceCodeRange.StopIndex, snode.SourceCodeRange.StopIndex).ToList();
            var indent = GenIndent();

            if (comments.Count > 0)
            {
                var linesBetweenLastNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                           node.SourceCodeRange.LineEnd);


                Write(LSLFormatTools.CreateNewLinesString(linesBetweenLastNodeAndFirstComment));


                for (var commentIndex = 0; commentIndex < comments.Count; commentIndex++)
                {
                    var commentAheadIndex = commentIndex + 1;

                    var comment = comments[commentIndex];

                    if (comment.SourceCodeRange.LineStart != node.SourceCodeRange.LineEnd)
                    {
                        Write(FormatComment(indent, comment));
                    }
                    else
                    {
                        var space = _sourceReference.Substring(node.SourceCodeRange.StopIndex + 1,
                            (comment.SourceCodeRange.StartIndex - node.SourceCodeRange.StopIndex) - 1);

                        Write(space + comment.Text);
                    }

                    if (commentAheadIndex < comments.Count)
                    {
                        var nextComment = comments[commentAheadIndex];

                        var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                    comment.SourceCodeRange.LineEnd);

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                    }
                    else // last comment
                    {
                        var linesBetweenCommentAndEndOfScope = (snode.SourceCodeRange.LineEnd -
                                                                comment.SourceCodeRange.LineEnd);

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
                var linesBetweenNodeAndEndOfScope = (snode.SourceCodeRange.LineEnd -
                                                     node.SourceCodeRange.LineEnd);

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
            var expressionCount = node.ExpressionNodes.Count;

            for (var expressionIndex = 0; expressionIndex < expressionCount; expressionIndex++)
            {
                var expressionAheadIndex = expressionIndex + 1;

                Visit(node.ExpressionNodes[expressionIndex]);

                if (expressionAheadIndex < expressionCount)
                {
                    var start = node.ExpressionNodes[expressionIndex].SourceCodeRange.StopIndex + 1;

                    var len = node.ExpressionNodes[expressionAheadIndex].SourceCodeRange.StartIndex - start;

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