namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLIntegerLiteralNode : ILSLReadOnlyExprNode
    {
        string RawText { get; }
    }
}