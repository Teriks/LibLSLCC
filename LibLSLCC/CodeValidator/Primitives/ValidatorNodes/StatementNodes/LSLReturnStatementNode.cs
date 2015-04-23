using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLReturnStatementNode : ILSLReturnStatementNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLReturnStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLReturnStatementNode(LSLParser.ReturnStatementContext context, ILSLExprNode returnExpression,
            bool isSingleBlockStatement)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (returnExpression == null)
            {
                throw new ArgumentNullException("returnExpression");
            }

            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            ReturnExpression = returnExpression;

            ReturnExpression.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLReturnStatementNode(LSLParser.ReturnStatementContext context, bool isSingleBlockStatement)
        {
            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            ReturnExpression = null;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.ReturnStatementContext ParserContext { get; private set; }

        public ILSLExprNode ReturnExpression { get; private set; }


        public ILSLCodeStatement ReturnPath { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLDeadCodeType DeadCodeType { get; set; }


        ILSLReadOnlyExprNode ILSLReturnStatementNode.ReturnExpression
        {
            get { return ReturnExpression; }
        }

        public bool HasReturnExpression
        {
            get { return ReturnExpression != null; }
        }


        public ulong ScopeId { get; set; }




        #region ILSLCodeStatement Members


        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitReturnStatement(this);
        }



        public int StatementIndex { get; set; }
        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }


        public bool HasReturnPath
        {
            get { return true; }
        }


        #endregion




        #region Nested type: Err


        #endregion




        public static
            LSLReturnStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLReturnStatementNode(sourceRange, Err.Err);
        }



        protected enum Err
        {
            Err
        }
    }
}