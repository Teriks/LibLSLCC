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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LibLSLCC.Collections;
using LibLSLCC.Settings;

namespace LibLSLCC.CSharp
{
    public class CSharpConstructorSignature : SettingsBaseClass<CSharpConstructorSignature>, IObservableHashSetItem
    {
        private readonly IReadOnlyHashedSet<string> _hashEqualityPropertyNames = new HashedSet<string>()
        {
            "FullSignature"
        };

        private CSharpConstructorSignatureValidationResult _validatedSignature;
        private string _fullSignature;


        private CSharpConstructorSignature()
        {
        }

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

        [XmlIgnore]
        public CSharpConstructorSignatureValidationResult ValidatedSignature
        {
            get { return _validatedSignature; }
            set { _validatedSignature = value; }
        }


        public static implicit operator CSharpConstructorSignature(string fullSignature)
        {
            return new CSharpConstructorSignature(fullSignature);
        }


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

        public override string ToString()
        {
            return FullSignature;
        }


        public override int GetHashCode()
        {
            if (FullSignature == null) return -1;
            return FullSignature.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var ns = obj as CSharpConstructorSignature;
            if (ns == null) return false;

            if (ns.FullSignature != null && FullSignature != null)
                return FullSignature.Equals(ns.FullSignature, StringComparison.Ordinal);

            return ns.FullSignature == FullSignature;
        }

        public IReadOnlyHashedSet<string> HashEqualityPropertyNames
        {
            get { return _hashEqualityPropertyNames; }
        }
    }
}