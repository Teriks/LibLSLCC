#region FileInfo
// 
// File: ILSLParamTypeConverter.cs
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
using System.Reflection;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Interface for converting .NET runtime <see cref="ParameterInfo"/>'s into their equivalent <see cref="LSLType"/>.
    /// This is used to convert the parameter types encountered in reflected .NET methods.
    /// This interface is used with <see cref="LSLLibraryDataReflectionSerializer"/> and the library data attributes.
    /// </summary>
    /// <seealso cref="LSLLibraryDataSerializableAttribute.ConstantTypeConverter"/>
    /// <seealso cref="LSLLibraryDataSerializableAttribute.ReturnTypeConverter"/>
    /// <seealso cref="LSLLibraryDataSerializableAttribute.ParamTypeConverter"/>
    /// <seealso cref="LSLConstantAttribute.TypeConverter"/>
    /// <seealso cref="LSLFunctionAttribute.ReturnTypeConverter"/>
    /// <seealso cref="LSLFunctionAttribute.ParamTypeConverter"/>
    public interface ILSLParamTypeConverter
    {
        /// <summary>
        /// Converts the specified <see cref="ParameterInfo"/> into its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="parameterInfo">Runtime <see cref="ParameterInfo"/> for converting to an <see cref="LSLType"/>.</param>
        /// <param name="basicType">The basic type of the parameter, this will be the parameter arrays base type if the parameter is variadic.</param>
        /// <param name="outType">Resulting <see cref="LSLType"/> from the conversion.</param>
        /// <returns><c>true</c> if the conversion succeeded, <c>false</c> if it failed.</returns>
        bool ConvertParameter(ParameterInfo parameterInfo, Type basicType, out LSLType outType);
    }
}