#region FileInfo

// 
// File: LSLOpenSimCompilerSettings.cs
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
using LibLSLCC.CSharp;
using LibLSLCC.LibraryData;
using LibLSLCC.Settings;

#endregion

namespace LibLSLCC.Compilers.OpenSim
{
    /// <summary>
    ///     Settings for the <see cref="LSLOpenSimCompiler" /> class.
    /// </summary>
    public class LSLOpenSimCompilerSettings : SettingsBaseClass<LSLOpenSimCompilerSettings>
        // ReSharper restore InconsistentNaming
    {
        private CSharpFunctionCall _coOpTerminationFunctionCall = "opensim_reserved_CheckForCoopTermination()";
        private bool _generateClass;
        private ClassAccessibilityLevel _generatedClassAccessibility;
        private CSharpClassDeclarationName _generatedClassName;
        private CSharpNamespace _generatedClassNamespace;
        private MemberAccessibilityLevel _generatedConstructorAccessibility;
        private CSharpConstructorSignature _generatedConstructorSignature;
        private CSharpInheritanceList _generatedInheritanceList;

        private ObservableSettingsHashSet<CSharpNamespace> _generatedNamespaceImports =
            new ObservableSettingsHashSet<CSharpNamespace>();

        private bool _insertCoOpTerminationCalls;
        private string _scriptHeader;


        /// <summary>
        ///     Construct an <see cref="LSLOpenSimCompilerSettings" /> object that uses a given
        ///     <see cref="ILSLLibraryDataProvider" /> implementation
        ///     to provide library data to the compiler.
        /// </summary>
        // ReSharper disable once EmptyConstructor
        public LSLOpenSimCompilerSettings()
        {
        }


        /// <summary>
        ///     The call signature to use for co-op termination calls when InsertCoOpTerminationCalls is set to true
        ///     it defaults to "opensim_reserved_CheckForCoopTermination()"
        ///     Note that you should not add a semi-colon to the end of the signature string.
        /// </summary>
        public CSharpFunctionCall CoOpTerminationFunctionCall
        {
            get { return _coOpTerminationFunctionCall; }
            set { SetField(ref _coOpTerminationFunctionCall, value, "CoOpTerminationFunctionCall"); }
        }

        /// <summary>
        ///     If this is set to true, the function signature string specified by CoOpTerminationFunctionCall will be inserted
        ///     (called) at the top of user defined functions, state events, for loops, while loops, do while loops and immediately
        ///     after defined labels in generated code.
        /// </summary>
        public bool InsertCoOpTerminationCalls
        {
            get { return _insertCoOpTerminationCalls; }
            set { SetField(ref _insertCoOpTerminationCalls, value, "InsertCoOpTerminationCalls"); }
        }

        /// <summary>
        ///     Whether or not to generate a class around the generated code, defaults to false.
        /// </summary>
        public bool GenerateClass
        {
            get { return _generateClass; }
            set { SetField(ref _generateClass, value, "GenerateClass"); }
        }

        /// <summary>
        ///     This set should contain all the namespace's that the generated code should import
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableSettingsHashSet<CSharpNamespace> GeneratedNamespaceImports
        {
            get { return _generatedNamespaceImports; }
            set { SetField(ref _generatedNamespaceImports, value, "GeneratedNamespaceImports"); }
        }

        /// <summary>
        ///     The name of the namespace the class should reside in if GenerateClass is set to true.
        /// </summary>
        public CSharpNamespace GeneratedClassNamespace
        {
            get { return _generatedClassNamespace; }
            set { SetField(ref _generatedClassNamespace, value, "GeneratedClassNamespace"); }
        }

        /// <summary>
        ///     The name of the class around the generated code if GeneratedClass is set to true.
        /// </summary>
        public CSharpClassDeclarationName GeneratedClassName
        {
            get { return _generatedClassName; }
            set { SetField(ref _generatedClassName, value, "GeneratedClassName"); }
        }

        /// <summary>
        ///     The name of the class the generated class should inherit from if GenerateClass is set to true, or null/empty if you
        ///     don't want the
        ///     generated class to derive from anything.
        /// </summary>
        public CSharpInheritanceList GeneratedInheritanceList
        {
            get { return _generatedInheritanceList; }
            set { SetField(ref _generatedInheritanceList, value, "GeneratedInheritanceList"); }
        }

        /// <summary>
        ///     The constructor signature to be inserted into the generated class if GenerateClass is set to true, this string is
        ///     copied verbatim.
        ///     example: (int parameter) : base(parameter)
        /// </summary>
        public CSharpConstructorSignature GeneratedConstructorSignature
        {
            get { return _generatedConstructorSignature; }
            set { SetField(ref _generatedConstructorSignature, value, "GeneratedConstructorSignature"); }
        }

        /// <summary>
        ///     The accessibility level of the class if GenerateClass is set to true.
        ///     defaults to <see cref="ClassAccessibilityLevel.Default" />.
        /// </summary>
        public ClassAccessibilityLevel GeneratedClassAccessibility
        {
            get { return _generatedClassAccessibility; }
            set { SetField(ref _generatedClassAccessibility, value, "GeneratedClassAccessibility"); }
        }

        /// <summary>
        ///     The accessibility of the constructor signature to be inserted into the generated class if GenerateClass is set to
        ///     true.
        ///     defaults to <see cref="MemberAccessibilityLevel.Public" />.
        /// </summary>
        public MemberAccessibilityLevel GeneratedConstructorAccessibility
        {
            get { return _generatedConstructorAccessibility; }
            set { SetField(ref _generatedConstructorAccessibility, value, "GeneratedConstructorAccessibility"); }
        }

        /// <summary>
        ///     String content to be placed at the very beginning of the generated script, use this to place comments.
        ///     If its null or empty then the compiler ignores it.
        /// </summary>
        public string ScriptHeader
        {
            get { return _scriptHeader; }
            set { SetField(ref _scriptHeader, value, "ScriptHeader"); }
        }


        /// <summary>
        ///     Create a settings object that targets OpenSim's server side runtime.  The settings object will be setup to generate
        ///     a class named
        ///     "XEngineScript" that derives from "OpenSim.Region.ScriptEngine.XEngine.ScriptBase.XEngineScriptBase" and contains
        ///     all the generated code.
        ///     This class will be put in the namespace "SecondLife" and "OpenSim.Region.ScriptEngine.Shared" will be added to the
        ///     namespace imports.
        /// </summary>
        /// <returns>The generated <see cref="LSLOpenSimCompilerSettings" /> settings object.</returns>
        public static LSLOpenSimCompilerSettings OpenSimServerSideDefault()
        {
            var compilerSettings = new LSLOpenSimCompilerSettings()
            {
                GenerateClass = true,
                GeneratedClassNamespace = "SecondLife",
                GeneratedClassName = "XEngineScript",
                GeneratedInheritanceList = "OpenSim.Region.ScriptEngine.XEngine.ScriptBase.XEngineScriptBase",
                GeneratedConstructorSignature = "(System.Threading.WaitHandle coopSleepHandle) : base(coopSleepHandle)"
            };


            compilerSettings.GeneratedNamespaceImports.Add("OpenSim.Region.ScriptEngine.Shared");
            compilerSettings.GeneratedNamespaceImports.Add("System.Collections.Generic");

            return compilerSettings;
        }


        /// <summary>
        ///     Create a settings object that targets LibLSLCC's LSL Runtime.  (Which is not implemented yet)
        ///     The settings object will be setup to generate a class named "LSLScript" that derives from "LSLScriptBase".
        ///     This class will be put into the namespace "SecondLife" and "LibLSLCC.LSLRuntime" will be added to the namespace
        ///     imports.
        /// </summary>
        /// <returns>The generated <see cref="LSLOpenSimCompilerSettings" /> settings object.</returns>
        public static LSLOpenSimCompilerSettings LibLSLCCRuntimeDefault()
        {
            var compilerSettings = new LSLOpenSimCompilerSettings()
            {
                GenerateClass = true,
                GeneratedClassNamespace = "SecondLife",
                GeneratedClassName = "LSLScript",
                GeneratedInheritanceList = "LSLScriptBase",
                GeneratedConstructorSignature = "() : base()"
            };

            //compilerSettings.GeneratedNamespaceImports.Add("OpenSim.Region.ScriptEngine.Shared");
            //compilerSettings.GeneratedNamespaceImports.Add("System.Collections.Generic");

            return compilerSettings;
        }


        /// <summary>
        ///     Create a settings object that will settings that will make the <see cref="LSLOpenSimCompiler" /> generate code that
        ///     is up-loadable
        ///     to an OpenSim server via the in viewer editor.  (This only works if the OpenSim server has CSharp scripting
        ///     enabled)
        /// </summary>
        /// <returns>The generated <see cref="LSLOpenSimCompilerSettings" /> settings object.</returns>
        public static LSLOpenSimCompilerSettings OpenSimClientUploadable()
        {
            var compilerSettings = new LSLOpenSimCompilerSettings();
            return compilerSettings;
        }
    }
}