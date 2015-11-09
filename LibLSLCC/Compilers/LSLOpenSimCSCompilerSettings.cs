using System.Collections.Generic;
using LibLSLCC.Collections;
using LibLSLCC.LibraryData;
using LibLSLCC.Utility;

namespace LibLSLCC.Compilers
{
    /// <summary>
    /// Settings for the <see cref="LSLOpenSimCSCompiler"/> class.
    /// </summary>
    public class LSLOpenSimCSCompilerSettings : LSLSettingsBase
        // ReSharper restore InconsistentNaming
    {
        private string _coOpTerminationFunctionCall = "opensim_reserved_CheckForCoopTermination()";
        private bool _insertCoOpTerminationCalls;
        private bool _generateClass;
        private string _generatedClassNamespace;
        private string _generatedClassName;
        private string _generatedClassInherit;
        private string _generatedConstructorSignature;
        private AccessibilityLevel _generatedConstructorAccessibility;
        private ObservableSet<string> _generatedNamespaceImports;
        private string _scriptHeader;



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

        }

        /// <summary>
        /// The call signature to use for co-op termination calls when InsertCoOpTerminationCalls is set to true
        /// it defaults to "opensim_reserved_CheckForCoopTermination()"
        /// 
        /// Note that you should not add a semi-colon to the end of the signature string.
        /// </summary>
        public string CoOpTerminationFunctionCall
        {
            get { return _coOpTerminationFunctionCall; }
            set { SetField(ref _coOpTerminationFunctionCall,value, "CoOpTerminationFunctionCall"); }
        }

        /// <summary>
        /// If this is set to true, the function signature string specified by CoOpTerminationFunctionCall will be inserted
        /// (called) at the top of user defined functions, state events, for loops, while loops, do while loops and immediately
        /// after defined labels in generated code.
        /// </summary>
        public bool InsertCoOpTerminationCalls
        {
            get { return _insertCoOpTerminationCalls; }
            set { SetField(ref _insertCoOpTerminationCalls, value, "InsertCoOpTerminationCalls"); }
        }

        /// <summary>
        /// Whether or not to generate a class around the generated code, defaults to false.
        /// </summary>
        public bool GenerateClass
        {
            get { return _generateClass; }
            set { SetField(ref _generateClass, value, "GenerateClass"); }
        }

        /// <summary>
        /// This hashed set should contain all the namespace's that the generated code should import
        /// </summary>
        public ObservableSet<string> GeneratedNamespaceImports
        {
            get { return _generatedNamespaceImports; }
            set { SetField(ref _generatedNamespaceImports, value, "GeneratedNamespaceImports"); }
        }

        /// <summary>
        /// The name of the namespace the class should reside in if GenerateClass is set to true.
        /// </summary>
        public string GeneratedClassNamespace
        {
            get { return _generatedClassNamespace; }
            set { SetField(ref _generatedClassNamespace, value, "GeneratedClassNamespace"); }
        }

        /// <summary>
        /// The name of the class around the generated code if GeneratedClass is set to true.
        /// </summary>
        public string GeneratedClassName
        {
            get { return _generatedClassName; }
            set { SetField(ref _generatedClassName, value, "GeneratedClassName"); }
        }

        /// <summary>
        /// The name of the class the generated class should inherit from if GenerateClass is set to true, or null/empty if you don't want the
        /// generated class to derive from anything.
        /// </summary>
        public string GeneratedClassInherit
        {
            get { return _generatedClassInherit; }
            set { SetField(ref _generatedClassInherit, value, "GeneratedClassInherit"); }
        }


        /// <summary>
        /// The constructor signature to be inserted into the generated class if GenerateClass is set to true, this string is copied verbatim.
        /// example: (int parameter) : base(parameter)
        /// </summary>
        public string GeneratedConstructorSignature
        {
            get { return _generatedConstructorSignature; }
            set { SetField(ref _generatedConstructorSignature, value, "GeneratedConstructorSignature"); }
        }

        /// <summary>
        /// The accessibility of the constructor signature to be inserted into the generated class if GenerateClass is set to true.
        /// defaults to AccessibilityLevel.Public.
        /// </summary>
        public AccessibilityLevel GeneratedConstructorAccessibility
        {
            get { return _generatedConstructorAccessibility; }
            set { SetField(ref _generatedConstructorAccessibility, value, "GeneratedConstructorAccessibility"); }
        }


        /// <summary>
        /// String content to be placed at the very beginning of the generated script, use this to place comments.
        /// If its null or empty then the compiler ignores it.
        /// </summary>
        public string ScriptHeader
        {
            get { return _scriptHeader; }
            set { SetField(ref _scriptHeader, value, "ScriptHeader"); }
        }


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
                GeneratedNamespaceImports = new ObservableSet<string> { "OpenSim.Region.ScriptEngine.Shared","System.Collections.Generic" }
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
                GeneratedNamespaceImports = new ObservableSet<string> { "LibLSLCC.LSLRuntime", "System.Collections.Generic" }
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
}