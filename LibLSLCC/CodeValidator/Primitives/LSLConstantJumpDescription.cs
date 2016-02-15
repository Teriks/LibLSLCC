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

using LibLSLCC.CodeValidator.Nodes;
using LibLSLCC.CodeValidator.Nodes.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    /// <summary>
    /// Represents a statement that causes a jump to happen in a constant manner.
    /// As in:  There is no condition under which a jump to a single known label will not occur.
    /// </summary>
    public sealed class LSLConstantJumpDescription
    {

        /// <summary>
        /// Construct an <see cref="LSLConstantJumpDescription"/> from another <see cref="LSLConstantJumpDescription"/> and an <see cref="ILSLReadOnlyCodeStatement"/> that
        /// represents the actual jump statement in the syntax tree.
        /// 
        /// DeterminingJump in the constructed object is set to originalJump.DeterminingJump.
        /// EffectiveJumpStatement in the constructed object is set to the effectedJumpStatement parameter.
        /// </summary>
        /// <param name="originalJump">The LSLConstantJumpDescripton to copy the DeterminingJump property from.</param>
        /// <param name="effectiveJumpStatement">The <see cref="ILSLReadOnlyCodeStatement"/> that represents the jump statement in the syntax tree.</param>
        public LSLConstantJumpDescription(LSLConstantJumpDescription originalJump,
            ILSLReadOnlyCodeStatement effectiveJumpStatement)
        {
            DeterminingJump = originalJump.DeterminingJump;
            EffectiveJumpStatement = effectiveJumpStatement;
        }

        /// <summary>
        /// Constructs an <see cref="LSLConstantJumpDescription"/> from an ILSLJumpStatement node by 
        /// setting the DeterminingJump and EffectiveJumpStatement property in this object to the determiningJump parameter.
        /// </summary>
        /// <param name="determiningJump">The <see cref="ILSLJumpStatementNode"/> to set DeterminingJump and EffectiveJumpStatement to.</param>
        public LSLConstantJumpDescription(ILSLJumpStatementNode determiningJump)
        {
            DeterminingJump = determiningJump;
            EffectiveJumpStatement = determiningJump;
        }


        /// <summary>
        /// Constructs an <see cref="LSLConstantJumpDescription"/> from an ILSLJumpStatement node by 
        /// setting the DeterminingJump property of this object to the effectiveJumpStatement parameter, and
        /// the EffectiveJumpStatement of this object to the effectiveJumpStatement parameter.
        /// </summary>
        /// <param name="effectiveJumpStatement">The <see cref="ILSLReadOnlyCodeStatement"/> to set EffectiveJumpStatement to.</param>
        /// <param name="determiningJump">The <see cref="ILSLJumpStatementNode"/> to set DeterminingJump to.</param>
        public LSLConstantJumpDescription(ILSLReadOnlyCodeStatement effectiveJumpStatement,
            ILSLJumpStatementNode determiningJump)
        {
            DeterminingJump = determiningJump;
            EffectiveJumpStatement = effectiveJumpStatement;
        }

        /// <summary>
        ///     EffectiveJumpStatement should represent the statement that
        ///     the jump will always occur from, it may be an <see cref="LSLControlStatementNode"/>
        ///     where all branches jump to a single label in a constant manner, or simply an <see cref="LSLJumpStatementNode"/>
        ///     on its own.
        /// </summary>
        public ILSLReadOnlyCodeStatement EffectiveJumpStatement { get; private set; }

        /// <summary>
        ///     The actual <see cref="LSLJumpStatementNode"/> that caused or is part of the cause for the jump to happen
        ///     if EffectiveJumpStatement is an <see cref="LSLControlStatementNode"/>, it will be the jump from the 'if' statement
        ///     part of that node, otherwise EffectiveJumpStatement and DeterminingJump will be references to the same
        ///     object.
        /// </summary>
        public ILSLJumpStatementNode DeterminingJump { get; private set; }

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