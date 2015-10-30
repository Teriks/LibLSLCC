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

            public ILSLTypeConverter TypeConverterInstance { get; internal set; }

            public ILSLValueStringConverter ValueStringConverterInstance { get; internal set; }

            public bool Expand { get; internal set; }

            public bool Deprecated { get; internal set; }

            public string RetrievedValueString { get; internal set; }
        }


        public ILSLValueStringConverter FallBackValueStringConverter { get; set; }

        public ILSLTypeConverter FallBackTypeConverter { get; set; }

        public object OptionalDeclaringTypeInstance { get; set; }


        private Info _GetInfo(MemberInfo info)
        {
            if (!Attribute.IsDefined(info, typeof (LSLConstantAttribute)))
            {
                return null;
            }

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

                if (typeConverter.GetInterfaces().Contains(typeof (ILSLTypeConverter)))
                {
                    result.TypeConverterInstance =
                        Activator.CreateInstance(typeConverter) as ILSLTypeConverter;
                }
                else
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            "[LSLConstantAttribute.TypeConverter] does not implement ILSLTypeConverter on property/field '{0}' in class '{1}'.",
                            info.Name, info.DeclaringType.FullName));
                }
            }


            if (result.ExplicitTypePresent && result.ExplicitValueStringPresent)
            {
                return result;
            }


            var propertyInfo = info as PropertyInfo;
            var fieldInfo = info as FieldInfo;

            Type memberType;

            object fieldValue = null;


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

                if (propertyInfo.GetGetMethod(true).IsStatic)
                {
                    fieldValue = propertyInfo.GetValue(null, null);

                    if (fieldValue == null)
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Static Property '{0}' in class '{1}' is tagged with [LSLConstantAttribute] and returned a null value.",
                                info.Name, info.DeclaringType.FullName));
                    }
                }
                else if (this.OptionalDeclaringTypeInstance == null)
                {
                    if (!result.ExplicitValueStringPresent)
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Instance Property '{0}' in class '{1}' has cannot have its value retrieved because no instance of '{1}' was supplied and [LSLConstantAttribute.ValueString] was not explicitly set.",
                                info.Name, info.DeclaringType.FullName));
                    }
                }
                else
                {
                    if (this.OptionalDeclaringTypeInstance.GetType() != info.DeclaringType)
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Instance Property '{0}' in class '{1}' has cannot have its value retrieved because an instance of '{2}' was supplied to retrieve it from and an instance of '{1}' is required.",
                                info.Name, info.DeclaringType.FullName, OptionalDeclaringTypeInstance.GetType().FullName));
                    }


                    fieldValue = propertyInfo.GetValue(OptionalDeclaringTypeInstance, null);

                    if (fieldValue == null)
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Instance Property '{0}' in class '{1}' is tagged with [LSLConstantAttribute] and returned a null value.",
                                info.Name, info.DeclaringType.FullName));
                    }
                }
            }
            else
            {
                memberType = fieldInfo.FieldType;

                if (fieldInfo.IsStatic)
                {
                    fieldValue = fieldInfo.GetValue(null);

                    if (fieldValue == null)
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Static Field '{0}' in class '{1}' is tagged with [LSLConstantAttribute] and returned a null value.",
                                info.Name, info.DeclaringType.FullName));
                    }
                }
                else if (OptionalDeclaringTypeInstance == null)
                {
                    if (!result.ExplicitValueStringPresent)
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Instance Field '{0}' in class '{1}' has cannot have its value retrieved because no instance of '{1}' was supplied and [LSLConstantAttribute.ValueString] was not explicitly set.",
                                info.Name, info.DeclaringType.FullName));
                    }
                }
                else
                {
                    if (OptionalDeclaringTypeInstance.GetType() != info.DeclaringType)
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Instance Field '{0}' in class '{1}' has cannot have its value retrieved because an instance of '{2}' was supplied to retrieve it from and an instance of '{1}' is required.",
                                info.Name, info.DeclaringType.FullName, OptionalDeclaringTypeInstance.GetType().FullName));
                    }


                    fieldValue = fieldInfo.GetValue(OptionalDeclaringTypeInstance);

                    if (fieldValue == null)
                    {
                        throw new LSLLibraryDataAttributeException(
                            string.Format(
                                "Instance Field '{0}' in class '{1}' is tagged with [LSLConstantAttribute] and returned a null value.",
                                info.Name, info.DeclaringType.FullName));
                    }
                }
            }


            if (!result.ExplicitValueStringPresent && result.ValueStringConverterInstance == null &&
                FallBackValueStringConverter == null)
            {
                throw new LSLLibraryDataAttributeException(
                    string.Format(
                        (fieldValue == null ? "Property" : "Field") +
                        " '{0}' in class '{1}' uses an [LSLContantAttribute] without an [LSLContantAttribute.ValueString] or" +
                        " [LSLContantAttribute.ValueStringConverter], one or the other is required if no fall-back value string converter is defined in the serializer.",
                        info.Name, info.DeclaringType.FullName));
            }

            if (!result.ExplicitTypePresent && result.TypeConverterInstance == null && FallBackTypeConverter == null)
            {
                throw new LSLLibraryDataAttributeException(
                    string.Format(
                        (fieldValue == null ? "Property" : "Field") +
                        " '{0}' in class '{1}' uses an [LSLContantAttribute] without a [LSLContantAttribute.Type] or" +
                        " [LSLContantAttribute.TypeConverter], one or the other is required if no fall-back type converter is defined in the serializer.",
                        info.Name, info.DeclaringType.FullName));
            }


            if (!result.ExplicitTypePresent)
            {
                LSLType lslType;

                var converter = result.TypeConverterInstance ?? FallBackTypeConverter;

                if (!converter.Convert(memberType, out lslType))
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            (fieldValue == null ? "Property" : "Field") +
                            " '{0}' in class '{1}' was declared with the Type '{2}' that was unable to be converted by the selected type converter of Type '{3}'.",
                            info.Name, info.DeclaringType.FullName, memberType.FullName, converter.GetType().FullName));
                }

                if (lslType == LSLType.Void)
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            (fieldValue == null ? "Property" : "Field") +
                            " '{0}' in class '{1}' was declared with the Type '{2}' that the selected type converter of Type '{3}' converted into 'LSLType.Void'.",
                            info.Name, info.DeclaringType.FullName, memberType.FullName, converter.GetType().FullName));
                }


                result.Type = lslType;
            }


            if (!result.ExplicitValueStringPresent)
            {
                string convertedValueString;

                var converter = result.ValueStringConverterInstance ?? FallBackValueStringConverter;

                if (!converter.Convert(result.Type, fieldValue.ToString(), out convertedValueString))
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            (fieldValue == null ? "Property" : "Field") +
                            " '{0}' in class '{1}' was declared with the (ToString'd) value '{2}' that was unable to be converted by the selected value string converter of Type '{3}'.",
                            info.Name, info.DeclaringType.FullName, fieldValue, converter.GetType().FullName));
                }

                if (convertedValueString == null)
                {
                    throw new LSLLibraryDataAttributeException(
                        string.Format(
                            (fieldValue == null ? "Property" : "Field") +
                            " '{0}' in class '{1}' was declared with the (ToString'd) value '{2}' that the value" +
                            " string converter of Type '{3}' converted into 'null', null conversion result is not allowed.",
                            info.Name, info.DeclaringType.FullName, fieldValue, converter.GetType().FullName));
                }

                result.RetrievedValueString = convertedValueString;
            }


            return result;
        }
    }
}