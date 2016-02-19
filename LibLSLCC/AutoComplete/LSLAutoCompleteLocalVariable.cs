#region FileInfo
// 
// File: LSLAutoCompleteLocalVariable.cs
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

using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    /// Represents a local variable declaration parsed by the auto complete parser.
    /// </summary>
    public sealed class LSLAutoCompleteLocalVariable
    {
        internal LSLAutoCompleteLocalVariable(string name, string typeString, LSLSourceCodeRange range, LSLSourceCodeRange sourceRangeType,
            LSLSourceCodeRange sourceRangeName, LSLAutoCompleteScopeAddress address)
        {
            Name = name;
            TypeString = typeString;
            SourceRange = range;
            ScopeAddress = address;
            SourceRangeName = sourceRangeName;

            SourceRangeType = sourceRangeType;
        }

        /// <summary>
        /// Gets the <see cref="LSLSourceCodeRange"/> that encompasses the local variable name.
        /// </summary>
        /// <value>
        /// The <see cref="LSLSourceCodeRange"/> that encompasses the local variable name.
        /// </value>
        public LSLSourceCodeRange SourceRangeName { get; private set; }


        /// <summary>
        /// Gets the <see cref="LSLSourceCodeRange"/> that encompasses the local variable type specifier.
        /// </summary>
        /// <value>
        /// The <see cref="LSLSourceCodeRange"/> that encompasses the local variable type specifier.
        /// </value>
        public LSLSourceCodeRange SourceRangeType { get; private set; }

        /// <summary>
        /// Gets the scope address where this local variable came in to existence.
        /// </summary>
        /// <value>
        /// The scope address where this local variable came in to existence.
        /// </value>
        public LSLAutoCompleteScopeAddress ScopeAddress { get; private set; }

        /// <summary>
        /// Gets the name of the local variable.
        /// </summary>
        /// <value>
        /// The name of the local variable.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a string representing the type specifier used when declaring the local variable.
        /// </summary>
        /// <value>
        /// A string representing the type specifier used when declaring the local variable.
        /// </value>
        public string TypeString { get; private set; }

        /// <summary>
        /// Gets the <see cref="LSLSourceCodeRange"/> that encompasses the local variable declaration statement.
        /// </summary>
        /// <value>
        /// The <see cref="LSLSourceCodeRange"/> that encompasses the local variable declaration statement.
        /// </value>
        public LSLSourceCodeRange SourceRange { get; private set; }
    }
}