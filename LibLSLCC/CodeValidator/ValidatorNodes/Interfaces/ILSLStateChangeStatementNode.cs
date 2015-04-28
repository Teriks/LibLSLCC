using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLStateChangeStatementNode : ILSLReadOnlyCodeStatement
    {
        string StateTargetName { get; }
        new ILSLReadOnlySyntaxTreeNode Parent { get; }
        new bool HasErrors { get; }

        LSLSourceCodeRange StateKeywordSourceCodeRange { get; }

        LSLSourceCodeRange StateNameSourceCodeRange { get; }

        LSLSourceCodeRange SemiColonSourceCodeRange { get; }
    }
}