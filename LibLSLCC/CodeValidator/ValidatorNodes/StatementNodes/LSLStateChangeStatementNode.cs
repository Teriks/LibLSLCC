using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLStateChangeStatementNode : ILSLStateChangeStatementNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLStateChangeStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLStateChangeStatementNode(LSLParser.StateChangeStatementContext context, bool isSingleBlockStatement)
        {
            ParserContext = context;
            IsSingleBlockStatement = isSingleBlockStatement;
            SourceCodeRange = new LSLSourceCodeRange(context);
            StateKeywordSourceCodeRange = new LSLSourceCodeRange(context.state_keyword);
            SemiColonSourceCodeRange = new LSLSourceCodeRange(context.semi_colon);
            StateNameSourceCodeRange = new LSLSourceCodeRange(context.state_target);
        }



        internal LSLParser.StateChangeStatementContext ParserContext { get; private set; }




        #region ILSLCodeStatement Members


        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }

        public string StateTargetName
        {
            get { return ParserContext.state_target.Text; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLStateChangeStatementNode.Parent
        {
            get { return Parent; }
        }

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
            return visitor.VisitStateChangeStatement(this);
        }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        public ILSLCodeStatement ReturnPath { get; set; }


        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public ulong ScopeId { get; set; }



        public static
            LSLStateChangeStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLStateChangeStatementNode(sourceRange, Err.Err);
        }


        public LSLSourceCodeRange StateKeywordSourceCodeRange
        {
            get;
            private set;
        }

        public LSLSourceCodeRange StateNameSourceCodeRange
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