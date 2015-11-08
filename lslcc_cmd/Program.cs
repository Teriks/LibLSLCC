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

#endregion

namespace lslcc
{
    internal class Program
    {
        private const string InternalErrorMessage =
            "Please create a bug report with the code that caused this message, and the message itself.";


        /// <summary>
        /// The output header for lslcc related command errors.
        /// this is not related to syntax error messages from the compiler itself.
        /// </summary>
        private const string CmdErrorHeader = "ERROR: ";

        /// <summary>
        /// The output header for lslcc related command warnings.
        /// this is not related to syntax warning messages from the compiler itself.
        /// </summary>
        private const string CmdWarningHeader = "WARNING: ";

        /// <summary>
        /// The output header for lslcc related command notices.
        /// this is not related to messages from the compiler itself.
        /// </summary>
        private const string CmdNoticeHeader = "NOTICE: ";


        /// <summary>
        /// The output header that gets placed before the raw content of an exception message of any sort.
        /// </summary>
        private const string CmdExceptionHeader = "REASON: ";


        /// <summary>
        /// The client side script compiler header.
        /// This content gets placed at the top of scripts compiled with the -clientcode switch.
        /// </summary>
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

        /// <summary>
        /// The server side script compiler header.
        /// This content gets placed at the top of scripts compiled with the -servercode switch.
        /// </summary>
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
            FileInfo fi = null;

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
            var handleSwitchResult = new HandleSwitchResult();

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




        [AttributeUsage(AttributeTargets.Field)]
        private sealed class ReturnCodeHelp : Attribute
        {
            public ReturnCodeHelp(string help)
            {
                Help = help;
            }

            public string Help { get; private set; }
        }




        private static class ReturnCode
        {
            [ReturnCodeHelp("Success (Including when -h,-v or -returncodes is used).")]
            public const int Success = 0;

            [ReturnCodeHelp("Syntax Errors encountered while compiling.")]
            public const int SyntaxErrors = 1;

            [ReturnCodeHelp("Input File was not specified with -o.")]
            public const int MissingInputFile = 2;

            [ReturnCodeHelp(
                "An option was not valid given the previously defined options, or it did not like the parameters given to it."
                )]
            public const int InvalidOption = 3;

            [ReturnCodeHelp("IO failure while trying to read the input file.")]
            public const int InputFileUnreadable = 4;

            [ReturnCodeHelp("IO failure while trying to write to the output file.")]
            public const int OutputFileUnwritable = 5;

            [ReturnCodeHelp("An unknown option was passed to lslcc.")]
            public const int UnknownOption = 6;

            [ReturnCodeHelp("(ICE) Code Validator experienced a known internal error.")]
            public const int CodeValidatorInternalError = 7;

            [ReturnCodeHelp("(ICE) Code Validator experienced an un-handled internal error.")]
            public const int CodeValidatorUnknownError = 8;

            [ReturnCodeHelp("(ICE) Compiler experienced a known internal error.")]
            public const int CompilerInternalError = 9;

            [ReturnCodeHelp("(ICE) Compiler experienced an un-handled internal error.")]
            public const int CompilerUnknownError = 10;
        }




        private static void WriteReturnCodes()
        {
            const string indent = "    ";

            Console.WriteLine("======================================");

            var fields = typeof (ReturnCode).GetFields().ToList();

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttributes(typeof (ReturnCodeHelp), false).ToList();
                if (attr.Count <= 0) continue;

                var help = attr[0] as ReturnCodeHelp;

                if (help == null) continue;

                Console.WriteLine();
                Console.WriteLine("Code " + field.GetValue(null) + " = " + field.Name + ":");
                Console.WriteLine();
                Console.WriteLine(indent + "" + help.Help);
                Console.WriteLine();
                Console.WriteLine("======================================");
            }

            Console.WriteLine();
        }



