using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLReadOnlyExprNode : ILSLReadOnlySyntaxTreeNode
    {
        LSLType Type { get; }


        LSLExpressionType ExpressionType { get; }

        /// <summary>
        ///     Is this expression constant (can it be calculated at compile time)
        /// </summary>
        bool IsConstant { get; }


        string DescribeType();

        ILSLReadOnlyExprNode Clone();
    }
}