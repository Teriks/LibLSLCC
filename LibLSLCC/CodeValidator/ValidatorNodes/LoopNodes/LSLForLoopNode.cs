#region FileInfo
// 
// File: LSLForLoopNode.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
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