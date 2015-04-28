using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.Primitives
{
    public sealed class LSLEventSignatureRegex
    {

        public Regex Regex { get; private set; }

        public LSLEventSignatureRegex(IEnumerable<string> dataTypes, string before, string after)
        {

            string types = "(?:" + string.Join("|", dataTypes) + ")";
            const string id = "[a-zA-Z]+[a-zA-Z0-9_]*";
            Regex=new Regex(before + "(" + id + ")\\((\\s*(?:\\s*" + types + "\\s+" + id + "\\s*(?:\\s*,\\s*" + types + "\\s+" + id + "\\s*)*)?)\\)" + after);
        }

        public LSLEventSignatureRegex(string before, string after)
            : this(new[]
            {
                "[vV]oid", "[sS]tring", "[kK]ey", "[fF]loat", "[iI]nteger", "[lL]ist", "[vV]ector", "[rR]otation",
                "[qQ]uaternion"
            }, before, after)
        {

        }

        public LSLEventSignatureRegex()
            : this(new[]
            {
                "[vV]oid", "[sS]tring", "[kK]ey", "[fF]loat", "[iI]nteger", "[lL]ist", "[vV]ector", "[rR]otation",
                "[qQ]uaternion"
            }, "", "")
        {

        }

        public LSLEventSignature GetSignature(string inString)
        {
            return GetSignatures(inString).FirstOrDefault();
        }

        public IEnumerable<LSLEventSignature> GetSignatures(string inString)
        {

            var matches = Regex.Matches(inString);
            foreach (Match m in matches)
            {
                if (m.Success)
                {



                    string name = m.Groups[1].ToString();
                    string param = m.Groups[2].ToString();




                    var sig = new LSLLibraryEventSignature( name);

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