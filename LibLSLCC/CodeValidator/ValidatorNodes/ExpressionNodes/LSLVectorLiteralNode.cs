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
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLVectorLiteralNode : ILSLVectorLiteralNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLVectorLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLVectorLiteralNode(LSLParser.VectorLiteralContext context, ILSLExprNode x, ILSLExprNode y,
            ILSLExprNode z)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (x == null)
            {
                throw new ArgumentNullException("x");
            }
            x.Parent = this;

            if (y == null)
            {
                throw new ArgumentNullException("y");
            }
            y.Parent = this;

            if (z == null)
            {
                throw new ArgumentNullException("z");
            }
            z.Parent = this;


            ParserContext = context;
            XExpression = x;
            YExpression = y;
            ZExpression = z;

            SourceCodeRange = new LSLSourceCodeRange(context);

            CommaOneSourceCodeRange = new LSLSourceCodeRange(context.comma_one);

            CommaTwoSourceCodeRange = new LSLSourceCodeRange(context.comma_two);
        }

        internal LSLParser.VectorLiteralContext ParserContext { get; private set; }
        public ILSLExprNode XExpression { get; private set; }
        public ILSLExprNode YExpression { get; private set; }
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

        public static
            LSLVectorLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLVectorLiteralNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLVectorLiteralNode(ParserContext, XExpression.Clone(), YExpression.Clone(), ZExpression.Clone())
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }


        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }

        public LSLSourceCodeRange CommaOneSourceCodeRange { get; private set; }

        public LSLSourceCodeRange CommaTwoSourceCodeRange { get; private set; }

        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitVectorLiteral(this);
        }


        public LSLType Type
        {
            get { return LSLType.Vector; }
        }


        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }


        public bool IsConstant
        {
            get
            {
                var precondition = XExpression != null && YExpression != null && ZExpression != null;
                return precondition && XExpression.IsConstant && YExpression.IsConstant &&
                       ZExpression.IsConstant;
            }
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