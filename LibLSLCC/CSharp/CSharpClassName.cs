#region FileInfo
// 
// File: CSharpClassName.cs
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
    public class CSharpClassName : SettingsBaseClass<CSharpClassName>, IObservableHashSetItem
    {
        private readonly IReadOnlyHashedSet<string> _hashEqualityPropertyNames = new HashedSet<string>() {"FullSignature"};

        private CSharpClassNameValidationResult _validatedName;
        private string _fullSignature;

        /// <summary>
        /// Parameterless constructor used by <see cref="SettingsBaseClass{CSharpNamespace}"/>
        /// </summary>
        private CSharpClassName()
        {
            
        }

        public CSharpClassName(string fullSignature)
        {
            if (fullSignature == null)
            {
                throw new ArgumentNullException("fullSignature", "Class signature string cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(fullSignature))
            {
                throw new ArgumentException("Class signature string cannot be whitespace.", "fullSignature");
            }

            _validatedName = CSharpClassNameValidator.ValidateDeclaration(fullSignature);

            if (!_validatedName.Success)
            {
                throw new ArgumentException(_validatedName.ErrorDescription, "fullSignature");
            }

            _fullSignature = fullSignature;
        }


        public static implicit operator CSharpClassName(string fullSignature)
        {
            return new CSharpClassName(fullSignature);
        }

        [XmlIgnore]
        public string BaseName { get { return _validatedName.BaseName; } }


        [XmlIgnore]
        public string QualifiedName { get { return _validatedName.QualifiedName; } }


        [XmlIgnore]
        public CSharpClassNameValidationResult ValidatedName { get { return _validatedName; } }

        public string FullSignature
        {
            get { return _fullSignature; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Class signature string cannot be null.");
                }
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Class signature string cannot be whitespace.", "value");
                }

                var vName = CSharpClassNameValidator.ValidateDeclaration(value);

                if (!vName.Success)
                {
                    throw new ArgumentException(_validatedName.ErrorDescription, "value");
                }

                OnPropertyChanging("QualifiedName");
                OnPropertyChanging("BaseName");
                SetField(ref _validatedName, vName, "ValidatedName");
                OnPropertyChanged("QualifiedName");
                OnPropertyChanged("BaseName");

                SetField(ref _fullSignature, value, "FullSignature");
            }
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
            var ns = obj as CSharpClassName;
            if (ns == null) return false;

            if (ns.FullSignature != null && FullSignature != null) return FullSignature.Equals(ns.FullSignature, StringComparison.Ordinal);

            return ns.FullSignature == FullSignature;
        }

        public IReadOnlyHashedSet<string> HashEqualityPropertyNames
        {
            get { return _hashEqualityPropertyNames; }
        }
    }
}
