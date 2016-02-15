#region FileInfo

// 
// File: cs
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.LibraryData;

#endregion

namespace lslcc
{
    internal class Program
    {
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

            return !ReferenceEquals(fi, null);
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
            Console.WriteLine("Version: " + version);
            Console.WriteLine();
            Console.WriteLine("=================================");
        }




        public static int Main(string[] args)
        {
            var defaultLibraryDataProvider = new LSLEmbeddedLibraryDataProvider();

            ArgParser argParser = new ArgParser();


            argParser.AddSwitch(new ArgSwitchDesc("-i")
            {
                MinArgs = 1,
                HelpLine = "One or more input files or glob expressions (quote globs on *nix).",
            })
                .AddWriteBeforeShortHelp((sender, e) =>
                {
                    e.Writer.WriteLine();
                    e.Writer.WriteLine("LibLSLCC command line OpenSim LSL compiler");
                    e.Writer.WriteLine();
                    e.Writer.WriteLine();
                    e.Writer.WriteLine("Examples:");
                    e.Writer.WriteLine();
                    e.Writer.WriteLine(argParser.HelpDescriptionIndent + "lslcc -i script.lsl -o script.cs");
                    e.Writer.WriteLine();
                    e.Writer.WriteLine(
                        argParser.HelpDescriptionIndent +
                        "lslcc -i script.lsl dir{0}script2.lsl -o \"{{dir}}{{name}}.cs\"", Path.DirectorySeparatorChar);
                    e.Writer.WriteLine();
                    e.Writer.WriteLine(
                        argParser.HelpDescriptionIndent +
                        "lslcc -i \"dir{0}**{0}*.lsl\" \"dir2{0}*\" -o \"{{dir}}{{name}}.cs\"  (built in globing)",
                        Path.DirectorySeparatorChar);
                    e.Writer.WriteLine();
                    e.Writer.WriteLine();
                });


            argParser.AddSwitch(new ArgSwitchDesc("-o")
            {
                MinArgs = 1,
                MaxArgs = 1,
                HelpLine = "(Optional) output file or output file name template.",
                DescriptionLines =
                {
                    "If you do not specify an output file or output file template, the directory and",
                    "name of the input file is used and given a '.cs' extension (equivalent to -o \"{dir}{name}.cs\".",
                    "In that case, the output file is written to the directory the input file resides in.",
                    "",
                    "Note that the \"{dir}\" template expands to the directory the source code is in, with a trailing",
                    "directory separator at the end."
                }
            });

            argParser.AddSwitch(new ArgSwitchDesc("-log")
            {
                MinArgs = 1,
                MaxArgs = 1,
                HelpLine = "log file or log file name template.",
                DescriptionLines =
                {
                    "If this is a file name template such as \"{dir}{name}.log\", then a separate log file is created",
                    "for each input file.  Otherwise, all compiler output is appended to the specified file.",
                    "",
                    "The template \"{returncode}\" can also be used in the log file name, a textual representation of",
                    "the compiler return code will be written in its place.  See -returncodes for return code names."
                }
            });


            argParser.AddSwitch(new ArgSwitchDesc("-librarysubsets")
            {
                MinArgs = 1,
                MaxArgs = 1,
                HelpLine = "subset1;subset2;... (default is just lsl)",
                DescriptionLines =
                {
                    "Set the available library subsets when compiling ( Separated by semi-colons ; )",
                    "Spaces are not allowed between the names and the semi-colons unless you quote the string.",
                    "",
                    "All acceptable values are:",
                    "",
                    string.Join(";", defaultLibraryDataProvider.PossibleSubsets)
                }
            });



            argParser.AddSwitch(new ArgSwitchDesc("-clientcode")
            {
                MaxArgs = 0,
                HelpLine = "subset1;subset2;... (default is just lsl)",
                DescriptionLines =
                {
                    "Compile client uploadable code that works with all versions of OpenSim starting with 0.8, ",
                    "but does not support script resets.",
                    "",
                    "The default behavior of lslcc is to compile server side code, which cannot be uploaded",
                    "from the client unless the server your using is modified to allow it."
                }
            });


            argParser.AddSwitch(new ArgSwitchDesc("-coop-stop")
            {
                MaxArgs = 0,
                DescriptionLines =
                {
                    "Insert cooperative termination calls used by OpenSim when co-op script stop mode is enabled.",
                }
            });


            argParser.AddSwitch(new ArgSwitchDesc("-returncodes")
            {
                MustBeUsedAlone = true,
                MaxArgs = 0,
                HelpLine = "show lslcc return code descriptions."
            })
                .AddWriteAfterShortHelp((sender, e) => { e.Writer.WriteLine(); });


            argParser.AddSwitch(new ArgSwitchDesc("-h")
            {
                MustBeUsedAlone = true,
                MaxArgs = 1,
                HelpLine = "show lslcc general help."
            });

            argParser.AddSwitch(new ArgSwitchDesc("-v")
            {
                MustBeUsedAlone = true,
                MaxArgs = 0,
                HelpLine = "show lslcc version and program info."
            })
                .AddWriteAfterShortHelp((sender, e) => { e.Writer.WriteLine(); });


            Dictionary<string, ArgSwitch> prms;
            try
            {
                prms = argParser.ParseArgs(args).ToDictionary(x => x.Name);
            }
            catch (ArgParseException e)
            {
                Console.WriteLine(e.Message);
                return ReturnCode.InvalidOption;
            }


            if (prms.Count == 0)
            {
                Console.WriteLine("No arguments specified, see -h for help.");
                return ReturnCode.InvalidOption;
            }


            if (prms[""].Arguments.Count > 0)
            {
                Console.WriteLine("Invalid arguments, see -h for help.");
                return ReturnCode.UnknownOption;
            }


            if (prms.ContainsKey("-returncodes"))
            {
                WriteReturnCodes();
                return ReturnCode.Success;
            }
            if (prms.ContainsKey("-h"))
            {
                argParser.WriteHelp(Console.Out);
                return ReturnCode.Success;
            }
            if (prms.ContainsKey("-v"))
            {
                WriteAbout();
                return ReturnCode.Success;
            }

            

            ArgSwitch inputFiles;

            if (!prms.TryGetValue("-i", out inputFiles))
            {
                Console.WriteLine("Input file(s) not specified, use -i for help.");
                return ReturnCode.MissingInputFile;
            }



            ArgSwitch subsets;
            var specifiedLibrarySubsets = new HashSet<string>();

            if (!prms.TryGetValue("-librarysubsets", out subsets))
            {
                specifiedLibrarySubsets.Add("lsl");
            }
            else
            {
                var subsetString = subsets.Arguments.First();
                foreach (var subset in subsetString.Split(';'))
                {
                    if (subset == "lsl" && specifiedLibrarySubsets.Contains("os-lsl"))
                    {
                        Console.WriteLine("The library subset \"lsl\" cannot be used when the subset \"os-lsl\" is already specified.");
                        return ReturnCode.InvalidOption;
                    }

                    if (subset == "os-lsl" && specifiedLibrarySubsets.Contains("lsl"))
                    {
                        Console.WriteLine("The library subset \"os-lsl\" cannot be used when the subset \"lsl\" is already specified.");
                        return ReturnCode.InvalidOption;
                    }

                    specifiedLibrarySubsets.Add(subset);
                }
            }


            var specifiedFiles = new HashSet<string>();

            foreach (var file in inputFiles.Arguments.SelectMany(x => new Glob.Glob(x).Expand()))
            {
                if (specifiedFiles.Contains(file.FullName))
                {
                    Console.WriteLine("Input file \"{0}\" specified more than once.", file.FullName);
                    return ReturnCode.InvalidOption;
                }

                specifiedFiles.Add(file.FullName);
            }

            if (specifiedFiles.Count == 0)
            {
                Console.WriteLine("The specified input file(s) could not be found.");
                return ReturnCode.InvalidOption;
            }



            string logFileName = null;
            TextWriter singularLog = null;
            bool multiLog = false;

            ArgSwitch logFile;
            if (prms.TryGetValue("-log", out logFile))
            {
                logFileName = logFile.Arguments.First();

                if (!IsMultiFileOutputLogTemplate(logFileName))
                {
                    try
                    {
                        singularLog = new StreamWriter(logFileName) {AutoFlush = true};
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Could not open log file \"{0}\" for writing.", logFileName);
                        return ReturnCode.LogFileAccessError;
                    }
                }
                else
                {
                    multiLog = true;
                }
            }


