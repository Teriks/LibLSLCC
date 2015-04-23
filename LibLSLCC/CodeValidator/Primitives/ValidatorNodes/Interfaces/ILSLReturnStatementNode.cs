namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLReturnStatementNode : ILSLReadOnlyCodeStatement
    {
        ILSLReadOnlyExprNode ReturnExpression { get; }
        bool HasReturnExpression { get; }
    }
}