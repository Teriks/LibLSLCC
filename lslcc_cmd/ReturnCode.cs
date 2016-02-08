#region FileInfo
// 
// File: ReturnCode.cs
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
using System.Collections.Generic;
using System.Linq;

namespace lslcc
{
    internal static class ReturnCode
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

        [ReturnCodeHelp("IO failure while trying to write to an output file.")]
        public const int
            OutputFileUnwritable = 5;

        [ReturnCodeHelp("IO failure while writing/accessing a log file.")]
        public const int
            LogFileAccessError = 6;

        [ReturnCodeHelp("An unknown option was passed to lslcc.")] public const int UnknownOption = 7;

        [ReturnCodeHelp("(ICE) Code Validator experienced a known internal error.")] public const int
            CodeValidatorInternalError = 8;

        [ReturnCodeHelp("(ICE) Code Validator experienced an un-handled internal error.")] public const int
            CodeValidatorUnknownError = 9;

        [ReturnCodeHelp("(ICE) Compiler experienced a known internal error.")] public const int
            CompilerInternalError = 10;

        [ReturnCodeHelp("(ICE) Compiler experienced an un-handled internal error.")] public const int
            CompilerUnknownError = 11;

        public static readonly Dictionary<int, string> ReturnCodeNameMap = new Dictionary<int, string>();


        static ReturnCode()
        {
            var fields = typeof (ReturnCode).GetFields().ToList();

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttributes(typeof (ReturnCodeHelp), false).ToList();
                if (attr.Count <= 0) continue;

                var help = attr[0] as ReturnCodeHelp;

                if (help == null) continue;


                ReturnCodeNameMap.Add((int) field.GetValue(null), field.Name);
            }
        }
    }
}