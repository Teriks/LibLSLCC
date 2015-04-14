#region

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

        public UniqueValueDictionary<string, IReadOnlyList<LSLLibraryFunctionSignature>> OverloadsDictionary { get;
            private set; }

        public UniqueValueDictionary<string, LSLLibraryConstantSignature> ConstantDictionary { get; private set; }
        public UniqueValueDictionary<string, LSLLibraryEventSignature> EventDictionary { get; private set; }
        public ISet<LSLLibraryFunctionSignature> FunctionSet { get; private set; }
        public ISet<LSLLibraryEventSignature> EventSet { get; private set; }
        public ISet<LSLLibraryConstantSignature> ConstantSet { get; private set; }

        bool ILibraryData.LSLFunctionExist(string name)
        {
            return OverloadsDictionary.ContainsKey(name);
        }

        bool ILibraryData.LSLConstantExist(string name)
        {
            return ConstantDictionary.ContainsKey(name);
        }

        bool ILibraryData.LSLEventExist(string name)
        {
            return EventDictionary.ContainsKey(name);
        }

        IReadOnlyList<LSLLibraryFunctionSignature> ILibraryData.LSLFunctionOverloads(string name)
        {
            return OverloadsDictionary[name];
        }

        LSLLibraryConstantSignature ILibraryData.LSLConstant(string name)
        {
            return ConstantDictionary[name];
        }

        LSLLibraryEventSignature ILibraryData.LSLEvent(string name)
        {
            return EventDictionary[name];
        }

        IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> ILibraryData.LSLFunctionOverloadGroups()
        {
            return OverloadsDictionary.Values;
        }

        IEnumerable<LSLLibraryFunctionSignature> ILibraryData.LSLFunctions()
        {
            return FunctionSet;
        }

        IEnumerable<LSLLibraryConstantSignature> ILibraryData.LSLConstants()
        {
            return ConstantSet;
        }

        IEnumerable<LSLLibraryEventSignature> ILibraryData.LSLEvents()
        {
            return EventSet;
        }
    }
}