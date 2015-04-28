using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLIfStatementNode : ILSLReturnPathNode, ILSLBranchStatementNode
    {
        ILSLCodeScopeNode Code { get; }
        ILSLReadOnlyExprNode ConditionExpression { get; }

        LSLSourceCodeRange IfKeywordSourceCodeRange { get; }

        LSLSourceCodeRange OpenParenthSourceCodeRange { get; }

        LSLSourceCodeRange CloseParenthSourceCodeRange { get; }
    }
}