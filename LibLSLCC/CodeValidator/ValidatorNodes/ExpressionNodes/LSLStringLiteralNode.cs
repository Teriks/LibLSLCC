#region FileInfo
// 
// File: LSLStringLiteralNode.cs
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

using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public class LSLStringLiteralNode : LSLConstantLiteralNode, ILSLStringLiteralNode
    {
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLStringLiteralNode(LSLSourceCodeRange sourceRange, Err err)
            : base(sourceRange, Err.Err)
            // ReSharper restore UnusedParameter.Local
        {
        }

        internal LSLStringLiteralNode(LSLParser.Expr_AtomContext context, string preProccessedText)
            : base(context, LSLType.String)
        {
            PreProccessedText = preProccessedText;
        }


        /// <summary>
        /// The pre-processed text of the string literal.
        /// 
        /// LSLCodeValidator relies on an implementation of ILSLStringPreProcessor to fill this value out by passing ILSLStringPreProcessor
        /// the raw text for the string literal and assigning the string it produces to this property.
        /// <see cref="ILSLStringPreProcessor"/>
        /// </summary>
        public string PreProccessedText { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        /// Accept a visit from an implementor of ILSLValidatorNodeVisitor
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public override T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitStringLiteral(this);
        }

        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }

        public static LSLStringLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLStringLiteralNode(sourceRange, Err.Err);
        }

        /// <summary>
        /// Deep clones the expression node.  It should clone the node and also clone all of its children.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public override ILSLExprNode Clone()
        {
            if (HasErrors)
            {
                return GetError(SourceCodeRange);
            }

            var x = new LSLStringLiteralNode(ParserContext, PreProccessedText)
            {
                HasErrors = HasErrors,
                Parent = Parent
            };

            return x;
        }
    }
}