#region FileInfo
// 
// 
// File: ILSLCodeScopeNode.cs
// 
// Last Compile: 25/09/2015 @ 5:46 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// ============================================================
// ============================================================
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// Copyright (c) 2015, Teriks
// All rights reserved.
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

using System.Collections.Generic;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLCodeScopeNode : ILSLReadOnlyCodeStatement
    {
        /// <summary>
        ///     Constant jump descriptors for constant jumps that occur in this scope
        ///     used only with JumpStatementAnalysis is turned on and dead code caused by
        ///     jump statements is being detected
        /// </summary>
        IEnumerable<LSLConstantJumpDescription> ConstantJumps { get; }

        /// <summary>
        ///     Is this scope a single statement scope, like a brace-less 'if' branch.
        ///     true if IsCodeScope is false
        /// </summary>
        bool IsSingleStatement { get; }

        /// <summary>
        ///     Is this a normal braced code scope.
        ///     true if IsSingleStatement is false
        /// </summary>
        bool IsCodeScope { get; }

        /// <summary>
        ///     Code statements that are children of this code scope
        /// </summary>
        IEnumerable<ILSLReadOnlyCodeStatement> CodeStatements { get; }

        /// <summary>
        ///     ReturnStatementNode != null
        /// </summary>
        bool HasReturnStatementNode { get; }

        /// <summary>
        ///     FirstDeadStatementNode != null
        /// </summary>
        bool HasDeadStatementNodes { get; }

        /// <summary>
        ///     The first statement node to be considered dead, when dead code is detected
        /// </summary>
        ILSLReadOnlyCodeStatement FirstDeadStatementNode { get; }

        /// <summary>
        ///     Returns descriptions of all dead code segments in the top level of this scope,
        ///     if there are any
        /// </summary>
        IEnumerable<LSLDeadCodeSegment> DeadCodeSegments { get; }

        /// <summary>
        ///     The top level return statement for a code scope, if one exists
        /// </summary>
        LSLReturnStatementNode ReturnStatementNode { get; }

        LSLCodeScopeType CodeScopeType { get; }
    }
}