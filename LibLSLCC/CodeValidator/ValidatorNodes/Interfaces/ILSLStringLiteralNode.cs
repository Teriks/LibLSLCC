namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLStringLiteralNode : ILSLReadOnlyExprNode
    {
        string PreProccessedText { get; }
        string RawText { get; }
    }
}