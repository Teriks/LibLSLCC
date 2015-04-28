using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLWhileLoopNode : ILSLReadOnlyCodeStatement, ILSLLoopNode
    {
        ILSLReadOnlyExprNode ConditionExpression { get; }
        ILSLCodeScopeNode Code { get; }

        LSLSourceCodeRange WhileKeywordSourceCodeRange { get; }

        LSLSourceCodeRange OpenParenthSourceCodeRange { get; }

        LSLSourceCodeRange CloseParenthSourceCodeRange { get; }
    }
}