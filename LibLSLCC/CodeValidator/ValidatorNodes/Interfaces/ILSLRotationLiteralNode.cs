namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLRotationLiteralNode : ILSLReadOnlyExprNode
    {
        ILSLReadOnlyExprNode XExpression { get; }
        ILSLReadOnlyExprNode YExpression { get; }
        ILSLReadOnlyExprNode ZExpression { get; }
        ILSLReadOnlyExprNode SExpression { get; }
    }
}