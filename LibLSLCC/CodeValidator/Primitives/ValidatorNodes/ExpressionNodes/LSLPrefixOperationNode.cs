using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLPrefixOperationNode : ILSLPrefixOperationNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLPrefixOperationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLPrefixOperationNode(LSLParser.Expr_PrefixOperationContext context, LSLType resultType,
            ILSLExprNode rightExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (rightExpression == null)
            {
                throw new ArgumentNullException("rightExpression");
            }


            ParserContext = context;
            Type = resultType;
            RightExpression = rightExpression;
            RightExpression.Parent = this;

            ParseAndSetOperation(context.operation.Text);

            SourceCodeRange = new LSLSourceCodeRange(context);

            OperationSourceCodeRange = new LSLSourceCodeRange(context.operation);
        }



        internal LSLParser.Expr_PrefixOperationContext ParserContext { get; private set; }
        public ILSLExprNode RightExpression { get; private set; }




        #region ILSLExprNode Members


        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLPrefixOperationNode(ParserContext, Type, RightExpression.Clone())
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
            return visitor.VisitPrefixOperation(this);
        }



        public LSLType Type { get; private set; }

        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.PrefixExpression; }
        }

        public bool IsConstant
        {
            get { return RightExpression.IsConstant; }
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


        public LSLPrefixOperationType Operation { get; private set; }

        public string OperationString
        {
            get { return ParserContext.operation.Text; }
        }

        ILSLReadOnlyExprNode ILSLPrefixOperationNode.RightExpression
        {
            get { return RightExpression; }
        }



        public static
            LSLPrefixOperationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLPrefixOperationNode(sourceRange, Err.Err);
        }



        private void ParseAndSetOperation(string operationString)
        {
            Operation = LSLPrefixOperationTypeTools.ParseFromOperator(operationString);
        }




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion
    }
}