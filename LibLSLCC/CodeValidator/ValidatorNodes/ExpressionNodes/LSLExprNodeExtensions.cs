#region FileInfo
// 
// 
// File: LSLExprNodeExtensions.cs
// 
// Last Compile: 25/09/2015 @ 11:47 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
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
    public static class LSLExprNodeExtensions
    {
        public static bool IsLiteral(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.Literal;
        }

        public static bool IsCompoundExpression(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.BinaryExpression ||
                   node.ExpressionType == LSLExpressionType.ParenthesizedExpression ||
                   node.ExpressionType == LSLExpressionType.PostfixExpression ||
                   node.ExpressionType == LSLExpressionType.PrefixExpression ||
                   node.ExpressionType == LSLExpressionType.TypecastExpression;
        }

        public static bool IsFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.LibraryFunction ||
                   node.ExpressionType == LSLExpressionType.UserFunction;
        }

        public static bool IsLibraryFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.LibraryFunction;
        }

        public static bool IsUserFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.UserFunction;
        }

        public static bool IsVariable(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.GlobalVariable ||
                   node.ExpressionType == LSLExpressionType.LocalVariable;
        }

        public static bool IsLocalVariable(this ILSLReadOnlyExprNode node)
        {
            return
                node.ExpressionType == LSLExpressionType.LocalVariable;
        }

        public static bool IsGlobalVariable(this ILSLReadOnlyExprNode node)
        {
            return
                node.ExpressionType == LSLExpressionType.GlobalVariable;
        }
    }
}