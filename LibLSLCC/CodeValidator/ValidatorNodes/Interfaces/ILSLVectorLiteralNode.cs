using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLVectorLiteralNode : ILSLReadOnlyExprNode
    {
        ILSLReadOnlyExprNode XExpression { get; }

        LSLSourceCodeRange CommaOneSourceCodeRange { get; }
        ILSLReadOnlyExprNode YExpression { get; }

        LSLSourceCodeRange CommaTwoSourceCodeRange { get; }
        ILSLReadOnlyExprNode ZExpression { get; }
    }
}