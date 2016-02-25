#region FileInfo

// 
// File: LSLConstantAttribute.cs
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
using LibLSLCC.CodeValidator.Enums;

#endregion

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    ///     Optional attribute for exposing properties and fields to <see cref="LSLLibraryDataReflectionSerializer" /> without
    ///     having to map types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class LSLConstantAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLConstantAttribute" /> class.
        /// </summary>
        /// <param name="type">The <see cref="LSLType" /> of the constant.</param>
        public LSLConstantAttribute(LSLType type)
        {
            Type = type;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLConstantAttribute" /> class.
        /// </summary>
        public LSLConstantAttribute()
        {
        }


        /// <summary>
        ///     Gets the <see cref="LSLType" /> of the constant as specified by the attribute.
        /// </summary>
        /// <value>
        ///     The <see cref="LSLType" /> of the constant.
        /// </value>
        public LSLType Type { get; private set; }

        /// <summary>
        ///     Gets a value indicating what serialized <see cref="LSLLibraryConstantSignature" /> objects should have
        ///     <see cref="LSLLibraryConstantSignature.Expand" /> set to.
        /// </summary>
        /// <value>
        ///     The value to set <see cref="LSLLibraryConstantSignature.Expand" /> to.
        /// </value>
        public bool Expand { get; set; }

        /// <summary>
        ///     Gets a value indicating what serialized <see cref="LSLLibraryConstantSignature" /> objects should have
        ///     <see cref="LSLLibraryConstantSignature.Deprecated" /> set to.
        /// </summary>
        /// <value>
        ///     The value to set <see cref="LSLLibraryConstantSignature.Deprecated" /> to.
        /// </value>
        public bool Deprecated { get; set; }

        /// <summary>
        ///     Gets or sets the value string to be assigned to <see cref="LSLLibraryConstantSignature.ValueString" /> if no
        ///     <see cref="ValueStringConverter" /> type is present.
        ///     You cannot have both an explicit <see cref="LSLLibraryConstantSignature.ValueString" /> and
        ///     <see cref="ValueStringConverter" /> at the same time, this will cause an exception from the serializer.
        /// </summary>
        /// <value>
        ///     The value string to be assigned to <see cref="LSLLibraryConstantSignature.ValueString" />.
        /// </value>
        public string ValueString { get; set; }

        /// <summary>
        ///     <para>
        ///         Gets or sets the value string converter <see cref="Type" />, this type should derive from
        ///         <see cref="ILSLValueStringConverter" /> or you will get exceptions from the serializer.
        ///         The value string converter is responsible for converting field/property values into something that is parsable
        ///         by <see cref="LSLLibraryConstantSignature.ValueString" />.
        ///         You cannot have both an explicit <see cref="LSLLibraryConstantSignature.ValueString" /> and
        ///         <see cref="ValueStringConverter" /> at the same time, this will cause an exception from the serializer.
        ///         This property is only optional if the class is using a defined
        ///         <see cref="LSLLibraryDataSerializableAttribute.ValueStringConverter" /> or
        ///         <see cref="LSLLibraryDataReflectionSerializer.ValueStringConverter" /> is set.
        ///         Setting this property will override both
        ///         <see cref="LSLLibraryDataSerializableAttribute.ValueStringConverter" /> and
        ///         <see cref="LSLLibraryDataReflectionSerializer.ValueStringConverter" />.
        ///     </para>
        /// </summary>
        /// <value>
        ///     The value string converter <see cref="Type" /> which sould derive from <see cref="ILSLValueStringConverter" />.
        /// </value>
        public Type ValueStringConverter { get; set; }

        /// <summary>
        ///     <para>
        ///         Gets or sets the <see cref="ILSLConstantTypeConverter" /> that is used in the case that no <see cref="Type" />
        ///         is given, it takes the property type and converts it to an <see cref="LSLType" />.
        ///         This property is only optional if the class is using a defined
        ///         <see cref="LSLLibraryDataSerializableAttribute.ConstantTypeConverter" /> or
        ///         <see cref="LSLLibraryDataReflectionSerializer.ConstantTypeConverter" /> is set.
        ///         Setting this property will override both
        ///         <see cref="LSLLibraryDataSerializableAttribute.ConstantTypeConverter" /> and
        ///         <see cref="LSLLibraryDataReflectionSerializer.ConstantTypeConverter" />.
        ///     </para>
        /// </summary>
        /// <value>
        ///     The type converter which should derive from <see cref="ILSLConstantTypeConverter" />.
        /// </value>
        public Type TypeConverter { get; set; }
    }
}