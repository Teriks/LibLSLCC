#region FileInfo
// 
// File: ILSLExpressionListNode.cs
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

using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator.Nodes.Interfaces
{
    /// <summary>
    /// AST node interface for expression list, such as function call parameters, for loop init sections/afterthoughts, list initializers ect.
    /// </summary>
    public interface ILSLExpressionListNode : ILSLSyntaxTreeNode
    {
        /// <summary>
        /// The type of expression list this node represents.
        /// <see cref="LSLExpressionListType"/>
        /// </summary>
        LSLExpressionListType ListType { get; }


        /// <summary>
        /// A list of expression nodes that belong to this expression list in order of appearance, or an empty list object.
        /// </summary>
        IReadOnlyGenericArray<ILSLReadOnlyExprNode> ExpressionNodes { get; }

        /// <summary>
        /// The source code range for each comma separator that appears in the expression list in order, or an empty list object.
        /// </summary>
        IReadOnlyGenericArray<LSLSourceCodeRange> CommaSourceCodeRanges { get; }

        /// <summary>
        /// True if all expressions in the expression list are considered to be constant expressions.
        /// </summary>
        bool AllExpressionsConstant { get; }

        /// <summary>
        /// True if this expression list node actually has expression children, False if it is empty.
        /// </summary>
        bool HasExpressionNodes { get; }
    }
}