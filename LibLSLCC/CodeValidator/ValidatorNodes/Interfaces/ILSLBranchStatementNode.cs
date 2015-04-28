namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLBranchStatementNode : ILSLSyntaxTreeNode
    {
        bool IsConstantBranch { get; }
    }

    public interface ILSLReadOnlyBranchStatementNode : ILSLReadOnlySyntaxTreeNode
    {
        bool IsConstantBranch { get; }
    }
}