using System.Collections.Generic;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLFunctionDeclarationNode : ILSLReadOnlySyntaxTreeNode
    {
        bool HasParameters { get; }
        IReadOnlyList<ILSLFunctionCallNode> References { get; }
        IReadOnlyList<ILSLParameterNode> ParameterNodes { get; }
        string ReturnTypeString { get; }
        string Name { get; }
        LSLType ReturnType { get; }
        ILSLParameterListNode ParameterListNode { get; }
        ILSLCodeScopeNode FunctionBodyNode { get; }
    }
}