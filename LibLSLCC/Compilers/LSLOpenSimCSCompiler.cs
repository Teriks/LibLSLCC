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
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.Compilers.Visitors;
using LibLSLCC.LibraryData;

#endregion

namespace LibLSLCC.Compilers
{

    


    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Settings for the <see cref="LSLOpenSimCSCompiler"/> class.
    /// </summary>
    public class LSLOpenSimCSCompilerSettings
        // ReSharper restore InconsistentNaming
    {

        public enum AccessibilityLevel
        {
            Public,
            Private,
            Internal,
            Protected,
        }


        /// <summary>
        /// Construct an <see cref="LSLOpenSimCSCompilerSettings"/> object that uses a given <see cref="ILSLLibraryDataProvider"/> implementation
        /// to provide library data to the compiler.
        /// </summary>
        public LSLOpenSimCSCompilerSettings()
        {
            GenerateClass = false;
            GeneratedClassName = null;
            GeneratedClassInherit = null;
            GeneratedConstructorSignature = null;
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
        public string GeneratedClassNamespace { get; set; }

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
        /// The constructor signature to be inserted into the generated class if GenerateClass is set to true, this string is copied verbatim.
        /// example: (int parameter) : base(parameter)
        /// </summary>
        public string GeneratedConstructorSignature { get; set; }

        /// <summary>
        /// The accessibility of the constructor signature to be inserted into the generated class if GenerateClass is set to true.
        /// defaults to AccessibilityLevel.Public.
        /// </summary>
        public AccessibilityLevel GeneratedConstructorAccessibility { get; set; }




        /// <summary>
        /// String content to be placed at the very beginning of the generated script, use this to place comments.
        /// If its null or empty then the compiler ignores it.
        /// </summary>
        public string ScriptHeader { get; set; }


        /// <summary>
        /// Create a settings object that targets OpenSim's server side runtime.  The settings object will be setup to generate a class named
        /// "XEngineScript" that derives from "OpenSim.Region.ScriptEngine.XEngine.ScriptBase.XEngineScriptBase" and contains all the generated code.
        /// This class will be put in the namespace "SecondLife" and "OpenSim.Region.ScriptEngine.Shared" will be added to the namespace imports.
        /// </summary>
        /// <returns>The generated <see cref="LSLOpenSimCSCompilerSettings"/> settings object.</returns>
        public static LSLOpenSimCSCompilerSettings OpenSimServerSideDefault()
        {
            var compilerSettings = new LSLOpenSimCSCompilerSettings()
            {
                GenerateClass = true,
                GeneratedClassNamespace = "SecondLife",
                GeneratedClassName = "XEngineScript",
                GeneratedClassInherit = "OpenSim.Region.ScriptEngine.XEngine.ScriptBase.XEngineScriptBase",
                GeneratedConstructorSignature = "(System.Threading.WaitHandle coopSleepHandle) : base(coopSleepHandle)",
                GeneratedNamespaceImports = new HashSet<string> { "OpenSim.Region.ScriptEngine.Shared","System.Collections.Generic" }
            };

            return compilerSettings;
        }

        /// <summary>
        /// Create a settings object that targets LibLSLCC's LSL Runtime.  (Which is not implemented yet)
        /// The settings object will be setup to generate a class named "LSLScript" that derives from "LSLScriptBase".
        /// This class will be put into the namespace "SecondLife" and "LibLSLCC.LSLRuntime" will be added to the namespace imports.
        /// </summary>
        /// <returns>The generated <see cref="LSLOpenSimCSCompilerSettings"/> settings object.</returns>
        public static LSLOpenSimCSCompilerSettings LibLSLCCRuntimeDefault()
        {
            var compilerSettings = new LSLOpenSimCSCompilerSettings()
            {
                GenerateClass = true,
                GeneratedClassNamespace = "SecondLife",
                GeneratedClassName = "LSLScript",
                GeneratedClassInherit = "LSLScriptBase",
                GeneratedConstructorSignature = "() : base()",
                GeneratedNamespaceImports = new HashSet<string> { "LibLSLCC.LSLRuntime", "System.Collections.Generic" }
            };

            return compilerSettings;
        }


        /// <summary>
        /// Create a settings object that will settings that will make the <see cref="LSLOpenSimCSCompiler"/> generate code that is up-loadable
        /// to an OpenSim server via the in viewer editor.  (This only works if the OpenSim server has CSharp scripting enabled)
        /// </summary>
        /// <returns>The generated <see cref="LSLOpenSimCSCompilerSettings"/> settings object.</returns>
        public static LSLOpenSimCSCompilerSettings OpenSimClientUploadable()
        {
            var compilerSettings = new LSLOpenSimCSCompilerSettings();
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
        /// Construct an <see cref="LSLOpenSimCSCompiler"/> using the specified settings object.
        /// </summary>
        /// <param name="settings"><see cref="LSLOpenSimCSCompilerSettings"/> to use.</param>
        /// <param name="libraryDataProvider">An <see cref="ILSLLibraryDataProvider"/> implementation.</param>
        /// <exception cref="ArgumentNullException">If 'settings' is null.</exception>
        public LSLOpenSimCSCompiler(ILSLLibraryDataProvider libraryDataProvider, LSLOpenSimCSCompilerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            LibraryDataProvider = libraryDataProvider;
            Settings = settings;
        }


        /// <summary>
        /// Construct an <see cref="LSLOpenSimCSCompiler"/> using the default settings and the provided <see cref="ILSLLibraryDataProvider"/> object.
        /// </summary>
        /// <param name="libraryDataProvider">An <see cref="ILSLLibraryDataProvider"/> implementation.</param>
        public LSLOpenSimCSCompiler(ILSLLibraryDataProvider libraryDataProvider)
        {
            LibraryDataProvider = libraryDataProvider;
            Settings = new LSLOpenSimCSCompilerSettings();
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
        /// This is returned from <see cref="LSLCodeValidator.Validate"/> or user implemented Code DOM.</param>
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