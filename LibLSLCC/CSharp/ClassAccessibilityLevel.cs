#region FileInfo
// 
// File: ClassAccessibilityLevel.cs
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

namespace LibLSLCC.CSharp
{
    /// <summary>
    /// Represents the accessibility level of a CSharp class.
    /// </summary>
    public enum ClassAccessibilityLevel
    {
        Default,
        Internal,
        Public,
    }

    /// <summary>
    /// Extension methods for <see cref="ClassAccessibilityLevel"/>.
    /// </summary>
    public static class ClassAccessibilityLevelExtensions
    {
        /// <summary>
        /// Converts <see cref="ClassAccessibilityLevel"/> to its corresponding CSharp keyword.
        /// </summary>
        /// <param name="value">The <see cref="ClassAccessibilityLevel"/> to convert.</param>
        /// <param name="addTrailingSpace">if set to <c>true</c>, add a trailing space to the converted string when its not empty.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">If the given enum value is not convertible to a string (will not occur).</exception>
        public static string ToCSharpKeyword(this ClassAccessibilityLevel value, bool addTrailingSpace = false)
        {
            string spacer = addTrailingSpace ? " " : "";
            switch (value)
            {
                case ClassAccessibilityLevel.Default:
                    return "";
                case ClassAccessibilityLevel.Internal:
                    return "internal" + spacer;
                case ClassAccessibilityLevel.Public:
                    return "public" + spacer;
                default:
                    throw new ArgumentOutOfRangeException("value", value, null);
            }
        }
    }
}