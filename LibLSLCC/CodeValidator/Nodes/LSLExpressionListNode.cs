#region FileInfo

// 
// File: LSLExpressionListNode.cs
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
using System.Linq;
using LibLSLCC.Collections;
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLExpressionListNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLExpressionListNode : ILSLExpressionListNode
    {
        private readonly GenericArray<ILSLExprNode> _expressions = new GenericArray<ILSLExprNode>();
        private readonly GenericArray<LSLSourceCodeRange> _sourceRangeCommaList = new GenericArray<LSLSourceCodeRange>();
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLExpressionListNode(LSLSourceCodeRange sourceRange, Err err)

            // ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        /// Create an empty <see cref="LSLExpressionListNode"/>.
        /// </summary>
        public LSLExpressionListNode()
        {
            
        }


        /// <summary>
        /// Create an <see cref="LSLExpressionListNode"/> with the given expressions.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="expressions"/> is <c>null</c>.</exception>
        public LSLExpressionListNode(IEnumerable<ILSLExprNode> expressions)
        {
            if(expressions == null) throw new ArgumentNullException("expressions");

            foreach (var expression in expressions)
            {
                AddExpression(expression);
            }
        }


        /// <summary>
        /// Create an <see cref="LSLExpressionListNode"/> with the given expressions.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="expressions"/> is <c>null</c>.</exception>
        public LSLExpressionListNode(params ILSLExprNode[] expressions)
        {
            if (expressions == null) throw new ArgumentNullException("expressions");

            foreach (var expression in expressions)
            {
                AddExpression(expression);
            }
        }


        /// <summary>
        ///     Create an <see cref="LSLExpressionListNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLExpressionListNode(LSLExpressionListNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            ListType = other.ListType;


            foreach (var lslExprNode in other.Expressions)
            {
                var node = lslExprNode.Clone();

                node.Parent = this;

                AddExpression(node);
            }

            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;

                foreach (var commaRange in other.SourceRangeCommaList)
                {
                    _sourceRangeCommaList.Add(commaRange);
                }
            }

            HasErrors = other.HasErrors;
            Parent = other.Parent;
        }


        internal LSLExpressionListNode(LSLParser.OptionalExpressionListContext parserContext,
            LSLExpressionListType listType)
        {
            ListType = listType;
            SourceRange = new LSLSourceCodeRange(parserContext);

            SourceRangesAvailable = true;
        }


        /// <summary>
        ///     A list of expression nodes that belong to this expression list in order of appearance, or an empty list object.
        /// </summary>
        public IReadOnlyGenericArray<ILSLExprNode> Expressions
        {
            get { return _expressions; }
        }

        /// <summary>
        ///     True if all expressions in the expression list are considered to be constant expressions.
        /// </summary>
        public bool AllExpressionsConstant
        {
            get { return Expressions.Count == 0 || Expressions.All(lslExprNode => lslExprNode.IsConstant); }
        }

        /// <summary>
        ///     True if any expression in the expression list can possibly have side effects on the state of the program.
        /// </summary>
        public bool HasExpressionWithPossibleSideEffects
        {
            get { return Expressions.Any(lslExprNode => lslExprNode.HasPossibleSideEffects); }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     The type of expression list this node represents.
        ///     <see cref="LSLExpressionListType" />
        /// </summary>
        public LSLExpressionListType ListType { get; private set; }

        IReadOnlyGenericArray<ILSLReadOnlyExprNode> ILSLExpressionListNode.Expressions
        {
            get { return _expressions; }
        }

        /// <summary>
        ///     True if this expression list node actually has expression children, False if it is empty.
        /// </summary>
        public bool HasExpressions
        {
            get { return Expressions.Count > 0; }
        }

        /// <summary>
        ///     The source code range for each comma separator that appears in the expression list in order, or an empty list
        ///     object.
        /// </summary>
        public IReadOnlyGenericArray<LSLSourceCodeRange> SourceRangeCommaList
        {
            get { return _sourceRangeCommaList; }
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static LSLExpressionListNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLExpressionListNode(sourceRange, Err.Err);
        }


        /// <summary>
        ///     Adds a new expression to the expression list node.
        /// </summary>
        /// <param name="node">The expression node to add to the expression list.</param>
        /// <exception cref="ArgumentNullException">Thrown if the 'node' parameter is <c>null</c>.</exception>
        public void AddExpression(ILSLExprNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Parent = this;

            _expressions.Add(node);
        }


        /// <summary>
        ///     Deep clones the expression list node and all of its child expressions.
        /// </summary>
        /// <returns>A deep cloned copy of this expression list node.</returns>
        public LSLExpressionListNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLExpressionListNode(this);
        }


        /// <summary>
        ///     Add an <see cref="LSLSourceCodeRange" /> for a comma in the expression list.
        ///     This method does NOT clone <paramref name="sourceRange" /> for you.
        /// </summary>
        /// <param name="sourceRange"></param>
        internal void AddCommaSourceRange(LSLSourceCodeRange sourceRange)
        {
            _sourceRangeCommaList.Add(sourceRange);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

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
        ///     True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (ListType == LSLExpressionListType.ForLoopAfterthoughts)
            {
                return visitor.VisitForLoopAfterthoughts(this);
            }

            if (ListType == LSLExpressionListType.LibraryFunctionCallParameters)
            {
                return visitor.VisitLibraryFunctionCallParameters(this);
            }

            if (ListType == LSLExpressionListType.UserFunctionCallParameters)
            {
                return visitor.VisitUserFunctionCallParameters(this);
            }

            if (ListType == LSLExpressionListType.ListInitializer)
            {
                return visitor.VisitListLiteralInitializerList(this);
            }

            if (ListType == LSLExpressionListType.ForLoopInitExpressions)
            {
                return visitor.VisitForLoopInitExpressions(this);
            }

            throw new InvalidOperationException("Visit " + typeof (LSLExpressionListNode) +
                                                ", unknown LSLExpressionListType.");
        }


        /// <summary>
        ///     The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}