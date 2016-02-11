#region FileInfo
// 
// File: CSharpIDValidator.cs
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

using System.CodeDom.Compiler;
using System.Text.RegularExpressions;

namespace LibLSLCC.CSharp
{
    static class CSharpIDValidator
    {
        private const string TrailingCharacter =
            @"\u0041-\u005A\u00C0-\u00DE\u0061-\u007A\u01C5\u01C8\u01CB\u01F2\u02B0-\u02EE\u01BB\u01C0\u01C1\u01C2\u01C3\u0294\u16EE\u16EF\u16F0\u2160\u2161\u2162\u2163\u2164\u2165\u2166\u2167\u2168\u2169\u216A\u216B\u216C\u216D\u216E\u216F";

        private const string StartingCharacter = "_" + TrailingCharacter;


        private static readonly Regex StartingCharacterGroup = new Regex("[" + StartingCharacter + "]");

        private static readonly Regex TrailingCharacterGroup = new Regex("[" + TrailingCharacter + "]");

        private static readonly Regex Id = new Regex("^[" + StartingCharacter + "][" + TrailingCharacter + "]*$");

        public static bool IsStartingCharacter(char c)
        {
            return StartingCharacterGroup.IsMatch("" + c);
        }

        public static bool IsTrailingCharacter(char c)
        {
            return TrailingCharacterGroup.IsMatch("" + c);
        }

        private static readonly object CompilerLock = new object();



        public static bool IsValidIdentifier(string str)
        {
            using (var compiler = CodeDomProvider.CreateProvider("CSharp"))
            {
                return compiler.IsValidIdentifier(str);
            }
        }
    }
}
