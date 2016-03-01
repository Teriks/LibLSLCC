#region FileInfo

// 
// File: LSLCodeFormatter.cs
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
using System.Collections.Generic;
using System.IO;
using LibLSLCC.CodeValidator;

#endregion

namespace LibLSLCC.CodeFormatter
{
    /// <summary>
    ///     Implements a code formatter that can format LibLSLCC syntax tree nodes.
    /// </summary>
    public sealed class LSLCodeFormatter
    {
        private LSLCodeFormatterSettings _settings;


        /// <summary>
        ///     Construct a new LSLCodeFormatter.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings" /> is <c>null</c>.</exception>
        public LSLCodeFormatter(LSLCodeFormatterSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            _settings = settings;
        }


        /// <summary>
        ///     Construct a new LSLCodeFormatter.
        /// </summary>
        public LSLCodeFormatter()
        {
            _settings = new LSLCodeFormatterSettings();
        }


        /// <summary>
        ///     The code formatter configuration.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="LSLCodeFormatter.Settings" /> is set to <c>null</c>.</exception>
        public LSLCodeFormatterSettings Settings
        {
            get { return _settings; }
            set
            {
                if (_settings == null)
                {
                    throw new ArgumentNullException("value",
                        typeof (LSLCodeFormatter).Name + ".Settings cannot be null!.");
                }
                _settings = value;
            }
        }


        /// <summary>
        ///     Formats an <see cref="ILSLCompilationUnitNode" /> to an output writer.
        /// </summary>
        /// <param name="compilationUnit">A top level compilation unit syntax tree node.</param>
        /// <param name="writer">The writer to write the formated source code to.</param>
        /// <param name="closeStream">
        ///     <c>true</c> if this method should close <paramref name="writer" /> when finished.  The
        ///     default value is <c>false</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.HasErrors" /> is <c>true</c> in
        ///     <paramref name="compilationUnit" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="compilationUnit" /> or <paramref name="writer" /> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="LSLCodeFormatter.Settings" /> is <c>null</c>.</exception>
        public void Format(ILSLCompilationUnitNode compilationUnit, TextWriter writer,
            bool closeStream = false)
        {
            if (compilationUnit == null) throw new ArgumentNullException("compilationUnit");

            Format(compilationUnit.Comments, compilationUnit, writer, closeStream);
        }


        /// <summary>
        ///     Formats an <see cref="ILSLCompilationUnitNode" /> to an output writer, with the ability to provide optional source
        ///     code hint text.
        /// </summary>
        /// <param name="sourceCodeHint">
        ///     When provided the formatter can make more intelligent decisions in various places, such as retaining user spacing
        ///     when comments appear on the same line as a statement.
        /// </param>
        /// <param name="compilationUnit">A top level compilation unit syntax tree node.</param>
        /// <param name="writer">The writer to write the formated source code to.</param>
        /// <param name="closeStream">
        ///     <c>true</c> if this method should close <paramref name="writer" /> when finished.  The
        ///     default value is <c>false</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.HasErrors" /> is <c>true</c> in
        ///     <paramref name="compilationUnit" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="compilationUnit" /> or <paramref name="writer" /> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="LSLCodeFormatter.Settings" /> is <c>null</c>.</exception>
        public void Format(string sourceCodeHint, ILSLCompilationUnitNode compilationUnit, TextWriter writer,
            bool closeStream = false)
        {
            if (compilationUnit == null) throw new ArgumentNullException("compilationUnit");

            Format(sourceCodeHint, compilationUnit.Comments, compilationUnit, writer, closeStream);
        }


