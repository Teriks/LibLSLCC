namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLDoLoopNode : ILSLReadOnlyCodeStatement, ILSLLoopNode
    {
        ILSLCodeScopeNode Code { get; }
        ILSLReadOnlyExprNode ConditionExpression { get; }
    }
}