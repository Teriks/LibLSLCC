using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLTypecastExprNode : ILSLTypecastExprNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLTypecastExprNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLTypecastExprNode(LSLParser.Expr_TypeCastContext context, LSLType result,
            ILSLExprNode castedExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (castedExpression == null)
            {
                throw new ArgumentNullException("castedExpression");
            }


            ParserContext = context;
            CastedExpression = castedExpression;
            Type = result;
            CastedExpression.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.Expr_TypeCastContext ParserContext { get; private set; }

        public ILSLExprNode CastedExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLReadOnlyExprNode ILSLTypecastExprNode.CastedExpression
        {
            get { return CastedExpression; }
        }

        public LSLType CastToType
        {
            get { return LSLTypeTools.FromLSLTypeString(ParserContext.cast_type.Text); }
        }

        public string CastToTypeString
        {
            get { return ParserContext.cast_type.Text; }
        }




        #region ILSLExprNode Members


        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLTypecastExprNode(ParserContext, Type, CastedExpression.Clone())
            {
                HasErrors = HasErrors,
                Parent = Parent,
            };
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitTypecastExpression(this);
        }



        public LSLType Type { get; private set; }

        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.TypecastExpression; }
        }

        public bool IsConstant
        {
            get { return CastedExpression.IsConstant; }
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
            LSLTypecastExprNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLTypecastExprNode(sourceRange, Err.Err);
        }
    }
}