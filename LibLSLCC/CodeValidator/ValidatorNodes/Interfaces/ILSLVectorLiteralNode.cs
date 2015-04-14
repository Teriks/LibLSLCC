namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLVectorLiteralNode : ILSLReadOnlyExprNode
    {
        ILSLReadOnlyExprNode XExpression { get; }
        ILSLReadOnlyExprNode YExpression { get; }
        ILSLReadOnlyExprNode ZExpression { get; }
    }
}