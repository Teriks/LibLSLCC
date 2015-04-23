using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLStateScopeNode : ILSLReadOnlySyntaxTreeNode
    {
        string StateName { get; }
        bool IsDefinedState { get; }
        bool IsDefaultState { get; }
        IReadOnlyList<ILSLEventHandlerNode> EventHandlers { get; }

        LSLSourceCodeRange OpenBraceSourceCodeRange { get; }

        LSLSourceCodeRange CloseBraceSourceCodeRange { get; }

        LSLSourceCodeRange StateNameSourceCodeRange { get; }
    }
}