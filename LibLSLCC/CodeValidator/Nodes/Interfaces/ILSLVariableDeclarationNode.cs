#region FileInfo
// 
// File: ILSLVariableDeclarationNode.cs
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
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator.Nodes.Interfaces
{

    /// <summary>
    /// AST node interface for global and local variable declarations.
    /// </summary>
    public interface ILSLVariableDeclarationNode : ILSLReadOnlyCodeStatement
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        string Name { get; }


        /// <summary>
        /// The variable type.
        /// </summary>
        LSLType Type { get; }


        /// <summary>
        /// The raw type string representing the variable type, taken from the source code.
        /// </summary>
        string TypeString { get; }


        /// <summary>
        /// The initial variable node that was created by this variable declaration.
        /// </summary>
        ILSLVariableNode VariableNode { get; }


        /// <summary>
        /// A list of variable nodes representing references to this variable declaration in the source code.  Or an empty list.
        /// ILSLVariableNodes are used to represent a reference to a declared variable, and are present in the syntax tree at the site of reference.
        /// </summary>
        IReadOnlyGenericArray<ILSLVariableNode> References { get; }


        /// <summary>
        /// True if an expression was used to initialize this variable declaration node when it was defined.
        /// </summary>
        bool HasDeclarationExpression { get; }


        /// <summary>
        /// True if this variable declaration is local to a function or event handler.  False if it is a global variable, parameter definition, or library constant.
        /// </summary>
        bool IsLocal { get; }

        /// <summary>
        /// True if this variable declaration is in the global program scope.  False if it is a local variable, parameter definition, or library constant.
        /// </summary>
        bool IsGlobal { get; }


        /// <summary>
        /// True if this variable declaration represents a local function/event handler parameter.  False if it is a local variable, global variable, or library constant.
        /// </summary>
        bool IsParameter { get; }



        /// <summary>
        /// True if this variable declaration represents a library defined constant.  False if it is a local variable, global variable, or parameter definition.
        /// </summary>
        bool IsLibraryConstant { get; }


        /// <summary>
        /// The expression used to initialize this variable declaration, this will be null if 'HasDeclarationExpression' is false.
        /// If neither 'IsLocal' or 'IsGlobal' are true, than this property will always be null.
        /// </summary>
        ILSLReadOnlyExprNode DeclarationExpression { get; }

        /// <summary>
        /// The source code range of the type specifier for the variable declaration.
        /// </summary>
        LSLSourceCodeRange TypeSourceCodeRange { get; }

        /// <summary>
        /// The source code range that encompasses the variables name in the declaration.
        /// </summary>
        LSLSourceCodeRange NameSourceCodeRange { get; }


        /// <summary>
        /// The source code range of the assignment operator in the declaration expression if one was used.
        /// This value is only meaningful if either 'IsLocal' or 'IsGlobal' are true, and 'HasDeclarationExpression' is also true.
        /// </summary>
        LSLSourceCodeRange OperatorSourceCodeRange { get; }
    }
}