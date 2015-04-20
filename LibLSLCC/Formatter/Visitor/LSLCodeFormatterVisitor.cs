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
        private List<LSLComment> comments = new List<LSLComment>();
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
            comments.AddRange(node.Comments);


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


        public override bool VisitCompilationUnit(ILSLCompilationUnitNode node)
        {


            int ct = 0;
            int lastStopIndex = 0;

            foreach (var gvar in node.GlobalVariableDeclarations)
            {


                Visit(gvar);


                
                Writer.Write(";\n");

                foreach (var comment in comments)
                {
                    if (((ct + 1) < node.GlobalVariableDeclarations.Count) && comment.Start > gvar.SourceCodeRange.StopIndex &&
                        comment.Start < node.GlobalVariableDeclarations[ct + 1].SourceCodeRange.StartIndex)
                    {
                        compilationUnitNode.Comments.
                        Writer.Write(comment.Text + "\n");
                    }
                }

                lastStopIndex = gvar.SourceCodeRange.StopIndex;
                ct++;
            }

           
            foreach (var comment in comments)
            {
                if ((node.FunctionDeclarations.Count > 0) && comment.Start > lastStopIndex &&
                    comment.Start < node.FunctionDeclarations.First().SourceCodeRange.StartIndex)
                {
                    Writer.Write(comment.Text + "\n");
                }
            }

            ct = 0;


            foreach (var func in node.FunctionDeclarations)
            {
                Visit(func);
                Writer.Write("\n");

                foreach (var comment in comments)
                {
                    if (((ct + 1) < node.FunctionDeclarations.Count) && comment.Start > func.SourceCodeRange.StopIndex &&
                        comment.Start < node.FunctionDeclarations[ct + 1].SourceCodeRange.StartIndex)
                    {
                        
                        Writer.Write(comment.Text + "\n");
                    }
                }

                lastStopIndex = func.SourceCodeRange.StopIndex;
                ct++;
            }



            foreach (var comment in comments)
            {
                if (comment.Start > lastStopIndex &&
                    comment.Start < node.DefaultState.SourceCodeRange.StartIndex)
                {
                    Writer.Write(comment.Text + "\n");
                }
            }

            Visit(node.DefaultState);
            Writer.Write("\n");


            foreach (var st in node.StateDeclarations)
            {
                Visit(st);
                Writer.Write("\n");
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
            Writer.Write(")\n");
            Visit(node.EventBodyNode);

            return true;
        }

        public override bool VisitFunctionDeclaration(ILSLFunctionDeclarationNode node)
        {
            Writer.Write(node.ReturnTypeString +" "+node.Name+"(");
            Visit(node.ParameterListNode);
            Writer.Write(")");
            Visit(node.FunctionBodyNode);
           
            return true;
        }

        public override bool VisitDefaultState(ILSLStateScopeNode node)
        {
            Writer.Write("default\n{\n");

            indentLevel++;



            foreach (var eventHandler in node.EventHandlers)
            {
                Writer.Write(genIndent());
                Visit(eventHandler);
                Writer.Write("\n\n");
            }

            indentLevel--;

            Writer.Write("\n}");

            return true;
        }

        public override bool VisitDefinedState(ILSLStateScopeNode node)
        {
            Writer.Write("state "+node.StateName+"\n{");

            indentLevel++;

            foreach (var eventHandler in node.EventHandlers)
            {
                Writer.Write(genIndent());
                Visit(eventHandler);
                Writer.Write("\n\n");
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
            Writer.Write("return ");
            Visit(node.ReturnExpression);
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
            Writer.Write("else if(");
            Visit(node.ConditionExpression);
            Writer.Write(")");
            Visit(node.Code);
            return true;
        }

        public override bool VisitElseStatement(ILSLElseStatementNode node)
        {
            Writer.Write("else");
            Visit(node.Code);
            return true;
        }

        public override bool VisitExpressionStatement(ILSLExpressionStatementNode node)
        {
            Visit(node.Expression);
            return true;
        }


        public override bool VisitGlobalVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            Writer.Write(node.TypeString+" "+node.Name);

            return true;
        }

        public override bool VisitLocalVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            Writer.Write(node.TypeString + " " + node.Name);

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
            Writer.Write(genIndent()+"{\n");

            indentLevel++;

            foreach (var n in node.CodeStatements)
            {
                Writer.Write(genIndent());
                Visit(n);
                Writer.Write(";\n");
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
