using LibLSLCC.CodeValidator.Primitives;
namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLReturnStatementNode : ILSLReadOnlyCodeStatement
    {
        ILSLReadOnlyExprNode ReturnExpression { get; }
        bool HasReturnExpression { get; }


        LSLSourceCodeRange ReturnKeywordSourceCodeRange { get; }

        LSLSourceCodeRange SemiColonSourceCodeRange { get; }
    }
}