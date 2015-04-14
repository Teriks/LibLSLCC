using System.Collections.Generic;
using LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLExpressionListNode : ILSLReadOnlySyntaxTreeNode
    {
        LSLExpressionListType ListType { get; }
        IReadOnlyList<ILSLReadOnlyExprNode> ExpressionNodes { get; }
        bool AllExpressionsConstant { get; }
        bool HasExpressionNodes { get; }
    }
}