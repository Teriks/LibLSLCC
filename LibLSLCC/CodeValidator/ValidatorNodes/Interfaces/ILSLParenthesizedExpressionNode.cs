namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLParenthesizedExpressionNode : ILSLReadOnlyExprNode
    {
        ILSLReadOnlyExprNode InnerExpression { get; }
    }
}