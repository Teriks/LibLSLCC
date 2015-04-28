using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLEventHandlerNode : ILSLReadOnlySyntaxTreeNode
    {
        string Name { get; }
        bool HasParameterNodes { get; }
        IReadOnlyList<ILSLParameterNode> ParameterNodes { get; }
        ILSLCodeScopeNode EventBodyNode { get; }
        ILSLParameterListNode ParameterListNode { get; }

        LSLEventSignature ToSignature();
    }
}