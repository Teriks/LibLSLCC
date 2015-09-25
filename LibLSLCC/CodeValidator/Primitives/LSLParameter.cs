#region FileInfo
// 
// 
// File: LSLParameter.cs
// 
// Last Compile: 25/09/2015 @ 11:47 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
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
using LibLSLCC.CodeValidator.Enums;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLParameter
    {
        /// <summary>
        ///     Construct a parameter object
        /// </summary>
        /// <param name="type">The parameter type</param>
        /// <param name="name">The parameter name</param>
        /// <param name="variadic">Is the parameter variadic</param>
        /// <exception cref="ArgumentException">If variadic is true and type does not equal LSLType.Void</exception>
        public LSLParameter(LSLType type, string name, bool variadic)
        {
            Type = type;
            Name = name;
            Variadic = variadic;

            if (variadic && type != LSLType.Void)
            {
                throw new ArgumentException("LSLType must be Void for variadic parameters", "type");
            }
        }

        /// <summary>
        ///     Name of the parameter
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Does this parameter represent a variadic place holder
        /// </summary>
        public bool Variadic { get; }

        /// <summary>
        ///     The parameter type
        /// </summary>
        public LSLType Type { get; }

        /// <summary>
        ///     The parameter index, which gets set when the parameter is added to an LSLFunctionSignature or LSLEventSignature
        /// </summary>
        public int ParameterIndex { get; set; }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash*31 + Type.GetHashCode();
            hash = hash*31 + Name.GetHashCode();
            hash = hash*31 + ParameterIndex.GetHashCode();
            hash = hash*31 + Variadic.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            var o = obj as LSLParameter;
            if (o == null)
            {
                return false;
            }

            return o.Name == Name && o.ParameterIndex == ParameterIndex && o.Type == Type && Variadic == o.Variadic;
        }
    }
}