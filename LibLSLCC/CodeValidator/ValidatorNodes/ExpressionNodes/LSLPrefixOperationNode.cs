#region FileInfo
// 
// File: LSLPrefixOperationNode.cs
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
    public class LSLPrefixOperationNode : ILSLPrefixOperationNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLPrefixOperationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLPrefixOperationNode(LSLParser.Expr_PrefixOperationContext context, LSLType resultType,
            ILSLExprNode rightExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (rightExpression == null)
            {
                throw new ArgumentNullException("rightExpression");
            }


            ParserContext = context;
            Type = resultType;
            RightExpression = rightExpression;
            RightExpression.Parent = this;

            ParseAndSetOperation(context.operation.Text);

            SourceCodeRange = new LSLSourceCodeRange(context);

            OperationSourceCodeRange = new LSLSourceCodeRange(context.operation);
        }

        internal LSLParser.Expr_PrefixOperationContext ParserContext { get; private set; }
        public ILSLExprNode RightExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLPrefixOperationType Operation { get; private set; }

        public string OperationString
        {
            get { return ParserContext.operation.Text; }
        }

        ILSLReadOnlyExprNode ILSLPrefixOperationNode.RightExpression
        {
            get { return RightExpression; }
        }

        public static
            LSLPrefixOperationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLPrefixOperationNode(sourceRange, Err.Err);
        }

        private void ParseAndSetOperation(string operationString)
        {
            Operation = LSLPrefixOperationTypeTools.ParseFromOperator(operationString);
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

            return new LSLPrefixOperationNode(ParserContext, Type, RightExpression.Clone())
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }


        public ILSLSyntaxTreeNode Parent { get; set; }


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }

        public LSLSourceCodeRange OperationSourceCodeRange { get; private set; }

        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitPrefixOperation(this);
        }


        public LSLType Type { get; private set; }

        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.PrefixExpression; }
        }

        public bool IsConstant
        {
            get { return RightExpression.IsConstant; }
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