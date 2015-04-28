using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLDoLoopNode : ILSLReadOnlyCodeStatement, ILSLLoopNode
    {
        ILSLCodeScopeNode Code { get; }
        ILSLReadOnlyExprNode ConditionExpression { get; }

        LSLSourceCodeRange OpenParenthSourceCodeRange { get; }

        LSLSourceCodeRange CloseParenthSourceCodeRange { get; }

        LSLSourceCodeRange DoKeywordSourceCodeRange { get; }

        LSLSourceCodeRange WhileKeywordSourceCodeRange { get; }

        LSLSourceCodeRange SemiColonSourceCodeRange { get; }
    }
}