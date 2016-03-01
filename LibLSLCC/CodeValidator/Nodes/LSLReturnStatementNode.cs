#region FileInfo

// 
// File: LSLReturnStatementNode.cs
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
    ///     Default <see cref="ILSLReturnStatementNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLReturnStatementNode : ILSLReturnStatementNode, ILSLCodeStatement
    {
        private ILSLSyntaxTreeNode _parent;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLReturnStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        ///     Construct an <see cref="LSLReturnStatementNode" /> with a given return expression and <see cref="ParentScopeId" /> of
        ///     zero.
        /// </summary>
        /// <param name="returnExpression">The <see cref="ReturnExpression" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="returnExpression" /> is <c>null</c>.</exception>
        public LSLReturnStatementNode(ILSLExprNode returnExpression)
        {
            if (returnExpression == null)
            {
                throw new ArgumentNullException("returnExpression");
            }

            ReturnExpression = returnExpression;
            ReturnExpression.Parent = this;
        }


        /// <summary>
        ///     Construct an <see cref="LSLReturnStatementNode" /> with a <see cref="ParentScopeId" /> of zero and no return expression.
        /// </summary>
        public LSLReturnStatementNode()
        {
        }


        /// <summary>
        ///     Create an <see cref="LSLReturnStatementNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        private LSLReturnStatementNode(LSLReturnStatementNode other)
        {
            if (other == null) throw new ArgumentNullException("other");


            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
                SourceRangeReturnKeyword = other.SourceRangeReturnKeyword;
                SourceRangeSemicolon = other.SourceRangeSemicolon;
            }

            if (HasReturnExpression)
            {
                ReturnExpression = other.ReturnExpression.Clone();
                ReturnExpression.Parent = this;
            }


            LSLStatementNodeTools.CopyStatement(this, other);

            HasErrors = other.HasErrors;
        }


        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context" /> or <paramref name="returnExpression" /> is
        ///     <c>null</c>.
        /// </exception>
        internal LSLReturnStatementNode(LSLParser.ReturnStatementContext context, ILSLExprNode returnExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (returnExpression == null)
            {
                throw new ArgumentNullException("returnExpression");
            }

            ReturnExpression = returnExpression;
            ReturnExpression.Parent = this;

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeReturnKeyword = new LSLSourceCodeRange(context.return_keyword);
            SourceRangeSemicolon = new LSLSourceCodeRange(context.semi_colon);

            SourceRangesAvailable = true;
        }


        /// <exception cref="ArgumentNullException"><paramref name="context" /> is <c>null</c>.</exception>
        internal LSLReturnStatementNode(LSLParser.ReturnStatementContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ReturnExpression = null;

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeReturnKeyword = new LSLSourceCodeRange(context.return_keyword);
            SourceRangeSemicolon = new LSLSourceCodeRange(context.semi_colon);

            SourceRangesAvailable = true;
        }


        /// <summary>
        ///     The expression node that represents the returned expression, or null if no expression was used with the return
        ///     statement.
        /// </summary>
        public ILSLExprNode ReturnExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlyExprNode ILSLReturnStatementNode.ReturnExpression
        {
            get { return ReturnExpression; }
        }

        /// <summary>
        ///     True if the return statement uses a return expression.
        /// </summary>
        public bool HasReturnExpression
        {
            get { return ReturnExpression != null; }
        }

        /// <summary>
        ///     Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        ///     this is not the scopes level.
        /// </summary>
        public int ParentScopeId { get; set; }

        /// <summary>
        ///     The source code range of the 'return' keyword in the return statement.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeReturnKeyword { get; private set; }

        /// <summary>
        ///     The source code range of the semi-colon that ends the return statement.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeSemicolon { get; private set; }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLReturnStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLReturnStatementNode(sourceRange, Err.Err);
        }


        private enum Err
        {
            Err
        }

        #region ILSLCodeStatement Members

        /// <summary>
        ///     True if this statement belongs to a single statement code scope.
        ///     A single statement code scope is a braceless code scope that can be used in control or loop statements.
        /// </summary>
        /// <seealso cref="ILSLCodeScopeNode.IsSingleStatementScope" />
        public bool InsideSingleStatementScope { get; set; }


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
        public bool HasErrors { get; private set; }


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

            return visitor.VisitReturnStatement(this);
        }


        /// <summary>
        ///     Deep clones the node.  It should clone the node and all of its children and cloneable properties, except the
        ///     parent.
        ///     When cloned, the parent node reference should be left <c>null</c>.
        /// </summary>
        /// <returns>A deep clone of this statement tree node.</returns>
        public LSLReturnStatementNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLReturnStatementNode(this);
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
        ///     True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return true; }
        }

        #endregion

        #region Nested type: Err

        #endregion
    }
}