using LibLSLCC.CodeValidator.ValidatorNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

namespace LibLSLCC.CodeValidator.ValidatorNodeVisitor
{
    public class LSLValidatorNodeVisitor<T> : ILSLValidatorNodeVisitor<T>
    {
        /// <summary>
        ///     Default implementation calls Visit(node.LeftExpression) then Visit(node.RightExpression) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing a binary operation and its operands in an expression tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitBinaryExpression(ILSLBinaryExpressionNode node)
        {
            Visit(node.LeftExpression);
            Visit(node.RightExpression);


            return default(T);
        }



        /// <summary>
        ///     Default implementation calls VisitExpressionList(node)
        /// </summary>
        /// <param name="node">An object describing a list of expressions in the context of a list literal initializer list</param>
        /// <returns>VisitExpressionList(node)</returns>
        public virtual T VisitListLiteralInitializerList(ILSLExpressionListNode node)
        {
            return VisitExpressionList(node);
        }



        /// <summary>
        ///     Default implementation calls VisitExpressionList(node)
        /// </summary>
        /// <param name="node">An object describing a list of expressions in the context of function call parameters</param>
        /// <returns>VisitExpressionList(node)</returns>
        public virtual T VisitFunctionCallParameters(ILSLExpressionListNode node)
        {
            return VisitExpressionList(node);
        }



        /// <summary>
        ///     Default implementation calls VisitExpressionList(node)
        /// </summary>
        /// <param name="node">An object describing a list of expressions in the context of for loop afterthoughts</param>
        /// <returns>VisitExpressionList(node)</returns>
        public virtual T VisitForLoopAfterthoughts(ILSLExpressionListNode node)
        {
            return VisitExpressionList(node);
        }



        /// <summary>
        ///     Default implementation calls VisitExpressionList(node)
        /// </summary>
        /// <param name="node">An object describing a list of expressions in the context of for loop initializer expressions</param>
        /// <returns>VisitExpressionList(node)</returns>
        public virtual T VisitForLoopInitExpressions(ILSLExpressionListNode node)
        {
            return VisitExpressionList(node);
        }



        /// <summary>
        ///     Default implementation just returns default(T)
        /// </summary>
        /// <param name="node">An object describing an float literal in an expression tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitFloatLiteral(ILSLFloatLiteralNode node)
        {
            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.ParameterListNode) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing a function call in an expression tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitFunctionCall(ILSLFunctionCallNode node)
        {
            Visit(node.ParameterListNode);

            return default(T);
        }



        /// <summary>
        ///     Default implementation just returns default(T)
        /// </summary>
        /// <param name="node">An object describing an integer literal in an expression tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitIntegerLiteral(ILSLIntegerLiteralNode node)
        {
            return default(T);
        }



