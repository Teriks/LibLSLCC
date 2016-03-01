#region FileInfo

// 
// File: LSLRotationLiteralNode.cs
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
    ///     Default <see cref="ILSLRotationLiteralNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLRotationLiteralNode : ILSLRotationLiteralNode, ILSLExprNode
    {
        private ILSLSyntaxTreeNode _parent;
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLRotationLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        ///     Construct an <see cref="LSLRotationLiteralNode" /> with the given component expressions.
        /// </summary>
        /// <param name="x">The 'x' rotation component expression.</param>
        /// <param name="y">The 'y' rotation component expression.</param>
        /// <param name="z">The 'z' rotation component expression.</param>
        /// <param name="s">The 's' rotation component expression.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="x" /> or
        ///     <paramref name="y" /> or
        ///     <paramref name="z" /> or
        ///     <paramref name="s" /> is <c>null</c>.
        /// </exception>
        public LSLRotationLiteralNode(ILSLExprNode x, ILSLExprNode y,
            ILSLExprNode z, ILSLExprNode s)
        {
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
            if (s == null)
            {
                throw new ArgumentNullException("z");
            }

            XExpression = x;
            YExpression = y;
            ZExpression = z;
            SExpression = s;

            XExpression.Parent = this;
            YExpression.Parent = this;
            ZExpression.Parent = this;
            SExpression.Parent = this;
        }


        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context" /> or
        ///     <paramref name="x" /> or
        ///     <paramref name="y" /> or
        ///     <paramref name="z" /> or
        ///     <paramref name="s" /> is <c>null</c>.
        /// </exception>
        internal LSLRotationLiteralNode(LSLParser.RotationLiteralContext context, ILSLExprNode x, ILSLExprNode y,
            ILSLExprNode z, ILSLExprNode s) : this(x, y, z, s)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangeOpenBracket = new LSLSourceCodeRange(context.open_bracket);
            SourceRangeCommaOne = new LSLSourceCodeRange(context.comma_one);
            SourceRangeCommaTwo = new LSLSourceCodeRange(context.comma_two);
            SourceRangeCommaThree = new LSLSourceCodeRange(context.comma_three);
            SourceRangeCloseBracket = new LSLSourceCodeRange(context.close_bracket);

            SourceRangesAvailable = true;
        }


        /// <summary>
        ///     Create an <see cref="LSLRotationLiteralNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        private LSLRotationLiteralNode(LSLRotationLiteralNode other)
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
                SourceRangeCommaOne = other.SourceRangeCommaOne;
                SourceRangeCommaTwo = other.SourceRangeCommaTwo;
                SourceRangeCommaThree = other.SourceRangeCommaThree;
                SourceRangeCloseBracket = other.SourceRangeCloseBracket;
            }

            XExpression = other.XExpression.Clone();
            YExpression = other.YExpression.Clone();
            ZExpression = other.ZExpression.Clone();
            SExpression = other.SExpression.Clone();

            XExpression.Parent = this;
            YExpression.Parent = this;
            ZExpression.Parent = this;
            SExpression.Parent = this;

            HasErrors = other.HasErrors;
        }


        /// <summary>
        ///     The expression node used to initialize the X (first) Component of the rotation literal.
        ///     This should never be null.
        /// </summary>
        public ILSLExprNode XExpression { get; private set; }

        /// <summary>
        ///     The expression node used to initialize the Y (second) Component of the rotation literal.
        ///     This should never be null.
        /// </summary>
        public ILSLExprNode YExpression { get; private set; }

        /// <summary>
        ///     The expression node used to initialize the Z (third) Component of the rotation literal.
        ///     This should never be null.
        /// </summary>
        public ILSLExprNode ZExpression { get; private set; }

        /// <summary>
        ///     The expression node used to initialize the S (fourth) Component of the rotation literal.
        ///     This should never be null.
        /// </summary>
        public ILSLExprNode SExpression { get; private set; }

        /// <summary>
        ///     The source code range of the opening '&lt;' bracket of the rotation literal.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeOpenBracket { get; private set; }

        /// <summary>
        ///     The source code range of the closing '&gt;' bracket of the rotation literal.
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

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.XExpression
        {
            get { return XExpression; }
        }

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.YExpression
        {
            get { return YExpression; }
        }

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.ZExpression
        {
            get { return ZExpression; }
        }

        ILSLReadOnlyExprNode ILSLRotationLiteralNode.SExpression
        {
            get { return SExpression; }
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLRotationLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLRotationLiteralNode(sourceRange, Err.Err);
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
        public LSLRotationLiteralNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLRotationLiteralNode(this);
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
        ///     The source code range of the first component separator comma to appear in the rotation literal.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeCommaOne { get; private set; }

        /// <summary>
        ///     The source code range of the second component separator comma to appear in the rotation literal.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeCommaTwo { get; private set; }


        /// <summary>
        ///     The source code range of the third component separator comma to appear in the rotation literal.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeCommaThree { get; private set; }


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

            return visitor.VisitRotationLiteral(this);
        }


        /// <summary>
        ///     The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type
        {
            get { return LSLType.Rotation; }
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
            get
            {
                var precondition =
                    XExpression != null &&
                    YExpression != null &&
                    ZExpression != null &&
                    SExpression != null;

                return precondition &&
                       XExpression.IsConstant &&
                       YExpression.IsConstant &&
                       ZExpression.IsConstant &&
                       SExpression.IsConstant;
            }
        }

        /// <summary>
        ///     True if the expression statement has some modifying effect on a local parameter or global/local variable;  or is a
        ///     function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects
        {
            get
            {
                var precondition = XExpression != null && YExpression != null && ZExpression != null &&
                                   SExpression != null;
                return precondition &&
                       (XExpression.HasPossibleSideEffects || YExpression.HasPossibleSideEffects ||
                        ZExpression.HasPossibleSideEffects || SExpression.HasPossibleSideEffects);
            }
        }


        /// <summary>
        ///     Should produce a user friendly description of the expressions return type. <para/>
        ///     This is used in some syntax error messages, Ideally you should enclose your description in
        ///     parenthesis or something that will make it stand out in a string.
        /// </summary>
        /// <returns>A use friendly description of the node.</returns>
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