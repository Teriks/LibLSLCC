#region FileInfo

// 
// File: LSLFunctionSignature.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 

#endregion

#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Represents a basic LSL function signature.
    /// </summary>
    public class LSLFunctionSignature
    {
        private readonly GenericArray<LSLParameter> _parameters;
        private string _name;


        /// <summary>
        ///     Construct an empty <see cref="LSLFunctionSignature" />.  Only derived classes can do this.
        /// </summary>
        protected LSLFunctionSignature()
        {
            ReturnType = LSLType.Void;
            _parameters = new GenericArray<LSLParameter>();
            VariadicParameterIndex = -1;
        }


        /// <summary>
        ///     Construct an <see cref="LSLFunctionSignature" /> by cloning another <see cref="LSLFunctionSignature" /> object.
        /// </summary>
        /// <param name="other">The <see cref="LSLFunctionSignature" /> object to copy construct from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLFunctionSignature(LSLFunctionSignature other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            Name = other.Name;
            _parameters = new GenericArray<LSLParameter>(other._parameters);
            ReturnType = other.ReturnType;
            HasVariadicParameter = other.HasVariadicParameter;
            VariadicParameterIndex = other.VariadicParameterIndex;
        }


        /// <summary>
        ///     Construct a function signature by providing an associated <see cref="LSLType" /> for the return type, a function
        ///     Name and an optional enumerable of <see cref="LSLParameter" /> objects.
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <exception cref="ArgumentException">Thrown if more than one variadic parameter is added to the function signature.</exception>
        public LSLFunctionSignature(LSLType returnType, string name, IEnumerable<LSLParameter> parameters = null)
        {
            ReturnType = returnType;
            Name = name;
            VariadicParameterIndex = -1;

            if (parameters == null)
            {
                _parameters = new GenericArray<LSLParameter>();
            }
            else
            {
                _parameters = new GenericArray<LSLParameter>();
                foreach (var lslParameter in parameters)
                {
                    AddParameter(lslParameter);
                }
            }
        }


        /// <summary>
        ///     Returns the number of parameters the function signature has including variadic parameters
        /// </summary>
        public int ParameterCount
        {
            get { return _parameters.Count; }
        }

        /// <summary>
        ///     Returns the number of non variadic parameters the function signature has
        /// </summary>
        public int ConcreteParameterCount
        {
            get { return _parameters.Count - (HasVariadicParameter ? 1 : 0); }
        }

        /// <summary>
        ///     The functions LSL return type
        /// </summary>
        public LSLType ReturnType { get; set; }

        /// <summary>
        ///     The functions name, must follow LSL symbol naming conventions
        /// </summary>
        /// <exception cref="LSLInvalidSymbolNameException" accessor="set">
        ///     Thrown if the function does not follow LSL symbol naming
        ///     conventions for functions.
        /// </exception>
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new LSLInvalidSymbolNameException(GetType().FullName +
                                                            ": Function name was null or whitespace.");
                }

                if (!LSLTokenTools.IDRegexAnchored.IsMatch(value))
                {
                    throw new LSLInvalidSymbolNameException(
                        string.Format(
                            GetType().FullName + ": Function name '{0}' contained invalid characters or formatting.",
                            value));
                }
                _name = value;
            }
        }

        /// <summary>
        ///     Indexable list of objects describing the functions parameters
        /// </summary>
        public IReadOnlyGenericArray<LSLParameter> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        ///     An enumerable of all non-variadic parameters in the function signature.
        /// </summary>
        public IEnumerable<LSLParameter> ConcreteParameters
        {
            get { return Parameters.Take(ConcreteParameterCount); }
        }

        /// <summary>
        ///     Returns a formated signature string for the function signature without a trailing semi-colon.
        ///     Such as:  float llAbs(float value)
        ///     Or: modInvokeN(string fname, params any[] parms)
        ///     The later being a function from OpenSim's modInvoke API to demonstrate variadic parameter formating.
        ///     If a parameter is variadic and has a type that is not void, the 'any' keyword will be replaced with the
        ///     corresponding name for the type.
        /// </summary>
        public string SignatureString
        {
            get
            {
                var returnString = "";
                if (ReturnType != LSLType.Void)
                {
                    returnString = ReturnType.ToLSLTypeName() + " ";
                }

                var paramNames = Parameters.Select(x => x.SignatureString);

                return returnString + Name + "(" + string.Join(", ", paramNames) + ")";
            }
        }

        /// <summary>
        ///     Whether or not a variadic parameter has been added to this function signature.
        ///     There can only be one variadic parameter.
        /// </summary>
        public bool HasVariadicParameter { get; private set; }

        /// <summary>
        ///     The index of the variadic parameter in the Parameters list, or -1 if none exists.
        /// </summary>
        public int VariadicParameterIndex { get; private set; }


        /// <summary>
        ///     Delegates to SignatureString
        /// </summary>
        /// <returns>
        ///     SignatureString
        /// </returns>
        public override string ToString()
        {
            return SignatureString;
        }


        /// <summary>
        ///     Attempt to parse a function signature from a formated string.
        ///     Such as: float llAbs(float value) or llOwnerSay(string message);
        /// </summary>
        /// <param name="str">The string containing the formated function signature.</param>
        /// <returns>The LSLLibraryFunctionSignature that was parsed from the string, or null.</returns>
        /// <exception cref="ArgumentException">If there was a syntax error while parsing the function signature.</exception>
        public static LSLFunctionSignature Parse(string str)
        {
            var regex = new LSLFunctionSignatureRegex("", ";*");
            var m = regex.GetSignature(str);
            if (m == null)
            {
                throw new ArgumentException("Syntax error parsing function signature", "str");
            }
            return m;
        }


        /// <summary>
        ///     Add a new parameter to the function signature object
        /// </summary>
        /// <param name="parameter">The <see cref="LSLParameter" /> object to add to the signature.</param>
        /// <exception cref="ArgumentException">Thrown if more than one variadic parameter is added to the function signature.</exception>
        public void AddParameter(LSLParameter parameter)
        {
            if (parameter.Variadic)
            {
                if (!HasVariadicParameter)
                {
                    HasVariadicParameter = true;
                    VariadicParameterIndex = _parameters.Count;
                }
                else
                {
                    throw new ArgumentException(
                        GetType().FullName + ": Signature already has a variadic parameter, cannot add another",
                        "parameter");
                }
            }


            parameter.ParameterIndex = _parameters.Count;

            _parameters.Add(parameter);
        }


        /// <summary>
        ///     Determines if two function signatures match exactly (including return type), parameter names do not matter but
        ///     parameter types do.
        /// </summary>
        /// <param name="otherSignature">The other function signature to compare to</param>
        /// <returns>True if the two signatures are identical</returns>
        public bool SignatureEquivalent(LSLFunctionSignature otherSignature)
        {
            return LSLFunctionSignatureMatcher.SignaturesEquivalent(this, otherSignature);
        }


        /// <summary>
        ///     Determines if a given <see cref="LSLFunctionSignature" /> is a duplicate definition of this function signature.
        ///     The logic behind this is a bit different than SignatureMatches().
        ///     If the given function signature has the same name, a differing return type and both functions have no parameters;
        ///     than this function will return true
        ///     and <see cref="SignatureEquivalent(LSLFunctionSignature)" /> will not.
        ///     If the other signature is an overload that is ambiguous in all cases due to variadic parameters, this function
        ///     returns true.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="otherSignature">The other function signature to compare to</param>
        /// <returns>
        ///     True if the two signatures are duplicate definitions of each other, taking static overloading ambiguities into
        ///     account.
        /// </returns>
        public bool DefinitionIsDuplicate(LSLFunctionSignature otherSignature)
        {
            return LSLFunctionSignatureMatcher.DefinitionIsDuplicate(this, otherSignature);
        }


        /// <summary>
        ///     This implementation of GetHashCode() uses the name of the <see cref="LSLFunctionSignature" />, the
        ///     <see cref="ReturnType" /> and the <see cref="LSLParameter.Type" />/<see cref="LSLParameter.Variadic" /> status of
        ///     the parameters, this means the Hash Code is linked the Function name, return Type and the Types/Variadic'ness of
        ///     all its parameters.
        ///     Inherently, uniqueness is also determined by the number of parameters.
        /// </summary>
        /// <returns>Hash code for this <see cref="LSLFunctionSignature" /></returns>
        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash*31 + ReturnType.GetHashCode();
            hash = hash*31 + Name.GetHashCode();

            return Parameters.Aggregate(hash, (current, lslParameter) =>
            {
                var c = current*31 + lslParameter.Type.GetHashCode();
                c = c*31 + lslParameter.Variadic.GetHashCode();
                return c;
            });
        }


        /// <summary>
        ///     Equals(object obj) delegates to <see cref="SignatureEquivalent(LSLFunctionSignature)" />
        /// </summary>
        /// <param name="obj">The other function signature</param>
        /// <returns>Equality</returns>
        public override bool Equals(object obj)
        {
            var o = obj as LSLFunctionSignature;
            if (o == null)
            {
                return false;
            }

            return SignatureEquivalent(o);
        }
    }
}