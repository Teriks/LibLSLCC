#region FileInfo

// 
// File: LSLConstantJumpDescription.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:24 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
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