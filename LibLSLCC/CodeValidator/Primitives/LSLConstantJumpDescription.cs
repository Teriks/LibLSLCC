using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

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