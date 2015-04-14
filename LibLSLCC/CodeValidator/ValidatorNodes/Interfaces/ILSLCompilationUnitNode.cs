using System.Collections.Generic;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLCompilationUnitNode : ILSLReadOnlySyntaxTreeNode
    {
        IReadOnlyList<ILSLVariableDeclarationNode> GlobalVariableDeclarations { get; }
        IReadOnlyList<ILSLFunctionDeclarationNode> FunctionDeclarations { get; }
        IReadOnlyList<ILSLStateScopeNode> StateDeclarations { get; }
        ILSLStateScopeNode DefaultState { get; }
    }
}