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
using LibLSLCC.CodeValidator.Enums;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    /// <summary>
    /// Regex tools for parsing LSLFunctionSignature objects from strings.
    /// </summary>
    public sealed class LSLFunctionSignatureRegex
    {

        /// <summary>
        /// Construct a function signature regex, given an enumerable of acceptable LSL types, a string 'before' that is prefixed to the regex, and a string 'after' that is appended to the regex.
        /// </summary>
        /// <param name="dataTypes">Acceptable LSL types, or other types that can appear as a return type or parameter type in the function signature.</param>
        /// <param name="before">The string pre-pended to the regex.</param>
        /// <param name="after">The string appended to the regex.</param>
        public LSLFunctionSignatureRegex(IEnumerable<string> dataTypes, string before, string after)

        {
            var types = "(?:" + string.Join("|", dataTypes) + ")";
            const string id = "[a-zA-Z]+[a-zA-Z0-9_]*";
            Regex =
                new Regex(before + "(?:(" + types + ")\\s+)?(" + id + ")\\((\\s*(?:\\s*" + types + "\\s+" + id +
                          "\\s*(?:\\s*,\\s*" + types + "\\s+" + id + "\\s*)*)?)\\)" + after);
        }

        /// <summary>
        /// Construct a function signature regex that accepts the standard LSL types for the return type and parameter types.
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
        /// Construct a function signature regex that accepts the standard LSL types for the return type and parameter types.
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
        /// The function signature regex that was created upon construction.
        /// </summary>
        public Regex Regex { get; private set; }


        /// <summary>
        /// Parse an LSLFunction signature from a string.
        /// </summary>
        /// <param name="inString">The string to parse the LSLFunctionSignature from.</param>
        /// <returns>The parsed LSLFunctionSignature.</returns>
        public LSLFunctionSignature GetSignature(string inString)
        {
            return GetSignatures(inString).FirstOrDefault();
        }

        /// <summary>
        /// Return SimpleSignature objects for function signatures that were matched inside of a given string.
        /// </summary>
        /// <param name="inString">The string to match function signatures in.</param>
        /// <returns>An enumerable of SimpleSignature objects which represent the function signatures found in the string.</returns>
        public IEnumerable<SimpleSignature> GetSimpleSignatures(string inString)
        {
            var matches = Regex.Matches(inString);
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    var returnTypeParam = "void";

                    var returnType = m.Groups[1].ToString();
                    var name = m.Groups[2].ToString();
                    var param = m.Groups[3].ToString();

                    if (!string.IsNullOrWhiteSpace(returnType) && returnType.ToLower() != "void")
                    {
                        returnTypeParam = returnType;
                    }


                    var sig = new SimpleSignature {ReturnType = returnTypeParam, Name = name};

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
                            sig.Parameters.Add(new KeyValuePair<string, string>(prm[0], prm[1]));
                        }
                        yield return sig;
                    }
                }
            }
        }

        /// <summary>
        /// Returns all LSLFunctionSignatures that could be parsed out of a given string.
        /// </summary>
        /// <param name="inString">The string to parse LSLFunctionSignature objects from.</param>
        /// <returns>An enumerable of LSLFunctionSignature objects that were successfully parsed from the string.</returns>
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
                        returnTypeParam = LSLTypeTools.FromLSLTypeString(returnType);
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
                            sig.AddParameter(new LSLParameter(LSLTypeTools.FromLSLTypeString(prm[0]), prm[1], false));
                        }
                        yield return sig;
                    }
                }
            }
        }

        /// <summary>
        /// Represents a simplified function signature
        /// </summary>
        public class SimpleSignature : IEquatable<SimpleSignature>
        {
            /// <summary>
            /// Constructs a blank SimpleSignature object.
            /// </summary>
            public SimpleSignature()
            {
                ReturnType = "";
                Parameters = new List<KeyValuePair<string, string>>();
            }

            /// <summary>
            /// Function name raw string.
            /// </summary>
            public string Name { get; set; }


            /// <summary>
            /// Function return type raw string.
            /// </summary>
            public string ReturnType { get; set; }


            /// <summary>
            /// List of key value pairs representing the raw parameter type and name strings for each parsed parameter if there are any.
            /// </summary>
            public List<KeyValuePair<string, string>> Parameters { get; private set; }


            /// <summary>
            /// Compares this SimpleSignatures name, return type and parameter type strings to another SimpleSignatures.
            /// </summary>
            /// <param name="other">The other SimpleSignature to compare this one to.</param>
            /// <returns>True if the Name, Return type, and all parameter type strings of both SimpleSignature objects are equal.</returns>
            public bool Equals(SimpleSignature other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;


                bool parametersEqual = Parameters.Count == other.Parameters.Count;

                if (parametersEqual)
                {
                    int index = 0;
                    foreach (var param in Parameters)
                    {
                        if (other.Parameters[index].Key == param.Key)
                        {
                            parametersEqual = false;
                            break;
                        }
                        index++;
                    }
                }

                return string.Equals(Name, other.Name) && string.Equals(ReturnType, other.ReturnType) && parametersEqual;
            }

            /// <summary>
            /// Derives a hash code for the SimpleSignature object by using its Name, ReturnType and Parameters enumerable.
            /// </summary>
            /// <returns>The hash code for this SimpleSignature.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (Name != null ? Name.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (ReturnType != null ? ReturnType.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (Parameters != null ? Parameters.GetHashCode() : 0);
                    return hashCode;
                }
            }

            /// <summary>
            /// Compares SimpleSignature for equality using the Equals method.
            /// </summary>
            /// <param name="left">The SimpleSignature on the left side of the equality comparison.</param>
            /// <param name="right">The SimpleSignature on the right side of the equality comparison.</param>
            /// <returns>True if the SimpleSignatures are equal to each other.</returns>
            public static bool operator ==(SimpleSignature left, SimpleSignature right)
            {
                return Equals(left, right);
            }

            /// <summary>
            /// Compares SimpleSignature for in-equality using the negated value from the Equals method.
            /// </summary>
            /// <param name="left">The SimpleSignature on the left side of the in-equality comparison.</param>
            /// <param name="right">The SimpleSignature on the right side of the in-equality comparison.</param>
            /// <returns>True if the SimpleSignatures are not equal to each other.</returns>
            public static bool operator !=(SimpleSignature left, SimpleSignature right)
            {
                return !Equals(left, right);
            }


            /// <summary>
            /// Compares this SimpleSignatures name, return type and parameter type strings to another SimpleSignatures.
            /// </summary>
            /// <param name="obj">The other SimpleSignature to compare this one to.</param>
            /// <returns>True if 'obj' is a SimpleSignature object and the Name, Return type, and all parameter type strings of both SimpleSignature objects are equal.</returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((SimpleSignature) obj);
            }
        }
    }
}