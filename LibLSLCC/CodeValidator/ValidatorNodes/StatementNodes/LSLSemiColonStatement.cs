using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLSemiColonStatement : ILSLSemiColonStatement, ILSLCodeStatement
    {
        internal LSLSemiColonStatement(LSLParser.CodeStatementContext context, bool isSingleBlockStatement)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParserContext = context;
            IsSingleBlockStatement = isSingleBlockStatement;
        }



        internal LSLParser.CodeStatementContext ParserContext { get; private set; }
        public bool IsSingleBlockStatement { get; private set; }


        public ILSLCodeStatement ReturnPath { get; set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public ulong ScopeId { get; set; }

        public bool IsDeadCode { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange
        {
            get { return new LSLSourceCodeRange(ParserContext); }
        }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitSemiColonStatement(this);
        }



        public bool HasReturnPath
        {
            get { return false; }
        }

        public int StatementIndex { get; set; }
        public bool IsLastStatementInScope { get; set; }
    }
}