#region FileInfo
// 
// File: ILSLValidatorNodeVisitor.cs
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

using LibLSLCC.CodeValidator.Nodes.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator.Visitor
{
    /// <summary>
    /// Interface for LSL Syntax tree visitors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILSLValidatorNodeVisitor<out T>
    {

        /// <summary>
        /// Visits a binary expression node in an LSL syntax tree during tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitBinaryExpression(ILSLBinaryExpressionNode node);


        /// <summary>
        /// Visits an expression list node inside of a list literal initializer during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitExpressionList">VisitListLiteralInitializerList calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitListLiteralInitializerList(ILSLExpressionListNode node);



        /// <summary>
        /// Visits an expression list node representing a function calls parameters during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitExpressionList">VisitListLiteralInitializerList calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitFunctionCallParameters(ILSLExpressionListNode node);



        /// <summary>
        /// Visits an expression list node representing a for loops afterthought expressions during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitExpressionList">VisitForLoopAfterthoughts calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitForLoopAfterthoughts(ILSLExpressionListNode node);


        /// <summary>
        /// Visits an expression list node representing a for loops initializer expressions during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitExpressionList">VisitForLoopInitExpressions calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitForLoopInitExpressions(ILSLExpressionListNode node);


        /// <summary>
        /// Visits a float literal token node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitFloatLiteral(ILSLFloatLiteralNode node);


        /// <summary>
        /// Visits a function call node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitLibraryFunctionCall">VisitLibraryFunctionCall calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitUserFunctionCall">VisitUserFunctionCall calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitFunctionCall(ILSLFunctionCallNode node);


        /// <summary>
        /// Visits an integer literal token node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitIntegerLiteral(ILSLIntegerLiteralNode node);


        /// <summary>
        /// Visits a list literal expression during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitListLiteral(ILSLListLiteralNode node);


        /// <summary>
        /// Visits a parenthesized expression during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitParenthesizedExpression(ILSLParenthesizedExpressionNode node);



        /// <summary>
        /// Visits a postfix operation expression during a syntax tree traversal.
        /// This occurs for the postfix increment and decrement operators.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitPostfixOperation(ILSLPostfixOperationNode node);


        /// <summary>
        /// Visits a prefix operation expression during a syntax tree traversal.
        /// This includes the standard prefix decrement/increment operators, as well as negation/positive specification
        /// and the bitwise/logical NOT prefix operators.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitPrefixOperation(ILSLPrefixOperationNode node);

        /// <summary>
        /// Visits a rotation literal expression node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitRotationLiteral(ILSLRotationLiteralNode node);


        /// <summary>
        /// Visits a string literal token node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitStringLiteral(ILSLStringLiteralNode node);


        /// <summary>
        /// Visits a typecast expression node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitTypecastExpression(ILSLTypecastExprNode node);


        /// <summary>
        /// Visits a variable reference node during syntax tree traversal.
        /// This occurs for: parameters, local/global variables and library constants.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitLocalVariableReference">VisitLocalVariableReference calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitGlobalVariableReference">VisitGlobalVariableReference calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitLibraryConstantVariableReference">VisitLibraryConstantVariableReference calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitParameterVariableReference">VisitParameterVariableReference calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitVariableReference(ILSLVariableNode node);


        /// <summary>
        /// Visit a dot operator (member access) expression used on a vector or rotation.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitVecRotAccessor(ILSLTupleAccessorNode node);


        /// <summary>
        /// Visits a vector literal expression node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitVectorLiteral(ILSLVectorLiteralNode node);


        /// <summary>
        /// Visits a do-loop statement node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitDoLoop(ILSLDoLoopNode node);


        /// <summary>
        /// Visits a for-loop statement node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitForLoop(ILSLForLoopNode node);


        /// <summary>
        /// Visits a while-loop statement node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitWhileLoop(ILSLWhileLoopNode node);


        /// <summary>
        /// Visits a code scope node during syntax tree traversal.
        /// This should occur for all types of code scope.
        /// This includes: function/event handler code body's,  control/loop statement body's that either have or lack braces, and anonymous code scopes declared inside of a parent code scope.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitMultiStatementCodeScope">VisitMultiStatementCodeScope calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitSingleStatementCodeScope">VisitMultiStatementCodeScope calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitCodeScope(ILSLCodeScopeNode node);

        /// <summary>
        /// Visits the top level of an LSL compilation unit node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitCompilationUnit(ILSLCompilationUnitNode node);


        /// <summary>
        /// Visits and event handler usage node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitEventHandler(ILSLEventHandlerNode node);


        /// <summary>
        /// Visits the declaration node of a user defined function during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitFunctionDeclaration(ILSLFunctionDeclarationNode node);


        /// <summary>
        /// Visits the default script state node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitDefaultState(ILSLStateScopeNode node);



        /// <summary>
        /// Visits user defined script states during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitDefinedState(ILSLStateScopeNode node);


        /// <summary>
        /// Visits a parameter definition of either a user defined function or event handler during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitParameterDefinitionList">VisitParameterDefinitionList calls this method while visiting its children.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitParameterDefinition(ILSLParameterNode node);


        /// <summary>
        /// Visits a parameter list node of either a user defined function or event handler during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitParameterDefinition">VisitParameterDefinition is a child call of this visitor method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitEventHandler">VisitEventHandler calls this method while visiting its parameters.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionDeclaration">VisitFunctionDeclaration calls this method while visiting its parameters.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitParameterDefinitionList(ILSLParameterListNode node);


        /// <summary>
        /// Visits a return statement inside of a user defined event or event handler during syntax tree traversal.
        /// If the return statement returns an expression, that is visited to.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitReturnStatement(ILSLReturnStatementNode node);

        /// <summary>
        /// Visits a (vestigial) semi-colon statement during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitSemicolonStatement(ILSLSemicolonStatement node);


        /// <summary>
        /// Visits a state change statement during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitStateChangeStatement(ILSLStateChangeStatementNode node);


        /// <summary>
        /// Visits a jump statement during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitJumpStatement(ILSLJumpStatementNode node);


        /// <summary>
        /// Visits a label statement during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitLabelStatement(ILSLLabelStatementNode node);

        /// <summary>
        /// Visits the top level of a control statement chain during a syntax tree traversal.
        /// <see cref="ILSLControlStatementNode"/> contains IF, ELSE-IF, and ELSE nodes as its children.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitIfStatement">VisitIfStatement is a child call of this visitor method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitElseIfStatement">VisitElseIfStatement is a child call of this visitor method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitElseStatement">VisitElseStatement is a child call of this visitor method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitControlStatement(ILSLControlStatementNode node);


        /// <summary>
        /// Visits an if statement node during a syntax tree traversal.
        /// If statement nodes are children of <see cref="ILSLControlStatementNode"/> instances.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitControlStatement">VisitControlStatement should call this function while visiting its children.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitIfStatement(ILSLIfStatementNode node);


        /// <summary>
        /// Visits an else-if statement node during a syntax tree traversal.
        /// Else-If statement nodes are children of <see cref="ILSLControlStatementNode"/> instances.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitControlStatement">VisitControlStatement should call this function while visiting its children.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitElseIfStatement(ILSLElseIfStatementNode node);



        /// <summary>
        /// Visits an else statement node during a syntax tree traversal.
        /// Else statement nodes are children of <see cref="ILSLControlStatementNode"/> instances.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitControlStatement">VisitControlStatement should call this function while visiting its children.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitElseStatement(ILSLElseStatementNode node);

        /// <summary>
        /// Visits an expression statement node during a syntax tree traversal.
        /// Expression statement nodes are the individual statements that occur sequentially inside of a code scope.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitExpressionStatement(ILSLExpressionStatementNode node);


        /// <summary>
        /// The generic visit function for the syntax tree visitor.  It should delegate to treeNode.AcceptVisitor(this).
        /// </summary>
        /// <param name="treeNode">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T Visit(ILSLReadOnlySyntaxTreeNode treeNode);


        /// <summary>
        /// Visits a global variable declaration during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableDeclaration">VisitGlobalVariableDeclaration calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitGlobalVariableDeclaration(ILSLVariableDeclarationNode node);


        /// <summary>
        /// Visits a local variable declaration inside of a user defined function or event handler during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableDeclaration">VisitLocalVariableDeclaration calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitLocalVariableDeclaration(ILSLVariableDeclarationNode node);


        /// <summary>
        /// Visits a brace-less code scope during a syntax tree traversal.
        /// Brace-less code scopes can occur as the code body for if/else-if and else statements, as well for all types of loop statements. (for/while/do-while).
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitCodeScope">VisitSingleStatementCodeScope calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitSingleStatementCodeScope(ILSLCodeScopeNode node);


        /// <summary>
        /// Visit a standard multi-statement code scope during a syntax tree traversal.
        /// Multi-statement code scopes will always be: a function/event handler code body, the body of a control/loop statement, or an anonymous code scope declared inside of a parent code scope.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitCodeScope">VisitMultiStatementCodeScope calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitMultiStatementCodeScope(ILSLCodeScopeNode node);


        /// <summary>
        /// Visits a hex literal token node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitHexLiteral(ILSLHexLiteralNode node);


        /// <summary>
        /// Visits the function call parameter list node used for a call to a library defined function during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionCallParameters">VisitLibraryFunctionCallParameters calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitLibraryFunctionCallParameters(ILSLExpressionListNode node);


        /// <summary>
        /// Visits the function call parameter list node used for a call to a user defined function during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionCallParameters">VisitUserFunctionCallParameters calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitUserFunctionCallParameters(ILSLExpressionListNode node);


        /// <summary>
        /// Visits a variable node representing a reference to a global variable during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableReference">VisitGlobalVariableReference calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitGlobalVariableReference(ILSLVariableNode node);


        /// <summary>
        /// Visits a variable node representing a reference to a local variable during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableReference">VisitLocalVariableReference calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitLocalVariableReference(ILSLVariableNode node);


        /// <summary>
        /// Visits a variable node representing a reference to a locally defined function or event handler parameter during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableReference">VisitParameterVariableReference calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitParameterVariableReference(ILSLVariableNode node);


        /// <summary>
        /// Visits a variable node representing a reference to a library defined constant during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableReference">VisitLibraryConstantVariableReference calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitLibraryConstantVariableReference(ILSLVariableNode node);


        /// <summary>
        /// Visits a function call node representing a call to a library defined function during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionCall">VisitLibraryFunctionCall calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitLibraryFunctionCall(ILSLFunctionCallNode node);


        /// <summary>
        /// Visits a function call node representing a call to a user defined function during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionCall">VisitUserFunctionCall calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitUserFunctionCall(ILSLFunctionCallNode node);


        /// <summary>
        /// Visit an expression list node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionCallParameters">VisitFunctionCallParameters calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitListLiteralInitializerList">VisitListLiteralInitializerList calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitForLoopInitExpressions">VisitForLoopInitExpressions calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitForLoopAfterthoughts">VisitForLoopAfterthoughts calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitExpressionList(ILSLExpressionListNode node);


        /// <summary>
        /// Visit a code state definition node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitDefaultState">VisitDefaultState calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitDefinedState">VisitDefinedState calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitState(ILSLStateScopeNode node);



        /// <summary>
        /// Visit a variable declaration node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitGlobalVariableDeclaration">VisitGlobalVariableDeclaration calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitLocalVariableDeclaration">VisitLocalVariableDeclaration calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        T VisitVariableDeclaration(ILSLVariableDeclarationNode node);
    }
}