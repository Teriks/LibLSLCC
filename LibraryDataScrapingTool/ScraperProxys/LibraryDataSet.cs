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
using LibLSLCC.LibraryData;
using LibraryDataScrapingTools.ScraperInterfaces;

#endregion

namespace LibraryDataScrapingTools.ScraperProxys
{
    public class LibraryDataSet : ILibraryData
    {
        public LibraryDataSet(ILibraryData data)
        {
            OverloadsHashMap = new HashMap<string, IReadOnlyGenericArray<LSLLibraryFunctionSignature>>();
            ConstantSet = new HashSet<LSLLibraryConstantSignature>();
            EventSet = new HashSet<LSLLibraryEventSignature>();
            FunctionSet = new HashSet<LSLLibraryFunctionSignature>();
            EventHashMap = new HashMap<string, LSLLibraryEventSignature>();
            ConstantHashMap = new HashMap<string, LSLLibraryConstantSignature>();

            foreach (var f in data.LSLFunctions())
            {
                if (OverloadsHashMap.ContainsKey(f.Name))
                {
                    ((GenericArray<LSLLibraryFunctionSignature>) OverloadsHashMap[f.Name]).Add(f);
                }
                else
                {
                    OverloadsHashMap.Add(f.Name, new GenericArray<LSLLibraryFunctionSignature> {f});
                }

                FunctionSet.Add(f);
            }

            foreach (var f in data.LSLEvents())
            {
                EventHashMap.Add(f.Name, f);
                EventSet.Add(f);
            }

            foreach (var f in data.LSLConstants())
            {
                ConstantHashMap.Add(f.Name, f);
                ConstantSet.Add(f);
            }
        }

        public HashMap<string, IReadOnlyGenericArray<LSLLibraryFunctionSignature>> OverloadsHashMap { get; private set; }
        public HashMap<string, LSLLibraryConstantSignature> ConstantHashMap { get; private set; }
        public HashMap<string, LSLLibraryEventSignature> EventHashMap { get; private set; }
        public ISet<LSLLibraryFunctionSignature> FunctionSet { get; private set; }
        public ISet<LSLLibraryEventSignature> EventSet { get; private set; }
        public ISet<LSLLibraryConstantSignature> ConstantSet { get; private set; }

        public bool LSLFunctionExist(string name)
        {
            return OverloadsHashMap.ContainsKey(name);
        }

        public bool LSLConstantExist(string name)
        {
            return ConstantHashMap.ContainsKey(name);
        }

        public bool LSLEventExist(string name)
        {
            return EventHashMap.ContainsKey(name);
        }

        public IReadOnlyGenericArray<LSLLibraryFunctionSignature> LSLFunctionOverloads(string name)
        {
            return OverloadsHashMap[name];
        }

        public LSLLibraryConstantSignature LSLConstant(string name)
        {
            return ConstantHashMap[name];
        }

        public LSLLibraryEventSignature LSLEvent(string name)
        {
            return EventHashMap[name];
        }

        public IEnumerable<IReadOnlyGenericArray<LSLLibraryFunctionSignature>> LSLFunctionOverloadGroups()
        {
            return OverloadsHashMap.Values;
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