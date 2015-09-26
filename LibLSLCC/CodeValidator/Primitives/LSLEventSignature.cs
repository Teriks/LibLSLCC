#region FileInfo
// 
// File: LSLEventSignature.cs
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
    public class LSLEventSignature
    {
        private readonly List<LSLParameter> _parameters;

        public LSLEventSignature(LSLEventSignature other)
        {
            Name = other.Name;
            _parameters = other._parameters.ToList();
        }

        protected LSLEventSignature()
        {
            _parameters = new List<LSLParameter>();
            Name = "";
        }

        public LSLEventSignature(string name, IEnumerable<LSLParameter> parameters)
        {
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

        protected LSLEventSignature(string name)
        {
            Name = name;
            _parameters = new List<LSLParameter>();
        }

        /// <summary>
        ///     The number of parameters the event handler signature has
        /// </summary>
        public int ParameterCount
        {
            get { return _parameters.Count(); }
        }

        /// <summary>
        ///     The event handlers name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Indexable list of objects describing the event handlers parameters
        /// </summary>
        public IReadOnlyList<LSLParameter> Parameters
        {
            get { return _parameters; }
        }

        public string SignatureString
        {
            get
            {
                return Name + "(" +
                       string.Join(", ", Parameters.Select(x => LSLTypeTools.ToLSLTypeString(x.Type) + " " + x.Name)) +
                       ")";
            }
        }

        public override string ToString()
        {
            return SignatureString;
        }

        /// <summary>
        ///     Determines if two event handler signatures match exactly, parameter names do not matter but parameter
        ///     types do.
        /// </summary>
        /// <param name="otherSignature">The other event handler signature to compare to.</param>
        /// <returns>True if the two signatures are identical.</returns>
        public bool SignatureMatches(LSLEventSignature otherSignature)
        {
            if (Name != otherSignature.Name)
            {
                return false;
            }
            if (ParameterCount != otherSignature.ParameterCount)
            {
                return false;
            }
            for (var i = 0; i < ParameterCount; i++)
            {
                if (Parameters[i].Type != otherSignature.Parameters[i].Type)
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash*31 + Name.GetHashCode();

            return Parameters.Aggregate(hash, (current, lslParameter) => current*31 + lslParameter.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            var o = obj as LSLEventSignature;
            if (o == null)
            {
                return false;
            }

            return SignatureMatches(o);
        }

        public void AddParameter(LSLParameter parameter)
        {
            if (parameter.Variadic)
            {
                throw new ArgumentException(
                    "Cannot add variadic parameters to an event signature", "parameter");
            }

            parameter.ParameterIndex = _parameters.Count;
            _parameters.Add(parameter);
        }

        public static LSLEventSignature Parse(string cSignature)
        {
            var regex = new LSLEventSignatureRegex("", ";*");
            var m = regex.GetSignature(cSignature);
            if (m == null)
            {
                throw new ArgumentException("Syntax error parsing event signature", "cSignature");
            }
            return m;
        }
    }
}