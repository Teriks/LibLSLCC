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

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    /// Autocompleter options.
    /// </summary>
    [Flags]
    public enum LSLAutoCompleteParseOptions
    {
        /// <summary>
        /// Specify no options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Block autocomplete if a invalid prefix character is found before <see cref="ILSLAutoCompleteParserState.ParseToOffset"/>. <para/>
        /// Invalid prefixes are determined by <see cref="ILSLAutoCompleteParser.IsValidSuggestionPrefix"/>.
        /// </summary>
        /// <seealso cref="ILSLAutoCompleteParserState.InvalidPrefix"/>
        BlockOnInvalidPrefix = 1,

        /// <summary>
        /// Block autocomplete if a invalid prefix keyword is found before <see cref="ILSLAutoCompleteParserState.ParseToOffset"/>. <para/>
        /// Invalid prefixes are determined by <see cref="ILSLAutoCompleteParser.IsInvalidSuggestionKeywordPrefix"/>. <para/>
        /// An invalid keyword followed only by space before the ParseToOffset will cause autocomplete to be blocked.
        /// </summary>
        /// <seealso cref="ILSLAutoCompleteParserState.InvalidKeywordPrefix"/>
        BlockOnInvalidKeywordPrefix = 2,
    }

    /// <summary>
    ///     Interface for auto complete parsers.
    /// </summary>
    public interface ILSLAutoCompleteParser : ILSLAutoCompleteParserState
    {
        /// <summary>
        ///     Get an enumerable of <see cref="LSLAutoCompleteLocalLabel" /> objects representing local labels
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        /// <param name="sourceCode">The source code of the entire script.</param>
        /// <returns>An enumerable of <see cref="LSLAutoCompleteLocalLabel" /> objects that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.</returns>
        IEnumerable<LSLAutoCompleteLocalLabel> GetLocalLabels(string sourceCode);


        /// <summary>
        ///     Get an enumerable of <see cref="LSLAutoCompleteLocalJump" /> objects representing local jump statements
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />. 
        /// </summary>
        /// <param name="sourceCode">The source code of the entire script.</param>
        /// <returns>An enumerable of <see cref="LSLAutoCompleteLocalJump" /> objects that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.</returns>
        IEnumerable<LSLAutoCompleteLocalJump> GetLocalJumps(string sourceCode);


        /// <summary>
        ///     Preforms an auto-complete parse on the specified stream of LSL source code, up to an arbitrary offset.
        /// </summary>
        /// <param name="code">The input source code.</param>
        /// <param name="toOffset">To offset to parse up to (the cursor offset).</param>
        /// <param name="options">Parse options.</param>
        void Parse(string code, int toOffset, LSLAutoCompleteParseOptions options);


        /// <summary>
        /// Determine if autocomplete should be blocked if the only thing separating a given keyword from <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is whitespace. <para/>
        /// In other words, autocomplete cannot continue if <paramref name="keyword"/> comes before the cursor with only whitespace inbetween.
        /// </summary>
        /// <param name="keyword">The keyword or character sequence to test.</param>
        /// <returns><c>true</c> if the keyword/sequence blocks autocomplete.</returns>
        /// <seealso cref="LSLAutoCompleteParseOptions.BlockOnInvalidKeywordPrefix"/>
        /// <seealso cref="ILSLAutoCompleteParserState.InvalidKeywordPrefix"/>
        bool IsInvalidSuggestionKeywordPrefix(string keyword);

        /// <summary>
        /// Determine if a given character can come immediately before an autocomplete suggestion.  An empty string represents the begining of the source code.
        /// </summary>
        /// <param name="character">The character to test, or an empty string.</param>
        /// <returns><c>true</c> if the given character can appear before a suggestion.</returns>
        /// <seealso cref="LSLAutoCompleteParseOptions.BlockOnInvalidPrefix"/>
        /// <seealso cref="ILSLAutoCompleteParserState.InvalidPrefix"/>
        bool IsValidSuggestionPrefix(string character);

        /// <summary>
        /// Determine if a given character can come immediately after an autocomplete suggestion.  An empty string represents the end of the source code.
        /// </summary>
        /// <param name="character">The character to test, or an empty string.</param>
        /// <returns><c>true</c> if the given character can appear after a suggestion.</returns>
        bool IsValidSuggestionSuffix(string character);
    }
}