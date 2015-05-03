#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.Formatter.Visitor
{
    public class LSLCodeFormatterVisitor : LSLValidatorNodeVisitor<bool>
    {
        private readonly LinkedList<LSLComment> _comments = new LinkedList<LSLComment>();
        private readonly Stack<bool> _expressionWrappingStack = new Stack<bool>();

        private int _indentLevel;
        private string _sourceReference;



        class CodeWrappingContext 
        {
            public ILSLReadOnlySyntaxTreeNode Statement { get; set; }

            public int WriteColumn { get; set; }

            public int WriteLine { get; set; }

            public CodeWrappingContext(ILSLReadOnlySyntaxTreeNode statement, LSLCodeFormatterVisitor parent)
            {
                Statement = statement;
                WriteColumn = parent._writeColumn;
                WriteLine = parent._writeLine;
            }
        }


        private CodeWrappingContext _lastCodeWrappingContext;
        



        private int _writeColumn = 0;
        private int _writeLine = 0;


        public TextWriter Writer { get; private set; }

        private string GenIndent(int add = 0)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < (_indentLevel*4) + (add)*4; i++)
            {
                str.Append(" ");
            }
            return str.ToString();
        }

        private void Write(string str)
        {
            int lastNewLine = -1;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\n')
                {
                    lastNewLine = i;
                    _writeLine++;
                }
            }

            if (lastNewLine != -1)
            {
                _writeColumn = (str.Length - 1) - lastNewLine;
            }
            else
            {
                _writeColumn += str.Length;
            }

            Writer.Write(str);
        }

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

        public IList<LSLComment> CommentsBetweenRange(LSLSourceCodeRange left, LSLSourceCodeRange right)
        {
            return GetComments(left.StopIndex, right.StartIndex).ToList();
        }

        public bool WriteCommentsBetweenRange(LSLSourceCodeRange left, LSLSourceCodeRange right,
            int existingNewLinesBetweenNextNode = 0)
        {
            var comments = CommentsBetweenRange(left, right).ToList();
            return WriteCommentsBetweenRange(comments, left, right, existingNewLinesBetweenNextNode);
        }

        public bool WriteCommentsBetweenRange(IList<LSLComment> comments, LSLSourceCodeRange left,
            LSLSourceCodeRange right, int existingNewLinesBetweenNextNode = 0)
        {
            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - left.LineEnd;

                if (linesBetweenNodeAndFirstComment == 0)
                {
                    var spacesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.StartIndex - left.StopIndex) - 1;

                    spacesBetweenNodeAndFirstComment = spacesBetweenNodeAndFirstComment > 0
                        ? spacesBetweenNodeAndFirstComment
                        : 1;

                    while (spacesBetweenNodeAndFirstComment > 0)
                    {
                        Write(" ");
                        spacesBetweenNodeAndFirstComment--;
                    }
                }


                while (linesBetweenNodeAndFirstComment > 0)
                {
                    Write("\n");
                    linesBetweenNodeAndFirstComment--;
                }

                for (int j = 0; j < comments.Count; j++)
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
                        var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                   comment.SourceCodeRange.LineEnd;

                        if (linesBetweenComments == 0)
                        {
                            var spacesBetweenComments = (nextComment.SourceCodeRange.StartIndex -
                                                         comment.SourceCodeRange.StopIndex);

                            while (spacesBetweenComments > 0)
                            {
                                Write(" ");
                                spacesBetweenComments--;
                            }
                        }

                        while (linesBetweenComments > 0)
                        {
                            Write("\n");
                            linesBetweenComments--;
                        }
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = right.LineStart - comment.SourceCodeRange.LineEnd;

                        if (linesBetweenCommentAndNextNode == 0)
                        {
                            var spacesBetweenCommentAndNextNode = (right.StartIndex - comment.SourceCodeRange.StopIndex);

                            spacesBetweenCommentAndNextNode = spacesBetweenCommentAndNextNode > 0
                                ? spacesBetweenCommentAndNextNode
                                : 1;

                            while (spacesBetweenCommentAndNextNode > 0)
                            {
                                Write(" ");
                                spacesBetweenCommentAndNextNode--;
                            }
                        }
                        else
                        {
                            while (linesBetweenCommentAndNextNode > existingNewLinesBetweenNextNode)
                            {
                                Write("\n");
                                linesBetweenCommentAndNextNode--;
                            }

                            Write(GenIndent());
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public override bool VisitBinaryExpression(ILSLBinaryExpressionNode node)
        {

            bool t = ReferenceEquals(node.Parent, _lastCodeWrappingContext.Statement);
            if (node.Parent is ILSLVariableDeclarationNode && t)
            {

               _lastCodeWrappingContext = new CodeWrappingContext(node, this);
                
            }

            Visit(node.LeftExpression);




            if (!WriteCommentsBetweenRange(node.LeftExpression.SourceCodeRange, node.OperationSourceCodeRange))
            {
                Write(" ");
            }


            Write(node.OperationString);




            if(
                !(node.Parent is ILSLExpressionStatementNode) &&
                !(node.Parent is ILSLParenthesizedExpressionNode) &&

                ((_writeColumn - _lastCodeWrappingContext.WriteColumn) > 40))
            {
                var indentLevel = _lastCodeWrappingContext.WriteColumn - 1;
                Write("\n");
                while (indentLevel > 0)
                {
                    
                    Write(" ");

                    indentLevel--;
                }
            }


            if (!WriteCommentsBetweenRange(node.OperationSourceCodeRange, node.RightExpression.SourceCodeRange))
            {
                Write(" ");
            }



            if (node.Parent is ILSLExpressionStatementNode &&
                ReferenceEquals(node.Parent, _lastCodeWrappingContext.Statement))
            {
                if (node.Operation.IsAssignOrModifyAssign())
                {
                    _lastCodeWrappingContext = new CodeWrappingContext(node, this);
                }
            }


            Visit(node.RightExpression);

            return true;
        }

        public override bool VisitListLiteralInitializerList(ILSLExpressionListNode node)
        {
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

            return true;
        }

        public override bool VisitFunctionCallParameters(ILSLExpressionListNode node)
        {
            var cnt = node.ExpressionNodes.Count;
            var cntr = 0;


            if (((ILSLFunctionCallNode) node.Parent).Name == "chr")
            {
                Console.Write("TEST");
            }

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

            return true;
        }

        //public override bool VisitForLoopAfterthoughts(ILSLExpressionListNode node)
        //{
        //    throw new NotImplementedException();
        //}

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
            Write("(");

            WriteCommentsBetweenRange(node.SourceCodeRange.GetFirstCharRange(), node.InnerExpression.SourceCodeRange);


            Visit(node.InnerExpression);


            WriteCommentsBetweenRange(node.InnerExpression.SourceCodeRange, node.SourceCodeRange.GetLastCharRange());

            Write(")");

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

            Visit(node.ConditionExpression);

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

                Visit(node.InitExpressionsList);

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

                Visit(node.ConditionExpression);

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

                Visit(node.AfterthoughExpressions);

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

            WriteCommentsBetweenRange(node.OpenParenthSourceCodeRange, node.ConditionExpression.SourceCodeRange);

            Visit(node.ConditionExpression);

            WriteCommentsBetweenRange(node.ConditionExpression.SourceCodeRange, node.CloseParenthSourceCodeRange);

            Write(")");

            WriteCommentsBetweenRange(node.CloseParenthSourceCodeRange, node.Code.SourceCodeRange);

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

        public override bool VisitCompilationUnit(ILSLCompilationUnitNode snode)
        {
            var nodes = snode.GlobalVariableDeclarations.Concat<ILSLReadOnlySyntaxTreeNode>(snode.FunctionDeclarations)
                .Concat(new[] {snode.DefaultState})
                .Concat(snode.StateDeclarations).ToList();

            nodes.Sort((a, b) => a.SourceCodeRange.StartIndex.CompareTo(b.SourceCodeRange.StartIndex));


            if (nodes.Count > 0)
            {
                var comments = GetComments(0, nodes[0].SourceCodeRange.StartIndex).ToList();

                if (comments.Count > 0)
                {
                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                          snode.SourceCodeRange.LineStart;

                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];

                        Write(FormatComment("", comment));

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                       comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndNextNode = nodes[0].SourceCodeRange.LineStart -
                                                                 comment.SourceCodeRange.LineEnd;
                            while (linesBetweenCommentAndNextNode > 0)
                            {
                                Write("\n");
                                linesBetweenCommentAndNextNode--;
                            }
                        }
                    }
                }
                else
                {
                    var linesBetweenTwoNodes = nodes[0].SourceCodeRange.LineStart - snode.SourceCodeRange.LineStart;
                    while (linesBetweenTwoNodes > 1)
                    {
                        Write("\n");
                        linesBetweenTwoNodes--;
                    }
                }


                for (int i = 0; i < nodes.Count; i++)
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
                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                                  node.SourceCodeRange.LineEnd;

                            if (linesBetweenNodeAndFirstComment <= 2 && linesBetweenNodeAndFirstComment > 0)
                            {
                                if ((nextNode is ILSLFunctionDeclarationNode || nextNode is ILSLStateScopeNode)
                                    && (node is ILSLFunctionDeclarationNode || node is ILSLVariableDeclarationNode))
                                {
                                    linesBetweenNodeAndFirstComment = 3;
                                }
                            }

                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
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
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                               comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndNextNode = nextNode.SourceCodeRange.LineStart -
                                                                         comment.SourceCodeRange.LineEnd;


                                    linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                                      node.SourceCodeRange.LineEnd;

                                    if (linesBetweenNodeAndFirstComment == 0 && linesBetweenCommentAndNextNode < 3)
                                    {
                                        if ((nextNode is ILSLFunctionDeclarationNode || nextNode is ILSLStateScopeNode)
                                            &&
                                            (node is ILSLFunctionDeclarationNode || node is ILSLVariableDeclarationNode))
                                        {
                                            linesBetweenCommentAndNextNode = 3;
                                        }
                                    }

                                    while (linesBetweenCommentAndNextNode > 0)
                                    {
                                        Write("\n");
                                        linesBetweenCommentAndNextNode--;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var linesBetweenTwoNodes = nextNode.SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd;

                            if ((nextNode is ILSLFunctionDeclarationNode && node is ILSLFunctionDeclarationNode) ||
                                (nextNode is ILSLFunctionDeclarationNode && node is ILSLVariableDeclarationNode) ||
                                (nextNode is ILSLVariableDeclarationNode && node is ILSLFunctionDeclarationNode) ||
                                (nextNode is ILSLStateScopeNode && node is ILSLFunctionDeclarationNode) ||
                                (nextNode is ILSLStateScopeNode && node is ILSLVariableDeclarationNode) ||
                                (nextNode is ILSLStateScopeNode && node is ILSLStateScopeNode))
                            {
                                if (linesBetweenTwoNodes < 3) linesBetweenTwoNodes = 3;
                            }

                            while (linesBetweenTwoNodes > 0)
                            {
                                Write("\n");
                                linesBetweenTwoNodes--;
                            }
                        }
                    }
                    else
                    {
                        comments = GetComments(node.SourceCodeRange.StopIndex, _sourceReference.Length).ToList();

                        if (comments.Count > 0)
                        {
                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                                  node.SourceCodeRange.LineEnd;

                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
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
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                               comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndEndOfScope = snode.SourceCodeRange.LineEnd -
                                                                           comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenCommentAndEndOfScope > 0)
                                    {
                                        Write("\n");
                                        linesBetweenCommentAndEndOfScope--;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var linesBetweenNodeAndEndOfScope = snode.SourceCodeRange.LineEnd -
                                                                node.SourceCodeRange.LineEnd;
                            while (linesBetweenNodeAndEndOfScope > 0)
                            {
                                Write("\n");
                                linesBetweenNodeAndEndOfScope--;
                            }
                        }
                    }
                }
            }
            else
            {
                var comments = GetComments(snode.SourceCodeRange.StartIndex, snode.SourceCodeRange.StopIndex).ToList();

                if (comments.Count > 0)
                {
                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                          snode.SourceCodeRange.LineStart;

                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];
                        Write(FormatComment("", comment));

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                       comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndEndOfScope = snode.SourceCodeRange.LineEnd -
                                                                   comment.SourceCodeRange.LineEnd;
                            while (linesBetweenCommentAndEndOfScope > 0)
                            {
                                Write("\n");
                                linesBetweenCommentAndEndOfScope--;
                            }
                        }
                    }
                }
                else
                {
                    var linesBetweenNodeAndEndOfScope = snode.SourceCodeRange.LineEnd - snode.SourceCodeRange.LineStart;
                    while (linesBetweenNodeAndEndOfScope > 1)
                    {
                        Write("\n");
                        linesBetweenNodeAndEndOfScope--;
                    }
                }
            }


            return true;
        }

        /*public override bool VisitCodeScope(ILSLCodeScopeNode node)
        {
            Write("{");

            foreach (var n in node.CodeStatements)
            {
                Visit(n);
                Write(";\n");
            }

            Write("}");
            Visit(node.Code);

            return true;
        }*/

        //public override bool VisitCompilationUnit(ILSLCompilationUnitNode node)
        //{
        //    throw new NotImplementedException();
        //}

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
                var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                      node.ParameterListNode.SourceCodeRange.LineEnd;


                while (linesBetweenNodeAndFirstComment > 0)
                {
                    Write("\n");
                    linesBetweenNodeAndFirstComment--;
                }

                for (int j = 0; j < comments.Count; j++)
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
                        var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                   comment.SourceCodeRange.LineEnd;
                        while (linesBetweenComments > 0)
                        {
                            Write("\n");
                            linesBetweenComments--;
                        }
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = node.EventBodyNode.SourceCodeRange.LineStart -
                                                             comment.SourceCodeRange.LineEnd;
                        while (linesBetweenCommentAndNextNode > 1)
                        {
                            Write("\n");
                            linesBetweenCommentAndNextNode--;
                        }
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
                var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                      node.ParameterListNode.SourceCodeRange.LineEnd;

                while (linesBetweenNodeAndFirstComment > 0)
                {
                    Write("\n");
                    linesBetweenNodeAndFirstComment--;
                }

                for (int j = 0; j < comments.Count; j++)
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
                        var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                   comment.SourceCodeRange.LineEnd;
                        while (linesBetweenComments > 0)
                        {
                            Write("\n");
                            linesBetweenComments--;
                        }
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = node.FunctionBodyNode.SourceCodeRange.LineStart -
                                                             comment.SourceCodeRange.LineEnd;
                        while (linesBetweenCommentAndNextNode > 1)
                        {
                            Write("\n");
                            linesBetweenCommentAndNextNode--;
                        }
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
                Write("state " + snode.StateName + "");
            }

            if (!WriteCommentsBetweenRange(snode.StateNameSourceCodeRange, snode.OpenBraceSourceCodeRange))
            {
                Write("\n");
            }

            Write("{\n");

            _indentLevel++;

            var nodes = snode.EventHandlers;

            string indent = GenIndent();


            if (nodes.Count > 0)
            {
                var comments =
                    GetComments(snode.OpenBraceSourceCodeRange.StartIndex, nodes[0].SourceCodeRange.StartIndex).ToList();

                if (comments.Count > 0)
                {
                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                          snode.OpenBraceSourceCodeRange.LineStart;

                    linesBetweenNodeAndFirstComment = linesBetweenNodeAndFirstComment > 2
                        ? 2
                        : linesBetweenNodeAndFirstComment;

                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];

                        Write(indent + comment.Text);

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                       comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndNextNode = nodes[0].SourceCodeRange.LineStart -
                                                                 comment.SourceCodeRange.LineEnd;
                            while (linesBetweenCommentAndNextNode > 0)
                            {
                                Write("\n");
                                linesBetweenCommentAndNextNode--;
                            }
                        }
                    }
                }
                else
                {
                    var linesBetweenTwoNodes = nodes[0].SourceCodeRange.LineStart -
                                               snode.OpenBraceSourceCodeRange.LineStart;

                    linesBetweenTwoNodes = linesBetweenTwoNodes > 2 ? 2 : linesBetweenTwoNodes;

                    while (linesBetweenTwoNodes > 1)
                    {
                        Write("\n");
                        linesBetweenTwoNodes--;
                    }
                }


                for (int i = 0; i < nodes.Count; i++)
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
                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                                  node.SourceCodeRange.LineEnd;

                            if (linesBetweenNodeAndFirstComment <= 2)
                            {
                                linesBetweenNodeAndFirstComment = 3;
                            }

                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
                            {
                                var comment = comments[j];

                                Write(indent + comment.Text);

                                if ((j + 1) < comments.Count)
                                {
                                    var nextComment = comments[j + 1];
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                               comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndNextNode = nextNode.SourceCodeRange.LineStart -
                                                                         comment.SourceCodeRange.LineEnd;

                                    while (linesBetweenCommentAndNextNode > 0)
                                    {
                                        Write("\n");
                                        linesBetweenCommentAndNextNode--;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var linesBetweenTwoNodes = (nextNode.SourceCodeRange.LineStart -
                                                        node.SourceCodeRange.LineEnd);

                            if (linesBetweenTwoNodes < 2) linesBetweenTwoNodes = (2 - singleLineBroken);

                            while (linesBetweenTwoNodes > 0)
                            {
                                Write("\n");
                                linesBetweenTwoNodes--;
                            }
                        }
                    }
                    else
                    {
                        comments = GetComments(node.SourceCodeRange.StopIndex, snode.SourceCodeRange.StopIndex).ToList();

                        if (comments.Count > 0)
                        {
                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                                  node.SourceCodeRange.LineEnd;

                            if (linesBetweenNodeAndFirstComment == 0) linesBetweenNodeAndFirstComment = 1;

                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
                            {
                                var comment = comments[j];
                                Write(indent + comment.Text);

                                if ((j + 1) < comments.Count)
                                {
                                    var nextComment = comments[j + 1];
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                               comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndEndOfScope = snode.CloseBraceSourceCodeRange.LineEnd -
                                                                           comment.SourceCodeRange.LineEnd;

                                    linesBetweenCommentAndEndOfScope = linesBetweenCommentAndEndOfScope > 2
                                        ? 2
                                        : linesBetweenCommentAndEndOfScope;

                                    if (linesBetweenCommentAndEndOfScope == 0)
                                    {
                                        linesBetweenCommentAndEndOfScope = 1;
                                    }


                                    while (linesBetweenCommentAndEndOfScope > 0)
                                    {
                                        Write("\n");
                                        linesBetweenCommentAndEndOfScope--;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var linesBetweenNodeAndEndOfScope = snode.CloseBraceSourceCodeRange.LineEnd -
                                                                node.SourceCodeRange.LineEnd;


                            linesBetweenNodeAndEndOfScope = linesBetweenNodeAndEndOfScope > 2
                                ? 2
                                : linesBetweenNodeAndEndOfScope;

                            if (linesBetweenNodeAndEndOfScope == 0)
                            {
                                linesBetweenNodeAndEndOfScope = 1;
                            }

                            while (linesBetweenNodeAndEndOfScope > 0)
                            {
                                Write("\n");
                                linesBetweenNodeAndEndOfScope--;
                            }
                        }
                    }
                }
            }
            else
            {
                var comments =
                    GetComments(snode.OpenBraceSourceCodeRange.StartIndex, snode.CloseBraceSourceCodeRange.StopIndex)
                        .ToList();

                if (comments.Count > 0)
                {
                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                          snode.OpenBraceSourceCodeRange.LineStart;


                    linesBetweenNodeAndFirstComment = linesBetweenNodeAndFirstComment > 2
                        ? 2
                        : linesBetweenNodeAndFirstComment;

                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];
                        Write(indent + comment.Text);

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                       comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndEndOfScope = snode.CloseBraceSourceCodeRange.LineEnd -
                                                                   comment.SourceCodeRange.LineEnd;

                            linesBetweenCommentAndEndOfScope = linesBetweenCommentAndEndOfScope > 2
                                ? 2
                                : linesBetweenCommentAndEndOfScope;

                            if (linesBetweenCommentAndEndOfScope == 0)
                            {
                                linesBetweenCommentAndEndOfScope = 1;
                            }

                            while (linesBetweenCommentAndEndOfScope > 0)
                            {
                                Write("\n");
                                linesBetweenCommentAndEndOfScope--;
                            }
                        }
                    }
                }
                else
                {
                    var linesBetweenNodeAndEndOfScope = snode.CloseBraceSourceCodeRange.LineEnd -
                                                        snode.OpenBraceSourceCodeRange.LineStart;

                    linesBetweenNodeAndEndOfScope = linesBetweenNodeAndEndOfScope > 2
                        ? 2
                        : linesBetweenNodeAndEndOfScope;

                    while (linesBetweenNodeAndEndOfScope > 1)
                    {
                        Write("\n");
                        linesBetweenNodeAndEndOfScope--;
                    }
                }
            }

            _indentLevel--;

            Write("}");

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

                Visit(node.ReturnExpression);

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

            for (int i = 0; i < nodes.Count; i++)
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


            Visit(node.ConditionExpression);

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
            Write("\n" + GenIndent() + "else");

            if (!WriteCommentsBetweenRange(node.ElseKeywordSourceCodeRange, node.IfKeywordSourceCodeRange))
            {
                Write(" ");
            }

            Write("if");

            WriteCommentsBetweenRange(node.IfKeywordSourceCodeRange, node.OpenParenthSourceCodeRange);

            Write("(");

            WriteCommentsBetweenRange(node.OpenParenthSourceCodeRange, node.ConditionExpression.SourceCodeRange);


            Visit(node.ConditionExpression);

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
            Write("\n" + GenIndent() + "else");


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


            _lastCodeWrappingContext = new CodeWrappingContext(node, this);

            Visit(node.Expression);




            WriteCommentsBetweenRange(node.Expression.SourceCodeRange, node.SourceCodeRange);


            Write(";");

            return true;
        }

        public override bool VisitVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            
            _lastCodeWrappingContext = new CodeWrappingContext(node, this);


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

                if (!WriteCommentsBetweenRange(node.OperatorSourceCodeRange, node.DeclarationExpression.SourceCodeRange))
                {
                    Write(" ");
                }

                Visit(node.DeclarationExpression);


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

        /* public override bool VisitLocalVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            Write(node.TypeString + " " + node.Name);
            if (node.HasDeclarationExpression)
            {
                Write(" = ");
                Visit(node.DeclarationExpression);
            }
            Write(";");

            return true;
        }*/

        public override bool VisitSingleStatementCodeScope(ILSLCodeScopeNode node)
        {
            _indentLevel++;

            var statement = node.CodeStatements.First();

            if (!(statement is ILSLSemiColonStatement))
            {
                Write("\n" + GenIndent());
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


            var c = comment.Text.Trim('\t', '*', '/', ' ', '\n', '\r');

            bool multiLine = comment.Text.Contains('\n');

            if (!multiLine)
            {
                return indent + comment.Text;
            }


            var r = indent + "/*" + (multiLine ? "\n" + indent + "   " : " ");

            for (int i = 0; i < c.Length; i++)
            {
                var ch = c[i];
                if (ch == '\n')
                {
                    r += '\n';

                    i++;

                    while (c[i] == '\t' || c[i] == ' ')
                    {
                        i++;
                    }

                    ch = c[i];

                    if (ch != '\r')
                    {
                        r += indent + "   " + ch;
                    }
                }
                else
                {
                    r += ch;
                }
            }

            return r + (multiLine ? "\n" + indent : " ") + "*/";
        }

        public override bool VisitMultiStatementCodeScope(ILSLCodeScopeNode snode)
        {
            //if (snode.CodeScopeType == LSLCodeScopeType.FunctionCodeRoot || snode.CodeScopeType == LSLCodeScopeType.EventHandlerCodeRoot)
            //{
            Write("\n" + GenIndent() + "{\n");

            //}
            //else
            //{
            //    Write("{\n");
            //}

            _indentLevel++;

            var nodes = snode.CodeStatements.ToList();

            string indent = GenIndent();


            if (nodes.Count > 0)
            {
                var comments =
                    GetComments(snode.SourceCodeRange.StartIndex, nodes[0].SourceCodeRange.StartIndex).ToList();

                if (comments.Count > 0)
                {
                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                          snode.SourceCodeRange.LineStart;

                    linesBetweenNodeAndFirstComment = linesBetweenNodeAndFirstComment > 2
                        ? 2
                        : linesBetweenNodeAndFirstComment;

                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];


                        Write(FormatComment(indent, comment));

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                       comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndNextNode = nodes[0].SourceCodeRange.LineStart -
                                                                 comment.SourceCodeRange.LineEnd;

                            if (linesBetweenCommentAndNextNode == 0) linesBetweenCommentAndNextNode = 1;

                            while (linesBetweenCommentAndNextNode > 0)
                            {
                                Write("\n");
                                linesBetweenCommentAndNextNode--;
                            }
                        }
                    }
                }
                else
                {
                    var linesBetweenTwoNodes = nodes[0].SourceCodeRange.LineStart - snode.SourceCodeRange.LineStart;

                    linesBetweenTwoNodes = linesBetweenTwoNodes > 2 ? 2 : linesBetweenTwoNodes;

                    while (linesBetweenTwoNodes > 1)
                    {
                        Write("\n");
                        linesBetweenTwoNodes--;
                    }
                }


                for (int i = 0; i < nodes.Count; i++)
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
                        }


                        comments =
                            GetComments(node.SourceCodeRange.StopIndex, nextNode.SourceCodeRange.StartIndex).ToList();

                        if (comments.Count > 0)
                        {
                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                                  node.SourceCodeRange.LineEnd;


                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
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
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                               comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndNextNode = nextNode.SourceCodeRange.LineStart -
                                                                         comment.SourceCodeRange.LineEnd +
                                                                         singleLineBroken;
                                    var linesBetweenCommentAndLastNode = node.SourceCodeRange.LineEnd -
                                                                         comment.SourceCodeRange.LineStart;

                                    if (linesBetweenCommentAndLastNode == 0 && linesBetweenCommentAndNextNode == 1 &&
                                        node is ILSLControlStatementNode)
                                    {
                                        linesBetweenCommentAndNextNode++;
                                    }

                                    while (linesBetweenCommentAndNextNode > 0)
                                    {
                                        Write("\n");
                                        linesBetweenCommentAndNextNode--;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var linesBetweenTwoNodes = (nextNode.SourceCodeRange.LineStart -
                                                        node.SourceCodeRange.LineEnd) + singleLineBroken;

                            if ((nextNode is ILSLVariableDeclarationNode ||
                                 nextNode is ILSLControlStatementNode ||
                                 nextNode is ILSLExpressionStatementNode ||
                                 nextNode is ILSLLoopNode ||
                                 nextNode is ILSLReturnStatementNode ||
                                 nextNode is ILSLStateChangeStatementNode ||
                                 nextNode is ILSLJumpStatementNode ||
                                 nextNode is ILSLLabelStatementNode)
                                &&
                                (node is ILSLVariableDeclarationNode ||
                                 node is ILSLControlStatementNode ||
                                 node is ILSLExpressionStatementNode ||
                                 node is ILSLLoopNode ||
                                 node is ILSLReturnStatementNode ||
                                 node is ILSLStateChangeStatementNode ||
                                 node is ILSLJumpStatementNode ||
                                 node is ILSLLabelStatementNode))

                            {
                                if (linesBetweenTwoNodes < 2 && !(
                                    (nextNode is ILSLVariableDeclarationNode && node is ILSLVariableDeclarationNode) ||
                                    (nextNode is ILSLExpressionStatementNode && node is ILSLExpressionStatementNode)
                                    ))
                                {
                                    linesBetweenTwoNodes = 2;
                                }
                            }


                            while (linesBetweenTwoNodes > 0)
                            {
                                Write("\n");
                                linesBetweenTwoNodes--;
                            }
                        }
                    }
                    else
                    {
                        comments = GetComments(node.SourceCodeRange.StopIndex, snode.SourceCodeRange.StopIndex).ToList();

                        if (comments.Count > 0)
                        {
                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                                  node.SourceCodeRange.LineEnd;


                            if (linesBetweenNodeAndFirstComment == 0) linesBetweenNodeAndFirstComment = 1;

                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
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
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                               comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndEndOfScope = snode.SourceCodeRange.LineEnd -
                                                                           comment.SourceCodeRange.LineEnd;

                                    linesBetweenCommentAndEndOfScope = linesBetweenCommentAndEndOfScope > 2
                                        ? 2
                                        : linesBetweenCommentAndEndOfScope;

                                    if (linesBetweenCommentAndEndOfScope == 0)
                                    {
                                        linesBetweenCommentAndEndOfScope = 1;
                                    }

                                    while (linesBetweenCommentAndEndOfScope > 0)
                                    {
                                        Write("\n");
                                        linesBetweenCommentAndEndOfScope--;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var linesBetweenNodeAndEndOfScope = snode.SourceCodeRange.LineEnd -
                                                                node.SourceCodeRange.LineEnd;

                            linesBetweenNodeAndEndOfScope = linesBetweenNodeAndEndOfScope > 2
                                ? 2
                                : linesBetweenNodeAndEndOfScope;


                            if (linesBetweenNodeAndEndOfScope == 0)
                            {
                                linesBetweenNodeAndEndOfScope = 1;
                            }




                            while (linesBetweenNodeAndEndOfScope > 0)
                            {
                                Write("\n");
                                linesBetweenNodeAndEndOfScope--;
                            }
                        }
                    }
                }
            }
            else
            {
                var comments = GetComments(snode.SourceCodeRange.StartIndex, snode.SourceCodeRange.StopIndex).ToList();

                if (comments.Count > 0)
                {
                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart -
                                                          snode.SourceCodeRange.LineStart;

                    linesBetweenNodeAndFirstComment = linesBetweenNodeAndFirstComment > 2
                        ? 2
                        : linesBetweenNodeAndFirstComment;

                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];


                        Write(FormatComment(indent, comment));

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart -
                                                       comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndEndOfScope = snode.SourceCodeRange.LineEnd -
                                                                   comment.SourceCodeRange.LineEnd;

                            linesBetweenCommentAndEndOfScope = linesBetweenCommentAndEndOfScope > 2
                                ? 2
                                : linesBetweenCommentAndEndOfScope;

                            if (linesBetweenCommentAndEndOfScope == 0)
                            {
                                linesBetweenCommentAndEndOfScope = 1;
                            }
                            while (linesBetweenCommentAndEndOfScope > 0)
                            {
                                Write("\n");
                                linesBetweenCommentAndEndOfScope--;
                            }
                        }
                    }
                }
                else
                {
                    var linesBetweenNodeAndEndOfScope = snode.SourceCodeRange.LineEnd - snode.SourceCodeRange.LineStart;

                    linesBetweenNodeAndEndOfScope = linesBetweenNodeAndEndOfScope > 2
                        ? 2
                        : linesBetweenNodeAndEndOfScope;

                    while (linesBetweenNodeAndEndOfScope > 1)
                    {
                        Write("\n");
                        linesBetweenNodeAndEndOfScope--;
                    }
                }
            }


            _indentLevel--;

            Write(GenIndent() + "}");

            return true;
        }

        public override bool VisitHexLiteral(ILSLHexLiteralNode lslHexLiteralNode)
        {
            Write(lslHexLiteralNode.RawText);

            return true;
        }

        //public override bool VisitLibraryFunctionCallParameters(ILSLExpressionListNode lslExpressionListNode)
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool VisitUserFunctionCallParameters(ILSLExpressionListNode lslExpressionListNode)
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool VisitGlobalVariableReference(ILSLVariableNode lslVariableNode)
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool VisitLocalVariableReference(ILSLVariableNode lslVariableNode)
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool VisitParameterVariableReference(ILSLVariableNode lslVariableNode)
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool VisitLibraryConstantVariableReference(ILSLVariableNode lslVariableNode)
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool VisitLibraryFunctionCall(ILSLFunctionCallNode lslFunctionCallNode)
        //{
        //    throw new NotImplementedException();
        //}

        // public override bool VisitUserFunctionCall(ILSLFunctionCallNode lslFunctionCallNode)
        //{
        //    throw new NotImplementedException();
        //}

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
    }
}