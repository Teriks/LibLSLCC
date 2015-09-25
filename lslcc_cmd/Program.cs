#region FileInfo

// 
// File: Program.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:26 PM
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