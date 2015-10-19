#region FileInfo
// 
// File: LSLValidatorNodeVisitor.cs
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

using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.ValidatorNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodeVisitor
{
    /// <summary>
    /// Interface for LSL Syntax tree visitors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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

        T VisitUserFunctionCall(ILSLFunctionCallNode lslFunctionCallNode);
    }
}