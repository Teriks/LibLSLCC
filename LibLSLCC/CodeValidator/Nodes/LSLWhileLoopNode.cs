#region FileInfo

// 
// File: LSLWhileLoopNode.cs
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
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLWhileLoopNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLWhileLoopNode : ILSLWhileLoopNode, ILSLCodeStatement
    {
        private ILSLSyntaxTreeNode _parent;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLWhileLoopNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        ///     Construct an <see cref="LSLWhileLoopNode" /> with a <see cref="ParentScopeId" /> of zero, condition and code body.
        /// </summary>
        /// <param name="condition">The <see cref="ConditionExpression" />.</param>
        /// <param name="code">The <see cref="Code" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="condition" /> or <paramref name="code" /> is <c>null</c>.</exception>
        public LSLWhileLoopNode(ILSLExprNode condition, LSLCodeScopeNode code)
        {
            if (condition == null) throw new ArgumentNullException("condition");
            if (code == null) throw new ArgumentNullException("code");


            ConditionExpression = condition;
            ConditionExpression.Parent = this;

            Code = code;
            Code.Parent = this;
            Code.CodeScopeType = LSLCodeScopeType.WhileLoop;
        }


        /// <exception cref="ArgumentNullException">
        ///     <paramref name="code" /> or <paramref name="conditionExpression" /> is
        ///     <c>null</c>.
        /// </exception>
        internal LSLWhileLoopNode(LSLParser.WhileLoopContext context, ILSLExprNode conditionExpression,
            LSLCodeScopeNode code)
        {
            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            if (conditionExpression == null)
            {
                throw new ArgumentNullException("conditionExpression");
            }

            ConditionExpression = conditionExpression;
            ConditionExpression.Parent = this;

            Code = code;
            Code.Parent = this;
            Code.CodeScopeType = LSLCodeScopeType.WhileLoop;

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeWhileKeyword = new LSLSourceCodeRange(context.loop_keyword);
            SourceRangeOpenParenth = new LSLSourceCodeRange(context.open_parenth);
            SourceRangeCloseParenth = new LSLSourceCodeRange(context.close_parenth);

            SourceRangesAvailable = true;
        }


        /*
        /// <summary>
        ///     Create an <see cref="LSLWhileLoopNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        private LSLWhileLoopNode(LSLWhileLoopNode other)
        {
            if (other == null) throw new ArgumentNullException("other");

            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRangeCloseParenth = other.SourceRangeCloseParenth;
                SourceRangeOpenParenth = other.SourceRangeOpenParenth;
                SourceRangeWhileKeyword = other.SourceRangeWhileKeyword;
                SourceRange = other.SourceRange;
            }


            ConditionExpression = other.ConditionExpression.Clone();
            ConditionExpression.Parent = this;

            Code = other.Code.Clone();
            Code.Parent = this;

            LSLStatementNodeTools.CopyStatement(this, other);

            HasErrors = other.HasErrors;
        }*/


        /// <summary>
        ///     The condition expression that controls the loop.
        /// </summary>
        public ILSLExprNode ConditionExpression { get; private set; }

        /// <summary>
        ///     The code scope node that represents the code scope of the loop body.
        /// </summary>
        public LSLCodeScopeNode Code { get; private set; }

        /// <summary>
        ///     True if this statement belongs to a single statement code scope.
        ///     A single statement code scope is a braceless code scope that can be used in control or loop statements.
        /// </summary>
        /// <seealso cref="ILSLCodeScopeNode.IsSingleStatementScope" />
        public bool InsideSingleStatementScope { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLCodeScopeNode ILSLLoopNode.Code
        {
            get { return Code; }
        }

        ILSLReadOnlyExprNode ILSLLoopNode.ConditionExpression
        {
            get { return ConditionExpression; }
        }

        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; set; }

        /// <summary>
        ///     Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        ///     this is not the scopes level.
        /// </summary>
        public int ParentScopeId { get; set; }

        /// <summary>
        ///     True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return false; }
        }

        /// <summary>
        ///     The index of this statement in its scope
        /// </summary>
        public int StatementIndex { get; set; }

        /// <summary>
        ///     Is this statement the last statement in its scope
        /// </summary>
        public bool IsLastStatementInScope { get; set; }

        /// <summary>
        ///     Is this statement dead code
        /// </summary>
        public bool IsDeadCode { get; set; }

        /// <summary>
        ///     The source code range of the 'while' keyword in the while loop statement.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeWhileKeyword { get; private set; }

        /// <summary>
        ///     The source code range of the opening parenthesis starting the condition area of a while loop statement.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeOpenParenth { get; private set; }

        /// <summary>
        ///     The source code range of the closing parenthesis ending the condition area of a while loop statement.
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
            LSLWhileLoopNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLWhileLoopNode(sourceRange, Err.Err);
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

            return visitor.VisitWhileLoop(this);
        }

        #endregion
    }
}