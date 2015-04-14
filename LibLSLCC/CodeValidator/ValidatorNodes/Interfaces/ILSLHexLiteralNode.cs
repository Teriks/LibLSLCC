namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLHexLiteralNode : ILSLReadOnlyExprNode
    {
        string RawText { get; }
    }
}