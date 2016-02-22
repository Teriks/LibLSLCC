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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    /// <summary>
    /// Default <see cref="ILSLTupleAccessorNode"/> implementation used by <see cref="LSLCodeValidator"/>
    /// </summary>
    public sealed class LSLTupleAccessorNode : ILSLTupleAccessorNode, ILSLExprNode
    {

        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLTupleAccessorNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <exception cref="ArgumentException">If <paramref name="accessedExpressionType"/> is not <see cref="LSLType.Vector"/> or <see cref="LSLType.Rotation"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> or <paramref name="accessedExpression"/> is <see langword="null" />.</exception>
        internal LSLTupleAccessorNode(LSLParser.DotAccessorExprContext context, ILSLExprNode accessedExpression,
            LSLType accessedExpressionType,
            LSLTupleComponent accessedComponent)
        {
            if (accessedExpressionType != LSLType.Vector && accessedExpressionType != LSLType.Rotation)
            {
                throw new ArgumentException("accessedExpressionType can only be LSLType.Vector or LSLType.Rotation");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (accessedExpression == null)
            {
                throw new ArgumentNullException("context");
            }

            AccessedComponentString = context.member.Text;
            AccessedComponent = accessedComponent;

            SourceRangeAccessedComponent = new LSLSourceCodeRange(context.member);

            AccessedExpression = accessedExpression;
            AccessedExpression.Parent = this;

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangesAvailable = true;
        }


        /// <summary>
        /// Create an <see cref="LSLTupleAccessorNode"/> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null" />.</exception>
        public LSLTupleAccessorNode(LSLTupleAccessorNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            AccessedComponentString = other.AccessedComponentString;
            AccessedComponent = other.AccessedComponent;

            AccessedExpression = other.AccessedExpression.Clone();
            AccessedExpression.Parent = this;

            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange.Clone();
                SourceRangeAccessedComponent = other.SourceRangeAccessedComponent.Clone();
            }

            HasErrors = other.HasErrors;
            Parent = other.Parent;
        }


        /// <summary>
        /// The expression that the member access operator was used on.
        /// This should only ever be a reference to a variable.
        /// Using a member accessor on a constant, even if it is a vector or rotation, is not allowed.
        /// </summary>
        public ILSLExprNode AccessedExpression { get; private set; }



        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        /// The raw name of the accessed tuple member, taken from the source code.
        /// </summary>
        public string AccessedComponentString { get; private set; }

        /// <summary>
        /// The source code range of the tuple member that was accessed.
        /// </summary>
        public LSLSourceCodeRange SourceRangeAccessedComponent { get; private set; }

        /// <summary>
        /// The tuple component accessed.
        /// <see cref="LSLTupleComponent"/>
        /// </summary>
        public LSLTupleComponent AccessedComponent { get; private set; }




        ILSLReadOnlyExprNode ILSLTupleAccessorNode.AccessedExpression
        {
            get { return AccessedExpression; }
        }

        /// <summary>
        /// Returns a version of this node type that represents its error state;  in case of a syntax error
        /// in the node that prevents the node from being even partially built.
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

        #region AccessType enum

        #endregion

        #region Component enum

        #endregion

        #region ILSLExprNode Members


        /// <summary>
        /// Deep clones the expression node.  It should clone the node and also clone all of its children.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public ILSLExprNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLTupleAccessorNode(this);
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; private set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceRange { get; private set; }


        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <see langword="null" />.</exception>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }

            return visitor.VisitVecRotAccessor(this);
        }



        /// <summary>
        /// The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type
        {
            get { return LSLType.Float; }
        }


        /// <summary>
        /// The expression type/classification of the expression. see: <see cref="LSLExpressionType" />
        /// </summary>
        /// <value>
        /// The type of the expression.
        /// </value>
        /// <exception cref="InvalidOperationException" accessor="get">If <see cref="AccessedExpression"/> is <see langword="null"/>.</exception>
        public LSLExpressionType ExpressionType
        {
            get
            {
                if (AccessedExpression == null)
                {
                    throw new InvalidOperationException(
                        typeof(LSLTupleAccessorNode).Name+".AccessedExpression == null.  node is an error node, do not invoke methods on it;  Check 'HasErrors' first.");
                }

                return AccessedExpression.Type == LSLType.Vector
                    ? LSLExpressionType.VectorComponentAccess
                    : LSLExpressionType.RotationComponentAccess;
            }
        }


        /// <summary>
        /// True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant
        {
            get { return AccessedExpression != null && AccessedExpression.IsConstant; }
        }


        /// <summary>
        /// True if the expression statement has some modifying effect on a local parameter or global/local variable;  or is a function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects
        {
            get
            {
                return AccessedExpression != null && AccessedExpression.HasPossibleSideEffects;
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