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
using Antlr4.Runtime.Tree;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Collections;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    /// <summary>
    /// Default <see cref="ILSLExpressionListNode"/> implementation used by <see cref="LSLCodeValidator"/>
    /// </summary>
    public sealed class LSLExpressionListNode : ILSLExpressionListNode
    {
        private readonly GenericArray<LSLSourceCodeRange> _sourceRangeCommaList = new GenericArray<LSLSourceCodeRange>();
        private readonly GenericArray<ILSLExprNode> _expressionNodes = new GenericArray<ILSLExprNode>();
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLExpressionListNode(LSLSourceCodeRange sourceRange, Err err)

            // ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }




        /// <summary>
        /// Create an <see cref="LSLExpressionListNode"/> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        public LSLExpressionListNode(LSLExpressionListNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            ListType = other.ListType;


            foreach (var lslExprNode in other.ExpressionNodes)
            {
                var node = lslExprNode.Clone();

                node.Parent = this;

                AddExpression(node);
            }

            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange.Clone();

                foreach (var commaRange in other.SourceRangeCommaList)
                {
                    _sourceRangeCommaList.Add(commaRange.Clone());
                }
            }

            HasErrors = other.HasErrors;
            Parent = other.Parent;
        }


        internal LSLExpressionListNode(LSLParser.OptionalExpressionListContext parserContext, LSLExpressionListType listType)
        {
            ListType = listType;
            SourceRange = new LSLSourceCodeRange(parserContext);

            SourceRangesAvailable = true;
        }


        /// <summary>
        /// A list of expression nodes that belong to this expression list in order of appearance, or an empty list object.
        /// </summary>
        public IReadOnlyGenericArray<ILSLExprNode> ExpressionNodes
        {
            get { return _expressionNodes; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        /// The type of expression list this node represents.
        /// <see cref="LSLExpressionListType"/>
        /// </summary>
        public LSLExpressionListType ListType { get; private set; }


        IReadOnlyGenericArray<ILSLReadOnlyExprNode> ILSLExpressionListNode.ExpressionNodes
        {
            get { return _expressionNodes; }
        }

        /// <summary>
        /// True if all expressions in the expression list are considered to be constant expressions.
        /// </summary>
        public bool AllExpressionsConstant
        {
            get { return ExpressionNodes.Count == 0 || ExpressionNodes.All(lslExprNode => lslExprNode.IsConstant); }
        }

        /// <summary>
        /// True if any expressions in the expression list are considered to possibly have side effects on the state of the program.
        /// </summary>
        public bool AnyExpressionHasPossibleSideEffects
        {
            get { return ExpressionNodes.Any(lslExprNode => lslExprNode.HasPossibleSideEffects); }
        }

        /// <summary>
        /// True if this expression list node actually has expression children, False if it is empty.
        /// </summary>
        public bool HasExpressionNodes
        {
            get { return ExpressionNodes.Count > 0; }
        }

        /// <summary>
        /// The source code range for each comma separator that appears in the expression list in order, or an empty list object.
        /// </summary>
        public IReadOnlyGenericArray<LSLSourceCodeRange> SourceRangeCommaList
        {
            get { return _sourceRangeCommaList; }
        }


        /// <summary>
        /// Returns a version of this node type that represents its error state;  in case of a syntax error
        /// in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static LSLExpressionListNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLExpressionListNode(sourceRange, Err.Err);
        }

        /// <summary>
        /// Adds a new expression to the expression list node.
        /// </summary>
        /// <param name="node">The expression node to add to the expression list.</param>
        /// <exception cref="ArgumentNullException">Thrown if the 'node' parameter is null.</exception>
        public void AddExpression(ILSLExprNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Parent = this;

            _expressionNodes.Add(node);
        }

        /// <summary>
        /// Deep clones the expression list node and all of its child expressions.
        /// </summary>
        /// <returns>A deep cloned copy of this expression list node.</returns>
        public LSLExpressionListNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLExpressionListNode(this);
        }


        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceRange { get; private set; }



        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
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

            throw new InvalidOperationException("Visit "+typeof(LSLExpressionListNode)+", unknown LSLExpressionListType.");
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion

        /// <summary>
        /// Add an <see cref="LSLSourceCodeRange"/> for a comma in the expression list.
        /// This method does NOT clone <paramref name="sourceRange"/> for you.
        /// </summary>
        /// <param name="sourceRange"></param>
        internal void AddCommaSourceRange(LSLSourceCodeRange sourceRange)
        {
            _sourceRangeCommaList.Add(sourceRange);
        }
    }
}