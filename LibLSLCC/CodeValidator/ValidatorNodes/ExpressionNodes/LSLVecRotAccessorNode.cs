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
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
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

        internal LSLTupleAccessorNode(LSLParser.Expr_DotAccessorContext context, ILSLExprNode accessedExpression,
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
        }

        internal LSLParser.Expr_DotAccessorContext ParserContext { get; private set; }
        public ILSLExprNode AccessedExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public string AccessedComponentString
        {
            get { return ParserContext.member.Text; }
        }

        public LSLTupleComponent AccessedComponent { get; private set; }
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


        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }

            return visitor.VisitVecRotAccessor(this);
        }


        public LSLType Type
        {
            get { return LSLType.Float; }
        }


        public LSLExpressionType ExpressionType
        {
            get
            {
                return AccessedType == LSLType.Vector
                    ? LSLExpressionType.VectorComponentAccess
                    : LSLExpressionType.RotationComponentAccess;
            }
        }


        public bool IsConstant
        {
            get { return AccessedExpression.IsConstant; }
        }


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