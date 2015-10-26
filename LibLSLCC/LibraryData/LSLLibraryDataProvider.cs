using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

namespace LibLSLCC.LibraryData
{


    /// <summary>
    /// The default base implementation of <see cref="ILSLLibraryDataProvider"/> which features optional live filtering of data
    /// and stores library information in memory. 
    /// </summary>
    public class LSLLibraryDataProvider : ILSLLibraryDataProvider
    {
        //The query functions and properties of this class will be optimized
        //later after I get the correct live filtering behaviors down entirely

       
        /// <summary>
        /// If this is false, functions, constants and events which do not 
        /// belong to subsets in ActiveSubsets will be discarded upon adding
        /// them to the object
        /// </summary>
        public bool LiveFiltering { get; private set; }

        /// <summary>
        /// The subsets of library data to present during query's.
        /// <para>
        /// If LiveFiltering is disabled, when classes that derive from this class try to call the Add* functions
        /// to add signatures, the signatures will be discarded if their Subset property does not overlap with this collection.
        /// 
        /// During live filtering, all data added from derived classes is accepted and kept in memory, the <see cref="ActiveSubsets"/> collection will
        /// then determine what subsets are actually presented during query's.
        /// 
        /// ActiveSubsets can only be changed after the construction of this object if LiveFiltering is enabled.
        /// 
        /// If you try to add a subset to the active subsets collection while LiveFiltering is enabled,
        /// and an <see cref="LSLLibrarySubsetDescription"/> object has not been added for that subset name;  An <see cref="LSLMissingSubsetDescriptionException"/> will be thrown by the collection.
        /// </para>
        /// </summary>
        public LSLLibraryDataSubsetCollection ActiveSubsets { get; private set;}



        /// <summary>
        /// All possible subset names in the library data currently loaded into this provider, taken from the <see cref="SubsetDescriptions"/> dictionary.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is only really useful if LiveFiltering mode is enabled.
        /// If the provider is not in live filtering mode, subset descriptions who's subsets do not overlap with the <see cref="ActiveSubsets"/> collection are discarded when they are added;
        /// Therefore they could never be a part of the <see cref="PossibleSubsets"/> collection.
        /// </para>
        /// </remarks>
        public IEnumerable<string> PossibleSubsets
        {
            get
            {
                return SubsetDescriptions.Values.Select(x=>x.Subset);
            }
        }


        /// <summary>
        /// Retrieves the friendly names of all the possible subsets that can be added to the <see cref="ActiveSubsets"/> collection of this library data provider.
        /// The friendly name values are retrieved from the <see cref="SubsetDescriptions"/> dictionary.
        /// </summary>
        public IEnumerable<string> PossibleSubsetsFriendlyNames
        {
            get { return SubsetDescriptions.Values.Select(x => x.FriendlyName); }
        }



        /// <summary>
        /// Retrieves the friendly names of the active subsets in this library data provider.
        /// The friendly name values are retrieved from the <see cref="SubsetDescriptions"/> dictionary.
        /// </summary>
        public IEnumerable<string> ActiveSubsetsFriendlyNames
        {
            get
            {
                return ActiveSubsets.Subsets.Select(x =>
                {
                    LSLLibrarySubsetDescription desc;
                    return SubsetDescriptions.TryGetValue(x, out desc) ? desc.FriendlyName : x;
                });
            }
        }


        /// <summary>
        /// Contains descriptions of all the subsets that signatures can possibly belong to.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The key to this dictionary is the subset name the description is associated with.
        /// 
        /// If a signature is added to the library data provider who's Subsets's property contains a subset
        /// without a description in this dictionary, an <see cref="LSLMissingSubsetDescriptionException"/> is thrown.
        /// 
        /// You can add descriptions for subsets using the <see cref="AddSubsetDescription"/> method of this class.
        /// </para>
        /// </remarks>
        public IReadOnlyHashMap<string, LSLLibrarySubsetDescription> SubsetDescriptions
        {
            get { return _subsetDescriptions; }
        }


