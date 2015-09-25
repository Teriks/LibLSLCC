#region FileInfo

// 
// File: CompoundLibraryData.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:27 PM
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