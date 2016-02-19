#region FileInfo
// 
// File: LSLDefaultValidatorNodeVisitor.cs
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

namespace LibLSLCC.CodeValidator.ValidatorNodeVisitor
{
    /// <summary>
    /// Default visitor base class for LSL Syntax tree visitors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LSLValidatorNodeVisitor<T> : ILSLValidatorNodeVisitor<T>
    {
        /// <summary>
        /// Visits a binary expression node in an LSL syntax tree during tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitBinaryExpression(ILSLBinaryExpressionNode node)
        {
            Visit(node.LeftExpression);
            Visit(node.RightExpression);


            return default(T);
        }


        /// <summary>
        /// Visits an expression list node inside of a list literal initializer during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitExpressionList">VisitListLiteralInitializerList makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitListLiteralInitializerList(ILSLExpressionListNode node)
        {
            return VisitExpressionList(node);
        }


        /// <summary>
        /// Visits an expression list node representing a function calls parameters during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitExpressionList">VisitListLiteralInitializerList makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitFunctionCallParameters(ILSLExpressionListNode node)
        {
            return VisitExpressionList(node);
        }


        /// <summary>
        /// Visits an expression list node representing a for loops afterthought expressions during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitExpressionList">VisitForLoopAfterthoughts makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitForLoopAfterthoughts(ILSLExpressionListNode node)
        {
            return VisitExpressionList(node);
        }


        /// <summary>
        /// Visits an expression list node representing a for loops initializer expressions during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitExpressionList">VisitForLoopInitExpressions makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitForLoopInitExpressions(ILSLExpressionListNode node)
        {
            return VisitExpressionList(node);
        }


        /// <summary>
        /// Visits a float literal token node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitFloatLiteral(ILSLFloatLiteralNode node)
        {
            return default(T);
        }


        /// <summary>
        /// Visits a function call node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitLibraryFunctionCall">VisitLibraryFunctionCall calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitUserFunctionCall">VisitUserFunctionCall calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitFunctionCall(ILSLFunctionCallNode node)
        {
            Visit(node.ParamExpressionListNode);

            return default(T);
        }


        /// <summary>
        /// Visits an integer literal token node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitIntegerLiteral(ILSLIntegerLiteralNode node)
        {
            return default(T);
        }


        /// <summary>
        /// Visits a list literal expression during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitListLiteral(ILSLListLiteralNode node)
        {
            Visit(node.ExpressionListNode);

            return default(T);
        }


        /// <summary>
        /// Visits a parenthesized expression during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitParenthesizedExpression(ILSLParenthesizedExpressionNode node)
        {
            Visit(node.InnerExpression);

            return default(T);
        }


        /// <summary>
        /// Visits a postfix operation expression during a syntax tree traversal.
        /// This occurs for the postfix increment and decrement operators.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitPostfixOperation(ILSLPostfixOperationNode node)
        {
            Visit(node.LeftExpression);

            return default(T);
        }


        /// <summary>
        /// Visits a prefix operation expression during a syntax tree traversal.
        /// This includes the standard prefix decrement/increment operators, as well as negation/positive specification
        /// and the bitwise/logical NOT prefix operators.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitPrefixOperation(ILSLPrefixOperationNode node)
        {
            Visit(node.RightExpression);

            return default(T);
        }


        /// <summary>
        /// Visits a rotation literal expression node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitRotationLiteral(ILSLRotationLiteralNode node)
        {
            Visit(node.XExpression);
            Visit(node.YExpression);
            Visit(node.ZExpression);
            Visit(node.SExpression);

            return default(T);
        }


        /// <summary>
        /// Visits a string literal token node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitStringLiteral(ILSLStringLiteralNode node)
        {
            return default(T);
        }


        /// <summary>
        /// Visits a typecast expression node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitTypecastExpression(ILSLTypecastExprNode node)
        {
            Visit(node.CastedExpression);

            return default(T);
        }


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
        public virtual T VisitVariableReference(ILSLVariableNode node)
        {
            return default(T);
        }


        /// <summary>
        /// Visit a dot operator (member access) expression used on a vector or rotation.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitVecRotAccessor(ILSLTupleAccessorNode node)
        {
            Visit(node.AccessedExpression);

            return default(T);
        }


        /// <summary>
        /// Visits a vector literal expression node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitVectorLiteral(ILSLVectorLiteralNode node)
        {
            Visit(node.XExpression);
            Visit(node.YExpression);
            Visit(node.ZExpression);

            return default(T);
        }


        /// <summary>
        /// Visits a do-loop statement node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitDoLoop(ILSLDoLoopNode node)
        {
            Visit(node.Code);
            Visit(node.ConditionExpression);
            return default(T);
        }


        /// <summary>
        /// Visits a for-loop statement node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
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
        /// Visits a while-loop statement node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitWhileLoop(ILSLWhileLoopNode node)
        {
            Visit(node.ConditionExpression);
            Visit(node.Code);
            return default(T);
        }


        /// <summary>
        /// Visits a code scope node during syntax tree traversal.
        /// This should occur for all types of code scope.
        /// This includes: function/event handler code body's,  control/loop statement body's that either have or lack braces, and anonymous code scopes declared inside of a parent code scope.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitMultiStatementCodeScope">VisitMultiStatementCodeScope calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitSingleStatementCodeScope">VisitMultiStatementCodeScope calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitCodeScope(ILSLCodeScopeNode node)
        {
            foreach (var st in node.CodeStatements)
            {
                Visit(st);
            }

            return default(T);
        }


        /// <summary>
        /// Visits the top level of an LSL compilation unit node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
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
        /// Visits and event handler usage node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitEventHandler(ILSLEventHandlerNode node)
        {
            Visit(node.ParameterListNode);
            Visit(node.EventBodyNode);

            return default(T);
        }


        /// <summary>
        /// Visits the declaration node of a user defined function during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitFunctionDeclaration(ILSLFunctionDeclarationNode node)
        {
            Visit(node.ParameterListNode);
            Visit(node.FunctionBodyNode);

            return default(T);
        }


        /// <summary>
        /// Visits the default script state node during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitDefaultState(ILSLStateScopeNode node)
        {
            return VisitState(node);
        }


        /// <summary>
        /// Visits user defined script states during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitDefinedState(ILSLStateScopeNode node)
        {
            return VisitState(node);
        }


        /// <summary>
        /// Visits a parameter definition of either a user defined function or event handler during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitParameterDefinitionList">VisitParameterDefinitionList calls this method while visiting its children.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitParameterDefinition(ILSLParameterNode node)
        {
            return default(T);
        }


        /// <summary>
        /// Visits a parameter list node of either a user defined function or event handler during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitParameterDefinition">VisitParameterDefinition is a child call of this visitor method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitEventHandler">VisitEventHandler calls this method while visiting its parameters.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionDeclaration">VisitFunctionDeclaration calls this method while visiting its parameters.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitParameterDefinitionList(ILSLParameterListNode node)
        {
            foreach (var parameter in node.Parameters)
            {
                Visit(parameter);
            }

            return default(T);
        }


        /// <summary>
        /// Visits a return statement inside of a user defined event or event handler during syntax tree traversal.
        /// If the return statement returns an expression, that is visited to.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitReturnStatement(ILSLReturnStatementNode node)
        {
            if (node.HasReturnExpression)
            {
                Visit(node.ReturnExpression);
            }
            return default(T);
        }


        /// <summary>
        /// Visits a (vestigial) semi-colon statement during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitSemicolonStatement(ILSLSemicolonStatement node)
        {
            return default(T);
        }


        /// <summary>
        /// Visits a state change statement during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitStateChangeStatement(ILSLStateChangeStatementNode node)
        {
            return default(T);
        }


        /// <summary>
        /// Visits a jump statement during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitJumpStatement(ILSLJumpStatementNode node)
        {
            return default(T);
        }


        /// <summary>
        /// Visits a label statement during syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitLabelStatement(ILSLLabelStatementNode node)
        {
            return default(T);
        }


        /// <summary>
        /// Visits the top level of a control statement chain during a syntax tree traversal.
        /// <see cref="ILSLControlStatementNode"/> contains IF, ELSE-IF, and ELSE nodes as its children.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitIfStatement">VisitIfStatement is a child call of this visitor method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitElseIfStatement">VisitElseIfStatement is a child call of this visitor method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitElseStatement">VisitElseStatement is a child call of this visitor method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
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
        /// Visits an if statement node during a syntax tree traversal.
        /// If statement nodes are children of <see cref="ILSLControlStatementNode"/> instances.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitControlStatement">VisitControlStatement should call this function while visiting its children.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitIfStatement(ILSLIfStatementNode node)
        {
            Visit(node.ConditionExpression);
            Visit(node.Code);


            return default(T);
        }


        /// <summary>
        /// Visits an else-if statement node during a syntax tree traversal.
        /// Else-If statement nodes are children of <see cref="ILSLControlStatementNode"/> instances.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitControlStatement">VisitControlStatement should call this function while visiting its children.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitElseIfStatement(ILSLElseIfStatementNode node)
        {
            Visit(node.ConditionExpression);
            Visit(node.Code);


            return default(T);
        }


        /// <summary>
        /// Visits an else statement node during a syntax tree traversal.
        /// Else statement nodes are children of <see cref="ILSLControlStatementNode"/> instances.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitControlStatement">VisitControlStatement should call this function while visiting its children.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitElseStatement(ILSLElseStatementNode node)
        {
            Visit(node.Code);
            return default(T);
        }


        /// <summary>
        /// Visits an expression statement node during a syntax tree traversal.
        /// Expression statement nodes are the individual statements that occur sequentially inside of a code scope.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitExpressionStatement(ILSLExpressionStatementNode node)
        {
            Visit(node.Expression);
            return default(T);
        }


        /// <summary>
        /// The generic visit function for the syntax tree visitor.  It should delegate to treeNode.AcceptVisitor(this).
        /// </summary>
        /// <param name="treeNode">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T Visit(ILSLReadOnlySyntaxTreeNode treeNode)
        {
            return treeNode.AcceptVisitor(this);
        }


        /// <summary>
        /// Visits a global variable declaration during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableDeclaration">VisitGlobalVariableDeclaration makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitGlobalVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            return VisitVariableDeclaration(node);
        }


