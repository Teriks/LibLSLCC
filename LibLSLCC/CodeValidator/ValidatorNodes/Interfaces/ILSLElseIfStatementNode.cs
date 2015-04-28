using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLElseIfStatementNode : ILSLReturnPathNode, ILSLReadOnlyBranchStatementNode
    {
        ILSLCodeScopeNode Code { get; }
        ILSLReadOnlyExprNode ConditionExpression { get; }

        LSLSourceCodeRange ElseKeywordSourceCodeRange { get; }

        LSLSourceCodeRange IfKeywordSourceCodeRange { get; }

        LSLSourceCodeRange OpenParenthSourceCodeRange { get; }

        LSLSourceCodeRange CloseParenthSourceCodeRange { get; }
    }
}