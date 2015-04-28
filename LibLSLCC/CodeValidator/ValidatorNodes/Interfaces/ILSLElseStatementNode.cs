using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLElseStatementNode : ILSLReturnPathNode, ILSLReadOnlyBranchStatementNode
    {
        ILSLCodeScopeNode Code { get; }

        LSLSourceCodeRange ElseKeywordSourceCodeRange { get; }
    }
}