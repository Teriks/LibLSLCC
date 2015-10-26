using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

namespace LibLSLCC.CodeValidator.Primitives
{

    /// <summary>
    /// A tool for matching comparing <see cref="LSLFunctionSignature"/> objects and preforming overload resolution across multiple <see cref="LSLFunctionSignature"/> objects.
    /// </summary>
    public static class LSLFunctionSignatureMatcher
    {
        /// <summary>
        /// Represents the status of an attempted function signature match against parameter expressions
        /// </summary>
        public class  Match
        { 
            /// <summary>
            /// True if not enough parameter expressions were supplied to the function signature.
            /// </summary>
            public bool NotEnoughParameters { get; private set; }


            /// <summary>
            /// True if to many parameter expressions were supplied to the function signature.
            /// </summary>
            public bool ToManyParameters { get; private set; }

            /// <summary>
            /// The index where there was a parameter mismatch if TypeMismatch is true, otherwise -1.
            /// </summary>
            public int TypeMismatchIndex { get; private set; }


            /// <summary>
            /// True if a type mismatch between a passed expression and a defined parameter type occurred.
            /// </summary>
            public bool TypeMismatch { get; private set; }


            /// <summary>
            /// True if the function can successfully be compiled with the given parameter expressions.
            /// Equivalent to !NotEnoughParameters &amp;&amp; !ToManyParameters &amp;&amp; !TypeMismatch
            /// </summary>
            public bool Success
            {
                get
                {
                    return !NotEnoughParameters && !ToManyParameters && !TypeMismatch;
                }
            }

            /// <summary>
            /// True if either ToManyParameters or NotEnoughParameters are true
            /// </summary>
            public bool ImproperParameterCount
            {
                get { return NotEnoughParameters || ToManyParameters; }
            }

            internal Match(bool notEnoughParameters, bool toManyParameters, bool typeMismatch, int typeMismatchIndex)
            {
                NotEnoughParameters = notEnoughParameters;
                ToManyParameters = toManyParameters;
                TypeMismatch = typeMismatch;
                TypeMismatchIndex = typeMismatchIndex;
            }
        }

