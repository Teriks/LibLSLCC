using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLWhileLoopNode : ILSLReadOnlyCodeStatement, ILSLLoopNode
    {


        LSLSourceCodeRange WhileKeywordSourceCodeRange { get; }

        LSLSourceCodeRange OpenParenthSourceCodeRange { get; }

        LSLSourceCodeRange CloseParenthSourceCodeRange { get; }
    }
}