using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLFunctionCallNode : ILSLReadOnlyExprNode
    {
        string Name { get; }
        IReadOnlyList<ILSLReadOnlyExprNode> ParameterExpressions { get; }
        LSLFunctionSignature Signature { get; }
        ILSLExpressionListNode ParameterListNode { get; }
        ILSLFunctionDeclarationNode DefinitionNode { get; }
    }
}