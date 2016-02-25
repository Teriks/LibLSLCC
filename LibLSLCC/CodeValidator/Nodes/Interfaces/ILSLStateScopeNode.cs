#region FileInfo

// 
// File: ILSLStateScopeNode.cs
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
    ///     AST node interface for code state blocks.
    /// </summary>
    public interface ILSLStateScopeNode : ILSLReadOnlySyntaxTreeNode
    {
        /// <summary>
        ///     The name of the state block in the source code.
        ///     'default' should be returned if the node represents the default state.
        /// </summary>
        string StateName { get; }

        /// <summary>
        ///     True if this state scope node represents the 'default' state,  False if it is a user defined state.
        /// </summary>
        bool IsDefaultState { get; }

        /// <summary>
        ///     A list of event handlers nodes for each event handler that was used in the state.
        ///     This should never be empty.
        /// </summary>
        IReadOnlyGenericArray<ILSLEventHandlerNode> EventHandlers { get; }

        /// <summary>
        ///     The source code range of the opening brace of the state block's scope.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        LSLSourceCodeRange SourceRangeOpenBrace { get; }

        /// <summary>
        ///     The source code range of the closing brace of the state block's scope.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        LSLSourceCodeRange SourceRangeCloseBrace { get; }

        /// <summary>
        ///     The source code range where the name of the state is located.
        ///     For the default state, this will be the location of the 'default' keyword.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        LSLSourceCodeRange SourceRangeStateName { get; }

        /// <summary>
        ///     The source code range where the state keyword is located.
        ///     For the default state, this will be the location of the 'default' keyword.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        LSLSourceCodeRange SourceRangeStateKeyword { get; }
    }
}