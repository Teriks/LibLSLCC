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

namespace LibLSLCC.Formatter.Visitor
{
    public class LSLCodeFormatterVisitor : LSLValidatorNodeVisitor<bool>
    {
        private LinkedList<LSLComment> comments = new LinkedList<LSLComment>();
        private ILSLCompilationUnitNode compilationUnitNode;
        private int indentLevel = 0;


        private string genIndent()
        {

            StringBuilder str = new StringBuilder();
            for (int i = 0; i < indentLevel; i++)
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


        public override bool VisitBinaryExpression(ILSLBinaryExpressionNode node)
        {
            Visit(node.LeftExpression);

            Writer.Write(" "+node.OperationString+" ");
            
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

            Visit(node.InnerExpression);

            Writer.Write(")");

            return true;
        }

        public override bool VisitPostfixOperation(ILSLPostfixOperationNode node)
        {
            Visit(node.LeftExpression);
            Writer.Write(node.OperationString);

            return true;
        }

        public override bool VisitPrefixOperation(ILSLPrefixOperationNode node)
        {
            Writer.Write(node.OperationString);
            Visit(node.RightExpression);
            return true;
        }

        public override bool VisitRotationLiteral(ILSLRotationLiteralNode node)
        {
            Writer.Write("<");
            Visit(node.XExpression);
            Writer.Write(", ");
            Visit(node.YExpression);
            Writer.Write(", ");
            Visit(node.ZExpression);
            Writer.Write(", ");
            Visit(node.SExpression);
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
            Writer.Write(node.AccessedComponentString);

            return true;
        }

        public override bool VisitVectorLiteral(ILSLVectorLiteralNode node)
        {
            Writer.Write("<");
            Visit(node.XExpression);
            Writer.Write(", ");
            Visit(node.YExpression);
            Writer.Write(", ");
            Visit(node.ZExpression);
            Writer.Write(">");

            return true;
        }

        public override bool VisitDoLoop(ILSLDoLoopNode node)
        {
            Writer.Write("do");
            Visit(node.Code);
            Writer.Write("while(");
            Visit(node.ConditionExpression);
            Writer.Write(")");

            return true;
        }

        public override bool VisitForLoop(ILSLForLoopNode node)
        {
            Writer.Write("for(");
            Visit(node.InitExpression);
            Writer.Write(";");
            Visit(node.ConditionExpression);
            Writer.Write(";");
            Visit(node.AfterthoughExpressions);
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

                if (comment.Start >= sourceRangeStart &&  comment.End <= sourceRangeEnd)
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

        public override bool VisitCompilationUnit(ILSLCompilationUnitNode node)
        {

            var nodes = node.GlobalVariableDeclarations.Concat<ILSLReadOnlySyntaxTreeNode>(node.FunctionDeclarations).ToList();

            nodes.Sort((emp1, emp2) => { return emp1.SourceCodeRange.StartIndex.CompareTo(emp2.SourceCodeRange.StartIndex); });


            int cnt = 0;
            int lastStopIndex = 0;
            int lastStopLine = 1;

            List<LSLComment> comments;
            int spaceBetween;

            ILSLReadOnlySyntaxTreeNode next;


            if (nodes.Count > 0)
            {
                comments = GetComments(0, nodes.First().SourceCodeRange.StartIndex, true).ToList();
                foreach (var comment in comments)
                {
                    Writer.Write(comment.Text + "\n");
                }

            }
            

            foreach (var n in nodes)
            {
               
                Visit(n);

                if ((cnt + 1) < nodes.Count) {
                    next = nodes[cnt + 1];

                    comments = GetComments(n.SourceCodeRange.StopIndex, next.SourceCodeRange.StartIndex, true).ToList();

                    if (comments.Count == 0){
                        spaceBetween = next.SourceCodeRange.LineStart - n.SourceCodeRange.LineEnd;
                    }
                    else{
                        spaceBetween = comments.First().StartLine - n.SourceCodeRange.LineEnd;
                    }

                    while (spaceBetween != 0){
                        Writer.Write("\n");
                        spaceBetween--;
                    }

                    foreach (var comment in comments){
                        Writer.Write(comment.Text + "\n");
                    }
                
                }

                lastStopIndex = n.SourceCodeRange.StopIndex;
                lastStopLine = n.SourceCodeRange.LineEnd;
                cnt++;
            }


            next = node.DefaultState;
            comments = GetComments(lastStopIndex, next.SourceCodeRange.StartIndex, true).ToList();

            if (comments.Count == 0)
            {
                spaceBetween = next.SourceCodeRange.LineStart - lastStopLine;
            }
            else
            {
                spaceBetween = comments.First().StartLine - lastStopLine;
            }

            while (spaceBetween != 0)
            {
                Writer.Write("\n");
                spaceBetween--;
            }

            foreach (var comment in comments)
            {
                Writer.Write(comment.Text + "\n");
            }


            lastStopIndex = node.DefaultState.SourceCodeRange.StopIndex;
            lastStopLine = node.DefaultState.SourceCodeRange.LineEnd;

            Visit(node.DefaultState);


            if (node.StateDeclarations.Count > 0)
            {

                next = node.StateDeclarations.First();

                comments = GetComments(lastStopIndex, next.SourceCodeRange.StartIndex, true).ToList();

                if (comments.Count == 0)
                {
                    spaceBetween = next.SourceCodeRange.LineStart - lastStopLine;
                }
                else
                {
                    spaceBetween = comments.First().StartLine - lastStopLine;
                }

                while (spaceBetween != 0)
                {
                    Writer.Write("\n");
                    spaceBetween--;
                }

                foreach (var comment in comments)
                {
                    Writer.Write(comment.Text + "\n");
                }

            }
            else
            {
                comments = GetComments(lastStopIndex, int.MaxValue, true).ToList();


                if (comments.Count > 0)
                {

                    spaceBetween = comments.First().StartLine - lastStopLine;


                    while (spaceBetween != 0)
                    {
                        Writer.Write("\n");
                        spaceBetween--;
                    }

                    foreach (var comment in comments)
                    {
                        Writer.Write(comment.Text + "\n");
                    }
                }

            }

            cnt = 0;


            foreach (var n in node.StateDeclarations)
            {
                Visit(n);

                if ((cnt + 1) < node.StateDeclarations.Count)
                {
                    next = node.StateDeclarations[cnt + 1];

                    comments = GetComments(n.SourceCodeRange.StopIndex, next.SourceCodeRange.StartIndex, true).ToList();

                    if (comments.Count == 0)
                    {
                        spaceBetween = next.SourceCodeRange.LineStart - n.SourceCodeRange.LineEnd;
                    }
                    else
                    {
                        spaceBetween = comments.First().StartLine - n.SourceCodeRange.LineEnd;
                    }

                    while (spaceBetween != 0)
                    {
                        Writer.Write("\n");
                        spaceBetween--;
                    }

                    foreach (var comment in comments)
                    {
                        Writer.Write(comment.Text + "\n");
                    }

                }

                lastStopIndex = n.SourceCodeRange.StopIndex;
                lastStopLine = n.SourceCodeRange.LineEnd;
                cnt++;
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
            Visit(node.FunctionBodyNode);
           
            return true;
        }

        public override bool VisitState(ILSLStateScopeNode node)
        {
            if (node.IsDefaultState)
            {
                Writer.Write("default\n{\n");
            }
            else
            {
                Writer.Write("state "+node.StateName+"\n{\n");
            }

            indentLevel++;

            var cnt = node.EventHandlers.Count-1;
            int i;
            for (i = 0; i < cnt; i++)
            {
                Writer.Write(genIndent());
                Visit(node.EventHandlers[i]);
                Writer.Write("\n\n");

            }

            Writer.Write(genIndent());
            Visit(node.EventHandlers[i]);

            indentLevel--;

            Writer.Write("\n}");

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
            Writer.Write("state "+node.StateTargetName);
            return true;
        }

        public override bool VisitJumpStatement(ILSLJumpStatementNode node)
        {
            Writer.Write("jump " + node.LabelName);

            return true;
        }

        public override bool VisitLabelStatement(ILSLLabelStatementNode node)
        {
            Writer.Write("@" + node.LabelName);

            return true;
        }

        //public override bool VisitControlStatement(ILSLControlStatementNode node)
        //{
        //    throw new NotImplementedException();
        //}

        public override bool VisitIfStatement(ILSLIfStatementNode node)
        {
            Writer.Write("if(");
            Visit(node.ConditionExpression);
            Writer.Write(")");
            Visit(node.Code);
            return true;
        }

        public override bool VisitElseIfStatement(ILSLElseIfStatementNode node)
        {
            Writer.Write("\n"+genIndent()+"else if(");
            Visit(node.ConditionExpression);
            Writer.Write(")");
            Visit(node.Code);
            return true;
        }

        public override bool VisitElseStatement(ILSLElseStatementNode node)
        {
            Writer.Write("\n" + genIndent() + "else");
            Visit(node.Code);
            return true;
        }

        public override bool VisitExpressionStatement(ILSLExpressionStatementNode node)
        {
            Visit(node.Expression);
            Writer.Write(";");
            return true;
        }


        public override bool VisitGlobalVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            Writer.Write(node.TypeString+" "+node.Name);
            if (node.HasDeclarationExpression)
            {
                Writer.Write(" = ");
                Visit(node.DeclarationExpression);
            }
            Writer.Write(";");

            return true;
        }

        public override bool VisitLocalVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            Writer.Write(node.TypeString + " " + node.Name);
            if (node.HasDeclarationExpression)
            {
                Writer.Write(" = ");
                Visit(node.DeclarationExpression);
            }
            Writer.Write(";");

            return true;
        }

        public override bool VisitSingleStatementCodeScope(ILSLCodeScopeNode node)
        {
            Writer.Write(" ");
            Visit(node.CodeStatements.First());
        
            return true;
        }

        public override bool VisitMultiStatementCodeScope(ILSLCodeScopeNode node)
        {
            Writer.Write("\n"+genIndent()+"{\n");

            indentLevel++;

            var ct = 0;
            var lastStopIndex = node.SourceCodeRange.StartIndex;

            List<ILSLReadOnlyCodeStatement> codeStatements = new List<ILSLReadOnlyCodeStatement>();
            codeStatements.AddRange(node.CodeStatements);

            if (codeStatements.Count > 0)
            {
                

                foreach (var comment in
                        GetComments(lastStopIndex,
                        codeStatements[0].SourceCodeRange.StartIndex, true))
                {

                    Writer.Write(genIndent()+comment.Text + "\n");
                }

                lastStopIndex = codeStatements[0].SourceCodeRange.StopIndex;
            }

            foreach (var n in codeStatements)
            {
                Writer.Write(genIndent());
                Visit(n);
                Writer.Write("\n");

                if ((ct + 1) < codeStatements.Count)
                {
                    foreach (var comment in
                        GetComments(n.SourceCodeRange.StopIndex,
                        codeStatements[ct + 1].SourceCodeRange.StartIndex, true))
                    {

                        Writer.Write(genIndent() + comment.Text + "\n");
                    }
                }

                lastStopIndex = n.SourceCodeRange.StopIndex;
            }


            foreach (var comment in
                        GetComments(lastStopIndex,
                        node.SourceCodeRange.StopIndex, true))
            {

                Writer.Write(genIndent() + comment.Text + "\n");
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
