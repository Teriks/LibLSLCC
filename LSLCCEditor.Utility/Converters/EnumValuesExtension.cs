﻿#region FileInfo
// 
// File: EnumValuesExtension.cs
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
using System.Windows.Markup;

namespace LSLCCEditor.Utility.Converters
{
    public class EnumValuesExtension : MarkupExtension
    {
        private readonly Type _enumType;


        /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Argument enumType must derive from type Enum.</exception>
        public EnumValuesExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum)
                throw new ArgumentException("Argument enumType must derive from type Enum.");

            _enumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_enumType);
        }
    }

}
