#region FileInfo

// 
// File: LSLVecRotAccessorNode.cs
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
    ///     Default <see cref="ILSLTupleAccessorNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLTupleAccessorNode : ILSLTupleAccessorNode, ILSLExprNode
    {
        private ILSLSyntaxTreeNode _parent;
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLTupleAccessorNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        ///     Construct an <see cref="LSLTupleAccessorNode" /> from the accessed expression and component accessed.
        /// </summary>
        /// <param name="accessedExpression">The expression the '.' tuple access operator was used on.</param>
        /// <param name="accessedComponent">The tuple component accessed: "x", "y", "z" or "s".</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="accessedExpression" /> or <paramref name="accessedComponent" />
        ///     is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>
        ///         <paramref name="accessedExpression" />.Type is not <see cref="LSLType.Vector" /> or
        ///         <see cref="LSLType.Rotation" />.
        ///     </para>
        ///     <para>
        ///         Or <paramref name="accessedComponent" /> is not one of: "x", "y", "z" or "s".
        ///     </para>
        /// </exception>
        public LSLTupleAccessorNode(ILSLExprNode accessedExpression, string accessedComponent)
        {
            if (accessedExpression == null)
            {
                throw new ArgumentNullException("accessedExpression");
            }

            if (accessedComponent == null)
            {
                throw new ArgumentNullException("accessedComponent");
            }


            if (accessedExpression.Type != LSLType.Vector && accessedExpression.Type != LSLType.Rotation)
            {
                throw new ArgumentException("accessedExpression.Type can only be LSLType.Vector or LSLType.Rotation");
            }


            if (!Utility.EqualsOneOf(accessedComponent, "x", "y", "z", "s"))
            {
                throw new ArgumentException("accessedComponent is not x, y, z or s.", "accessedComponent");
            }

            AccessedComponent = accessedComponent;
            AccessedExpression = accessedExpression;

            AccessedExpression.Parent = this;
        }


        /// <exception cref="ArgumentException">
        ///     <para>
        ///         If <paramref name="accessedExpression" />.Type is not <see cref="LSLType.Vector" /> or
        ///         <see cref="LSLType.Rotation" />.
        ///     </para>
        ///     <para>
        ///         Or <paramref name="context" />.member.Text is not one of: "x", "y", "z" or "s".
        ///     </para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context" /> or <paramref name="accessedExpression" /> is
        ///     <c>null</c>.
        /// </exception>
        internal LSLTupleAccessorNode(LSLParser.DotAccessorExprContext context, ILSLExprNode accessedExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (accessedExpression == null)
            {
                throw new ArgumentNullException("context");
            }

            if (accessedExpression.Type != LSLType.Vector && accessedExpression.Type != LSLType.Rotation)
            {
                throw new ArgumentException("accessedExpression.Type can only be LSLType.Vector or LSLType.Rotation");
            }

            if (!Utility.EqualsOneOf(context.member.Text, "x", "y", "z", "s"))
            {
                throw new ArgumentException("context.member.Text is not x, y, z or s.", "context");
            }

            AccessedComponent = context.member.Text;

            SourceRangeAccessedComponent = new LSLSourceCodeRange(context.member);

            AccessedExpression = accessedExpression;
            AccessedExpression.Parent = this;

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangesAvailable = true;
        }


        /// <summary>
        ///     Create an <see cref="LSLTupleAccessorNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        private LSLTupleAccessorNode(LSLTupleAccessorNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
                SourceRangeAccessedComponent = other.SourceRangeAccessedComponent;
            }

            AccessedComponent = other.AccessedComponent;

            AccessedExpression = other.AccessedExpression.Clone();
            AccessedExpression.Parent = this;

            HasErrors = other.HasErrors;
        }


        /// <summary>
        ///     The expression that the member access operator was used on.
        ///     This should only ever be a reference to a variable.
        ///     Using a member accessor on a constant, even if it is a vector or rotation, is not allowed.
        /// </summary>
        public ILSLExprNode AccessedExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     The raw name of the accessed tuple member, taken from the source code.
        /// </summary>
        public string AccessedComponent { get; private set; }

        /// <summary>
        ///     The source code range of the tuple member that was accessed.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeAccessedComponent { get; private set; }

        ILSLReadOnlyExprNode ILSLTupleAccessorNode.AccessedExpression
        {
            get { return AccessedExpression; }
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLTupleAccessorNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLTupleAccessorNode(sourceRange, Err.Err);
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
        public LSLTupleAccessorNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLTupleAccessorNode(this);
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
        /// <exception cref="ArgumentNullException"><paramref name="visitor" /> is <c>null</c>.</exception>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }

            return visitor.VisitVecRotAccessor(this);
        }


        /// <summary>
        ///     The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type
        {
            get { return LSLType.Float; }
        }


        /// <summary>
        ///     The expression type/classification of the expression. see: <see cref="LSLExpressionType" />
        /// </summary>
        /// <value>
        ///     The type of the expression.
        /// </value>
        /// <exception cref="InvalidOperationException" accessor="get">If <see cref="AccessedExpression" /> is <c>null</c>.</exception>
        public LSLExpressionType ExpressionType
        {
            get
            {
                if (AccessedExpression == null)
                {
                    throw new InvalidOperationException(
                        typeof (LSLTupleAccessorNode).Name +
                        ".AccessedExpression == null.  node is an error node, do not invoke methods on it;  Check 'HasErrors' first.");
                }

                return AccessedExpression.Type == LSLType.Vector
                    ? LSLExpressionType.VectorComponentAccess
                    : LSLExpressionType.RotationComponentAccess;
            }
        }


        /// <summary>
        ///     True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant
        {
            get { return AccessedExpression != null && AccessedExpression.IsConstant; }
        }


        /// <summary>
        ///     True if the expression statement has some modifying effect on a local parameter or global/local variable;  or is a
        ///     function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects
        {
            get { return AccessedExpression != null && AccessedExpression.HasPossibleSideEffects; }
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