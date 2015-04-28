using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLLabelStatementNode : ILSLReadOnlyCodeStatement
    {
        string LabelName { get; }
        IReadOnlyList<ILSLJumpStatementNode> JumpsToHere { get; }



        LSLSourceCodeRange LabelPrefixSourceCodeRange { get; }

        LSLSourceCodeRange LabelNameSourceCodeRange { get; }

        LSLSourceCodeRange SemiColonSourceCodeRange { get; }
    }
}