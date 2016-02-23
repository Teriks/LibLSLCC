#region FileInfo

// 
// File: ILSLAutoCompleteParser.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2016, Teriks
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
using System.IO;

#endregion

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    ///     Interface for auto complete parsers.
    /// </summary>
    public interface ILSLAutoCompleteParser : ILSLAutoCompleteParserState
    {
        /// <summary>
        ///     Get an enumerable of <see cref="LSLAutoCompleteLocalLabel" /> objects representing local labels
        ///     that are currently accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <param name="sourceCode">The source code of the entire script.</param>
        /// </summary>
        IEnumerable<LSLAutoCompleteLocalLabel> GetLocalLabels(string sourceCode);


        /// <summary>
        ///     Get an enumerable of <see cref="LSLAutoCompleteLocalJump" /> objects representing local jump statements
        ///     that are currently accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <param name="sourceCode">The source code of the entire script.</param>
        /// </summary>
        IEnumerable<LSLAutoCompleteLocalJump> GetLocalJumps(string sourceCode);


        /// <summary>
        ///     Preforms an auto-complete parse on the specified stream of LSL source code, up to an arbitrary offset.
        /// </summary>
        /// <param name="stream">The input source code stream.</param>
        /// <param name="toOffset">To offset to parse up to (the cursor offset).</param>
        void Parse(TextReader stream, int toOffset);
    }
}