        /// <summary>
        /// Visits a local variable declaration inside of a user defined function or event handler during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableDeclaration">VisitLocalVariableDeclaration makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitLocalVariableDeclaration(ILSLVariableDeclarationNode node)
        {
            return VisitVariableDeclaration(node);
        }


        /// <summary>
        /// Visits a brace-less code scope during a syntax tree traversal.
        /// Brace-less code scopes can occur as the code body for if/else-if and else statements, as well for all types of loop statements. (for/while/do-while).
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitCodeScope">VisitSingleStatementCodeScope makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitSingleStatementCodeScope(ILSLCodeScopeNode node)
        {
            return VisitCodeScope(node);
        }


        /// <summary>
        /// Visit a standard multi-statement code scope during a syntax tree traversal.
        /// Multi-statement code scopes will always be: a function/event handler code body, the body of a control/loop statement, or an anonymous code scope declared inside of a parent code scope.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitCodeScope">VisitMultiStatementCodeScope makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitMultiStatementCodeScope(ILSLCodeScopeNode node)
        {
            return VisitCodeScope(node);
        }


        /// <summary>
        /// Visits a hex literal token node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitHexLiteral(ILSLHexLiteralNode node)
        {
            return default(T);
        }


        /// <summary>
        /// Visits the function call parameter list node used for a call to a library defined function during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionCallParameters">VisitLibraryFunctionCallParameters makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitLibraryFunctionCallParameters(ILSLExpressionListNode node)
        {
            return VisitFunctionCallParameters(node);
        }


