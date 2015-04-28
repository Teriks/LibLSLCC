using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.LoopNodes
{
    public class LSLWhileLoopNode : ILSLWhileLoopNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLWhileLoopNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLWhileLoopNode(LSLParser.WhileLoopContext context, ILSLExprNode conditionExpression,
            LSLCodeScopeNode code, bool isSingleBlockStatement)
        {
            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            if (conditionExpression == null)
            {
                throw new ArgumentNullException("conditionExpression");
            }

            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            ConditionExpression = conditionExpression;
            Code = code;

            SourceCodeRange = new LSLSourceCodeRange(context);
            WhileKeywordSourceCodeRange = new LSLSourceCodeRange(context.loop_keyword);
            OpenParenthSourceCodeRange = new LSLSourceCodeRange(context.open_parenth);
            CloseParenthSourceCodeRange = new LSLSourceCodeRange(context.close_parenth);


            code.Parent = this;
            conditionExpression.Parent = this;
        }



        internal LSLParser.WhileLoopContext ParserContext { get; private set; }
        public bool IsSingleBlockStatement { get; private set; }


        public ILSLExprNode ConditionExpression { get; private set; }
        public LSLCodeScopeNode Code { get; private set; }


        public ILSLCodeStatement ReturnPath { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        ILSLCodeScopeNode ILSLWhileLoopNode.Code
        {
            get { return Code; }
        }

        ILSLCodeScopeNode ILSLLoopNode.Code
        {
            get { return Code; }
        }


        ILSLReadOnlyExprNode ILSLWhileLoopNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }

        ILSLReadOnlyExprNode ILSLLoopNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }


        public LSLDeadCodeType DeadCodeType { get; set; }


        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public ulong ScopeId { get; set; }


        public bool HasReturnPath
        {
            get { return false; }
        }

        public int StatementIndex { get; set; }

        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }




        #region ILSLTreeNode Members


        public ILSLSyntaxTreeNode Parent { get; set; }

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitWhileLoop(this);
        }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        public static
            LSLWhileLoopNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLWhileLoopNode(sourceRange, Err.Err);
        }


        public LSLSourceCodeRange WhileKeywordSourceCodeRange
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