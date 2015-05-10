using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLRotationLiteralNode : ILSLReadOnlyExprNode
    {
        ILSLReadOnlyExprNode XExpression { get; }

        LSLSourceCodeRange CommaOneSourceCodeRange { get; }

        ILSLReadOnlyExprNode YExpression { get; }

        LSLSourceCodeRange CommaTwoSourceCodeRange { get; }
        ILSLReadOnlyExprNode ZExpression { get; }

        LSLSourceCodeRange CommaThreeSourceCodeRange { get; }


        ILSLReadOnlyExprNode SExpression { get; }
    }
}