#region FileInfo
// 
// File: LSLCommentStringSkipper.cs
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
namespace LibLSLCC.AutoComplete
{

    /// <summary>
    /// A class to help determine if an offset into LSL source code is inside of a comment or string literal.
    /// </summary>
    public class LSLCommentStringSkipper
    {
        private int _lastStringStart;


        /// <summary>
        /// Construct an LSLCommentStringSkipper that will be ready to parse upon construction.
        /// </summary>
        public LSLCommentStringSkipper()
        {
        }

        /// <summary>
        /// Construct an LSLCommentStringSkipper and automatically parse the provided text up to a given offset.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        /// <param name="parseUpTo">The offset to be parsed up to.</param>
        public LSLCommentStringSkipper(string text, int parseUpTo)
        {
            ParseUpTo(text, parseUpTo);
        }

        /// <summary>
        /// A rough estimation of scope level at the current offset.  This counter is incremented every
        /// time an opening brace is found and decremented whenever a closing brace is found.
        /// 
        /// If there are missing/mismatched braces then its value will not be accurate.
        /// </summary>
        public int ScopeLevel { get; set; }


        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// True if the offset that was parsed up to is inside a block style comment.
        /// </summary>
        public bool InBlockComment { get; private set; }


        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// True if the offset that was parsed up to is inside a line style comment.
        /// </summary>
        public bool InLineComment { get; set; }


        /// <summary>
        /// True if the offset that was parsed up to is inside a string literal.
        /// </summary>
        public bool InString { get; private set; }


        /// <summary>
        /// True if the offset that was parsed up to is inside any sort of comment.
        /// Effectively equal to: (InLineComment || InBlockComment)
        /// </summary>
        public bool InComment
        {
            get { return InLineComment || InBlockComment; }
        }

        /// <summary>
        /// Reset the LSLCommentStringSkipper so it can parse again.
        /// </summary>
        public void Reset()
        {
            InBlockComment = false;
            InLineComment = false;
            InString = false;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Parse the given text up to a given offset.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="offset">The offset into the text to parse up to.</param>
        public void ParseUpTo(string text, int offset)
        {
            for (var i = 0; i < offset; i++)
            {
                FeedChar(text, i, offset);
            }
        }

        /// <summary>
        /// Feeds a single character from the given text at a certain index into the parser.
        /// </summary>
        /// <param name="text">The text the character is to be pulled from.</param>
        /// <param name="i">The index of the character in the text to feed to the parser.</param>
        /// <param name="offset">The offset at which the parser is set to parse up to.</param>
        private void FeedChar(string text, int i, int offset)
        {
            var lookAhead = i + 1;
            var lookBehind = i - 1;
            var lookBehindTwo = i - 2;
            var offsetMOne = offset - 1;

            if (!InLineComment && !InString)
            {
                if (text[i] == '/' && i < offsetMOne && text[lookAhead] == '*')
                {
                    InBlockComment = true;
                }
                else if (InBlockComment && (lookBehindTwo > 0) && text[lookBehindTwo] == '*' && text[lookBehind] == '/')
                {
                    InBlockComment = false;
                }
            }
            if (!InBlockComment && !InString)
            {
                if (text[i] == '/' && i < offsetMOne && text[lookAhead] == '/')
                {
                    InLineComment = true;
                }
                else if (InLineComment && lookBehind > 0 && text[lookBehind] == '\n')
                {
                    InLineComment = false;
                }
            }
            if (!InLineComment && !InBlockComment)
            {
                if (!InString && text[i] == '"')
                {
                    _lastStringStart = i;
                    InString = true;
                }
                else if (InString && lookBehind > 0 && text[lookBehind] == '"' && lookBehind != _lastStringStart)
                {
                    if ((lookBehind - _lastStringStart) > 2)
                    {
                        var c = 0;
                        var s = i - 2;

                        for (var o = s; text[o] == '\\'; o--, c++)
                        {
                        }

                        if ((c%2) == 0)
                        {
                            InString = false;
                        }
                    }
                    else
                    {
                        InString = false;
                    }
                }
            }


            if (!InComment && !InString)
            {
                if (text[i] == '{') ScopeLevel++;
                if (text[i] == '}') ScopeLevel--;
            }
        }
    }
}