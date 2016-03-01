#region FileInfo

// 
// File: SettingsBaseClassTools.cs
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

using System;

#endregion

namespace LibLSLCC.Settings
{
    /// <summary>
    ///     Misc tools for dealing with classes that derive from <see cref="SettingsBaseClass{T}" />
    /// </summary>
    public static class SettingsBaseClassTools
    {
        /// <summary>
        ///     Detects if a type derives from the generic <see cref="SettingsBaseClass{T}" />
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <param name="baseType">
        ///     Outputs the full generic type of the <see cref="SettingsBaseClass{T}" /> if the class derives
        ///     from it, otherwise <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the class derives from a form of <see cref="SettingsBaseClass{T}" /></returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public static bool HasSettingsBase(Type type, out Type baseType)
        {
            if (type == null) throw new ArgumentNullException("type");

            var baseSearch = type.BaseType;

            bool isSettingsBase = false;

            while (baseSearch != null)
            {
                var genericTickIndex = baseSearch.FullName.IndexOf("`", StringComparison.Ordinal);

                if (genericTickIndex == -1) break;

                var nonGenericName = baseSearch.FullName.Substring(0, genericTickIndex);

                if (nonGenericName != "LibLSLCC.Settings.SettingsBaseClass")
                {
                    baseSearch = baseSearch.BaseType;
                    continue;
                }


                isSettingsBase = true;
                break;
            }

            baseType = isSettingsBase ? baseSearch : null;
            return isSettingsBase;
        }


        /// <summary>
        ///     Detects if a type derives from the generic <see cref="SettingsBaseClass{T}" />
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns><c>true</c> if the class derives from a form of <see cref="SettingsBaseClass{T}" /></returns>
        public static bool HasSettingsBase(Type type)
        {
            Type discard;
            return HasSettingsBase(type, out discard);
        }
    }
}