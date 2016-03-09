#region FileInfo

// 
// File: LSLAutoCompleteLocalParameter.cs
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

using LibLSLCC.CodeValidator;

#endregion

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    ///     Represents a local parameter parsed by the auto complete parser.
    /// </summary>
    public sealed class LSLAutoCompleteLocalParameter
    {
        internal LSLAutoCompleteLocalParameter(string name, string typeName, LSLSourceCodeRange range,
            LSLSourceCodeRange sourceRangeType,
            LSLSourceCodeRange sourceRangeName, LSLAutoCompleteScopeAddress address)
        {
            Name = name;
            TypeName = typeName;
            SourceRange = range;
            ScopeAddress = address;

            SourceRangeType = sourceRangeType;
            SourceRangeName = sourceRangeName;
        }


        /// <summary>
        ///     The <see cref="LSLSourceCodeRange" /> that encompasses the name of the local parameter.
        /// </summary>
        public LSLSourceCodeRange SourceRangeName { get; private set; }

        /// <summary>
        ///     The <see cref="LSLSourceCodeRange" /> that encompasses the type specifier of the local parameter.
        /// </summary>
        public LSLSourceCodeRange SourceRangeType { get; private set; }

        /// <summary>
        ///     Gets the scope address where the local parameter declaration began to be visible to child scopes.
        /// </summary>
        /// <value>
        ///     The scope address where the local parameter declaration began to be visible to child scopes.
        /// </value>
        public LSLAutoCompleteScopeAddress ScopeAddress { get; private set; }

        /// <summary>
        ///     Gets the name of the local parameter.
        /// </summary>
        /// <value>
        ///     The name of the local parameter.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets a string representing the type specifier used the declare the local parameter.
        /// </summary>
        /// <value>
        ///     A string representing the type specifier used the declare the local parameter.
        /// </value>
        public string TypeName { get; private set; }

        /// <summary>
        ///     Gets the <see cref="LSLSourceCodeRange" /> that encompasses the type specifier and name of the local parameter
        ///     declaration.
        /// </summary>
        /// <value>
        ///     The <see cref="LSLSourceCodeRange" /> that encompasses the type specifier and name of the local parameter
        ///     declaration.
        /// </value>
        public LSLSourceCodeRange SourceRange { get; private set; }
    }
}