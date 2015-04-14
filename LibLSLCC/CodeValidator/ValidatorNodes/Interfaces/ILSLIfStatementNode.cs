namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLIfStatementNode : ILSLReturnPathNode, ILSLBranchStatementNode
    {
        ILSLCodeScopeNode Code { get; }
        ILSLReadOnlyExprNode ConditionExpression { get; }
    }
}