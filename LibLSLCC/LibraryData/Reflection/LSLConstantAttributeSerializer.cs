#region FileInfo
// 
// File: LSLConstantAttributeSerializer.cs
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
using System.Reflection;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{
    internal class LSLConstantAttributeSerializer
    {
        public Info GetInfo(PropertyInfo info)
        {
            return _GetInfo(info);
        }


        public Info GetInfo(FieldInfo info)
        {
            return _GetInfo(info);
        }


        public class Info
        {
            public bool ExplicitTypePresent { get; internal set; }

            public bool ExplicitValueStringPresent { get; internal set; }

            public string ExplicitValueString { get; internal set; }

            public LSLType Type { get; internal set; }

            public ILSLConstantTypeConverter TypeConverterInstance { get; internal set; }

            public ILSLValueStringConverter ValueStringConverterInstance { get; internal set; }

            public bool Expand { get; internal set; }

            public bool Deprecated { get; internal set; }

            public string RetrievedValueString { get; internal set; }
        }


        public ILSLValueStringConverter FallBackValueStringConverter { get; set; }

        public ILSLConstantTypeConverter FallBackTypeConverter { get; set; }

        public object OptionalDeclaringTypeInstance { get; set; }


        private Info _GetInfo(MemberInfo info)
        {
            if (!Attribute.IsDefined(info, typeof (LSLConstantAttribute)))
            {
                return null;
            }


            var propertyInfo = info as PropertyInfo;
            var fieldInfo = info as FieldInfo;

            bool isProperty = propertyInfo != null;


            bool isStatic = isProperty ? propertyInfo.GetGetMethod(true).IsStatic : fieldInfo.IsStatic;



            Type memberType = isProperty ? propertyInfo.PropertyType : fieldInfo.FieldType;




            var attrList =
                info.GetCustomAttributesData()
                    .Where(x => x.Constructor.DeclaringType == typeof (LSLConstantAttribute))
                    .ToList();
            if (attrList.Count == 0) return null;


            var attr = attrList[0];

            var namedArgs = attr.NamedArguments;


            var result = new Info();


            if (attr.ConstructorArguments.Count == 1)
            {
                result.ExplicitTypePresent = true;
                result.Type = (LSLType) attr.ConstructorArguments[0].Value;
            }
            else
            {
                if (namedArgs.Any(x => x.MemberInfo.Name == "Type"))
                {
                    result.ExplicitTypePresent = true;
                    result.Type = (LSLType) namedArgs.First(x => x.MemberInfo.Name == "Type").TypedValue.Value;
                }
            }


            if (namedArgs.Any(x => x.MemberInfo.Name == "Deprecated"))
            {
                result.Deprecated = (bool) namedArgs.First(x => x.MemberInfo.Name == "Deprecated").TypedValue.Value;
            }

            if (namedArgs.Any(x => x.MemberInfo.Name == "Expand"))
            {
                result.Expand = (bool) namedArgs.First(x => x.MemberInfo.Name == "Expand").TypedValue.Value;
            }


            if (namedArgs.Any(x => x.MemberInfo.Name == "ValueString"))
            {
                result.ExplicitValueStringPresent = true;
                result.ExplicitValueString =
                    (string) namedArgs.First(x => x.MemberInfo.Name == "ValueString").TypedValue.Value;
                result.RetrievedValueString = result.ExplicitValueString;
            }


            if (namedArgs.Any(x => x.MemberInfo.Name == "ValueStringConverter"))
            {
                if (result.ExplicitValueStringPresent)
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "[LSLConstantAttribute.ValueStringConverter] cannot be used on property/field '{0}' in class '{1}' because [LSLConstantAttribute.ValueString] is already explicitly set.",
                            info.Name, info.DeclaringType.FullName));
                }


                var valueStringConverter =
                    (Type) namedArgs.First(x => x.MemberInfo.Name == "ValueStringConverter").TypedValue.Value;

                if (valueStringConverter.GetInterfaces().Contains(typeof (ILSLValueStringConverter)))
                {
                    result.ValueStringConverterInstance =
                        Activator.CreateInstance(valueStringConverter) as ILSLValueStringConverter;
                }
                else
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "[LSLConstantAttribute.ValueStringConverter] does not implement ILSLValueStringConverter on property/field '{0}' in class '{1}'.",
                            info.Name, info.DeclaringType.FullName));
                }
            }


            if (namedArgs.Any(x => x.MemberInfo.Name == "TypeConverter"))
            {
                if (result.ExplicitTypePresent)
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "[LSLConstantAttribute.TypeConverter] cannot be used on property/field '{0}' in class '{1}' because [LSLConstantAttribute.Type] is already explicitly set.",
                            info.Name, info.DeclaringType.FullName));
                }

                var typeConverter =
                    (Type) namedArgs.First(x => x.MemberInfo.Name == "TypeConverter").TypedValue.Value;

                if (typeConverter.GetInterfaces().Contains(typeof (ILSLConstantTypeConverter)))
                {
                    result.TypeConverterInstance =
                        Activator.CreateInstance(typeConverter) as ILSLConstantTypeConverter;
                }
                else
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "[LSLConstantAttribute.TypeConverter] does not implement ILSLConstantTypeConverter on property/field '{0}' in class '{1}'.",
                            info.Name, info.DeclaringType.FullName));
                }
            }


            if (result.ExplicitTypePresent && result.ExplicitValueStringPresent)
            {
                return result;
            }


            

            object retrievedMemberValue = null;


            if (propertyInfo != null)
            {
                memberType = propertyInfo.PropertyType;

                if (!propertyInfo.CanRead)
                {
                    if (!result.ExplicitValueStringPresent)
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Property '{0}' in class '{1}' has no get accessor and [LSLConstantAttribute.ValueString] was not explicitly set.",
                                info.Name, info.DeclaringType.FullName));
                    }
                }
            }


            if (isStatic)
            {
                retrievedMemberValue = isProperty ? propertyInfo.GetValue(null, null) : fieldInfo.GetValue(null);

                if (retrievedMemberValue == null)
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "Static {0} '{1}' in class '{2}' is tagged with [LSLConstantAttribute] and returned a null value.", 
                            isProperty ? "Property" : "Field",
                            info.Name, info.DeclaringType.FullName));
                }
            }
            else if (OptionalDeclaringTypeInstance == null)
            {
                if (!result.ExplicitValueStringPresent)
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "Instance {0} '{1}' in class '{2}' has cannot have its value retrieved because no instance of '{1}' was supplied and [LSLConstantAttribute.ValueString] was not explicitly set.",
                            isProperty ? "Property" : "Field",
                            info.Name, info.DeclaringType.FullName));
                }
            }
            else
            {
                if (OptionalDeclaringTypeInstance.GetType() != info.DeclaringType)
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "Instance {0} '{1}' in class '{2}' has cannot have its value retrieved because an instance of '{3}' was supplied to retrieve it from and an instance of '{2}' is required.",
                            isProperty ? "Property" : "Field",
                            info.Name, 
                            info.DeclaringType.FullName, 
                            OptionalDeclaringTypeInstance.GetType().FullName));
                }


                retrievedMemberValue = isProperty ? propertyInfo.GetValue(OptionalDeclaringTypeInstance, null) : fieldInfo.GetValue(OptionalDeclaringTypeInstance);

                if (retrievedMemberValue == null)
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "Instance {0} '{1}' in class '{2}' is tagged with [LSLConstantAttribute] and returned a null value.",
                            isProperty ? "Property" : "Field",
                            info.Name, info.DeclaringType.FullName));
                }
            }
  


            if (!result.ExplicitValueStringPresent && result.ValueStringConverterInstance == null &&
                FallBackValueStringConverter == null)
            {
                throw new LSLLibraryDataAttributeException(
                    string.Format(
                        (isProperty ? "Property" : "Field") +
                        " '{0}' in class '{1}' uses an [LSLContantAttribute] without an [LSLContantAttribute.ValueString] or" +
                        " [LSLContantAttribute.ValueStringConverter], one or the other is required if no fall-back value string converter is defined in the serializer.",
                        info.Name, info.DeclaringType.FullName));
            }

            if (!result.ExplicitTypePresent && result.TypeConverterInstance == null && FallBackTypeConverter == null)
            {
                throw new LSLLibraryDataAttributeException(
                    string.Format(
                        (isProperty ? "Property" : "Field") +
                        " '{0}' in class '{1}' uses an [LSLContantAttribute] without a [LSLContantAttribute.Type] or" +
                        " [LSLContantAttribute.TypeConverter], one or the other is required if no fall-back type converter is defined in the serializer.",
                        info.Name, info.DeclaringType.FullName));
            }


            if (!result.ExplicitTypePresent)
            {
                LSLType lslType;

                var converter = result.TypeConverterInstance ?? FallBackTypeConverter;

                if (isProperty)
                {
                    if (!converter.ConvertProperty(propertyInfo, out lslType))
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Property '{0}' in class '{1}' was declared with the Type '{2}' that was unable to be converted by the selected type converter of Type '{3}'.",
                                info.Name, info.DeclaringType.FullName, memberType.FullName,
                                converter.GetType().FullName));
                    }
                }
                else
                {
                    if (!converter.ConvertField(fieldInfo, out lslType))
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Property '{0}' in class '{1}' was declared with the Type '{2}' that was unable to be converted by the selected type converter of Type '{3}'.",
                                info.Name, info.DeclaringType.FullName, memberType.FullName,
                                converter.GetType().FullName));
                    }
                }

                if (lslType == LSLType.Void)
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            (isProperty ? "Property" : "Field") +
                            " '{0}' in class '{1}' was declared with the Type '{2}' that the selected type converter of Type '{3}' converted into 'LSLType.Void'.",
                            info.Name, info.DeclaringType.FullName, memberType.FullName, converter.GetType().FullName));
                }


                result.Type = lslType;
            }


            if (!result.ExplicitValueStringPresent)
            {
                string convertedValueString;

                var converter = result.ValueStringConverterInstance ?? FallBackValueStringConverter;

                if (!converter.Convert(result.Type, retrievedMemberValue.ToString(), out convertedValueString))
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            (isProperty ? "Property" : "Field") +
                            " '{0}' in class '{1}' was declared with the (ToString'd) value '{2}' that was unable to be converted by the selected value string converter of Type '{3}'.",
                            info.Name, info.DeclaringType.FullName, retrievedMemberValue, converter.GetType().FullName));
                }

                if (convertedValueString == null)
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            (isProperty ? "Property" : "Field") +
                            " '{0}' in class '{1}' was declared with the (ToString'd) value '{2}' that the value" +
                            " string converter of Type '{3}' converted into 'null', null conversion result is not allowed.",
                            info.Name, info.DeclaringType.FullName, retrievedMemberValue, converter.GetType().FullName));
                }

                result.RetrievedValueString = convertedValueString;
            }


            return result;
        }
    }
}