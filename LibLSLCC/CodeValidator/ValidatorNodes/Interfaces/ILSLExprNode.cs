namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLExprNode : ILSLReadOnlyExprNode, ILSLSyntaxTreeNode
    {
        new ILSLExprNode Clone();
    }
}