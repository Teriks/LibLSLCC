#region FileInfo
// 
// File: LSLTypecastExprNode.cs
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
    /// Default <see cref="ILSLTypecastExprNode"/> implementation used by <see cref="LSLCodeValidator"/>
    /// </summary>
    public sealed class LSLTypecastExprNode : ILSLTypecastExprNode, ILSLExprNode
    {

        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLTypecastExprNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }

        internal LSLTypecastExprNode(LSLParser.Expr_TypeCastContext context, LSLType result,
            ILSLExprNode castedExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (castedExpression == null)
            {
                throw new ArgumentNullException("castedExpression");
            }

            CastToTypeString = context.cast_type.Text;
            CastToType = LSLTypeTools.FromLSLTypeString(CastToTypeString);

            CastedExpression = castedExpression;
            CastedExpression.Parent = this;

            Type = result;

            SourceRange = new LSLSourceCodeRange(context);
            SourceRangeCloseParenth = new LSLSourceCodeRange(context.close_parenth);
            SourceRangeOpenParenth = new LSLSourceCodeRange(context.open_parenth);
            SourceRangeCastTypeName = new LSLSourceCodeRange(context.cast_type);

            SourceRangesAvailable = true;
        }


        /// <summary>
        /// Create an <see cref="LSLTypecastExprNode"/> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        public LSLTypecastExprNode(LSLTypecastExprNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            CastToTypeString = other.CastToTypeString;
            CastToType = other.CastToType;

            CastedExpression = other.CastedExpression.Clone();
            CastedExpression.Parent = this;

            Type = other.Type;

            SourceRangesAvailable = other.SourceRangesAvailable;


            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange.Clone();
                SourceRangeOpenParenth = other.SourceRangeOpenParenth.Clone();
                SourceRangeCastTypeName = other.SourceRangeCastTypeName.Clone();
                SourceRangeCloseParenth = other.SourceRangeCloseParenth.Clone();
            }

            HasErrors = other.HasErrors;
            Parent = other.Parent;
        }



        /// <summary>
        /// The expression node that represents the expression being casted.  This should never be null.
        /// </summary>
        public ILSLExprNode CastedExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        ILSLReadOnlyExprNode ILSLTypecastExprNode.CastedExpression
        {
            get { return CastedExpression; }
        }

        /// <summary>
        /// The <see cref="LSLType"/> that represents the type the expression is being cast to.
        /// </summary>
        public LSLType CastToType { get; private set; }

        /// <summary>
        /// The raw type name of the type the expression is being cast to, taken from the source code.
        /// </summary>
        public string CastToTypeString { get; private set; }

        /// <summary>
        /// The source code range of the closing parenthesis of the enclosed cast type.
        /// </summary>
        public LSLSourceCodeRange SourceRangeCloseParenth { get; private set; }

        /// <summary>
        /// The source code range of the open parenthesis of the enclosed cast type.
        /// </summary>
        public LSLSourceCodeRange SourceRangeOpenParenth { get; private set; }

        /// <summary>
        /// The source code range of the type name used for the cast.
        /// </summary>
        public LSLSourceCodeRange SourceRangeCastTypeName { get; private set; }


        /// <summary>
        /// Returns a version of this node type that represents its error state;  in case of a syntax error
        /// in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLTypecastExprNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLTypecastExprNode(sourceRange, Err.Err);
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
            return HasErrors ? GetError(SourceRange) : new LSLTypecastExprNode(this);
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
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitTypecastExpression(this);
        }



        /// <summary>
        /// The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type { get; private set; }



        /// <summary>
        /// The expression type/classification of the expression. see: <see cref="LSLExpressionType" />
        /// </summary>
        /// <value>
        /// The type of the expression.
        /// </value>
        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.TypecastExpression; }
        }

        /// <summary>
        /// True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant
        {
            get { return CastedExpression != null && CastedExpression.IsConstant; }
        }


        /// <summary>
        /// True if the expression has some modifying effect on a local parameter or global/local variable;  or is a function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects
        {
            get { return CastedExpression != null && CastedExpression.HasPossibleSideEffects; }
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