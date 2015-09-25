#region FileInfo

// 
// File: LSLValidatorNodeVisitor.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:25 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Imports

using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.ValidatorNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

#endregion

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

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "lsl")]
        T VisitUserFunctionCall(ILSLFunctionCallNode lslFunctionCallNode);
    }
}