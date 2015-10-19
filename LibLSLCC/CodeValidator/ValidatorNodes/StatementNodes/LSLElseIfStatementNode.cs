#region FileInfo
// 
// File: LSLElseIfStatementNode.cs
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Parser;

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
            get { return Code == null ? new List<LSLConstantJumpDescription>() : Code.ConstantJumps ; }
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

        /// <summary>
        /// Determines if the condition controlling the branch is a constant expression.
        /// </summary>
        public bool IsConstantBranch
        {
            get { return ConditionExpression != null && ConditionExpression.IsConstant; }
        }

        #endregion

        #region ILSLReturnPathNode Members

        /// <summary>
        /// True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return Code != null && Code.HasReturnPath; }
        }

        #endregion

        /// <summary>
        /// The source code range of the 'if' keyword in the else-if statement.
        /// </summary>
        public LSLSourceCodeRange IfKeywordSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the 'else' keyword in the else-if statement.
        /// </summary>
        public LSLSourceCodeRange ElseKeywordSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the opening parenthesis in the else-if statement where the condition area starts.
        /// </summary>
        public LSLSourceCodeRange OpenParenthSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the closing parenthesis in the else-if statement where the condition area ends.
        /// </summary>
        public LSLSourceCodeRange CloseParenthSourceCodeRange { get; private set; }

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

        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of ILSLValidatorNodeVisitor
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitElseIfStatement(this);
        }

        #endregion
    }
}