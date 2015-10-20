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
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    /// <summary>
    /// Represents the different types of expression lists that an ILSLExpressionListNode can represent.
    /// </summary>
    public enum LSLExpressionListType
    {
        /// <summary>
        /// The expression list is a list literal initializer.
        /// </summary>
        ListInitializer,

        /// <summary>
        /// The expression list represents the parameters used to call a user defined function.
        /// </summary>
        UserFunctionCallParameters,

        /// <summary>
        /// The expression list represents the parameters used to call a library defined function.
        /// </summary>
        LibraryFunctionCallParameters,

        /// <summary>
        /// The expression list represents the expression list used in a for-loops afterthoughts clause.
        /// </summary>
        ForLoopAfterthoughts,

        /// <summary>
        /// The expression list represents the expression list used in a for-loops initialization clause.
        /// </summary>
        ForLoopInitExpressions
    }


    public class LSLExpressionListNode : ILSLExpressionListNode
    {
        private readonly List<LSLSourceCodeRange> _commaSourceCodeRanges = new List<LSLSourceCodeRange>();
        private readonly List<ILSLExprNode> _expressionNodes = new List<ILSLExprNode>();
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLExpressionListNode(LSLSourceCodeRange sourceRange, Err err)

            // ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLExpressionListNode(LSLParser.OptionalExpressionListContext parserContext,
            LSLExpressionListType listType)
        {
            ParserContext = parserContext;
            ListType = listType;
            SourceCodeRange = new LSLSourceCodeRange(parserContext);

            SourceCodeRangesAvailable = true;
        }

        internal LSLExpressionListNode(LSLParser.OptionalExpressionListContext parserContext,
            IEnumerable<ILSLExprNode> expressions,
            LSLExpressionListType listType)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }

            ListType = listType;

            foreach (var lslExprNode in expressions)
            {
                AddExpression(lslExprNode);
            }
            SourceCodeRange = new LSLSourceCodeRange(parserContext);

            SourceCodeRangesAvailable = true;
        }

        internal LSLParser.OptionalExpressionListContext ParserContext { get; set; }


        /// <summary>
        /// A list of expression nodes that belong to this expression list in order of appearance, or an empty list object.
        /// </summary>
        public IReadOnlyList<ILSLExprNode> ExpressionNodes
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


        IReadOnlyList<ILSLReadOnlyExprNode> ILSLExpressionListNode.ExpressionNodes
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
        /// True if this expression list node actually has expression children, False if it is empty.
        /// </summary>
        public bool HasExpressionNodes
        {
            get { return ExpressionNodes.Count > 0; }
        }

        /// <summary>
        /// The source code range for each comma separator that appears in the expression list in order, or an empty list object.
        /// </summary>
        public IReadOnlyList<LSLSourceCodeRange> CommaSourceCodeRanges
        {
            get { return _commaSourceCodeRanges; }
        }

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
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var r = new LSLExpressionListNode(ParserContext, ListType)
            {
                HasErrors = HasErrors,
                Parent = Parent,
                SourceCodeRangesAvailable = SourceCodeRangesAvailable
            };


            foreach (var expressionNode in _expressionNodes)
            {
                r.AddExpression(expressionNode);
            }

            return r;
        }

        /// <summary>
        /// Adds a new source code range for a comma encountered in an expression list.
        /// </summary>
        /// <param name="range">The source code range to add.</param>
        public void AddCommaRange(LSLSourceCodeRange range)
        {
            _commaSourceCodeRanges.Add(range);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceCodeRangesAvailable { get; private set; }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; set; }


        /// <summary>
        /// Accept a visit from an implementor of ILSLValidatorNodeVisitor
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

            throw new InvalidOperationException("Visit LSLExpressionList, unknown ListTypes");
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }

        #endregion
    }
}