#region FileInfo
// File: CompoundLibraryData.cs
// 
// Last Compile: 25/09/2015 @ 5:43 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// Copyright (c) 2015, Teriks
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
#endregion
#region Imports

using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Components;
using LibraryDataScrapingTools.ScraperInterfaces;

#endregion

namespace LibraryDataScrapingTools.ScraperProxys
{
    public class CompoundLibraryData : ILibraryData
    {
        private readonly List<ILibraryData> _providers = new List<ILibraryData>();

        public CompoundLibraryData(params ILibraryData[] libraryDataProviders)
        {
            foreach (var data in libraryDataProviders)
            {
                AddProvider(data);
            }
        }

        public bool LSLFunctionExist(string name)
        {
            return _providers.Any(x => x.LSLFunctionExist(name));
        }

        public bool LSLConstantExist(string name)
        {
            return _providers.Any(x => x.LSLConstantExist(name));
        }

        public bool LSLEventExist(string name)
        {
            return _providers.Any(x => x.LSLEventExist(name));
        }

        public IReadOnlyList<LSLLibraryFunctionSignature> LSLFunctionOverloads(string name)
        {
            return LSLFunctionOverloadGroups().First(x => x.First().Name == name);
        }

        public LSLLibraryConstantSignature LSLConstant(string name)
        {
            return _providers.First(x => x.LSLConstantExist(name)).LSLConstant(name);
        }

        public LSLLibraryEventSignature LSLEvent(string name)
        {
            return _providers.First(x => x.LSLEventExist(name)).LSLEvent(name);
        }

        public IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LSLFunctionOverloadGroups()
        {
            return LSLFunctions().GroupBy(x => x.Name).Select(x =>
            {
                var s = new HashSet<LSLLibraryFunctionSignature>();
                foreach (var f in x)
                {
                    if (!s.Contains(f))
                    {
                        s.Add(f);
                    }
                }
                return s.ToList();
            }).ToList();
        }

        public IEnumerable<LSLLibraryFunctionSignature> LSLFunctions()
        {
            var sigs = new HashSet<LSLLibraryFunctionSignature>();
            foreach (var libraryData in _providers)
            {
                sigs.UnionWith(libraryData.LSLFunctions());
            }
            return sigs;
        }

        public IEnumerable<LSLLibraryConstantSignature> LSLConstants()
        {
            var sigs = new HashSet<LSLLibraryConstantSignature>();
            foreach (var libraryData in _providers)
            {
                sigs.UnionWith(libraryData.LSLConstants());
            }
            return sigs;
        }

        public IEnumerable<LSLLibraryEventSignature> LSLEvents()
        {
            var sigs = new HashSet<LSLLibraryEventSignature>();
            foreach (var libraryData in _providers)
            {
                sigs.UnionWith(libraryData.LSLEvents());
            }
            return sigs;
        }

        public void AddProvider(ILibraryData p)
        {
            _providers.Add(p);
        }
    }
}