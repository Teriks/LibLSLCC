using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLPostfixOperationNode : ILSLReadOnlyExprNode
    {
        LSLPostfixOperationType Operation { get; }
        string OperationString { get; }
        ILSLReadOnlyExprNode LeftExpression { get; }
    }
}