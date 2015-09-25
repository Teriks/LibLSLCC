#region FileInfo

// 
// File: LSLOpenSimCSCompiler.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:25 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
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