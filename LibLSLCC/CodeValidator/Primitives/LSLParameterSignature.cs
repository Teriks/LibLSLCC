#region FileInfo

// 
// File: LSLParameter.cs
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
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Represents a basic parameter that belongs to either an event handler or function signature.
    /// </summary>
    /// <remarks>Immutable.</remarks>
    public sealed class LSLParameterSignature
    {

        /// <summary>
        ///     Construct a parameter signature object.
        /// </summary>
        /// <param name="type">The parameter type</param>
        /// <param name="name">The parameter name</param>
        /// <param name="variadic">Is the parameter variadic</param>
        /// <param name="parameterIndex">The parameter index.</param>
        /// <exception cref="LSLInvalidSymbolNameException">
        ///     Thrown if the parameter name does not follow LSL symbol naming
        ///     conventions for parameters.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown if type is <see cref="LSLType.Void" /> and variadic is false.</exception>
        public LSLParameterSignature(LSLType type, string name, bool variadic, int parameterIndex)
        {
            if (type == LSLType.Void && variadic == false)
            {
                throw new ArgumentException(GetType().FullName +
                                            ": Type cannot be LSLType.Void unless the parameter it is variadic.");
            }


            if (string.IsNullOrWhiteSpace(name))
            {
                throw new LSLInvalidSymbolNameException(GetType().FullName + ": Parameter name was null or whitespace.");
            }

            if (!LSLTokenTools.IDRegexAnchored.IsMatch(name))
            {
                throw new LSLInvalidSymbolNameException(
                    string.Format(
                        GetType().FullName + ": Parameter name '{0}' contained invalid characters or formatting.", name));
            }

            Type = type;
            Name = name;
            Variadic = variadic;
            ParameterIndex = parameterIndex;
        }



        /// <summary>
        ///     Construct a prototype parameter signature object without a parameter index.
        /// </summary>
        /// <param name="type">The parameter type</param>
        /// <param name="name">The parameter name</param>
        /// <param name="variadic">Is the parameter variadic</param>
        /// <exception cref="LSLInvalidSymbolNameException">
        ///     Thrown if the parameter name does not follow LSL symbol naming
        ///     conventions for parameters.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown if type is <see cref="LSLType.Void" /> and variadic is false.</exception>
        public LSLParameterSignature(LSLType type, string name, bool variadic) : this(type, name, variadic, -1)
        {
        }


        /// <summary>
        ///     Construct a prototype parameter signature object by cloning another and giving it the specified index.
        /// </summary>
        /// <param name="other">The parameter signature to clone from.</param>
        /// <param name="parameterIndex">The parameter index.</param>
        /// <exception cref="LSLInvalidSymbolNameException">
        ///     Thrown if the parameter name does not follow LSL symbol naming
        ///     conventions for parameters.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown if type is <see cref="LSLType.Void" /> and variadic is false.</exception>
        public LSLParameterSignature(LSLParameterSignature other, int parameterIndex) : 
            this(other.Type, other.Name, other.Variadic, other.ParameterIndex)
        {
        }


        /// <summary>
        ///     Name of the parameter
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Does this parameter represent a variadic place holder
        /// </summary>
        public bool Variadic { get; private set; }

        /// <summary>
        ///     The parameter type
        /// </summary>
        public LSLType Type { get; private set; }

        /// <summary>
        ///     The parameter index, which gets set when the parameter is added to an <see cref="LSLFunctionSignature" /> or
        ///     <see cref="LSLEventSignature" />
        /// </summary>
        public int ParameterIndex { get; private set; }

        /// <summary>
        ///     Returns the signature string of the parameter.
        ///     If the parameter is not variadic, then the signature is simply the type name followed by the parameter name,
        ///     separated with a space.
        ///     Otherwise if the parameter is variadic, it will be formated as:  params any[] parameter_name
        ///     If the variadic parameter actually has a type, the 'any' keyword will be replaced with the name of that type.
        /// </summary>
        public string SignatureString
        {
            get
            {
                if (Variadic)
                {
                    return "params " + (Type == LSLType.Void ? "any" : Type.ToLSLTypeName()) + "[] " + Name;
                }

                //the any part is just for future proofing at the moment
                return (Type == LSLType.Void ? "any" : Type.ToLSLTypeName()) + " " + Name;
            }
        }


        /// <summary>
        ///     Returns a hash code for the <see cref="LSLParameterSignature" /> object.
        ///     The hash code is generated from the parameter Type, Name, ParameterIndex and Variadic status.
        /// </summary>
        /// <returns>The generated hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash*31 + Type.GetHashCode();
                hash = hash*31 + Name.GetHashCode();
                hash = hash*31 + ParameterIndex.GetHashCode();
                hash = hash*31 + Variadic.GetHashCode();
                return hash;
            }
        }


        /// <summary>
        ///     Determines if the <see cref="Type" />, <see cref="Name" />, <see cref="ParameterIndex" /> and
        ///     <see cref="Variadic" /> status are of equal values in another <see cref="LSLParameterSignature" /> object.
        ///     If obj is not actually an <see cref="LSLParameterSignature" /> object, or derived from one, than this function will always
        ///     return false.
        /// </summary>
        /// <param name="obj">The <see cref="LSLParameterSignature" /> object to compare this one to.</param>
        /// <returns>
        ///     True if 'obj' is an <see cref="LSLParameterSignature" /> object or derived type; And <see cref="Type" />,
        ///     <see cref="Name" />, <see cref="ParameterIndex" /> and <see cref="Variadic" /> status are of equal values in both
        ///     objects.
        /// </returns>
        public override bool Equals(object obj)
        {
            var o = obj as LSLParameterSignature;
            if (o == null)
            {
                return false;
            }

            return o.Name == Name && o.ParameterIndex == ParameterIndex && o.Type == Type && Variadic == o.Variadic;
        }
    }
}