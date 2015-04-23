using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.LoopNodes
{
    public class LSLForLoopNode : ILSLForLoopNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLForLoopNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLForLoopNode(LSLParser.ForLoopContext context, ILSLExpressionListNode initExpression,
            ILSLExprNode conditionExpression,
            LSLExpressionListNode afterthoughExpressions, LSLCodeScopeNode code, bool isSingleBlockStatement)
        {
            if (afterthoughExpressions == null)
            {
                throw new ArgumentNullException("afterthoughExpressions");
            }


            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            InitExpressionsList = initExpression;
            ConditionExpression = conditionExpression;
            AfterthoughExpressions = afterthoughExpressions;
            Code = code;

            SourceCodeRange = new LSLSourceCodeRange(context);
            FirstSemiColonSourceCodeRange = new LSLSourceCodeRange(context.first_semi_colon);
            SecondSemiColonSourceCodeRange = new LSLSourceCodeRange(context.second_semi_colon);
            OpenParenthSourceCodeRange = new LSLSourceCodeRange(context.open_parenth);
            CloseParenthSourceCodeRange = new LSLSourceCodeRange(context.close_parenth);
            ForKeywordSourceCodeRange = new LSLSourceCodeRange(context.loop_keyword);

            afterthoughExpressions.Parent = this;


            if (code != null)
            {
                code.Parent = this;
            }

            if (conditionExpression != null)
            {
                conditionExpression.Parent = this;
            }


            if (initExpression != null)
            {
                initExpression.Parent = this;
            }
        }



        internal LSLParser.ForLoopContext ParserContext { get; private set; }


        public ILSLExpressionListNode InitExpressionsList { get; private set; }
        public ILSLExprNode ConditionExpression { get; private set; }
        public LSLExpressionListNode AfterthoughExpressions { get; private set; }
        public LSLCodeScopeNode Code { get; private set; }


        public ILSLCodeStatement ReturnPath { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        ILSLReadOnlyExprNode ILSLForLoopNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }

        ILSLReadOnlyExprNode ILSLLoopNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }

        ILSLExpressionListNode ILSLForLoopNode.AfterthoughExpressions
        {
            get { return AfterthoughExpressions; }
        }

        ILSLCodeScopeNode ILSLForLoopNode.Code
        {
            get { return Code; }
        }

        ILSLCodeScopeNode ILSLLoopNode.Code
        {
            get { return Code; }
        }


        ILSLExpressionListNode ILSLForLoopNode.InitExpressionsList
        {
            get { return InitExpressionsList; }
        }


        public bool HasInitExpression
        {
            get { return InitExpressionsList != null; }
        }

        public bool HasConditionExpression
        {
            get { return ConditionExpression != null; }
        }

        public bool HasAfterthoughtExpressions
        {
            get { return AfterthoughExpressions != null; }
        }


        public LSLDeadCodeType DeadCodeType { get; set; }


        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }


        public ulong ScopeId { get; set; }




        #region ILSLCodeStatement Members


        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }

        public int StatementIndex { get; set; }

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }

        public LSLSourceCodeRange ForKeywordSourceCodeRange { get; private set; }

        public LSLSourceCodeRange FirstSemiColonSourceCodeRange { get; private set; }


        public LSLSourceCodeRange SecondSemiColonSourceCodeRange { get; private set; }


        public LSLSourceCodeRange OpenParenthSourceCodeRange { get; private set; }


        public LSLSourceCodeRange CloseParenthSourceCodeRange { get; private set; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitForLoop(this);
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
            LSLForLoopNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLForLoopNode(sourceRange, Err.Err);
        }
    }
}