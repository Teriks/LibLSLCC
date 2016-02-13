#region FileInfo
// 
// File: ILSLCodeStatement.cs
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

#endregion

namespace LibLSLCC.CodeValidator.Nodes.Interfaces
{
    /// <summary>
    /// Read only AST node interface for code statements, code statements are the line by line statements that appear inside code scopes.
    /// </summary>
    public interface ILSLReadOnlyCodeStatement : ILSLReadOnlySyntaxTreeNode, ILSLReturnPathNode
    {
        /// <summary>
        /// The index of this statement in its scope
        /// </summary>
        int StatementIndex { get; }

        /// <summary>
        /// Is this statement the last statement in its scope
        /// </summary>
        bool IsLastStatementInScope { get; }

        /// <summary>
        /// The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        LSLDeadCodeType DeadCodeType { get; }

        /// <summary>
        /// Is this statement dead code
        /// </summary>
        bool IsDeadCode { get; }

        /// <summary>
        /// If the scope has a return path, this is set to the node that causes the function to return.
        /// it may be a return statement, or a control chain node.
        /// </summary>
        ILSLReadOnlyCodeStatement ReturnPath { get; }

        /// <summary>
        /// Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        /// this is not the scopes level.
        /// </summary>
        int ScopeId { get; }


        /// <summary>
        /// True if this statement belongs to a single statement code scope.
        /// A single statement code scope is a brace-less code scope that can be used in control or loop statements.
        /// </summary>
        bool IsSingleBlockStatement { get; }
    }


    /// <summary>
    /// Interface for code statements, code statements are the line by line statements that appear inside code scopes.
    /// </summary>
    public interface ILSLCodeStatement : ILSLReadOnlyCodeStatement, ILSLSyntaxTreeNode
    {
        /// <summary>
        /// The index of this statement in its scope
        /// </summary>
        new int StatementIndex { get; set; }

        /// <summary>
        /// Is this statement the last statement in its scope
        /// </summary>
        new bool IsLastStatementInScope { get; set; }

        /// <summary>
        /// The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        new LSLDeadCodeType DeadCodeType { get; set; }

        /// <summary>
        /// Is this statement dead code
        /// </summary>
        new bool IsDeadCode { get; set; }

        /// <summary>
        /// If the scope has a return path, this is set to the node that causes the function to return.
        /// it may be a return statement, or a control chain node.
        /// </summary>
        new ILSLCodeStatement ReturnPath { get; set; }

        /// <summary>
        /// Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        /// this is not the scopes level.
        /// </summary>
        new int ScopeId { get; set; }
    }
}