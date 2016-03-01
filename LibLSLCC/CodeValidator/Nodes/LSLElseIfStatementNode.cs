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
using Antlr4.Runtime;
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLElseIfStatementNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLElseIfStatementNode : ILSLElseIfStatementNode, ILSLBranchStatementNode
    {
        private ILSLSyntaxTreeNode _parent;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLElseIfStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        ///     Construct an <see cref="LSLElseIfStatementNode" /> with the given condition expression and code.
        /// </summary>
        /// <param name="condition">The branch condition.</param>
        /// <param name="code">The code.</param>
        /// <exception cref="ArgumentNullException"><paramref name="condition" /> or <paramref name="code" /> is <c>null</c>.</exception>
        public LSLElseIfStatementNode(ILSLExprNode condition, LSLCodeScopeNode code)
        {
            if (condition == null) throw new ArgumentNullException("condition");
            if (code == null) throw new ArgumentNullException("code");

            ConditionExpression = condition;
            ConditionExpression.Parent = this;

            Code = code;
            Code.Parent = this;
            Code.CodeScopeType = LSLCodeScopeType.ElseIf;
        }


        /// <exception cref="ArgumentNullException">
        ///     <paramref name="code" /> or <paramref name="conditionExpression" /> is
        ///     <c>null</c>.
        /// </exception>
        internal LSLElseIfStatementNode(
            IToken elseKeyword,
            LSLParser.ControlStructureContext context,
            LSLCodeScopeNode code,
            ILSLExprNode conditionExpression)
        {
            if (conditionExpression == null)
            {
                throw new ArgumentNullException("conditionExpression");
            }

            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            Code = code;
            Code.Parent = this;
            Code.CodeScopeType = LSLCodeScopeType.ElseIf;

            ConditionExpression = conditionExpression;
            ConditionExpression.Parent = this;

            SourceRangeElseKeyword = new LSLSourceCodeRange(elseKeyword);
            SourceRangeIfKeyword = new LSLSourceCodeRange(context.if_keyword);
            SourceRangeOpenParenth = new LSLSourceCodeRange(context.open_parenth);
            SourceRangeCloseParenth = new LSLSourceCodeRange(context.close_parenth);


            SourceRange = new LSLSourceCodeRange(SourceRangeElseKeyword, code.SourceRange);

            SourceRangesAvailable = true;
        }


        /*
        /// <summary>
        ///     Create an <see cref="LSLElseIfStatementNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        private LSLElseIfStatementNode(LSLElseIfStatementNode other)
        {
            if (other == null) throw new ArgumentNullException("other");


            SourceRangesAvailable = other.SourceRangesAvailable;
            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
                SourceRangeCloseParenth = other.SourceRangeCloseParenth;
                SourceRangeElseKeyword = other.SourceRangeElseKeyword;
                SourceRangeIfKeyword = other.SourceRangeIfKeyword;
                SourceRangeOpenParenth = other.SourceRangeOpenParenth;
            }

            ConditionExpression = other.ConditionExpression.Clone();
            ConditionExpression.Parent = this;

            Code = other.Code.Clone();
            Code.Parent = this;

            HasErrors = other.HasErrors;
        }*/


        /// <summary>
        ///     <see cref="LSLCodeScopeNode.ConstantJumps" /> returned from <see cref="Code" />
        /// </summary>
        public IEnumerable<LSLConstantJumpDescription> ConstantJumps
        {
            get { return Code == null ? new List<LSLConstantJumpDescription>() : Code.ConstantJumps; }
        }

        /// <summary>
        ///     The code scope associated with the else-if branch.
        /// </summary>
        public LSLCodeScopeNode Code { get; private set; }

        /// <summary>
        ///     The condition expression of the else-if statement.
        /// </summary>
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
        ///     Determines if the condition controlling the branch is a constant expression.
        /// </summary>
        public bool IsConstantBranch
        {
            get { return ConditionExpression != null && ConditionExpression.IsConstant; }
        }

        #endregion

        #region ILSLReturnPathNode Members

        /// <summary>
        ///     True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return Code != null && Code.HasReturnPath; }
        }

        #endregion

        /// <summary>
        ///     The source code range of the 'if' keyword in the else-if statement.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeIfKeyword { get; private set; }

        /// <summary>
        ///     The source code range of the 'else' keyword in the else-if statement.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeElseKeyword { get; private set; }

        /// <summary>
        ///     The source code range of the opening parenthesis in the else-if statement where the condition area starts.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeOpenParenth { get; private set; }

        /// <summary>
        ///     The source code range of the closing parenthesis in the else-if statement where the condition area ends.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeCloseParenth { get; private set; }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLElseIfStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLElseIfStatementNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        /// <summary>
        ///     The parent node of this syntax tree node.
        /// </summary>
        /// <exception cref="InvalidOperationException" accessor="set">If Parent has already been set.</exception>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <c>null</c>.</exception>
        public ILSLSyntaxTreeNode Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                {
                    throw new InvalidOperationException(GetType().Name +
                                                        ": Parent node already set, it can only be set once.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value", GetType().Name + ": Parent cannot be set to null.");
                }

                _parent = value;
            }
        }


        /// <summary>
        ///     True if this syntax tree node contains syntax errors. <para/>
        ///     <see cref="SourceRange"/> should point to a more specific error location when this is <c>true</c>. <para/>
        ///     Other source ranges will not be available.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        ///     The source code range that this syntax tree node occupies.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRange { get; private set; }


        /// <summary>
        ///     Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <c>null</c>.</exception>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            return visitor.VisitElseIfStatement(this);
        }

        #endregion
    }
}