        private static void WriteHelp()
        {
            const string indent = "    ";

            var defaultLibraryDataProvider = new LSLEmbeddedLibraryDataProvider();


            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("Usage: lslcc -i script.lsl -o script.cs");
            Console.WriteLine();
            Console.WriteLine("-i: input file.");
            Console.WriteLine("-o: (Optional) output file.");
            Console.WriteLine();
            Console.WriteLine(indent +
                              "If you do not specify an output file, the name of the input file is used and given a '.cs' extension.");
            Console.WriteLine(indent + "In that case the file is output to the current working directory.");
            Console.WriteLine();

            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-librarysubsets: (Optional, default is lsl)");
            Console.WriteLine();
            Console.WriteLine(indent + "Set the available library subsets when compiling ( Separated by semi-colons ; )");
            Console.WriteLine(indent +
                              "Spaces are not allowed between the names and the semi-colons unless you quote the string.");
            Console.WriteLine();
            Console.WriteLine(indent + "All acceptable values are:");
            Console.WriteLine();
            Console.WriteLine(indent + "" + string.Join(";", defaultLibraryDataProvider.PossibleSubsets));
            Console.WriteLine();

            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-servercode: (Default)");
            Console.WriteLine();
            Console.WriteLine(indent +
                              "Compile server side code, code that OpenSim would pass directly to the CSharp compiler.");
            Console.WriteLine(indent +
                              "This type of code supports script resets, but un-modified builds of OpenSim provide no way to upload it from your client.");
            Console.WriteLine();

            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-clientcode:");
            Console.WriteLine();
            Console.WriteLine(indent + "Compile client up-loadable code that works with all versions of OpenSim, ");
            Console.WriteLine(indent + "but does not support script resets.");
            Console.WriteLine();

            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-coop-stop:");
            Console.WriteLine();
            Console.WriteLine(indent +
                              "Insert cooperative termination calls used by OpenSim when co-op script stop mode is enabled.");
            Console.WriteLine();


            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-implicit-param-tolist:");
            Console.WriteLine();
            Console.WriteLine(indent + "Allow implicit conversion of all LSL types into list within function call parameters.");
            Console.WriteLine(indent + "Example: llListFindList([\"john\", \"smith\", \"jane\"], \"smith\");");
            Console.WriteLine();

            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-returncodes: show lslcc return code descriptions.");
            Console.WriteLine();
            Console.WriteLine("-h: show lslcc general help.");
            Console.WriteLine();
            Console.WriteLine("-v: show lslcc version and program info.");

            Console.WriteLine();
            Console.WriteLine("======================================");
            Console.WriteLine();
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



        private class Options
        {
            public string InFile { get; set; }
            public string OutFile { get; set; }

            public bool ServerCode { get; set; }
            public bool ClientCode { get; set; }

            public bool CoOpStop { get; set; }

            public bool ImplicitParamToList { get; set; }

            public HashSet<string> LibrarySubsets { get; set; }


            public Options()
            {
                LibrarySubsets = new HashSet<string>();
            }
        }




        public static int Main(string[] args)
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
                    Console.WriteLine(CmdErrorHeader+"Input File '{0}' has an invalid file name.", fileArg);
                    result.OptionValid = false;
                }
                else if (options.InFile == null && File.Exists(fileArg))
                {
                    options.InFile = fileArg;
                    result.OptionValid = true;
                }
                else if (options.InFile == null)
                {
                    Console.WriteLine(CmdErrorHeader+"Input File \"" + fileArg + "\" does not exist.");
                    result.OptionValid = false;
                }
                else
                {
                    Console.WriteLine(CmdErrorHeader+"Input file specified multiple times, use -h for help.");
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
                    Console.WriteLine(CmdErrorHeader+"Output File '{0}' has an invalid file name.", fileArg);
                    result.OptionValid = false;
                }
                else if (options.OutFile == null)
                {
                    options.OutFile = fileArg;
                    result.OptionValid = true;
                }
                else
                {
                    Console.WriteLine(CmdErrorHeader+"Output file specified multiple times, use -h for help.");
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
                        Console.WriteLine(CmdErrorHeader+
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
                    Console.WriteLine(CmdErrorHeader+"Cannot specify -clientcode because -servercode was already specified.");
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
                    Console.WriteLine(CmdErrorHeader+"Cannot specify -servercode because -clientcode was already specified.");
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

                return new SwitchResult {OptionValid = true};
            });


            switchHandlers.Add("-implicit-param-tolist", (argArray, index) =>
            {
                options.ImplicitParamToList = true;

                return new SwitchResult { OptionValid = true };
            });


            switchHandlers.Add("-h", (argArray, index) =>
            {
                WriteHelp();

                return new SwitchResult {OptionValid = true, Terminates = true};
            });


            switchHandlers.Add("-v", (argArray, index) =>
            {
                WriteAbout();

                return new SwitchResult {OptionValid = true, Terminates = true};
            });


            switchHandlers.Add("-returncodes", (argArray, index) =>
            {
                WriteReturnCodes();

                return new SwitchResult {OptionValid = true, Terminates = true};
            });


            var switchResults = HandleSwitches(args, switchHandlers);


            if (switchResults.UnknownOption)
            {
                Console.WriteLine(CmdErrorHeader+"Unknown Option '{0}',  use -h for help.", switchResults.UnknownOptionString);
                Console.WriteLine(CmdNoticeHeader+"Arguments Passed: '{0}'",
                    string.Join(" ", args.Select(x => x.Contains(' ') ? '"' + x + '"' : x)));
                return ReturnCode.UnknownOption;
            }


            if (switchResults.InvalidOption)
            {
                return ReturnCode.InvalidOption;
            }

            if (switchResults.TerminationRequested)
            {
                return ReturnCode.Success;
            }


            if (options.InFile == null)
            {
                Console.WriteLine(CmdErrorHeader+"Input file not specified, use -h for help.");
                return ReturnCode.MissingInputFile;
            }


