using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLLibraryDataProvider : ILSLMainLibraryDataProvider
    {

        public bool LiveFiltering { get; private set; } 


        public LSLLibraryDataSubsetCollection ActiveSubsets { get; private set;}


        public IEnumerable<string> PossibleSubsets
        {
            get
            {
                return
                    _eventSignaturesBySubsetAndName.Keys.Union(_constantSignaturesBySubsetAndName.Keys)
                        .Union(_functionSignaturesBySubsetAndName.Keys);
            }
        } 

        public IEnumerable<LSLLibraryEventSignature> SupportedEventHandlers
        {
            get
            {
                return ActiveSubsets.Subsets.SelectMany<string, LSLLibraryEventSignature>(x =>
                {
                    Dictionary<string, LSLLibraryEventSignature> subsetContent;
                    if (_eventSignaturesBySubsetAndName.TryGetValue(x, out subsetContent))
                    {
                        return subsetContent.Values;
                    }
                    return new List<LSLLibraryEventSignature>();
                });
            }
        }

        public IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LibraryFunctions
        {
            get
            {
                return ActiveSubsets.Subsets.SelectMany<string, List<LSLLibraryFunctionSignature>>(x =>
                {
                    Dictionary<string, List<LSLLibraryFunctionSignature>> subsetContent;
                    if (_functionSignaturesBySubsetAndName.TryGetValue(x, out subsetContent))
                    {
                        return subsetContent.Values;
                    }
                    return new List<List<LSLLibraryFunctionSignature>>();
                });
            }
        }



        public IEnumerable<LSLLibraryConstantSignature> LibraryConstants
        {
            get
            {
                return ActiveSubsets.Subsets.SelectMany<string, LSLLibraryConstantSignature>(x =>
                {
                    Dictionary<string, LSLLibraryConstantSignature> subsetContent;
                    if (_constantSignaturesBySubsetAndName.TryGetValue(x, out subsetContent))
                    {
                        return subsetContent.Values;
                    }
                    return new List<LSLLibraryConstantSignature>();
                });
            }
        }

        private readonly Dictionary<string, Dictionary<string, List<LSLLibraryFunctionSignature>>>
            _functionSignaturesBySubsetAndName = new Dictionary<string, Dictionary<string, List<LSLLibraryFunctionSignature>>>();

        private readonly Dictionary<string, Dictionary<string, LSLLibraryConstantSignature>>
            _constantSignaturesBySubsetAndName = new Dictionary<string, Dictionary<string, LSLLibraryConstantSignature>>();

        private readonly Dictionary<string, Dictionary<string, LSLLibraryEventSignature>>
           _eventSignaturesBySubsetAndName = new Dictionary<string, Dictionary<string, LSLLibraryEventSignature>>();



        

        public void ClearLibraryFunctions()
        {
            _functionSignaturesBySubsetAndName.Clear();
        }

        public void ClearEventHandlers()
        {
            _eventSignaturesBySubsetAndName.Clear();
        }

        public void ClearLibraryConstants()
        {
            _constantSignaturesBySubsetAndName.Clear();
        }


        public void DefineEventHandler(LSLLibraryEventSignature signature)
        {
            var sig = GetEventHandlerSignature(signature.Name, PossibleSubsets);
            if (sig != null)
            {
                if (sig.Subsets.Overlaps(signature.Subsets))
                {
                    throw new LSLDuplicateSignatureException(
                        "Cannot defined an event handler with the same name more than once in the same subset, see: " +
                        sig.SignatureString);
                }
            }

            if (!LiveFiltering && !signature.Subsets.Overlaps(ActiveSubsets.Subsets))
            {
                //dont add it
                return;
            }

            foreach (var subset in signature.Subsets)
            {
                if (_eventSignaturesBySubsetAndName.ContainsKey(subset))
                {
                    _eventSignaturesBySubsetAndName[subset][signature.Name] = signature;
                }
                else
                {
                    _eventSignaturesBySubsetAndName[subset] = new Dictionary<string, LSLLibraryEventSignature> { {signature.Name,signature} };
                }
            }
            
        }


        public void DefineConstant(LSLLibraryConstantSignature signature)
        {
            var sig = GetLibraryConstantSignature(signature.Name, PossibleSubsets);
            if (sig != null)
            {
                if (sig.Subsets.Overlaps(signature.Subsets))
                {
                    throw new LSLDuplicateSignatureException(
                        "Cannot defined an constant with the same name more than once in the same subset, see: " +
                        sig.SignatureString);
                }
            }

            if (!LiveFiltering && !signature.Subsets.Overlaps(ActiveSubsets.Subsets))
            {
                //dont add it
                return;
            }


            foreach (var subset in signature.Subsets)
            {
                if (_constantSignaturesBySubsetAndName.ContainsKey(subset))
                {
                    _constantSignaturesBySubsetAndName[subset][signature.Name] = signature;
                }
                else
                {
                    _constantSignaturesBySubsetAndName[subset] = new Dictionary<string, LSLLibraryConstantSignature> { { signature.Name, signature } };
                }
            }
            
        }



        public void DefineFunction(LSLLibraryFunctionSignature signature)
        {
            var sigs = GetLibraryFunctionSignatures(signature.Name, PossibleSubsets);

            if (sigs != null)
            {
                var duplicate = sigs.FirstOrDefault(x => x.DefinitionIsDuplicate(signature));

                if (duplicate != null)
                {
                    if (duplicate.Subsets.Overlaps(signature.Subsets))
                    {
                        throw new LSLDuplicateSignatureException(
                            "Cannot define function as it is a duplicate of or ambiguous with another function in the same subset, attempted to add: " +
                            signature.SignatureString + ";, but: " + duplicate.SignatureString +
                            "; is considered a duplicate or ambiguous definition.");
                    }
                }

            }

            if (!LiveFiltering && !signature.Subsets.Overlaps(ActiveSubsets.Subsets))
            {
                //dont add it
                return;
            }


            foreach (var subset in signature.Subsets)
            {
                if (!_functionSignaturesBySubsetAndName.ContainsKey(subset))
                {
                    _functionSignaturesBySubsetAndName[subset] = new Dictionary<string, List<LSLLibraryFunctionSignature>>();
                }

                if (_functionSignaturesBySubsetAndName[subset].ContainsKey(signature.Name))
                {
                    _functionSignaturesBySubsetAndName[subset][signature.Name].Add(signature);
                }
                else
                {
                    _functionSignaturesBySubsetAndName[subset][signature.Name] = new List<LSLLibraryFunctionSignature> {signature};
                }
            }
        }



        public LSLLibraryDataProvider(bool liveFiltering = true)
        {
            LiveFiltering = liveFiltering;
            ActiveSubsets = new LSLLibraryDataSubsetCollection();

            ActiveSubsets.OnSubsetsChanged += ActiveSubsetsOnOnSubsetsChanged;
        }


        public LSLLibraryDataProvider(IEnumerable<string> activeSubsets, bool liveFiltering = true)
        {
            LiveFiltering = liveFiltering;
            ActiveSubsets = new LSLLibraryDataSubsetCollection(activeSubsets);

            ActiveSubsets.OnSubsetsChanged += ActiveSubsetsOnOnSubsetsChanged;
        }



        private void ActiveSubsetsOnOnSubsetsChanged(object o, string s)
        {
            if (!LiveFiltering)
            {
                throw new InvalidOperationException("Cannot change the active subsets of a LSLLibraryDataProvider when the object is not in LiveFiltering mode.");
            }
        }



        public bool EventHandlerExist(string name)
        {
            var match = 
                ActiveSubsets.Subsets.Where(x=>_eventSignaturesBySubsetAndName.ContainsKey(x))
                .FirstOrDefault(x => _eventSignaturesBySubsetAndName[x].ContainsKey(name));

            return match != null;
        }


        public LSLLibraryEventSignature GetEventHandlerSignature(string name)
        {
            return GetEventHandlerSignature(name, ActiveSubsets.Subsets);
        }



        private LSLLibraryEventSignature GetEventHandlerSignature(string name, IEnumerable<string> subsets)
        {
            foreach (var subset in subsets.Where(y => _eventSignaturesBySubsetAndName.ContainsKey(y)))
            {
                LSLLibraryEventSignature ev;
                if (_eventSignaturesBySubsetAndName[subset].TryGetValue(name, out ev))
                {
                    return ev;
                }
            }

            return null;
        }


        public bool LibraryFunctionExist(string name)
        {
            var match =
                ActiveSubsets.Subsets.Where(x => _functionSignaturesBySubsetAndName.ContainsKey(x))
                .FirstOrDefault(x => _functionSignaturesBySubsetAndName[x].ContainsKey(name));

            return match != null;
        }



        public bool IsConsideredOverload(LSLFunctionSignature signatureToTest)
        {
            var match =
               ActiveSubsets.Subsets.Where(x => _functionSignaturesBySubsetAndName.ContainsKey(x))
               .FirstOrDefault(x =>
               {
                   var subsetDict = _functionSignaturesBySubsetAndName[x];
                   //its not an overload its the first definition
                   if (!subsetDict.ContainsKey(signatureToTest.Name)) return false;

                   //if there is a duplicate this cannot be an overload, otherwise it is an overload
                   var duplicate = _functionSignaturesBySubsetAndName[x][signatureToTest.Name].FirstOrDefault(f=>f.DefinitionIsDuplicate(signatureToTest));
                   return duplicate == null;
               });

            return match != null;
        }



        public bool LibraryFunctionExist(LSLFunctionSignature signatureToTest)
        {
            var match =
               ActiveSubsets.Subsets.Where(x => _functionSignaturesBySubsetAndName.ContainsKey(x))
               .FirstOrDefault(x =>
               {
                   var subsetDict = _functionSignaturesBySubsetAndName[x];
                   //its does not exist
                   if (!subsetDict.ContainsKey(signatureToTest.Name)) return false;

                   //return true if a signature equivalent exists
                   return _functionSignaturesBySubsetAndName[x][signatureToTest.Name].FirstOrDefault(f => f.SignatureEquivalent(signatureToTest)) != null;
               });

            return match != null;
        }



        public IReadOnlyList<LSLLibraryFunctionSignature> GetLibraryFunctionSignatures(string name)
        {
            return GetLibraryFunctionSignatures(name, ActiveSubsets.Subsets);
        }


        private IReadOnlyList<LSLLibraryFunctionSignature> GetLibraryFunctionSignatures(string name, IEnumerable<string> subsets)
        {
            var results = new List<LSLLibraryFunctionSignature>();

            foreach (var subset in subsets.Where(x => _functionSignaturesBySubsetAndName.ContainsKey(x) && _functionSignaturesBySubsetAndName[x].ContainsKey(name)))
            {
                foreach (var overload in _functionSignaturesBySubsetAndName[subset][name])
                {

                    var overload1 = overload;
                    var duplicate = results.FirstOrDefault(x => x.DefinitionIsDuplicate(overload1));

                    //if its a reference to the same object, then the function was added with multiple subsets, its not an error
                    if (duplicate != null && !ReferenceEquals(overload,duplicate))
                    {
                        throw new LSLDuplicateSignatureException(
                            string.Format(
                                "GetLibraryFunctionSignatures for {0} failed because the more than one ActiveSubset had a duplicate/ambiguous definition of it.",
                                name));

                    }
                    results.Add(overload);
                }
            }

            return results.Count == 0 ? null : results;
        }


        public LSLLibraryFunctionSignature GetLibraryFunctionSignature(LSLFunctionSignature signatureToTest)
        {
            var sigs = GetLibraryFunctionSignatures(signatureToTest.Name);

            if (sigs == null) return null;

            return sigs.FirstOrDefault(x=>x.SignatureEquivalent(signatureToTest));
        }


        public bool LibraryConstantExist(string name)
        {
            var match =
                ActiveSubsets.Subsets.Where(x => _constantSignaturesBySubsetAndName.ContainsKey(x))
                .FirstOrDefault(x => _constantSignaturesBySubsetAndName[x].ContainsKey(name));

            return match != null;
        }


        public LSLLibraryConstantSignature GetLibraryConstantSignature(string name)
        {
            return GetLibraryConstantSignature(name, ActiveSubsets.Subsets);
        }


        private LSLLibraryConstantSignature GetLibraryConstantSignature(string name, IEnumerable<string> possibleSubsets)
        {
            return
                possibleSubsets.Where(
                    x =>
                        _constantSignaturesBySubsetAndName.ContainsKey(x) &&
                        _constantSignaturesBySubsetAndName[x].ContainsKey(name))
                    .Select(x => _constantSignaturesBySubsetAndName[x][name])
                    .FirstOrDefault();
        }
    }
}
