using LibLSLCC.CodeValidator.ValidatorNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

namespace LibLSLCC.CodeValidator.ValidatorNodeVisitor
{
    public interface ILSLValidatorNodeVisitor<out T>
    {
        T VisitBinaryExpression(ILSLBinaryExpressionNode node);


        T VisitListLiteralInitializerList(ILSLExpressionListNode node);

        T VisitFunctionCallParameters(ILSLExpressionListNode node);

        T VisitForLoopAfterthoughts(ILSLExpressionListNode node);

        T VisitForLoopInitExpressions(ILSLExpressionListNode node);


        T VisitFloatLiteral(ILSLFloatLiteralNode node);

        T VisitFunctionCall(ILSLFunctionCallNode node);

        T VisitIntegerLiteral(ILSLIntegerLiteralNode node);

        T VisitListLiteral(ILSLListLiteralNode node);

        T VisitParenthesizedExpression(ILSLParenthesizedExpressionNode node);

        T VisitPostfixOperation(ILSLPostfixOperationNode node);

        T VisitPrefixOperation(ILSLPrefixOperationNode node);


        T VisitRotationLiteral(ILSLRotationLiteralNode node);

        T VisitStringLiteral(ILSLStringLiteralNode node);

        T VisitTypecastExpression(ILSLTypecastExprNode node);

        T VisitVariableReference(ILSLVariableNode node);

        T VisitVecRotAccessor(ILSLTupleAccessorNode node);

        T VisitVectorLiteral(ILSLVectorLiteralNode node);

        T VisitDoLoop(ILSLDoLoopNode node);

        T VisitForLoop(ILSLForLoopNode node);

        T VisitWhileLoop(ILSLWhileLoopNode node);

        T VisitCodeScope(ILSLCodeScopeNode node);

        T VisitCompilationUnit(ILSLCompilationUnitNode unode);

        T VisitEventHandler(ILSLEventHandlerNode node);

        T VisitFunctionDeclaration(ILSLFunctionDeclarationNode node);

        T VisitDefaultState(ILSLStateScopeNode node);

        T VisitDefinedState(ILSLStateScopeNode node);

        T VisitParameterDefinition(ILSLParameterNode node);

        T VisitParameterDefinitionList(ILSLParameterListNode node);

        T VisitReturnStatement(ILSLReturnStatementNode node);

        T VisitSemiColonStatement(ILSLSemiColonStatement node);

        T VisitStateChangeStatement(ILSLStateChangeStatementNode node);

        T VisitJumpStatement(ILSLJumpStatementNode node);

        T VisitLabelStatement(ILSLLabelStatementNode node);

        T VisitControlStatement(ILSLControlStatementNode node);

        T VisitIfStatement(ILSLIfStatementNode node);

        T VisitElseIfStatement(ILSLElseIfStatementNode node);

        T VisitElseStatement(ILSLElseStatementNode node);

        T VisitExpressionStatement(ILSLExpressionStatementNode node);


        T Visit(ILSLReadOnlySyntaxTreeNode treeNode);


        T VisitGlobalVariableDeclaration(ILSLVariableDeclarationNode node);


        T VisitLocalVariableDeclaration(ILSLVariableDeclarationNode node);


        T VisitSingleStatementCodeScope(ILSLCodeScopeNode lslCodeScopeNode);


        T VisitMultiStatementCodeScope(ILSLCodeScopeNode lslCodeScopeNode);


        T VisitHexLiteral(ILSLHexLiteralNode lslHexLiteralNode);


        T VisitLibraryFunctionCallParameters(ILSLExpressionListNode lslExpressionListNode);


        T VisitUserFunctionCallParameters(ILSLExpressionListNode lslExpressionListNode);


        T VisitGlobalVariableReference(ILSLVariableNode lslVariableNode);


        T VisitLocalVariableReference(ILSLVariableNode lslVariableNode);


        T VisitParameterVariableReference(ILSLVariableNode lslVariableNode);

        T VisitLibraryConstantVariableReference(ILSLVariableNode lslVariableNode);

        T VisitLibraryFunctionCall(ILSLFunctionCallNode lslFunctionCallNode);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "lsl")]
        T VisitUserFunctionCall(ILSLFunctionCallNode lslFunctionCallNode);
    }
}