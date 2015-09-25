#region FileInfo
// 
// 
// File: LSLElseIfStatementNode.cs
// 
// Last Compile: 25/09/2015 @ 5:46 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// ============================================================
// ============================================================
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// Copyright (c) 2015, Teriks
// All rights reserved.
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLElseIfStatementNode : ILSLElseIfStatementNode, ILSLBranchStatementNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
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

            IfKeywordSourceCodeRange = new LSLSourceCodeRange(context.if_keyword);
            ElseKeywordSourceCodeRange = new LSLSourceCodeRange(context.else_keyword);
            OpenParenthSourceCodeRange = new LSLSourceCodeRange(context.open_parenth);
            CloseParenthSourceCodeRange = new LSLSourceCodeRange(context.close_parenth);
        }

        public IEnumerable<LSLConstantJumpDescription> ConstantJumps
        {
            get { return Code.ConstantJumps; }
        }

        internal LSLParser.ElseIfStatementContext ParserContext { get; private set; }
        public LSLCodeScopeNode Code { get; }
        public ILSLExprNode ConditionExpression { get; }

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

        public LSLSourceCodeRange IfKeywordSourceCodeRange { get; }
        public LSLSourceCodeRange ElseKeywordSourceCodeRange { get; }
        public LSLSourceCodeRange OpenParenthSourceCodeRange { get; }
        public LSLSourceCodeRange CloseParenthSourceCodeRange { get; }

        public static
            LSLElseIfStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLElseIfStatementNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        public ILSLSyntaxTreeNode Parent { get; set; }

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitElseIfStatement(this);
        }

        #endregion
    }
}