            int returnCode = ReturnCode.Success;

            Console.WriteLine();

            try
            {
                bool firstFile = true;

                foreach (var file in specifiedFiles)
                {

                    specifiedFiles.Add(file);

                    var compiler = new InputFileCompiler(file)
                    {
                        LibrarySubsets = specifiedLibrarySubsets,
                        ClientCode = prms.ContainsKey("-clientcode"),
                        CoOpStop = prms.ContainsKey("-coop-stop")
                    };


                    ArgSwitch outFile;

                    var outputFile = file + ".cs";

                    if (prms.TryGetValue("-o", out outFile))
                    {
                        outputFile = outFile.Arguments.First();

                        if (IsMultiFileOutputTemplate(outputFile))
                        {
                            outputFile = ExpandOutputFileVars(file, outputFile);
                        }
                    }

                    returnCode = compiler.Compile(outputFile);

                    if (multiLog)
                    {
                        var outLogFile = ExpandOutputLogFileVars(file, logFileName, returnCode);

                        var directory = Path.GetDirectoryName(outLogFile);
                        if (directory != null) Directory.CreateDirectory(directory);

                        try
                        {
                            using (var writer = new StreamWriter(outLogFile) {AutoFlush = true})
                            {
                                if (!firstFile)
                                {
                                    WriteLogSeperator(Console.Out);
                                }
                                compiler.WriteMessageQueue(true, Console.Out, writer);
                            }
                        }
                        catch (IOException)
                        {
                            Console.WriteLine("NOTICE: Could not write log file \"{0}\" due to file access error.",
                                outLogFile);
                        }
                    }
                    else
                    {
                        if (singularLog != null)
                        {
                            try
                            {
                                if (!firstFile)
                                {
                                    WriteLogSeperator(Console.Out);
                                    WriteLogSeperator(singularLog);
                                }
                                compiler.WriteMessageQueue(true, Console.Out, singularLog);
                            }
                            catch (IOException)
                            {
                                Console.WriteLine("NOTICE: Could not write log file \"{0}\" due to file access error.", logFileName);
                            }
                        }
                        else
                        {
                            if (!firstFile)
                            {
                                WriteLogSeperator(Console.Out);
                            }
                            compiler.WriteMessageQueue(true);
                        }
                    }

                    firstFile = false;
                }

            }
            finally
            {
                if (singularLog != null)
                {
                    singularLog.Close();
                }
            }

            return returnCode;
        }


        private static void WriteLogSeperator(TextWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("__________________________________________");
            writer.WriteLine();
        }

        private static bool IsMultiFileOutputTemplate(string inFile)
        {
            return inFile.Contains("{name}") || inFile.Contains("{dir}");
        }

        private static bool IsMultiFileOutputLogTemplate(string inFile)
        {
            return inFile.Contains("{name}") || inFile.Contains("{dir}") || inFile.Contains("{returncode}");
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



        private static string ExpandOutputLogFileVars(string inFile, string outputLogFileTemplate, int returnCode)
        {
            string outFile = outputLogFileTemplate.Replace("{name}", Path.GetFileName(inFile));

            var directory = Path.GetDirectoryName(inFile);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                outFile = outFile.Replace("{dir}", directory + Path.DirectorySeparatorChar);
            }
            else
            {
                outFile = outFile.Replace("{dir}", "." + Path.DirectorySeparatorChar);
            }

            outFile = outFile.Replace("{returncode}", ReturnCode.ReturnCodeNameMap[returnCode]);

            return outFile;
        }

    }
}