using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLJumpStatementNode : ILSLJumpStatementNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLJumpStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLJumpStatementNode(LSLParser.JumpStatementContext context, LSLLabelStatementNode jumpTarget,
            bool isSingleBlockStatement)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (jumpTarget == null)
            {
                throw new ArgumentNullException("jumpTarget");
            }

            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            JumpTarget = jumpTarget;
            jumpTarget.AddJumpToHere(this);
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.JumpStatementContext ParserContext { get; private set; }
        public LSLLabelStatementNode JumpTarget { get; set; }




        #region ILSLCodeStatement Members


        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public bool ConstantJump { get; set; }

        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public int StatementIndex { get; set; }

        public bool HasReturnPath
        {
            get { return false; }
        }

// ReSharper disable UnusedParameter.Local


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitJumpStatement(this);
        }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        public ILSLCodeStatement ReturnPath { get; set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLDeadCodeType DeadCodeType { get; set; }


        public string LabelName
        {
            get { return ParserContext.jump_target.Text; }
        }

        ILSLLabelStatementNode ILSLJumpStatementNode.JumpTarget
        {
            get { return JumpTarget; }
        }


        public ulong ScopeId { get; set; }



        public static
            LSLJumpStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLJumpStatementNode(sourceRange, Err.Err);
        }
    }
}