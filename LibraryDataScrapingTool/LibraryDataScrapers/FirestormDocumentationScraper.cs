#region FileInfo
// 
// File: FirestormDocumentationScraper.cs
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

using System.Text.RegularExpressions;
using System.Xml;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.LibraryData;
using LibraryDataScrapingTools.LibraryDataScrapers.FirestormLibraryDataDom;
using LibraryDataScrapingTools.ScraperInterfaces;

#endregion

namespace LibraryDataScrapingTools.LibraryDataScrapers
{
    public class FirestormDocumentationScraper : IDocumentationProvider
    {
        private readonly LSLFunctionSignatureRegex _eventSig = new LSLFunctionSignatureRegex("", ";*");
        private readonly LSLFunctionSignatureRegex _functionSig = new LSLFunctionSignatureRegex("", ";*");
        private readonly ScriptLibrary _scriptLibrary;

        public FirestormDocumentationScraper(XmlReader reader)
        {
            _scriptLibrary = ScriptLibrary.Read(reader);
        }

        public FirestormDocumentationScraper(ScriptLibrary library)
        {
            _scriptLibrary = library;
        }

        public string DocumentFunction(LSLLibraryFunctionSignature function)
        {
            var scriptLibraryHasFunction = _scriptLibrary.Functions.Contains(function.Name);
            if (scriptLibraryHasFunction)
            {
                var doc = "";
                var hasDocSig = false;
                var hasMatchingDocSig = false;

                foreach (var f in _scriptLibrary.Functions.Get(function.Name))
                {
                    var matches = _functionSig.Regex.Matches(f.Desc);


                    foreach (Match match in matches)
                    {
                        if (match.Success)
                        {
                            hasDocSig = true;
                            var sig = LSLLibraryFunctionSignature.Parse(match.ToString());

                            doc = _functionSig.Regex.Replace(f.Desc, "");

                            if (!sig.SignatureEquivalent(function)) continue;

                            hasMatchingDocSig = true;

                            break;
                        }

                        doc = f.Desc;
                        hasDocSig = false;
                    }
                }
                /*
                if (function.Name == "lsSetWindlightScene")
                {
                    Console.WriteLine("test");
                }*/

                if (hasDocSig)
                {
                    if (hasMatchingDocSig)
                    {
                        Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Matching docstring signature found for function {1}",
                            _scriptLibrary.Name, function.Name);
                        return doc;
                    }
                    Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Docstring signature found for function {1} mismatches passed function signature",
                        _scriptLibrary.Name, function.Name);
                    return doc;
                }

                if (string.IsNullOrWhiteSpace(doc))
                {
                    Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Docstring for function {1} is empty",
                        _scriptLibrary.Name, function.Name);
                    return null;
                }

                Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Docstring found for function {1} did not contain a signature",
                    _scriptLibrary.Name, function.Name);
                return doc;
            }

            Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Function {1} not defined in firestorm data",
                _scriptLibrary.Name, function.Name);

            return null;
        }

        public string DocumentEvent(LSLLibraryEventSignature eventHandler)
        {
            if (!_scriptLibrary.Keywords.Contains(eventHandler.Name))
            {
                Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Event {1} not defined in firestorm script library",
                    _scriptLibrary.Name, eventHandler.Name);
                return null;
            }


            var e = _scriptLibrary.Keywords.Get(eventHandler.Name);
            if (string.IsNullOrWhiteSpace(e.Desc))
            {
                Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Docstring for event {1} is empty",
                    _scriptLibrary.Name, eventHandler.Name);
                return null;
            }

            var match = _eventSig.Regex.Match(e.Desc);
            if (match.Success)
            {
                var sig = LSLLibraryEventSignature.Parse(match.ToString());
                if (!sig.SignatureMatches(eventHandler))
                {
                    Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Docstring signature for event {1} mismatches passed event signature",
                        _scriptLibrary.Name, e.Name);
                }
                else
                {
                    Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Docstring signature for event {1} matches passed event signature",
                        _scriptLibrary.Name, e.Name);
                }
                return _functionSig.Regex.Replace(e.Desc, "");
            }

            Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Docstring for event {1} does not contain a signature",
                _scriptLibrary.Name, e.Name);


            return e.Desc;
        }

        public string DocumentConstant(LSLLibraryConstantSignature constant)
        {
            if (_scriptLibrary.Keywords.Contains(constant.Name))
            {
                var c = _scriptLibrary.Keywords.Get(constant.Name);
                if (string.IsNullOrWhiteSpace(c.Desc))
                {
                    Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Docstring for constant {1} is empty",
                        _scriptLibrary.Name, c.Name);
                    return null;
                }

                Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Docstring for constant {1} found in firestorm data",
                    _scriptLibrary.Name, c.Name);
                return c.Desc;
            }
            Log.WriteLineWithHeader("[FirestormDocumentationScraper]: ", "script_library name={0}: Constant {1} not defined in firestorm script library",
                _scriptLibrary.Name, constant.Name);
            return null;
        }
    }
}