using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLDoLoopNode : ILSLReadOnlyCodeStatement, ILSLLoopNode
    {

        LSLSourceCodeRange OpenParenthSourceCodeRange { get; }

        LSLSourceCodeRange CloseParenthSourceCodeRange { get; }

        LSLSourceCodeRange DoKeywordSourceCodeRange { get; }

        LSLSourceCodeRange WhileKeywordSourceCodeRange { get; }

        LSLSourceCodeRange SemiColonSourceCodeRange { get; }
    }
}