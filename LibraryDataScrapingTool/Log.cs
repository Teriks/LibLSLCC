#region FileInfo
// 
// File: Log.cs
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

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LibLSLCC.Collections;

namespace LibraryDataScrapingTool
{
    public static class Log
    {

        private static readonly object LogLock = new object();
        private static int _lastWrittenLineCount = 1;
        public static readonly GenericArray<TextWriter> LogWriters = new GenericArray<TextWriter>();

        public static void WriteLine(string format, params object[] args)
        {
            lock (LogLock)
            {
                format = Regex.Replace(format, @"\r", "");

                var lines = format.Count(x => x == '\n') + 1;

                foreach (var textWriter in LogWriters)
                {
                    if (_lastWrittenLineCount == 1 && lines > 1)
                    {
                        textWriter.WriteLine("============================", args);
                    }
                    else if (_lastWrittenLineCount > 1)
                    {
                        textWriter.WriteLine("============================", args);
                        
                    }
                    textWriter.WriteLine(format, args);
                }

                _lastWrittenLineCount = lines;
            }
        }

        public static void WriteLineWithHeader(string header, string format, params object[] args)
        {
            lock (LogLock)
            {
                header = Regex.Replace(header, @"\t|\n|\r", "");

                var spaces = header.Length + 1;


                format = Regex.Replace(format, @"\r", "");

                var lines = format.Split('\n');

                

                string r = "";
                int lineIndex = 0;
                foreach (var line in lines)
                {
                    if (lineIndex != 0)
                    {
                        for (int i = 0; i < spaces; i++)
                        {
                            r += " ";
                        }
                    }
                    else
                    {
                        r += " ";
                    }
                    if (lineIndex != (lines.Length - 1))
                    {
                        r += line + Environment.NewLine;
                    }
                    else
                    {
                        r += line;
                    }

                    lineIndex++;
                }

                foreach (var textWriter in LogWriters)
                {
                    if (_lastWrittenLineCount == 1 && lines.Length > 1)
                    {
                        textWriter.WriteLine("============================", args);
                    }
                    else if (_lastWrittenLineCount > 1)
                    {
                        textWriter.WriteLine("============================", args);

                    }
                    textWriter.WriteLine(header+r, args);

                }

                _lastWrittenLineCount = lines.Length;
            }
        }
    }
}