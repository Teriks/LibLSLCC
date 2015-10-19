#region FileInfo
// 
// File: ILSLJumpStatementNode.cs
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

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    /// <summary>
    /// AST node interface for jump statements.
    /// </summary>
    public interface ILSLJumpStatementNode : ILSLReadOnlyCodeStatement
    {
        /// <summary>
        /// The name of the label that the jump statement jumps to.
        /// </summary>
        string LabelName { get; }

        /// <summary>
        /// The label statement node in the syntax tree that this jump statement jumps to.
        /// </summary>
        ILSLLabelStatementNode JumpTarget { get; }

        /// <summary>
        /// True if this jump is guaranteed to occur in a constant manner.  
        /// IE, the jump is always encountered regardless of program control flow.
        /// </summary>
        bool ConstantJump { get; }

        /// <summary>
        /// The source code range of the 'jump' keyword in the jump statement.
        /// </summary>
        LSLSourceCodeRange JumpKeywordSourceCodeRange { get; }

        /// <summary>
        /// The source code range of the target label name in the jump statement.
        /// </summary>
        LSLSourceCodeRange LabelNameSourceCodeRange { get; }

        /// <summary>
        /// The source code range of the semi-colon that follows the jump statement.
        /// </summary>
        LSLSourceCodeRange SemiColonSourceCodeRange { get; }
    }
}