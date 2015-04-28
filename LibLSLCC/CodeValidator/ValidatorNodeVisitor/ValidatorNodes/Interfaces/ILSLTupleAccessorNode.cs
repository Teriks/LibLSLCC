using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLTupleAccessorNode : ILSLReadOnlyExprNode
    {
        string AccessedComponentString { get; }
        LSLTupleComponent AccessedComponent { get; }
        LSLType AccessedType { get; }
        ILSLReadOnlyExprNode AccessedExpression { get; }
    }
}