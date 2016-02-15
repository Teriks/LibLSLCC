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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Collections;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    public sealed class LSLListLiteralNode : ILSLListLiteralNode, ILSLExprNode
    {

        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLListLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLListLiteralNode(LSLParser.ListLiteralContext context, LSLExpressionListNode expressionListNode)
        {
            if (expressionListNode == null)
            {
                throw new ArgumentNullException("expressionListNode");
            }

            HasPossibleSideEffects = expressionListNode.AnyExpressionHasPossibleSideEffects;
            IsConstant = expressionListNode.AllExpressionsConstant;

            ExpressionListNode = expressionListNode;
            ExpressionListNode.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);

            SourceCodeRangesAvailable = true;
        }


        public LSLListLiteralNode(LSLListLiteralNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            HasPossibleSideEffects = other.HasPossibleSideEffects;
            IsConstant = other.IsConstant;

            ExpressionListNode = other.ExpressionListNode.Clone();
            ExpressionListNode.Parent = this;

            SourceCodeRangesAvailable = other.SourceCodeRangesAvailable;

            if (SourceCodeRangesAvailable)
            {
                SourceCodeRange = other.SourceCodeRange.Clone();
            }

            HasErrors = other.HasErrors;
            Parent = other.Parent;
        }


        /// <summary>
        /// A list of expressions that were used to initialize the list literal, or an empty list.
        /// </summary>
        public IReadOnlyGenericArray<ILSLExprNode> ListEntryExpressions
        {
            get { return ExpressionListNode.ExpressionNodes; }
        }


        /// <summary>
        /// The expression list node that contains all of the expressions used to initialize the list literal.
        /// This will never be null, even when the list literal was defined as being empty.
        /// </summary>
        public LSLExpressionListNode ExpressionListNode { get; private set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        IReadOnlyGenericArray<ILSLReadOnlyExprNode> ILSLListLiteralNode.ListEntryExpressions
        {
            get { return ListEntryExpressions; }
        }

        ILSLExpressionListNode ILSLListLiteralNode.ExpressionListNode
        {
            get { return ExpressionListNode; }
        }

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
        /// Deep clones the expression node.  It should clone the node and also clone all of its children.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public ILSLExprNode Clone()
        {
            return HasErrors ? GetError(SourceCodeRange) : new LSLListLiteralNode(this);
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; internal set; }

        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceCodeRangesAvailable { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitListLiteral(this);
        }



        /// <summary>
        /// The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type
        {
            get { return LSLType.List; }
        }


        /// <summary>
        /// The expression type/classification of the expression. see: <see cref="LSLExpressionType" />
        /// </summary>
        /// <value>
        /// The type of the expression.
        /// </value>
        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }

        /// <summary>
        /// True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant { get; private set; }


        /// <summary>
        /// True if the expression has some modifying effect on a local parameter or global/local variable;  or is a function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects { get; private set; }


        /// <summary>
        /// Should produce a user friendly description of the expressions return type.
        /// This is used in some syntax error messages, Ideally you should enclose your description in
        /// parenthesis or something that will make it stand out in a string.
        /// </summary>
        /// <returns></returns>
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