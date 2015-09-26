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
using System.IO;
using System.Reflection;
using System.Text;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Exceptions;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.Compilers;

#endregion

namespace lslcc
{
    internal class Program
    {
        private static void WriteHelp()
        {
            Console.WriteLine("Usage: lslcc -i script.lsl -o script.cs");
            Console.WriteLine("-i: input file");
            Console.WriteLine("-o: output file");
            Console.WriteLine("-h: help");
            Console.WriteLine("-v: lslcc, version and info");
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

        private static void WriteAbout()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("Author: Teriks");
            Console.WriteLine("License: GNU GPLv3");
            Console.WriteLine();
            Console.WriteLine("Compile Date: " + RetrieveLinkerTimestamp());
#if DEBUG
            Console.WriteLine("Build Type: DEBUG");
#else
            Console.WriteLine("Build Type: Release");
#endif
            Console.WriteLine("Version: 0.0.0");
            Console.WriteLine("State: Pre-Alpha, combined build");
            Console.WriteLine("=================================");
        }

        public static void Main(string[] args)
        {
            string inFile = null;
            string outFile = null;
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];


                if (arg.Substring(0, 1) == "-")
                {
                    if (arg.Substring(1) == "i")
                    {
                        if (File.Exists(args[i + 1]) && inFile == null)
                        {
                            inFile = args[i + 1];
                        }
                        else if (inFile == null)
                        {
                            Console.WriteLine("File \"" + args[i + 1] + "\" does not exist");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Input file specified multiple times, use -h for help");
                            return;
                        }
                    }
                    else if (arg.Substring(1) == "o")
                    {
                        if (outFile == null)
                        {
                            outFile = args[i + 1];
                        }
                        else
                        {
                            Console.WriteLine("Output file specified multiple times, use -h for help");
                            return;
                        }
                    }
                    else if (arg.Substring(1) == "h")
                    {
                        WriteHelp();
                        return;
                    }
                    else if (arg.Substring(1) == "v")
                    {
                        WriteAbout();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Unknown switch:" + arg + ", use -h for help");
                        return;
                    }
                }
            }

            if (inFile == null)
            {
                Console.WriteLine("Input file not specified, use -h for help.");
                return;
            }

            if (outFile == null)
            {
                Console.WriteLine("Output file not specified, use -h for help");
                return;
            }

            Console.WriteLine("================================================");
            Console.WriteLine("Compiling \"" + inFile + "\"...");
            Console.WriteLine();


            var validator = new LSLCodeValidator();
            ILSLCompilationUnitNode validated;


            try
            {
                try
                {
                    using (var infile = new StreamReader(inFile))
                    {
                        validated = validator.Validate(infile);

                        if (validator.HasSyntaxErrors)
                        {
                            Console.WriteLine("Compilation phase did not start due to syntax errors");
                            Console.WriteLine("================================================");
                            return;
                        }
                    }
                }
                catch (LSLCodeValidatorInternalError error)
                {
                    Console.WriteLine();
                    Console.WriteLine("Code Validator internal error: \"" + error.Message + "\"");
                    Console.WriteLine("Please report to the developer the code that caused this message.");
                    Console.WriteLine("================================================");
                    return;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine();
                Console.WriteLine("Code Validator unknown error: \"" + error.Message + "\"");
                Console.WriteLine("Please report to the developer the code that caused this message.");
                Console.WriteLine("================================================");
                return;
            }


            if (!validator.HasSyntaxErrors)
            {
                if (File.Exists(outFile))
                {
                    File.Delete(outFile);
                }
                using (var outfile = File.OpenWrite(outFile))
                {
                    var compiler = new LSLOpenSimCSCompiler(validator.ValidatorServices.MainLibraryDataProvider);
                    try
                    {
                        try
                        {
                            compiler.Compile(validated, new StreamWriter(outfile, Encoding.UTF8));
                        }
                        catch (LSLCompilerInternalException error)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Compiler internal error: \"" + error.Message + "\"");
                            Console.WriteLine("Please report to the developer the code that caused this message.");
                            Console.WriteLine("================================================");
                            return;
                        }
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Compiler unknown error: \"" + error.Message + "\"");
                        Console.WriteLine("Please report to the developer the code that caused this message.");
                        Console.WriteLine("================================================");
                        return;
                    }
                }


                Console.WriteLine("Finished, output to \"" + outFile + "\"");
                Console.WriteLine("================================================");
            }
            else
            {
                Console.WriteLine("Compilation phase did not start due to syntax errors");
                Console.WriteLine("================================================");
            }
        }
    }
}