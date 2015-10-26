#region FileInfo
// 
// File: LSLLibraryDataSubsetNameParser.cs
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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace LibLSLCC.LibraryData
{
    /// <summary>
    /// Regex utility for parsing subset lists from the XML attributes in LSL library data documents
    /// and elsewhere in the library.
    /// </summary>

    public static class LSLLibraryDataSubsetNameParser 
    {
        private static readonly Regex SubsetName = new Regex(@"^([a-zA-Z]+[a-zA-Z_0-9\-]*)$");


        /// <summary>
        /// Parse a subset list from a string and return all the subset names in an enumerable.
        /// Subset names start with characters [a-zA-Z] followed by zero or more of the characters [a-zA-Z_0-9\-]
        /// and can be separated into a list of multiple subset names using commas.
        /// </summary>
        /// <param name="parse">The comma separated subset list to parse, or a single subset name.</param>
        /// <exception cref="LSLInvalidSubsetNameException">If a subset name that does not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*) is encountered.</exception>
        /// <returns></returns>
        public static IEnumerable<string> ParseSubsets(string parse)
        {
            return ThrowIfInvalid(parse.Split(','));
        }


        /// <summary>
        /// Throws an LSLInvalidSubsetNameException if any of the subset names in the given collection do not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*)
        /// </summary>
        /// <param name="names">An enumerable containing subset names to check.</param>
        /// <exception cref="LSLInvalidSubsetNameException">If an invalid subset name is encountered in the enumerable.</exception>
        public static IEnumerable<string> ThrowIfInvalid(IEnumerable<string> names)
        {
            foreach (var name in names.Select(item => item.Trim()))
            {
                ThrowIfInvalid(name);
                yield return name;
            }
        }


        /// <summary>
        /// Throws an LSLInvalidSubsetNameException if a given subset name does not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*)
        /// </summary>
        /// <param name="name">A subset name to check.</param>
        /// <exception cref="LSLInvalidSubsetNameException">If the given subset name was invalid.</exception>
        public static void ThrowIfInvalid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new LSLInvalidSubsetNameException("Subset name is null or whitespace, subset names must match the pattern: ([a-zA-Z]+[a-zA-Z_0-9\\-]*)");
            }

            if (!ValidateSubsetName(name))
            {
                throw new LSLInvalidSubsetNameException(string.Format("Subset name '{0}' is invalid, subset names must match the pattern: ([a-zA-Z]+[a-zA-Z_0-9\\-]*)", name));
            }

        }

        /// <summary>
        /// Validates that a single subset name matches the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*)
        /// </summary>
        /// <param name="name">The string to check for a correct subset name.</param>
        /// <returns>True if the string is a properly formated subset name.</returns>
        public static bool ValidateSubsetName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && SubsetName.IsMatch(name);
        }
    }
}