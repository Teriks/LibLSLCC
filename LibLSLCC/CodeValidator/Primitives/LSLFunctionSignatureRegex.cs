using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.Primitives
{
    public sealed class LSLFunctionSignatureRegex
    {

        public Regex Regex { get; private set; }

        public LSLFunctionSignatureRegex(IEnumerable<string> dataTypes, string before, string after) 
            
        {
            
            string types = "(?:" + string.Join("|", dataTypes) + ")";
            const string id = "[a-zA-Z]+[a-zA-Z0-9_]*";
            this.Regex = new Regex(before + "(?:(" + types + ")\\s+)?(" + id + ")\\((\\s*(?:\\s*" + types + "\\s+" + id + "\\s*(?:\\s*,\\s*" + types + "\\s+" + id + "\\s*)*)?)\\)" + after);
        }

        public LSLFunctionSignatureRegex(string before, string after) : this (new[]
        {
            "[vV]oid", "[sS]tring", "[kK]ey", "[fF]loat", "[iI]nteger", "[lL]ist", "[vV]ector", "[rR]otation",
            "[qQ]uaternion"
        }, before, after){

        }

        public LSLFunctionSignatureRegex()
            : this(new[]
            {
                "[vV]oid", "[sS]tring", "[kK]ey", "[fF]loat", "[iI]nteger", "[lL]ist", "[vV]ector", "[rR]otation",
                "[qQ]uaternion"
            }, "", "")
        {

        }

        public LSLFunctionSignature GetSignature(string inString)
        {
            return GetSignatures(inString).FirstOrDefault();
        }


        public class SimpleSignature : IEquatable<SimpleSignature>
        {
            public bool Equals(SimpleSignature other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Name, other.Name) && string.Equals(ReturnType, other.ReturnType) && Equals(Parameters, other.Parameters);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (Name != null ? Name.GetHashCode() : 0);
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

            public string Name { get; set; }
            public string ReturnType { get; set; }
            public List<KeyValuePair<string, string>> Parameters { get; private set; }

            public SimpleSignature()
            {
                ReturnType = "";
                Parameters=new List<KeyValuePair<string, string>>();
            }

            // override object.Equals
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((SimpleSignature) obj);
            }


        }

        public IEnumerable<SimpleSignature> GetSimpleSignatures(string inString)
        {
            var matches = Regex.Matches(inString);
            foreach (Match m in matches)
            {
                if (m.Success)
                {

                    var returnTypeParam = "void";

                    string returnType = m.Groups[1].ToString();
                    string name = m.Groups[2].ToString();
                    string param = m.Groups[3].ToString();

                    if (!string.IsNullOrWhiteSpace(returnType) && returnType.ToLower() != "void")
                    {
                        returnTypeParam = returnType;
                    }


                    var sig = new SimpleSignature();
                    sig.ReturnType = returnTypeParam;
                    sig.Name = name;

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

            var matches = this.Regex.Matches(inString);
            foreach (Match m in matches)
            {
                if (m.Success)
                {

                    var returnTypeParam=LSLType.Void;

                    string returnType = m.Groups[1].ToString();
                    string name = m.Groups[2].ToString();
                    string param = m.Groups[3].ToString();

                    if (!string.IsNullOrWhiteSpace(returnType) && returnType.ToLower() != "void")
                    {
                        returnTypeParam = LSLTypeTools.FromLSLTypeString(returnType);
                    }

                    
                    var sig = new LSLFunctionSignature(returnTypeParam,name);

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
                            sig.AddParameter(new LSLParameter(LSLTypeTools.FromLSLTypeString(prm[0]), prm[1],false));
                        }
                        yield return sig;
                    }
                   
                }
            }

        } 
    }
}