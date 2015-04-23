namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLWhileLoopNode : ILSLReadOnlyCodeStatement, ILSLLoopNode
    {
        ILSLReadOnlyExprNode ConditionExpression { get; }
        ILSLCodeScopeNode Code { get; }
    }
}