        /// <summary>
        /// Returns all supported event handler signatures belonging to the current <see cref="ActiveSubsets"/>.
        /// <exception cref="LSLDuplicateSignatureException">
        /// If the current ActiveSubsets caused an event with a duplicate name to be loaded.  
        /// And that event was not shared across subsets.  This can only really happen when <see cref="LiveFiltering"/> is enabled.
        /// </exception>
        /// </summary>
        public IEnumerable<LSLLibraryEventSignature> SupportedEventHandlers
        {
            get
            {
                
                var events = ActiveSubsets.Subsets.SelectMany(x =>
                {
                    HashMap<string, LSLLibraryEventSignature> subsetContent;
                    return _eventSignaturesBySubsetAndName.TryGetValue(x, out subsetContent) ? subsetContent.Values : new GenericArray<LSLLibraryEventSignature>();

                })
                .Distinct(new LambdaEqualityComparer<LSLLibraryEventSignature>(ReferenceEquals));

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


        /// <summary>
        /// Returns all supported function signature overloads belonging to the current <see cref="ActiveSubsets"/>.
        /// <exception cref="LSLDuplicateSignatureException">
        /// If the current <see cref="ActiveSubsets"/> caused a function signature with duplicate/ambiguous definition to be retrieved.
        /// And that function was not shared across subsets.  This can only really happen when <see cref="LiveFiltering"/> is enabled.
        /// </exception>
        /// </summary>
        public IEnumerable<IReadOnlyGenericArray<LSLLibraryFunctionSignature>> LibraryFunctions
        {
            get
            {
                //This could really be optimized, need to make sure the basic gist of this works first for what I want though.

                var funcs = new HashMap<string, GenericArray<LSLLibraryFunctionSignature>>();

                return ActiveSubsets.Subsets.SelectMany(x =>
                {
                    HashMap<string, GenericArray<LSLLibraryFunctionSignature>> subsetContent;
                    return _functionSignaturesBySubsetAndName.TryGetValue(x, out subsetContent) ? subsetContent.Values : new GenericArray<GenericArray<LSLLibraryFunctionSignature>>();

                })
                .SelectMany(x=>x)
                .Distinct(new LambdaEqualityComparer<LSLLibraryFunctionSignature>(ReferenceEquals))
                .Select(x =>
                {
                    if (funcs.ContainsKey(x.Name))
                    {
                        var func = funcs[x.Name];

                        //Large amounts of overloads for a single function are not really expected.
                        if (func.Any( y=> y.DefinitionIsDuplicate(x)))
                        {
                            throw new LSLDuplicateSignatureException(
                                "LibraryFunctions {get} failed because a function with a duplicate or ambiguous signature exists in more than one active subset, " +
                                "and it is not shared across the involved subsets.");
                        }

                        //subset adds an overload
                        func.Add(x);
                        return x;
                    }
                    //subset adds a new function
                    funcs.Add(x.Name, new GenericArray<LSLLibraryFunctionSignature> {x});
                    return x;
                })
                .GroupBy(x=>x.Name).Select(x=>x.ToGenericArray());
            }
        }


        /// <summary>
        /// Returns all supported constant signatures for the current <see cref="ActiveSubsets"/>.
        /// <exception cref="LSLDuplicateSignatureException">
        /// If the current <see cref="ActiveSubsets"/> caused a constant with a duplicate name to be loaded.  
        /// And that constant was not shared across subsets.  This can only really happen when <see cref="LiveFiltering"/> is enabled.
        /// </exception>
        /// </summary>
        public IEnumerable<LSLLibraryConstantSignature> LibraryConstants
        {
            get
            {
                var constants = ActiveSubsets.Subsets.SelectMany(x =>
                {
                    HashMap<string, LSLLibraryConstantSignature> subsetContent;
                    return _constantSignaturesBySubsetAndName.TryGetValue(x, out subsetContent) ? subsetContent.Values : new GenericArray<LSLLibraryConstantSignature>();

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

        private readonly HashMap<string, HashMap<string, GenericArray<LSLLibraryFunctionSignature>>>
            _functionSignaturesBySubsetAndName = new HashMap<string, HashMap<string, GenericArray<LSLLibraryFunctionSignature>>>();

         

        private readonly HashMap<string, HashMap<string, LSLLibraryConstantSignature>>
            _constantSignaturesBySubsetAndName = new HashMap<string, HashMap<string, LSLLibraryConstantSignature>>();

        private readonly HashMap<string, HashMap<string, LSLLibraryEventSignature>>
           _eventSignaturesBySubsetAndName = new HashMap<string, HashMap<string, LSLLibraryEventSignature>>();

        private readonly HashMap<string, LSLLibrarySubsetDescription> _subsetDescriptions 
            = new HashMap<string, LSLLibrarySubsetDescription>();


        private readonly HashMap<string, LSLLibrarySubsetDescription> _candidateSubsetDescriptions 
            = new HashMap<string, LSLLibrarySubsetDescription>();


        /// <summary>
        /// Clear all library functions defined in this library data provider.
        /// </summary>
        public void ClearLibraryFunctions()
        {
            _functionSignaturesBySubsetAndName.Clear();
        }


        /// <summary>
        /// Clear all library event handlers defined in this library data provider.
        /// </summary>
        public void ClearEventHandlers()
        {
            _eventSignaturesBySubsetAndName.Clear();
        }


        /// <summary>
        /// Clear all library constants defined in this library data provider.
        /// </summary>
        public void ClearLibraryConstants()
        {
            _constantSignaturesBySubsetAndName.Clear();
        }


        /// <summary>
        /// Clear all subset descriptions defined in this library data provider.
        /// </summary>
        public void ClearSubsetDescriptions()
        {
            _subsetDescriptions.Clear();
        }

        /// <summary>
        /// Clears all library definitions and subset descriptions.
        /// </summary>
        public void ClearLibraryData()
        {
            ClearSubsetDescriptions();
            ClearEventHandlers();
            ClearLibraryConstants();
            ClearLibraryFunctions();
        }

        /// <summary>
        /// Add a subset description to this library data provider.
        /// </summary>
        /// <remarks>
        /// If <see cref="LiveFiltering"/> is not enabled, the subset description will be put into a holding area and not added to <see cref="SubsetDescriptions"/> until it is referenced by a signature.
        /// </remarks>
        /// <exception cref="LSLDuplicateSubsetDescriptionException">If a subset description for the same Subset name already exists.</exception>
        /// <param name="description">The subset description to add.</param>
        public void AddSubsetDescription(LSLLibrarySubsetDescription description)
        {
            HashMap<string, LSLLibrarySubsetDescription> dictRef;

            if (!LiveFiltering && !ActiveSubsets.Subsets.Contains(description.Subset))
            {
                dictRef = _candidateSubsetDescriptions;
            }
            else
            {
                dictRef = _subsetDescriptions;
            }


            if (dictRef.ContainsKey(description.Subset))
            {
                throw new LSLDuplicateSubsetDescriptionException(string.Format("Description for subset {0} already exists.", description.Subset));
            }

            dictRef.Add(description.Subset, description);

        }


        /// <summary>
        /// Adds multiple a subset descriptions to this library data provider.
        /// </summary>
        /// <remarks>
        /// If <see cref="LiveFiltering"/> is not enabled, the subset description will be put into a holding area and not added to <see cref="SubsetDescriptions"/> until it is referenced by a signature.
        /// </remarks>
        /// <exception cref="LSLDuplicateSubsetDescriptionException">If a subset description for the same Subset name already exists.</exception>
        /// <param name="descriptions">The subset descriptions to add.</param>
        public void AddSubsetDescriptions(IEnumerable<LSLLibrarySubsetDescription> descriptions)
        {
            foreach (var lslLibrarySubsetDescription in descriptions)
            {
                AddSubsetDescription(lslLibrarySubsetDescription);
            }
        }




        /// <summary>
        /// Define a library event handler signature
        /// </summary>
        /// <param name="signature">The LSLLibraryEventSignature representing the library event handler to be defined.</param>
        /// <exception cref="LSLMissingSubsetDescriptionException">Thrown if the event signatures <exception cref="LSLLibraryEventSignature.Subsets"/> property contains a subset that is not described in the library data providers <see cref="SubsetDescriptions"/> collection.</exception>
        /// <exception cref="LSLDuplicateSignatureException">If the event handler could not be defined because it's name existed in the same subset already.</exception>
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
                //don't add it
                return;
            }

            foreach (var subset in signature.Subsets.Where(subset => !_subsetDescriptions.ContainsKey(subset)))
            {
                LSLLibrarySubsetDescription candidate;
                if (LiveFiltering || !_candidateSubsetDescriptions.TryGetValue(subset,out candidate))
                {
                    throw new LSLMissingSubsetDescriptionException("Event signature: " + signature.SignatureString +
                                                                   "; belongs to the subset \"" + subset +
                                                                   "\" but that subset has no associated SubsetDescription.");
                }
                _subsetDescriptions.Add(subset, candidate);
            }

            foreach (var subset in signature.Subsets)
            {
                if (_eventSignaturesBySubsetAndName.ContainsKey(subset))
                {
                    _eventSignaturesBySubsetAndName[subset][signature.Name] = signature;
                }
                else
                {
                    _eventSignaturesBySubsetAndName[subset] = new HashMap<string, LSLLibraryEventSignature> { {signature.Name,signature} };
                }
            }
            
        }

        /// <summary>
        /// Define a library constant signature
        /// </summary>
        /// <param name="signature">The LSLLibraryConstantSignature representing the library constant to be defined.</param>
        /// <exception cref="LSLMissingSubsetDescriptionException">Thrown if the constant signatures <exception cref="LSLLibraryConstantSignature.Subsets"/> property contains a subset that is not described in the library data providers <see cref="SubsetDescriptions"/> collection.</exception>
        /// <exception cref="LSLDuplicateSignatureException">If the constant could not be defined because it's name existed in the same subset already.</exception>
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
                //don't add it
                return;
            }

            foreach (var subset in signature.Subsets.Where(subset => !_subsetDescriptions.ContainsKey(subset)))
            {
                LSLLibrarySubsetDescription candidate;
                if (LiveFiltering || !_candidateSubsetDescriptions.TryGetValue(subset, out candidate))
                {
                    throw new LSLMissingSubsetDescriptionException("Constant signature: " + signature.SignatureString +
                                                                   "; belongs to the subset \"" + subset +
                                                                   "\" but that subset has no associated SubsetDescription.");
                }
                _subsetDescriptions.Add(subset, candidate);
            }


            foreach (var subset in signature.Subsets)
            {
                if (_constantSignaturesBySubsetAndName.ContainsKey(subset))
                {
                    _constantSignaturesBySubsetAndName[subset][signature.Name] = signature;
                }
                else
                {
                    _constantSignaturesBySubsetAndName[subset] = new HashMap<string, LSLLibraryConstantSignature> { { signature.Name, signature } };
                }
            }
            
        }


        /// <summary>
        /// Define a library function signature
        /// </summary>
        /// <param name="signature">The LSLLibraryFunctionSignature representing the library function to be defined.</param>
        /// <exception cref="LSLMissingSubsetDescriptionException">Thrown if the function signatures <exception cref="LSLLibraryFunctionSignature.Subsets"/> property contains a subset that is not described in the library data providers <see cref="SubsetDescriptions"/> collection.</exception>
        /// <exception cref="LSLDuplicateSignatureException">If the function could not be defined because a duplicate or ambiguous definition existed in the same subset already.</exception>
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
                //don't add it
                return;
            }


            foreach (var subset in signature.Subsets.Where(subset => !_subsetDescriptions.ContainsKey(subset)))
            {
                LSLLibrarySubsetDescription candidate;
                if (LiveFiltering || !_candidateSubsetDescriptions.TryGetValue(subset, out candidate))
                {
                    throw new LSLMissingSubsetDescriptionException("Function signature: " + signature.SignatureString +
                                                                   "; belongs to the subset \"" + subset +
                                                                   "\" but that subset has no associated SubsetDescription.");
                }
                _subsetDescriptions.Add(subset, candidate);
            }


            foreach (var subset in signature.Subsets)
            {
                if (!_functionSignaturesBySubsetAndName.ContainsKey(subset))
                {
                    _functionSignaturesBySubsetAndName[subset] = new HashMap<string, GenericArray<LSLLibraryFunctionSignature>>();
                }

                if (_functionSignaturesBySubsetAndName[subset].ContainsKey(signature.Name))
                {
                    _functionSignaturesBySubsetAndName[subset][signature.Name].Add(signature);
                }
                else
                {
                    _functionSignaturesBySubsetAndName[subset][signature.Name] = new GenericArray<LSLLibraryFunctionSignature> {signature};
                }
            }
        }


