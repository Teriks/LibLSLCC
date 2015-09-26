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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLRotationLiteralNode : ILSLRotationLiteralNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLRotationLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLRotationLiteralNode(LSLParser.RotationLiteralContext context, ILSLExprNode x, ILSLExprNode y,
            ILSLExprNode z, ILSLExprNode s)
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

            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            s.Parent = this;

            ParserContext = context;
            XExpression = x;
            YExpression = y;
            ZExpression = z;
            SExpression = s;

            SourceCodeRange = new LSLSourceCodeRange(context);

            CommaOneSourceCodeRange = new LSLSourceCodeRange(context.comma_one);
            CommaTwoSourceCodeRange = new LSLSourceCodeRange(context.comma_two);
            CommaThreeSourceCodeRange = new LSLSourceCodeRange(context.comma_three);
        }

        internal LSLParser.RotationLiteralContext ParserContext { get; }
        public ILSLExprNode XExpression { get; }
        public ILSLExprNode YExpression { get; }
        public ILSLExprNode ZExpression { get; }
        public ILSLExprNode SExpression { get; }

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

        public static
            LSLRotationLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLRotationLiteralNode(sourceRange, Err.Err);
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

            return new LSLRotationLiteralNode(ParserContext, XExpression.Clone(), YExpression.Clone(),
                ZExpression.Clone(),
                SExpression.Clone())
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }


        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }

        public LSLSourceCodeRange CommaOneSourceCodeRange { get; }
        public LSLSourceCodeRange CommaTwoSourceCodeRange { get; }
        public LSLSourceCodeRange CommaThreeSourceCodeRange { get; }

        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitRotationLiteral(this);
        }


        public LSLType Type
        {
            get { return LSLType.Rotation; }
        }


        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }


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