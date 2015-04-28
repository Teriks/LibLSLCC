using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.LoopNodes
{
    public class LSLDoLoopNode : ILSLDoLoopNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLDoLoopNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLDoLoopNode(LSLParser.DoLoopContext context, LSLCodeScopeNode code, ILSLExprNode conditionExpression,
            bool isSingleBlockStatement)
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
            Code = code;
            ConditionExpression = conditionExpression;
            code.Parent = this;
            conditionExpression.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);
            DoKeywordSourceCodeRange = new LSLSourceCodeRange(context.loop_keyword);
            WhileKeywordSourceCodeRange = new LSLSourceCodeRange(context.while_keyword);
            OpenParenthSourceCodeRange = new LSLSourceCodeRange(context.open_parenth);
            CloseParenthSourceCodeRange = new LSLSourceCodeRange(context.close_parenth);
            SemiColonSourceCodeRange = new LSLSourceCodeRange(context.semi_colon);
        }



        internal LSLParser.DoLoopContext ParserContext { get; private set; }


        public LSLCodeScopeNode Code { get; private set; }


        public ILSLCodeStatement ReturnPath { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        ILSLCodeScopeNode ILSLDoLoopNode.Code
        {
            get { return Code; }
        }

        ILSLCodeScopeNode ILSLLoopNode.Code
        {
            get { return Code; }
        }


        public ILSLExprNode ConditionExpression { get; private set; }

        ILSLReadOnlyExprNode ILSLLoopNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }

        ILSLReadOnlyExprNode ILSLDoLoopNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }


        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public ulong ScopeId { get; set; }




        #region ILSLCodeStatement Members


        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public bool IsLastStatementInScope { get; set; }

        public LSLDeadCodeType DeadCodeType { get; set; }

        public bool IsDeadCode { get; set; }

        public int StatementIndex { get; set; }

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitDoLoop(this);
        }



        public bool HasReturnPath
        {
            get { return false; }
        }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        public static
            LSLDoLoopNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLDoLoopNode(sourceRange, Err.Err);
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

        public LSLSourceCodeRange DoKeywordSourceCodeRange
        {
            get;
            private set;
        }

        public LSLSourceCodeRange WhileKeywordSourceCodeRange
        {
            get;
            private set;
        }

        public LSLSourceCodeRange SemiColonSourceCodeRange
        {
            get;
            private set;
        }
    }
}