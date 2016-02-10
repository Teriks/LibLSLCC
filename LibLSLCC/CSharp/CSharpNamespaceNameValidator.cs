#region FileInfo
// 
// File: CSharpNamespaceNameValidator.cs
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
using System.Text.RegularExpressions;

namespace LibLSLCC.CSharp
{
    /// <summary>
    /// Validation result created by <see cref="CSharpNamespaceNameValidator.Validate"/>
    /// </summary>
    public class CSharpNamespaceValidatorResult
    {
        /// <summary>
        /// Gets a value indicating whether the namespace was successfully parsed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; private set; }

        /// <summary>
        /// Gets a user friendly parsing error description when <see cref="Success"/> is false, otherwise <c>null</c>.
        /// </summary>
        /// <value>
        /// The parsing error description, if <see cref="Success"/> is <c>false</c>; otherwise, <c>null</c>.
        /// </value>
        public string ErrorDescription { get; private set; }

        internal CSharpNamespaceValidatorResult(bool success, string errorDescription = null)
        {
            Success = success;
            ErrorDescription = errorDescription;
        }
    }

    /// <summary>
    /// Tools to check the syntax validity of raw namespace name string
    /// </summary>
    public static class CSharpNamespaceNameValidator
    {
        private static readonly Regex DoubleDot = new Regex("\\.\\.");


        /// <summary>
        /// Parses and validates a given CSharp namespace string.
        /// </summary>
        /// <param name="namespaceName">A string representing the namespace.</param>
        /// <returns></returns>
        public static CSharpNamespaceValidatorResult Validate(string namespaceName)
        {
            if (DoubleDot.IsMatch(namespaceName))
            {
                return new CSharpNamespaceValidatorResult(false, "'..' is not valid in a namespace name.");
            }
            var inputs = namespaceName.Split('.');
            foreach (var item in inputs)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    return new CSharpNamespaceValidatorResult(false, "The namespace name is incomplete.");
                }

                if (!CSharpIDValidator.IsValidIdentifier(item))
                {
                    return new CSharpNamespaceValidatorResult(false,
                        string.Format("'{0}' is invalid namespace content.", item));
                }
            }
            return new CSharpNamespaceValidatorResult(true);
        }
    }
}