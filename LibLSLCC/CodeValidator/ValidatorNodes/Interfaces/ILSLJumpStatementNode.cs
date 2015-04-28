using LibLSLCC.CodeValidator.Primitives;
namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLJumpStatementNode : ILSLReadOnlyCodeStatement
    {
        string LabelName { get; }
        ILSLLabelStatementNode JumpTarget { get; }
        bool ConstantJump { get; }

        LSLSourceCodeRange JumpKeywordSourceCodeRange { get; }

        LSLSourceCodeRange LabelNameSourceCodeRange { get; }

        LSLSourceCodeRange SemiColonSourceCodeRange { get; }
    }
}