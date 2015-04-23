using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLPostfixOperationNode : ILSLReadOnlyExprNode
    {
        LSLPostfixOperationType Operation { get; }
        string OperationString { get; }
        ILSLReadOnlyExprNode LeftExpression { get; }


        LSLSourceCodeRange OperationSourceCodeRange { get; }
    }
}