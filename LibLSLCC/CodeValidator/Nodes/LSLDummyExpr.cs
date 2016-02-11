#region FileInfo
// 
// File: LSLDummyExpr.cs
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

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    internal class LSLDummyExpr : ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLDummyExpr(Err err)
// ReSharper restore UnusedParameter.Local
        {
        }

        public LSLDummyExpr()
        {
            SourceCodeRangesAvailable = false;
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; private set; }

        public LSLSourceCodeRange SourceCodeRange
        {
            get { return new LSLSourceCodeRange(); }
        }

        public bool SourceCodeRangesAvailable { get; private set; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            throw new NotImplementedException("Visited LSLDummyExpr, Don't visit trees with syntax errors.");
        }


        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        /// The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type { get; set; }



        /// <summary>
        /// The expression type/classification of the expression. see: <see cref="LSLExpressionType" />
        /// </summary>
        /// <value>
        /// The type of the expression.
        /// </value>
        public LSLExpressionType ExpressionType { get; set; }


        /// <summary>
        /// True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant { get; set; }

        /// <summary>
        /// True if the expression statement has some modifying effect on a local parameter or global/local variable;  or is a function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects { get; private set; }


        /// <summary>
        /// Deep clones the expression node.  It should clone the node and also clone all of its children.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public ILSLExprNode Clone()
        {
            return new LSLDummyExpr
            {
                Parent = Parent,
                ExpressionType = ExpressionType,
                Type = Type,
                HasErrors = HasErrors,
                IsConstant = IsConstant,
                HasPossibleSideEffects = HasPossibleSideEffects
            };
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
        /// Deep clone the expression node into a new node.
        /// This should deep clone all of the node's children as well.
        /// </summary>
        /// <returns>Cloned <see cref="ILSLReadOnlyExprNode"/></returns>
        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }

        #endregion
    }
}