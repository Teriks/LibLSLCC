#region FileInfo

// 
// File: CloningDefaultValueFactory.cs
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
using System.Reflection;

#endregion

namespace LibLSLCC.Settings
{
    /// <summary>
    ///     An implementation of <see cref="IDefaultSettingsValueFactory" /> that gets its
    ///     default values by cloning from a static instance of the provided generic type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CloningDefaultValueFactory<T> : IDefaultSettingsValueFactory where T : new()
    {
        private static readonly T Default = new T();
        // ReSharper disable once StaticMemberInGenericType
        private static readonly DefaultCloner Cloner = new DefaultCloner();


        /// <summary>
        ///     The default implementation simply checks the property value for <c>null</c>.
        /// </summary>
        /// <param name="member">The member of the settings field/property being checked for a necessary reset.</param>
        /// <param name="objectInstance">The object instance the settings field/property belongs to.</param>
        /// <param name="settingValue">The value of the settings field/property being checked.</param>
        /// <returns>Whether or not <paramref name="settingValue"/> equals <c>null</c>.</returns>
        public virtual bool CheckForNecessaryResets(MemberInfo member, object objectInstance, object settingValue)
        {
            return settingValue == null;
        }


        /// <summary>
        ///     Returns the default value for a given settings member (field/property).
        /// </summary>
        /// <param name="member">The <see cref="FieldInfo" /> or <see cref="PropertyInfo" /> of the settings field/property.</param>
        /// <param name="objectInstance">The settings object instance.</param>
        /// <returns>The default value for the field/property.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="member" /> is not a <see cref="FieldInfo" /> or
        ///     <see cref="PropertyInfo" /> object.
        /// </exception>
        public virtual object GetDefaultValue(MemberInfo member, object objectInstance)
        {
            var prop = member as PropertyInfo;
            var field = member as FieldInfo;

            if (prop == null && field == null)
            {
                throw new ArgumentException("member must be a PropertyInfo or FieldInfo object.", "member");
            }

            var defaultValue = prop != null ? prop.GetValue(Default, null) : field.GetValue(Default);

            var clone = Cloner.Clone(defaultValue);

            return clone ?? defaultValue;
        }
    }
}