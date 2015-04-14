namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLForLoopNode : ILSLReadOnlyCodeStatement
    {
        ILSLReadOnlyExprNode InitExpression { get; }
        ILSLReadOnlyExprNode ConditionExpression { get; }
        ILSLExpressionListNode AfterthoughExpressions { get; }
        ILSLCodeScopeNode Code { get; }
        bool HasInitExpression { get; }
        bool HasConditionExpression { get; }
        bool HasAfterthoughtExpressions { get; }
    }
}