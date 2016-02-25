#region FileInfo

// 
// File: LSLOpenSimCompiler.cs
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
using System.IO;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Nodes;
using LibLSLCC.CodeValidator.Strategies;
using LibLSLCC.LibraryData;

#endregion

namespace LibLSLCC.Compilers.OpenSim
{
    /// <summary>
    ///     A compiler that converts LSL Syntax trees into CSharp code that is compatible with OpenSim's CSharp LSL runtime.
    /// </summary>
    public sealed class LSLOpenSimCompiler
        // ReSharper restore InconsistentNaming
    {
        private readonly LSLOpenSimCompilerVisitor _visitor = new LSLOpenSimCompilerVisitor();


        /// <summary>
        ///     Construct an <see cref="LSLOpenSimCompiler" /> using the provided <see cref="ILSLBasicLibraryDataProvider" /> and
        ///     <see cref="LSLOpenSimCompilerSettings" /> object.
        /// </summary>
        /// <param name="settings"><see cref="LSLOpenSimCompilerSettings" /> to use.</param>
        /// <param name="libraryDataProvider">An <see cref="ILSLBasicLibraryDataProvider" /> implementation.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="libraryDataProvider" /> or <paramref name="settings" /> is
        ///     <c>null</c>.
        /// </exception>
        public LSLOpenSimCompiler(ILSLBasicLibraryDataProvider libraryDataProvider, LSLOpenSimCompilerSettings settings)
        {
            if (libraryDataProvider == null)
            {
                throw new ArgumentNullException("libraryDataProvider");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            _visitor.LibraryDataProvider = libraryDataProvider;
            Settings = settings;
        }


        /// <summary>
        ///     Construct an <see cref="LSLOpenSimCompiler" /> using the default settings and the provided
        ///     <see cref="ILSLLibraryDataProvider" /> object.
        /// </summary>
        /// <param name="libraryDataProvider">An <see cref="ILSLBasicLibraryDataProvider" /> implementation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="libraryDataProvider" /> is <c>null</c>.</exception>
        public LSLOpenSimCompiler(ILSLBasicLibraryDataProvider libraryDataProvider)
        {
            if (libraryDataProvider == null)
            {
                throw new ArgumentNullException("libraryDataProvider");
            }

            _visitor.LibraryDataProvider = libraryDataProvider;
            Settings = new LSLOpenSimCompilerSettings();
        }


        /// <summary>
        ///     The library data provider the compiler is using to provide modInvoke information.
        /// </summary>
        public ILSLBasicLibraryDataProvider LibraryDataProvider
        {
            get { return _visitor.LibraryDataProvider; }
        }

        /// <summary>
        ///     Settings for the OpenSim CSharp Compiler
        /// </summary>
        /// <exception cref="ArgumentNullException">If the value is set to null.</exception>
        public LSLOpenSimCompilerSettings Settings
        {
            get { return _visitor.Settings; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _visitor.Settings = value;
            }
        }


        /// <summary>
        ///     Compiles a syntax tree into OpenSim compatible CSharp code, writing the output to the specified TextWriter.
        /// </summary>
        /// <param name="compilationUnit">
        ///     The top node of an LSL Syntax tree to compile.
        ///     This is returned from <see cref="LSLCodeValidator.Validate" /> or user implemented Code DOM.
        /// </param>
        /// <param name="writer">The text writer to write the generated code to.</param>
        /// <param name="closeStream">
        ///     Whether or not to close <paramref name="writer" /> once compilation is done.  The default
        ///     value is <c>false</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.HasErrors" /> is <c>true</c> in
        ///     <paramref name="compilationUnit" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="compilationUnit" /> or <paramref name="writer" /> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="IOException">When an IO Error occurs while writing to <paramref name="writer" />.</exception>
        /// <exception cref="ObjectDisposedException">If <paramref name="writer" /> is already disposed.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Settings" /> is <c>null</c>.</exception>
        public void Compile(ILSLCompilationUnitNode compilationUnit, TextWriter writer, bool closeStream = false)
        {
            if (compilationUnit == null)
            {
                throw new ArgumentNullException("compilationUnit");
            }

            if (compilationUnit.HasErrors)
            {
                throw new ArgumentException(typeof (ILSLCompilationUnitNode).Name +
                                            ".HasErrors is true, cannot compile a tree with syntax errors.");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            _visitor.WriteAndFlush(compilationUnit, writer, closeStream);
        }
    }
}