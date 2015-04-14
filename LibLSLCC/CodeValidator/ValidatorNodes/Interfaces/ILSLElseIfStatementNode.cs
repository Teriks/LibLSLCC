namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLElseIfStatementNode : ILSLReturnPathNode, ILSLReadOnlyBranchStatementNode
    {
        ILSLCodeScopeNode Code { get; }
        ILSLReadOnlyExprNode ConditionExpression { get; }
    }
}