#region FileInfo

// 
// File: Program.cs
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
using System.Linq;
using System.Reflection;
using System.Text;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.Compilers;
using LibLSLCC.LibraryData;
using LibLSLCC.LibraryData.Reflection;

#endregion

namespace lslcc
{
    internal class Program
    {
        private static string InternalErrorMessage = "Please create a bug report with the code that caused this message, and the message itself.";


        private const string ClientSideScriptCompilerHeader =
            @"//c#
/** 
*  Do not remove //c# from the first line of this script.
*
*  This is OpenSim CSharp code, CSharp scripting must be enabled on the server to run.
*
*  Please note this script does not support being reset, because a constructor was not generated.
*  Compile using the server side script option to generate a script constructor.
*
*  This code will run on an unmodified OpenSim server, however script resets will not reset global variables,
*  and OpenSim will be unable to save the state of this script as its global variables are created in an object container.
*
*/ 
";


        private const string ServerSideScriptCompilerHeader =
            @"//c#-raw
/** 
*  Do not remove //c#-raw from the first line of this script.
*
*  This is OpenSim CSharp code, CSharp scripting must be enabled on the server to run.
*
*  This is a server side script.  It constitutes a fully generated script class that
*  will be sent to the CSharp compiler in OpenSim.  This code supports script resets.
*
*  This script is meant to upload compatible with the LibLSLCC OpenSim fork.
*
*  If you are running a version of OpenSim with the LibLSLCC compiler enabled, you must add 'csraw'
*  to the allowed list of compiler languages under [XEngine] for this script to successfully upload.
*
*  Adding 'csraw' to your allowed language list when using the old OpenSim compiler will have no effect
*  besides an error being written to your log file.  OpenSim will run but you will not actually be able
*  to use the 'csraw' upload type.
*
*  Note that you can also set 'CreateClassWrapperForCSharpScripts' to 'false' under the [LibLCLCC]
*  OpenSim.ini config section in order to enable 'csraw' mode uploads for every CSharp script sent to the 
*  LibLSLCC compiler;  Including those marked with '//c#' if you have 'cs' in your list of allowed languages.
*
*/ 
";

        private static bool FileNameIsValid(string fileName)
        {
            System.IO.FileInfo fi = null;

            try
            {
                fi = new FileInfo(fileName);
            }
            catch (ArgumentException)
            {
            }
            catch (PathTooLongException)
            {
            }
            catch (NotSupportedException)
            {
            }

            if (ReferenceEquals(fi, null))
            {
                return false;
            }

            return true;
        }


