#region FileInfo

// 
// File: LSLAutoCompleteParser.cs
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
using Antlr4.Runtime;
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    ///     An LSL parser that can help with implementing context aware auto-complete inside of code editors.
    /// </summary>
    public sealed class LSLAutoCompleteParser : LSLAutoCompleteParserBase
    {

        private static string LookBehindOffset(string text, int caretOffset, int behindOffset, int length)
        {
            return (caretOffset - behindOffset) > 0 ? text.Substring(caretOffset - behindOffset, length) : "";
        }


        //An auto complete parse only takes place if the character preceding the inserted text
        //matches one of these
        private static readonly HashSet<string> _validSuggestionPrefixes = new HashSet<string>
        {
            "\t", //after a word break
            "\r", // ^
            "\n", // ^
            " ", // ^
            //".",  //member accessor (not used yet)
            "{", //after the start of a scope
            "}", //after a scope
            "[", //after the start of a list literal
            "(", //after the start of a parenthesized expression group, cast, parameter list, ect..
            ")", //after a single statement code scope starts
            "<", //after comparison, vectors start and left shift operator
            ">", //after right shift, and comparison
            "&", //after logical and bitwise and
            "^", //after xor
            "|", //after logical and bitwise or
            "~", //after bitwise not
            "!", //after logical not
            ",", //after a comma in an expression list
            ";", //after a statement terminator
            "=", //after all types of assignment
            "+", //after addition operator or prefix increment
            "-", //after negation, subtraction operator or prefix decrement
            "*", //after multiplication operator
            "/", //after division operator
            "%", //after modulus operator
            "@", //after a label name prefix
            "" //at the beginning of the file
        };


        //Used when something is being inserted over a selection.
        //an auto complete parse only takes place if character after the end of the selection
        //matches one of these.
        private static readonly HashSet<string> _validSuggestionSuffixes = new HashSet<string>
        {
            "\t", //before a word break
            "\r", // ^
            "\n", // ^
            " ", // ^
            "{", //ending point is at the start of an anonymous scope
            "}", //for when a selection ending point is at the end of a scope
            "]", //before the end of a list literal
            "(", //before a cast or parenthesized expression
            ")", //at end of function parameters, cast, parenthesized expr, etc..
            "<", //before comparison or left shift
            ">", //before comparison or right shift
            "&", //before logical or bitwise and
            "^", //before xor
            "|", //before or or bitwise or
            ",", //before a comma in an expression list
            ";", //before a statement terminator
            "=", //before direct assignment
            "+", //before addition, add assign or postfix increment
            "-", //before subtraction, subtract assign or postfix decrement
            "*", //before multiply or multiply assign
            "/", //before divide or divide assign
            "%", //before modulus or modulus assign
            "" //before the end of the file
        };





        private static string FindKeywordWordBehindOffset(string text, int caretOffset)
        {
            var behindOffset = caretOffset > 1 ? caretOffset - 1 : -1;

            var inWord = false;
            var word = "";
            while (behindOffset >= 0)
            {
                var c = text[behindOffset];

                if (char.IsWhiteSpace(c) && inWord == false)
                {
                    behindOffset--;
                    continue;
                }


                inWord = true;


                //take advantage of the fact LSL keywords have no special symbols in them
                if (char.IsLetter(c))
                {
                    word = c + word;
                }
                else
                {
                    return word == "" ? null : word;
                }

                behindOffset--;
            }

            return word == "" ? null : word;
        }


        private  bool KeywordPriorBlocksCompletion(string code, int caretOffset)
        {
            var keywordBehindOffset = FindKeywordWordBehindOffset(code, caretOffset);

            if (keywordBehindOffset == null) return false;

            return !IsInvalidSuggestionKeywordPrefix(keywordBehindOffset);
        }


        /// <summary>
        ///     Preforms an auto-complete parse on the specified stream of LSL source code, up to an arbitrary offset.
        /// </summary>
        /// <param name="code">The input source code.</param>
        /// <param name="toOffset">To offset to parse up to (the cursor offset).</param>
        /// <param name="options">Parse options.</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="toOffset" /> is not greater than zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="code" /> is <c>null</c>.</exception>
        public override void Parse(string code, int toOffset, LSLAutoCompleteParseOptions options)
        {
            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            if (toOffset < 0)
            {
                throw new ArgumentOutOfRangeException("toOffset", "toOffset cannot be less than zero.");
            }

            var detectInStringOrComment = new LSLCommentAndStringDetector(code, toOffset);

            var visitor = new LSLAutoCompleteVisitor(toOffset);

            ParserState = visitor;


            var behind = LookBehindOffset(code, toOffset, 1, 1);



            if ((options & LSLAutoCompleteParseOptions.BlockOnInvalidPrefix) == LSLAutoCompleteParseOptions.BlockOnInvalidPrefix)
            {
                if (!IsValidSuggestionPrefix(behind))
                {
                    visitor.InvalidPrefix = true;

                    return;
                }
            }


            if ((options & LSLAutoCompleteParseOptions.BlockOnInvalidKeywordPrefix) ==
                LSLAutoCompleteParseOptions.BlockOnInvalidKeywordPrefix)
            {
                if (KeywordPriorBlocksCompletion(code, toOffset))
                {
                    visitor.InvalidKeywordPrefix = true;

                    return;
                }
            }


            if (detectInStringOrComment.InComment || detectInStringOrComment.InString)
            {
                visitor.InString = detectInStringOrComment.InString;
                visitor.InComment = detectInStringOrComment.InComment;
                visitor.InLineComment = detectInStringOrComment.InLineComment;
                visitor.InBlockComment = detectInStringOrComment.InBlockComment;
                return;
            }


            var inputStream = new AntlrInputStream(new StringReader(code), toOffset);

            var lexer = new LSLLexer(inputStream);

            var tokenStream = new CommonTokenStream(lexer);

            var parser = new LSLParser(tokenStream);


            parser.RemoveErrorListeners();
            lexer.RemoveErrorListeners();

            visitor.Parse(parser.compilationUnit());
        }


        /// <summary>
        /// Determine if autocomplete should be blocked if the only thing separating a given keyword from <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is whitespace. <para/>
        /// In other words, autocomplete cannot continue if <paramref name="keyword"/> comes before the cursor with only whitespace inbetween.
        /// </summary>
        /// <param name="keyword">The keyword or character sequence to test.</param>
        /// <returns><c>true</c> if the keyword/sequence blocks autocomplete.</returns>
        /// <seealso cref="LSLAutoCompleteParseOptions.BlockOnInvalidKeywordPrefix"/>
        /// <seealso cref="ILSLAutoCompleteParserState.InvalidKeywordPrefix"/>
        /// <exception cref="ArgumentNullException"><paramref name="keyword"/> is <see langword="null" />.</exception>
        public override bool IsInvalidSuggestionKeywordPrefix(string keyword)
        {
            if (keyword == null) throw new ArgumentNullException("keyword");

            switch (keyword)
            {
                case "integer":
                case "float":
                case "string":
                case "vector":
                case "rotation":
                case "key":
                case "list":
                case "default":
                    return true;

                default:
                    return false;
            }
        }


        /// <summary>
        /// Determine if a given character can come immediately before an autocomplete suggestion.  An empty string represents the begining of the source code.
        /// </summary>
        /// <param name="character">The character to test, or an empty string.</param>
        /// <returns><c>true</c> if the given character can appear before a suggestion.</returns>
        /// <seealso cref="LSLAutoCompleteParseOptions.BlockOnInvalidPrefix"/>
        /// <seealso cref="ILSLAutoCompleteParserState.InvalidPrefix"/>
        /// <exception cref="ArgumentNullException"><paramref name="character"/> is <see langword="null" />.</exception>
        public override bool IsValidSuggestionPrefix(string character)
        {
            if (character == null) throw new ArgumentNullException("character");

            return _validSuggestionPrefixes.Contains(character);
        }


        /// <summary>
        /// Determine if a given character can come immediately after an autocomplete suggestion.  An empty string represents the end of the source code.
        /// </summary>
        /// <param name="character">The character to test, or an empty string.</param>
        /// <returns><c>true</c> if the given character can appear after a suggestion.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="character"/> is <see langword="null" />.</exception>
        public override bool IsValidSuggestionSuffix(string character)
        {
            if (character == null) throw new ArgumentNullException("character");

            return _validSuggestionSuffixes.Contains(character);
        }


        /// <summary>
        /// Returns a hashset copy of all valid suggestion suffixes.  see: <see cref="IsValidSuggestionSuffix"/>
        /// </summary>
        public static HashSet<string> ValidSuggestionSuffixes
        {
            get { return new HashSet<string>(_validSuggestionSuffixes); }
        }


        /// <summary>
        /// Returns a hashset copy of all valid suggestion prefixes.  see: <see cref="IsValidSuggestionPrefix"/>
        /// </summary>
        public static HashSet<string> ValidSuggestionPrefixes
        {
            get { return new HashSet<string>(_validSuggestionPrefixes); }
        }
    }
}