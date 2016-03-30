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
using LibLSLCC.CodeValidator;
using LibLSLCC.CSharp;
using LibLSLCC.LibraryData;
using LibLSLCC.Settings;

#endregion

namespace LibLSLCC.Compilers
{
    /// <summary>
    ///     Settings for the <see cref="LSLOpenSimCompiler" /> class.
    /// </summary>
    public class LSLOpenSimCompilerSettings : SettingsBaseClass<LSLOpenSimCompilerSettings>
        // ReSharper restore InconsistentNaming
    {
        private CSharpFunctionCall _coOpTerminationFunctionCall = DefaultCoOpTerminationFunctionCall;
        private bool _generateClass;
        private ClassAccessibilityLevel _generatedClassAccessibility = ClassAccessibilityLevel.Public;
        private CSharpClassDeclarationName _generatedClassName;
        private CSharpNamespace _generatedClassNamespace;
        private MemberAccessibilityLevel _generatedConstructorAccessibility = MemberAccessibilityLevel.Public;
        private CSharpConstructorSignature _generatedConstructorSignature;
        private CSharpInheritanceList _generatedInheritanceList;

        private ObservableSettingsHashSet<CSharpNamespace> _generatedNamespaceImports =
            new ObservableSettingsHashSet<CSharpNamespace>();

        private bool _insertCoOpTerminationCalls;
        private string _scriptHeader;
        private bool _keysAreStrings = true;
        private bool _keyConstantsThatExpandAreStrings = true;
        private bool _keyElementsInListConstantsThatExpandAreStrings = true;


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
        /// Default is: "XEngineScript"
        /// </summary>
        public const string DefaultCoOpTerminationFunctionCall 
            = "opensim_reserved_CheckForCoopTermination()";


        /// <summary>
        ///     The call signature to use for co-op termination calls when <see cref="InsertCoOpTerminationCalls"/> is set to <c>true</c>. <para/>
        ///     Note that you should not add a semi-colon to the end of the signature string.  <para/>
        ///     If set to <c>null</c>, <see cref="DefaultCoOpTerminationFunctionCall"/> is used.
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
        ///     Whether or not to generate a class around the generated code, defaults to <c>false</c>.
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
        ///     The name of the namespace the class should reside in if <see cref="GenerateClass"/> is set to <c>true</c>. <para/>
        ///     Set to <c>null</c> if a the generated class should not be put in a namespace.
        /// </summary>
        public CSharpNamespace GeneratedClassNamespace
        {
            get { return _generatedClassNamespace; }
            set { SetField(ref _generatedClassNamespace, value, "GeneratedClassNamespace"); }
        }


        /// <summary>
        /// Default is: "XEngineScript"
        /// </summary>
        public const string DefaultGeneratedClassName = "XEngineScript";

        /// <summary>
        ///     The name of the class around the generated code if <see cref="GenerateClass"/> is set to <c>true</c>. <para/>
        ///     If set to <c>null</c>, <see cref="DefaultGeneratedClassName"/> will be used.
        /// </summary>
        public CSharpClassDeclarationName GeneratedClassName
        {
            get { return _generatedClassName; }
            set { SetField(ref _generatedClassName, value, "GeneratedClassName"); }
        }

        /// <summary>
        ///     The name of the class the generated class should inherit from if <see cref="GenerateClass"/> is set to <c>true</c>.
        ///     Set to <c>null</c> or <see cref="string.Empty"/> if the generated class should not derive from anything.
        /// </summary>
        public CSharpInheritanceList GeneratedInheritanceList
        {
            get { return _generatedInheritanceList; }
            set { SetField(ref _generatedInheritanceList, value, "GeneratedInheritanceList"); }
        }



        /// <summary>
        /// Default is: "(System.Threading.WaitHandle coopSleepHandle) : base(coopSleepHandle)"
        /// </summary>
        public const string DefaultGeneratedConstructorSignature =
            "(System.Threading.WaitHandle coopSleepHandle) : base(coopSleepHandle)";

        /// <summary>
        ///     The constructor signature to be inserted into the generated class if <see cref="GenerateClass"/> is set to <c>true</c>. <para/>
        ///     Example: "(int parameter) : base(parameter)"  <para/>
        ///     If set to <c>null</c>, <see cref="DefaultGeneratedConstructorSignature"/> will be used.
        /// </summary>
        public CSharpConstructorSignature GeneratedConstructorSignature
        {
            get { return _generatedConstructorSignature; }
            set { SetField(ref _generatedConstructorSignature, value, "GeneratedConstructorSignature"); }
        }

        /// <summary>
        ///     The accessibility level of the class if <see cref="GenerateClass"/> is set to <c>true</c>.
        ///     Defaults to <see cref="ClassAccessibilityLevel.Default" />.
        /// </summary>
        public ClassAccessibilityLevel GeneratedClassAccessibility
        {
            get { return _generatedClassAccessibility; }
            set { SetField(ref _generatedClassAccessibility, value, "GeneratedClassAccessibility"); }
        }

        /// <summary>
        ///     The accessibility level of the constructor in the generated class if <see cref="GenerateClass"/> is set to <c>true</c>.
        ///     Defaults to <see cref="MemberAccessibilityLevel.Public" />.
        /// </summary>
        public MemberAccessibilityLevel GeneratedConstructorAccessibility
        {
            get { return _generatedConstructorAccessibility; }
            set { SetField(ref _generatedConstructorAccessibility, value, "GeneratedConstructorAccessibility"); }
        }

        /// <summary>
        ///     String content to be placed at the very beginning of the generated script, use this to place comments. <para/>
        ///     If it's <c>null</c> or empty then the compiler ignores it.
        /// </summary>
        public string ScriptHeader
        {
            get { return _scriptHeader; }
            set { SetField(ref _scriptHeader, value, "ScriptHeader"); }
        }


        /// <summary>
        ///     If <c>true</c>; user defined variables, function/event parameters and function return types of type 'key' will be
        ///     declared as the runtime 'string' type in generated code. <para/>
        ///     
        ///     'key' type expressions will still be boxed to the runtime key type when they are used in the condition area of a branch or loop statement. <para/>
        ///     
        ///     The default value is <c>true</c>.
        /// </summary>
        public bool KeysAreStrings
        {
            get { return _keysAreStrings; }
            set { SetField(ref _keysAreStrings, value, "KeysAreStrings"); }
        }

        /// <summary>
        ///     If <c>true</c>; library constants that are declared as <see cref="LSLType.Key"/> where <see cref="LSLLibraryConstantSignature.Expand"/> is <c>true</c>
        ///     will expand into the runtime 'string' type instead of the 'key' type. <para/>
        /// 
        ///     The default value is <c>true</c>.
        /// </summary>
        public bool KeyConstantsThatExpandAreStrings
        {
            get { return _keyConstantsThatExpandAreStrings; }
            set { SetField(ref _keyConstantsThatExpandAreStrings, value, "KeyConstantsThatExpandAreStrings"); }
        }

        /// <summary>
        ///     If <c>true</c>; library constants with the type 'list' where <see cref="LSLLibraryConstantSignature.Expand"/> is <c>true</c> will have 'key' type elements
        ///     expanded to the runtime 'string' type instead of the 'key' type.  <para/>
        ///     
        ///     The default value is <c>true</c>.
        /// </summary>
        public bool KeyElementsInListConstantsThatExpandAreStrings
        {
            get { return _keyElementsInListConstantsThatExpandAreStrings; }
            set { SetField(ref _keyElementsInListConstantsThatExpandAreStrings, value, "KeyElementsInListConstantsThatExpandAreStrings"); }
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
        public static LSLOpenSimCompilerSettings CreateOpenSimServerSide()
        {
            var compilerSettings = new LSLOpenSimCompilerSettings()
            {
                GenerateClass = true,
                GeneratedClassNamespace = "SecondLife",
                GeneratedClassName = DefaultGeneratedClassName,
                GeneratedClassAccessibility = ClassAccessibilityLevel.Public,
                GeneratedInheritanceList = "OpenSim.Region.ScriptEngine.XEngine.ScriptBase.XEngineScriptBase",
                GeneratedConstructorAccessibility = MemberAccessibilityLevel.Public,
                GeneratedConstructorSignature = DefaultGeneratedConstructorSignature
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
        public static LSLOpenSimCompilerSettings CreateLibLSLCCRuntime()
        {
            var compilerSettings = new LSLOpenSimCompilerSettings()
            {
                GenerateClass = true,
                GeneratedClassNamespace = "SecondLife",
                GeneratedClassName = "LSLScript",
                GeneratedClassAccessibility = ClassAccessibilityLevel.Public,
                GeneratedInheritanceList = "LSLScriptBase",
                GeneratedConstructorAccessibility = MemberAccessibilityLevel.Public,
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
        public static LSLOpenSimCompilerSettings CreateOpenSimClientSide()
        {
            var compilerSettings = new LSLOpenSimCompilerSettings();
            return compilerSettings;
        }
    }
}