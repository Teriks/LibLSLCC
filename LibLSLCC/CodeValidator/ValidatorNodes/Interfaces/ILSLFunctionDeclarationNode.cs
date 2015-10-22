#region FileInfo
// 
// File: ILSLFunctionDeclarationNode.cs
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
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    /// <summary>
    /// AST node interface for function declaration nodes.
    /// </summary>
    public interface ILSLFunctionDeclarationNode : ILSLReadOnlySyntaxTreeNode
    {
        /// <summary>
        /// True if the function definition node possesses parameter definitions.
        /// </summary>
        bool HasParameters { get; }


        /// <summary>
        /// A list of function call nodes that reference this function definition, or an empty list.
        /// </summary>
        IReadOnlyGenericArray<ILSLFunctionCallNode> References { get; }

        /// <summary>
        /// A list of  parameter definition nodes that belong to this function definition, or an empty list.
        /// </summary>
        IReadOnlyGenericArray<ILSLParameterNode> ParameterNodes { get; }

        /// <summary>
        /// The string from the source code that represents the return type assigned to the function definition,
        /// or an empty string if no return type was assigned.
        /// </summary>
        string ReturnTypeString { get; }

        /// <summary>
        /// The name of the function.
        /// </summary>
        string Name { get; }


        /// <summary>
        /// The return type assigned to the function definition, it will be LSLType.Void if no return type was given.
        /// </summary>
        LSLType ReturnType { get; }

        /// <summary>
        /// The parameter list node that contains the parameter list definitions for this function.
        /// It should never be null, even if the function definition contains no parameter definitions.
        /// </summary>
        ILSLParameterListNode ParameterListNode { get; }


        /// <summary>
        /// The code scope node that represents the code body of the function definition.
        /// </summary>
        ILSLCodeScopeNode FunctionBodyNode { get; }
    }
}