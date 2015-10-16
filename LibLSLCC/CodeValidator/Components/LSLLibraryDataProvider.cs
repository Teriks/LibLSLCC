using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Utility;

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLLibraryDataProvider : ILSLMainLibraryDataProvider
    {
        //The query functions and properties of this class will be optimized
        //later after I get the correct live filtering behaviors down entirely

       
        /// <summary>
        /// If this is false, functions, constants and events which do not 
        /// belong to subsets in ActiveSubsets will be discarded upon adding
        /// them to the object
        /// </summary>
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
                
                var events = ActiveSubsets.Subsets.SelectMany<string, LSLLibraryEventSignature>(x =>
                {
                    Dictionary<string, LSLLibraryEventSignature> subsetContent;
                    if (_eventSignaturesBySubsetAndName.TryGetValue(x, out subsetContent))
                    {
                        return subsetContent.Values;
                    }
                    return new List<LSLLibraryEventSignature>();

                }).Distinct(new LambdaEqualityComparer<LSLLibraryEventSignature>(ReferenceEquals));

                var eventNames = new HashSet<string>();

                foreach (var evnt in events)
                {
                    if (!eventNames.Contains(evnt.Name))
                    {
                        eventNames.Add(evnt.Name);
                        yield return evnt;
                    }
                    else
                    {
                        throw new LSLDuplicateSignatureException(
                            "SupportedEventHandlers {get} failed because an event with the same name exist in more than one active subset, "+
                            "and it is not shared across the involved subsets.");
                    }
                }
            }
        }

        public IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LibraryFunctions
        {
            get
            {
                var funcs = new Dictionary<string, LSLLibraryFunctionSignature>();

                return ActiveSubsets.Subsets.SelectMany<string, List<LSLLibraryFunctionSignature>>(x =>
                {
                    Dictionary<string, List<LSLLibraryFunctionSignature>> subsetContent;
                    if (_functionSignaturesBySubsetAndName.TryGetValue(x, out subsetContent))
                    {
                        return subsetContent.Values;
                    }
                    return new List<List<LSLLibraryFunctionSignature>>();
                })
                .SelectMany(x=>x)
                .Distinct(new LambdaEqualityComparer<LSLLibraryFunctionSignature>(ReferenceEquals))
                .Select(x =>
                {
                    if (funcs.ContainsKey(x.Name))
                    {
                        var func = funcs[x.Name];
                        if (func.DefinitionIsDuplicate(x))
                        {
                            throw new LSLDuplicateSignatureException(
                                "LibraryFunctions {get} failed because a function with a duplicate or ambiguous signature exists in more than one active subset, " +
                                "and it is not shared across the involved subsets.");
                        }

                        //subset adds an overload
                        funcs.Add(x.Name, x);
                        return x;
                    }
                    //subset adds a new function
                    funcs.Add(x.Name, x);
                    return x;
                })
                .GroupBy(x=>x.Name).Select(x=>x.ToList());
            }
        }



        public IEnumerable<LSLLibraryConstantSignature> LibraryConstants
        {
            get
            {
                var constants = ActiveSubsets.Subsets.SelectMany<string, LSLLibraryConstantSignature>(x =>
                {
                    Dictionary<string, LSLLibraryConstantSignature> subsetContent;
                    if (_constantSignaturesBySubsetAndName.TryGetValue(x, out subsetContent))
                    {
                        return subsetContent.Values;
                    }
                    return new List<LSLLibraryConstantSignature>();

                }).Distinct(new LambdaEqualityComparer<LSLLibraryConstantSignature>(ReferenceEquals));


                var eventNames = new HashSet<string>();

                foreach (var cons in constants)
                {
                    if (!eventNames.Contains(cons.Name))
                    {
                        eventNames.Add(cons.Name);
                        yield return cons;
                    }
                    else
                    {
                        throw new LSLDuplicateSignatureException(
                            "LibraryConstants {get} failed because a constant with the same name exist in more than one active subset, " +
                            "and it is not shared across the involved subsets.");
                    }
                }
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



        private LSLLibraryEventSignature GetEventHandlerSignature(string name, IEnumerable<string> possibleSubsets)
        {
            LSLLibraryEventSignature result = null;

            foreach (var evnt in possibleSubsets
                .Where(x => _eventSignaturesBySubsetAndName.ContainsKey(x) && _eventSignaturesBySubsetAndName[x].ContainsKey(name))
                .Select(subset => _eventSignaturesBySubsetAndName[subset][name]))
            {
                if (result == null)
                {
                    result = evnt;
                }
                else if (!ReferenceEquals(result, evnt))
                {
                    throw new LSLDuplicateSignatureException(
                        string.Format(
                            "GetEventHandlerSignature for {0} failed because the more than one active subset had a duplicate definition of it, and the event handler was not shared across the involved subsets.",
                            name));

                }
            }

            return result;
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
                                "GetLibraryFunctionSignatures for {0} failed because the more than one active subset had a duplicate/ambiguous definition of it, "+
                                "and the function was not shared across the involved subsets.",
                                name));

                    }
                    results.Add(overload);
                }
            }

            //we want distinct by reference here because we do not want to return copies of the same object
            //that have been put into the _functionSignaturesBySubsetAndName because they are shared across subsets
            return results.Count == 0 ? null : results.Distinct(new LambdaEqualityComparer<LSLLibraryFunctionSignature>(ReferenceEquals)).ToList();
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
            LSLLibraryConstantSignature result = null;

            foreach (var constant in possibleSubsets
                .Where(x => _constantSignaturesBySubsetAndName.ContainsKey(x) && _constantSignaturesBySubsetAndName[x].ContainsKey(name))
                .Select(subset => _constantSignaturesBySubsetAndName[subset][name]))
            {
                if (result == null)
                {
                    result = constant;
                }
                else if(!ReferenceEquals(result, constant))
                {
                    throw new LSLDuplicateSignatureException(
                        string.Format(
                            "GetLibraryConstantSignature for {0} failed because the more than one active subset had a duplicate definition of it, and the constant was not shared across the involved subsets.",
                            name));

                }
            }

            return result;
        }
    }
}
