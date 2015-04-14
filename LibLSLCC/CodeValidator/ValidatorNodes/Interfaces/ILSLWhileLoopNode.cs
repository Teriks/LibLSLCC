namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLWhileLoopNode : ILSLReadOnlyCodeStatement
    {
        ILSLReadOnlyExprNode ConditionExpression { get; }
        ILSLCodeScopeNode Code { get; }
    }
}