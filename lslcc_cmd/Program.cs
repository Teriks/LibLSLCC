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
using System.Runtime.Serialization;
using System.Text;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Compilers;
using LibLSLCC.Compilers.OpenSim;
using LibLSLCC.LibraryData;


#endregion

namespace lslcc
{
    internal class Program
    {
        private const string InternalErrorMessage =
            "Please create a bug report with the code that caused this message, and the message itself.";

        /// <summary>
        ///     The output header for lslcc related command errors.
        ///     this is not related to syntax error messages from the compiler itself.
        /// </summary>
        private const string CmdErrorHeader = "ERROR: ";

        /// <summary>
        ///     The output header for lslcc related command warnings.
        ///     this is not related to syntax warning messages from the compiler itself.
        /// </summary>
        private const string CmdWarningHeader = "WARNING: ";

        /// <summary>
        ///     The output header for lslcc related command notices.
        ///     this is not related to messages from the compiler itself.
        /// </summary>
        private const string CmdNoticeHeader = "NOTICE: ";

        /// <summary>
        ///     The output header that gets placed before the raw content of an exception message of any sort.
        /// </summary>
        private const string CmdExceptionHeader = "REASON: ";

        /// <summary>
        ///     The client side script compiler header.
        ///     This content gets placed at the top of scripts compiled with the -clientcode switch.
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
        ///     The server side script compiler header.
        ///     This content gets placed at the top of scripts compiled with the -servercode switch.
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

            Console.WriteLine();
            Console.WriteLine("LibLSLCC command line OpenSim LSL compiler");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine();
            Console.WriteLine(indent + "lslcc -i script.lsl -o script.cs");
            Console.WriteLine();
            Console.WriteLine(indent + "lslcc -i script.lsl dir{0}script2.lsl -o \"{{dir}}{{name}}.cs\"", Path.DirectorySeparatorChar);
            Console.WriteLine();
            Console.WriteLine(indent + "lslcc -i \"dir{0}**{0}*.lsl\" \"dir2{0}*\" -o \"{{dir}}{{name}}.cs\"  (built in globing)", Path.DirectorySeparatorChar);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("-i: One or more input files or glob expressions (quote globs on *nix).");
            Console.WriteLine("-o: (Optional) output file or output file name template.");
            Console.WriteLine();
            Console.WriteLine(indent + "If you do not specify an output file or output file template, the directory and");
            Console.WriteLine(indent + "name of the input file is used and given a '.cs' extension (equivalent to - o \"{dir}{name}.cs\".");
            Console.WriteLine(indent + "In that case, the output file is written to the directory the input file resides in.");
            Console.WriteLine();


            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-log: log file or log file name template");
            Console.WriteLine();
            Console.WriteLine(indent + "If this is a file name template such as \"{dir}{name}.log\", then a separate log file is created");
            Console.WriteLine(indent + "for each input file.  Otherwise, all compiler output is appended to the specified file.");
            Console.WriteLine();


            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-log-return-code:");
            Console.WriteLine();
            Console.WriteLine(indent + "Append a textual representation of the compiler return code to the end of each log file name.");
            Console.WriteLine();

            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-librarysubsets: subset1;subset2;... (default is just lsl)");
            Console.WriteLine();
            Console.WriteLine(indent + "Set the available library subsets when compiling ( Separated by semi-colons ; )");
            Console.WriteLine(indent + "Spaces are not allowed between the names and the semi-colons unless you quote the string.");
            Console.WriteLine();
            Console.WriteLine(indent + "All acceptable values are:");
            Console.WriteLine();
            Console.WriteLine(indent + "" + string.Join(";", defaultLibraryDataProvider.PossibleSubsets));
            Console.WriteLine();

            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("-servercode: (On by default)");
            Console.WriteLine();
            Console.WriteLine(indent + "Compile server side code, code that OpenSim would pass directly to the CSharp compiler.");
            Console.WriteLine(indent + "This type of code supports script resets, but un-modified builds of OpenSim provide");
            Console.WriteLine(indent + "no way to upload it from your client.");
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
            Console.WriteLine(indent + "Insert cooperative termination calls used by OpenSim when co-op script stop mode is enabled.");
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
            Console.WriteLine();
            Console.WriteLine("Author: Teriks");
            Console.WriteLine("License: Three Clause BSD");
            Console.WriteLine();
            Console.WriteLine("Compile Date: " + RetrieveLinkerTimestamp());
#if DEBUG
            Console.WriteLine();
            Console.WriteLine("Build Type: Debug");
#else
            Console.WriteLine();
            Console.WriteLine("Build Type: Release");
#endif
            var version = Assembly.GetEntryAssembly().GetName().Version;
            Console.WriteLine();
            Console.WriteLine("Version: "+version);
            Console.WriteLine();
            Console.WriteLine("=================================");
        }


