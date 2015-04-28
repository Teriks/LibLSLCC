using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLPostfixOperationNode : ILSLPostfixOperationNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLPostfixOperationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLPostfixOperationNode(LSLParser.Expr_PostfixOperationContext context, LSLType resultType,
            ILSLExprNode leftExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (leftExpression == null)
            {
                throw new ArgumentNullException("leftExpression");
            }

            ParserContext = context;
            Type = resultType;
            LeftExpression = leftExpression;
            LeftExpression.Parent = this;

            ParseAndSetOperation(context.operation.Text);

            SourceCodeRange = new LSLSourceCodeRange(context);

            OperationSourceCodeRange = new LSLSourceCodeRange(context.operation);
        }



        internal LSLParser.Expr_PostfixOperationContext ParserContext { get; private set; }
        public ILSLExprNode LeftExpression { get; private set; }




        #region ILSLExprNode Members


        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLPostfixOperationNode(ParserContext, Type, LeftExpression.Clone())
            {
                HasErrors = HasErrors,
                Parent = Parent,
            };
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        public LSLSourceCodeRange OperationSourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitPostfixOperation(this);
        }



        public LSLType Type { get; private set; }

        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.PostfixExpression; }
        }

        public bool IsConstant
        {
            get { return LeftExpression.IsConstant; }
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




        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        public LSLPostfixOperationType Operation { get; private set; }

        public string OperationString
        {
            get { return ParserContext.operation.Text; }
        }

        ILSLReadOnlyExprNode ILSLPostfixOperationNode.LeftExpression
        {
            get { return LeftExpression; }
        }



        public static
            LSLPostfixOperationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLPostfixOperationNode(sourceRange, Err.Err);
        }



        private void ParseAndSetOperation(string operationString)
        {
            Operation = LSLPostfixOperationTypeTools.ParseFromOperator(operationString);
        }




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion
    }
}