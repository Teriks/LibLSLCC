using System.Collections.Generic;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLStateScopeNode : ILSLReadOnlySyntaxTreeNode
    {
        string StateName { get; }
        bool IsDefinedState { get; }
        bool IsDefaultState { get; }
        IReadOnlyList<ILSLEventHandlerNode> EventHandlers { get; }
    }
}