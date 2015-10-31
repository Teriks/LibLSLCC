#region FileInfo
// 
// File: LSLFunctionAttribute.cs
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
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Optional Attribute for explicitly exposing methods to <see cref="LSLLibraryDataReflectionSerializer"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LSLFunctionAttribute : Attribute
    {
        public LSLType ReturnType { get; private set; }

        /// <summary>
        /// <para>
        /// Gets or sets the type converter that is used in the case that no <see cref="Type"/> is given for <see cref="ReturnType"/>, it takes the method return type and converts it to an <see cref="LSLType"/>.
        /// You cannot set both an explicit <see cref="ReturnType"/> and a <see cref="ReturnTypeConverter"/> or the serializer will throw an exception.
        /// This property is only optional if the class is using a defined <see cref="LSLLibraryDataSerializableAttribute.ReturnTypeConverter"/> or <see cref="LSLLibraryDataReflectionSerializer.ReturnTypeConverter"/> is set.
        /// Setting this property will override both <see cref="LSLLibraryDataSerializableAttribute.ReturnTypeConverter"/> and <see cref="LSLLibraryDataReflectionSerializer.ReturnTypeConverter"/>.
        /// </para>
        /// </summary>
        /// <value>
        /// The type converter which should derive from <see cref="ILSLTypeConverter"/>.
        /// </value>
        public Type ReturnTypeConverter { get; set; }


        /// <summary>
        /// <para>
        /// Gets or sets the type converter that is used in the case that no <see cref="LSLParamAttribute"/> is applied to a given method parameter, it takes the parameter type and converts it to an <see cref="LSLType"/>.
        /// Parameters that do not have an <see cref="LSLParamAttribute"/> applied to them will use this converter to convert the .NET parameter type into an <see cref="LSLType"/>.
        /// This property is only optional if the class is using a defined <see cref="LSLLibraryDataSerializableAttribute.ParamTypeConverter"/> or <see cref="LSLLibraryDataReflectionSerializer.ParamTypeConverter"/> is set.
        /// Setting this property will override both <see cref="LSLLibraryDataSerializableAttribute.ParamTypeConverter"/> and <see cref="LSLLibraryDataReflectionSerializer.ParamTypeConverter"/>.
        /// </para>
        /// </summary>
        /// <value>
        /// The type converter which should derive from <see cref="ILSLTypeConverter"/>.
        /// </value>
        public Type ParamTypeConverter { get; set; }

        /// <summary>
        /// Initializes the attribute without an explicit return type.
        /// </summary>
        public LSLFunctionAttribute()
        {
            
        }


        /// <summary>
        /// Gets a value indicating what serialized <see cref="LSLLibraryFunctionSignature"/> objects should have <see cref="LSLLibraryFunctionSignature.Deprecated"/> set to.
        /// </summary>
        /// <value>
        /// The value to set <see cref="LSLLibraryFunctionSignature.Deprecated"/> to.
        /// </value>
        public bool Deprecated { get;  set; }


        /// <summary>
        /// Gets a value indicating what serialized <see cref="LSLLibraryFunctionSignature"/> objects should have <see cref="LSLLibraryFunctionSignature.ModInvoke"/> set to.
        /// </summary>
        /// <value>
        /// The value to set <see cref="LSLLibraryFunctionSignature.ModInvoke"/> to.
        /// </value>
        public bool ModInvoke { get;  set; }


        /// <summary>
        /// Initializes the attribute with an explicit return type.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        public LSLFunctionAttribute(LSLType returnType)
        {
            ReturnType = returnType;
        }
    }
}