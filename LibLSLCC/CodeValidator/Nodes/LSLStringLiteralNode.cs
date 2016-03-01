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

using System;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLStringLiteralNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLStringLiteralNode : LSLConstantLiteralNode<LSLStringLiteralNode>, ILSLStringLiteralNode
    {
        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLStringLiteralNode(LSLSourceCodeRange sourceRange, Err err)
            : base(sourceRange, Err.Err)
            // ReSharper restore UnusedParameter.Local
        {
        }


        /// <summary>
        ///     Create an <see cref="LSLStringLiteralNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLStringLiteralNode(LSLStringLiteralNode other) : base(other)
        {
            PreProcessedText = other.PreProcessedText;
        }


        /// <summary>
        ///     Create an <see cref="LSLStringLiteralNode" /> from the given raw and preprocessed source code text.
        ///     <paramref name="preProccessedText" /> should contain the preprocessed text, with raw control characters entitized
        ///     into escape sequences,
        ///     and existing escape sequences double escaped with an extra backslash.
        /// </summary>
        /// <param name="rawContent">The raw string content, without quotes.</param>
        /// <param name="preProccessedText">The preprocessed string literal text, without quotes.</param>
        /// <seealso cref="LSLDefaultStringPreProcessor" />
        public LSLStringLiteralNode(string rawContent, string preProccessedText)
            : base("\"" + rawContent + "\"", LSLType.String, null)
        {
            PreProcessedText = "\"" + preProccessedText + "\"";
        }


        /// <summary>
        ///     Create an <see cref="LSLStringLiteralNode" /> from the given raw and preprocessed source code text.
        ///     <see cref="PreProcessedText" /> is generated using <see cref="LSLDefaultStringPreProcessor" />.
        /// </summary>
        /// <param name="rawContent">The raw source code text, without quotes.</param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="rawContent" /> contains invalid characters or invalid escape
        ///     sequences.
        /// </exception>
        public LSLStringLiteralNode(string rawContent)
            : base("\"" + rawContent + "\"", LSLType.String, null)
        {
            var preprocessor = new LSLDefaultStringPreProcessor();

            preprocessor.ProcessString(RawText);

            if (preprocessor.HasInvalidEscapeSequences)
            {
                throw new ArgumentException("rawText contains invalid escape sequences.");
            }

            if (preprocessor.HasInvalidCharacters)
            {
                throw new ArgumentException("rawText contains invalid characters.");
            }

            PreProcessedText = preprocessor.Result;
        }


        internal LSLStringLiteralNode(LSLParser.Expr_AtomContext context, string preProcessedText)
            : base(context.GetText(), LSLType.String, new LSLSourceCodeRange(context))
        {
            PreProcessedText = preProcessedText;
        }


        /// <summary>
        ///     The pre-processed text of the string literal.
        /// </summary>
        /// <remarks>
        ///     <see cref="LSLCodeValidator" /> relies on an implementation of <see cref="ILSLStringPreProcessor" /> to fill this
        ///     value out by passing <see cref="ILSLStringPreProcessor" />
        ///     the raw text for the string literal and assigning the string it produces to this property.
        /// </remarks>
        public string PreProcessedText { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        /// <summary>
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <c>null</c>.</exception>
        public override T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            return visitor.VisitStringLiteral(this);
        }


        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static LSLStringLiteralNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLStringLiteralNode(sourceRange, Err.Err);
        }


        /// <summary>
        ///     Deep clones the expression node.  It should clone the node and all of its children and cloneable properties, except
        ///     the parent.
        ///     When cloned, the parent node reference should still point to the same node.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public override LSLStringLiteralNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLStringLiteralNode(this);
        }
    }
}