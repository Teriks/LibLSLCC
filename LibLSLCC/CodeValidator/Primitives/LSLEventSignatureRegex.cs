#region FileInfo
// 
// File: LSLEventSignatureRegex.cs
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
using System.Linq;
using System.Text.RegularExpressions;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.LibraryData;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{

    /// <summary>
    /// Regex tools for parsing <see cref="LSLEventSignature "/> objects from strings.
    /// </summary>
    public sealed class LSLEventSignatureRegex
    {

        /// <summary>
        /// Construct an event signature regex, given an enumerable of acceptable LSL types, a string 'before' that is prefixed to the regex, and a string 'after' that is appended to the regex.
        /// </summary>
        /// <param name="dataTypes">Acceptable LSL types, or other types that can appear as parameter types in the event signature.</param>
        /// <param name="before">The string pre-pended to the regex.</param>
        /// <param name="after">The string appended to the regex.</param>
        public LSLEventSignatureRegex(IEnumerable<string> dataTypes, string before, string after)
        {
            var types = "(?:" + string.Join("|", dataTypes) + ")";
            const string id = "[a-zA-Z]+[a-zA-Z0-9_]*";
            Regex =
                new Regex(before + "(" + id + ")\\((\\s*(?:\\s*" + types + "\\s+" + id + "\\s*(?:\\s*,\\s*" + types +
                          "\\s+" + id + "\\s*)*)?)\\)" + after);
        }


        /// <summary>
        /// Construct a event signature regex that accepts the standard LSL types for the parameter types.
        /// </summary>
        /// <param name="before">The string pre-pended to the regex.</param>
        /// <param name="after">The string appended to the regex.</param>
        public LSLEventSignatureRegex(string before, string after)
            : this(new[]
            {
                "[vV]oid", "[sS]tring", "[kK]ey", "[fF]loat", "[iI]nteger", "[lL]ist", "[vV]ector", "[rR]otation",
                "[qQ]uaternion"
            }, before, after)
        {
        }


        /// <summary>
        /// Construct a event signature regex that accepts the standard LSL types for the parameter types.
        /// </summary>
        public LSLEventSignatureRegex()
            : this(new[]
            {
                "[vV]oid", "[sS]tring", "[kK]ey", "[fF]loat", "[iI]nteger", "[lL]ist", "[vV]ector", "[rR]otation",
                "[qQ]uaternion"
            }, "", "")
        {
        }

        /// <summary>
        /// The event signature regex that was created upon the construction of this object
        /// </summary>
        public Regex Regex { get; private set; }


        /// <summary>
        /// Parse an <see cref="LSLEventSignature "/> signature from a string.
        /// </summary>
        /// <param name="inString">The string to parse the <see cref="LSLEventSignature "/> from.</param>
        /// <returns>The parsed <see cref="LSLEventSignature "/>.</returns>
        public LSLEventSignature GetSignature(string inString)
        {
            return GetSignatures(inString).FirstOrDefault();
        }


        /// <summary>
        /// Returns all <see cref="LSLEventSignature "/> that could be parsed out of a given string.
        /// </summary>
        /// <param name="inString">The string to parse <see cref="LSLEventSignature "/> objects from.</param>
        /// <returns>An enumerable of <see cref="LSLEventSignature "/> objects that were successfully parsed from the string.</returns>
        public IEnumerable<LSLEventSignature> GetSignatures(string inString)
        {
            var matches = Regex.Matches(inString);
            foreach (Match m in matches)
            {
                if (!m.Success) continue;

                var name = m.Groups[1].ToString();
                var param = m.Groups[2].ToString();


                var sig = new LSLLibraryEventSignature(name);

                var ps = param.Split(',');

                if (ps.Length == 1 && string.IsNullOrWhiteSpace(ps[0]))
                {
                    yield return sig;
                }
                else
                {
                    foreach (var p in ps)
                    {
                        var prm = p.Trim().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                        sig.AddParameter(new LSLParameter(LSLTypeTools.FromLSLTypeString(prm[0]), prm[1], false));
                    }
                    yield return sig;
                }
            }
        }
    }
}