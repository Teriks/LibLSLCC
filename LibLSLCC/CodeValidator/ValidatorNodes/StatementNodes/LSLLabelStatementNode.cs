using System.Collections.Generic;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLLabelStatementNode : ILSLLabelStatementNode, ILSLCodeStatement
    {
        private readonly List<LSLJumpStatementNode> _jumpsToHere;
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLLabelStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLLabelStatementNode(LSLParser.LabelStatementContext context, bool isSingleBlockStatement)
        {
            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            _jumpsToHere = new List<LSLJumpStatementNode>();
            SourceCodeRange = new LSLSourceCodeRange(context);

            LabelNameSourceCodeRange = new LSLSourceCodeRange(context.label_name);
            LabelPrefixSourceCodeRange = new LSLSourceCodeRange(context.label_prefix);
            SemiColonSourceCodeRange = new LSLSourceCodeRange(context.semi_colon);
        }



        internal LSLParser.LabelStatementContext ParserContext { get; private set; }
        internal LSLParser.CodeScopeContext ParentScopeParserContext { get; set; }
        public LSLCodeScopeNode ParentScope { get; set; }


        public IReadOnlyList<LSLJumpStatementNode> JumpsToHere
        {
            get { return _jumpsToHere.AsReadOnly(); }
        }



        public void AddJumpToHere(LSLJumpStatementNode jump)
        {
            _jumpsToHere.Add(jump);
        }


        #region ILSLCodeStatement Members


        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
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
            return visitor.VisitLabelStatement(this);
        }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        public ILSLCodeStatement ReturnPath { get; set; }

        IReadOnlyList<ILSLJumpStatementNode> ILSLLabelStatementNode.JumpsToHere
        {
            get { return JumpsToHere; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLDeadCodeType DeadCodeType { get; set; }

        public string LabelName
        {
            get { return ParserContext.label_name.Text; }
        }

        public ulong ScopeId { get; set; }



        public static
            LSLLabelStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLLabelStatementNode(sourceRange, Err.Err);
        }


        public LSLSourceCodeRange LabelPrefixSourceCodeRange
        {
            get;
            private set;
        }

        public LSLSourceCodeRange LabelNameSourceCodeRange
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