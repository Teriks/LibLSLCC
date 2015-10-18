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
using System.Collections.Generic;
using System.IO;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.Compilers.Visitors;

#endregion

namespace LibLSLCC.Compilers
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Settings for the LSLOpenSimCSCompiler class.
    /// </summary>
    public class LSLOpenSimCSCompilerSettings
        // ReSharper restore InconsistentNaming
    {

        /// <summary>
        /// Construct an LSLOpenSimCSCompilerSettings object that uses a given ILSLMainLibraryDataProvider implementation
        /// to provide library data to the compiler.
        /// </summary>
        /// <param name="libraryData">The ILSLMainLibraryDataProvider implementation to use.</param>
        public LSLOpenSimCSCompilerSettings(ILSLMainLibraryDataProvider libraryData)
        {
            GenerateClass = false;
            GeneratedClassName = null;
            GeneratedClassInherit = null;
            GeneratedConstructorDefinition = null;
            LibraryData = libraryData;
            InsertCoOpTerminationCalls = false;
            CoOpTerminationFunctionCall = "opensim_reserved_CheckForCoopTermination()";
        }

        /// <summary>
        /// The call signature to use for co-op termination calls when InsertCoOpTerminationCalls is set to true
        /// it defaults to "opensim_reserved_CheckForCoopTermination()"
        /// 
        /// Note that you should not add a semi-colon to the end of the signature string.
        /// </summary>
        public string CoOpTerminationFunctionCall { get; set; }

        /// <summary>
        /// If this is set to true, the function signature string specified by CoOpTerminationFunctionCall will be inserted
        /// (called) at the top of user defined functions, state events, for loops, while loops, do while loops and immediately
        /// after defined labels in generated code.
        /// </summary>
        public bool InsertCoOpTerminationCalls { get; set; }

        /// <summary>
        /// Whether or not to generate a class around the generated code, defaults to false.
        /// </summary>
        public bool GenerateClass { get; set; }

        /// <summary>
        /// This hashed set should contain all the namespace's that the generated code should import
        /// </summary>
        public HashSet<string> GeneratedNamespaceImports { get; set; }

        /// <summary>
        /// The name of the namespace the class should reside in if GenerateClass is set to true.
        /// </summary>
        public string GenerateClassNamespace { get; set; }

        /// <summary>
        /// The name of the class around the generated code if GeneratedClass is set to true.
        /// </summary>
        public string GeneratedClassName { get; set; }

        /// <summary>
        /// The name of the class the generated class should inherit from if GenerateClass is set to true, or null/empty if you don't want the
        /// generated class to derive from anything.
        /// </summary>
        public string GeneratedClassInherit { get; set; }


        /// <summary>
        /// The constructor definition to be inserted into the generated class if GenerateClass is set to true, this string is copied verbatim.
        /// </summary>
        public string GeneratedConstructorDefinition { get; set; }

        /// <summary>
        /// The library data provider to use for the compilation process, it is used to lookup library function calls
        /// and determine if the use of ModInvoke is necessary to invoke a particular function from the source script.
        /// </summary>
        public ILSLMainLibraryDataProvider LibraryData { get; private set; }


        /// <summary>
        /// Create a settings object that targets OpenSim's server side runtime.  The settings object will be setup to generate a class named
        /// "XEngineScript" that derives from "OpenSim.Region.ScriptEngine.XEngine.ScriptBase.XEngineScriptBase" and contains all the generated code.
        /// This class will be put in the namespace "SecondLife" and "OpenSim.Region.ScriptEngine.Shared" will be added to the namespace imports.
        /// </summary>
        /// <param name="libraryData">The ILSLMainLibraryDataProvider implementation to use.</param>
        /// <returns>The generated LSLOpenSimCSCompilerSettings settings object.</returns>
        public static LSLOpenSimCSCompilerSettings OpenSimServerSideDefault(ILSLMainLibraryDataProvider libraryData)
        {
            var compilerSettings = new LSLOpenSimCSCompilerSettings(libraryData)
            {
                GenerateClass = true,
                GenerateClassNamespace = "SecondLife",
                GeneratedClassName = "XEngineScript",
                GeneratedClassInherit = "OpenSim.Region.ScriptEngine.XEngine.ScriptBase.XEngineScriptBase",
                GeneratedConstructorDefinition = "public XEngineScript(System.Threading.WaitHandle coopSleepHandle) : base(coopSleepHandle) {}",
                GeneratedNamespaceImports = new HashSet<string> { "OpenSim.Region.ScriptEngine.Shared","System.Collections.Generic" }
            };

            return compilerSettings;
        }

        /// <summary>
        /// Create a settings object that targets LibLSLCC's LSL Runtime.  (Which is not implemented yet)
        /// The settings object will be setup to generate a class named "LSLScript" that derives from "LSLScriptBase".
        /// This class will be put into the namespace "SecondLife" and "LibLSLCC.LSLRuntime" will be added to the namespace imports.
        /// </summary>
        /// <param name="libraryData">The ILSLMainLibraryDataProvider implementation to use.</param>
        /// <returns>The generated LSLOpenSimCSCompilerSettings settings object.</returns>
        public static LSLOpenSimCSCompilerSettings LibLSLCCRuntimeDefault(ILSLMainLibraryDataProvider libraryData)
        {
            var compilerSettings = new LSLOpenSimCSCompilerSettings(libraryData)
            {
                GenerateClass = true,
                GenerateClassNamespace = "SecondLife",
                GeneratedClassName = "LSLScript",
                GeneratedClassInherit = "LSLScriptBase",
                GeneratedConstructorDefinition = "public LSLScript() : base() {}",
                GeneratedNamespaceImports = new HashSet<string> { "LibLSLCC.LSLRuntime", "System.Collections.Generic" }
            };

            return compilerSettings;
        }


        /// <summary>
        /// Create a settings object that will settings that will make the LSLOpenSimCSCompiler generate code that is up-loadable
        /// to an OpenSim server via the in viewer editor.  (This only works if the OpenSim server has CSharp scripting enabled)
        /// </summary>
        /// <param name="libraryData">The ILSLMainLibraryDataProvider implementation to use.</param>
        /// <returns>The generated LSLOpenSimCSCompilerSettings settings object.</returns>
        public static LSLOpenSimCSCompilerSettings OpenSimClientUploadable(ILSLMainLibraryDataProvider libraryData)
        {
            var compilerSettings = new LSLOpenSimCSCompilerSettings(libraryData);
            return compilerSettings;
        }
    }




    // ReSharper disable InconsistentNaming
    /// <summary>
    /// A compiler that converts LSL Syntax trees into CSharp code that is compatible with OpenSim's CSharp LSL runtime.
    /// </summary>
    public class LSLOpenSimCSCompiler
        // ReSharper restore InconsistentNaming
    {
        private readonly LSLOpenSimCSCompilerVisitor _visitor = new LSLOpenSimCSCompilerVisitor();


        /// <summary>
        /// Construct an LSLOpenSimCSCompiler using the specified settings object.
        /// </summary>
        /// <param name="settings">LSLOpenSimCSCompilerSettings to use.</param>
        /// <exception cref="ArgumentNullException">If 'settings' is null.</exception>
        public LSLOpenSimCSCompiler(LSLOpenSimCSCompilerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            Settings = settings;
        }

        /// <summary>
        /// Construct an LSLOpenSimCSCompiler using the default settings and the provided ILSLMainLibraryDataProvider object.
        /// </summary>
        /// <param name="libraryData">An ILSLMainLibraryDataProvider implementation.</param>
        public LSLOpenSimCSCompiler(ILSLMainLibraryDataProvider libraryData)
        {
            Settings = new LSLOpenSimCSCompilerSettings(libraryData);
        }

        /// <summary>
        /// Settings for the OpenSim CSharp Compiler
        /// </summary>
        /// <exception cref="ArgumentNullException">If the value is set to null.</exception>
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


        /// <summary>
        /// Compiles a syntax tree into OpenSim compatible CSharp code, writing the output to the specified TextWriter.
        /// </summary>
        /// <param name="compilationUnit">
        /// The top of the LSL Syntax tree to compile.
        /// This is returned from LSLCodeValidator.Validate or user implemented Code DOM.</param>
        /// <see cref="LSLCodeValidator.Validate"/>
        /// <param name="textWriter">The text writer to write the generated code to.</param>
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