using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLBinaryExpressionNode : ILSLReadOnlyExprNode
    {
        LSLBinaryOperationType Operation { get; }
        string OperationString { get; }
        ILSLReadOnlyExprNode LeftExpression { get; }
        ILSLReadOnlyExprNode RightExpression { get; }

        LSLSourceCodeRange OperationSourceCodeRange { get; }
    }
}