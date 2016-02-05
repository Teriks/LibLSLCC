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
    public class LSLTupleAccessorNode : ILSLTupleAccessorNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLTupleAccessorNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLTupleAccessorNode(LSLParser.DotAccessorExprContext context, ILSLExprNode accessedExpression,
            LSLType accessedType,
            LSLTupleComponent accessedComponent)
        {
            if (accessedType != LSLType.Vector && accessedType != LSLType.Rotation)
            {
                throw new ArgumentException("accessedType can only be LSLType.Vector or LSLType.Rotation");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (accessedExpression == null)
            {
                throw new ArgumentNullException("context");
            }

            ParserContext = context;
            AccessedComponent = accessedComponent;
            AccessedType = accessedType;
            AccessedExpression = accessedExpression;
            AccessedExpression.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);

            SourceCodeRangesAvailable = true;
        }

        internal LSLParser.DotAccessorExprContext ParserContext { get; private set; }


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
        public string AccessedComponentString
        {
            get { return ParserContext.member.Text; }
        }

        /// <summary>
        /// The tuple component accessed.
        /// <see cref="LSLTupleComponent"/>
        /// </summary>
        public LSLTupleComponent AccessedComponent { get; private set; }


        /// <summary>
        /// The type that the member access was preformed on.
        /// This should only ever be <see cref="LSLType.Vector"/> or <see cref="LSLType.Rotation"/>.
        /// </summary>
        public LSLType AccessedType { get; private set; }



        ILSLReadOnlyExprNode ILSLTupleAccessorNode.AccessedExpression
        {
            get { return AccessedExpression; }
        }

        public static
            LSLTupleAccessorNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLTupleAccessorNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
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
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLTupleAccessorNode(ParserContext, AccessedExpression.Clone(), AccessedType, AccessedComponent)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; set; }


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
        public LSLExpressionType ExpressionType
        {
            get
            {
                return AccessedType == LSLType.Vector
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