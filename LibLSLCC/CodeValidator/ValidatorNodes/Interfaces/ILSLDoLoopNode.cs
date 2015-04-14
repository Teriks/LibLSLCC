namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLDoLoopNode : ILSLReadOnlyCodeStatement
    {
        ILSLCodeScopeNode Code { get; }
        ILSLExprNode ConditionExpression { get; }
    }
}