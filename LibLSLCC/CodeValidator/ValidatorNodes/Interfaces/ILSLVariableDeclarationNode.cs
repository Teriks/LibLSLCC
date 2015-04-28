using System.Collections.Generic;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLVariableDeclarationNode : ILSLReadOnlyCodeStatement
    {
        string Name { get; }
        LSLType Type { get; }
        string TypeString { get; }

        ILSLVariableNode VariableNode { get; }

        IReadOnlyList<ILSLVariableNode> References { get; } 

        bool HasDeclarationExpression { get; }
        bool IsLocal { get; }
        bool IsGlobal { get; }
        bool IsParameter { get; }
        bool IsLibraryConstant { get; }
        ILSLReadOnlyExprNode DeclarationExpression { get; }

        LSLSourceCodeRange TypeSourceCodeRange { get; }

        LSLSourceCodeRange NameSourceCodeRange { get; }

        LSLSourceCodeRange OperatorSourceCodeRange { get; }
    }
}