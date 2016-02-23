#region FileInfo
// 
// File: LSLVectorLiteralNode.cs
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
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    /// <summary>
    /// Default <see cref="ILSLVectorLiteralNode"/> implementation used by <see cref="LSLCodeValidator"/>
    /// </summary>
    public sealed class LSLVectorLiteralNode : ILSLVectorLiteralNode, ILSLExprNode
    {

        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLVectorLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or
        /// <paramref name="x"/> or
        /// <paramref name="y"/> or
        /// <paramref name="z"/> is <see langword="null" />.</exception>
        internal LSLVectorLiteralNode(LSLParser.VectorLiteralContext context, ILSLExprNode x, ILSLExprNode y, ILSLExprNode z)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }
            if (y == null)
            {
                throw new ArgumentNullException("y");
            }
            if (z == null)
            {
                throw new ArgumentNullException("z");
            }

            XExpression = x;
            YExpression = y;
            ZExpression = z;

            XExpression.Parent = this;
            YExpression.Parent = this;
            ZExpression.Parent = this;

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangeOpenBracket = new LSLSourceCodeRange(context.open_bracket);
            SourceRangeCommaOne = new LSLSourceCodeRange(context.comma_one);
            SourceRangeCommaTwo = new LSLSourceCodeRange(context.comma_two);
            SourceRangeCloseBracket = new LSLSourceCodeRange(context.close_bracket);

            SourceRangesAvailable = true;
        }


        /// <summary>
        /// Create an <see cref="LSLVectorLiteralNode"/> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null" />.</exception>
        public LSLVectorLiteralNode(LSLVectorLiteralNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            XExpression = other.XExpression.Clone();
            YExpression = other.YExpression.Clone();
            ZExpression = other.ZExpression.Clone();

            XExpression.Parent = this;
            YExpression.Parent = this;
            ZExpression.Parent = this;

            SourceRangesAvailable = other.SourceRangesAvailable;

            if (!SourceRangesAvailable) return;

            SourceRange = other.SourceRange.Clone();

            SourceRangeOpenBracket = other.SourceRangeOpenBracket.Clone();
            SourceRangeCommaOne = other.SourceRangeCommaOne.Clone();
            SourceRangeCommaTwo = other.SourceRangeCommaTwo.Clone();
            SourceRangeCloseBracket = other.SourceRangeCloseBracket.Clone();

            HasErrors = other.HasErrors;
            Parent = other.Parent;
        }


        /// <summary>
        /// The source code range of the opening '&lt;' bracket of the vector literal.
        /// </summary>
        public LSLSourceCodeRange SourceRangeOpenBracket { get; private set; }

        /// <summary>
        /// The source code range of the closing '&gt;' bracket of the vector literal.
        /// </summary>
        public LSLSourceCodeRange SourceRangeCloseBracket { get; private set; }

        /// <summary>
        /// The expression node used to initialize the X Axis Component of the vector literal.  
        /// This should never be null.
        /// </summary>
        public ILSLExprNode XExpression { get; private set; }


        /// <summary>
        /// The expression node used to initialize the Y Axis Component of the vector literal.  
        /// This should never be null.
        /// </summary>
        public ILSLExprNode YExpression { get; private set; }


        /// <summary>
        /// The expression node used to initialize the Z Axis Component of the vector literal.  
        /// This should never be null.
        /// </summary>
        public ILSLExprNode ZExpression { get; private set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLReadOnlyExprNode ILSLVectorLiteralNode.XExpression
        {
            get { return XExpression; }
        }

        ILSLReadOnlyExprNode ILSLVectorLiteralNode.YExpression
        {
            get { return YExpression; }
        }

        ILSLReadOnlyExprNode ILSLVectorLiteralNode.ZExpression
        {
            get { return ZExpression; }
        }

        /// <summary>
        /// Returns a version of this node type that represents its error state;  in case of a syntax error
        /// in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLVectorLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLVectorLiteralNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        /// <summary>
        /// Deep clones the expression node.  It should clone the node and all of its children and cloneable properties, except the parent.
        /// When cloned, the parent node reference should still point to the same node.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public ILSLExprNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLVectorLiteralNode(this);
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
        public LSLSourceCodeRange SourceRange { get; private set; }


        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        /// The source code range of the first component separator comma to appear in the vector literal.
        /// </summary>
        public LSLSourceCodeRange SourceRangeCommaOne { get; private set; }


        /// <summary>
        /// The source code range of the second component separator comma to appear in the vector literal.
        /// </summary>
        public LSLSourceCodeRange SourceRangeCommaTwo { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitVectorLiteral(this);
        }



        /// <summary>
        /// The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type
        {
            get { return LSLType.Vector; }
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
        public bool IsConstant
        {
            get
            {
                var precondition = XExpression != null && YExpression != null && ZExpression != null;
                return precondition && XExpression.IsConstant && YExpression.IsConstant &&
                       ZExpression.IsConstant;
            }
        }


        /// <summary>
        /// True if the expression statement has some modifying effect on a local parameter or global/local variable;  or is a function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects
        {
            get
            {
                var precondition = XExpression != null && YExpression != null && ZExpression != null;
                return precondition && (XExpression.HasPossibleSideEffects || YExpression.HasPossibleSideEffects || ZExpression.HasPossibleSideEffects);
            }
        }


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