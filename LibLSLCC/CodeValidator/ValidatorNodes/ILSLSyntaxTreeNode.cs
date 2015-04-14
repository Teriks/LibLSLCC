using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes
{
    public interface ILSLReadOnlySyntaxTreeNode
    {
        ILSLReadOnlySyntaxTreeNode Parent { get; }

        bool HasErrors { get; }

        LSLSourceCodeRange SourceCodeRange { get; }

        T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor);
    }

    public interface ILSLSyntaxTreeNode : ILSLReadOnlySyntaxTreeNode
    {
        new ILSLSyntaxTreeNode Parent { get; set; }
    }
}