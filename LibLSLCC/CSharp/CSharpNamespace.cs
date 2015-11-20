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

using System;
using LibLSLCC.Collections;
using LibLSLCC.Settings;

namespace LibLSLCC.CSharp
{
    public class CSharpNamespace : SettingsBaseClass<CSharpNamespace>, IObservableHashSetItem
    {
        private string _name;
        private readonly IReadOnlyHashedSet<string> _hashEqualityPropertyNames = new HashedSet<string> {"Name"};

        /// <summary>
        /// Parameterless constructor used by <see cref="SettingsBaseClass{CSharpNamespace}"/>
        /// </summary>
        private CSharpNamespace()
        {

        }

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

            _name = name;
        }

        public static implicit operator CSharpNamespace(string name)
        {
            return new CSharpNamespace(name);
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(GetType().Name+".Name cannot be null or whitespace.", "value");
                }

                var validate = CSharpNamespaceNameValidator.Validate(value);
                if (!validate.Success)
                {
                    throw new ArgumentException(validate.ErrorDescription, "value");
                }

                SetField(ref _name,value, "Name");
            }
        }

        public override int GetHashCode()
        {
            if (Name == null) return -1;
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var ns = obj as CSharpNamespace;
            if (ns == null) return false;

            if (ns.Name != null && Name != null) return Name.Equals(ns.Name, StringComparison.Ordinal);

            return ns.Name == Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public IReadOnlyHashedSet<string> HashEqualityPropertyNames
        {
            get { return _hashEqualityPropertyNames; }
        }
    }
}