#region FileInfo
// 
// File: CSharpConstructorSignature.cs
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

using System;
using System.Xml.Serialization;
using LibLSLCC.Collections;
using LibLSLCC.Settings;

namespace LibLSLCC.CSharp
{
    /// <summary>
    /// Abstracts a CSharp constructor signature in the form: () | (Type param, ..) | (...) : base(...) | (...) : this(...)
    /// If an invalid CSharp constructor signature string is used to construct this object, an <see cref="ArgumentException"/> will be thrown.
    /// </summary>
    public class CSharpConstructorSignature : SettingsBaseClass<CSharpConstructorSignature>, IObservableHashSetItem
    {
        private readonly IReadOnlyHashedSet<string> _hashEqualityPropertyNames = new HashedSet<string>()
        {
            "FullSignature"
        };

        private CSharpConstructorSignatureValidationResult _validatedSignature;
        private string _fullSignature;

        /// <summary>
        /// Parameterless constructor used by <see cref="SettingsBaseClass{CSharpNamespace}"/>
        /// </summary>
        private CSharpConstructorSignature()
        {
        }


        /// <summary>
        /// Gets or sets the full constructor signature.  Parsing and validation takes place when this property is set.
        /// </summary>
        /// <value>
        /// The full constructor signature.
        /// </value>
        /// <exception cref="System.ArgumentNullException">Thrown if the value is set to <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if you set a value that is whitespace.
        /// or
        /// If <see cref="CSharpConstructorSignatureValidator.Validate"/> returns an unsuccessful parse result when setting a value.
        /// </exception>
        public string FullSignature
        {
            get { return _fullSignature; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Constructor signature string cannot be null.");
                }
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Constructor signature string cannot be whitespace.", "value");
                }

                var vSig = CSharpConstructorSignatureValidator.Validate(value);

                if (!vSig.Success)
                {
                    throw new ArgumentException(_validatedSignature.ErrorDescription, "value");
                }


                SetField(ref _validatedSignature, vSig, "ValidatedSignature");
                SetField(ref _fullSignature, value, "FullSignature");
            }
        }

        /// <summary>
        /// Gets the parsing/validation results of the constructor signature string.
        /// </summary>
        /// <value>
        /// The parsing/validation results of the constructor signature string.
        /// </value>
        [XmlIgnore]
        public CSharpConstructorSignatureValidationResult ValidatedSignature
        {
            get { return _validatedSignature; }
        }


        public static implicit operator CSharpConstructorSignature(string fullSignature)
        {
            return new CSharpConstructorSignature(fullSignature);
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpConstructorSignature"/> class.
        /// </summary>
        /// <param name="fullSignature">The full signature.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="fullSignature"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if <paramref name="fullSignature"/> is whitespace.
        /// or
        /// If <see cref="CSharpConstructorSignatureValidator.Validate"/> returns an unsuccessful parse result.
        /// </exception>
        public CSharpConstructorSignature(string fullSignature)
        {
            if (fullSignature == null)
            {
                throw new ArgumentNullException("fullSignature", "Constructor signature string cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(fullSignature))
            {
                throw new ArgumentException("Constructor signature string cannot be whitespace.", "fullSignature");
            }

            _validatedSignature = CSharpConstructorSignatureValidator.Validate(fullSignature);

            if (!_validatedSignature.Success)
            {
                throw new ArgumentException(_validatedSignature.ErrorDescription, "fullSignature");
            }

            _fullSignature = fullSignature;
        }

        /// <summary>
        /// Returns <see cref="FullSignature"/>.
        /// </summary>
        /// <returns>
        /// <see cref="FullSignature"/>.
        /// </returns>
        public override string ToString()
        {
            return FullSignature;
        }

        /// <summary>
        /// Returns the hash code of <see cref="FullSignature"/>.
        /// </summary>
        /// <returns>
        /// Returns the hash code of <see cref="FullSignature"/>. 
        /// </returns>
        public override int GetHashCode()
        {
            if (FullSignature == null) return -1;
            return FullSignature.GetHashCode();
        }

        /// <summary>
        /// Compares using <see cref="FullSignature"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is a <see cref="CSharpConstructorSignature"/> with an equal <see cref="FullSignature"/> value; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var ns = obj as CSharpConstructorSignature;
            if (ns == null) return false;

            if (ns.FullSignature != null && FullSignature != null)
                return FullSignature.Equals(ns.FullSignature, StringComparison.Ordinal);

            return ns.FullSignature == FullSignature;
        }

        IReadOnlyHashedSet<string> IObservableHashSetItem.HashEqualityPropertyNames
        {
            get { return _hashEqualityPropertyNames; }
        }
    }
}