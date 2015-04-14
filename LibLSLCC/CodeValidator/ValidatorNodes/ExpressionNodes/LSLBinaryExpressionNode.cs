using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLBinaryExpressionNode : ILSLBinaryExpressionNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLBinaryExpressionNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLBinaryExpressionNode(LSLParser.ExpressionContext context,
            ILSLExprNode leftExpression,
            ILSLExprNode rightExpression,
            LSLType returns,
            string operationString)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (leftExpression == null)
            {
                throw new ArgumentNullException("leftExpression");
            }

            if (rightExpression == null)
            {
                throw new ArgumentNullException("rightExpression");
            }


            ParserContext = context;
            Type = returns;
            LeftExpression = leftExpression;
            RightExpression = rightExpression;

            leftExpression.Parent = this;
            rightExpression.Parent = this;


            ParseAndSetOperation(operationString);
            OperationString = operationString;
        }



        internal LSLParser.ExpressionContext ParserContext { get; private set; }
        public ILSLExprNode LeftExpression { get; private set; }
        public ILSLExprNode RightExpression { get; private set; }




        #region ILSLExprNode Members


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; internal set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpression(this);
        }



        public string DescribeType()
        {
            return "(" + Type + (this.IsLiteral() ? " Literal)" : ")");
        }



        public LSLType Type { get; private set; }


        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.BinaryExpression; }
        }


        public bool IsConstant
        {
            get { return (LeftExpression.IsConstant && RightExpression.IsConstant); }
        }



        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }



        public ILSLSyntaxTreeNode Parent { get; set; }



        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLBinaryExpressionNode(ParserContext, RightExpression.Clone(), LeftExpression.Clone(), Type,
                OperationString)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }


        #endregion




        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        public LSLBinaryOperationType Operation { get; private set; }

        public string OperationString { get; private set; }


        ILSLReadOnlyExprNode ILSLBinaryExpressionNode.LeftExpression
        {
            get { return LeftExpression; }
        }


        ILSLReadOnlyExprNode ILSLBinaryExpressionNode.RightExpression
        {
            get { return RightExpression; }
        }



        public static
            LSLBinaryExpressionNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLBinaryExpressionNode(sourceRange, Err.Err);
        }



        private void ParseAndSetOperation(string operationString)
        {
            Operation = LSLBinaryOperationTypeTools.ParseFromOperator(operationString);
        }




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion
    }
}