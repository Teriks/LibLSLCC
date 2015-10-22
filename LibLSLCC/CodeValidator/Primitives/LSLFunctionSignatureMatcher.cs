using System;
using System.Linq;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator.Primitives
{

    /// <summary>
    /// A tool for matching comparing LSLFunctionSignature objects and preforming overload resolution across multiple LSLFunctionSignatures.
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
            /// If multiple matches were found (Ambigious is set to true) then this will be null
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


            internal OverloadMatch(IReadOnlyGenericArray<T> matches)
            {
                Matches = matches;
            }
        }



        /// <summary>
        /// Find a matching overload from a list of function signatures, given the parameter expressions. return null if none is found.
        /// </summary>
        /// <param name="expressionValidator">The expression validator, which is used to determine if an expression can be passed into a parameter of a certain type.</param>
        /// <param name="functionSignatures">The function signatures to search through.</param>
        /// <param name="expressionNodes">The expression nodes of the function parameters we want to pass and find an overload for.</param>
        /// <returns>A matching LSLFunctionSignature overload or null.</returns>
        public static OverloadMatch<T> MatchOverloads<T>(IReadOnlyGenericArray<T> functionSignatures, IReadOnlyGenericArray<ILSLExprNode> expressionNodes, ILSLExpressionValidator expressionValidator) where T : LSLFunctionSignature
        {
            //if there are multiple matches, then the overload is ambiguous
            var matches =
                functionSignatures.Where(
                    functionSignature => TryMatch(functionSignature, expressionNodes, expressionValidator).Success).ToGenericArray();


            return new OverloadMatch<T>(matches);
        }

        /// <summary>
        /// Find a matching overload from a list of function signatures, given the parameter expressions. return null if none is found.
        /// </summary>
        /// <param name="typeComparer">
        /// A function used to compare an LSLParameter to an ILSLExprNode to check for a match.
        /// Should return true if the ILSLExprNode can be passed into the LSLParameter.
        /// </param>
        /// <param name="functionSignatures">The function signatures to search through.</param>
        /// <param name="expressionNodes">The expression nodes of the function parameters we want to pass and find an overload for.</param>
        /// <returns>A matching LSLFunctionSignature overload or null.</returns>
        public static OverloadMatch<T> MatchOverloads<T>(IReadOnlyGenericArray<T> functionSignatures, IReadOnlyGenericArray<ILSLExprNode> expressionNodes, Func<LSLParameter, ILSLExprNode, bool> typeComparer) where T : LSLFunctionSignature
        {
            var matches =
                functionSignatures.Where(
                    functionSignature => TryMatch(functionSignature, expressionNodes, typeComparer).Success).ToGenericArray();


            return new OverloadMatch<T>(matches);
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

            return TryMatch(functionSignature,expressions,((parameter, node) => expressionValidator.ValidFunctionParameter(functionSignature, parameter.ParameterIndex, node)));
        }

        /// <summary>
        /// Determines if two function signatures match exactly (including return type), parameter names do not matter but parameter types do.
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
                if (left.Parameters[i].Type != right.Parameters[i].Type)
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        ///     Determines if a two LSLFunctionSignatures are duplicate definitions of each other
        ///     The logic behind this is a bit different than SignaturesEquivalent().
        ///     
        ///     If the given function signature has the same name, a differing return type and both functions have no parameters; than this function will return true
        ///     and SignaturesEquivalent() will not. 
        /// 
        ///     If the other signature is an overload that is ambiguous in all cases due to variadic parameters, this function returns true.
        /// </summary>
        /// <param name="left">The first function signature in the comparison.</param>
        /// <param name="right">The other function signature in the comparison.</param>
        /// <returns>True if the two signatures are duplicate definitions of each other, taking static overloading ambiguities into account.</returns>
        public static bool DefinitionIsDuplicate(LSLFunctionSignature left, LSLFunctionSignature right)
        {
            //Cannot be duplicates of each other if the name is different
            if (left.Name != right.Name) return false;


            //Cases for when both signatures have an equal number of parameters
            //ParameterCount includes variadic parameters, variadic parameters can only
            //be the last parameter in a signature.  There can only be one variadic parameter
            //in a function signature, LSLFunctionSignature ensures this.
            if (left.ParameterCount == right.ParameterCount)
            {

                //Notes:
                //1: Linq's All function returns TRUE for empty collections
                //
                //2: The type of the variadic parameters is important in determining ambiguity, ie:
                //   func(integer a, params string[] b) and func(integer a, params integer[] b) are not duplicates
                //   OpenSim does not have any variadic library functions that take a non object[] (non generic) parameter type
                //   but I believe the best behavior is to implement a type check on the variadic parameter so they option is available
                //   to have overloads with the same basic signature and different types for the variadic parameter
                //
                //3: It is a fact that LSLFunctionSignature is only allowed to have one variadic parameter, it is enforced with an exception in the class
                //
                //4: if both functions have a single variadic parameter, an ambiguous match can occur at compile time, but this is not a duplicate
                //   definition because the overload can be resolved using the types of the parameters



                if (left.HasVariadicParameter && right.HasVariadicParameter)
                {
                    var rightVariadicType = right.Parameters.Last().Type;
                    var leftVariadicType = left.Parameters.Last().Type;

                    var eitherAreVoidOrTheyAreEqual =
                        (leftVariadicType == LSLType.Void || rightVariadicType == LSLType.Void) ||
                        leftVariadicType == rightVariadicType;

                    //TRUE IF:
                    //All concrete parameters match, and the variadic parameters are equal to each other or either one of them is Void
                    return
                        right.ConcreteParameters
                            .All(x => x.Type == left.Parameters[x.ParameterIndex].Type) && eitherAreVoidOrTheyAreEqual;
                }


                if (left.HasVariadicParameter)
                {
                    var leftVariadicType = left.Parameters.Last().Type;
                    var lastParamRight = right.Parameters.Last().Type;

                    //TRUE IF:
                    //All concrete parameters in the left signature match the corresponding ones in the right
                    //and the left variadic parameter type is either Void or equal to the last parameter in right
                    return
                        left.ConcreteParameters
                            .All(x => x.Type == right.Parameters[x.ParameterIndex].Type) &&
                        (leftVariadicType == LSLType.Void || leftVariadicType == lastParamRight);
                }

                if (right.HasVariadicParameter)
                {
                    var rightVariadicType = right.Parameters.Last().Type;
                    var lastParamLeft = left.Parameters.Last().Type;

                    //TRUE IF:
                    //All concrete parameters in the right signature match the corresponding ones in the left
                    //and the right variadic parameter type is either Void or equal to the last parameter in left
                    return
                        right.ConcreteParameters
                            .All(x => x.Type == left.Parameters[x.ParameterIndex].Type) &&
                        (rightVariadicType == LSLType.Void || rightVariadicType == lastParamLeft);
                }

                //TRUE IF:
                //Neither function has variadic parameters, and the parameter count is equal to zero
                return left.ParameterCount == 0 || left.Parameters.All(x => x.Type == right.Parameters[x.ParameterIndex].Type);
            }



            //Cases for when the function signatures do not have an equal amount of parameters.

            //
            // left: func(integer a, params TYPE[] b)
            // right:  func(integer a, integer b, integer c);
            //
            if (left.HasVariadicParameter && left.ParameterCount < right.ParameterCount)
            {
                


                var leftVariadicParameterType = left.Parameters.Last().Type;
                var rightParameterTypeWhereLeftVariadicIs = right.Parameters[left.VariadicParameterIndex].Type;

                //if we have a Void variadic parameter in the left it will consume/match the rest of the rights parameters
                //no matter what type they are.
                //
                //If the variadic parameter in the left signature is equal to the parameter in the right signature
                //at the corresponding position,  then the variadic parameter in the left signature also consumes/matches
                //the rest of the right signatures parameters
                var leftVariadicParameterConsumesRights = leftVariadicParameterType == LSLType.Void ||
                                                          leftVariadicParameterType ==
                                                          rightParameterTypeWhereLeftVariadicIs;


                //TRUE IF:
                //All concrete parameters match up, and my variadic parameter is either Void (accepts anything) or is the same type as the parameter
                //in their signature at the same index
                return left.ConcreteParameters.All(x => x.Type == right.Parameters[x.ParameterIndex].Type) && leftVariadicParameterConsumesRights;
            }


            //
            // left: func(integer a, integer b, integer c);
            // right:  func(integer a, params TYPE[] b);
            //
            if (right.HasVariadicParameter && left.ParameterCount > right.ParameterCount)
            {
                var rightVariadicParameterType = right.Parameters.Last().Type;
                var leftParameterTypeWhereRightVariadicIs =
                    left.Parameters[right.VariadicParameterIndex].Type;

                //if we have a Void variadic parameter in the right it will consume/match the rest of the lefts parameters
                //no matter what type they are.
                //
                //If the variadic parameter in the right signature is equal to the parameter in the left signature
                //at the corresponding position,  then the variadic parameter in the right signature also consumes/matches
                //the rest of the left signatures parameters
                var rightVariadicParameterConsumesLefts =
                    rightVariadicParameterType == LSLType.Void ||
                    rightVariadicParameterType == leftParameterTypeWhereRightVariadicIs;

                //TRUE IF:
                //All concrete parameters match up, and the rights variadic parameter is either Void (accepts anything) or is the same type as the parameter
                //in the left signature at the same index
                return right.ConcreteParameters.All(x => x.Type == left.Parameters[x.ParameterIndex].Type)
                    && rightVariadicParameterConsumesLefts;
            }

            //this cannot be a duplicate if none of the above conditions were met.
            return false;
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
                var currentSignatureParameter = functionSignature.Parameters[parameterNumber];

                //no type check required, the variadic parameter allows everything in because its type is Void, anything you put in is a match
                if (currentSignatureParameter.Variadic && currentSignatureParameter.Type == LSLType.Void)
                    continue;

                //use the type comparer to check for a match, there might be some special case like string literals
                //being passed into a Key parameter, this behavior is delegated for better re-usability
                if (typeComparer(functionSignature.Parameters[parameterNumber], expressions[parameterNumber]))
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