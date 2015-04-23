namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLStateChangeStatementNode : ILSLReadOnlyCodeStatement
    {
        string StateTargetName { get; }
        new ILSLReadOnlySyntaxTreeNode Parent { get; }
        new bool HasErrors { get; }
    }
}