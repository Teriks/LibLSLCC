#region FileInfo

// 
// File: LSLFunctionSignatureRegex.cs
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
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Regex tools for parsing <see cref="LSLFunctionSignature" /> objects from strings.
    /// </summary>
    public sealed class LSLFunctionSignatureRegex
    {
        /// <summary>
        ///     Construct a function signature regex, given an enumerable of acceptable LSL types, a string 'before' that is
        ///     prefixed to the regex, and a string 'after' that is appended to the regex.
        /// </summary>
        /// <param name="dataTypes">
        ///     Acceptable LSL types, or other types that can appear as a return type or parameter type in the
        ///     function signature.
        /// </param>
        /// <param name="before">The string pre-pended to the regex.</param>
        /// <param name="after">The string appended to the regex.</param>
        public LSLFunctionSignatureRegex(IEnumerable<string> dataTypes, string before, string after)

        {
            var types = "(?:" + string.Join("|", dataTypes) + ")";
            string id = LSLTokenTools.IDRegexString;
            Regex =
                new Regex(before + "(?:(" + types + ")\\s+)?(" + id + ")\\((\\s*(?:\\s*" + types + "\\s+" + id +
                          "\\s*(?:\\s*,\\s*" + types + "\\s+" + id + "\\s*)*)?)\\)" + after);
        }


        /// <summary>
        ///     Construct a function signature regex that accepts the standard LSL types for the return type and parameter types.
        /// </summary>
        /// <param name="before">The string pre-pended to the regex.</param>
        /// <param name="after">The string appended to the regex.</param>
        public LSLFunctionSignatureRegex(string before, string after) : this(new[]
        {
            "[vV]oid", "[sS]tring", "[kK]ey", "[fF]loat", "[iI]nteger", "[lL]ist", "[vV]ector", "[rR]otation",
            "[qQ]uaternion"
        }, before, after)
        {
        }


        /// <summary>
        ///     Construct a function signature regex that accepts the standard LSL types for the return type and parameter types.
        /// </summary>
        public LSLFunctionSignatureRegex()
            : this(new[]
            {
                "[vV]oid", "[sS]tring", "[kK]ey", "[fF]loat", "[iI]nteger", "[lL]ist", "[vV]ector", "[rR]otation",
                "[qQ]uaternion"
            }, "", "")
        {
        }


        /// <summary>
        ///     The function signature regex that was created upon construction.
        /// </summary>
        public Regex Regex { get; private set; }


        /// <summary>
        ///     Parse an LSLFunction signature from a string.
        /// </summary>
        /// <param name="inString">The string to parse the <see cref="LSLFunctionSignature" /> from.</param>
        /// <returns>The parsed <see cref="LSLFunctionSignature" />.</returns>
        public LSLFunctionSignature GetSignature(string inString)
        {
            return GetSignatures(inString).FirstOrDefault();
        }


        /// <summary>
        ///     Returns all LSLFunctionSignatures that could be parsed out of a given string.
        /// </summary>
        /// <param name="inString">The string to parse <see cref="LSLFunctionSignature" /> objects from.</param>
        /// <returns>An enumerable of <see cref="LSLFunctionSignature" /> objects that were successfully parsed from the string.</returns>
        public IEnumerable<LSLFunctionSignature> GetSignatures(string inString)
        {
            var matches = Regex.Matches(inString);
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    var returnTypeParam = LSLType.Void;

                    var returnType = m.Groups[1].ToString();
                    var name = m.Groups[2].ToString();
                    var param = m.Groups[3].ToString();

                    if (!string.IsNullOrWhiteSpace(returnType) && returnType.ToLower() != "void")
                    {
                        returnTypeParam = LSLTypeTools.FromLSLTypeName(returnType);
                    }


                    var sig = new LSLFunctionSignature(returnTypeParam, name);

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
                            sig.AddParameter(new LSLParameterSignature(LSLTypeTools.FromLSLTypeName(prm[0]), prm[1], false));
                        }
                        yield return sig;
                    }
                }
            }
        }
    }
}