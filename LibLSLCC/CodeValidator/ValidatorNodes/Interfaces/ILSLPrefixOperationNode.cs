using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLPrefixOperationNode : ILSLReadOnlyExprNode
    {
        LSLPrefixOperationType Operation { get; }
        string OperationString { get; }
        ILSLReadOnlyExprNode RightExpression { get; }
    }
}