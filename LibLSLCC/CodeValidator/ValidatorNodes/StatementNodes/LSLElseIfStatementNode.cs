using System;
using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLElseIfStatementNode : ILSLElseIfStatementNode, ILSLBranchStatementNode
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLElseIfStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLElseIfStatementNode(LSLParser.ElseIfStatementContext context, LSLCodeScopeNode code,
            ILSLExprNode conditionExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (conditionExpression == null)
            {
                throw new ArgumentNullException("conditionExpression");
            }

            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            ParserContext = context;

            Code = code;

            ConditionExpression = conditionExpression;

            Code.Parent = this;
            ConditionExpression.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        public IEnumerable<LSLConstantJumpDescription> ConstantJumps
        {
            get { return Code.ConstantJumps; }
        }


        internal LSLParser.ElseIfStatementContext ParserContext { get; private set; }

        public LSLCodeScopeNode Code { get; private set; }

        public ILSLExprNode ConditionExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLCodeScopeNode ILSLElseIfStatementNode.Code
        {
            get { return Code; }
        }

        ILSLReadOnlyExprNode ILSLElseIfStatementNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }




        #region ILSLBranchStatementNode Members


        public bool IsConstantBranch
        {
            get { return ConditionExpression.IsConstant; }
        }


        #endregion




        #region ILSLReturnPathNode Members


        public bool HasReturnPath
        {
            get { return Code.HasReturnPath; }
        }


        #endregion




        #region ILSLTreeNode Members


        public ILSLSyntaxTreeNode Parent { get; set; }

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitElseIfStatement(this);
        }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        public static
            LSLElseIfStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLElseIfStatementNode(sourceRange, Err.Err);
        }
    }
}