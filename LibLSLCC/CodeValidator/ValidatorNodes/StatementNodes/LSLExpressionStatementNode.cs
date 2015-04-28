using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLExpressionStatementNode : ILSLExpressionStatementNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLExpressionStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLExpressionStatementNode(LSLParser.ExpressionStatementContext context, ILSLExprNode expression,
            bool isSingleBlockStatement, bool hasEffect)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            IsSingleBlockStatement = isSingleBlockStatement;
            HasEffect = hasEffect;
            ParserContext = context;
            Expression = expression;

            expression.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.ExpressionStatementContext ParserContext { get; private set; }


        public ILSLExprNode Expression { get; private set; }


        public ILSLCodeStatement ReturnPath { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlyExprNode ILSLExpressionStatementNode.Expression { get { return Expression; } }

        public ulong ScopeId { get; set; }




        #region ILSLCodeStatement Members


        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public bool HasEffect { get; set; }
        public int StatementIndex { get; set; }

        public bool HasReturnPath
        {
            get { return false; }
        }

        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitExpressionStatement(this);
        }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        public static
            LSLExpressionStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLExpressionStatementNode(sourceRange, Err.Err);
        }
    }
}