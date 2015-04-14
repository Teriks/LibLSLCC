using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public enum LSLExpressionListType
    {
        ListInitializer,
        UserFunctionCallParameters,
        LibraryFunctionCallParameters,
        ForLoopAfterthoughts
    }


    public class LSLExpressionListNode : ILSLExpressionListNode, ILSLSyntaxTreeNode
    {
        private readonly List<ILSLExprNode> _expressionNodes = new List<ILSLExprNode>();


        // ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLExpressionListNode(LSLSourceCodeRange sourceRange, Err err)

            // ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLExpressionListNode(LSLParser.OptionalExpressionListContext parserContext,
            LSLExpressionListType listType)
        {
            ParserContext = parserContext;
            ListType = listType;
            SourceCodeRange = new LSLSourceCodeRange(parserContext);
        }



        internal LSLExpressionListNode(LSLParser.OptionalExpressionListContext parserContext,
            IEnumerable<ILSLExprNode> expressions,
            LSLExpressionListType listType)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }

            ListType = listType;

            foreach (var lslExprNode in expressions)
            {
                AddExpression(lslExprNode);
            }
            SourceCodeRange = new LSLSourceCodeRange(parserContext);
        }



        internal LSLParser.OptionalExpressionListContext ParserContext { get; set; }


        public IReadOnlyList<ILSLExprNode> ExpressionNodes
        {
            get { return _expressionNodes; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLExpressionListType ListType { get; private set; }

        IReadOnlyList<ILSLReadOnlyExprNode> ILSLExpressionListNode.ExpressionNodes
        {
            get { return _expressionNodes; }
        }


        public bool AllExpressionsConstant
        {
            get { return ExpressionNodes.Count == 0 || ExpressionNodes.All(lslExprNode => lslExprNode.IsConstant); }
        }

        public bool HasExpressionNodes
        {
            get { return ExpressionNodes.Count > 0; }
        }




        #region ILSLTreeNode Members


        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        public bool HasErrors { get; set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (ListType == LSLExpressionListType.ForLoopAfterthoughts)
            {
                return visitor.VisitForLoopAfterthoughts(this);
            }

            if (ListType == LSLExpressionListType.LibraryFunctionCallParameters)
            {
                return visitor.VisitLibraryFunctionCallParameters(this);
            }

            if (ListType == LSLExpressionListType.UserFunctionCallParameters)
            {
                return visitor.VisitUserFunctionCallParameters(this);
            }

            if (ListType == LSLExpressionListType.ListInitializer)
            {
                return visitor.VisitListLiteralInitializerList(this);
            }

            throw new InvalidOperationException("Visit LSLExpressionList, unknown ListTypes");
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        #endregion




        public static LSLExpressionListNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLExpressionListNode(sourceRange, Err.Err);
        }



        public void AddExpression(ILSLExprNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Parent = this;

            _expressionNodes.Add(node);
        }



        public LSLExpressionListNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var r = new LSLExpressionListNode(ParserContext, ListType)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };


            foreach (var expressionNode in _expressionNodes)
            {
                r.AddExpression(expressionNode);
            }

            return r;
        }




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion
    }
}