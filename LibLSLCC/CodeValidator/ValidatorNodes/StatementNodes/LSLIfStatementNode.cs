using System;
using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLIfStatementNode : ILSLIfStatementNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLIfStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLIfStatementNode(LSLParser.IfStatementContext context, LSLCodeScopeNode code,
            ILSLExprNode conditionExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            if (conditionExpression == null)
            {
                throw new ArgumentNullException("conditionExpression");
            }

            ParserContext = context;
            Code = code;
            ConditionExpression = conditionExpression;

            Code.Parent = this;
            ConditionExpression.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);

            IfKeywordSourceCodeRange = new LSLSourceCodeRange(context.if_keyword);
            OpenParenthSourceCodeRange = new LSLSourceCodeRange(context.open_parenth);
            CloseParenthSourceCodeRange = new LSLSourceCodeRange(context.close_parenth);
        }



        public IEnumerable<LSLConstantJumpDescription> ConstantJumps
        {
            get { return Code.ConstantJumps; }
        }

        internal LSLParser.IfStatementContext ParserContext { get; private set; }


        public LSLCodeScopeNode Code { get; private set; }

        public ILSLExprNode ConditionExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLCodeScopeNode ILSLIfStatementNode.Code
        {
            get { return Code; }
        }

        ILSLReadOnlyExprNode ILSLIfStatementNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }




        #region ILSLBranchStatementNode Members


        public bool IsConstantBranch
        {
            get { return ConditionExpression.IsConstant; }
        }


        #endregion




        #region ILSLReturnPathNode Members


        public bool HasReturnPath
        {
            get { return Code.HasReturnPath; }
        }


        #endregion




        #region ILSLTreeNode Members


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitIfStatement(this);
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        public static
            LSLIfStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLIfStatementNode(sourceRange, Err.Err);
        }


        public LSLSourceCodeRange IfKeywordSourceCodeRange
        {
            get;
            private set;
        }

        public LSLSourceCodeRange OpenParenthSourceCodeRange
        {
            get;
            private set;
        }

        public LSLSourceCodeRange CloseParenthSourceCodeRange
        {
            get;
            private set;
        }
    }
}