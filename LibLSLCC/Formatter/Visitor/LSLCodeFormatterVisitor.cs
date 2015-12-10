﻿#region FileInfo

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
            if (str == string.Empty)
            {
                return;
            }

            foreach (var c in str)
            {
                if (c == '\n')
                {
                    _writeLine++;
                    _writeColumn = 0;
                    _tabsWrittenSinceLastLine = 0;
                    _nonTabsWrittenSinceLastLine = 0;
                    OnNewLineWritten();
                }
                else if (c == '\t')
                {
                    _tabsWrittenSinceLastLine++;
                    OnTabWritten();
                }
                else
                {
                    _nonTabsWrittenSinceLastLine++;
                    OnNonTabWritten();
                }

                if (c == '\n') continue;

                _writeColumn++;
                OnColumnCharacterWritten();
            }

            Writer.Write(str);
        }


        private void OnColumnCharacterWritten()
        {
        }

        private void OnNonTabWritten()
        {
        }

        private void OnTabWritten()
        {
        }


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
            if (comments.Count > 0)
            {
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

                for (var j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    if (comment.SourceCodeRange.LineStart != left.LineEnd)
                    {
                        Write(GenIndent() + comment.Text);
                    }
                    else
                    {
                        Write(comment.Text);
                    }

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
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
                            Write(
                                LSLFormatTools.CreateNewLinesString(
                                    linesBetweenCommentAndNextNode - existingNewLinesBetweenNextNode) + GenIndent());
                        }
                    }
                }

                return true;
            }

            return false;
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

            var cnt = node.ExpressionNodes.Count;
            var cntr = 0;
            int start, len;

            if (cnt > 0)
            {
                start = node.Parent.SourceCodeRange.StartIndex + 1;
                len = node.ExpressionNodes[0].SourceCodeRange.StartIndex - start;
                Write(_sourceReference.Substring(start, len));
            }
            else
            {
                start = node.Parent.SourceCodeRange.StartIndex + 1;
                len = node.Parent.SourceCodeRange.StopIndex - start;
                Write(_sourceReference.Substring(start, len));

                return true;
            }

            for (var x = 0; x < cnt; x++)
            {
                Visit(node.ExpressionNodes[x]);
                if ((cntr + 1) < cnt)
                {
                    start = node.ExpressionNodes[x].SourceCodeRange.StopIndex + 1;
                    len = node.ExpressionNodes[x + 1].SourceCodeRange.StartIndex - start;
                    Write(_sourceReference.Substring(start, len));
                }

                cntr++;
            }

            if (cnt > 0)
            {
                start = node.ExpressionNodes.Last().SourceCodeRange.StopIndex + 1;
                len = node.Parent.SourceCodeRange.StopIndex - start;
                Write(_sourceReference.Substring(start, len));
            }

            ExpressionWrappingPop();

            return true;
        }

        public override bool VisitFunctionCallParameters(ILSLExpressionListNode node)
        {
            var cnt = node.ExpressionNodes.Count;
            var cntr = 0;


            ExpressionWrappingPush(false, null);

            var parent = (ILSLFunctionCallNode) node.Parent;
            int start, len;

            if (cnt > 0)
            {
                start = parent.OpenParenthSourceCodeRange.StartIndex + 1;
                len = node.ExpressionNodes[0].SourceCodeRange.StartIndex - start;
                Write(_sourceReference.Substring(start, len));
            }
            else
            {
                start = parent.OpenParenthSourceCodeRange.StartIndex + 1;
                len = parent.CloseParenthSourceCodeRange.StopIndex - start;
                Write(_sourceReference.Substring(start, len));

                return true;
            }


            for (var x = 0; x < cnt; x++)
            {
                Visit(node.ExpressionNodes[x]);
                if ((cntr + 1) < cnt)
                {
                    start = node.ExpressionNodes[x].SourceCodeRange.StopIndex + 1;
                    len = node.ExpressionNodes[x + 1].SourceCodeRange.StartIndex - start;
                    Write(_sourceReference.Substring(start, len));
                }

                cntr++;
            }


            if (cnt > 0)
            {
                start = node.ExpressionNodes.Last().SourceCodeRange.StopIndex + 1;
                len = parent.CloseParenthSourceCodeRange.StopIndex - start;
                Write(_sourceReference.Substring(start, len));
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

            WriteCommentsBetweenRange(node.SourceCodeRange.GetFirstCharRange(), node.InnerExpression.SourceCodeRange);


            Visit(node.InnerExpression);


            WriteCommentsBetweenRange(node.InnerExpression.SourceCodeRange, node.SourceCodeRange.GetLastCharRange());

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

            WriteCommentsBetweenRange(node.SourceCodeRange.GetFirstCharRange(), node.XExpression.SourceCodeRange);

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

            WriteCommentsBetweenRange(node.SExpression.SourceCodeRange, node.SourceCodeRange.GetLastCharRange());

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

            WriteCommentsBetweenRange(node.SourceCodeRange.GetFirstCharRange(), node.XExpression.SourceCodeRange);

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

            WriteCommentsBetweenRange(node.ZExpression.SourceCodeRange, node.SourceCodeRange.GetLastCharRange());

            Write(">");

            ExpressionWrappingPop();

            return true;
        }

        public override bool VisitDoLoop(ILSLDoLoopNode node)
        {
            Write("do");

            WriteCommentsBetweenRange(node.DoKeywordSourceCodeRange, node.Code.SourceCodeRange, 1);

            Visit(node.Code);

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

            WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange, node.Code.SourceCodeRange, 1);

            Visit(node.Code);

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

            WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange, node.Code.SourceCodeRange, 1);

            Visit(node.Code);


            return true;
        }

        public IEnumerable<LSLComment> GetComments(int sourceRangeStart, int sourceRangeEnd)
        {
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


                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    Visit(node);

                    if ((i + 1) < nodes.Count)
                    {
                        var nextNode = nodes[i + 1];
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

            for (var j = 0; j < comments.Count; j++)
            {
                var comment = comments[j];
                Write(FormatComment("", comment));

                if ((j + 1) < comments.Count)
                {
                    var nextComment = comments[j + 1];
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


            for (var j = 0; j < comments.Count; j++)
            {
                var comment = comments[j];

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

                if ((j + 1) < comments.Count)
                {
                    var nextComment = comments[j + 1];
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

            for (var j = 0; j < comments.Count; j++)
            {
                var comment = comments[j];

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


                if ((j + 1) < comments.Count)
                {
                    var nextComment = comments[j + 1];
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

                    if (linesBetweenNodeAndFirstComment == 0 && linesBetweenCommentAndNextNode < 3)
                    {
                        if ((nextNode is ILSLFunctionDeclarationNode || nextNode is ILSLStateScopeNode) &&
                            (node is ILSLFunctionDeclarationNode || node is ILSLVariableDeclarationNode))
                        {
                            linesBetweenCommentAndNextNode = 3;
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
                if (linesBetweenTwoNodes < 3) linesBetweenTwoNodes = 3;
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


                for (var j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    Write(FormatComment("", comment));

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
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
                var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                       node.ParameterListNode.SourceCodeRange.LineEnd);


                Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));

                for (var j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    if (comment.SourceCodeRange.LineStart != node.ParameterListNode.SourceCodeRange.LineEnd)
                    {
                        Write(GenIndent() + comment.Text);
                    }
                    else
                    {
                        Write(" " + comment.Text);
                    }

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
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
                var linesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.LineStart -
                                                       node.ParameterListNode.SourceCodeRange.LineEnd);


                Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));


                for (var j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    if (comment.SourceCodeRange.LineStart != node.ParameterListNode.SourceCodeRange.LineEnd)
                    {
                        Write(GenIndent() + comment.Text);
                    }
                    else
                    {
                        Write(" " + comment.Text);
                    }

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
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

            WriteCommentsBetweenRange(snode.StateNameSourceCodeRange, snode.OpenBraceSourceCodeRange);

            var spaceBeforeOpeningBrace =
                LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningStateBrace);

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

                for (var j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    Write(FormatComment(indent, comment));

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
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


            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                var singleLineBroken = 0;

                Write(indent);
                Visit(node);

                if ((i + 1) < nodes.Count)
                {
                    var nextNode = nodes[i + 1];

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

                        if (linesBetweenNodeAndFirstComment < Settings.MinimumNewLinesBetweenEventAndFollowingComment)
                        {
                            linesBetweenNodeAndFirstComment = Settings.MinimumNewLinesBetweenEventAndFollowingComment;
                        }

                        Write(LSLFormatTools.CreateNewLinesString(linesBetweenNodeAndFirstComment));


                        for (var j = 0; j < comments.Count; j++)
                        {
                            var comment = comments[j];

                            Write(FormatComment(indent, comment));

                            if ((j + 1) < comments.Count)
                            {
                                var nextComment = comments[j + 1];
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
                        var linesBetweenTwoNodes = (nextNode.SourceCodeRange.LineStart -
                                                    node.SourceCodeRange.LineEnd);

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


                        for (var j = 0; j < comments.Count; j++)
                        {
                            var comment = comments[j];
                            Write(FormatComment(indent, comment));

                            if ((j + 1) < comments.Count)
                            {
                                var nextComment = comments[j + 1];
                                var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                            comment.SourceCodeRange.LineEnd);

                                Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                            }
                            else
                            {
                                var linesBetweenCommentAndEndOfScope = (snode.CloseBraceSourceCodeRange.LineEnd -
                                                                        comment.SourceCodeRange.LineEnd);

                                linesBetweenCommentAndEndOfScope = linesBetweenCommentAndEndOfScope >
                                                                   Settings.MaximumNewLinesAtEndOfStateScope
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

            var cnt = node.Parameters.Count;
            var cntr = 0;
            for (var x = 0; x < cnt; x++)
            {
                Visit(node.Parameters[x]);
                if (cntr != (cnt - 1))
                {
                    Write(", ");
                }

                cntr++;
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

            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];


                Visit(node);

                if ((i + 1) >= nodes.Count) continue;

                var nextNode = nodes[i + 1];

                WriteCommentsBetweenRange(node.SourceCodeRange, nextNode.SourceCodeRange, 1);
            }

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

            if (node.Code.IsSingleStatement)
            {
                _indentLevel++;
                WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange, node.Code.SourceCodeRange, 1);
                _indentLevel--;
            }
            else
            {
                WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange, node.Code.SourceCodeRange, 1);
            }

            Visit(node.Code);
            return true;
        }

        public override bool VisitElseIfStatement(ILSLElseIfStatementNode node)
        {
            if (Settings.ElseIfStatementOnNewLine)
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


            if (node.Code.IsSingleStatement)
            {
                _indentLevel++;
                WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange, node.Code.SourceCodeRange, 1);
                _indentLevel--;
            }
            else
            {
                WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange, node.Code.SourceCodeRange, 1);
            }

            Visit(node.Code);
            return true;
        }

        public override bool VisitElseStatement(ILSLElseStatementNode node)
        {
            if (Settings.ElseStatementOnNewLine)
            {
                Write("\n" + GenIndent() + "else");
            }
            else
            {
                Write(LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeUnbrokenElseStatement) + "else");
            }


            if (node.Code.IsSingleStatement)
            {
                _indentLevel++;
                WriteCommentsBetweenRange(node.ElseKeywordSourceCodeRange, node.Code.SourceCodeRange, 1);
                _indentLevel--;
            }
            else
            {
                WriteCommentsBetweenRange(node.ElseKeywordSourceCodeRange, node.Code.SourceCodeRange, 1);
            }

            Visit(node.Code);

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
                    node.SourceCodeRange.GetLastCharRange());

                Write(";");
            }
            else
            {
                WriteCommentsBetweenRange(node.NameSourceCodeRange, node.SourceCodeRange.GetLastCharRange());

                Write(";");
            }


            return true;
        }

        public override bool VisitSingleStatementCodeScope(ILSLCodeScopeNode node)
        {
            _indentLevel++;

            var statement = node.CodeStatements.First();

            if (!(statement is ILSLSemiColonStatement))
            {
                if (Settings.IndentBracelessControlStatements)
                {
                    Write("\n" + GenIndent());
                }
                else
                {
                    Write(" ");
                }
            }

            Visit(statement);

            _indentLevel--;

            return true;
        }

        private string FormatComment(string indent, LSLComment comment)
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

            var indnt = LSLFormatTools.CreateTabCorrectSpaceString(indentSpaces == 0 ? 0 : indentSpaces - 1);

            firstLine = indnt + "/*" + firstLine + "\n";

            for (var i = 1; i < parts.Count; i++)
            {
                var part = parts[i];
                var userIndent = part.GetStringSpacesIndented();
                if (indentSpaces != userIndent || (indentSpaces == 0 && userIndent == 0))
                {
                    part = indnt + " " + part.Trim();
                }

                firstLine += part + (i == parts.Count - 1 ? "" : "\n");
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
                    spaceBeforeOpeningBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningIfBrace);
                }
                else if (snode.CodeScopeType == LSLCodeScopeType.ElseIf)
                {
                    newLine = Settings.ElseIfStatementBracesOnNewLine;
                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingElseIfBrace);
                    spaceBeforeOpeningBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningElseIfBrace);
                }
                else if (snode.CodeScopeType == LSLCodeScopeType.Else)
                {
                    newLine = Settings.ElseStatementBracesOnNewLine;
                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingElseBrace);
                    spaceBeforeOpeningBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningElseBrace);
                }
                else if (snode.CodeScopeType == LSLCodeScopeType.ForLoop)
                {
                    newLine = Settings.ForLoopBracesOnNewLine;
                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingForBrace);
                    spaceBeforeOpeningBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningForBrace);
                }
                else if (snode.CodeScopeType == LSLCodeScopeType.DoLoop)
                {
                    newLine = Settings.DoLoopBracesOnNewLine;
                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingDoLoopBrace);
                    spaceBeforeOpeningBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningDoLoopBrace);
                }
                else if (snode.CodeScopeType == LSLCodeScopeType.WhileLoop)
                {
                    newLine = Settings.WhileLoopBracesOnNewLine;
                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingWhileLoopBrace);
                    spaceBeforeOpeningBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningWhileLoopBrace);
                }
                else if (snode.CodeScopeType == LSLCodeScopeType.EventHandler)
                {
                    newLine = Settings.EventBracesOnNewLine;
                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingEventBrace);
                    spaceBeforeOpeningBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningEventBrace);
                }
                else if (snode.CodeScopeType == LSLCodeScopeType.Function)
                {
                    newLine = Settings.FunctionBracesOnNewLine;
                    spaceBeforeClosingBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeClosingFunctionBrace);
                    spaceBeforeOpeningBrace =
                        LSLFormatTools.CreateTabCorrectSpaceString(Settings.SpacesBeforeOpeningFunctionBrace);
                }

                if (newLine)
                {
                    Write("\n" + GenIndent() + spaceBeforeOpeningBrace + "{\n");
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

                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];

                    var singleLineBroken = 0;


                    Write(indent);
                    Visit(node);

                    if ((i + 1) < nodes.Count)
                    {
                        var nextNode = nodes[i + 1];

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


            for (var j = 0; j < comments.Count; j++)
            {
                var comment = comments[j];

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

                if ((j + 1) < comments.Count)
                {
                    var nextComment = comments[j + 1];
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

                for (var j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];


                    Write(FormatComment(indent, comment));

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
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
                if (linesBetweenTwoNodes < Settings.MinimumNewLinesBetweenDistinctStatements && !(
                    (nextNode is ILSLVariableDeclarationNode && node is ILSLVariableDeclarationNode) ||
                    (nextNode is ILSLExpressionStatementNode && node is ILSLExpressionStatementNode)
                    ))
                {
                    linesBetweenTwoNodes = Settings.MinimumNewLinesBetweenDistinctStatements;
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

            for (var j = 0; j < comments.Count; j++)
            {
                var comment = comments[j];


                Write(FormatComment(indent, comment));

                if ((j + 1) < comments.Count)
                {
                    var nextComment = comments[j + 1];
                    var linesBetweenComments = (nextComment.SourceCodeRange.LineStart -
                                                comment.SourceCodeRange.LineEnd);

                    Write(LSLFormatTools.CreateNewLinesString(linesBetweenComments));
                }
                else
                {
                    var linesBetweenCommentAndNextNode = (firstNode.SourceCodeRange.LineStart -
                                                          comment.SourceCodeRange.LineEnd);

                    if (linesBetweenCommentAndNextNode == 0) linesBetweenCommentAndNextNode = 1;


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


                for (var j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

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

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
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
            var cnt = node.ExpressionNodes.Count;
            var cntr = 0;


            for (var x = 0; x < cnt; x++)
            {
                Visit(node.ExpressionNodes[x]);
                if ((cntr + 1) < cnt)
                {
                    var start = node.ExpressionNodes[x].SourceCodeRange.StopIndex + 1;
                    var len = node.ExpressionNodes[x + 1].SourceCodeRange.StartIndex - start;
                    Write(_sourceReference.Substring(start, len));
                }

                cntr++;
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