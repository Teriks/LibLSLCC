#region FileInfo
// 
// File: ILSLVariableNode.cs
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
namespace LibLSLCC.CodeValidator.Nodes.Interfaces
{

    /// <summary>
    /// AST node interface for variable references.  
    /// An <see cref="ILSLVariableNode"/> is also created as a child of an <see cref="ILSLVariableDeclarationNode"/>.
    /// </summary>
    public interface ILSLVariableNode : ILSLReadOnlyExprNode
    {
        /// <summary>
        /// True if this variable node references a library constant, False if it references a user defined variable or parameter.
        /// </summary>
        bool IsLibraryConstant { get; }

        /// <summary>
        /// True if this variable node references a user defined global variable.
        /// </summary>
        bool IsGlobal { get; }

        /// <summary>
        /// True if this variable node references a user defined local variable.
        /// </summary>
        bool IsLocal { get; }


        /// <summary>
        /// A reference to the <see cref="ILSLVariableDeclarationNode"/> in the syntax tree where this variable was initially declared.
        /// </summary>
        ILSLVariableDeclarationNode Declaration { get; }


        /// <summary>
        /// True if this variable node references a function or event handler parameter.
        /// </summary>
        bool IsParameter { get; }

        /// <summary>
        /// The raw type string describing the type of the variable referenced.
        /// </summary>
        string TypeString { get; }

        /// <summary>
        /// The name of the referenced variable.
        /// </summary>
        string Name { get; }
    }
}