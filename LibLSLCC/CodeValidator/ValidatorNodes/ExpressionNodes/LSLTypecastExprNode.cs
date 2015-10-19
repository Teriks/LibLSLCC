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
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLTypecastExprNode : ILSLTypecastExprNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLTypecastExprNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
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


            ParserContext = context;
            CastedExpression = castedExpression;
            Type = result;
            CastedExpression.Parent = this;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }

        internal LSLParser.Expr_TypeCastContext ParserContext { get; private set; }

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
        /// The LSLType that represents the type the expression is being cast to.
        /// </summary>
        public LSLType CastToType
        {
            get { return LSLTypeTools.FromLSLTypeString(ParserContext.cast_type.Text); }
        }

        /// <summary>
        /// The raw type name of the type the expression is being cast to, taken from the source code.
        /// </summary>
        public string CastToTypeString
        {
            get { return ParserContext.cast_type.Text; }
        }

        public static
            LSLTypecastExprNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLTypecastExprNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
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
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLTypecastExprNode(ParserContext, Type, CastedExpression.Clone())
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
        /// Accept a visit from an implementor of ILSLValidatorNodeVisitor
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitTypecastExpression(this);
        }


        /// <summary>
        /// The return type of the expression.
        /// </summary>
        public LSLType Type { get; private set; }


        /// <summary>
        /// The expression type/classification of the expression.
        /// <see cref="LSLExpressionType"/>
        /// </summary>
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