#region FileInfo
// 
// File: LSLFormatTools.cs
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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace LibLSLCC.Utility
{
    /// <summary>
    /// Various utilities for formatting string that are useful for generating/dealing with
    /// indented code and escaped strings.
    /// </summary>
    public static class LSLFormatTools
    {
        /// <summary>
        ///     Gets the number of spaces required to match the length of the whitespace leading up to the first non-whitespace
        ///     character in a string (new line is not considered whitespace here).
        /// </summary>
        /// <param name="str">The string to consider</param>
        /// <param name="tabSize">The size of a tab character in spaces</param>
        /// <returns>
        ///     The number of space characters required to match the length of all the whitespace characters at the end of the
        ///     string (except newlines)
        /// </returns>
        public static int GetStringSpacesIndented(this string str, int tabSize = 4)
        {
            var columns = 0;

            foreach (var t in str)
            {
                if (char.IsWhiteSpace(t))
                {
                    if (t == '\t')
                    {
                        columns += 4;
                    }
                    else if (t == ' ')
                    {
                        columns++;
                    }
                }
                else
                {
                    break;
                }
            }
            return columns;
        }

        /// <summary>
        ///     Gets the number of spaces required to exactly match the length of a given string up to the first new line
        /// </summary>
        /// <param name="str">Input string to get the length in spaces of</param>
        /// <param name="tabSize">Tab size in spaces, defaults to 4</param>
        /// <returns>Number of spaces required to match the length of the string</returns>
        public static int GetStringSpacesEquivalent(this string str, int tabSize = 4)
        {
            if (str.Length == 0) return 0;

            var columns = 0;

            for (var index = 0; index < str.Length; index++)
            {
                var t = str[index];
                if (char.IsWhiteSpace(t))
                {
                    if (t == '\t')
                    {
                        columns += tabSize;
                    }
                    else if (t == ' ')
                    {
                        columns++;
                    }
                }
                else if (char.IsDigit(t) || char.IsLetter(t) || char.IsSymbol(t) || char.IsPunctuation(t))
                {
                    columns += 1;
                }
                else if (index + 1 < str.Length && char.IsHighSurrogate(t) && char.IsLowSurrogate(str[index + 1]))
                {
                    columns += 1;
                    index++;
                }
                else if (t == '\n')
                {
                    break;
                }
            }
            return columns;
        }

        /// <summary>
        ///     Creates a spacer string using tabs up until spaces are required for alignment.
        ///     Strings less than tabSize end up being only spaces.
        /// </summary>
        /// <param name="spaces">The number of spaces the spacer string should be equivalent to</param>
        /// <param name="tabSize">The size of a tab character in spaces, default value is 4</param>
        /// <returns>
        ///     A string consisting of leading tabs and possibly trailing spaces that is equivalent in length
        ///     to the number of spaces provided in the spaces parameter
        /// </returns>
        public static string CreateTabCorrectSpaceString(int spaces, int tabSize = 4)
        {
            var space = "";
            var actual = 0;
            for (var i = 0; i < (spaces/tabSize); i++)
            {
                space += '\t';
                actual += tabSize;
            }

            while (actual < spaces)
            {
                space += ' ';
                actual++;
            }


            return space;
        }


        /// <summary>
        /// If a string has control codes in it, this will return a string with those control codes 
        /// replaced with their symbolic representation, IE: \n \t ect..
        /// 
        /// Supports every escape code supported by C# itself
        /// </summary>
        /// <param name="str">String to replace control codes in.</param>
        /// <returns>String with control codes replaced with symbolic representation.</returns>
        public static string ShowControlCodeEscapes(string str)
        {

            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(str), writer, new CodeGeneratorOptions { IndentString = "\t" });
                    var literal = writer.ToString();
                    literal = literal.Replace(string.Format("\" +{0}\t\"", Environment.NewLine), "");
                    return literal.Trim('"');
                }
            }
        }

        /// <summary>
        /// Create a repeating string by repeating the content string a number of times.
        /// </summary>
        /// <param name="repeats">The number of times the 'content' string should repeat.</param>
        /// <param name="content">The content string to repeat.</param>
        /// <returns></returns>
        public static string CreateRepeatingString(int repeats, string content)
        {
            var r = "";
            for (var i = 0; i < repeats; i++) r += content;
            return r;
        }

        /// <summary>
        ///     Generate a string with N number of spaces in it
        /// </summary>
        /// <param name="spaces">Number of spaces</param>
        /// <returns>A string containing 'spaces' number of spaces</returns>
        public static string CreateSpacesString(int spaces)
        {
            return CreateRepeatingString(spaces, " ");
        }

        /// <summary>
        ///     Generate a string with N number of tabs in it
        /// </summary>
        /// <param name="tabs">Number of tabs</param>
        /// <returns>A string containing 'tabs' number of tabs</returns>
        public static string CreateTabsString(int tabs)
        {
            return CreateRepeatingString(tabs, "\t");
        }

        /// <summary>
        ///     Generate a string with N number of newlines in it
        /// </summary>
        /// <param name="newLines">Number of newlines</param>
        /// <returns>A string containing 'newLines' number of newlines</returns>
        public static string CreateNewLinesString(int newLines)
        {
            return CreateRepeatingString(newLines, "\n");
        }
    }
}