        /// <summary>
        /// Construct an <see cref="LSLLibraryDataProvider"/> with the option of enabling live filtering mode.
        /// </summary>
        /// <param name="liveFiltering">Whether or not to enable <see cref="LiveFiltering"/> mode.  Default value is true.</param>
        public LSLLibraryDataProvider(bool liveFiltering = true)
        {
            LiveFiltering = liveFiltering;
            ActiveSubsets = new LSLLibraryDataSubsetCollection();

            ActiveSubsets.OnSubsetsChanged += ActiveSubsetsOnOnSubsetsChanged;
            ActiveSubsets.OnSubsetAdded += ActiveSubsetsOnOnSubsetAdded;
        }



        private void ActiveSubsetsOnOnSubsetAdded(object o, string subset)
        {
            if (!LiveFiltering) return;

            if (!SubsetDescriptions.ContainsKey(subset))
            {
                throw new LSLMissingSubsetDescriptionException(
                    "The subset \""+subset+
                    "\" cannot be added to the library data providers ActiveSubsets collection while live filtering because a SubsetDescription does not exist for that subset name");
            }
        }

        /// <summary>
        /// Construct an LSLLibraryDataProvider an initialize <see cref="ActiveSubsets"/> from the constructor parameter 'activeSubsets'.
        /// Optionally enable <see cref="LiveFiltering"/> mode using the 'liveFiltering' parameter.
        /// 
        /// </summary>
        /// <param name="activeSubsets">The subsets to add to the <see cref="ActiveSubsets"/> collection upon construction.</param>
        /// <param name="liveFiltering">Whether or not to enable <see cref="LiveFiltering"/> mode.  Default value is true.</param>
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