        /// <summary>
        /// Represents the status of an attempted overload resolution match against parameter expressions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class OverloadMatch<T> where T : LSLFunctionSignature
        {
             /// <summary>
             /// This is set to true if multiple overload matches were found given the expressions passed into the function
             /// </summary>
             public bool Ambiguous {
                 get
                 {
                     return Matches.Count > 1;
                 }
            }


            /// <summary>
            /// This is a list of all possible overload matches.  This will have more
            /// than one element when Ambiguous is set to true
            /// </summary>
            public IReadOnlyGenericArray<T> Matches { get; private set; }

            /// <summary>
            /// If multiple matches were found (Ambiguous is set to true) then this will be null
            /// This will also be null if no matches were found at all
            /// </summary>
            public T MatchingOverload {
                get
                {
                    return Ambiguous ? null : Matches.FirstOrDefault();
                }
            }



            /// <summary>
            /// This returns true if there are no ambiguous matches, and a single overload match was found
            /// it is equivalent to: (!Ambiguous &amp;&amp; Matches.Count != 0)
            /// </summary>
            public bool Success
            {
                get
                {
                    return !Ambiguous && Matches.Count != 0;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="OverloadMatch{T}"/> class with an <see cref="IReadOnlyGenericArray{T}"/> containing signature matches.
            /// </summary>
            /// <param name="matches">The matches.</param>
            internal OverloadMatch(IReadOnlyGenericArray<T> matches)
            {
                Matches = matches;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="OverloadMatch{T}"/> class with no signature matches at all.
            /// </summary>
            internal OverloadMatch()
            {
                Matches = new GenericArray<T>();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="OverloadMatch{T}"/> class with a single un-ambiguous signature match.
            /// </summary>
            /// <param name="match">The match.</param>
            internal OverloadMatch(T match)
            {
                Matches = new GenericArray<T> { match };
            }
        }



        /// <summary>
        /// Find a matching overload from a list of function signatures, given the parameter expressions. return null if none is found.
        /// </summary>
        /// <param name="expressionValidator">The expression validator, which is used to determine if an expression can be passed into a parameter of a certain type.</param>
        /// <param name="functionSignatures">The function signatures to search through.</param>
        /// <param name="expressionNodes">The expression nodes of the function parameters we want to pass and find an overload for.</param>
        /// <returns>A matching <see cref="LSLFunctionSignature"/> overload or null.</returns>
        public static OverloadMatch<T> MatchOverloads<T>(IReadOnlyGenericArray<T> functionSignatures, IReadOnlyGenericArray<ILSLExprNode> expressionNodes, ILSLExpressionValidator expressionValidator) where T : LSLFunctionSignature
        {
            return MatchOverloads(functionSignatures, expressionNodes,(expressionValidator.ValidFunctionParameter));
        }

        /// <summary>
        /// Find a matching overload from a list of function signatures, given the parameter expressions. return null if none is found.
        /// </summary>
        /// <param name="typeComparer">
        /// A function used to compare an <see cref="LSLParameter"/> to another <see cref="ILSLExprNode"/> to check for a match.
        /// Should return true if the <see cref="ILSLExprNode"/> can be passed into the <see cref="LSLParameter"/>.
        /// </param>
        /// <param name="functionSignatures">The function signatures to search through.</param>
        /// <param name="expressionNodes">The expression nodes of the function parameters we want to pass and find an overload for.</param>
        /// <returns>A matching <see cref="LSLFunctionSignature"/> overload or null.</returns>
        public static OverloadMatch<T> MatchOverloads<T>(IReadOnlyGenericArray<T> functionSignatures, IReadOnlyGenericArray<ILSLExprNode> expressionNodes, Func<LSLParameter, ILSLExprNode, bool> typeComparer) where T : LSLFunctionSignature
        {
            //discover candidates 'applicable' functions, using a typeComparer function/lambda to compare the signature parameters to the passed expression nodes.
            //anything that could possibly match the signature as an individual non-overloaded function is considered an overload match candidate. (see the TryMatch function of this class)
            // 
            //The type comparer is allowed to match signatures with implicit parameter conversions if it wants to.
            //such as for LSL's (key to string) and (string to key) conversion.
            var matches =
                functionSignatures.Where(
                    functionSignature => TryMatch(functionSignature, expressionNodes, typeComparer).Success).ToGenericArray();

            


            //More than one matching signature, we need to tie break.
            if (matches.Count <= 1) return new OverloadMatch<T>(matches);


            //Prefer function declarations that have no parameters, over function declarations with one variadic parameter.
            if (expressionNodes.Count == 0)
            {
                return new OverloadMatch<T>(matches.First(x=>x.ParameterCount == 0));
            }


            //Rank and group the matches by the number implicit type conversions that occur.
            //Implicit conversion is the only real match quality degradation that can occur in LSL.
            var rankingToSignatureGroup = new HashMap<int, GenericArray<T>>();

        
            foreach (var match in matches)
            {
                //the higher the match ranking, the worse it is.
                int matchRank = 0;

                int idx = 0;

                foreach (var parameter in expressionNodes)
                {
                    //Select the signature parameter to compare, if the expression index 'idx' is greater than a matches parameter count;
                    //Then that match is a variadic function, with the parameters overflowing the parameter count because multiple parameters were passed into the variadic parameter slot.
                    //If this happens, we want to continue comparing the rest of the passed variadic parameters to the type of the last parameter in the match signature, IE. The one that is variadic. 
                    var signatureParameterToCompare = idx > (match.ParameterCount-1) ? match.Parameters.Last() : match.Parameters[idx];

                    //If a type of the passed expression does not exactly equal the signature parameter, but the type comparer says that the expression can actually be passed in anyway.
                    //Then the type that the expression is must be implicitly convertible to the type that the parameter is, an implicit conversion has occurred.  The match quality of the signature has degraded.
                    if (signatureParameterToCompare.Type != parameter.Type && typeComparer(signatureParameterToCompare, parameter))
                    {
                        matchRank++;
                    }

                    //increment the current expression index
                    idx++;
                }

                //group by rank, using the HashMap object.
                GenericArray<T> signaturesWithTheSameRank = null;

                //get a reference to a group with the same rank, if one exists
                if (rankingToSignatureGroup.TryGetValue(matchRank, out signaturesWithTheSameRank))
                { 
                    signaturesWithTheSameRank.Add(match);
                }
                else
                {
                    //first group seen with this rank, make a new group
                    signaturesWithTheSameRank = new GenericArray<T> {match};
                    rankingToSignatureGroup.Add(matchRank, signaturesWithTheSameRank);
                }
            }

            //check if all the matching signatures have the same ranking, which would mean there is not a 'best' choice.
            //we grouped by rank, so if there is just one group, then all the signatures have the same rank.
            if (rankingToSignatureGroup.Count == 1)
            {
                //all candidates share the same rank, ambiguous match because no signature can be the 'best' choice, return all matches.
                return new OverloadMatch<T>(matches);
            }

            //Find the grouping with the smallest ranking number, this is the best group to look in.
            KeyValuePair<int, GenericArray<T>> ?groupingWithTheBestRank = null;

            foreach (var groupPair in rankingToSignatureGroup)
            {
                //find the lowest rank
                if (!groupingWithTheBestRank.HasValue || groupPair.Key < groupingWithTheBestRank.Value.Key)
                {
                    //assign this group if it has lower rank than the previous group.
                    groupingWithTheBestRank = groupPair;
                }
            }
            


            //no groupings were created, no matches at all.  This is not expected to happen.
            if (!groupingWithTheBestRank.HasValue)
            {
                return new OverloadMatch<T>(new GenericArray<T>());
            }

            var selectedGroup = groupingWithTheBestRank.Value.Value;

            //if we found a grouping, and that grouping has more than one matching signature, we need to tie break again.
            if (selectedGroup.Count != 1)
            {
                if (
                    selectedGroup.Distinct(
                        new LambdaEqualityComparer<T>(
                            (sig, sig2) => sig.ParameterCount == sig2.ParameterCount,
                            sig => sig.ParameterCount.GetHashCode())).Count() == 1)
                {
                    //all the signatures in the grouping have a matching number of parameters, overload resolution is ambiguous, return all signatures that matched.
                    return new OverloadMatch<T>(selectedGroup);
                }

                
                //Otherwise find the signature in the grouping with a concrete (non-variadic) parameter count closest to the amount of parameter expressions given to call the function.

                var minDistance = selectedGroup.Min(n => Math.Abs(expressionNodes.Count - n.ConcreteParameterCount));
                var closest = selectedGroup.First(n => Math.Abs(expressionNodes.Count - n.ConcreteParameterCount) == minDistance);

                //The one with the closest amount of concrete parameters wins.
                return new OverloadMatch<T>(closest);
            }

            if (selectedGroup.Count > 1)
            {
                throw new InvalidOperationException("LSLFunctionSignatureMatcher.MatchOverloads: Algorithm bug check assertion.");
            }

            //There was only one signature match in the grouping, it had the lowest rank so its the best.
            return new OverloadMatch<T>(selectedGroup.First());
        }



        /// <summary>
        /// Check if a given function signature can be called with the given expressions as parameters
        /// </summary>
        /// <param name="expressionValidator">The expression validator, which is used to determine if an expression can be passed into a parameter of a certain type.</param>
        /// <param name="functionSignature">The function signature of the functions your passing the parameter expressions to.</param>
        /// <param name="expressions">The expressions we want to test against this function signatures defined parameters.</param>
        /// <returns>A LSLFunctionCallSignatureMatcher.MatchStatus object containing information about how the parameters matched or did not match the call signature.</returns>
        public static Match TryMatch( LSLFunctionSignature functionSignature, IReadOnlyGenericArray<ILSLExprNode> expressions, ILSLExpressionValidator expressionValidator)
        {

            return TryMatch(functionSignature,expressions,(expressionValidator.ValidFunctionParameter));
        }



        /// <summary>
        /// Determines if two function signatures match exactly (including return type), parameter names do not matter but parameter types and variadic parameter status do.
        /// </summary>
        /// <param name="left">The first function signature in the comparison.</param>
        /// <param name="right">The other function signature in the comparison.</param>
        /// <returns>True if the two signatures are identical</returns>
        public static bool SignaturesEquivalent(LSLFunctionSignature left, LSLFunctionSignature right)
        {
            if (left.ReturnType != right.ReturnType)
            {
                return false;
            }
            if (left.Name != right.Name)
            {
                return false;
            }
            if (left.ParameterCount != right.ParameterCount)
            {
                return false;
            }
            for (var i = 0; i < left.ParameterCount; i++)
            {
                var l = left.Parameters[i];
                var r = right.Parameters[i];
                if (l.Type != r.Type || l.Variadic != r.Variadic)
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        ///     Determines if a two LSLFunctionSignatures are duplicate definitions of each other.
        /// <para>
        ///     The logic behind this is a bit different than <see cref="SignaturesEquivalent"/>.
        ///     
        ///     If the given function signatures have the same name, differing return types and no parameters; than this function will return true
        ///     and <see cref="SignaturesEquivalent"/> will not. 
        /// 
        ///     If the given function signatures have differing return types, and the exact same parameter types/count; than this function will return true
        ///     and <see cref="SignaturesEquivalent"/> will not.
        /// </para>
        /// </summary>
        /// <param name="left">The first function signature in the comparison.</param>
        /// <param name="right">The other function signature in the comparison.</param>
        /// <returns>True if the two signatures are duplicate definitions of each other, taking static overloading ambiguities into account.</returns>
        public static bool DefinitionIsDuplicate(LSLFunctionSignature left, LSLFunctionSignature right)
        {
            //Cannot be duplicates of each other if the name is different
            if (left.Name != right.Name) return false;

            //Both functions have no parameters and the same name, they are duplicate definitions of each other.
            if (left.ParameterCount == right.ParameterCount && left.ParameterCount == 0)
            {
                return true;
            }

            //we don't care about the return type, it does not make a function definition unique, it does not participate in overload resolution.

            //simple case, these functions cannot be a duplicate definition if they have a different parameter count.
            if (left.ParameterCount != right.ParameterCount)
            {
                return false;
            }
            for (var i = 0; i < left.ParameterCount; i++)
            {
                //check all the parameters have identical specifications.
                //IE, type and variadic status.
                var l = left.Parameters[i];
                var r = right.Parameters[i];
                if (l.Type != r.Type || l.Variadic != r.Variadic)
                {
                    //nope, there was a mismatch.
                    return false;
                }
            }

            //everything matched up, the return type was ignored, it does not matter.
            return true;
        }




        /// <summary>
        /// Check if a given function signature can be called with the given expressions as parameters
        /// </summary>
        /// <param name="typeComparer">A function which should return true if two types match</param>
        /// <param name="functionSignature">The function signature of the functions your passing the parameter expressions to.</param>
        /// <param name="expressions">The expressions we want to test against this function signatures defined parameters.</param>
        /// <returns>A LSLFunctionCallSignatureMatcher.MatchStatus object containing information about how the parameters matched or did not match the call signature.</returns>
        public static Match TryMatch(LSLFunctionSignature functionSignature, IReadOnlyGenericArray<ILSLExprNode> expressions, Func<LSLParameter,ILSLExprNode,bool> typeComparer)
        {

            int parameterNumber = 0;
            bool parameterTypeMismatch = false;

            //if we supplied to many parameters, and there is not a variadic 
            //parameter at the end of the signature, then there is not a match
            if (functionSignature.HasVariadicParameter == false &&
                expressions.Count > functionSignature.ParameterCount)
            {
                return new Match(false, true, false, -1);
            }

 
            //not enough parameters to fill all of the concrete (non variadic) parameters in
            //we do not have enough expressions to call this function signature
            if (expressions.Count < functionSignature.ConcreteParameterCount)
            {
                return new Match(true, false, false, -1);
            }


            //check the types of all parameters match, including variadic parameters from the signature if they are not Void
            //if the variadic parameter is Void than anything can go in it, so it is not even checked
            for (; parameterNumber < expressions.Count; parameterNumber++)
            {
                LSLParameter compareWithThisSignatureParameter;

                if (parameterNumber > (functionSignature.ParameterCount-1))
                {
                    //If this happens it means we are in a continuation of a variadic parameter, we want to continue comparing the rest of the passed
                    //expressions to the last parameter in the function signature, IE. the variadic parameter.
                    compareWithThisSignatureParameter = functionSignature.Parameters[functionSignature.ParameterCount - 1];
                }
                else
                {
                    //We have not flowed over the parameter count of the signature, so theres no danger
                    //of an out of bounds index on the parameters array in the signature.
                    compareWithThisSignatureParameter = functionSignature.Parameters[parameterNumber];
                }
                

                //no type check required, the variadic parameter allows everything in because its type is Void, anything you put in is a match
                //from here on out.  So its safe to return from the loop now and stop checking parameters.
                if (compareWithThisSignatureParameter.Variadic && compareWithThisSignatureParameter.Type == LSLType.Void)
                    break;

                //use the type comparer to check for a match, there might be some special case like string literals
                //being passed into a Key parameter, this behavior is delegated for better re-usability
                if (typeComparer(compareWithThisSignatureParameter, expressions[parameterNumber]))
                    continue;

                //the expression could not be passed into the parameter due to a type mismatch, we are done checking the parameters
                //because we know for a fact the expressions given do not match this call signature
                parameterTypeMismatch = true;
                break;
            }


            //if there was a parameter mismatch, the last checked parameter index is the index at which the mismatch occurred
            var badParameterIndex = parameterTypeMismatch ? parameterNumber : -1;

            //we had an allowable amount of parameters, but there was a type mismatch somewhere
            return new Match(false, false, parameterTypeMismatch, badParameterIndex);
        }
    }
}