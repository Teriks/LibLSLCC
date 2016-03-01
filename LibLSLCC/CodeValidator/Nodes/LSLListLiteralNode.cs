#region FileInfo

// 
// File: LSLListLiteralNode.cs
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
    ///     Default <see cref="ILSLListLiteralNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLListLiteralNode : ILSLListLiteralNode, ILSLExprNode
    {
        private ILSLSyntaxTreeNode _parent;
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLListLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        ///     Create a <see cref="LSLListLiteralNode" /> with an empty <see cref="LSLExpressionListNode" /> as content.
        /// </summary>
        public LSLListLiteralNode() : this(new LSLExpressionListNode())
        {
        }


        /// <summary>
        ///     Create a <see cref="LSLListLiteralNode" /> with the given <see cref="LSLExpressionListNode" /> as content.
        /// </summary>
        /// <param name="expressionListNode">The expression list node.</param>
        /// <exception cref="ArgumentNullException"><paramref name="expressionListNode" /> is <c>null</c>.</exception>
        public LSLListLiteralNode(LSLExpressionListNode expressionListNode)
        {
            if (expressionListNode == null)
            {
                throw new ArgumentNullException("expressionListNode");
            }

            ExpressionListNode = expressionListNode;
            ExpressionListNode.Parent = this;

            SourceRangesAvailable = true;
        }


        /// <summary>
        ///     Create a <see cref="LSLListLiteralNode" /> with the given list of <see cref="ILSLExprNode"/> as content.
        /// </summary>
        /// <param name="expressionList">The <see cref="ILSLExprNode"/> list.</param>
        /// <exception cref="ArgumentNullException"><paramref name="expressionList" /> is <c>null</c>.</exception>
        public LSLListLiteralNode(params ILSLExprNode[] expressionList)
            : this(new LSLExpressionListNode(expressionList))
        {
        }


        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context" /> or <paramref name="expressionListNode" /> is
        ///     <c>null</c>.
        /// </exception>
        internal LSLListLiteralNode(LSLParser.ListLiteralContext context, LSLExpressionListNode expressionListNode)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (expressionListNode == null)
            {
                throw new ArgumentNullException("expressionListNode");
            }

            ExpressionListNode = expressionListNode;
            ExpressionListNode.Parent = this;

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeOpenBracket = new LSLSourceCodeRange(context.open_bracket);
            SourceRangeCloseBracket = new LSLSourceCodeRange(context.close_bracket);

            SourceRangesAvailable = true;
        }


        /// <summary>
        ///     Create an <see cref="LSLListLiteralNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        private LSLListLiteralNode(LSLListLiteralNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
                SourceRangeOpenBracket = other.SourceRangeOpenBracket;
                SourceRangeCloseBracket = other.SourceRangeCloseBracket;
            }

            ExpressionListNode = other.ExpressionListNode.Clone();
            ExpressionListNode.Parent = this;

            HasErrors = other.HasErrors;
        }


        /// <summary>
        ///     The expression list node that contains all of the expressions used to initialize the list literal.
        ///     This will never be null, even when the list literal was defined as being empty.
        /// </summary>
        public LSLExpressionListNode ExpressionListNode { get; private set; }

        /// <summary>
        ///     The source code range of the list literals opening bracket.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeOpenBracket { get; private set; }

        /// <summary>
        ///     The source code range of the list literals closing bracket.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeCloseBracket { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLExpressionListNode ILSLListLiteralNode.ExpressionList
        {
            get { return ExpressionListNode; }
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLListLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLListLiteralNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        /// <summary>
        ///     Deep clones the expression node.  It should clone the node and all of its children and cloneable properties, except
        ///     the parent.
        ///     When cloned, the parent node reference should be left <c>null</c>.
        /// </summary>
        /// <returns>A deep clone of this expression tree node.</returns>
        public LSLListLiteralNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLListLiteralNode(this);
        }


        ILSLExprNode ILSLExprNode.Clone()
        {
            return Clone();
        }


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

            return visitor.VisitListLiteral(this);
        }


        /// <summary>
        ///     The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type
        {
            get { return LSLType.List; }
        }


        /// <summary>
        ///     The expression type/classification of the expression. see: <see cref="LSLExpressionType" />
        /// </summary>
        /// <value>
        ///     The type of the expression.
        /// </value>
        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }

        /// <summary>
        ///     True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant
        {
            get { return ExpressionListNode != null && ExpressionListNode.AllExpressionsConstant; }
        }


        /// <summary>
        ///     True if the expression has some modifying effect on a local parameter or global/local variable;  or is a function
        ///     call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects
        {
            get { return ExpressionListNode != null && ExpressionListNode.HasExpressionWithPossibleSideEffects; }
        }


        /// <summary>
        ///     Should produce a user friendly description of the expressions return type. <para/>
        ///     This is used in some syntax error messages, Ideally you should enclose your description in
        ///     parenthesis or something that will make it stand out in a string.
        /// </summary>
        /// <returns>A use friendly description of the node's <see cref="Type"/>.</returns>
        public string DescribeType()
        {
            return "(" + Type + (this.IsLiteral() ? " Literal)" : ")");
        }


        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }

        #endregion
    }
}