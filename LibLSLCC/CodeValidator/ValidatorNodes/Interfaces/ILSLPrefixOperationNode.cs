using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLPrefixOperationNode : ILSLReadOnlyExprNode
    {
        LSLPrefixOperationType Operation { get; }
        string OperationString { get; }
        ILSLReadOnlyExprNode RightExpression { get; }

        LSLSourceCodeRange OperationSourceCodeRange { get; }
    }
}