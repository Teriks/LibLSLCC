using System.Collections.Generic;

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLListLiteralNode : ILSLReadOnlyExprNode
    {
        IReadOnlyList<ILSLReadOnlyExprNode> ListEntryExpressions { get; }
        ILSLExpressionListNode ExpressionListNode { get; }
    }
}