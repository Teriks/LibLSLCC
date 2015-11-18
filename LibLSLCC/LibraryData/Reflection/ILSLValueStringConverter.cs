#region FileInfo
// 
// File: ILSLValueStringConverter.cs
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

using System.Reflection;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Interface for converting property/field values into strings assignable to <see cref="LSLLibraryConstantSignature.ValueString"/>.
    /// This interface is for types that are assigned to the attribute property <see cref="LSLConstantAttribute.ValueStringConverter"/> using the <c>typeof</c> operator.
    /// </summary>
    public interface ILSLValueStringConverter
    {
        /// <summary>
        /// Convert the value taken from a property with the <see cref="LSLConstantAttribute"/> into
        /// something that is valid to assign to <see cref="LSLLibraryConstantSignature.ValueString"/> given the specified
        /// <see cref="LSLType"/> that is to be assigned to <see cref="LSLLibraryConstantSignature.Type"/>.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> of the property the value was taken from.</param>
        /// <param name="constantType">The <see cref="LSLType"/> being assigned to <see cref="LSLLibraryConstantSignature.Type"/>.</param>
        /// <param name="fieldValue">The value taking from the property or field with an <see cref="LSLConstantAttribute"/>.</param>
        /// <param name="valueString">
        /// The string to assign to <see cref="LSLLibraryConstantSignature.ValueString"/>.
        /// this should be a string that <see cref="LSLLibraryConstantSignature"/> is able to parse for the given <see cref="LSLType"/>.
        /// You should not assign <c>null</c> to <paramref name="valueString"/> if you intend to return <c>true</c>, this is invalid and the serializer will throw an exception.
        /// </param>
        /// <returns>
        /// True if the conversion succeeded, false if it did not.
        /// </returns>
        bool ConvertProperty(PropertyInfo propertyInfo, LSLType constantType, object fieldValue, out string valueString);


        /// <summary>
        /// Convert the value taken from a field with the <see cref="LSLConstantAttribute"/> into
        /// something that is valid to assign to <see cref="LSLLibraryConstantSignature.ValueString"/> given the specified
        /// <see cref="LSLType"/> that is to be assigned to <see cref="LSLLibraryConstantSignature.Type"/>.
        /// </summary>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/> of the field the value was taken from.</param>
        /// <param name="constantType">The <see cref="LSLType"/> being assigned to <see cref="LSLLibraryConstantSignature.Type"/>.</param>
        /// <param name="fieldValue">The value taking from the property or field with an <see cref="LSLConstantAttribute"/>.</param>
        /// <param name="valueString">
        /// The string to assign to <see cref="LSLLibraryConstantSignature.ValueString"/>.
        /// this should be a string that <see cref="LSLLibraryConstantSignature"/> is able to parse for the given <see cref="LSLType"/>.
        /// You should not assign <c>null</c> to <paramref name="valueString"/> if you intend to return <c>true</c>, this is invalid and the serializer will throw an exception.
        /// </param>
        /// <returns>
        /// True if the conversion succeeded, false if it did not.
        /// </returns>
        bool ConvertField(FieldInfo fieldInfo, LSLType constantType, object fieldValue, out string valueString);
    }
}