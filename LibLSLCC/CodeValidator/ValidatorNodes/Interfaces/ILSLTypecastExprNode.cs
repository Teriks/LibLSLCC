using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLTypecastExprNode : ILSLReadOnlyExprNode
    {
        ILSLReadOnlyExprNode CastedExpression { get; }
        LSLType CastToType { get; }
        string CastToTypeString { get; }
    }
}