#region FileInfo

// 
// File: CSharpNamespace.cs
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
using LibLSLCC.Collections;
using LibLSLCC.Settings;

#endregion

namespace LibLSLCC.CSharp
{
    /// <summary>
    ///     Abstraction that provides parsing and validation for CSharp namespace strings.
    /// </summary>
    public sealed class CSharpNamespace : SettingsBaseClass<CSharpNamespace>, IObservableHashSetItem
    {
        private readonly IReadOnlyHashedSet<string> _hashEqualityPropertyNames = new HashedSet<string> {"Name"};
        private string _fullSignature;


        /// <summary>
        ///     Parameterless constructor used by <see cref="SettingsBaseClass{CSharpNamespace}" />
        /// </summary>
        private CSharpNamespace()
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="CSharpNamespace" /> class by parsing a namespace string.
        /// </summary>
        /// <param name="name">The namespace string to parse.</param>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if <paramref name="name" /> is <c>null</c>.
        ///     or
        ///     If <paramref name="name" /> does not pass validation by <see cref="CSharpNamespaceNameValidator.Validate" />.
        /// </exception>
        public CSharpNamespace(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or whitespace.", "name");
            }

            var validate = CSharpNamespaceNameValidator.Validate(name);
            if (!validate.Success)
            {
                throw new ArgumentException(validate.ErrorDescription, "name");
            }

            _fullSignature = name;
        }


        /// <summary>
        ///     Gets or sets the full signature of the namespace.
        /// </summary>
        /// <value>
        ///     The full namespace signature.
        /// </value>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if the value is set to <c>null</c>.
        ///     or
        ///     If the set value does not pass validation by <see cref="CSharpNamespaceNameValidator.Validate" />.
        /// </exception>
        public string FullSignature
        {
            get { return _fullSignature; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(GetType().Name + ".FullSignature cannot be null or whitespace.", "value");
                }

                var validate = CSharpNamespaceNameValidator.Validate(value);
                if (!validate.Success)
                {
                    throw new ArgumentException(validate.ErrorDescription, "value");
                }

                SetField(ref _fullSignature, value, "FullSignature");
            }
        }

        IReadOnlyHashedSet<string> IObservableHashSetItem.HashEqualityPropertyNames
        {
            get { return _hashEqualityPropertyNames; }
        }


        /// <summary>
        ///     Implicitly converts a string into a CSharpNamespace.
        /// </summary>
        /// <param name="name">The name string to convert from.</param>
        /// <returns>A new <see cref="CSharpNamespace"/> implicitly converted from a string.</returns>
        public static implicit operator CSharpNamespace(string name)
        {
            return new CSharpNamespace(name);
        }


        /// <summary>
        ///     Returns the hash code of <see cref="FullSignature" />.
        /// </summary>
        /// <returns>
        ///     Returns the hash code of <see cref="FullSignature" />.
        /// </returns>
        public override int GetHashCode()
        {
            if (FullSignature == null) return -1;
            return FullSignature.GetHashCode();
        }


        /// <summary>
        ///     Compares using <see cref="FullSignature" />.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is a <see cref="CSharpNamespace" /> with an equal
        ///     <see cref="FullSignature" /> value; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var ns = obj as CSharpNamespace;
            if (ns == null) return false;

            if (ns.FullSignature != null && FullSignature != null)
                return FullSignature.Equals(ns.FullSignature, StringComparison.Ordinal);

            return ns.FullSignature == FullSignature;
        }


        /// <summary>
        ///     Returns <see cref="FullSignature" />.
        /// </summary>
        /// <returns>
        ///     <see cref="FullSignature" />.
        /// </returns>
        public override string ToString()
        {
            return FullSignature;
        }
    }
}