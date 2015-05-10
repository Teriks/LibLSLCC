#region


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