        /// <summary>
        /// Check if a library event handler with the given name is defined whiten this data provider.
        /// </summary>
        /// <param name="name">The name of the library event handler to query for existence.</param>
        /// <returns>True if the library event handler is defined, false if it is not.</returns>
        public bool EventHandlerExist(string name)
        {
            var match = 
                ActiveSubsets.Subsets.Where(x=>_eventSignaturesBySubsetAndName.ContainsKey(x))
                .FirstOrDefault(x => _eventSignaturesBySubsetAndName[x].ContainsKey(name));

            return match != null;
        }



        /// <summary>
        /// Attempt to return a library event handler signature with the given name if it is defined in this data provider, other wise return null.
        /// </summary>
        /// <param name="name">The name of the library event handler signature to return.</param>
        /// <returns>The signature if an event with the given name is defined, otherwise null.</returns>
        /// <exception cref="LSLDuplicateSignatureException">
        /// Thrown if <see cref="LiveFiltering"/> is enabled and more than one active subset contains a definition of an event handler with this name that is not shared across those subsets.
        /// </exception>
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


        /// <summary>
        /// Check if a library function with the given name is defined whiten this data provider.
        /// </summary>
        /// <param name="name">The name of the library function to query for existence.</param>
        /// <returns>True if the library function is defined, false if it is not.</returns>
        public bool LibraryFunctionExist(string name)
        {
            var match =
                ActiveSubsets.Subsets.Where(x => _functionSignaturesBySubsetAndName.ContainsKey(x))
                .FirstOrDefault(x => _functionSignaturesBySubsetAndName[x].ContainsKey(name));

            return match != null;
        }


