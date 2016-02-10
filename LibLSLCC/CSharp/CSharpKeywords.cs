#region FileInfo
// 
// File: CSharpKeywords.cs
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
using LibLSLCC.Collections;

namespace LibLSLCC.CSharp
{
    public static class CSharpKeywords
    {
        public static readonly IReadOnlyHashMap<string, Type> BuiltInTypeMap = new HashMap<string, Type>()
        {
            {"bool", typeof (bool)},
            {"byte", typeof (byte)},
            {"sbyte", typeof (sbyte)},
            {"char", typeof (char)},
            {"decimal", typeof (decimal)},
            {"double", typeof (double)},
            {"float", typeof (float)},
            {"int", typeof (int)},
            {"uint", typeof (uint)},
            {"long", typeof (long)},
            {"ulong", typeof (ulong)},
            {"object", typeof (object)},
            {"short", typeof (short)},
            {"ushort", typeof (ushort)},
            {"string", typeof (string)}
        };



        /// <summary>
        /// Converts a CSharp type alias such as 'int', 'float' or object (etc..) to its corresponding <see cref="Type"/>.
        /// </summary>
        /// <param name="keywordTypeName">The keyword type alias to convert to an actual <see cref="Type"/>.</param>
        /// <returns>The type if there is a corresponding <see cref="Type"/>, otherwise <c>null</c>.</returns>
        public static Type KeywordTypetoType(string keywordTypeName)
        {
            Type t;
            if (BuiltInTypeMap.TryGetValue(keywordTypeName, out t))
            {
                return t;
            }
            return null;
        }


        /// <summary>
        /// Determines if a string is a built in type alias reference such as int.
        /// </summary>
        /// <returns><c>true</c> if the given string contains a built in type alias name; otherwise <c>false</c>.</returns>
        public static bool IsTypeAliasKeyword(string keywordTypeName)
        {
            return BuiltInTypeMap.ContainsKey(keywordTypeName);
        }



        public static readonly IReadOnlyHashedSet<string> NonContextualKeywordSet = new HashedSet<string>()
        {
            "abstract",
            "as",
            "base",
            "bool",
            "break",
            "byte",
            "case",
            "catch",
            "char",
            "checked",
            "class",
            "const",
            "continue",
            "decimal",
            "default",
            "delegate",
            "do",
            "double",
            "else",
            "enum",
            "event",
            "explicit",
            "extern",
            "false",
            "finally",
            "fixed",
            "float",
            "for",
            "foreach",
            "goto",
            "if",
            "implicit",
            "in",
            "int",
            "interface",
            "internal",
            "is",
            "lock",
            "long",
            "namespace",
            "new",
            "null",
            "object",
            "operator",
            "out",
            "override",
            "params",
            "private",
            "protected",
            "public",
            "readonly",
            "ref",
            "return",
            "sbyte",
            "sealed",
            "short",
            "sizeof",
            "stackalloc",
            "static",
            "string",
            "struct",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "uint",
            "ulong",
            "unchecked",
            "unsafe",
            "ushort",
            "using",
            "virtual",
            "void",
            "volatile",
            "while"
        };

        public static bool IsNonContextualKeyword(string str)
        {
            return NonContextualKeywordSet.Contains(str);
        }


    }
}