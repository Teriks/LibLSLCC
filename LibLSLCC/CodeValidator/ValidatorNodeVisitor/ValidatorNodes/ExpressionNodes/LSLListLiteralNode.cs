using System;
using System.Collections.Generic;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLListLiteralNode : ILSLListLiteralNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLListLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLListLiteralNode(LSLParser.ListLiteralContext context, LSLExpressionListNode expressionListNode)
        {
            if (expressionListNode == null)
            {
                throw new ArgumentNullException("expressionListNode");
            }


            IsConstant = expressionListNode.AllExpressionsConstant;
            ParserContext = context;
            ExpressionListNode = expressionListNode;


            ExpressionListNode.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.ListLiteralContext ParserContext { get; private set; }

        public IReadOnlyList<ILSLExprNode> ListEntryExpressions
        {
            get { return ExpressionListNode.ExpressionNodes; }
        }


        public LSLExpressionListNode ExpressionListNode { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        IReadOnlyList<ILSLReadOnlyExprNode> ILSLListLiteralNode.ListEntryExpressions
        {
            get { return ListEntryExpressions; }
        }


        ILSLExpressionListNode ILSLListLiteralNode.ExpressionListNode
        {
            get { return ExpressionListNode; }
        }




        #region ILSLExprNode Members


        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLListLiteralNode(ParserContext, ExpressionListNode.Clone())
            {
                HasErrors = HasErrors,
                IsConstant = IsConstant,
                Parent = Parent,
                ParserContext = ParserContext
            };
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitListLiteral(this);
        }



        public LSLType Type
        {
            get { return LSLType.List; }
        }

        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }

        public bool IsConstant { get; private set; }



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
            LSLListLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLListLiteralNode(sourceRange, Err.Err);
        }
    }
}