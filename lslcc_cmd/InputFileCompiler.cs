using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Compilers;
using LibLSLCC.Compilers.OpenSim;
using LibLSLCC.LibraryData;

namespace lslcc
{
    internal class InputFileCompiler
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
        private static readonly LSLDefaultExpressionValidator ExpressionValidator = new LSLDefaultExpressionValidator();
        private static readonly LSLDefaultStringPreProcessor StringLiteralPreProcessor = new LSLDefaultStringPreProcessor();

        

        private readonly string _fileName;

        private readonly List<string> _syntaxErrorMessages = new List<string>();
        private readonly List<string> _syntaxWarningMessages = new List<string>();
        private readonly List<string> _compilerMessageQueue = new List<string>();


        public bool CoOpStop { get; set; }


        public bool ClientCode { get; set; }

        public HashSet<string> LibrarySubsets { get; set; }

        private class SyntaxErrorListener : LSLDefaultSyntaxErrorListener
        {
            private InputFileCompiler Parent { get; set; }


            public SyntaxErrorListener(InputFileCompiler parent)
            {
                Parent = parent;
            }


            protected override void OnError(LSLSourceCodeRange location, string message)
            {
                Parent._syntaxErrorMessages.Add(
                    string.Format("({0},{1}) ERROR: {2}", MapLineNumber(location.LineStart),
                        location.ColumnStart, message + Environment.NewLine));
            }
        }


        private class SyntaxWarningListener : LSLDefaultSyntaxWarningListener
        {
            private InputFileCompiler Parent { get; set; }


            public SyntaxWarningListener(InputFileCompiler parent)
            {
                Parent = parent;
            }


            protected override void OnWarning(LSLSourceCodeRange location, string message)
            {
                Parent._syntaxWarningMessages.Add(
                    string.Format("({0},{1}) WARNING: {2}", MapLineNumber(location.LineStart),
                        location.ColumnStart, message + Environment.NewLine));
            }
        }


        public InputFileCompiler(string fileName)
        {
            _fileName = fileName;
        }


        public IEnumerable<string> CompilerMessageQueue
        {
            get { return _compilerMessageQueue; }
        }


        private void WriteLine(string message = "", params object[] formatArgs)
        {
            _compilerMessageQueue.Add(formatArgs.Length == 0 ? message : string.Format(message, formatArgs));
        }


        public void ClearMessageQueue()
        {
            _compilerMessageQueue.Clear();
        }

        public void WriteMessageQueue(bool clearMessageQueue, params TextWriter[] writers)
        {
            if (writers.Length == 0)
            {
                writers = new[] { Console.Out };
            }
            try
            {
                foreach (var msg in _compilerMessageQueue)
                {
                    foreach (var writer in writers)
                    {
                        writer.WriteLine(msg);
                    }
                }
            }
            finally
            {
                if (clearMessageQueue)
                {
                    ClearMessageQueue();
                }
            }
        }


