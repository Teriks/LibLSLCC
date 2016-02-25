#region FileInfo

// 
// File: CSharpClassDeclarationName.cs
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
using System.Xml.Serialization;
using LibLSLCC.Collections;
using LibLSLCC.Settings;

#endregion

namespace LibLSLCC.CSharp
{
    /// <summary>
    ///     Abstracts a CSharp class declaration name/name signature string, providing validation through parsing.
    /// </summary>
    public sealed class CSharpClassDeclarationName : SettingsBaseClass<CSharpClassDeclarationName>,
        IObservableHashSetItem
    {
        private readonly IReadOnlyHashedSet<string> _hashEqualityPropertyNames = new HashedSet<string>()
        {
            "FullSignature"
        };

        private string _fullSignature;
        private CSharpClassNameValidationResult _validatedSignature;


        /// <summary>
        ///     Parameterless constructor used by <see cref="SettingsBaseClass{CSharpNamespace}" />
        /// </summary>
        private CSharpClassDeclarationName()
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="CSharpClassDeclarationName" /> class.
        /// </summary>
        /// <param name="fullSignature">The full signature.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="fullSignature" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if <paramref name="fullSignature" /> is whitespace.
        ///     or
        ///     If <paramref name="fullSignature" /> does not pass validation using
        ///     <see cref="CSharpClassNameValidator.ValidateDeclaration" />.
        /// </exception>
        public CSharpClassDeclarationName(string fullSignature)
        {
            if (fullSignature == null)
            {
                throw new ArgumentNullException("fullSignature", "Class signature string cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(fullSignature))
            {
                throw new ArgumentException("Class signature string cannot be whitespace.", "fullSignature");
            }

            _validatedSignature = CSharpClassNameValidator.ValidateDeclaration(fullSignature);

            if (!_validatedSignature.Success)
            {
                throw new ArgumentException(_validatedSignature.ErrorDescription, "fullSignature");
            }

            _fullSignature = fullSignature;
        }


        /// <summary>
        ///     Gets the BaseName of the parsed type, excluding generic arguments.  This is the last name in a qualified type name,
        ///     if the type is qualified.
        /// </summary>
        /// <value>
        ///     This is the last name in a qualified type name, if the type is qualified;  otherwise, simply the full given name.
        /// </value>
        [XmlIgnore]
        public string BaseName
        {
            get { return _validatedSignature.BaseName; }
        }

        /// <summary>
        ///     Gets the qualified name of the parsed type, excluding generic arguments.
        /// </summary>
        /// <value>
        ///     The qualified name of the parsed type, excluding generic arguments.
        /// </value>
        [XmlIgnore]
        public string QualifiedName
        {
            get { return _validatedSignature.QualifiedName; }
        }

        /// <summary>
        ///     Gets the parsing/validation results of the class/type signature string.
        /// </summary>
        /// <value>
        ///     The parsing/validation results of the class/type signature string.
        /// </value>
        [XmlIgnore]
        public CSharpClassNameValidationResult ValidatedSignature
        {
            get { return _validatedSignature; }
        }

        /// <summary>
        ///     Gets or sets the full class/type signature.  Parsing and validation is done when this property is set.
        /// </summary>
        /// <value>
        ///     The full class/type signature.
        /// </value>
        /// <exception cref="System.ArgumentNullException">Thrown if you set a <c>null</c> value.</exception>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if you set a value that is whitespace.
        ///     or
        ///     If you set a value that does not pass validation using <see cref="CSharpClassNameValidator.ValidateDeclaration" />.
        /// </exception>
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
                    throw new ArgumentException(_validatedSignature.ErrorDescription, "value");
                }

                string oldQualifiedName = null;
                string oldBaseName = null;

                if (_validatedSignature != null)
                {
                    oldQualifiedName = _validatedSignature.QualifiedName;
                    oldBaseName = _validatedSignature.BaseName;
                }

                OnPropertyChanging("QualifiedName", oldQualifiedName, vName.QualifiedName);
                OnPropertyChanging("BaseName", oldBaseName, vName.BaseName);

                SetField(ref _validatedSignature, vName, "ValidatedSignature");

                OnPropertyChanged("QualifiedName", oldQualifiedName, QualifiedName);
                OnPropertyChanged("BaseName", oldBaseName, BaseName);

                SetField(ref _fullSignature, value, "FullSignature");
            }
        }

        IReadOnlyHashedSet<string> IObservableHashSetItem.HashEqualityPropertyNames
        {
            get { return _hashEqualityPropertyNames; }
        }


        /// <summary>
        ///     Implicitly converts a string into a <see cref="CSharpClassDeclarationName" /> by parsing it.
        /// </summary>
        /// <param name="fullSignature">The string representing the full signature of the class declaration name.</param>
        /// <returns>The newly created <see cref="CSharpClassDeclarationName" /> from the string.</returns>
        public static implicit operator CSharpClassDeclarationName(string fullSignature)
        {
            return new CSharpClassDeclarationName(fullSignature);
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
        ///     <c>true</c> if the specified <see cref="System.Object" /> is a <see cref="CSharpClassDeclarationName" /> with an
        ///     equal <see cref="FullSignature" /> value; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var ns = obj as CSharpClassDeclarationName;
            if (ns == null) return false;

            if (ns.FullSignature != null && FullSignature != null)
                return FullSignature.Equals(ns.FullSignature, StringComparison.Ordinal);

            return ns.FullSignature == FullSignature;
        }
    }
}