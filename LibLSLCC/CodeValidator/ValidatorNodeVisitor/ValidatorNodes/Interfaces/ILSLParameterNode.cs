using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLParameterNode : ILSLReadOnlySyntaxTreeNode
    {
        string Name { get; }
        LSLType Type { get; }
        string TypeString { get; }
        int ParameterIndex { get; }
    }
}