        /// <summary>
        ///     DefaultImplementation calls Visit(node.ExpressionListNode) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing a list literal in an expression tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitListLiteral(ILSLListLiteralNode node)
        {
            Visit(node.ExpressionListNode);

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.InnerExpression) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the parenthesized expression in an expression tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitParenthesizedExpression(ILSLParenthesizedExpressionNode node)
        {
            Visit(node.InnerExpression);

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.LeftExpression) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the postfix operation and its operand</param>
        /// <returns>default(T)</returns>
        public virtual T VisitPostfixOperation(ILSLPostfixOperationNode node)
        {
            Visit(node.LeftExpression);

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.RightExpression) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the prefix operation and its operand</param>
        /// <returns>default(T)</returns>
        public virtual T VisitPrefixOperation(ILSLPrefixOperationNode node)
        {
            Visit(node.RightExpression);

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls in order:
        ///     Visit(node.XExpression), Visit(node.YExpression), Visit(node.ZExpression), Visit(node.SExpression).
        ///     Then returns default(T)
        /// </summary>
        /// <param name="node">An object describing a rotation literal in an expression tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitRotationLiteral(ILSLRotationLiteralNode node)
        {
            Visit(node.XExpression);
            Visit(node.YExpression);
            Visit(node.ZExpression);
            Visit(node.SExpression);

            return default(T);
        }



        /// <summary>
        ///     Default implementation returns default(T)
        /// </summary>
        /// <param name="node">An object describing a string literal in an expression tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitStringLiteral(ILSLStringLiteralNode node)
        {
            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.CastedExpression) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the prefix style typecast operation</param>
        /// <returns>default(T)</returns>
        public virtual T VisitTypecastExpression(ILSLTypecastExprNode node)
        {
            Visit(node.CastedExpression);

            return default(T);
        }



        /// <summary>
        ///     Default implementation just returns default(T)
        /// </summary>
        /// <param name="node">An object describing a variable reference in an expression tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitVariableReference(ILSLVariableNode node)
        {
            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.AccessedExpression) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing a member access operation on a vector or rotation</param>
        /// <returns>default(T)</returns>
        public virtual T VisitVecRotAccessor(ILSLTupleAccessorNode node)
        {
            Visit(node.AccessedExpression);

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls in order: Visit(node.XExpression), Visit(node.YExpression), Visit(node.ZExpression).
        ///     Then returns default(T)
        /// </summary>
        /// <param name="node">An object describing a vector literal in an expression tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitVectorLiteral(ILSLVectorLiteralNode node)
        {
            Visit(node.XExpression);
            Visit(node.YExpression);
            Visit(node.ZExpression);

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.Code) then Visit(node.ConditionExpression) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the do loop</param>
        /// <returns>default(T)</returns>
        public virtual T VisitDoLoop(ILSLDoLoopNode node)
        {
            Visit(node.Code);
            Visit(node.ConditionExpression);
            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.InitExpression) if node.HasInitExpression is true.
        ///     Then calls Visit(node.ConditionExpression) if node.HasConditionExpression is true.
        ///     Finally it calls Visit(node.AfterthoughExpressions) if node.HasAfterthoughtExpressions is true,
        ///     and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the for loop</param>
        /// <returns>default(T)</returns>
        public virtual T VisitForLoop(ILSLForLoopNode node)
        {
            if (node.HasInitExpressions)
            {
                Visit(node.InitExpressionsList);
            }

            if (node.HasConditionExpression)
            {
                Visit(node.ConditionExpression);
            }

            if (node.HasAfterthoughtExpressions)
            {
                Visit(node.AfterthoughExpressions);
            }

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.ConditionExpression) then Visit(node.Code) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the while loop</param>
        /// <returns>default(T)</returns>
        public virtual T VisitWhileLoop(ILSLWhileLoopNode node)
        {
            Visit(node.ConditionExpression);
            Visit(node.Code);
            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(codeStatement) for each ILSLCodeStatement in node.CodeStatements
        ///     and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the code scope</param>
        /// <returns>default(T)</returns>
        public virtual T VisitCodeScope(ILSLCodeScopeNode node)
        {
            foreach (var st in node.CodeStatements)
            {
                Visit(st);
            }

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(globalVariable) for each LSLVariableNode in node.GlobalVariableDeclarations.
        ///     Then it calls Visit(functionDeclaration for each LSLVariableNode in node.FunctionDeclarations.
        ///     Then it calls Visit(stateDeclaration) for each LSLVariableNode in node.StateDeclarations.
        ///     Finally it calls Visit(node.DefaultState) to visit the single default state in the script, and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the compilation unit, which is the top of the syntax tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitCompilationUnit(ILSLCompilationUnitNode node)
        {
            foreach (var gvar in node.GlobalVariableDeclarations)
            {
                Visit(gvar);
            }

            foreach (var func in node.FunctionDeclarations)
            {
                Visit(func);
            }

            foreach (var st in node.StateDeclarations)
            {
                Visit(st);
            }

            Visit(node.DefaultState);

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.ParameterListNode) then Visit(node.EventBodyNode)
        ///     and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the event handler declaration</param>
        /// <returns>default(T)</returns>
        public virtual T VisitEventHandler(ILSLEventHandlerNode node)
        {
            Visit(node.ParameterListNode);
            Visit(node.EventBodyNode);

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.ParameterListNode) then Visit(node.FunctionBodyNode)
        ///     and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the function declaration</param>
        /// <returns>default(T)</returns>
        public virtual T VisitFunctionDeclaration(ILSLFunctionDeclarationNode node)
        {
            Visit(node.ParameterListNode);
            Visit(node.FunctionBodyNode);

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls VisitState(node)
        /// </summary>
        /// <param name="node">An object describing the state declaration and its scope</param>
        /// <returns>VisitState(node)</returns>
        public virtual T VisitDefaultState(ILSLStateScopeNode node)
        {
            return VisitState(node);
        }



        /// <summary>
        ///     Default implementation calls VisitState(node)
        /// </summary>
        /// <param name="node">An object describing the state declaration and its scope</param>
        /// <returns>VisitState(node)</returns>
        public virtual T VisitDefinedState(ILSLStateScopeNode node)
        {
            return VisitState(node);
        }



        /// <summary>
        ///     Default implementation just returns default(T)
        /// </summary>
        /// <param name="node">An object describing the function/event parameter definition</param>
        /// <returns>default(T)</returns>
        public virtual T VisitParameterDefinition(ILSLParameterNode node)
        {
            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(parameter) on each parameter in node.Parameters, then returns default(T)
        /// </summary>
        /// <param name="node">An object describing the parameter list</param>
        /// <returns>default(T)</returns>
        public virtual T VisitParameterDefinitionList(ILSLParameterListNode node)
        {
            foreach (var parameter in node.Parameters)
            {
                Visit(parameter);
            }

            return default(T);
        }



        /// <summary>
        ///     Default implementation just returns default(T)
        /// </summary>
        /// <param name="node">An object describing the return statement</param>
        /// <returns>default(T)</returns>
        public virtual T VisitReturnStatement(ILSLReturnStatementNode node)
        {
            if (node.HasReturnExpression)
            {
                Visit(node.ReturnExpression);
            }
            return default(T);
        }



        /// <summary>
        ///     Default implementation just returns default(T)
        /// </summary>
        /// <param name="node">An object describing the semi colon statement</param>
        /// <returns>default(T)</returns>
        public virtual T VisitSemiColonStatement(ILSLSemiColonStatement node)
        {
            return default(T);
        }



        /// <summary>
        ///     Default implementation just returns default(T)
        /// </summary>
        /// <param name="node">An object describing the state change statement</param>
        /// <returns>default(T)</returns>
        public virtual T VisitStateChangeStatement(ILSLStateChangeStatementNode node)
        {
            return default(T);
        }



        /// <summary>
        ///     Default implementation just returns default(T)
        /// </summary>
        /// <param name="node">An object describing the jump statement</param>
        /// <returns>default(T)</returns>
        public virtual T VisitJumpStatement(ILSLJumpStatementNode node)
        {
            return default (T);
        }



        /// <summary>
        ///     Default implementation just returns default(T)
        /// </summary>
        /// <param name="node">An object describing the label statement</param>
        /// <returns>default(T)</returns>
        public virtual T VisitLabelStatement(ILSLLabelStatementNode node)
        {
            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.IfStatement), then Visit(elseIfStatement) for each
        ///     LSLElseIfStatement node in node.ElseIfStatements; then if node.HasElseStatement is true it
        ///     calls Visit(node.ElseStatement). Returns default(T) when all sub visits are completed
        /// </summary>
        /// <param name="node">An object describing the control statement</param>
        /// <returns>default(T)</returns>
        public virtual T VisitControlStatement(ILSLControlStatementNode node)
        {
            Visit(node.IfStatement);

            foreach (var elif in node.ElseIfStatements)
            {
                Visit(elif);
            }


            if (node.HasElseStatement)
            {
                Visit(node.ElseStatement);
            }

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.ConditionExpression) then Visit(node.Code) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the if statement</param>
        /// <returns>default(T)</returns>
        public virtual T VisitIfStatement(ILSLIfStatementNode node)
        {
            Visit(node.ConditionExpression);
            Visit(node.Code);


            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.ConditionExpression) then Visit(node.Code) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the else if statement</param>
        /// <returns>default(T)</returns>
        public virtual T VisitElseIfStatement(ILSLElseIfStatementNode node)
        {
            Visit(node.ConditionExpression);
            Visit(node.Code);


            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.Code) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the else statement</param>
        /// <returns>default(T)</returns>
        public virtual T VisitElseStatement(ILSLElseStatementNode node)
        {
            Visit(node.Code);
            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.Expression) and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the expression statement</param>
        /// <returns>default(T)</returns>
        public virtual T VisitExpressionStatement(ILSLExpressionStatementNode node)
        {
            Visit(node.Expression);
            return default (T);
        }



        /// <summary>
        ///     Default implementation calls treeNode.AcceptVisitor(this)
        /// </summary>
        /// <param name="treeNode">The tree node to visit</param>
        /// <returns>treeNode.AcceptVisitor(this)</returns>
        public virtual T Visit(ILSLReadOnlySyntaxTreeNode treeNode)
        {
            return treeNode.AcceptVisitor(this);
        }



        /// <summary>
        ///     Default implementation calls VisitVariableDeclaration(node)
        /// </summary>
        /// <param name="node">An object describing the variable declaration</param>
        /// <returns>VisitVariableDeclaration(node)</returns>
        public virtual T VisitGlobalVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            return VisitVariableDeclaration(node);
        }



        /// <summary>
        ///     Default implementation calls VisitVariableDeclaration(node)
        /// </summary>
        /// <param name="node">An object describing the variable declaration</param>
        /// <returns>VisitVariableDeclaration(node)</returns>
        public virtual T VisitLocalVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            return VisitVariableDeclaration(node);
        }



        /// <summary>
        ///     Default implementation calls VisitCodeScope(node) and returns default(T)
        /// </summary>
        /// <param name="node">An object providing data about the single statement scope being visited</param>
        /// <returns>VisitCodeScope(node)</returns>
        public virtual T VisitSingleStatementCodeScope(ILSLCodeScopeNode node)
        {
            return VisitCodeScope(node);
        }



        /// <summary>
        ///     Default implementation calls VisitCodeScope(node) and returns default(T)
        /// </summary>
        /// <param name="node">An object providing data about the multi statement scope being visited</param>
        /// <returns>VisitCodeScope(node)</returns>
        public virtual T VisitMultiStatementCodeScope(ILSLCodeScopeNode node)
        {
            return VisitCodeScope(node);
        }



        /// <summary>
        ///     Default implementation returns default(T)
        /// </summary>
        /// <param name="lslHexLiteralNode">An object describing a hex literal in an expression tree</param>
        /// <returns>default(T)</returns>
        public virtual T VisitHexLiteral(ILSLHexLiteralNode lslHexLiteralNode)
        {
            return default(T);
        }



        /// <summary>
        ///     Default implementation calls VisitUserFunctionCallParameters(lslExpressionListNode)
        /// </summary>
        /// <param name="lslExpressionListNode">
        ///     An object providing data about the expression list, in the context of parameters
        ///     in a call to a standard library function
        /// </param>
        /// <returns>VisitFunctionCallParameters(lslExpressionListNode)</returns>
        public virtual T VisitLibraryFunctionCallParameters(ILSLExpressionListNode lslExpressionListNode)
        {
            return VisitFunctionCallParameters(lslExpressionListNode);
        }



        /// <summary>
        ///     Default implementation calls VisitUserFunctionCallParameters(lslExpressionListNode)
        /// </summary>
        /// <param name="lslExpressionListNode">
        ///     An object providing data about the expression list, in the context of parameters
        ///     in a call to a user defined function
        /// </param>
        /// <returns>VisitFunctionCallParameters(lslExpressionListNode)</returns>
        public virtual T VisitUserFunctionCallParameters(ILSLExpressionListNode lslExpressionListNode)
        {
            return VisitFunctionCallParameters(lslExpressionListNode);
        }



        /// <summary>
        ///     Default implementation calls VisitVariableReference(lslVariableNode)
        /// </summary>
        /// <param name="lslVariableNode">An object providing data about a global variable reference in an expression tree</param>
        /// <returns>VisitVariableReference(lslVariableNode)</returns>
        public virtual T VisitGlobalVariableReference(ILSLVariableNode lslVariableNode)
        {
            return VisitVariableReference(lslVariableNode);
        }



        /// <summary>
        ///     Default implementation calls VisitVariableReference(lslVariableNode)
        /// </summary>
        /// <param name="lslVariableNode">An object providing data about a local variable reference in an expression tree</param>
        /// <returns>VisitVariableReference(lslVariableNode)</returns>
        public virtual T VisitLocalVariableReference(ILSLVariableNode lslVariableNode)
        {
            return VisitVariableReference(lslVariableNode);
        }



        /// <summary>
        ///     Default implementation calls VisitVariableReference(lslVariableNode)
        /// </summary>
        /// <param name="lslVariableNode">An object providing data about a parameter variable reference in an expression tree</param>
        /// <returns>VisitVariableReference(lslVariableNode)</returns>
        public virtual T VisitParameterVariableReference(ILSLVariableNode lslVariableNode)
        {
            return VisitVariableReference(lslVariableNode);
        }



        /// <summary>
        ///     Default implementation calls VisitVariableReference(lslVariableNode)
        /// </summary>
        /// <param name="lslVariableNode">An object providing data about a library constant reference in an expression tree</param>
        /// <returns>VisitVariableReference(lslVariableNode)</returns>
        public virtual T VisitLibraryConstantVariableReference(ILSLVariableNode lslVariableNode)
        {
            return VisitVariableReference(lslVariableNode);
        }



        /// <summary>
        ///     Default implementation calls VisitFunctionCall(lslFunctionCallNode)
        /// </summary>
        /// <param name="lslFunctionCallNode">
        ///     An object describing a function call in an expression tree,
        ///     in the context of a standard library defined function.
        /// </param>
        /// <returns>VisitFunctionCall(lslFunctionCallNode)</returns>
        public virtual T VisitLibraryFunctionCall(ILSLFunctionCallNode lslFunctionCallNode)
        {
            return VisitFunctionCall(lslFunctionCallNode);
        }



        /// <summary>
        ///     Default implementation calls VisitFunctionCall(lslFunctionCallNode)
        /// </summary>
        /// <param name="lslFunctionCallNode">
        ///     An object describing a function call in an expression tree,
        ///     in the context of a user defined function.
        /// </param>
        /// <returns>VisitFunctionCall(lslFunctionCallNode)</returns>
        public virtual T VisitUserFunctionCall(ILSLFunctionCallNode lslFunctionCallNode)
        {
            return VisitFunctionCall(lslFunctionCallNode);
        }



        /// <summary>
        ///     Default implementation calls Visit(expression) for each expression in node.ExpressionNodes, and returns default(T)
        /// </summary>
        /// <param name="node">An object describing a list of expression</param>
        /// <returns>default(T)</returns>
        public virtual T VisitExpressionList(ILSLExpressionListNode node)
        {
            foreach (var exp in node.ExpressionNodes)
            {
                Visit(exp);
            }


            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(eventHandler) for each eventHandler in node.EventHandlers, then returns
        ///     default(T)
        /// </summary>
        /// <param name="node">An object describing the state declaration and its scope</param>
        /// <returns>default(T)</returns>
        public virtual T VisitState(ILSLStateScopeNode node)
        {
            foreach (var ev in node.EventHandlers)
            {
                Visit(ev);
            }

            return default(T);
        }



        /// <summary>
        ///     Default implementation calls Visit(node.DeclarationExpression) if node.HasDeclarationExpression is true
        ///     and returns default(T)
        /// </summary>
        /// <param name="node">An object describing the variable declaration</param>
        /// <returns>default(T)</returns>
        public virtual T VisitVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            if (node.HasDeclarationExpression)
            {
                Visit(node.DeclarationExpression);
            }

            return default(T);
        }
    }
}