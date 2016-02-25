#region FileInfo

// 
// File: LSLPreDefinedFunctionSignature.cs
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

using System.Linq;
using LibLSLCC.CodeValidator.Nodes;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Represents a function signature that was parsed during the pre-pass that occurs during code validation.
    /// </summary>
    public sealed class LSLPreDefinedFunctionSignature : LSLFunctionSignature
    {
        /// <summary>
        ///     Construct an <see cref="LSLPreDefinedFunctionSignature" /> from an <see cref="LSLType" /> representing the return
        ///     type, a function name and an <see cref="LSLParameterListNode" />
        ///     from an LSL Syntax tree.
        /// </summary>
        /// <param name="returnType">The return type of the function signature.</param>
        /// <param name="name">The name of the function.</param>
        /// <param name="parameters">
        ///     The <see cref="LSLParameterListNode" /> from an LSL syntax tree that represents the function
        ///     signatures parameters.
        /// </param>
        public LSLPreDefinedFunctionSignature(LSLType returnType, string name, LSLParameterListNode parameters)
            : base(returnType, name, parameters.Parameters.Select(x => new LSLParameter(x.Type, x.Name, false)))
        {
            ParameterListNode = parameters;
        }


        /// <summary>
        ///     The LSLParameterListNOde from an LSL syntax tree the represents the function signatures parameters.
        /// </summary>
        public LSLParameterListNode ParameterListNode { get; private set; }

        /// <summary>
        ///     The <see cref="LSLFunctionDeclarationNode" /> in the syntax tree that this function signature belongs
        ///     to/represents.
        /// </summary>
        public LSLFunctionDeclarationNode DefinitionNode { get; private set; }


        /// <summary>
        ///     Internal method that sets the DefinitionNode property, this method is named this way to bring clarity to the source
        ///     code where it is used.
        /// </summary>
        /// <param name="definition"></param>
        internal void GiveDefinition(LSLFunctionDeclarationNode definition)
        {
            DefinitionNode = definition;
        }
    }
}