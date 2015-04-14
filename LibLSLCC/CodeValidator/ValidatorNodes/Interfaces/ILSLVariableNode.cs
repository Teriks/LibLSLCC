namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLVariableNode : ILSLReadOnlyExprNode
    {
        bool IsLibraryConstant { get; }
        bool IsGlobal { get; }
        bool IsLocal { get; }
        ILSLVariableDeclarationNode Declaration { get; }
        bool IsParameter { get; }
        string TypeString { get; }
        string Name { get; }
    }
}