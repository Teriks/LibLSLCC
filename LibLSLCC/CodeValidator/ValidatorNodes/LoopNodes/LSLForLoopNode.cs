#region FileInfo

// 
// File: LSLForLoopNode.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:24 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Imports

using System;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.LoopNodes
{
    public class LSLForLoopNode : ILSLForLoopNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLForLoopNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLForLoopNode(LSLParser.ForLoopContext context, ILSLExpressionListNode initExpression,
            ILSLExprNode conditionExpression,
            LSLExpressionListNode afterthoughExpressions, LSLCodeScopeNode code, bool isSingleBlockStatement)
        {
            if (afterthoughExpressions == null)
            {
                throw new ArgumentNullException("afterthoughExpressions");
            }


            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            InitExpressionsList = initExpression;
            ConditionExpression = conditionExpression;
            AfterthoughExpressions = afterthoughExpressions;
            Code = code;

            SourceCodeRange = new LSLSourceCodeRange(context);
            FirstSemiColonSourceCodeRange = new LSLSourceCodeRange(context.first_semi_colon);
            SecondSemiColonSourceCodeRange = new LSLSourceCodeRange(context.second_semi_colon);
            OpenParenthSourceCodeRange = new LSLSourceCodeRange(context.open_parenth);
            CloseParenthSourceCodeRange = new LSLSourceCodeRange(context.close_parenth);
            ForKeywordSourceCodeRange = new LSLSourceCodeRange(context.loop_keyword);

            afterthoughExpressions.Parent = this;


            if (code != null)
            {
                code.Parent = this;
            }

            if (conditionExpression != null)
            {
                conditionExpression.Parent = this;
            }


            if (initExpression != null)
            {
                initExpression.Parent = this;
            }
        }

        internal LSLParser.ForLoopContext ParserContext { get; private set; }
        public ILSLExpressionListNode InitExpressionsList { get; }
        public ILSLExprNode ConditionExpression { get; }
        public LSLExpressionListNode AfterthoughExpressions { get; }
        public LSLCodeScopeNode Code { get; }
        public ILSLCodeStatement ReturnPath { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLReadOnlyExprNode ILSLLoopNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }

        ILSLExpressionListNode ILSLForLoopNode.AfterthoughExpressions
        {
            get { return AfterthoughExpressions; }
        }

        ILSLCodeScopeNode ILSLLoopNode.Code
        {
            get { return Code; }
        }

        ILSLExpressionListNode ILSLForLoopNode.InitExpressionsList
        {
            get { return InitExpressionsList; }
        }

        public bool HasInitExpressions
        {
            get { return InitExpressionsList != null && InitExpressionsList.HasExpressionNodes; }
        }

        public bool HasConditionExpression
        {
            get { return ConditionExpression != null; }
        }

        public bool HasAfterthoughtExpressions
        {
            get { return AfterthoughExpressions != null && AfterthoughExpressions.HasExpressionNodes; }
        }

        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public ulong ScopeId { get; set; }

        public static
            LSLForLoopNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLForLoopNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLCodeStatement Members

        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }

        public int StatementIndex { get; set; }

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }

        public LSLSourceCodeRange ForKeywordSourceCodeRange { get; }

        public LSLSourceCodeRange FirstSemiColonSourceCodeRange { get; }


        public LSLSourceCodeRange SecondSemiColonSourceCodeRange { get; }


        public LSLSourceCodeRange OpenParenthSourceCodeRange { get; }


        public LSLSourceCodeRange CloseParenthSourceCodeRange { get; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitForLoop(this);
        }


        public bool HasReturnPath
        {
            get { return false; }
        }

        #endregion
    }
}