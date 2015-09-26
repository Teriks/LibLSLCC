#region FileInfo
// 
// File: LSLOpenSimCSCompiler.cs
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
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.Compilers.Visitors;

#endregion

namespace LibLSLCC.Compilers
{
    // ReSharper disable InconsistentNaming
    public class LSLOpenSimCSCompilerSettings
        // ReSharper restore InconsistentNaming
    {
        public LSLOpenSimCSCompilerSettings(ILSLMainLibraryDataProvider libraryData)
        {
            GenerateClass = false;
            GeneratedClassName = null;
            GeneratedClassInherit = null;
            GeneratedConstructorDefinition = null;
            LibraryData = libraryData;
        }

        public bool GenerateClass { get; set; }
        public string GeneratedUsingSection { get; set; }
        public string GenerateClassNamespace { get; set; }
        public string GeneratedClassName { get; set; }
        public string GeneratedClassInherit { get; set; }
        public string GeneratedConstructorDefinition { get; set; }
        public ILSLMainLibraryDataProvider LibraryData { get; private set; }

        public static LSLOpenSimCSCompilerSettings OpenSimServerSideDefault(ILSLMainLibraryDataProvider libraryData)
        {
            var compilerSettings = new LSLOpenSimCSCompilerSettings(libraryData)
            {
                GenerateClass = true,
                GenerateClassNamespace = "SecondLife",
                GeneratedClassName = "Script",
                GeneratedClassInherit = "OpenSim.Region.ScriptEngine.Shared.ScriptBase.ScriptBaseClass",
                GeneratedConstructorDefinition = "public Script() : base() {}",
                GeneratedUsingSection = "using OpenSim.Region.ScriptEngine.Shared;\r\nusing System.Collections.Generic;"
            };

            return compilerSettings;
        }

        public static LSLOpenSimCSCompilerSettings LibLSLCCRuntimeDefault(ILSLMainLibraryDataProvider libraryData)
        {
            var compilerSettings = new LSLOpenSimCSCompilerSettings(libraryData)
            {
                GenerateClass = true,
                GenerateClassNamespace = "SecondLife",
                GeneratedClassName = "LSLScript",
                GeneratedClassInherit = "LSLScriptBase",
                GeneratedConstructorDefinition = "public LSLScript() : base() {}"
            };

            return compilerSettings;
        }

        public static LSLOpenSimCSCompilerSettings OpenSimClientUploadable(ILSLMainLibraryDataProvider libraryData)
        {
            var compilerSettings = new LSLOpenSimCSCompilerSettings(libraryData);
            return compilerSettings;
        }
    }


    // ReSharper disable InconsistentNaming
    public class LSLOpenSimCSCompiler
        // ReSharper restore InconsistentNaming
    {
        private readonly LSLOpenSimCSCompilerVisitor _visitor = new LSLOpenSimCSCompilerVisitor();

        public LSLOpenSimCSCompiler(LSLOpenSimCSCompilerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            Settings = settings;
        }

        public LSLOpenSimCSCompiler(ILSLMainLibraryDataProvider libraryData)
        {
            Settings = new LSLOpenSimCSCompilerSettings(libraryData);
        }

        public LSLOpenSimCSCompilerSettings Settings
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

        public void Compile(ILSLCompilationUnitNode compilationUnit, TextWriter textWriter)
        {
            try
            {
                _visitor.WriteAndFlush(compilationUnit, textWriter, false);
            }
            catch (LSLCompilerInternalException)
            {
                _visitor.Reset();
                throw;
            }
        }
    }
}