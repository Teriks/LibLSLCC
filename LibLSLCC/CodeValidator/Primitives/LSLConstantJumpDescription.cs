#region FileInfo
// 
// File: LSLConstantJumpDescription.cs
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

using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    /// <summary>
    ///     Represents a statement that causes a jump to happen in a constant manner.
    ///     (As in, there is no condition under which a jump to a single known label will not occur)
    /// </summary>
    public class LSLConstantJumpDescription
    {
        public LSLConstantJumpDescription(LSLConstantJumpDescription origionalJump,
            ILSLReadOnlyCodeStatement effectiveJumpStatement)
        {
            DeterminingJump = origionalJump.DeterminingJump;
            EffectiveJumpStatement = effectiveJumpStatement;
        }

        public LSLConstantJumpDescription(ILSLJumpStatementNode determiningJump)
        {
            DeterminingJump = determiningJump;
            EffectiveJumpStatement = determiningJump;
        }

        public LSLConstantJumpDescription(ILSLReadOnlyCodeStatement effectiveJumpStatement,
            ILSLJumpStatementNode determiningJump)
        {
            DeterminingJump = determiningJump;
            EffectiveJumpStatement = effectiveJumpStatement;
        }

        /// <summary>
        ///     EffectiveJumpStatement should represent the statement that
        ///     the jump will always occur from, it may be an LSLControlStatementNode
        ///     where all branches jump to a single label in a constant manner, or simply an LSLJumpStatementNode
        ///     on its own.
        /// </summary>
        public ILSLReadOnlyCodeStatement EffectiveJumpStatement { get; private set; }

        /// <summary>
        ///     The actual LSLJumpStatementNode that caused or is part of the cause for the jump to happen
        ///     if EffectiveJumpStatement is an LSLControlStatementNode, it will be the jump from the 'if' statement
        ///     part of that node, otherwise EffectiveJumpStatement and DeterminingJump will be references to the same
        ///     object.
        /// </summary>
        public ILSLJumpStatementNode DeterminingJump { get; }

        /// <summary>
        ///     The label node that DeterminingJump jumps to,
        ///     it is equal to DeterminingJump.JumpTarget
        /// </summary>
        public ILSLLabelStatementNode JumpTarget
        {
            get { return DeterminingJump.JumpTarget; }
        }
    }
}