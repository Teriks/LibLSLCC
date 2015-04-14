using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.LoopNodes
{
    public class LSLDoLoopNode : ILSLDoLoopNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLDoLoopNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLDoLoopNode(LSLParser.DoLoopContext context, LSLCodeScopeNode code, ILSLExprNode conditionExpression,
            bool isSingleBlockStatement)
        {
            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            if (conditionExpression == null)
            {
                throw new ArgumentNullException("conditionExpression");
            }

            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            Code = code;
            ConditionExpression = conditionExpression;
            code.Parent = this;
            conditionExpression.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.DoLoopContext ParserContext { get; private set; }


        public LSLCodeScopeNode Code { get; private set; }


        public ILSLCodeStatement ReturnPath { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        ILSLCodeScopeNode ILSLDoLoopNode.Code
        {
            get { return Code; }
        }

        public ILSLExprNode ConditionExpression { get; private set; }


        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public ulong ScopeId { get; set; }




        #region ILSLCodeStatement Members


        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public bool IsLastStatementInScope { get; set; }

        public LSLDeadCodeType DeadCodeType { get; set; }

        public bool IsDeadCode { get; set; }

        public int StatementIndex { get; set; }

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitDoLoop(this);
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
            LSLDoLoopNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLDoLoopNode(sourceRange, Err.Err);
        }
    }
}