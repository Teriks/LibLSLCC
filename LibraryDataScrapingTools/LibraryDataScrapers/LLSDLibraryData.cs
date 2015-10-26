#region FileInfo
// 
// File: LLSDLibraryData.cs
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;
using LibLSLCC.LibraryData;
using LibraryDataScrapingTools.ScraperInterfaces;
using OpenMetaverse.StructuredData;

namespace LibraryDataScrapingTools.LibraryDataScrapers
{
    public class LLSDLibraryData : ILibraryData
    {
        private readonly IEnumerable<string> _subsets;


        private readonly OSDMap _data;
        public LLSDLibraryData(string filepath, IEnumerable<string> subsets)
        {
            _subsets = subsets;

            using (var file = File.OpenRead(filepath))
            {
                _data = (OSDMap)OSDParser.DeserializeLLSDXml(file);
            }
        }

        internal static OSDMap GetFunctionOSDMap(OSDMap data, string function)
        {
            var functions = GetFunctionsOSDMap(data);

            if (!functions.ContainsKey(function)) return null;

            var theFunction = (OSDMap)functions[function];

            return theFunction;
        }

        internal static OSDMap GetEventOSDMap(OSDMap data, string eventHandler)
        {
            var events = GetEventsOSDMap(data);

            if (!events.ContainsKey(eventHandler)) return null;

            var theEvent = (OSDMap)events[eventHandler];

            return theEvent;
        }

        internal static OSDMap GetConstantOSDMap(OSDMap data, string constant)
        {
            var constants = GetConstantsOSDMap(data);

            if (!constants.ContainsKey(constant)) return null;

            var theConstant = (OSDMap)constants[constant];

            if (!theConstant.ContainsKey("type")) return null;

            return theConstant;
        }

        internal static OSDMap GetConstantsOSDMap(OSDMap data)
        {
            if(!data.ContainsKey("constants")) return null;
            return (OSDMap)data["constants"];
        }

        internal static OSDMap GetFunctionsOSDMap(OSDMap data)
        {
            if (!data.ContainsKey("functions")) return null;
            return (OSDMap)data["functions"];
        }

        internal static OSDMap GetEventsOSDMap(OSDMap data)
        {
            if (!data.ContainsKey("events")) return null;
            return (OSDMap)data["events"];
        }

        public static string DocumentFunction(OSDMap data, string function)
        {

            var node = GetFunctionOSDMap(data, function);
            if (node == null) return null;

            return node.ContainsKey("tooltip") ? node["tooltip"].AsString() : null;
        }

        public static string DocumentEvent(OSDMap data, string eventHandler)
        {
            var node = GetEventOSDMap(data, eventHandler);
            if (node == null) return null;

            return node.ContainsKey("tooltip") ? node["tooltip"].AsString() : null;
        }

        public static string DocumentConstant(OSDMap data, string constant)
        {
            var node = GetConstantOSDMap(data, constant);
            if (node == null) return null;

            return node.ContainsKey("tooltip") ? node["tooltip"].AsString() : null;
        }


        public bool LSLFunctionExist(string name)
        {
            return GetFunctionOSDMap(_data, name) != null;
        }

        public bool LSLConstantExist(string name)
        {
            return GetConstantOSDMap(_data, name) != null;
        }

        public bool LSLEventExist(string name)
        {
            return GetEventOSDMap(_data, name) != null;
        }


        private string replaceStringCodePoints(string str)
        {

            str = Regex.Replace(str, "[Uu]\\+([a-fA-F0-9]{1,8}(?![Uu]))", (match =>
                ((char) int.Parse(match.Groups[1].Value, NumberStyles.HexNumber)).ToString()), RegexOptions.None);


            str = Regex.Replace(str, "0[xX]([a-fA-F0-9]{1,8}(?![xX]))", (match =>
                ((char) int.Parse(match.Groups[1].Value, NumberStyles.HexNumber)).ToString()), RegexOptions.None);

            return str;
        }

        private string replaceHexInts(string str)
        {
            str = Regex.Replace(str, "0[xX]([a-fA-F0-9]{1,8}(?![xX]))", (match =>
                int.Parse(match.Groups[1].Value, NumberStyles.HexNumber).ToString()), RegexOptions.None);

            return str;
        }


