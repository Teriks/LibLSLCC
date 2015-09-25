#region FileInfo
// 
// 
// File: LSLBinaryExpressionNode.cs
// 
// Last Compile: 25/09/2015 @ 11:47 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
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
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLBinaryExpressionNode : ILSLBinaryExpressionNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLBinaryExpressionNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLBinaryExpressionNode(LSLParser.ExpressionContext context,
            IToken operationToken,
            ILSLExprNode leftExpression,
            ILSLExprNode rightExpression,
            LSLType returns,
            string operationString)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (leftExpression == null)
            {
                throw new ArgumentNullException("leftExpression");
            }

            if (rightExpression == null)
            {
                throw new ArgumentNullException("rightExpression");
            }


            ParserContext = context;
            Type = returns;
            LeftExpression = leftExpression;
            RightExpression = rightExpression;

            leftExpression.Parent = this;
            rightExpression.Parent = this;


            ParseAndSetOperation(operationString);
            OperationString = operationString;

            SourceCodeRange = new LSLSourceCodeRange(context);

            OperationToken = operationToken;

            OperationSourceCodeRange = new LSLSourceCodeRange(operationToken);
        }

        internal IToken OperationToken { get; }
        internal LSLParser.ExpressionContext ParserContext { get; }
        public ILSLExprNode LeftExpression { get; }
        public ILSLExprNode RightExpression { get; }
        public LSLSourceCodeRange OperationSourceCodeRange { get; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLBinaryOperationType Operation { get; private set; }
        public string OperationString { get; }

        ILSLReadOnlyExprNode ILSLBinaryExpressionNode.LeftExpression
        {
            get { return LeftExpression; }
        }

        ILSLReadOnlyExprNode ILSLBinaryExpressionNode.RightExpression
        {
            get { return RightExpression; }
        }

        public static
            LSLBinaryExpressionNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLBinaryExpressionNode(sourceRange, Err.Err);
        }

        private void ParseAndSetOperation(string operationString)
        {
            Operation = LSLBinaryOperationTypeTools.ParseFromOperator(operationString);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; internal set; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpression(this);
        }


        public string DescribeType()
        {
            return "(" + Type + (this.IsLiteral() ? " Literal)" : ")");
        }


        public LSLType Type { get; }


        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.BinaryExpression; }
        }


        public bool IsConstant
        {
            get { return (LeftExpression.IsConstant && RightExpression.IsConstant); }
        }


        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }


        public ILSLSyntaxTreeNode Parent { get; set; }


        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            return new LSLBinaryExpressionNode(ParserContext, OperationToken, RightExpression.Clone(),
                LeftExpression.Clone(), Type,
                OperationString)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };
        }

        #endregion
    }
}