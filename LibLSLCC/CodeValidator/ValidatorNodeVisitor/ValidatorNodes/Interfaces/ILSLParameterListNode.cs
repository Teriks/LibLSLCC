using System.Collections.Generic;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLParameterListNode : ILSLSyntaxTreeNode
    {
        bool HasParameterNodes { get; }
        IReadOnlyList<ILSLParameterNode> Parameters { get; }
    }
}