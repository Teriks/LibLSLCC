using System;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLRotationLiteralNode : ILSLRotationLiteralNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLRotationLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLRotationLiteralNode(LSLParser.RotationLiteralContext context, ILSLExprNode x, ILSLExprNode y,
            ILSLExprNode z, ILSLExprNode s)
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

            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            s.Parent = this;

            ParserContext = context;
            XExpression = x;
            YExpression = y;
            ZExpression = z;
            SExpression = s;

            SourceCodeRange = new LSLSourceCodeRange(context);

            CommaOneSourceCodeRange = new LSLSourceCodeRange(context.comma_one);
            CommaTwoSourceCodeRange = new LSLSourceCodeRange(context.comma_two);
            CommaThreeSourceCodeRange = new LSLSourceCodeRange(context.comma_three);
        }



        internal LSLParser.RotationLiteralContext ParserContext { get; private set; }


        public ILSLExprNode XExpression { get; private set; }

        public ILSLExprNode YExpression { get; private set; }

        public ILSLExprNode ZExpression { get; private set; }

        public ILSLExprNode SExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.XExpression
        {
            get { return XExpression; }
        }

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.YExpression
        {
            get { return YExpression; }
        }

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.ZExpression
        {
            get { return ZExpression; }
        }

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.SExpression
        {
            get { return SExpression; }
        }




        #region ILSLExprNode Members


        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLRotationLiteralNode(ParserContext, XExpression.Clone(), YExpression.Clone(),
                ZExpression.Clone(),
                SExpression.Clone())
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }

        public LSLSourceCodeRange CommaOneSourceCodeRange { get; private set; }
        public LSLSourceCodeRange CommaTwoSourceCodeRange { get; private set; }
        public LSLSourceCodeRange CommaThreeSourceCodeRange { get; private set; }

        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitRotationLiteral(this);
        }



        public LSLType Type
        {
            get { return LSLType.Rotation; }
        }


        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }


        public bool IsConstant
        {
            get
            {
                var precondition =
                    XExpression != null &&
                    YExpression != null &&
                    ZExpression != null &&
                    SExpression != null;

                return precondition &&
                       XExpression.IsConstant &&
                       YExpression.IsConstant &&
                       ZExpression.IsConstant &&
                       SExpression.IsConstant;
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
            LSLRotationLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLRotationLiteralNode(sourceRange, Err.Err);
        }
    }
}