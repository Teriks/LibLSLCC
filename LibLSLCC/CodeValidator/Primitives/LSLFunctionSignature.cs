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
using LibLSLCC.CodeValidator.Enums;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLFunctionSignature
    {
        private readonly List<LSLParameter> _parameters;

        protected LSLFunctionSignature()
        {
            _parameters = new List<LSLParameter>();
            Name = "";
            ReturnType = LSLType.Void;
        }

        public LSLFunctionSignature(LSLFunctionSignature other)
        {
            Name = other.Name;
            _parameters = other._parameters.ToList();
            ReturnType = other.ReturnType;
            HasVariadicParameter = other.HasVariadicParameter;
            VariadicParameterIndex = other.VariadicParameterIndex;
        }

        public LSLFunctionSignature(LSLType returnType, string name, IEnumerable<LSLParameter> parameters = null)
        {
            ReturnType = returnType;
            Name = name;

            if (parameters == null)
            {
                _parameters = new List<LSLParameter>();
            }
            else
            {
                _parameters = new List<LSLParameter>();
                foreach (var lslParameter in parameters)
                {
                    AddParameter(lslParameter);
                }
            }
        }

        /// <summary>
        ///     Returns the number of parameters the function signature has including variadic parameters
        /// </summary>
        public int ParameterCount {
            get
            {
                return _parameters.Count;
            }
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
        ///     The functions name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Indexable list of objects describing the functions parameters
        /// </summary>
        public IReadOnlyList<LSLParameter> Parameters
        {
            get { return _parameters; }
        }

        public IEnumerable<LSLParameter> ConcreteParameters
        {
            get
            {
                return Parameters.Take(ConcreteParameterCount);
            }
        } 

        public string SignatureString
        {
            get
            {
                var returnString = "";
                if (ReturnType != LSLType.Void)
                {
                    returnString = ReturnType.ToLSLTypeString() + " ";
                }

                var paramNames = Parameters.Select(x => x.SignatureString);

                return returnString + Name + "(" + string.Join(", ", paramNames) + ")";
            }
        }

        public bool HasVariadicParameter { get; private set; }
        public int VariadicParameterIndex { get; private set; }

        public override string ToString()
        {
            return SignatureString;
        }

        public static LSLFunctionSignature Parse(string cSignature)
        {
            var regex = new LSLFunctionSignatureRegex("", ";*");
            var m = regex.GetSignature(cSignature);
            if (m == null)
            {
                throw new ArgumentException("Syntax error parsing function signature", "cSignature");
            }
            return m;
        }

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
                        "Signature already has a variadic parameter, cannot add another",
                        "parameter");
                }
            }
            

            parameter.ParameterIndex = _parameters.Count;

            _parameters.Add(parameter);
        }

        /// <summary>
        ///     Determines if two function signatures match exactly (including return type), parameter names do not matter but parameter types do.
        /// </summary>
        /// <param name="otherSignature">The other function signature to compare to</param>
        /// <returns>True if the two signatures are identical</returns>
        public bool SignatureEquivalent(LSLFunctionSignature otherSignature)
        {

            return LSLFunctionSignatureMatcher.SignaturesEquivalent(this, otherSignature);
        }


        /// <summary>
        ///     Determines if a given LSLFunctionSignature is a duplicate definition of this function signature.
        ///     The logic behind this is a bit different than SignatureMatches().
        ///     
        ///     If the given function signature has the same name, a differing return type and both functions have no parameters; than this function will return true
        ///     and SignatureMatches() will not. 
        /// 
        ///     If the other signature is an overload that is ambiguous in all cases due to variadic parameters, this function returns true.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="otherSignature">The other function signature to compare to</param>
        /// <returns>True if the two signatures are duplicate definitions of each other, taking static overloading ambiguities into account.</returns>
        public bool DefinitionIsDuplicate(LSLFunctionSignature otherSignature)
        {
            return LSLFunctionSignatureMatcher.DefinitionIsDuplicate(this, otherSignature);
        }



        /// <summary>
        /// This implementation of GetHashCode() uses the name of the LSLFunctionSignature, the ReturnType and the LSL Type/Variadic status of
        /// the parameters, this means the Hash Code is linked the Function name, return Type and the Types/Variadic'ness of all its parameters.
        /// 
        /// Inherently, uniqueness is also determined by the number of parameters.
        /// </summary>
        /// <returns>Hash code for this LSLFunctionSignature</returns>
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
        /// Equals(object obj) delegates to SignatureMatches(LSLFunctionSignature other)
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