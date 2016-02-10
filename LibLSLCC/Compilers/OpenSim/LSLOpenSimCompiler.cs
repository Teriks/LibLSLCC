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
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.Compilers.OpenSim.Visitors;
using LibLSLCC.LibraryData;

#endregion

namespace LibLSLCC.Compilers.OpenSim
{
    /// <summary>
    /// A compiler that converts LSL Syntax trees into CSharp code that is compatible with OpenSim's CSharp LSL runtime.
    /// </summary>
    public class LSLOpenSimCompiler
        // ReSharper restore InconsistentNaming
    {
        private readonly LSLOpenSimCompilerVisitor _visitor = new LSLOpenSimCompilerVisitor();


        /// <summary>
        /// Construct an <see cref="LSLOpenSimCompiler"/> using the specified settings object.
        /// </summary>
        /// <param name="settings"><see cref="LSLOpenSimCompilerSettings"/> to use.</param>
        /// <param name="libraryDataProvider">An <see cref="ILSLLibraryDataProvider"/> implementation.</param>
        /// <exception cref="ArgumentNullException">If 'settings' is null.</exception>
        public LSLOpenSimCompiler(ILSLLibraryDataProvider libraryDataProvider, LSLOpenSimCompilerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            LibraryDataProvider = libraryDataProvider;
            Settings = settings;
        }


        /// <summary>
        /// Construct an <see cref="LSLOpenSimCompiler"/> using the default settings and the provided <see cref="ILSLLibraryDataProvider"/> object.
        /// </summary>
        /// <param name="libraryDataProvider">An <see cref="ILSLLibraryDataProvider"/> implementation.</param>
        public LSLOpenSimCompiler(ILSLLibraryDataProvider libraryDataProvider)
        {
            LibraryDataProvider = libraryDataProvider;
            Settings = new LSLOpenSimCompilerSettings();
        }



        public ILSLLibraryDataProvider LibraryDataProvider
        {
            get { return _visitor.LibraryDataProvider; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _visitor.LibraryDataProvider = value;
            }
        }



        /// <summary>
        /// Settings for the OpenSim CSharp Compiler
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
        /// Compiles a syntax tree into OpenSim compatible CSharp code, writing the output to the specified TextWriter.
        /// </summary>
        /// <param name="compilationUnit">
        /// The top of the LSL Syntax tree to compile.
        /// This is returned from <see cref="LSLCodeValidator.Validate"/> or user implemented Code DOM.</param>
        /// <param name="textWriter">The text writer to write the generated code to.</param>
        public void Compile(ILSLCompilationUnitNode compilationUnit, TextWriter textWriter)
        {
            try
            {
                _visitor.WriteAndFlush(compilationUnit, textWriter, false);
            }
            finally
            {
                _visitor.Reset();
            }
        }
    }
}