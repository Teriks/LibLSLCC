using System;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLTupleAccessorNode : ILSLTupleAccessorNode, ILSLExprNode
    {
        #region AccessType enum


        #endregion




        #region Component enum


        #endregion




// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLTupleAccessorNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLTupleAccessorNode(LSLParser.Expr_DotAccessorContext context, ILSLExprNode accessedExpression,
            LSLType accessedType,
            LSLTupleComponent accessedComponent)
        {
            if (accessedType != LSLType.Vector && accessedType != LSLType.Rotation)
            {
                throw new ArgumentException("accessedType can only be LSLType.Vector or LSLType.Rotation");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (accessedExpression == null)
            {
                throw new ArgumentNullException("context");
            }

            ParserContext = context;
            AccessedComponent = accessedComponent;
            AccessedType = accessedType;
            AccessedExpression = accessedExpression;
            AccessedExpression.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.Expr_DotAccessorContext ParserContext { get; private set; }
        public ILSLExprNode AccessedExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public string AccessedComponentString
        {
            get { return ParserContext.member.Text; }
        }


        public LSLTupleComponent AccessedComponent { get; private set; }


        public LSLType AccessedType { get; private set; }


        ILSLReadOnlyExprNode ILSLTupleAccessorNode.AccessedExpression
        {
            get { return AccessedExpression; }
        }




        #region ILSLExprNode Members


        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLTupleAccessorNode(ParserContext, AccessedExpression.Clone(), AccessedType, AccessedComponent)
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
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }

            return visitor.VisitVecRotAccessor(this);
        }



        public LSLType Type
        {
            get { return LSLType.Float; }
        }


        public LSLExpressionType ExpressionType
        {
            get
            {
                return AccessedType == LSLType.Vector
                    ? LSLExpressionType.VectorComponentAccess
                    : LSLExpressionType.RotationComponentAccess;
            }
        }


        public bool IsConstant
        {
            get { return AccessedExpression.IsConstant; }
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
            LSLTupleAccessorNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLTupleAccessorNode(sourceRange, Err.Err);
        }
    }
}