        /// <summary>
        /// Check if a given function signature would be considered an overload to any function given the current <see cref="ActiveSubsets"/>.
        /// </summary>
        /// <param name="signatureToTest">The signature to test.</param>
        /// <returns>True if non-(duplicate/ambiguous) definition of a function signature with the same name as the given signature exist in this library data provider. </returns>
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


        /// <summary>
        /// Check if this library data provider contains a function with an exact signature match to the given function signature among the current <see cref="ActiveSubsets"/>.
        /// </summary>
        /// <param name="signatureToTest">The function signature to try to find exact match with.</param>
        /// <returns>True if a function exists in the current active subsets that exactly matches the given signature.</returns>
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


        /// <summary>
        /// Attempt to return a list of overloads for a function that might be defined in the library data provider given its name.
        /// </summary>
        /// <param name="name">The name of the function to attempt to return a list of overloads for.</param>
        /// <returns>
        /// A list of function signatures that represent the overloads among the active subsets for a function with a given name, or null if no function with that name exists.
        /// If the function exists but has no overloads, only one item will be returned in the list.
        /// </returns>
        /// <exception cref="LSLDuplicateSignatureException">
        /// Thrown if <see cref="LiveFiltering"/> is enabled and more than one active subset contains a duplicate definition of the function or one of its overloads,
        /// and the function/overload is not shared across those subsets.
        /// </exception>
        public IReadOnlyGenericArray<LSLLibraryFunctionSignature> GetLibraryFunctionSignatures(string name)
        {
            return GetLibraryFunctionSignatures(name, ActiveSubsets.Subsets);
        }


