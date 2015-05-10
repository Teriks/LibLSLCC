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