        public IReadOnlyGenericArray<LSLLibraryFunctionSignature> LSLFunctionOverloads(string name)
        {
            var map = GetFunctionOSDMap(_data, name);
            if (map == null) return new GenericArray<LSLLibraryFunctionSignature> ();

            LSLType returnType = LSLType.Void;

            if (map.ContainsKey("return") )
            {
                var value = map["return"].AsString();
                if (value != "void")
                {
                    returnType = LSLTypeTools.FromLSLTypeString(value);
                }

            }

            LSLLibraryFunctionSignature func = new LSLLibraryFunctionSignature(returnType,name)
            {
                DocumentationString = DocumentFunction(_data,name)
            };

            func.SetSubsets(_subsets);
            func.Deprecated = map.ContainsKey("deprecated");


            if (!map.ContainsKey("arguments")) return new GenericArray<LSLLibraryFunctionSignature> {func};



            var args = map["arguments"] as OSDArray;

            if (args == null) return new GenericArray<LSLLibraryFunctionSignature> {func};


            var paramNameDuplicates = new HashMap<string, int>();

            foreach (var arg in args.Cast<OSDMap>())
            {
                var argName = arg.Keys.First();

                var paramDetails = (OSDMap)arg[argName];

                if (paramNameDuplicates.ContainsKey(argName))
                {
                    //rename duplicates with a trailing number
                    int curValue = paramNameDuplicates[argName];
                    int newValue = curValue + 1;
                    paramNameDuplicates[argName] = newValue;

                    argName = argName + "_" + newValue;
                }
                else
                {
                    paramNameDuplicates.Add(argName,0);
                }


                func.AddParameter(new LSLParameter(
                    LSLTypeTools.FromLSLTypeString(paramDetails["type"].AsString()), argName, false));
            }

            return new GenericArray<LSLLibraryFunctionSignature> {func};
        }


        public LSLLibraryConstantSignature LSLConstant(string name)
        {
            var map = GetConstantOSDMap(_data, name);
            if (map == null) return null;

            if (!map.ContainsKey("type")) return null;

            LSLType type = LSLTypeTools.FromLSLTypeString(map["type"].AsString());

            if (!map.ContainsKey("value")) return null;

            
            var val = map["value"].AsString();


            switch (type)
            {
                case LSLType.String:
                    val = replaceStringCodePoints(val);
                    val = val.Replace("&quot;", "").
                              Replace("\"", "");
                    break;
                case LSLType.Integer:
                    val = replaceHexInts(val);
                    break;
                case LSLType.Rotation:
                case LSLType.Vector:
                    val = 
                        val.Replace("&lt;", "").
                        Replace("&gt;", "").
                        Replace("<","").
                        Replace(">","");
                    break;
                case LSLType.Key:
                    val = val.Replace("&quot;", "").
                              Replace("\"", "");
                    break;
            }

            var sig = new LSLLibraryConstantSignature(type, name,val)
            {
                DocumentationString = DocumentConstant(_data, name)
            };

            sig.SetSubsets(_subsets);
            sig.Deprecated = map.ContainsKey("deprecated");

            return sig;
        }

        public LSLLibraryEventSignature LSLEvent(string name)
        {
            var map = GetEventOSDMap(_data, name);
            if (map == null) return null;

            LSLLibraryEventSignature ev = new LSLLibraryEventSignature(name)
            {
                DocumentationString = DocumentEvent(_data, name)    
            };

            ev.SetSubsets(_subsets);

            ev.Deprecated = map.ContainsKey("deprecated");

            if (map.ContainsKey("arguments"))
            {
                var args = map["arguments"] as OSDArray;

                if (args == null) return ev;


                var paramNameDuplicates = new HashMap<string, int>();

                foreach (var arg in args.Cast<OSDMap>())
                {
                    var argName = arg.Keys.First();

                    var paramDetails = (OSDMap)arg[argName];

                    if (paramNameDuplicates.ContainsKey(argName))
                    {
                        //rename duplicates with a trailing number
                        int curValue = paramNameDuplicates[argName];
                        int newValue = curValue + 1;
                        paramNameDuplicates[argName] = newValue;

                        argName = argName + "_" + newValue;
                    }
                    else
                    {
                        paramNameDuplicates.Add(argName, 0);
                    }


                    ev.AddParameter(new LSLParameter(
                        LSLTypeTools.FromLSLTypeString(paramDetails["type"].AsString()), argName, false));
                }
            }

            return ev;
        }

        public IEnumerable<IReadOnlyGenericArray<LSLLibraryFunctionSignature>> LSLFunctionOverloadGroups()
        {
            return LSLFunctions().Select(x => new GenericArray<LSLLibraryFunctionSignature> {x});
        }

        public IEnumerable<LSLLibraryFunctionSignature> LSLFunctions()
        {
            var functions = GetFunctionsOSDMap(_data);
            if(functions == null ) yield break;


            foreach (var func in functions.Keys)
            {
                var function = LSLFunctionOverloads(func).FirstOrDefault();
                if(function == null) continue;

                yield return function;
            }
        }

        public IEnumerable<LSLLibraryConstantSignature> LSLConstants()
        {
            var constants = GetConstantsOSDMap(_data);
            if (constants == null) yield break;


            foreach (var constantName in constants.Keys)
            {
                var constant = LSLConstant(constantName);
                if(constant == null) continue;

                yield return constant;
            }
        }

        public IEnumerable<LSLLibraryEventSignature> LSLEvents()
        {
            var events = GetEventsOSDMap(_data);
            if (events == null) yield break;


            foreach (var eventName in events.Keys)
            {
                var ev = LSLEvent(eventName);
                if (ev == null) continue;

                yield return ev;
            }
        }
    }
}