        /// <summary>
        /// Visits the function call parameter list node used for a call to a user defined function during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionCallParameters">VisitUserFunctionCallParameters makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitUserFunctionCallParameters(ILSLExpressionListNode node)
        {
            return VisitFunctionCallParameters(node);
        }


        /// <summary>
        /// Visits a variable node representing a reference to a global variable during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableReference">VisitGlobalVariableReference makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitGlobalVariableReference(ILSLVariableNode node)
        {
            return VisitVariableReference(node);
        }


        /// <summary>
        /// Visits a variable node representing a reference to a local variable during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableReference">VisitLocalVariableReference makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitLocalVariableReference(ILSLVariableNode node)
        {
            return VisitVariableReference(node);
        }


        /// <summary>
        /// Visits a variable node representing a reference to a locally defined function or event handler parameter during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableReference">VisitParameterVariableReference makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitParameterVariableReference(ILSLVariableNode node)
        {
            return VisitVariableReference(node);
        }


        /// <summary>
        /// Visits a variable node representing a reference to a library defined constant during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitVariableReference">VisitLibraryConstantVariableReference makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitLibraryConstantVariableReference(ILSLVariableNode node)
        {
            return VisitVariableReference(node);
        }


        /// <summary>
        /// Visits a function call node representing a call to a library defined function during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionCall">VisitLibraryFunctionCall makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitLibraryFunctionCall(ILSLFunctionCallNode node)
        {
            return VisitFunctionCall(node);
        }


        /// <summary>
        /// Visits a function call node representing a call to a user defined function during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionCall">VisitUserFunctionCall makes a call to the method seen here.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitUserFunctionCall(ILSLFunctionCallNode node)
        {
            return VisitFunctionCall(node);
        }




        /// <summary>
        /// Visit an expression list node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitFunctionCallParameters">VisitFunctionCallParameters calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitListLiteralInitializerList">VisitListLiteralInitializerList calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitForLoopInitExpressions">VisitForLoopInitExpressions calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitForLoopAfterthoughts">VisitForLoopAfterthoughts calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitExpressionList(ILSLExpressionListNode node)
        {
            foreach (var exp in node.ExpressionNodes)
            {
                Visit(exp);
            }


            return default(T);
        }

        /// <summary>
        /// Visit a code state definition node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitDefaultState">VisitDefaultState calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitDefinedState">VisitDefinedState calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
        public virtual T VisitState(ILSLStateScopeNode node)
        {
            foreach (var ev in node.EventHandlers)
            {
                Visit(ev);
            }

            return default(T);
        }



        /// <summary>
        /// Visit a variable declaration node during a syntax tree traversal.
        /// </summary>
        /// <param name="node">The Syntax Tree Node.</param>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitGlobalVariableDeclaration">VisitGlobalVariableDeclaration calls this method.</seealso>
        /// <seealso cref="LSLValidatorNodeVisitor{T}.VisitLocalVariableDeclaration">VisitLocalVariableDeclaration calls this method.</seealso>
        /// <returns>An object of type (T) from the visitor implementation of this function.</returns>
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