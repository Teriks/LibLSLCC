#region FileInfo

// 
// File: ILSLLibrarySignature.cs
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

#endregion

namespace LibLSLCC.LibraryData
{
    /// <summary>
    ///     Interface for the common properties shared by LSLLibrary*Signature objects.
    /// </summary>
    public interface ILSLLibrarySignature
    {
        /// <summary>
        ///     Whether or not this library signature is marked as deprecated or not.
        /// </summary>
        bool Deprecated { get; }

        /// <summary>
        ///     Returns the documentation string attached to this library signature.
        /// </summary>
        string DocumentationString { get; }

        /// <summary>
        ///     Additional dynamic property values that can be attached to the constant signature and parsed from XML
        /// </summary>
        IDictionary<string, string> Properties { get; }

        /// <summary>
        ///     Returns a formated string containing the signature and documentation for this library signature.
        ///     It consists of the SignatureString followed by a semi-colon, and then followed by a new-line and
        ///     DocumentationString
        ///     if the documentation string is not null.
        /// </summary>
        string SignatureAndDocumentation { get; }

        /// <summary>
        ///     The library subsets this signature belongs to/is shared among.
        /// </summary>
        LSLLibraryDataSubsetCollection Subsets { get; }
    }
}