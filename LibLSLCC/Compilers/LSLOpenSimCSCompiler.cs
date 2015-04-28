#region

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