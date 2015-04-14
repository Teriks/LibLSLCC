namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLElseStatementNode : ILSLReturnPathNode, ILSLReadOnlyBranchStatementNode
    {
        ILSLCodeScopeNode Code { get; }
    }
}