#region FileInfo

// 
// File: LSLFunctionSignatureRegex.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:24 PM
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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LibLSLCC.CodeValidator.Enums;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    public sealed class LSLFunctionSignatureRegex
    {
        public LSLFunctionSignatureRegex(IEnumerable<string> dataTypes, string before, string after)

        {
            var types = "(?:" + string.Join("|", dataTypes) + ")";
            const string id = "[a-zA-Z]+[a-zA-Z0-9_]*";
            Regex =
                new Regex(before + "(?:(" + types + ")\\s+)?(" + id + ")\\((\\s*(?:\\s*" + types + "\\s+" + id +
                          "\\s*(?:\\s*,\\s*" + types + "\\s+" + id + "\\s*)*)?)\\)" + after);
        }

        public LSLFunctionSignatureRegex(string before, string after) : this(new[]
        {
            "[vV]oid", "[sS]tring", "[kK]ey", "[fF]loat", "[iI]nteger", "[lL]ist", "[vV]ector", "[rR]otation",
            "[qQ]uaternion"
        }, before, after)
        {
        }

        public LSLFunctionSignatureRegex()
            : this(new[]
            {
                "[vV]oid", "[sS]tring", "[kK]ey", "[fF]loat", "[iI]nteger", "[lL]ist", "[vV]ector", "[rR]otation",
                "[qQ]uaternion"
            }, "", "")
        {
        }

        public Regex Regex { get; }

        public LSLFunctionSignature GetSignature(string inString)
        {
            return GetSignatures(inString).FirstOrDefault();
        }

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

        public class SimpleSignature : IEquatable<SimpleSignature>
        {
            public SimpleSignature()
            {
                ReturnType = "";
                Parameters = new List<KeyValuePair<string, string>>();
            }

            public string Name { get; set; }
            public string ReturnType { get; set; }
            public List<KeyValuePair<string, string>> Parameters { get; }

            public bool Equals(SimpleSignature other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Name, other.Name) && string.Equals(ReturnType, other.ReturnType) &&
                       Equals(Parameters, other.Parameters);
            }

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

            public static bool operator ==(SimpleSignature left, SimpleSignature right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(SimpleSignature left, SimpleSignature right)
            {
                return !Equals(left, right);
            }

            // override object.Equals
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