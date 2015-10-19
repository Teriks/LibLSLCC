#region FileInfo
// 
// File: LSLExprNodeExtensions.cs
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

using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    /// <summary>
    /// Various extensions for dealing with syntax tree expression nodes.
    /// </summary>
    public static class LSLExprNodeExtensions
    {
        /// <summary>
        /// Determines if the expression node represents a code literal.  Such as a string, vector, rotation or list literal.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a code literal.</returns>
        public static bool IsLiteral(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.Literal;
        }


        /// <summary>
        /// Determines if the expression node represents a compound expression.
        /// 
        /// Compound expressions are:
        /// 
        /// Binary Expressions
        /// Parenthesized Expressions
        /// Postfix Expressions
        /// Prefix Expressions
        /// Typecast Expressions
        /// 
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a compound expression.</returns>
        public static bool IsCompoundExpression(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.BinaryExpression ||
                   node.ExpressionType == LSLExpressionType.ParenthesizedExpression ||
                   node.ExpressionType == LSLExpressionType.PostfixExpression ||
                   node.ExpressionType == LSLExpressionType.PrefixExpression ||
                   node.ExpressionType == LSLExpressionType.TypecastExpression;
        }

        /// <summary>
        /// Determines if an expression node represents a function call to a user defined or library defined function.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a function call to either a user defined or library defined function.</returns>
        public static bool IsFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.LibraryFunction ||
                   node.ExpressionType == LSLExpressionType.UserFunction;
        }


        /// <summary>
        /// Determines if an expression node represents a function call to a library defined function.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a function call to a library defined function.</returns>
        public static bool IsLibraryFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.LibraryFunction;
        }


        /// <summary>
        /// Determines if an expression node represents a function call to a user defined function.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a function call to a user defined function.</returns>
        public static bool IsUserFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.UserFunction;
        }

        /// <summary>
        /// Determines if an expression node represents a reference to a global or local variable.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a reference to either a global or local variable.</returns>
        public static bool IsVariable(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.GlobalVariable ||
                   node.ExpressionType == LSLExpressionType.LocalVariable;
        }


        /// <summary>
        /// Determines if an expression node represents a reference to a local variable.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a reference to a local variable.</returns>
        public static bool IsLocalVariable(this ILSLReadOnlyExprNode node)
        {
            return
                node.ExpressionType == LSLExpressionType.LocalVariable;
        }


        /// <summary>
        /// Determines if an expression node represents a reference to a local parameter.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a reference to a local parameter.</returns>
        public static bool IsLocalParameter(this ILSLReadOnlyExprNode node)
        {
            return
                node.ExpressionType == LSLExpressionType.ParameterVariable;
        }


        /// <summary>
        /// Determines if an expression node represents a reference to a global variable.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a reference to a global variable.</returns>
        public static bool IsGlobalVariable(this ILSLReadOnlyExprNode node)
        {
            return
                node.ExpressionType == LSLExpressionType.GlobalVariable;
        }
    }
}