using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLParenthesizedExpressionNode : ILSLParenthesizedExpressionNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLParenthesizedExpressionNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLParenthesizedExpressionNode(LSLParser.ParenthesizedExpressionContext context,
            ILSLExprNode innerExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (innerExpression == null)
            {
                throw new ArgumentNullException("innerExpression");
            }

            ParserContext = context;
            InnerExpression = innerExpression;
            InnerExpression.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.ParenthesizedExpressionContext ParserContext { get; private set; }
        public ILSLExprNode InnerExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        ILSLReadOnlyExprNode ILSLParenthesizedExpressionNode.InnerExpression
        {
            get { return InnerExpression; }
        }




        #region ILSLExprNode Members


        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLParenthesizedExpressionNode(ParserContext, InnerExpression.Clone())
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitParenthesizedExpression(this);
        }



        public LSLType Type
        {
            get { return InnerExpression.Type; }
        }

        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.ParenthesizedExpression; }
        }

        public bool IsConstant
        {
            get { return InnerExpression.IsConstant; }
        }



        public string DescribeType()
        {
            return "(" + Type + (this.IsLiteral() ? " Literal)" : ")");
        }



        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        public static
            LSLParenthesizedExpressionNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLParenthesizedExpressionNode(sourceRange, Err.Err);
        }
    }
}