        private static DateTime RetrieveLinkerTimestamp()
        {
            var filePath = Assembly.GetCallingAssembly().Location;
            const int cPeHeaderOffset = 60;
            const int cLinkerTimestampOffset = 8;
            var b = new byte[2048];
            Stream s = null;

            try
            {
                s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            var i = BitConverter.ToInt32(b, cPeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(b, i + cLinkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }


        private class SwitchResult
        {
            public SwitchResult(int argsConsumed)
            {
                ArgsConsumed = argsConsumed;
            }

            public SwitchResult()
            {
                ArgsConsumed = 0;
            }

            public int ArgsConsumed { get; private set; }

            public bool OptionValid { get; set; }

            public bool Terminates { get; set; }
        }


        private class HandleSwitchResult
        {
            public bool InvalidOption { get; set; }

            public bool TerminationRequested { get; set; }

            public bool UnknownOption
            {
                get { return UnknownOptionString != null; }
            }

            public string UnknownOptionString { get; set; }
        }


        private static HandleSwitchResult HandleSwitches(string[] args,
            IDictionary<string, Func<string[], int, SwitchResult>> handlers)
        {

            HandleSwitchResult handleSwitchResult = new HandleSwitchResult();

            for (var i = 0; i < args.Length;)
            {
                var arg = args[i];


                Func<string[], int, SwitchResult> handler;

                if (handlers.TryGetValue(arg, out handler))
                {
                    var result = handler(args, i);

                    if (!result.OptionValid)
                    {
                        handleSwitchResult.InvalidOption = true;
                    }

                    if (result.Terminates)
                    {
                        handleSwitchResult.TerminationRequested = true;
                    }

                    i += 1 + result.ArgsConsumed;
                }
                else
                {
                    handleSwitchResult.UnknownOptionString = arg;
                    return handleSwitchResult;
                }
            }
            return handleSwitchResult;
        }




        private static void WriteAbout()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("Author: Teriks");
            Console.WriteLine("License: Three Clause BSD");
            Console.WriteLine();
            Console.WriteLine("Compile Date: " + RetrieveLinkerTimestamp());
#if DEBUG
            Console.WriteLine("Build Type: DEBUG");
#else
            Console.WriteLine("Build Type: Release");
#endif
            Console.WriteLine("Version: 0.1.0");
            Console.WriteLine("=================================");
        }




        private static void WriteHelp()
        {
            var defaultLibraryDataProvider = new LSLDefaultLibraryDataProvider();

            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("Usage: lslcc -i script.lsl -o script.cs -librarysubsets lsl");
            Console.WriteLine("-i: input file.");
            Console.WriteLine("-o: (Optional) output file.");
            Console.WriteLine();
            Console.WriteLine("If you do not specify an output file, the name of the input file is used and given a '.cs' extension.");
            Console.WriteLine("In that case the file is output to the current working directory.");

            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-librarysubsets: (Optional, default is lsl)");
            Console.WriteLine();
            Console.WriteLine("Set the available library subsets when compiling ( Separated by semi-colons ; )");
            Console.WriteLine("All acceptable values are: (Spaces are not allowed)");
            Console.WriteLine();
            Console.WriteLine(string.Join(";", defaultLibraryDataProvider.PossibleSubsets));

            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-servercode: (Default)");
            Console.WriteLine();
            Console.WriteLine("Compile server side code, code that OpenSim would pass directly to the CSharp compiler.");
            Console.WriteLine("This type of code supports script resets, but un-modified builds of OpenSim provide no way to upload it from your client.");

            Console.WriteLine("======================================");
            Console.WriteLine("-clientcode:");
            Console.WriteLine();
            Console.WriteLine("Compile client up-loadable code that works with all versions of OpenSim, ");
            Console.WriteLine("but does not support script resets.");

            Console.WriteLine("======================================");
            Console.WriteLine("-coop-stop:");
            Console.WriteLine();
            Console.WriteLine("Insert cooperative termination calls used by OpenSim when co-op script stop mode is enabled.");

            Console.WriteLine("======================================");
            Console.WriteLine("-h: help");
            Console.WriteLine("-v: lslcc, version and info");
        }



        private class Options
        {
            public string InFile { get; set; }
            public string OutFile { get; set; }

            public bool ServerCode { get; set; }
            public bool ClientCode { get; set; }

            public bool CoOpStop { get; set; }

            public HashSet<string> LibrarySubsets { get; set; }


            public Options()
            {
                LibrarySubsets = new HashSet<string>();
            }
        }

        private static void ShowCommandWarning(string notice, params object[] formatArgs)
        {
            Console.WriteLine("WARNING: " + notice, formatArgs);
        }

        private static void ShowCommandNotice(string notice, params object[] formatArgs)
        {
            Console.WriteLine("NOTICE: "+ notice, formatArgs);
        }


        private static void ShowCommandError(string error, params object[] formatArgs)
        {
            Console.WriteLine("ERROR: " + error, formatArgs);
        }



        public static void Main(string[] args)
        {
            Options options = new Options();


            Dictionary<string, Func<string[], int, SwitchResult>> switchHandlers =
                new Dictionary<string, Func<string[], int, SwitchResult>>();


            switchHandlers.Add("-i", (argArray, index) =>
            {
                string fileArg = argArray[index + 1];

                var result = new SwitchResult(1);

                if (!FileNameIsValid(fileArg))
                {
                    ShowCommandError("Input File '{0}' has an invalid file name.", fileArg);
                    result.OptionValid = false;
                }
                else if (options.InFile == null && File.Exists(fileArg))
                {
                    options.InFile = fileArg;
                    result.OptionValid = true;
                }
                else if (options.InFile == null)
                {
                    ShowCommandError("Input File \"" + fileArg + "\" does not exist.");
                    result.OptionValid = false;
                }
                else
                {
                    ShowCommandError("Input file specified multiple times, use -h for help.");
                    result.OptionValid = false;
                }

                return result;
            });



            switchHandlers.Add("-o", (argArray, index) =>
            {
                string fileArg = argArray[index + 1];

                var result = new SwitchResult(1);

                if (!FileNameIsValid(fileArg))
                {
                    ShowCommandError("Output File '{0}' has an invalid file name.", fileArg);
                    result.OptionValid = false;
                }
                else if (options.OutFile == null)
                {
                    options.OutFile = fileArg;
                    result.OptionValid = true;
                }
                else
                {
                    ShowCommandError("Output file specified multiple times, use -h for help.");
                    result.OptionValid = false;
                }

                return result;
            });


            switchHandlers.Add("-librarysubsets", (argArray, index) =>
            {
                var libs = argArray[index + 1];

                var result = new SwitchResult(1) {OptionValid = true};

                foreach (var lib in libs.Split(';').Select(x => x.Trim()))
                {
                    if (LSLLibraryDataSubsetNameParser.ValidateSubsetName(lib))
                    {
                        options.LibrarySubsets.Add(lib);
                    }
                    else
                    {
                        ShowCommandError(
                            "LibrarySubset '{0}' has an invalid name, it does not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*).",
                            lib);

                        result.OptionValid = false;
                        break;
                    }
                }

                return result;
            });



            switchHandlers.Add("-clientcode", (argArray, index) =>
            {
                var result = new SwitchResult();

                if (options.ServerCode)
                {
                    ShowCommandError("Cannot specify -clientcode because -servercode was already specified.");
                    result.OptionValid = false;
                }
                else
                {
                    result.OptionValid = true;
                    options.ClientCode = true;
                }

                return result;
            });


            switchHandlers.Add("-servercode", (argArray, index) =>
            {
                var result = new SwitchResult();

                if (options.ClientCode)
                {
                    ShowCommandError("Cannot specify -servercode because -clientcode was already specified.");
                    result.OptionValid = false;
                }
                else
                {
                    result.OptionValid = true;
                    options.ServerCode = true;
                }

                return result;
            });


            switchHandlers.Add("-coop-stop", (argArray, index) =>
            {
                options.CoOpStop = true;

                return new SwitchResult { OptionValid = true};
            });


            switchHandlers.Add("-h", (argArray, index) =>
            {
                WriteHelp();

                return new SwitchResult { OptionValid = true, Terminates = true };
            });


            switchHandlers.Add("-v", (argArray, index) =>
            {
                WriteAbout();

                return new SwitchResult { OptionValid = true, Terminates = true };
            });



            var switchResults = HandleSwitches(args, switchHandlers);



            if (switchResults.UnknownOption)
            {
                ShowCommandError("Unknown Option '{0}',  use -h for help.", switchResults.UnknownOptionString);
                ShowCommandNotice("Arguments Passed: '{0}'",  string.Join(" ", args.Select(x=> x.Contains(' ') ? '"'+x+'"' : x)));
                return;
            }


            if (switchResults.InvalidOption || switchResults.TerminationRequested)
            {
                return;
            }



            if (options.InFile == null)
            {
                ShowCommandError("Input file not specified, use -h for help.");
                return;
            }


            if (options.OutFile == null)
            {
                options.OutFile = Path.GetFileNameWithoutExtension(options.InFile) + ".cs";

                ShowCommandNotice("Output file not specified, using '{0}'.", options.OutFile);
            }


            if (options.LibrarySubsets.Count == 0)
            {
                options.LibrarySubsets.Add("lsl");
                ShowCommandNotice("No library subsets specified, adding 'lsl'.");
            }


            Console.WriteLine();
            Console.WriteLine("Compiling \"" + options.InFile + "\"...");



            //==========
            // Validate Code, Build Tree
            //==========

            //so we can print the errors exactly when we want to
            var syntaxListener = new LSLSyntaxListenerPriorityQueue(
                new LSLDefaultSyntaxErrorListener(), 
                new LSLDefaultSyntaxWarningListener()
                );


            var validatorServices = new LSLCustomValidatorServiceProvider
            {
                ExpressionValidator = new LSLDefaultExpressionValidator(),
                StringLiteralPreProcessor = new LSLDefaultStringPreProcessor(),
                SyntaxErrorListener = syntaxListener,
                SyntaxWarningListener = syntaxListener
            };


            var defaultProvider = new LSLDefaultLibraryDataProvider();

            validatorServices.LibraryDataProvider = defaultProvider;


            foreach (var library in options.LibrarySubsets)
            {
                if (defaultProvider.SubsetDescriptions.ContainsKey(library))
                {
                    defaultProvider.ActiveSubsets.Add(library);
                }
                else
                {
                    ShowCommandWarning("Library subset '{0}' does not exist and was ignored.", library);
                }
            }


            var validator = new LSLCodeValidator(validatorServices);


            ILSLCompilationUnitNode validated;



            try
            {
                using (var infile = new StreamReader(options.InFile))
                {
                    validated = validator.Validate(infile);

                    
                    if (validator.HasSyntaxErrors)
                    {
                        Console.WriteLine();
                        Console.WriteLine("===============================");
                        Console.WriteLine();
                        Console.WriteLine("Syntax Errors:");
                        Console.WriteLine();

                        syntaxListener.InvokeQueuedErrors();

                        Console.WriteLine("===============================");
                        Console.WriteLine();
                    }


                    if (validator.HasSyntaxWarnings)
                    {
                        if (!validator.HasSyntaxErrors)
                        {
                            Console.WriteLine();
                            Console.WriteLine("===============================");
                            Console.WriteLine();
                        }
                                           
                        Console.WriteLine("Syntax Warnings:");
                        Console.WriteLine();

                        syntaxListener.InvokeQueuedWarnings();

                        Console.WriteLine("===============================");
                        Console.WriteLine();
                    }


                    if (validator.HasSyntaxErrors)
                    {
                        Console.WriteLine("Compilation phase did not start due to syntax errors.");
                        return;
                    }
                }
            }
            catch (LSLCodeValidatorInternalException error)
            {
                Console.WriteLine();
                Console.WriteLine("Code Validator, internal error:");
                Console.WriteLine();
                Console.WriteLine(error.Message);
                Console.WriteLine();
                Console.WriteLine(InternalErrorMessage);
                return;
            }
            catch (Exception error)
            {
                Console.WriteLine();
                Console.WriteLine("Code Validator, unknown error:");
                Console.WriteLine();
                Console.WriteLine(error.Message);
                Console.WriteLine();
                Console.WriteLine(InternalErrorMessage);
                return;
            }


            //==========
            // Compile Tree Into Code
            //==========


            LSLOpenSimCSCompilerSettings compilerSettings;

            if (!options.ServerCode && !options.ClientCode)
            {
                options.ServerCode = true;
            }

            if (options.ServerCode)
            {
                compilerSettings =
                    LSLOpenSimCSCompilerSettings.OpenSimServerSideDefault(
                        validator.ValidatorServices.LibraryDataProvider);

                compilerSettings.ScriptHeader = ServerSideScriptCompilerHeader;
                compilerSettings.InsertCoOpTerminationCalls = options.CoOpStop;
            }
            else
            {
                compilerSettings =
                    LSLOpenSimCSCompilerSettings.OpenSimClientUploadable(
                        validator.ValidatorServices.LibraryDataProvider);

                compilerSettings.ScriptHeader = ClientSideScriptCompilerHeader;
                compilerSettings.InsertCoOpTerminationCalls = options.CoOpStop;
            }


            try
            {
                using (var outfile = File.Create(options.OutFile))
                {
                    var compiler = new LSLOpenSimCSCompiler(compilerSettings);
                    try
                    {
                        compiler.Compile(validated, new StreamWriter(outfile, Encoding.UTF8));
                    }
                    catch (LSLCompilerInternalException error)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Compiler internal error:");
                        Console.WriteLine();
                        Console.WriteLine(error.Message);
                        Console.WriteLine();
                        Console.WriteLine(InternalErrorMessage);
                        return;
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Compiler unknown error:");
                        Console.WriteLine();
                        Console.WriteLine(error.Message);
                        Console.WriteLine();
                        Console.WriteLine(InternalErrorMessage);
                        return;
                    }
                }

                Console.WriteLine("Finished, output to \"" + options.OutFile + "\"");
            }
            catch (IOException)
            {
                ShowCommandError("Output File '{0}' could not be written to.", options.OutFile);
            }
        }
    }
}