        /// <summary>
        ///     Formats an <see cref="ILSLReadOnlySyntaxTreeNode" /> to an output writer. <para/>
        ///     Comments are discarded.
        /// </summary>
        /// <param name="syntaxTree">Syntax tree node to format to output.</param>
        /// <param name="writer">The writer to write the formated source code to.</param>
        /// <param name="closeStream">
        ///     <c>true</c> if this method should close <paramref name="writer" /> when finished.  The
        ///     default value is <c>false</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.HasErrors" /> is <c>true</c> in
        ///     <paramref name="syntaxTree" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="syntaxTree" /> or <paramref name="writer" /> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="LSLCodeFormatter.Settings" /> is <c>null</c>.</exception>
        public void Format(
            ILSLReadOnlySyntaxTreeNode syntaxTree,
            TextWriter writer,
            bool closeStream = false)
        {
            if (syntaxTree == null)
            {
                throw new ArgumentNullException("syntaxTree");
            }

            if (syntaxTree.HasErrors)
            {
                throw new ArgumentException(typeof(ILSLCompilationUnitNode).Name +
                                            ".HasErrors is true, cannot format a tree with syntax errors.");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (Settings == null)
            {
                throw new InvalidOperationException(typeof(LSLCodeFormatter).Name + ".Settings cannot be null.");
            }

            var formatter = new LSLCodeFormatterVisitor(Settings);
            formatter.WriteAndFlush(null, null, syntaxTree, writer, closeStream);
        }



        /// <summary>
        ///     Formats an <see cref="ILSLReadOnlySyntaxTreeNode" /> to an output writer.
        /// </summary>
        /// <param name="sourceComments">Source code comments.</param>
        /// <param name="syntaxTree">Syntax tree node to format to output.</param>
        /// <param name="writer">The writer to write the formated source code to.</param>
        /// <param name="closeStream">
        ///     <c>true</c> if this method should close <paramref name="writer" /> when finished.  The
        ///     default value is <c>false</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.HasErrors" /> is <c>true</c> in
        ///     <paramref name="syntaxTree" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="sourceComments"/> or <paramref name="syntaxTree" /> or <paramref name="writer" /> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="LSLCodeFormatter.Settings" /> is <c>null</c>.</exception>
        public void Format(
            IEnumerable<LSLComment> sourceComments, 
            ILSLReadOnlySyntaxTreeNode syntaxTree,
            TextWriter writer,
            bool closeStream = false)
        {
            if (sourceComments == null)
            {
                throw new ArgumentNullException("sourceComments");
            }
            if (syntaxTree == null)
            {
                throw new ArgumentNullException("syntaxTree");
            }

            if (syntaxTree.HasErrors)
            {
                throw new ArgumentException(typeof (ILSLCompilationUnitNode).Name +
                                            ".HasErrors is true, cannot format a tree with syntax errors.");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (Settings == null)
            {
                throw new InvalidOperationException(typeof (LSLCodeFormatter).Name + ".Settings cannot be null.");
            }

            var formatter = new LSLCodeFormatterVisitor(Settings);
            formatter.WriteAndFlush(null, sourceComments, syntaxTree, writer, closeStream);
        }


        /// <summary>
        ///     Formats an <see cref="ILSLReadOnlySyntaxTreeNode" /> to an output writer, with the ability to provide source code hint text. <para/>
        ///     Comments are discarded.
        /// </summary>
        /// <param name="sourceCodeHint">
        ///     When provided the formatter can make more intelligent decisions in various places, such as retaining user spacing
        ///     when comments appear on the same line as a statement.
        /// </param>
        /// <param name="syntaxTree">Syntax tree node to format to output.</param>
        /// <param name="writer">The writer to write the formated source code to.</param>
        /// <param name="closeStream">
        ///     <c>true</c> if this method should close <paramref name="writer" /> when finished.  The
        ///     default value is <c>false</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.HasErrors" /> is <c>true</c> in
        ///     <paramref name="syntaxTree" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="sourceCodeHint" /> or <paramref name="syntaxTree" /> or <paramref name="writer" /> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="LSLCodeFormatter.Settings" /> is <c>null</c>.</exception>
        public void Format(
            string sourceCodeHint,
            ILSLReadOnlySyntaxTreeNode syntaxTree, 
            TextWriter writer,
            bool closeStream = false)
        {
            if (sourceCodeHint == null)
            {
                throw new ArgumentNullException("sourceCodeHint");
            }
            if (syntaxTree == null)
            {
                throw new ArgumentNullException("syntaxTree");
            }
            if (syntaxTree.HasErrors)
            {
                throw new ArgumentException(typeof(ILSLCompilationUnitNode).Name +
                                            ".HasErrors is true, cannot format a tree with syntax errors.");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (Settings == null)
            {
                throw new InvalidOperationException(typeof(LSLCodeFormatter).Name + ".Settings cannot be null.");
            }

            var formatter = new LSLCodeFormatterVisitor(Settings);
            formatter.WriteAndFlush(sourceCodeHint, null, syntaxTree, writer, closeStream);
        }


        /// <summary>
        ///     Formats an <see cref="ILSLReadOnlySyntaxTreeNode" /> to an output writer, with the ability to provide source code hint text.
        /// </summary>
        /// <param name="sourceCodeHint">
        ///     When provided the formatter can make more intelligent decisions in various places, such as retaining user spacing
        ///     when comments appear on the same line as a statement.
        /// </param>
        /// <param name="sourceComments">Source code comments.</param>
        /// <param name="syntaxTree">Syntax tree node to format to output.</param>
        /// <param name="writer">The writer to write the formated source code to.</param>
        /// <param name="closeStream">
        ///     <c>true</c> if this method should close <paramref name="writer" /> when finished.  The
        ///     default value is <c>false</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.HasErrors" /> is <c>true</c> in
        ///     <paramref name="syntaxTree" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="sourceCodeHint" /> or <paramref name="sourceComments"/> or <paramref name="syntaxTree" /> or <paramref name="writer" /> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="LSLCodeFormatter.Settings" /> is <c>null</c>.</exception>
        public void Format(
            string sourceCodeHint, 
            IEnumerable<LSLComment> sourceComments,
            ILSLReadOnlySyntaxTreeNode syntaxTree, 
            TextWriter writer,
            bool closeStream = false)
        {
            if (sourceCodeHint == null)
            {
                throw new ArgumentNullException("sourceCodeHint");
            }
            if (sourceComments == null)
            {
                throw new ArgumentNullException("sourceComments");
            }
            if (syntaxTree == null)
            {
                throw new ArgumentNullException("syntaxTree");
            }
            if (syntaxTree.HasErrors)
            {
                throw new ArgumentException(typeof (ILSLCompilationUnitNode).Name +
                                            ".HasErrors is true, cannot format a tree with syntax errors.");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (Settings == null)
            {
                throw new InvalidOperationException(typeof (LSLCodeFormatter).Name + ".Settings cannot be null.");
            }

            var formatter = new LSLCodeFormatterVisitor(Settings);
            formatter.WriteAndFlush(sourceCodeHint, sourceComments, syntaxTree, writer, closeStream);
        }
    }
}