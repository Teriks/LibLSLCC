using Antlr4.Runtime;

namespace LibLSLCC.CodeValidator.Visitor
{
    internal partial class LSLCodeValidationVisitor
    {
        #region InternalClasses


        /// <summary>
        ///     Represents a generic binary expression operation.
        ///     I made alternative grammar label names for each different type of expression operation so that I could
        ///     differentiate between postfix, casts etc.. unfortunately each type of binary operation must be named
        ///     uniquely or antlr will complain, so the different binary expression visitor functions build this object
        ///     and pass it it to VisitBinaryExpression.  All binary expression contexts have the same properties,
        ///     but they cannot be made to derive from one type as antlr generates them and it does not have a feature to
        ///     allow it
        /// </summary>
        private struct BinaryExpressionContext
        {
            public readonly LSLParser.ExpressionContext LeftContext;
            public readonly bool ModifiesLValue;
            public readonly IToken OperationToken;
            public readonly LSLParser.ExpressionContext OriginalContext;
            public readonly LSLParser.ExpressionContext RightContext;



            public BinaryExpressionContext(LSLParser.ExpressionContext exprLvalue, IToken operationToken,
                LSLParser.ExpressionContext exprRvalue, LSLParser.ExpressionContext originalContext,
                bool modifiesLValue = false)
            {
                LeftContext = exprLvalue;
                OperationToken = operationToken;
                RightContext = exprRvalue;
                OriginalContext = originalContext;
                ModifiesLValue = modifiesLValue;
            }
        }


        #endregion
    }
}