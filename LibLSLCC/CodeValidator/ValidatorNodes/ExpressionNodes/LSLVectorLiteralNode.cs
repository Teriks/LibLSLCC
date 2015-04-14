using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLVectorLiteralNode : ILSLVectorLiteralNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLVectorLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLVectorLiteralNode(LSLParser.VectorLiteralContext context, ILSLExprNode x, ILSLExprNode y,
            ILSLExprNode z)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }
            x.Parent = this;

            if (y == null)
            {
                throw new ArgumentNullException("y");
            }
            y.Parent = this;

            if (z == null)
            {
                throw new ArgumentNullException("z");
            }
            z.Parent = this;


            ParserContext = context;
            XExpression = x;
            YExpression = y;
            ZExpression = z;

            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.VectorLiteralContext ParserContext { get; private set; }

        public ILSLExprNode XExpression { get; private set; }

        public ILSLExprNode YExpression { get; private set; }

        public ILSLExprNode ZExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLReadOnlyExprNode ILSLVectorLiteralNode.XExpression
        {
            get { return XExpression; }
        }

        ILSLReadOnlyExprNode ILSLVectorLiteralNode.YExpression
        {
            get { return YExpression; }
        }

        ILSLReadOnlyExprNode ILSLVectorLiteralNode.ZExpression
        {
            get { return ZExpression; }
        }




        #region ILSLExprNode Members


        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLVectorLiteralNode(ParserContext, XExpression.Clone(), YExpression.Clone(), ZExpression.Clone())
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
            return visitor.VisitVectorLiteral(this);
        }



        public LSLType Type
        {
            get { return LSLType.Vector; }
        }


        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }


        public bool IsConstant
        {
            get
            {
                var precondition = XExpression != null && YExpression != null && ZExpression != null;
                return precondition && XExpression.IsConstant && YExpression.IsConstant &&
                       ZExpression.IsConstant;
            }
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
            LSLVectorLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLVectorLiteralNode(sourceRange, Err.Err);
        }
    }
}