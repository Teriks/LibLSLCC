#region FileInfo
// 
// File: MemberClonerAttribute.cs
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

namespace LibLSLCC.Settings
{
    /// <summary>
    /// An attribute to specify a specific <see cref="ICloner"/> implementation for a field/property in an object derived from <see cref="SettingsBaseClass{TSetting}"/>.
    /// </summary>
    /// <seealso cref="SettingsBaseClass{TSetting}"/>
    /// <seealso cref="SettingsBaseClass{TSetting}.Clone"/>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class MemberClonerAttribute : Attribute
    {
        /// <summary>
        /// The <see cref="Type"/> which implements <see cref="ICloner"/>
        /// </summary>
        public Type ClonerType { get; private set; }


        /// <summary>
        /// Construct an <see cref="MemberClonerAttribute"/> with a given <see cref="Type"/>.
        /// The <see cref="Type"/> should implement <see cref="ICloner"/>.
        /// </summary>
        /// <param name="clonerType">The <see cref="Type"/> which implements <see cref="ICloner"/> and will preform the clone operation for a given field/property.</param>
        public MemberClonerAttribute(Type clonerType)
        {
            ClonerType = clonerType;
        }

    }
}
