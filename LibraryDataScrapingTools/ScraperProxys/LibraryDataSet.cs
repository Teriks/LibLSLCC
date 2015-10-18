#region FileInfo
// 
// File: LibraryDataSet.cs
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
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.Collections;
using LibraryDataScrapingTools.ScraperInterfaces;

#endregion

namespace LibraryDataScrapingTools.ScraperProxys
{
    public class LibraryDataSet : ILibraryData
    {
        public LibraryDataSet(ILibraryData data)
        {
            OverloadsDictionary = new Dictionary<string, IReadOnlyList<LSLLibraryFunctionSignature>>();
            ConstantSet = new HashSet<LSLLibraryConstantSignature>();
            EventSet = new HashSet<LSLLibraryEventSignature>();
            FunctionSet = new HashSet<LSLLibraryFunctionSignature>();
            EventDictionary = new Dictionary<string, LSLLibraryEventSignature>();
            ConstantDictionary = new Dictionary<string, LSLLibraryConstantSignature>();

            foreach (var f in data.LSLFunctions())
            {
                if (OverloadsDictionary.ContainsKey(f.Name))
                {
                    ((List<LSLLibraryFunctionSignature>) OverloadsDictionary[f.Name]).Add(f);
                }
                else
                {
                    OverloadsDictionary.Add(f.Name, new List<LSLLibraryFunctionSignature> {f});
                }

                FunctionSet.Add(f);
            }

            foreach (var f in data.LSLEvents())
            {
                EventDictionary.Add(f.Name, f);
                EventSet.Add(f);
            }

            foreach (var f in data.LSLConstants())
            {
                ConstantDictionary.Add(f.Name, f);
                ConstantSet.Add(f);
            }
        }

        public Dictionary<string, IReadOnlyList<LSLLibraryFunctionSignature>> OverloadsDictionary { get; private set; }
        public Dictionary<string, LSLLibraryConstantSignature> ConstantDictionary { get; private set; }
        public Dictionary<string, LSLLibraryEventSignature> EventDictionary { get; private set; }
        public ISet<LSLLibraryFunctionSignature> FunctionSet { get; private set; }
        public ISet<LSLLibraryEventSignature> EventSet { get; private set; }
        public ISet<LSLLibraryConstantSignature> ConstantSet { get; private set; }

        public bool LSLFunctionExist(string name)
        {
            return OverloadsDictionary.ContainsKey(name);
        }

        public bool LSLConstantExist(string name)
        {
            return ConstantDictionary.ContainsKey(name);
        }

        public bool LSLEventExist(string name)
        {
            return EventDictionary.ContainsKey(name);
        }

        public IReadOnlyList<LSLLibraryFunctionSignature> LSLFunctionOverloads(string name)
        {
            return OverloadsDictionary[name];
        }

        public LSLLibraryConstantSignature LSLConstant(string name)
        {
            return ConstantDictionary[name];
        }

        public LSLLibraryEventSignature LSLEvent(string name)
        {
            return EventDictionary[name];
        }

        public IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LSLFunctionOverloadGroups()
        {
            return OverloadsDictionary.Values;
        }

        public IEnumerable<LSLLibraryFunctionSignature> LSLFunctions()
        {
            return FunctionSet;
        }

        public IEnumerable<LSLLibraryConstantSignature> LSLConstants()
        {
            return ConstantSet;
        }

        public IEnumerable<LSLLibraryEventSignature> LSLEvents()
        {
            return EventSet;
        }
    }
}