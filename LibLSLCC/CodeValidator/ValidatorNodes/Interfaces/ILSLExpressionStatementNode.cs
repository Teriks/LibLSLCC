namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLExpressionStatementNode : ILSLReadOnlyCodeStatement
    {
        ILSLReadOnlyExprNode Expression { get;  }
        bool HasEffect { get; set; }
    }
}