        private IReadOnlyGenericArray<LSLLibraryFunctionSignature> GetLibraryFunctionSignatures(string name, IEnumerable<string> subsets)
        {
            var results = new GenericArray<LSLLibraryFunctionSignature>();

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
            return results.Count == 0 ? null : results.Distinct(new LambdaEqualityComparer<LSLLibraryFunctionSignature>(ReferenceEquals)).ToGenericArray();
        }


        /// <summary>
        /// Attempt to return a library function signature from the active subsets with an exact signature match to the give signature.
        /// </summary>
        /// <param name="signatureToTest">The function signature that should match exactly with the library function signature you want to return from the provider.</param>
        /// <returns>A signature defined in the provider among the active subsets that matches exactly with the given signature, or null if none is found.</returns>
        /// <exception cref="LSLDuplicateSignatureException">
        /// Thrown if <see cref="ActiveSubsets"/> is enabled and more than one active subset contains a duplicate definition of the function or one of its overloads,
        /// and the function/overload is not shared across those subsets.
        /// </exception>
        public LSLLibraryFunctionSignature GetLibraryFunctionSignature(LSLFunctionSignature signatureToTest)
        {
            var sigs = GetLibraryFunctionSignatures(signatureToTest.Name);

            if (sigs == null) return null;

            return sigs.FirstOrDefault(x=>x.SignatureEquivalent(signatureToTest));
        }


        /// <summary>
        /// Check whether a library constant with the given name is defined among the active subsets of this library data provider.
        /// </summary>
        /// <param name="name">The name of the constant to check the existence of.</param>
        /// <returns>True if the constant is defined among the current active subsets, or false if it is not.</returns>
        public bool LibraryConstantExist(string name)
        {
            var match =
                ActiveSubsets.Subsets.Where(x => _constantSignaturesBySubsetAndName.ContainsKey(x))
                .FirstOrDefault(x => _constantSignaturesBySubsetAndName[x].ContainsKey(name));

            return match != null;
        }



        /// <summary>
        /// Attempt to return a library constant signature from the active subsets with the same name as the given name.
        /// </summary>
        /// <param name="name">The name of the library constant that you want to find among the active subsets.</param>
        /// <returns>A library constant signature with a matching name, or null if none is found.</returns>
        /// <exception cref="LSLDuplicateSignatureException">
        /// Thrown if <see cref="ActiveSubsets"/> is enabled and more than one active subset contains a constant with a duplicate name, 
        /// and the constant is not shared across those subsets.
        /// </exception>
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
