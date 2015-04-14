using System.Collections.Generic;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLLabelStatementNode : ILSLReadOnlyCodeStatement
    {
        string LabelName { get; }
        IReadOnlyList<ILSLJumpStatementNode> JumpsToHere { get; }
    }
}