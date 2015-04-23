using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.Formatter.Visitor
{
    public class LSLCodeFormatterVisitor : LSLValidatorNodeVisitor<bool>
    {
        private LinkedList<LSLComment> comments = new LinkedList<LSLComment>();
        private ILSLCompilationUnitNode compilationUnitNode;
        private int indentLevel = 0;


        private string genIndent(int add=0)
        {

            StringBuilder str = new StringBuilder();
            for (int i = 0; i < indentLevel+add; i++)
            {
                str.Append("\t");
            }
            return str.ToString();

        }



        public TextWriter Writer { get; private set; }



        public void WriteAndFlush(ILSLCompilationUnitNode node, TextWriter writer, bool closeStream = true)
        {
            compilationUnitNode = node;

            foreach (var comment in node.Comments)
            {
                comments.AddLast(comment);
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
        public bool WriteCommentsBetweenRange(LSLSourceCodeRange left, LSLSourceCodeRange right, int existingNewLinesBetweenNextNode = 0)
        {
            var comments = CommentsBetweenRange(left, right).ToList();
            return WriteCommentsBetweenRange(comments, left, right, existingNewLinesBetweenNextNode);
        }
        public bool WriteCommentsBetweenRange(IList<LSLComment> comments, LSLSourceCodeRange left, LSLSourceCodeRange right, int existingNewLinesBetweenNextNode = 0)
        {

            

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - left.LineEnd;

                if (linesBetweenNodeAndFirstComment == 0)
                {
                    var spacesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.StartIndex - left.StopIndex) - 1;

                    spacesBetweenNodeAndFirstComment = spacesBetweenNodeAndFirstComment > 0 ? spacesBetweenNodeAndFirstComment : 1;

                    while (spacesBetweenNodeAndFirstComment > 0)
                    {
                        Writer.Write(" ");
                        spacesBetweenNodeAndFirstComment--;
                    }
                }


                while (linesBetweenNodeAndFirstComment > 0)
                {
                    Writer.Write("\n");
                    linesBetweenNodeAndFirstComment--;
                }

                for (int j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    if (comment.SourceCodeRange.LineStart != left.LineEnd)
                    {
                        Writer.Write(genIndent() + comment.Text);
                    }
                    else
                    {
                        Writer.Write(comment.Text);
                    }

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
                        var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;

                        if (linesBetweenComments == 0)
                        {
                            var spacesBetweenComments = (nextComment.SourceCodeRange.StartIndex - comment.SourceCodeRange.StopIndex) - 1;

                            while (spacesBetweenComments > 0)
                            {
                                Writer.Write(" ");
                                spacesBetweenComments--;
                            }
                        }

                        while (linesBetweenComments > 0)
                        {
                            Writer.Write("\n");
                            linesBetweenComments--;
                        }
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = right.LineStart - comment.SourceCodeRange.LineEnd;

                        if (linesBetweenCommentAndNextNode == 0)
                        {
                            var spacesBetweenCommentAndNextNode = (right.StartIndex - comment.SourceCodeRange.StopIndex);

                            spacesBetweenCommentAndNextNode = spacesBetweenCommentAndNextNode > 0 ? spacesBetweenCommentAndNextNode : 1;

                            while (spacesBetweenCommentAndNextNode > 0)
                            {
                                Writer.Write(" ");
                                spacesBetweenCommentAndNextNode--;
                            }
                        }
                        else
                        {
                            while (linesBetweenCommentAndNextNode > existingNewLinesBetweenNextNode)
                            {
                                Writer.Write("\n");
                                linesBetweenCommentAndNextNode--;
                            }

                            Writer.Write(genIndent());
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public override bool VisitBinaryExpression(ILSLBinaryExpressionNode node)
        {
            Visit(node.LeftExpression);

            

            if (!WriteCommentsBetweenRange(node.LeftExpression.SourceCodeRange, node.OperationSourceCodeRange))
            {
                Writer.Write(" ");
            }

            Writer.Write(node.OperationString);


            if (!WriteCommentsBetweenRange(node.OperationSourceCodeRange, node.RightExpression.SourceCodeRange))
            {
                Writer.Write(" ");
            }
            
            Visit(node.RightExpression);

            return true;
        }

        public override bool VisitListLiteralInitializerList(ILSLExpressionListNode node)
        {
            var cnt = node.ExpressionNodes.Count;
            var cntr = 0;
            for (var x = 0; x < cnt; x++)
            {
                Visit(node.ExpressionNodes[x]);
                if (cntr != (cnt - 1))
                {
                    Writer.Write(", ");
                }

                cntr++;
            }

            return true;
        }

        public override bool VisitFunctionCallParameters(ILSLExpressionListNode node)
        {
            var cnt = node.ExpressionNodes.Count;
            var cntr = 0;
            for(var x = 0; x < cnt; x++)
            {
                Visit(node.ExpressionNodes[x]);
                if (cntr != (cnt-1))
                {
                    Writer.Write(", ");    
                }

                cntr++;
            }

            return true;
        }

        //public override bool VisitForLoopAfterthoughts(ILSLExpressionListNode node)
        //{
        //    throw new NotImplementedException();
        //}

        public override bool VisitFloatLiteral(ILSLFloatLiteralNode node)
        {
            Writer.Write(node.RawText);

            return true;
        }

        public override bool VisitFunctionCall(ILSLFunctionCallNode node)
        {
            Writer.Write(node.Name+"(");
            Visit(node.ParameterListNode);
            Writer.Write(")");

            return true;
        }

        public override bool VisitIntegerLiteral(ILSLIntegerLiteralNode node)
        {
            Writer.Write(node.RawText);

            return true;
        }

        public override bool VisitListLiteral(ILSLListLiteralNode node)
        {
            Writer.Write("[");

            Visit(node.ExpressionListNode);

            Writer.Write("]");

            return true;
        }

        public override bool VisitParenthesizedExpression(ILSLParenthesizedExpressionNode node)
        {
            Writer.Write("(");

            WriteCommentsBetweenRange(node.SourceCodeRange.GetFirstCharRange(), node.InnerExpression.SourceCodeRange);
           
            Visit(node.InnerExpression);

            WriteCommentsBetweenRange(node.InnerExpression.SourceCodeRange, node.SourceCodeRange.GetLastCharRange());

            Writer.Write(")");

            return true;
        }

        public override bool VisitPostfixOperation(ILSLPostfixOperationNode node)
        {
            Visit(node.LeftExpression);


            WriteCommentsBetweenRange(node.LeftExpression.SourceCodeRange, node.OperationSourceCodeRange);

            Writer.Write(node.OperationString);

            return true;
        }

        public override bool VisitPrefixOperation(ILSLPrefixOperationNode node)
        {
            Writer.Write(node.OperationString);

            WriteCommentsBetweenRange(node.OperationSourceCodeRange, node.RightExpression.SourceCodeRange);

            Visit(node.RightExpression);
            return true;
        }

        public override bool VisitRotationLiteral(ILSLRotationLiteralNode node)
        {
            Writer.Write("<");
            bool comments;

            comments=WriteCommentsBetweenRange(node.SourceCodeRange.GetFirstCharRange(), node.XExpression.SourceCodeRange);

            Visit(node.XExpression);

            comments = WriteCommentsBetweenRange(node.XExpression.SourceCodeRange, node.CommaOneSourceCodeRange);

            Writer.Write(",");

            comments = WriteCommentsBetweenRange(node.CommaOneSourceCodeRange, node.YExpression.SourceCodeRange);
            if (!comments)
            {
                Writer.Write(" ");
            }


            Visit(node.YExpression);

            comments = WriteCommentsBetweenRange(node.YExpression.SourceCodeRange, node.CommaTwoSourceCodeRange);

            Writer.Write(",");

            comments = WriteCommentsBetweenRange(node.CommaTwoSourceCodeRange, node.ZExpression.SourceCodeRange);
            if (!comments)
            {
                Writer.Write(" ");
            }


            Visit(node.ZExpression);

            comments = WriteCommentsBetweenRange(node.ZExpression.SourceCodeRange, node.CommaThreeSourceCodeRange);

            Writer.Write(",");

            comments = WriteCommentsBetweenRange(node.CommaThreeSourceCodeRange, node.SExpression.SourceCodeRange);
            if (!comments)
            {
                Writer.Write(" ");
            }

            Visit(node.SExpression);

            comments = WriteCommentsBetweenRange(node.SExpression.SourceCodeRange, node.SourceCodeRange.GetLastCharRange());

            Writer.Write(">");

            return true;
        }

        public override bool VisitStringLiteral(ILSLStringLiteralNode node)
        {
            Writer.Write(node.RawText);
            return true;

        }

        public override bool VisitTypecastExpression(ILSLTypecastExprNode node)
        {
            Writer.Write("(" + node.CastToTypeString + ")");

            Visit(node.CastedExpression);
            return true;
        }

        public override bool VisitVariableReference(ILSLVariableNode node)
        {
            Writer.Write(node.Name);

            return true;
        }

        public override bool VisitVecRotAccessor(ILSLTupleAccessorNode node)
        {
            Visit(node.AccessedExpression);
            Writer.Write("."+node.AccessedComponentString);

            return true;
        }

        public override bool VisitVectorLiteral(ILSLVectorLiteralNode node)
        {
            Writer.Write("<");
            bool comments;

            comments = WriteCommentsBetweenRange(node.SourceCodeRange.GetFirstCharRange(), node.XExpression.SourceCodeRange);

            Visit(node.XExpression);

            comments = WriteCommentsBetweenRange(node.XExpression.SourceCodeRange, node.CommaOneSourceCodeRange);

            Writer.Write(",");

            comments = WriteCommentsBetweenRange(node.CommaOneSourceCodeRange, node.YExpression.SourceCodeRange);
            if (!comments)
            {
                Writer.Write(" ");
            }


            Visit(node.YExpression);

            comments = WriteCommentsBetweenRange(node.YExpression.SourceCodeRange, node.CommaTwoSourceCodeRange);

            Writer.Write(",");

            comments = WriteCommentsBetweenRange(node.CommaTwoSourceCodeRange, node.ZExpression.SourceCodeRange);
            if (!comments)
            {
                Writer.Write(" ");
            }

            Visit(node.ZExpression);

            comments = WriteCommentsBetweenRange(node.ZExpression.SourceCodeRange, node.SourceCodeRange.GetLastCharRange());

            Writer.Write(">");



            return true;
        }

        public override bool VisitDoLoop(ILSLDoLoopNode node)
        {
            Writer.Write("do");
            Visit(node.Code);
            Writer.Write("while(");
            Visit(node.ConditionExpression);
            Writer.Write(");");

            return true;
        }

        public override bool VisitForLoop(ILSLForLoopNode node)
        {
            Writer.Write("for");


            WriteCommentsBetweenRange(node.ForKeywordSourceCodeRange, node.OpenParenthSourceCodeRange);

            Writer.Write("(");



            if (node.HasInitExpression) {

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

                var comments = this.CommentsBetweenRange(node.FirstSemiColonSourceCodeRange, node.ConditionExpression.SourceCodeRange);

                if (comments.Count > 0)
                {
                    Writer.Write(";");
                }
                else
                {
                    Writer.Write("; ");
                }

                WriteCommentsBetweenRange(comments, node.FirstSemiColonSourceCodeRange, node.ConditionExpression.SourceCodeRange);

                Visit(node.ConditionExpression);

                WriteCommentsBetweenRange(node.ConditionExpression.SourceCodeRange, node.SecondSemiColonSourceCodeRange);
            }
            else
            {
                Writer.Write(";");
                WriteCommentsBetweenRange(node.FirstSemiColonSourceCodeRange, node.SecondSemiColonSourceCodeRange);
            }






            if (node.HasAfterthoughtExpressions)
            {

                var comments = this.CommentsBetweenRange(node.SecondSemiColonSourceCodeRange, node.AfterthoughExpressions.SourceCodeRange);

                if (comments.Count > 0)
                {
                    Writer.Write(";");
                }
                else
                {
                    Writer.Write("; ");
                }

                WriteCommentsBetweenRange(comments, node.SecondSemiColonSourceCodeRange, node.AfterthoughExpressions.SourceCodeRange);

                Visit(node.AfterthoughExpressions);

                WriteCommentsBetweenRange(node.AfterthoughExpressions.SourceCodeRange, node.CloseParenthSourceCodeRange);
            }
            else
            {
                WriteCommentsBetweenRange(node.SecondSemiColonSourceCodeRange, node.CloseParenthSourceCodeRange);
            }


            Writer.Write(")");
            Visit(node.Code);

            return true;
            
        }

        public override bool VisitWhileLoop(ILSLWhileLoopNode node)
        {
            Writer.Write("while(");
            Visit(node.ConditionExpression);
            Writer.Write(")");
            Visit(node.Code);

            return true;
        }
        public IEnumerable<LSLComment> GetComments(int sourceRangeStart, int sourceRangeEnd, bool remove = false)
        {
            var first = comments.First;

            while (first != null)
            {
                var next = first.Next;
                var comment = first.Value;

                if (comment.SourceCodeRange.StartIndex >= sourceRangeStart &&  comment.SourceCodeRange.StopIndex <= sourceRangeEnd)
                {
                        yield return comment;
                        if (remove)
                        {
                            comments.Remove(first);
                        }
                }
                first = next;
            }
        }

        public override bool VisitCompilationUnit(ILSLCompilationUnitNode snode)
        {

            var nodes = snode.GlobalVariableDeclarations.Concat<ILSLReadOnlySyntaxTreeNode>(snode.FunctionDeclarations)
                .Concat(new []{snode.DefaultState})
                .Concat(snode.StateDeclarations).ToList();

            nodes.Sort((a, b) => a.SourceCodeRange.StartIndex.CompareTo(b.SourceCodeRange.StartIndex));


            if (nodes.Count > 0)
            {
                var comments = GetComments(0, nodes[0].SourceCodeRange.StartIndex).ToList();

                if (comments.Count > 0)
                {
                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - snode.SourceCodeRange.LineStart;

                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Writer.Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];

                        Writer.Write(comment.Text);

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndNextNode = nodes[0].SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                            while (linesBetweenCommentAndNextNode > 0)
                            {
                                Writer.Write("\n");
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
                        Writer.Write("\n");
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
                        comments = GetComments(node.SourceCodeRange.StopIndex, nextNode.SourceCodeRange.StartIndex).ToList();

                        if (comments.Count > 0)
                        {
                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd;

                            if (linesBetweenNodeAndFirstComment <= 2)
                            {
                                if (nextNode is ILSLFunctionDeclarationNode && (node is ILSLFunctionDeclarationNode || node is ILSLVariableDeclarationNode))
                                {
                                    linesBetweenNodeAndFirstComment = 3;
                                }
                            }

                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
                            {
                                var comment = comments[j];





                                Writer.Write(comment.Text);

                                if ((j + 1) < comments.Count)
                                {
                                    var nextComment = comments[j + 1];
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Writer.Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndNextNode = nextNode.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenCommentAndNextNode > 0)
                                    {
                                        Writer.Write("\n");
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
                                Writer.Write("\n");
                                linesBetweenTwoNodes--;
                            }
                        }
                    }
                    else
                    {
                        comments = GetComments(node.SourceCodeRange.StopIndex, snode.SourceCodeRange.StopIndex).ToList();

                        if (comments.Count > 0)
                        {

                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd;

                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
                            {
                                var comment = comments[j];
                                Writer.Write(comment.Text);

                                if ((j + 1) < comments.Count)
                                {
                                    var nextComment = comments[j + 1];
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Writer.Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndEndOfScope = snode.SourceCodeRange.LineEnd - comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenCommentAndEndOfScope > 0)
                                    {
                                        Writer.Write("\n");
                                        linesBetweenCommentAndEndOfScope--;
                                    }
                                }
                            }


                        }
                        else
                        {
                            var linesBetweenNodeAndEndOfScope = snode.SourceCodeRange.LineEnd - node.SourceCodeRange.LineEnd;
                            while (linesBetweenNodeAndEndOfScope > 0)
                            {
                                Writer.Write("\n");
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

                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - snode.SourceCodeRange.LineStart;

                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Writer.Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];
                        Writer.Write(comment.Text);

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndEndOfScope = snode.SourceCodeRange.LineEnd - comment.SourceCodeRange.LineEnd;
                            while (linesBetweenCommentAndEndOfScope > 0)
                            {
                                Writer.Write("\n");
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
                        Writer.Write("\n");
                        linesBetweenNodeAndEndOfScope--;
                    }
                }

            }
            

            return true;
        }

        /*public override bool VisitCodeScope(ILSLCodeScopeNode node)
        {
            Writer.Write("{");

            foreach (var n in node.CodeStatements)
            {
                Visit(n);
                Writer.Write(";\n");
            }

            Writer.Write("}");
            Visit(node.Code);

            return true;
        }*/

        //public override bool VisitCompilationUnit(ILSLCompilationUnitNode node)
        //{
        //    throw new NotImplementedException();
        //}

        public override bool VisitEventHandler(ILSLEventHandlerNode node)
        {
            Writer.Write(node.Name+"(");
            Visit(node.ParameterListNode);
            Writer.Write(")");

            var comments = GetComments(node.ParameterListNode.SourceCodeRange.StopIndex, node.EventBodyNode.SourceCodeRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.ParameterListNode.SourceCodeRange.LineEnd;



                while (linesBetweenNodeAndFirstComment > 0)
                {
                    Writer.Write("\n");
                    linesBetweenNodeAndFirstComment--;
                }

                for (int j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    if (comment.SourceCodeRange.LineStart != node.ParameterListNode.SourceCodeRange.LineEnd)
                    {
                        Writer.Write(genIndent() + comment.Text);
                    }
                    else
                    {
                        Writer.Write(" "+comment.Text);
                    }

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
                        var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                        while (linesBetweenComments > 0)
                        {
                            Writer.Write("\n");
                            linesBetweenComments--;
                        }
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = node.EventBodyNode.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                        while (linesBetweenCommentAndNextNode > 1)
                        {
                            Writer.Write("\n");
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
            if (node.ReturnType != CodeValidator.Enums.LSLType.Void)
            {
                Writer.Write(node.ReturnTypeString + " " + node.Name + "(");
            }
            else
            {
                Writer.Write(node.Name + "(");
            }

            Visit(node.ParameterListNode);
            Writer.Write(")");


            var comments = GetComments(node.ParameterListNode.SourceCodeRange.StopIndex, node.FunctionBodyNode.SourceCodeRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.ParameterListNode.SourceCodeRange.LineEnd;

                while (linesBetweenNodeAndFirstComment > 0)
                {
                    Writer.Write("\n");
                    linesBetweenNodeAndFirstComment--;
                }

                for (int j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    if (comment.SourceCodeRange.LineStart != node.ParameterListNode.SourceCodeRange.LineEnd)
                    {
                        Writer.Write(genIndent() + comment.Text);
                    }
                    else
                    {
                        Writer.Write(" " + comment.Text);
                    }

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
                        var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                        while (linesBetweenComments > 0)
                        {
                            Writer.Write("\n");
                            linesBetweenComments--;
                        }
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = node.FunctionBodyNode.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                        while (linesBetweenCommentAndNextNode > 1)
                        {
                            Writer.Write("\n");
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
                Writer.Write("default");
            }
            else
            {
                Writer.Write("state "+snode.StateName+"");
            }

            if (!WriteCommentsBetweenRange(snode.StateNameSourceCodeRange, snode.OpenBraceSourceCodeRange))
            {
                Writer.Write("\n");
            }

            Writer.Write("{\n");

            indentLevel++;

            var nodes = snode.EventHandlers;

            string indent = genIndent();


            if (nodes.Count > 0)
            {
                var comments = GetComments(snode.OpenBraceSourceCodeRange.StartIndex, nodes[0].SourceCodeRange.StartIndex).ToList();

                if (comments.Count > 0)
                {
                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - snode.OpenBraceSourceCodeRange.LineStart;


                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Writer.Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];

                        Writer.Write(indent + comment.Text);

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndNextNode = nodes[0].SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                            while (linesBetweenCommentAndNextNode > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenCommentAndNextNode--;
                            }
                        }
                    }
                }
                else
                {
                    var linesBetweenTwoNodes = nodes[0].SourceCodeRange.LineStart - snode.OpenBraceSourceCodeRange.LineStart;
                    while (linesBetweenTwoNodes > 1)
                    {
                        Writer.Write("\n");
                        linesBetweenTwoNodes--;
                    }
                }




                for (int i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];

                    var singleLineBroken = 0;

                    Writer.Write(indent);
                    Visit(node);

                    if ((i + 1) < nodes.Count)
                    {
                        var nextNode = nodes[i + 1];

                        if (node.SourceCodeRange.LineStart == nextNode.SourceCodeRange.LineStart)
                        {
                            singleLineBroken = 1;
                            Writer.Write("\n");
                        }


                        comments = GetComments(node.SourceCodeRange.StopIndex, nextNode.SourceCodeRange.StartIndex).ToList();

                        if (comments.Count > 0)
                        {
                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd;

                            if (linesBetweenNodeAndFirstComment <= 2)
                            {
                                linesBetweenNodeAndFirstComment = 3;
                            }

                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
                            {
                                var comment = comments[j];

                                Writer.Write(indent + comment.Text);

                                if ((j + 1) < comments.Count)
                                {
                                    var nextComment = comments[j + 1];
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Writer.Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndNextNode = nextNode.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;

                                    while (linesBetweenCommentAndNextNode > 0)
                                    {
                                        Writer.Write("\n");
                                        linesBetweenCommentAndNextNode--;
                                    }
                                }
                            }
                        }
                        else
                        {

                            var linesBetweenTwoNodes = (nextNode.SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd);

                            if (linesBetweenTwoNodes < 2) linesBetweenTwoNodes = (2 - singleLineBroken);

                            while (linesBetweenTwoNodes > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenTwoNodes--;
                            }

                        }
                    }
                    else
                    {
                        comments = GetComments(node.SourceCodeRange.StopIndex, snode.SourceCodeRange.StopIndex).ToList();

                        if (comments.Count > 0)
                        {

                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd;

                            if (linesBetweenNodeAndFirstComment == 0) indent = " ";

                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
                            {
                                var comment = comments[j];
                                Writer.Write(indent + comment.Text);

                                if ((j + 1) < comments.Count)
                                {
                                    var nextComment = comments[j + 1];
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Writer.Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndEndOfScope = snode.CloseBraceSourceCodeRange.LineEnd - comment.SourceCodeRange.LineEnd;
                                    if (linesBetweenCommentAndEndOfScope == 0)
                                    {
                                        linesBetweenCommentAndEndOfScope = 1;
                                    }
                                    while (linesBetweenCommentAndEndOfScope > 0)
                                    {
                                        Writer.Write("\n");
                                        linesBetweenCommentAndEndOfScope--;
                                    }
                                }
                            }


                        }
                        else
                        {
                            var linesBetweenNodeAndEndOfScope = snode.CloseBraceSourceCodeRange.LineEnd - node.SourceCodeRange.LineEnd;
                            if (linesBetweenNodeAndEndOfScope == 0)
                            {
                                linesBetweenNodeAndEndOfScope = 1;
                            }
                            while (linesBetweenNodeAndEndOfScope > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenNodeAndEndOfScope--;
                            }
                        }
                    }
                }
            }
            else
            {
                var comments = GetComments(snode.OpenBraceSourceCodeRange.StartIndex, snode.CloseBraceSourceCodeRange.StopIndex).ToList();

                if (comments.Count > 0)
                {

                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - snode.OpenBraceSourceCodeRange.LineStart;

                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Writer.Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];
                        Writer.Write(indent + comment.Text);

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndEndOfScope = snode.CloseBraceSourceCodeRange.LineEnd - comment.SourceCodeRange.LineEnd;
                            if (linesBetweenCommentAndEndOfScope == 0)
                            {
                                linesBetweenCommentAndEndOfScope = 1;
                            }
                            while (linesBetweenCommentAndEndOfScope > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenCommentAndEndOfScope--;
                            }
                        }
                    }


                }
                else
                {
                    var linesBetweenNodeAndEndOfScope = snode.CloseBraceSourceCodeRange.LineEnd - snode.OpenBraceSourceCodeRange.LineStart;
                    while (linesBetweenNodeAndEndOfScope > 1)
                    {
                        Writer.Write("\n");
                        linesBetweenNodeAndEndOfScope--;
                    }
                }

            }

            indentLevel--;

            Writer.Write("}");

            return true;
        }


        public override bool VisitParameterDefinition(ILSLParameterNode node)
        {
            Writer.Write(node.TypeString+" "+node.Name);

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
                    Writer.Write(", ");
                }

                cntr++;
            }

            return true;
        }

        public override bool VisitReturnStatement(ILSLReturnStatementNode node)
        {
            if (node.HasReturnExpression)
            {
                Writer.Write("return ");
                Visit(node.ReturnExpression);
                Writer.Write(";");
            }
            else
            {
                Writer.Write("return;");
            }
            return true;
        }

        public override bool VisitSemiColonStatement(ILSLSemiColonStatement node)
        {
            Writer.Write(";");
          
            return true;
        }

        public override bool VisitStateChangeStatement(ILSLStateChangeStatementNode node)
        {
            Writer.Write("state "+node.StateTargetName +";");
            return true;
        }

        public override bool VisitJumpStatement(ILSLJumpStatementNode node)
        {
            Writer.Write("jump " + node.LabelName + ";");

            return true;
        }

        public override bool VisitLabelStatement(ILSLLabelStatementNode node)
        {
            Writer.Write("@" + node.LabelName + ";");

            return true;
        }

        public override bool VisitControlStatement(ILSLControlStatementNode snode)
        {
            IEnumerable<ILSLReadOnlySyntaxTreeNode> nodese = (new ILSLReadOnlySyntaxTreeNode[] { snode.IfStatement });

            if (snode.HasElseIfStatements)
            {
                nodese = nodese.Concat<ILSLReadOnlySyntaxTreeNode>(snode.ElseIfStatements);
            }

            if (snode.HasElseStatement)
            {
                nodese = nodese.Concat<ILSLReadOnlySyntaxTreeNode>(new ILSLReadOnlySyntaxTreeNode[] { snode.ElseStatement }).ToList();
            }


            var nodes = nodese.ToList();


            string indent = genIndent();

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];



                Visit(node);

                if ((i + 1) < nodes.Count)
                {
                    var nextNode = nodes[i + 1];


                    WriteCommentsBetweenRange(node.SourceCodeRange, nextNode.SourceCodeRange, 1);
                    
                }
            }
            
            return true;
        }
        

        public override bool VisitIfStatement(ILSLIfStatementNode node)
        {
            Writer.Write("if");

            WriteCommentsBetweenRange(node.IfKeywordSourceCodeRange, node.OpenParenthSourceCodeRange);

            Writer.Write("(");

            WriteCommentsBetweenRange(node.OpenParenthSourceCodeRange, node.ConditionExpression.SourceCodeRange);


            Visit(node.ConditionExpression);

            WriteCommentsBetweenRange(node.ConditionExpression.SourceCodeRange, node.CloseParenthSourceCodeRange);

            Writer.Write(")");


            var comments = GetComments(node.ConditionExpression.SourceCodeRange.StopIndex, node.Code.SourceCodeRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.ConditionExpression.SourceCodeRange.LineEnd;

                while (linesBetweenNodeAndFirstComment > 0)
                {
                    Writer.Write("\n");
                    linesBetweenNodeAndFirstComment--;
                }

                for (int j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    if (comment.SourceCodeRange.LineStart != node.CloseParenthSourceCodeRange.LineEnd)
                    {
                        Writer.Write(genIndent() + comment.Text);
                    }
                    else
                    {
                        Writer.Write(" " + comment.Text);
                    }

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
                        var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                        while (linesBetweenComments > 0)
                        {
                            Writer.Write("\n");
                            linesBetweenComments--;
                        }
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = node.Code.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                        var cnt = linesBetweenCommentAndNextNode;
                        var limit = node.Code.IsSingleStatement ? 0 : 1;
                        while (cnt > limit)
                        {
                            Writer.Write("\n");
                            cnt--;
                        }
                        if (node.Code.IsSingleStatement && linesBetweenCommentAndNextNode > 0)
                        {
                            Writer.Write(genIndent(1));
                        }
                    }
                }
            }
            else if(node.Code.IsSingleStatement)
            {
                Writer.Write("\n" + genIndent(1));
            }



            Visit(node.Code);
            return true;
        }

        public override bool VisitElseIfStatement(ILSLElseIfStatementNode node)
        {
            Writer.Write("\n"+genIndent()+"else");

            WriteCommentsBetweenRange(node.ElseKeywordSourceCodeRange, node.IfKeywordSourceCodeRange);

            Writer.Write("if");

            WriteCommentsBetweenRange(node.IfKeywordSourceCodeRange, node.OpenParenthSourceCodeRange);

            Writer.Write("(");

            WriteCommentsBetweenRange(node.OpenParenthSourceCodeRange, node.ConditionExpression.SourceCodeRange);


            Visit(node.ConditionExpression);

            WriteCommentsBetweenRange(node.ConditionExpression.SourceCodeRange, node.CloseParenthSourceCodeRange);

            Writer.Write(")");



            Visit(node.ConditionExpression);
            Writer.Write(")");

            var comments = GetComments(node.ConditionExpression.SourceCodeRange.StopIndex, node.Code.SourceCodeRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.ConditionExpression.SourceCodeRange.LineEnd;

                while (linesBetweenNodeAndFirstComment > 0)
                {
                    Writer.Write("\n");
                    linesBetweenNodeAndFirstComment--;
                }

                for (int j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    if (comment.SourceCodeRange.LineStart != node.CloseParenthSourceCodeRange.LineEnd)
                    {
                        Writer.Write(genIndent() + comment.Text);
                    }
                    else
                    {
                        Writer.Write(" " + comment.Text);
                    }

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
                        var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                        while (linesBetweenComments > 0)
                        {
                            Writer.Write("\n");
                            linesBetweenComments--;
                        }
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = node.Code.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                        var cnt = linesBetweenCommentAndNextNode;
                        var limit = node.Code.IsSingleStatement ? 0 : 1;
                        while (cnt > limit)
                        {
                            Writer.Write("\n");
                            cnt--;
                        }
                        if (node.Code.IsSingleStatement && linesBetweenCommentAndNextNode > 0)
                        {
                            Writer.Write(genIndent(1));
                        }
                    }
                }
            }
            else if (node.Code.IsSingleStatement)
            {
                Writer.Write("\n" + genIndent(1));
            }

            Visit(node.Code);
            return true;
        }

        public override bool VisitElseStatement(ILSLElseStatementNode node)
        {
            Writer.Write("\n" + genIndent() + "else");


            var comments = GetComments(node.SourceCodeRange.StartIndex, node.Code.SourceCodeRange.StartIndex).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd;

                while (linesBetweenNodeAndFirstComment > 0)
                {
                    Writer.Write("\n");
                    linesBetweenNodeAndFirstComment--;
                }

                for (int j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    if (comment.SourceCodeRange.LineStart != node.ElseKeywordSourceCodeRange.LineEnd)
                    {
                        Writer.Write(genIndent() + comment.Text);
                    }
                    else
                    {
                        Writer.Write(" " + comment.Text);
                    }

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
                        var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                        while (linesBetweenComments > 0)
                        {
                            Writer.Write("\n");
                            linesBetweenComments--;
                        }
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = node.Code.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                        var cnt = linesBetweenCommentAndNextNode;
                        var limit = node.Code.IsSingleStatement ? 0 : 1;
                        while (cnt > limit)
                        {
                            Writer.Write("\n");
                            cnt--;
                        }
                        if (node.Code.IsSingleStatement && linesBetweenCommentAndNextNode > 0)
                        {
                            Writer.Write(genIndent(1));
                        }
                    }
                }
            }
            else if (node.Code.IsSingleStatement)
            {
                Writer.Write("\n" + genIndent(1));
            }

            Visit(node.Code);
            return true;
        }

        public override bool VisitExpressionStatement(ILSLExpressionStatementNode node)
        {
            Visit(node.Expression);

            var comments = GetComments(node.Expression.SourceCodeRange.StopIndex, node.SourceCodeRange.StopIndex, true).ToList();

            if (comments.Count > 0)
            {
                var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.Expression.SourceCodeRange.LineEnd;

                if (linesBetweenNodeAndFirstComment == 0)
                {
                    var spacesBetweenNodeAndFirstComment = (comments[0].SourceCodeRange.StartIndex - node.Expression.SourceCodeRange.StopIndex) - 1;

                    spacesBetweenNodeAndFirstComment = spacesBetweenNodeAndFirstComment > 0 ? spacesBetweenNodeAndFirstComment : 1;

                    while (spacesBetweenNodeAndFirstComment > 0)
                    {
                        Writer.Write(" ");
                        spacesBetweenNodeAndFirstComment--;
                    }
                }

                while (linesBetweenNodeAndFirstComment > 0)
                {
                    Writer.Write("\n");
                    linesBetweenNodeAndFirstComment--;
                }

                for (int j = 0; j < comments.Count; j++)
                {
                    var comment = comments[j];

                    if (comment.SourceCodeRange.LineStart != node.Expression.SourceCodeRange.LineEnd)
                    {
                        Writer.Write(genIndent() + comment.Text);
                    }
                    else
                    {
                        Writer.Write(comment.Text);
                    }

                    if ((j + 1) < comments.Count)
                    {
                        var nextComment = comments[j + 1];
                        var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;

                        if (linesBetweenComments == 0)
                        {
                            var spacesBetweenComments = (nextComment.SourceCodeRange.StartIndex - comment.SourceCodeRange.StopIndex) - 1;
                            while (spacesBetweenComments > 0)
                            {
                                Writer.Write(" ");
                                spacesBetweenComments--;
                            }
                        }


                        while (linesBetweenComments > 0)
                        {
                            Writer.Write("\n");
                            linesBetweenComments--;
                        }
                    }
                    else
                    {
                        var linesBetweenCommentAndNextNode = node.SourceCodeRange.LineEnd - comment.SourceCodeRange.LineEnd;

                        if (linesBetweenCommentAndNextNode == 0)
                        {
                            var spacesBetweenCommentAndNextNode = (node.SourceCodeRange.StartIndex - comment.SourceCodeRange.StopIndex);

                            spacesBetweenCommentAndNextNode = spacesBetweenCommentAndNextNode > 0 ? spacesBetweenCommentAndNextNode : 1;

                            while (spacesBetweenCommentAndNextNode > 0)
                            {
                                Writer.Write(" ");
                                spacesBetweenCommentAndNextNode--;
                            }
                        }
                        else
                        {
                            while (linesBetweenCommentAndNextNode > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenCommentAndNextNode--;
                            }

                            Writer.Write(genIndent());
                        }
                    }
                }
            }


            Writer.Write(";");
            return true;
        }


        public override bool VisitVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            Writer.Write(node.TypeString);

            if (!WriteCommentsBetweenRange(node.TypeSourceCodeRange, node.NameSourceCodeRange))
            {
                Writer.Write(" ");
            }

            Writer.Write(node.Name);

            if (node.HasDeclarationExpression)
            {

                if (!WriteCommentsBetweenRange(node.NameSourceCodeRange, node.OperatorSourceCodeRange))
                {
                    Writer.Write(" ");
                }

                Writer.Write("=");

                if (!WriteCommentsBetweenRange(node.OperatorSourceCodeRange, node.DeclarationExpression.SourceCodeRange))
                {
                    Writer.Write(" ");
                }

                Visit(node.DeclarationExpression);

                WriteCommentsBetweenRange(node.DeclarationExpression.SourceCodeRange, node.SourceCodeRange.GetLastCharRange());

                Writer.Write(";");

            }
            else
            {
                WriteCommentsBetweenRange(node.NameSourceCodeRange, node.SourceCodeRange.GetLastCharRange());

                Writer.Write(";");

            }
            

            return true;
        }

       /* public override bool VisitLocalVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            Writer.Write(node.TypeString + " " + node.Name);
            if (node.HasDeclarationExpression)
            {
                Writer.Write(" = ");
                Visit(node.DeclarationExpression);
            }
            Writer.Write(";");

            return true;
        }*/

        public override bool VisitSingleStatementCodeScope(ILSLCodeScopeNode node)
        {

            Writer.Write(" ");

            Visit(node.CodeStatements.First());
        
            return true;
        }

        public override bool VisitMultiStatementCodeScope(ILSLCodeScopeNode snode)
        {
            //if (snode.CodeScopeType == LSLCodeScopeType.FunctionCodeRoot || snode.CodeScopeType == LSLCodeScopeType.EventHandlerCodeRoot)
            //{
                Writer.Write("\n" + genIndent() + "{\n");
                
            //}
            //else
            //{
            //    Writer.Write("{\n");
            //}

            indentLevel++;

            var nodes = snode.CodeStatements.ToList();

            string indent = genIndent();


            if (nodes.Count > 0)
            {
                var comments = GetComments(snode.SourceCodeRange.StartIndex, nodes[0].SourceCodeRange.StartIndex).ToList();

                if (comments.Count > 0)
                {
                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - snode.SourceCodeRange.LineStart;

                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Writer.Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];

                        Writer.Write(indent + comment.Text);

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndNextNode = nodes[0].SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                            while (linesBetweenCommentAndNextNode > 0)
                            {
                                Writer.Write("\n");
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
                        Writer.Write("\n");
                        linesBetweenTwoNodes--;
                    }
                }




                for (int i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];

                    var singleLineBroken = 0;

                    Writer.Write(indent);
                    Visit(node);

                    if ((i + 1) < nodes.Count)
                    {
                        var nextNode = nodes[i + 1];

                        if (node.SourceCodeRange.LineStart == nextNode.SourceCodeRange.LineStart)
                        {
                            singleLineBroken = 1;
                            Writer.Write("\n");
                        }


                        comments = GetComments(node.SourceCodeRange.StopIndex, nextNode.SourceCodeRange.StartIndex).ToList();

                        if (comments.Count > 0)
                        {
                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd;

                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
                            {
                                var comment = comments[j];

                                Writer.Write(indent + comment.Text);

                                if ((j + 1) < comments.Count)
                                {
                                    var nextComment = comments[j + 1];
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Writer.Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndNextNode = nextNode.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                                    var linesBetweenCommentAndLastNode = node.SourceCodeRange.LineEnd - comment.SourceCodeRange.LineStart;

                                    if (linesBetweenCommentAndLastNode == 0 && linesBetweenCommentAndNextNode == 1 && node is ILSLControlStatementNode)
                                    {
                                        linesBetweenCommentAndNextNode++;
                                    }

                                    while (linesBetweenCommentAndNextNode > 0)
                                    {
                                        Writer.Write("\n");
                                        linesBetweenCommentAndNextNode--;
                                    }
                                }
                            }
                        }
                        else
                        {

                            var linesBetweenTwoNodes = (nextNode.SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd);

                            if (   (nextNode is ILSLVariableDeclarationNode ||
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
                                    (nextNode is ILSLExpressionStatementNode && node is ILSLExpressionStatementNode) ||
                                    (nextNode is ILSLControlStatementNode && node is ILSLControlStatementNode) 
                                    )) linesBetweenTwoNodes = (2 - singleLineBroken);
                            }

                            while (linesBetweenTwoNodes > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenTwoNodes--;
                            }

                        }
                    }
                    else
                    {
                        comments = GetComments(node.SourceCodeRange.StopIndex, snode.SourceCodeRange.StopIndex).ToList();

                        if (comments.Count > 0)
                        {

                            var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - node.SourceCodeRange.LineEnd;

                            if (linesBetweenNodeAndFirstComment == 0) indent = " ";

                            while (linesBetweenNodeAndFirstComment > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenNodeAndFirstComment--;
                            }

                            for (int j = 0; j < comments.Count; j++)
                            {
                                var comment = comments[j];
                                Writer.Write(indent + comment.Text);

                                if ((j + 1) < comments.Count)
                                {
                                    var nextComment = comments[j + 1];
                                    var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                                    while (linesBetweenComments > 0)
                                    {
                                        Writer.Write("\n");
                                        linesBetweenComments--;
                                    }
                                }
                                else
                                {
                                    var linesBetweenCommentAndEndOfScope = snode.SourceCodeRange.LineEnd - comment.SourceCodeRange.LineEnd;
                                    if (linesBetweenCommentAndEndOfScope == 0)
                                    {
                                        linesBetweenCommentAndEndOfScope = 1;
                                    }
                                    while (linesBetweenCommentAndEndOfScope > 0)
                                    {
                                        Writer.Write("\n");
                                        linesBetweenCommentAndEndOfScope--;
                                    }
                                }
                            }


                        }
                        else
                        {
                            var linesBetweenNodeAndEndOfScope = snode.SourceCodeRange.LineEnd - node.SourceCodeRange.LineEnd;
                            if (linesBetweenNodeAndEndOfScope == 0)
                            {
                                linesBetweenNodeAndEndOfScope = 1;
                            }
                            while (linesBetweenNodeAndEndOfScope > 0)
                            {
                                Writer.Write("\n");
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

                    var linesBetweenNodeAndFirstComment = comments[0].SourceCodeRange.LineStart - snode.SourceCodeRange.LineStart;

                    while (linesBetweenNodeAndFirstComment > 1)
                    {
                        Writer.Write("\n");
                        linesBetweenNodeAndFirstComment--;
                    }

                    for (int j = 0; j < comments.Count; j++)
                    {
                        var comment = comments[j];
                        Writer.Write(indent + comment.Text);

                        if ((j + 1) < comments.Count)
                        {
                            var nextComment = comments[j + 1];
                            var linesBetweenComments = nextComment.SourceCodeRange.LineStart - comment.SourceCodeRange.LineEnd;
                            while (linesBetweenComments > 0)
                            {
                                Writer.Write("\n");
                                linesBetweenComments--;
                            }
                        }
                        else
                        {
                            var linesBetweenCommentAndEndOfScope = snode.SourceCodeRange.LineEnd - comment.SourceCodeRange.LineEnd;
                            if (linesBetweenCommentAndEndOfScope == 0)
                            {
                                linesBetweenCommentAndEndOfScope = 1;
                            }
                            while (linesBetweenCommentAndEndOfScope > 0)
                            {
                                Writer.Write("\n");
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
                        Writer.Write("\n");
                        linesBetweenNodeAndEndOfScope--;
                    }
                }

            }


            indentLevel--;

            Writer.Write(genIndent()+"}");

            return true;
        }

        public override bool VisitHexLiteral(ILSLHexLiteralNode lslHexLiteralNode)
        {
            Writer.Write(lslHexLiteralNode.RawText);

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
                if (cntr != (cnt - 1))
                {
                    Writer.Write(", ");
                }

                cntr++;
            }

            return true;
        }

        //public override bool VisitState(ILSLStateScopeNode node)
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool VisitVariableDeclaration(ILSLVariableDeclarationNode node)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
