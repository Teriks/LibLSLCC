#region FileInfo
// 
// File: LSLLibraryDataSerializableAttribute.cs
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
using System.Linq;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Optional Attribute for specifying class wide type converters and the constant ValueStringConverter that overrides the one's in <see cref="LSLLibraryDataReflectionSerializer"/>.
    /// This is to be used on classes that contain methods and fields/properties that are to be reflected as library data objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LSLLibraryDataSerializableAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the <see cref="ILSLReturnTypeConverter"/> used to convert the return types of methods in a given class to <see cref="LSLType"/>'s.
        /// The return type converter defined in the class attribute can be overridden per method by <see cref="LSLFunctionAttribute.ReturnTypeConverter"/>.
        /// This property will override <see cref="LSLLibraryDataReflectionSerializer.ReturnTypeConverter"/> if it is set.
        /// </summary>
        /// <value>
        /// The return type converter, should derive from <see cref="ILSLReturnTypeConverter"/>.
        /// </value>
        public Type ReturnTypeConverter { get; set; }


        /// <summary>
        /// Gets or sets the <see cref="ILSLParamTypeConverter"/> used to convert the parameter types of methods in a given class to <see cref="LSLType"/>'s.
        /// The parameter type converter defined in the class attribute can be overridden per method by <see cref="LSLFunctionAttribute.ParamTypeConverter"/>.
        /// This property will override <see cref="LSLLibraryDataReflectionSerializer.ParamTypeConverter"/> if it is set.
        /// </summary>
        /// <value>
        /// The parameter type converter, should derive from <see cref="ILSLParamTypeConverter"/>.
        /// </value>
        public Type ParamTypeConverter { get; set; }


        /// <summary>
        /// Gets or sets the <see cref="ILSLConstantTypeConverter"/> used to convert the types of fields/properties in a given class to <see cref="LSLType"/>'s.
        /// The constant type converter defined in the class attribute can be overridden per method by <see cref="LSLConstantAttribute.TypeConverter"/>.
        /// This property will override <see cref="LSLLibraryDataReflectionSerializer.ConstantTypeConverter"/> if it is set.
        /// </summary>
        /// <value>
        /// The parameter type converter, should derive from <see cref="ILSLConstantTypeConverter"/>.
        /// </value>
        public Type ConstantTypeConverter { get; set; }


        /// <summary>
        /// Gets or sets the <see cref="ILSLValueStringConverter"/> used to convert the values taken from fields/properties in given class to strings that can be parsed by <see cref="LSLLibraryConstantSignature.ValueString"/>.
        /// The value string converter defined in the class attribute can be overridden per field/property by <see cref="LSLConstantAttribute.ValueStringConverter"/>.
        /// This property will override <see cref="LSLLibraryDataReflectionSerializer.ValueStringConverter"/> if it is set.
        /// </summary>
        /// <value>
        /// The value string converter, should derive from <see cref="ILSLValueStringConverter"/>.
        /// </value>
        public Type ValueStringConverter { get; set; }



        private static T _GetTypeConverter<T>(Type fromClass, string name) where T : class
        {
            if (!IsDefined(fromClass, typeof (LSLLibraryDataSerializableAttribute))) return null;

            var attr =
                fromClass.GetCustomAttributesData()
                    .First(x => x.Constructor.DeclaringType == typeof (LSLLibraryDataSerializableAttribute));


            if (attr.NamedArguments.Any(x => x.MemberInfo.Name == name))
            {
                var constantConverterType =
                    (Type)
                        attr.NamedArguments.First(x => x.MemberInfo.Name == name)
                            .TypedValue.Value;

                if (constantConverterType.GetInterfaces().Contains(typeof (T)))
                {
                    return (T) Activator.CreateInstance(constantConverterType);
                }

                throw new LSLLibraryDataAttributeException(
                    string.Format(
                        "Class '{0}' is tagged with an [LSLLibraryDataSerializableAttribute.{1}] type" +
                        "'{2}' that does not derive from {3}.",
                        fromClass.FullName,
                        name,
                        constantConverterType.FullName,
                        typeof(T).Name));
            }

            return null;
        }





        internal static ILSLReturnTypeConverter GetReturnTypeConverter(Type fromClass)
        {
            return _GetTypeConverter<ILSLReturnTypeConverter>(fromClass, "ReturnTypeConverter");
        }

        internal static ILSLParamTypeConverter GetParamTypeConverter(Type fromClass)
        {
            return _GetTypeConverter<ILSLParamTypeConverter>(fromClass, "ParamTypeConverter");
        }

        internal static ILSLConstantTypeConverter GetConstantTypeConverter(Type fromClass)
        {
            return _GetTypeConverter<ILSLConstantTypeConverter>(fromClass, "ConstantTypeConverter");
        }

        internal static ILSLValueStringConverter GetValueStringConverter(Type fromClass)
        {
            return _GetTypeConverter<ILSLValueStringConverter>(fromClass, "ValueStringConverter");
        }
    }
}