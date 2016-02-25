#region FileInfo

// 
// File: ILSLTypecastExprNode.cs
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
using LibLSLCC.CodeValidator.Primitives;

#endregion

namespace LibLSLCC.CodeValidator.Nodes.Interfaces
{
    /// <summary>
    ///     AST node interface for typecast expressions.
    /// </summary>
    public interface ILSLTypecastExprNode : ILSLReadOnlyExprNode
    {
        /// <summary>
        ///     The expression node that represents the expression being casted.  This should never be null.
        /// </summary>
        ILSLReadOnlyExprNode CastedExpression { get; }

        /// <summary>
        ///     The <see cref="LSLType" /> that represents the type the expression is being cast to.
        /// </summary>
        LSLType CastToType { get; }

        /// <summary>
        ///     The raw type name of the type the expression is being cast to, taken from the source code.
        /// </summary>
        string CastToTypeName { get; }

        /// <summary>
        ///     The source code range of the closing parenthesis of the enclosed cast type.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        LSLSourceCodeRange SourceRangeCloseParenth { get; }

        /// <summary>
        ///     The source code range of the open parenthesis of the enclosed cast type.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        LSLSourceCodeRange SourceRangeOpenParenth { get; }

        /// <summary>
        ///     The source code range of the type name used for the cast.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        LSLSourceCodeRange SourceRangeCastToType { get; }
    }
}