        public int Compile(string outputFile)
        {
            LSLValidatorServiceProvider validatorServices = new LSLValidatorServiceProvider
            {
                ExpressionValidator = ExpressionValidator,
                StringLiteralPreProcessor = StringLiteralPreProcessor
            };


            validatorServices.SyntaxErrorListener = new SyntaxErrorListener(this);
            validatorServices.SyntaxWarningListener = new SyntaxWarningListener(this);


            var defaultProvider = new LSLEmbeddedLibraryDataProvider();

            foreach (var library in LibrarySubsets)
            {
                if (defaultProvider.SubsetDescriptions.ContainsKey(library))
                {
                    defaultProvider.ActiveSubsets.Add(library);
                }
                else
                {
                    WriteLine(CmdWarningHeader + "Library subset '{0}' does not exist and was ignored.", library);
                }
            }

            validatorServices.LibraryDataProvider = defaultProvider;


            var validator = new LSLCodeValidator(validatorServices);


            ILSLCompilationUnitNode validated;

            WriteLine(CmdNoticeHeader + "Compiling '{0}'...", _fileName);

            try
            {
                using (var infile = new StreamReader(_fileName))
                {
                    validated = validator.Validate(infile);


                    if (validator.HasSyntaxErrors)
                    {
                        WriteLine();
                        WriteLine("===============================");
                        WriteLine();
                        WriteLine("Syntax Errors:");
                        WriteLine();

                        foreach (var msg in _syntaxErrorMessages)
                        {
                            WriteLine(msg);
                        }

                        _syntaxErrorMessages.Clear();

                        WriteLine("===============================");
                        WriteLine();
                    }


                    if (validator.HasSyntaxWarnings)
                    {
                        if (!validator.HasSyntaxErrors)
                        {
                            WriteLine();
                            WriteLine("===============================");
                            WriteLine();
                        }

                        WriteLine("Syntax Warnings:");
                        WriteLine();

                        foreach (var msg in _syntaxWarningMessages)
                        {
                            WriteLine(msg);
                        }

                        _syntaxWarningMessages.Clear();

                        WriteLine("===============================");
                        WriteLine();
                    }

                    if (validator.HasSyntaxErrors)
                    {
                        WriteLine("Compilation phase did not start due to syntax errors.");
                        return ReturnCode.SyntaxErrors;
                    }
                }
            }
            catch (IOException error)
            {
                WriteLine(CmdErrorHeader + "Input File '{0}' could not be read from.", _fileName);
                WriteLine();
                WriteLine(CmdExceptionHeader + error.Message);
                return ReturnCode.InputFileUnreadable;
            }
            catch (UnauthorizedAccessException error)
            {
                WriteLine(CmdErrorHeader + "Input File '{0}' could not be read from.", _fileName);
                WriteLine();
                WriteLine(CmdExceptionHeader + error.Message);
                return ReturnCode.InputFileUnreadable;
            }
            catch (LSLCodeValidatorInternalException error)
            {
                WriteLine();
                WriteLine("Code Validator, internal error.");
                WriteLine();
                WriteLine(CmdExceptionHeader + error.Message);
                WriteLine();
                WriteLine(InternalErrorMessage);
                return ReturnCode.CodeValidatorInternalError;
            }
            catch (Exception error)
            {
                WriteLine();
                WriteLine("Code Validator, unknown error.");
                WriteLine();
                WriteLine(CmdExceptionHeader + error.Message);
                WriteLine();
                WriteLine(InternalErrorMessage);
                return ReturnCode.CodeValidatorUnknownError;
            }


            //==========
            // Compile Tree Into Code
            //==========


            LSLOpenSimCompilerSettings compilerSettings;


            if (!ClientCode)
            {
                compilerSettings =
                    LSLOpenSimCompilerSettings.OpenSimServerSideDefault();

                compilerSettings.ScriptHeader = ServerSideScriptCompilerHeader;
                compilerSettings.InsertCoOpTerminationCalls = CoOpStop;
            }
            else
            {
                compilerSettings =
                    LSLOpenSimCompilerSettings.OpenSimClientUploadable();

                compilerSettings.ScriptHeader = ClientSideScriptCompilerHeader;
                compilerSettings.InsertCoOpTerminationCalls = CoOpStop;
            }
            try
            {
                var dir = Path.GetDirectoryName(outputFile);

                if (!string.IsNullOrWhiteSpace(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (var outfile = File.Create(outputFile))
                {
                    var compiler = new LSLOpenSimCompiler(defaultProvider, compilerSettings);

                    compiler.Compile(validated, new StreamWriter(outfile, Encoding.UTF8));
                }

                WriteLine("Finished, output to \"" + outputFile + "\"");
            }
            catch (IOException error)
            {
                WriteLine(CmdErrorHeader + "Output File '{0}' could not be written to.", outputFile);
                WriteLine();
                WriteLine(CmdExceptionHeader + error.Message);
                return ReturnCode.OutputFileUnwritable;
            }
            catch (UnauthorizedAccessException error)
            {
                WriteLine(CmdErrorHeader + "Output File '{0}' could not be written to.", outputFile);
                WriteLine();
                WriteLine(CmdExceptionHeader + error.Message);
                return ReturnCode.OutputFileUnwritable;
            }
            catch (LSLCompilerInternalException error)
            {
                WriteLine();
                WriteLine("Compiler internal error:");
                WriteLine();
                WriteLine(CmdExceptionHeader + error.Message);
                WriteLine();
                WriteLine(InternalErrorMessage);
                return ReturnCode.CompilerInternalError;
            }
            catch (Exception error)
            {
                WriteLine();
                WriteLine("Compiler unknown error:");
                WriteLine();
                WriteLine(CmdExceptionHeader + error.Message);
                WriteLine();
                WriteLine(InternalErrorMessage);
                return ReturnCode.CompilerUnknownError;
            }

            return ReturnCode.Success;
        }
    }
}