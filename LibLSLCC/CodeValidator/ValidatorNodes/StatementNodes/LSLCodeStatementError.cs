using System;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public interface ILSLCodeStatementError : ILSLReadOnlyCodeStatement
    {
    }

    public class LSLCodeStatementError : ILSLCodeStatementError, ILSLCodeStatement
    {
        public LSLCodeStatementError(LSLParser.CodeStatementContext parserContext, bool isSingleBlockStatement)
        {
            ParserContext = parserContext;
            IsSingleBlockStatement = isSingleBlockStatement;
        }



        internal LSLParser.CodeStatementContext ParserContext { get; private set; }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public bool HasReturnStatementNode
        {
            get { return false; }
        }

        public bool IsSingleBlockStatement { get; set; }

        public ILSLSyntaxTreeNode Parent { get; set; }


        public ILSLCodeStatement ReturnPath { get; set; }
        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        public bool HasErrors
        {
            get { return true; }
        }

        public LSLSourceCodeRange SourceCodeRange
        {
            get { return new LSLSourceCodeRange(ParserContext); }
        }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            throw new NotImplementedException("Visited LSLCodeStatementError");
        }



        public bool HasReturnPath
        {
            get { return false; }
        }

        public int StatementIndex { get; set; }

        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public ulong ScopeId { get; set; }
    }
}