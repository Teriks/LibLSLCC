#region FileInfo

// 
// File: LibraryDataSet.cs
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
            OverloadsDictionary = new UniqueValueDictionary<string, IReadOnlyList<LSLLibraryFunctionSignature>>();
            ConstantSet = new HashSet<LSLLibraryConstantSignature>();
            EventSet = new HashSet<LSLLibraryEventSignature>();
            FunctionSet = new HashSet<LSLLibraryFunctionSignature>();
            EventDictionary = new UniqueValueDictionary<string, LSLLibraryEventSignature>();
            ConstantDictionary = new UniqueValueDictionary<string, LSLLibraryConstantSignature>();

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

        public UniqueValueDictionary<string, IReadOnlyList<LSLLibraryFunctionSignature>> OverloadsDictionary { get; }
        public UniqueValueDictionary<string, LSLLibraryConstantSignature> ConstantDictionary { get; }
        public UniqueValueDictionary<string, LSLLibraryEventSignature> EventDictionary { get; }
        public ISet<LSLLibraryFunctionSignature> FunctionSet { get; }
        public ISet<LSLLibraryEventSignature> EventSet { get; }
        public ISet<LSLLibraryConstantSignature> ConstantSet { get; }

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