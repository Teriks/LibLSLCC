namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLFloatLiteralNode : ILSLReadOnlyExprNode
    {
        string RawText { get; }
    }
}