            if (options.OutFile == null)
            {
                options.OutFile = Path.GetFileNameWithoutExtension(options.InFile) + ".cs";

                Console.WriteLine(CmdNoticeHeader+"Output file not specified, using '{0}'.", options.OutFile);
            }


            if (options.LibrarySubsets.Count == 0)
            {
                options.LibrarySubsets.Add("lsl");
                Console.WriteLine(CmdNoticeHeader+"No library subsets specified, adding 'lsl'.");
            }


            Console.WriteLine();
            Console.WriteLine("Compiling \"" + options.InFile + "\"...");


            //==========
            // Validate Code, Build Tree
            //==========

            //so we can print the errors exactly when we want to
            var syntaxListener = new LSLSyntaxListenerPriorityQueue(
                new LSLSyntaxErrorListener(),
                new LSLSyntaxWarningListener()
                );


            var expressionValidatorSettings = new LSLExpressionValidatorSettings
            {
                ImplicitParamToListConversion = options.ImplicitParamToList
            };


            var validatorServices = new LSLValidatorServiceProvider
            {
                ExpressionValidator = new LSLExpressionValidator(expressionValidatorSettings),
                StringLiteralPreProcessor = new LSLStringPreProcessor(),
                SyntaxErrorListener = syntaxListener,
                SyntaxWarningListener = syntaxListener
            };


            var defaultProvider = new LSLEmbeddedLibraryDataProvider();

            validatorServices.LibraryDataProvider = defaultProvider;


            foreach (var library in options.LibrarySubsets)
            {
                if (defaultProvider.SubsetDescriptions.ContainsKey(library))
                {
                    defaultProvider.ActiveSubsets.Add(library);
                }
                else
                {
                    Console.WriteLine(CmdWarningHeader+"Library subset '{0}' does not exist and was ignored.", library);
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
                        return ReturnCode.SyntaxErrors;
                    }
                }
            }
            catch (IOException error)
            {
                Console.WriteLine(CmdErrorHeader+"Input File '{0}' could not be read from.", options.InFile);
                Console.WriteLine();
                Console.WriteLine(CmdExceptionHeader + error.Message);
                return ReturnCode.InputFileUnreadable;
            }
            catch (LSLCodeValidatorInternalException error)
            {
                Console.WriteLine();
                Console.WriteLine("Code Validator, internal error.");
                Console.WriteLine();
                Console.WriteLine(CmdExceptionHeader + error.Message);
                Console.WriteLine();
                Console.WriteLine(InternalErrorMessage);
                return ReturnCode.CodeValidatorInternalError;
            }
            catch (Exception error)
            {
                Console.WriteLine();
                Console.WriteLine("Code Validator, unknown error.");
                Console.WriteLine();
                Console.WriteLine(CmdExceptionHeader + error.Message);
                Console.WriteLine();
                Console.WriteLine(InternalErrorMessage);
                return ReturnCode.CodeValidatorUnknownError;
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
                    LSLOpenSimCSCompilerSettings.OpenSimServerSideDefault();

                compilerSettings.ScriptHeader = ServerSideScriptCompilerHeader;
                compilerSettings.InsertCoOpTerminationCalls = options.CoOpStop;
            }
            else
            {
                compilerSettings =
                    LSLOpenSimCSCompilerSettings.OpenSimClientUploadable();

                compilerSettings.ScriptHeader = ClientSideScriptCompilerHeader;
                compilerSettings.InsertCoOpTerminationCalls = options.CoOpStop;
            }


            try
            {
                using (var outfile = File.Create(options.OutFile))
                {
                    var compiler = new LSLOpenSimCSCompiler(defaultProvider, compilerSettings);

                    compiler.Compile(validated, new StreamWriter(outfile, Encoding.UTF8));
                }

                Console.WriteLine("Finished, output to \"" + options.OutFile + "\"");
            }
            catch (IOException error)
            {
                Console.WriteLine(CmdErrorHeader+"Output File '{0}' could not be written to.", options.OutFile);
                Console.WriteLine();
                Console.WriteLine(CmdExceptionHeader + error.Message);
                return ReturnCode.OutputFileUnwritable;
            }
            catch (LSLCompilerInternalException error)
            {
                Console.WriteLine();
                Console.WriteLine("Compiler internal error:");
                Console.WriteLine();
                Console.WriteLine(CmdExceptionHeader + error.Message);
                Console.WriteLine();
                Console.WriteLine(InternalErrorMessage);
                return ReturnCode.CompilerInternalError;
            }
            catch (Exception error)
            {
                Console.WriteLine();
                Console.WriteLine("Compiler unknown error:");
                Console.WriteLine();
                Console.WriteLine(CmdExceptionHeader + error.Message);
                Console.WriteLine();
                Console.WriteLine(InternalErrorMessage);
                return ReturnCode.CompilerUnknownError;
            }

            return ReturnCode.Success;
        }
    }
}