#region FileInfo
// 
// File: LSLExpressionType.cs
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
using LibLSLCC.CodeValidator.Nodes.Interfaces;

namespace LibLSLCC.CodeValidator.Enums
{
    /// <summary>
    ///     Represents the expression type of an <see cref="ILSLReadOnlyExprNode"/> object
    /// </summary>
    public enum LSLExpressionType
    {
        /// <summary>
        /// A literal node, such as a string literal, vector literal, rotation literal or list literal
        /// </summary>
        Literal,

        /// <summary>
        /// A node that is a call to a library function.
        /// </summary>
        LibraryFunction,

        /// <summary>
        /// A node that is a reference to a library constant.
        /// </summary>
        LibraryConstant,

        /// <summary>
        /// A node that is a reference to a user defined function.
        /// </summary>
        UserFunction,

        /// <summary>
        /// A node that is a reference to a local variable.
        /// </summary>
        LocalVariable,

        /// <summary>
        /// A node that is a reference to a function or event handler parameter.
        /// </summary>
        ParameterVariable,

        /// <summary>
        /// A node that is a reference to a global variable.
        /// </summary>
        GlobalVariable,


        /// <summary>
        /// A node that represents a vector component access expression on a vector variable.
        /// </summary>
        VectorOrRotationComponentAccess,

        /// <summary>
        /// A node that represents a rotation component access expression on a rotation variable.
        /// </summary>
        RotationComponentAccess,

        /// <summary>
        /// A node that represents a binary expression.
        /// </summary>
        BinaryExpression,

        /// <summary>
        /// A node that represents a postfix expression such as a postfix increment or decrement.
        /// </summary>
        PostfixExpression,

        /// <summary>
        /// A node that represents a prefix expression such as a prefix increment or decrement.
        /// </summary>
        PrefixExpression,

        /// <summary>
        /// A node that represents a typecast expression.
        /// </summary>
        TypecastExpression,

        /// <summary>
        /// A node that represents a parenthesized expression containing sub expressions.
        /// </summary>
        ParenthesizedExpression
    }
}