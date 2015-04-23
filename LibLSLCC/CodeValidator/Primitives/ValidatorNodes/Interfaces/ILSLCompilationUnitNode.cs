using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLCompilationUnitNode : ILSLReadOnlySyntaxTreeNode
    {
        IReadOnlyList<LSLComment> Comments { get; }
        IReadOnlyList<ILSLVariableDeclarationNode> GlobalVariableDeclarations { get; }
        IReadOnlyList<ILSLFunctionDeclarationNode> FunctionDeclarations { get; }
        IReadOnlyList<ILSLStateScopeNode> StateDeclarations { get; }
        ILSLStateScopeNode DefaultState { get; }
    }
}