        private static void WriteCompilerMessage(TextWriter log, string format, params object[] args)
        {
            if (log != null)
            {
                try
                {
                    if (args.Length == 0)
                    {
                        log.WriteLine(format);
                    }
                    else
                    {
                        log.WriteLine(format, args);
                    }
                }
                catch (Exception e)
                {
                    throw new LogWriteException(e.Message, e);
                }
            }

            if (args.Length == 0)
            {
                Console.WriteLine(format);
            }
            else
            {
                Console.WriteLine(format, args);
            }
        }


        private class SyntaxErrorListener : LSLDefaultSyntaxErrorListener
        {
            private readonly TextWriter _log;


            public SyntaxErrorListener(TextWriter log)
            {
                _log = log;
            }


            protected override void OnError(LSLSourceCodeRange location, string message)
            {
                WriteCompilerMessage(_log, "({0},{1}) ERROR: {2}", MapLineNumber(location.LineStart),
                    location.ColumnStart, message + Environment.NewLine);
            }
        }


        private class SyntaxWarningListener : LSLDefaultSyntaxWarningListener
        {
            private readonly TextWriter _log;


            public SyntaxWarningListener(TextWriter log)
            {
                _log = log;
            }


            protected override void OnWarning(LSLSourceCodeRange location, string message)
            {
                WriteCompilerMessage(_log, "({0},{1}) WARNING: {2}", MapLineNumber(location.LineStart),
                    location.ColumnStart, message + Environment.NewLine);
            }
        }


        private static void WriteCompilerMessage(TextWriter log)
        {
            if (log != null)
            {
                try
                {
                    log.WriteLine();
                }
                catch (Exception e)
                {
                    throw new LogWriteException(e.Message, e);
                }
            }

            Console.WriteLine();
        }


