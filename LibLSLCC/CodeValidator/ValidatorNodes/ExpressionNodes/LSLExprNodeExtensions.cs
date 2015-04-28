using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public static class LSLExprNodeExtensions
    {
        public static bool IsLiteral(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.Literal;
        }



        public static bool IsCompoundExpression(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.BinaryExpression ||
                   node.ExpressionType == LSLExpressionType.ParenthesizedExpression ||
                   node.ExpressionType == LSLExpressionType.PostfixExpression ||
                   node.ExpressionType == LSLExpressionType.PrefixExpression ||
                   node.ExpressionType == LSLExpressionType.TypecastExpression;
        }



        public static bool IsFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.LibraryFunction ||
                   node.ExpressionType == LSLExpressionType.UserFunction;
        }



        public static bool IsLibraryFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.LibraryFunction;
        }



        public static bool IsUserFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.UserFunction;
        }



        public static bool IsVariable(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.GlobalVariable ||
                   node.ExpressionType == LSLExpressionType.LocalVariable;
        }



        public static bool IsLocalVariable(this ILSLReadOnlyExprNode node)
        {
            return
                node.ExpressionType == LSLExpressionType.LocalVariable;
        }



        public static bool IsGlobalVariable(this ILSLReadOnlyExprNode node)
        {
            return
                node.ExpressionType == LSLExpressionType.GlobalVariable;
        }
    }
}