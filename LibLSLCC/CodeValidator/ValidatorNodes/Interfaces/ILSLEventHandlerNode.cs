#region FileInfo
// 
// File: ILSLEventHandlerNode.cs
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

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    /// <summary>
    /// AST node interface for library event handler references.
    /// </summary>
    public interface ILSLEventHandlerNode : ILSLReadOnlySyntaxTreeNode
    {
        /// <summary>
        /// The name of the event handler.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// True if the event handler has parameters.
        /// </summary>
        bool HasParameterNodes { get; }

        /// <summary>
        /// An in order list of parameter nodes that belong to the event handler, or an empty enumerable if none exist.
        /// </summary>
        IReadOnlyGenericArray<ILSLParameterNode> ParameterNodes { get; }

        /// <summary>
        /// The code scope node that represents the code body of the event handler.
        /// </summary>
        ILSLCodeScopeNode EventBodyNode { get; }

        /// <summary>
        /// The parameter list node for the parameters of the event handler.  This is not null even when no parameters exist.
        /// It can be null if there are errors in the event handler node that prevent the parameters from being parsed.
        /// Ideally you should not be handling a syntax tree with syntax errors in it.
        /// </summary>
        ILSLParameterListNode ParameterListNode { get; }

        /// <summary>
        /// Get an <see cref="LSLEventSignature "/> representation of the event handlers signature.
        /// This could be null or throw an exception if the event handler node contains syntax errors.
        /// Ideally you should not be handling a syntax tree with syntax errors in it.
        /// </summary>
        /// <returns>An <see cref="LSLEventSignature "/> representing the signature of the event handler node.</returns>
        LSLEventSignature ToSignature();
    }
}