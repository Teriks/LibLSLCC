#region FileInfo
// 
// File: LSLBinaryExpressionNode.cs
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
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    /// <summary>
    /// Default <see cref="ILSLBinaryExpressionNode"/> implementation used by <see cref="LSLCodeValidator"/>
    /// </summary>
    public sealed class LSLBinaryExpressionNode : ILSLBinaryExpressionNode, ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLBinaryExpressionNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <summary>
        /// Create an <see cref="LSLBinaryExpressionNode"/> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        public LSLBinaryExpressionNode(LSLBinaryExpressionNode other)
        {
            Type = other.Type;
            LeftExpression = other.LeftExpression.Clone();
            RightExpression = other.RightExpression.Clone();

            LeftExpression.Parent = this;
            RightExpression.Parent = this;

            SourceRangesAvailable = other.SourceRangesAvailable;

            Operation = other.Operation;
            OperationString = other.OperationString;


            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange.Clone();

                SourceRangeOperation = other.SourceRangeOperation.Clone();
            }

            HasErrors = other.HasErrors;
            Parent = other.Parent;

        }


        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or 
        /// <paramref name="leftExpression"/> or 
        /// <paramref name="rightExpression"/> is <see langword="null" />.
        /// </exception>
        internal LSLBinaryExpressionNode(
            LSLParser.ExpressionContext context,
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


            Type = returns;
            LeftExpression = leftExpression;
            RightExpression = rightExpression;

            leftExpression.Parent = this;
            rightExpression.Parent = this;

            ParseAndSetOperation(operationString);
            
            SourceRange = new LSLSourceCodeRange(context);

            SourceRangeOperation = new LSLSourceCodeRange(operationToken);

            SourceRangesAvailable = true;
        }



        /// <summary>
        /// The expression tree on the left of side of the binary operation.
        /// </summary>
        public ILSLExprNode LeftExpression { get; private set; }



        /// <summary>
        /// The expression tree on the right side of the binary operation.
        /// </summary>
        public ILSLExprNode RightExpression { get; private set; }


        /// <summary>
        /// The source code range that encompasses the binary expression and its children.
        /// </summary>
        public LSLSourceCodeRange SourceRangeOperation { get; private set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        /// The binary operation type of this node.
        /// </summary>
        public LSLBinaryOperationType Operation { get; private set; }


        /// <summary>
        /// The string representation of the binary operation this node preforms.
        /// </summary>
        public string OperationString { get; private set; }


        ILSLReadOnlyExprNode ILSLBinaryExpressionNode.LeftExpression
        {
            get { return LeftExpression; }
        }

        ILSLReadOnlyExprNode ILSLBinaryExpressionNode.RightExpression
        {
            get { return RightExpression; }
        }

        /// <summary>
        /// Gets an instance of this node that represents a syntax error in the node.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>An error form of the node.</returns>
        public static
            LSLBinaryExpressionNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLBinaryExpressionNode(sourceRange, Err.Err);
        }

        private void ParseAndSetOperation(string operationString)
        {
            OperationString = operationString;
            Operation = LSLBinaryOperationTypeTools.ParseFromOperator(OperationString);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceRange { get; private set; }


        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values
        /// for this node.
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
            return visitor.VisitBinaryExpression(this);
        }

        /// <summary>
        /// True if the expression statement has some modifying effect on a local parameter or global/local variable;  or is a function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects
        {
            get
            {

                if (LeftExpression == null || RightExpression == null) return false;

                var eitherSideHaveSideEffects = (LeftExpression.HasPossibleSideEffects || RightExpression.HasPossibleSideEffects);
     
                var operatorModifiesLeftVariable = LeftExpression.IsModifiableLeftValue() && Operation.IsAssignOrModifyAssign();

                return (eitherSideHaveSideEffects || operatorModifiesLeftVariable);
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
            get { return LSLExpressionType.BinaryExpression; }
        }


        /// <summary>
        /// True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant
        {
            get { return 
                    (LeftExpression != null && RightExpression != null) &&
                    (LeftExpression.IsConstant && RightExpression.IsConstant); }
        }


        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }



        /// <summary>
        /// Deep clones the expression node.  It should clone the node and all of its children and cloneable properties, except the parent.
        /// When cloned, the parent node reference should still point to the same node.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceRange);
            }

            return new LSLBinaryExpressionNode(this);
        }

        #endregion
    }
}