        public static int Main(string[] args)
        {
            Options options = new Options();


            var switchHandlers =
                new Dictionary<string, Func<string[], int, SwitchResult>>();


            switchHandlers.Add("-i", (argArray, index) =>
            {
                var result = new SwitchResult(0);

                result.OptionValid = true;

                for (int i = index + 1; i < argArray.Length; i++)
                {
                    string fileArg = argArray[i];


                    if (fileArg.StartsWith("-"))
                    {
                        break;
                    }

                    foreach (var file in Glob.Glob.Expand(fileArg))
                    {
                        bool specifiedAlready = options.InFiles.Contains(file.FullName);

                        if (specifiedAlready)
                        {
                            Console.WriteLine(CmdErrorHeader + "An input file was specified multiple times, use -h for help.");
                            result.OptionValid = false;
                        }
                        else if (result.OptionValid)
                        {
                            options.InFiles.Add(file.FullName);
                        }
                    }
                    result.ArgsConsumed++;
                }

                return result;
            });


            switchHandlers.Add("-o", (argArray, index) =>
            {
                int paramIndex = index + 1;

                if (paramIndex > (argArray.Length-1))
                {
                    Console.WriteLine(CmdErrorHeader + "Option -o requires an argument, use -h for help.");
                    return new SwitchResult() {OptionValid = false};
                }


                string fileArg = argArray[paramIndex];

                var result = new SwitchResult(1);

                var validateName = fileArg.Replace("{name}", "name");
                validateName = validateName.Replace("{dir}", "dir" + Path.DirectorySeparatorChar);

                if (!FileNameIsValid(validateName))
                {
                    Console.WriteLine(CmdErrorHeader + "Output file '{0}' uses an invalid file name.", fileArg);
                    result.OptionValid = false;
                }
                else if (options.OutFile == null)
                {
                    options.OutFile = fileArg;
                    result.OptionValid = true;
                }
                else
                {
                    Console.WriteLine(CmdErrorHeader + "Output file specified multiple times, use -h for help.");
                    result.OptionValid = false;
                }

                return result;
            });


            switchHandlers.Add("-log", (argArray, index) =>
            {
                int paramIndex = index + 1;

                if (paramIndex > (argArray.Length - 1))
                {
                    Console.WriteLine(CmdErrorHeader + "Option -log requires an argument, use -h for help.");
                    return new SwitchResult() { OptionValid = false };
                }

                string fileArg = argArray[paramIndex];

                var result = new SwitchResult(1);

                var validateName = fileArg.Replace("{name}", "name");
                validateName = validateName.Replace("{dir}", "dir" + Path.DirectorySeparatorChar);

                if (!FileNameIsValid(validateName))
                {
                    Console.WriteLine(CmdErrorHeader + "Log file '{0}' uses an invalid file name.", fileArg);
                    result.OptionValid = false;
                }
                else if (options.LogFile == null)
                {
                    options.LogFile = fileArg;
                    result.OptionValid = true;
                }
                else
                {
                    Console.WriteLine(CmdErrorHeader + "Log file specified multiple times, use -h for help.");
                    result.OptionValid = false;
                }

                return result;
            });


            switchHandlers.Add("-log-return-code", (argArray, index) =>
            {
                var result = new SwitchResult() {OptionValid = true};

                options.LogReturnCode = true;

                return result;
            });


            switchHandlers.Add("-librarysubsets", (argArray, index) =>
            {
                int paramIndex = index + 1;

                if (paramIndex > (argArray.Length - 1))
                {
                    Console.WriteLine(CmdErrorHeader + "Option -librarysubsets requires an argument, use -h for help.");
                    return new SwitchResult() { OptionValid = false };
                }

                var libs = argArray[paramIndex];

                var result = new SwitchResult(1) {OptionValid = true};

                foreach (var lib in libs.Split(';').Select(x => x.Trim()))
                {
                    if (LSLLibraryDataSubsetNameParser.ValidateSubsetName(lib))
                    {
                        options.LibrarySubsets.Add(lib);
                    }
                    else
                    {
                        Console.WriteLine(CmdErrorHeader +
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
                    Console.WriteLine(CmdErrorHeader +
                                      "Cannot specify -clientcode because -servercode was already specified.");
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
                    Console.WriteLine(CmdErrorHeader +
                                      "Cannot specify -servercode because -clientcode was already specified.");
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


            var switchResults = ArgumentParser.HandleSwitches(args, switchHandlers);


            //=======================
            //=======================


            if (switchResults.UnknownOption)
            {
                Console.WriteLine(CmdErrorHeader + "Unknown Option '{0}',  use -h for help.",
                    switchResults.UnknownOptionString);
                Console.WriteLine(CmdNoticeHeader + "Arguments Passed: '{0}'",
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


            if (options.InFiles.Count == 0)
            {
                Console.WriteLine(CmdErrorHeader + "No input files specified, use -h for help.");
                return ReturnCode.MissingInputFile;
            }


            if (options.OutFile == null)
            {
                options.OutFile = "{dir}{name}.cs";

                Console.WriteLine(CmdNoticeHeader + "Output file not specified, using '{0}'.", options.OutFile);
            }


            if (options.LibrarySubsets.Count == 0)
            {
                options.LibrarySubsets.Add("lsl");
                Console.WriteLine(CmdNoticeHeader + "No library subsets specified, adding 'lsl'.");
            }

            if (options.InFiles.Count > 1 && !(options.OutFile.Contains("{dir}") || options.OutFile.Contains("{name}")))
            {
                Console.WriteLine(CmdErrorHeader +
                                  "Option -o must us a filename template such as \"{dir}{name}.cs\" when multiple input files are specified.");
                return ReturnCode.InvalidOption;
            }


            if (options.OutFile == options.LogFile)
            {
                Console.WriteLine(CmdErrorHeader + "Parameter for option -o and -log cannot be the same.");
                return ReturnCode.InvalidOption;
            }


            int lastReturnCode = ReturnCode.Success;


            StreamWriter log = null;


            bool multipleLogs = options.LogFile != null && 
                (options.LogFile.Contains("{name}") || options.LogFile.Contains("{dir}"));

            string logFileName = null;

            try
            {
                foreach (var inFile in options.InFiles)
                {
                    if (options.LogFile != null)
                    {
                        logFileName = ExpandOutputFileVars(inFile, options.LogFile);

                        if (log == null)
                        {
                            try
                            {
                                log = new StreamWriter(logFileName) {AutoFlush = true};
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(CmdErrorHeader + "Log File '{0}' could not be opened for writing.",
                                    logFileName);
                                Console.WriteLine();
                                Console.WriteLine(CmdExceptionHeader + e.Message);
                                lastReturnCode = ReturnCode.LogFileAccessError;
                            }
                        }
                    }


                    try
                    {
                        WriteCompilerMessage(log);
                        WriteCompilerMessage(log, "Compiling \"" + inFile + "\"...");

                        lastReturnCode = CompileFile(log, options, inFile);
                    }
                    catch (LogWriteException e)
                    {
                        Console.WriteLine(CmdErrorHeader + "Log File '{0}' could not be written to.", logFileName);
                        Console.WriteLine();
                        Console.WriteLine(CmdExceptionHeader + e.Message);
                        lastReturnCode = ReturnCode.LogFileAccessError;
                    }
                    finally
                    {
                        if (log != null && multipleLogs)
                        {
                            log.Close();
                            log = null;

                            if (options.LogReturnCode)
                            {
                                try
                                {
                                    MoveFileWithOverwrite(logFileName,
                                        logFileName + ReturnCode.ReturnCodeNameMap[lastReturnCode]);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(
                                        CmdErrorHeader + "Log File '{0}' could not be renamed to append return code.",
                                        logFileName);
                                    Console.WriteLine();
                                    Console.WriteLine(CmdExceptionHeader + e.Message);
                                    lastReturnCode = ReturnCode.LogFileAccessError;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (log != null && !multipleLogs)
                {
                    log.Close();

                    if (options.LogReturnCode)
                    {
                        try
                        {
                            MoveFileWithOverwrite(logFileName,
                                logFileName + ReturnCode.ReturnCodeNameMap[lastReturnCode]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(
                                CmdErrorHeader + "Log File '{0}' could not be renamed to append return code.",
                                logFileName);
                            Console.WriteLine();
                            Console.WriteLine(CmdExceptionHeader + e.Message);
                            lastReturnCode = ReturnCode.LogFileAccessError;
                        }
                    }
                }
            }

            return lastReturnCode;
        }


        private static void MoveFileWithOverwrite(string fileName, string newName)
        {
            if (File.Exists(newName)) File.Delete(newName);
            File.Move(fileName, newName);
        }



        private static readonly LSLValidatorServiceProvider ValidatorServices = new LSLValidatorServiceProvider
        {
            ExpressionValidator = new LSLDefaultExpressionValidator(),
            StringLiteralPreProcessor = new LSLDefaultStringPreProcessor()
        };


        private static int CompileFile(StreamWriter log, Options options, string inFile)
        {


            //==========
            // Validate Code, Build Tree
            //==========

            //so we can print the errors exactly when we want to
            var syntaxListener = new LSLSyntaxListenerPriorityQueue(
                new SyntaxErrorListener(log),
                new SyntaxWarningListener(log)
                );



            ValidatorServices.SyntaxErrorListener = syntaxListener;
            ValidatorServices.SyntaxWarningListener = syntaxListener;



            var defaultProvider = new LSLEmbeddedLibraryDataProvider();

            ValidatorServices.LibraryDataProvider = defaultProvider;


            foreach (var library in options.LibrarySubsets)
            {
                if (defaultProvider.SubsetDescriptions.ContainsKey(library))
                {
                    defaultProvider.ActiveSubsets.Add(library);
                }
                else
                {
                    WriteCompilerMessage(log,
                        CmdWarningHeader + "Library subset '{0}' does not exist and was ignored.", library);
                }
            }


            var validator = new LSLCodeValidator(ValidatorServices);


            ILSLCompilationUnitNode validated;


            try
            {
                using (var infile = new StreamReader(inFile))
                {
                    validated = validator.Validate(infile);


                    if (validator.HasSyntaxErrors)
                    {
                        WriteCompilerMessage(log);
                        WriteCompilerMessage(log, "===============================");
                        WriteCompilerMessage(log);
                        WriteCompilerMessage(log, "Syntax Errors:");
                        WriteCompilerMessage(log);

                        syntaxListener.InvokeQueuedErrors();

                        WriteCompilerMessage(log, "===============================");
                        WriteCompilerMessage(log);
                    }


                    if (validator.HasSyntaxWarnings)
                    {
                        if (!validator.HasSyntaxErrors)
                        {
                            WriteCompilerMessage(log);
                            WriteCompilerMessage(log, "===============================");
                            WriteCompilerMessage(log);
                        }

                        WriteCompilerMessage(log, "Syntax Warnings:");
                        WriteCompilerMessage(log);

                        syntaxListener.InvokeQueuedWarnings();

                        WriteCompilerMessage(log, "===============================");
                        WriteCompilerMessage(log);
                    }

                    if (validator.HasSyntaxErrors)
                    {
                        WriteCompilerMessage(log, "Compilation phase did not start due to syntax errors.");
                        return ReturnCode.SyntaxErrors;
                    }
                }
            }
            catch (IOException error)
            {
                WriteCompilerMessage(log, CmdErrorHeader + "Input File '{0}' could not be read from.", inFile);
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, CmdExceptionHeader + error.Message);
                return ReturnCode.InputFileUnreadable;
            }
            catch (LSLCodeValidatorInternalException error)
            {
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, "Code Validator, internal error.");
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, CmdExceptionHeader + error.Message);
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, InternalErrorMessage);
                return ReturnCode.CodeValidatorInternalError;
            }
            catch (Exception error)
            {
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, "Code Validator, unknown error.");
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, CmdExceptionHeader + error.Message);
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, InternalErrorMessage);
                return ReturnCode.CodeValidatorUnknownError;
            }


            //==========
            // Compile Tree Into Code
            //==========


            LSLOpenSimCompilerSettings compilerSettings;

            if (!options.ServerCode && !options.ClientCode)
            {
                options.ServerCode = true;
            }

            if (options.ServerCode)
            {
                compilerSettings =
                    LSLOpenSimCompilerSettings.OpenSimServerSideDefault();

                compilerSettings.ScriptHeader = ServerSideScriptCompilerHeader;
                compilerSettings.InsertCoOpTerminationCalls = options.CoOpStop;
            }
            else
            {
                compilerSettings =
                    LSLOpenSimCompilerSettings.OpenSimClientUploadable();

                compilerSettings.ScriptHeader = ClientSideScriptCompilerHeader;
                compilerSettings.InsertCoOpTerminationCalls = options.CoOpStop;
            }


            string outFile = ExpandOutputFileVars(inFile, options.OutFile);

            try
            {
                using (var outfile = File.Create(outFile))
                {
                    var compiler = new LSLOpenSimCompiler(defaultProvider, compilerSettings);

                    compiler.Compile(validated, new StreamWriter(outfile, Encoding.UTF8));
                }

                WriteCompilerMessage(log, "Finished, output to \"" + outFile + "\"");
            }
            catch (IOException error)
            {
                WriteCompilerMessage(log, CmdErrorHeader + "Output File '{0}' could not be written to.", outFile);
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, CmdExceptionHeader + error.Message);
                return ReturnCode.OutputFileUnwritable;
            }
            catch (LSLCompilerInternalException error)
            {
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, "Compiler internal error:");
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, CmdExceptionHeader + error.Message);
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, InternalErrorMessage);
                return ReturnCode.CompilerInternalError;
            }
            catch (Exception error)
            {
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, "Compiler unknown error:");
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, CmdExceptionHeader + error.Message);
                WriteCompilerMessage(log);
                WriteCompilerMessage(log, InternalErrorMessage);
                return ReturnCode.CompilerUnknownError;
            }

            return ReturnCode.Success;
        }


        private static string ExpandOutputFileVars(string inFile, string outputFileTemplate)
        {
            string outFile = outputFileTemplate.Replace("{name}", Path.GetFileName(inFile));

            var directory = Path.GetDirectoryName(inFile);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                outFile = outFile.Replace("{dir}", directory + Path.DirectorySeparatorChar);
            }
            else
            {
                outFile = outFile.Replace("{dir}", "." + Path.DirectorySeparatorChar);
            }
            return outFile;
        }


        [Serializable]
        public class LogWriteException : Exception
        {
            //
            // For guidelines regarding the creation of new exception types, see
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
            // and
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
            //

            public LogWriteException()
            {
            }


            public LogWriteException(string message) : base(message)
            {
            }


            public LogWriteException(string message, Exception inner) : base(message, inner)
            {
            }


            protected LogWriteException(
                SerializationInfo info,
                StreamingContext context) : base(info, context)
            {
            }
        }
    }
}