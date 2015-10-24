#region FileInfo
// 
// File: LSLDefaultStringPreProcessor.cs
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

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    /// The default implementation of <see cref="ILSLStringPreProcessor"/> for the library
    /// </summary>
    public class LSLDefaultStringPreProcessor : ILSLStringPreProcessor
    {

        private readonly GenericArray<LSLStringCharacterError> _invalidEscapeCodes = new GenericArray<LSLStringCharacterError>();

        /// <summary>
        /// Construct the default implementation of <see cref="ILSLStringPreProcessor"/>
        /// </summary>
        public LSLDefaultStringPreProcessor()
        {
            HasErrors = false;
            Result = "";
        }

        /// <summary>
        /// True if the string that was just pre-processed contains invalid escape sequences or illegal character errors.
        /// </summary>
        public bool HasErrors { get; private set; }


        /// <summary>
        /// An enumerable of all invalid escape sequences found in the string.
        /// </summary>
        public IEnumerable<LSLStringCharacterError> InvalidEscapeCodes
        {
            get { return _invalidEscapeCodes; }
        }

        /// <summary>
        /// An enumerable of all illegal characters found in the string.
        /// </summary>
        public IEnumerable<LSLStringCharacterError> IllegalCharacters
        {
            //None that I know of so far
            get { return new GenericArray<LSLStringCharacterError>(); }
        }

        /// <summary>
        /// The resulting string after the input string has been pre-processed.
        /// </summary>
        public string Result { get; private set; }



        /// <summary>
        /// Process the string and place descriptions of invalid escape codes in the InvalidEscapeCodes enumerable,
        /// Place illegal character errors in the IllegalCharacters enumerable.
        /// </summary>
        /// <param name="stringLiteral">The string literal to be processed, the string is expected to be wrapped in double quote characters still.</param>
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

        /// <summary>
        /// Reset the pre-processor so it can process another string
        /// </summary>
        public void Reset()
        {
            _invalidEscapeCodes.Clear();
            //_illegalCharacters.Clear();
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


                    var result = FilterCharacter(chr);

                    if (HasErrors) yield break;

                    yield return result;

                    sequentialEscapes = 0;
                    i++;
                }
            }
        }


        private static string FilterCharacter(char chr)
        {
            switch (chr)
            {
                case '\n':
                    return "\\n";
                case '\r':
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
                //don't even ask, I have no fucking idea why
                //the Linden LSL compiler turns "\r" into "r"
                //try it with llOwnerSay("\r");
                //or llOwnerSay((string)("\r"=="r"));
                return "r"; 
            }

            HasErrors = true;

            _invalidEscapeCodes.Add(new LSLStringCharacterError(code, index));

            return "";
        }
    }
}