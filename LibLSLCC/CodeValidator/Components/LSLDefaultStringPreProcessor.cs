#region FileInfo
// 
// 
// File: LSLDefaultStringPreProcessor.cs
// 
// Last Compile: 25/09/2015 @ 11:47 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using LibLSLCC.CodeValidator.Components.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLDefaultStringPreProcessor : ILSLStringPreProcessor
    {
        private readonly List<LSLStringCharacterError> _illegalCharacters = new List<LSLStringCharacterError>();
        private readonly List<LSLStringCharacterError> _invalidEscapeCodes = new List<LSLStringCharacterError>();

        public LSLDefaultStringPreProcessor()
        {
            HasErrors = false;
            Result = "";
        }

        public bool HasErrors { get; private set; }

        public IEnumerable<LSLStringCharacterError> InvalidEscapeCodes
        {
            get { return _invalidEscapeCodes; }
        }

        public IEnumerable<LSLStringCharacterError> IllegalCharacters
        {
            get { return _illegalCharacters; }
        }

        public string Result { get; private set; }

        public void ProcessString(string stringLiteral)
        {
            var result = new StringBuilder();


            var itr = CStringUnescape(stringLiteral);


            foreach (var str in itr)
            {
                result.Append(str);
            }


            Result = HasErrors ? "" : result.ToString();
        }

        public void Reset()
        {
            _invalidEscapeCodes.Clear();
            _illegalCharacters.Clear();
            HasErrors = false;
            Result = "";
        }

        private IEnumerable<string> CStringUnescape(string str)
        {
            var sequentialEscapes = 0;

            for (var i = 0; i < str.Length;)
            {
                var chr = str[i];

                if (chr == '\\' && ((i + 1) < str.Length && (sequentialEscapes%2 == 0)))
                {
                    if (HasErrors) yield break;


                    var chr1 = str[i + 1];

                    var result = ReplaceEscapeCode(i, chr1);

                    if (HasErrors) yield break;

                    yield return result;
                    i += 2;
                }
                else if (chr == '\\')
                {
                    yield return "\\";
                    sequentialEscapes++;
                    i++;
                }
                else
                {
                    if (HasErrors) yield break;


                    var result = FilterCharacter(i, chr);

                    if (HasErrors) yield break;

                    yield return result;

                    sequentialEscapes = 0;
                    i++;
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "index")]
        private static string FilterCharacter(int index, char chr)
        {
            if (chr == '\n')
            {
                return "\\n";
            }
            if (chr == '\r')
            {
                return "\\r";
            }

            return chr.ToString(CultureInfo.InvariantCulture);
        }

        private string ReplaceEscapeCode(int index, char code)
        {
            if (code == 't')
            {
                return "    "; //4 spaces
            }

            if (code == 'n' || code == '"' || code == '\\')
            {
                return "\\" + code;
            }

            if (code == 'r')
            {
                return "r"; //don't even ask, I have no fucking idea why
            }

            HasErrors = true;

            _invalidEscapeCodes.Add(new LSLStringCharacterError(code, index));

            return "";
        }
    }
}