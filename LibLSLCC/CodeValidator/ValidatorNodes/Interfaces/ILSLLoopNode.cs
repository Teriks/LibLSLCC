namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLLoopNode
    {
        ILSLReadOnlyExprNode ConditionExpression { get; }

        ILSLCodeScopeNode Code { get; }
    }
}
