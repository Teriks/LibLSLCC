using System;
using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLElseStatementNode : ILSLElseStatementNode, ILSLBranchStatementNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLElseStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLElseStatementNode(LSLParser.ElseStatementContext context, LSLCodeScopeNode code,
            bool isConstantBranch)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (code == null)
            {
                throw new ArgumentNullException("code");
            }


            ParserContext = context;
            IsConstantBranch = isConstantBranch;
            Code = code;

            code.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        public IEnumerable<LSLConstantJumpDescription> ConstantJumps
        {
            get { return Code.ConstantJumps; }
        }


        public LSLCodeScopeNode Code { get; private set; }


        internal LSLParser.ElseStatementContext ParserContext { get; private set; }




        #region ILSLBranchStatementNode Members


        public bool IsConstantBranch { get; private set; }


        #endregion




        #region ILSLReturnPathNode Members


        public bool HasReturnPath
        {
            get { return Code.HasReturnPath; }
        }


        #endregion




        #region ILSLTreeNode Members


        public ILSLSyntaxTreeNode Parent { get; set; }
        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        public bool HasErrors { get; set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitElseStatement(this);
        }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLCodeScopeNode ILSLElseStatementNode.Code
        {
            get { return Code; }
        }



        public static
            LSLElseStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLElseStatementNode(sourceRange, Err.Err);
        }
    }
}