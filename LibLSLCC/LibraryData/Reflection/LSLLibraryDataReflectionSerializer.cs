using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using Antlr4.Runtime.Atn;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Serializes library signature objects from CSharp types using runtime reflection
    /// </summary>
    public class LSLLibraryDataReflectionSerializer
    {

        /// <summary>
        /// Gets or sets the method filter which can pre-filter <see cref="MethodInfo"/> objects from the reflection search results.
        /// </summary>
        /// <value>
        /// The method filter.
        /// </value>
        public ILSLMethodFilter MethodFilter { get; set; }


        /// <summary>
        /// Gets or sets the constant filter which can pre-filter <see cref="FieldInfo"/> and <see cref="PropertyInfo"/> objects from the reflection search results.
        /// </summary>
        /// <value>
        /// The constant filter.
        /// </value>
        public ILSLReflectionConstantFilter ConstantFilter { get; set; }


        /// <summary>
        /// Gets or sets the reflection <see cref="BindingFlags"/> used to search for class properties.
        /// </summary>
        /// <value>
        /// The property binding flags.
        /// </value>
        public BindingFlags PropertyBindingFlags { get; set; }


        /// <summary>
        /// Gets or sets the reflection <see cref="BindingFlags"/> used to search for class fields.
        /// </summary>
        /// <value>
        /// The field binding flags.
        /// </value>
        public BindingFlags FieldBindingFlags { get; set; }


        /// <summary>
        /// Gets or sets the reflection <see cref="BindingFlags"/> used to search for class methods.
        /// </summary>
        /// <value>
        /// The method binding flags.
        /// </value>
        public BindingFlags MethodBindingFlags { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to throw a <see cref="LSLReflectionTypeMappingException"/> when <see cref="ReturnTypeConverter"/>
        /// fails to map a type on a method without a <see cref="LSLFunctionAttribute"/>.  If false then the method is just discarded (filtered). 
        /// </summary>
        /// <value>
        /// <c>true</c> if a failed return type conversion by <see cref="ReturnTypeConverter"/> on a method lacking an <see cref="LSLFunctionAttribute"/> causes an <see cref="LSLReflectionTypeMappingException"/> otherwise, <c>false</c>.
        /// </value>
        public bool ThrowOnUnmappedReturnTypeInMethod { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to throw a <see cref="LSLReflectionTypeMappingException"/> when <see cref="ParamTypeConverter"/>
        /// fails to map a type on a method without a <see cref="LSLFunctionAttribute"/>.  If false then the method is just discarded (filtered). 
        /// </summary>
        /// <value>
        /// <c>true</c> if a failed return type conversion by <see cref="ParamTypeConverter"/> on a method lacking an <see cref="LSLFunctionAttribute"/> causes an <see cref="LSLReflectionTypeMappingException"/> otherwise, <c>false</c>.
        /// </value>
        public bool ThrowOnUnmappedParamTypeInMethod { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to throw a <see cref="LSLReflectionTypeMappingException"/> when <see cref="ConstantTypeConverter"/>
        /// fails to map a type on a field/property without a <see cref="LSLConstantAttribute"/>.  If false then the field or property is just discarded (filtered). 
        /// </summary>
        /// <value>
        /// <c>true</c> if a failed return type conversion by <see cref="ConstantTypeConverter"/> on a field or property lacking an <see cref="LSLConstantAttribute"/> causes an <see cref="LSLReflectionTypeMappingException"/> otherwise, <c>false</c>.
        /// </value>
        public bool ThrowOnUnmappedTypeInConstant { get; set; }


        /// <summary>
        /// Gets or sets base <see cref="ILSLValueStringConverter"/> to be used when a field or property lacks an <see cref="LSLConstantAttribute"/> or
        /// when <see cref="LSLLibraryDataSerializableAttribute.ValueStringConverter"/> or <see cref="LSLConstantAttribute.ValueStringConverter"/> is not specified
        /// to override it.
        /// </summary>
        /// <value>
        /// The value string converter.
        /// </value>
        public ILSLValueStringConverter ValueStringConverter { get; set; }


        /// <summary>
        /// Gets or sets base <see cref="ILSLTypeConverter"/> to be used when a field or property lacks an <see cref="LSLConstantAttribute"/> or
        /// when <see cref="LSLLibraryDataSerializableAttribute.ConstantTypeConverter"/> or <see cref="LSLConstantAttribute.TypeConverter"/> is not specified
        /// to override it.
        /// </summary>
        /// <value>
        /// The constant type converter.
        /// </value>
        public ILSLTypeConverter ConstantTypeConverter { get; set; }


        /// <summary>
        /// Gets or sets base <see cref="ILSLTypeConverter"/> to be used when a method lacks an <see cref="LSLFunctionAttribute"/> or
        /// when <see cref="LSLLibraryDataSerializableAttribute.ReturnTypeConverter"/> or <see cref="LSLFunctionAttribute.ReturnTypeConverter"/> is not specified
        /// to override it.
        /// </summary>
        /// <value>
        /// The return type converter.
        /// </value>
        public ILSLTypeConverter ReturnTypeConverter { get; set; }


        /// <summary>
        /// Gets or sets base <see cref="ILSLTypeConverter"/> to be used when a method parameter lacks an <see cref="LSLParamAttribute"/> or
        /// when <see cref="LSLLibraryDataSerializableAttribute.ParamTypeConverter"/> or <see cref="LSLFunctionAttribute.ParamTypeConverter"/> is not specified
        /// to override it.
        /// </summary>
        /// <value>
        /// The parameter type converter.
        /// </value>
        public ILSLTypeConverter ParamTypeConverter { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to filter fields and properties that lack an <see cref="LSLConstantAttribute"/> and are declared with/return a <c>null</c> value.
        /// Instance fields will be considered to have <c>null</c> values if you do not provide an object instance to <see cref="DeSerializeConstants(System.Type,object)"/>.
        /// If a <c>null</c> value is encountered in a field or property that lacks an <see cref="LSLConstantAttribute"/> and this is <c>false</c>, an <see cref="LSLLibraryDataReflectionException"/> will be thrown.
        /// </summary>
        /// <value>
        /// <c>true</c> to filter out null field/property values on field's/properties lacking an <see cref="LSLConstantAttribute"/>; otherwise, <c>false</c> to throw <see cref="LSLLibraryDataReflectionException"/>.
        /// </value>
        public bool FilterNullFieldsAndProperties { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the serializer should only de-serialize methods marked with an <see cref="LSLFunctionAttribute"/>.
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> If the serializer should only de-serialize methods marked with an <see cref="LSLFunctionAttribute"/>; otherwise, <c>false</c>.
        /// </value>
        public bool AttributedMethodsOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the serializer should only de-serialize fields and properties marked with an <see cref="LSLConstantAttribute"/>.
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> If the serializer should only de-serialize fields and properties marked with an <see cref="LSLConstantAttribute"/>; otherwise, <c>false</c>.
        /// </value>
        public bool AttributedConstantsOnly { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataReflectionSerializer"/> class.
        /// </summary>
        public LSLLibraryDataReflectionSerializer()
        {
            AttributedConstantsOnly = true;
            AttributedMethodsOnly = true;
        }



        private LSLLibraryConstantSignature TryConvertConstantValueString(MemberInfo info,
            LSLLibraryConstantSignature sig, object value)
        {
            bool isProperty = info is PropertyInfo;

            Type constantMemberType = isProperty
                ? ((PropertyInfo) info).PropertyType
                : ((FieldInfo) info).FieldType;


            string fieldDescription = isProperty ? "Property" : "Field";
            string fieldDescriptionPossessive = isProperty ? "Properties" : "Field's";


            string convertedValueString;

            if (!ValueStringConverter.Convert(sig.Type, value, out convertedValueString))
            {
                return sig;
            }

            if (convertedValueString == null)
            {
                throw new LSLLibraryDataReflectionException(
                    string.Format(
                        "LSLLibraryDataReflectionSerializer.ValueStringConverter returned a null ValueString " +
                        "from {0} '{1}' of type '{2}' in class of type '{3}'.  " +
                        "The {4} retrieved value was {5} (ToSTring'd):",
                        fieldDescription,
                        info.Name,
                        constantMemberType.Name,
                        info.DeclaringType,
                        fieldDescriptionPossessive,
                        value));
            }


            try
            {
                sig.ValueString = convertedValueString;
            }
            catch (LSLInvalidConstantValueStringException e)
            {
                throw new LSLLibraryDataReflectionException(
                    string.Format(
                        "LSLLibraryDataReflectionSerializer.ValueStringConverter returned the ValueString '{0}' that " +
                        "LSLLibraryConstantSignature could not parse for LSLType '{1}'. The property value given to the converter was " +
                        "taken from {2} '{3}' of type '{4}' in class of type '{5}'.  " +
                        "The {6} value was '{7}' (ToString'd):" +
                        LSLFormatTools.CreateNewLinesString(2) + e.Message,
                        convertedValueString,
                        sig.Type,
                        fieldDescription,
                        info.Name,
                        constantMemberType.Name,
                        info.DeclaringType.Name,
                        fieldDescriptionPossessive,
                        value));
            }

            return sig;
        }


        private LSLLibraryConstantSignature _DoDeSerializeConstant(
            MemberInfo info,
            ILSLTypeConverter optionalClassConstantTypeConverter,
            ILSLValueStringConverter optionalClassConstantValueStringConverter,
            object fieldValueInstance = null)
        {
            var attributeSerializer = new LSLConstantAttributeSerializer
            {
                OptionalDeclaringTypeInstance = fieldValueInstance,

                //prefer the converters on the class attribute to ours if they are available
                FallBackTypeConverter = optionalClassConstantTypeConverter ?? ConstantTypeConverter,
                FallBackValueStringConverter = optionalClassConstantValueStringConverter ?? ValueStringConverter
            };


            var propertyInfo = info as PropertyInfo;
            var fieldInfo = info as FieldInfo;

            bool isProperty = propertyInfo != null;


            var attributeInfo = isProperty
                ? attributeSerializer.GetInfo(propertyInfo)
                : attributeSerializer.GetInfo(fieldInfo);


            if (attributeInfo != null)
            {
                return new LSLLibraryConstantSignature(attributeInfo.Type, info.Name, attributeInfo.RetrievedValueString)
                {
                    Deprecated = attributeInfo.Deprecated,
                    Expand = attributeInfo.Expand
                };
            }

            if (AttributedConstantsOnly)
            {
                //no attribute, and we did not want anything without an attribute to be serialized
                return null;
            }


            if (propertyInfo != null && !propertyInfo.CanRead) return null;


            Type fieldType = isProperty ? propertyInfo.PropertyType : fieldInfo.FieldType;
            bool fieldIsStatic = isProperty ? propertyInfo.GetGetMethod(true).IsStatic : fieldInfo.IsStatic;


            //cant serialize without either of these
            if (ConstantTypeConverter == null || ValueStringConverter == null)
            {
                return null;
            }

            LSLType propertyType;
            if (!ConstantTypeConverter.Convert(fieldType, out propertyType))
            {
                if (ThrowOnUnmappedTypeInConstant)
                {
                    throw new LSLReflectionTypeMappingException(
                        string.Format(
                            "Class field/property '{0}' was declared with a Type {1} that could not be mapped by the ConstantTypeConverter of Type {2}",
                            info.Name, fieldType.FullName, ConstantTypeConverter.GetType().FullName),
                        fieldType);
                }

                return null;
            }


            if (propertyType == LSLType.Void)
            {
                throw new LSLLibraryDataReflectionException(
                    string.Format(
                        "Field/Property '{0}' of type '{1}' is was mapped to LSLType.Void by the ConstantTypeConverter of Type '{2}', this is not valid.",
                        info.Name,
                        info.DeclaringType.FullName,
                        ConstantTypeConverter));
            }

            object fieldValue;

            if (!fieldIsStatic)
            {
                //cant declare without a value, cant get a value without an instance, filter it
                if (fieldValueInstance == null) return null;

                if (fieldValueInstance.GetType() != info.DeclaringType)
                {
                    throw new LSLLibraryDataReflectionException(
                        string.Format(
                            "Cannot retrieve field/property '{0}''s value from 'constantValueInstance' of Type {1}.  " +
                            "Because the given {2}'.DeclaringType' (Type '{3}') did not " +
                            "equal 'constantValueInstance.GetType()' (Type '{1}'), and '{2}' described a non-static field which requires an object instance to retrieve a value from.",
                            info.Name,
                            fieldValueInstance.GetType().FullName,
                            info.GetType().Name,
                            info.DeclaringType.Name));
                }


                fieldValue = isProperty
                    ? propertyInfo.GetValue(fieldValueInstance, null)
                    : fieldInfo.GetValue(fieldValueInstance);

                if (fieldValue == null && FilterNullFieldsAndProperties)
                {
                    return null;
                }
                if (fieldValue != null)
                {
                    return TryConvertConstantValueString(info, new LSLLibraryConstantSignature(propertyType, info.Name),
                        fieldValue);
                }

                //throw if the field/property value was null and we asked the serializer to throw in this condition
                throw new LSLLibraryDataReflectionException(
                    string.Format("Instance field/property '{0}' belonging to type {1} returned a null field value.",
                        info.Name, info.DeclaringType.FullName));
            }


            //static field/property detected

            fieldValue = isProperty ? propertyInfo.GetValue(null, null) : fieldInfo.GetValue(null);

            if (fieldValue == null && FilterNullFieldsAndProperties)
            {
                return null;
            }
            if (fieldValue != null)
            {
                return TryConvertConstantValueString(info, new LSLLibraryConstantSignature(propertyType, info.Name),
                    fieldValue);
            }

            //throw if the field/property value was null and we asked the serializer to throw in this condition
            throw new LSLLibraryDataReflectionException(
                string.Format("Static field/property '{0}' belonging to type {1} returned a null field value.",
                    info.Name,
                    info.DeclaringType.FullName));

            //no value converter
        }




        private LSLLibraryFunctionSignature _DoDeSerializeMethod(MethodInfo info,
            ILSLTypeConverter optionalClassReturnTypeConverter,
            ILSLTypeConverter optionalClassParamTypeConverter)
        {
            var attributeSerializer = new LSLFunctionAttributeSerializer
            {
                //prefer the converters on the class we are serializing to ours if they are not null
                FallBackReturnTypeConverter = optionalClassReturnTypeConverter ?? ReturnTypeConverter,
                FallBackParameterTypeConverter = optionalClassParamTypeConverter ?? ParamTypeConverter
            };

            var attributeInfo = attributeSerializer.GetInfo(info);

            if (attributeInfo != null)
            {
                return new LSLLibraryFunctionSignature(attributeInfo.ReturnType, info.Name, attributeInfo.Parameters);
            }


            if (AttributedMethodsOnly)
            {
                return null;
            }


            //cant serialize without these.
            if (ReturnTypeConverter == null || ParamTypeConverter == null)
            {
                return null;
            }

            LSLType returnType;


            if (!ReturnTypeConverter.Convert(info.ReturnType, out returnType))
            {
                if (ThrowOnUnmappedReturnTypeInMethod)
                {
                    throw new LSLReflectionTypeMappingException(
                        string.Format("Unmapped return type '{0}' in .NET function named '{1}': ", info.ReturnType.Name,
                            info.Name), info.ReturnType);
                }
                return null;
            }

            var parameters = new GenericArray<LSLParameter>();
            foreach (var param in info.GetParameters())
            {
                var isVariadic =
                    param.GetCustomAttributesData()
                        .Any(x => x.Constructor.DeclaringType == typeof (ParamArrayAttribute));

                var cSharpParameterType = isVariadic ? param.ParameterType.GetElementType() : param.ParameterType;

                LSLType parameterType;
                if (!ParamTypeConverter.Convert(cSharpParameterType, out parameterType))
                {
                    if (ThrowOnUnmappedParamTypeInMethod)
                    {
                        throw new LSLReflectionTypeMappingException(
                            string.Format(
                                "Unmapped parameter type '{0}' in .NET function named '{1}' at parameter index {2}: ",
                                param.ParameterType.Name, info.Name, param.Position), info.ReturnType);
                    }
                    return null;
                }

                var name = param.Name;

                parameters.Add(new LSLParameter(parameterType, name, isVariadic));
            }

            var signature = new LSLLibraryFunctionSignature(returnType, info.Name, parameters);

            if (MethodFilter == null) return signature;

            return MethodFilter.MutateSignature(this, info, signature) ? null : signature;
        }




        /// <summary>
        /// de-serialize <see cref="LSLLibraryFunctionSignature"/>'s from a class or interface using the options provided to the serializer.
        /// </summary>
        /// <param name="objectType">The type of the class or interface to serialize method definitions from.</param>
        /// <returns>An enumerable of de-serialized <see cref="LSLLibraryFunctionSignature"/> generated from the object type's methods.</returns>
        public IEnumerable<LSLLibraryFunctionSignature> DeSerializeMethods(Type objectType)
        {
            var classReturnTypeConverter = LSLLibraryDataSerializableAttribute.GetReturnTypeConverter(objectType);
            var classParamTypeConverter = LSLLibraryDataSerializableAttribute.GetParamTypeConverter(objectType);

            return
                objectType.GetMethods(MethodBindingFlags)
                    .Where(
                        x => FilterCompilerGenerated(x) &&
                             (!x.IsSpecialName && !x.IsConstructor && !x.IsGenericMethod) &&
                             (MethodFilter == null || !MethodFilter.PreFilter(this, x)))
                    .Select(
                        x => _DoDeSerializeMethod(x, classReturnTypeConverter, classParamTypeConverter))
                    .Where(x => x != null);
        }




        private bool FilterCompilerGenerated(MemberInfo info)
        {
            return
                info.GetCustomAttributesData()
                    .All(x => x.Constructor.DeclaringType != typeof (CompilerGeneratedAttribute));
        }


        /// <summary>
        /// de-serialize <see cref="LSLLibraryConstantSignature"/>'s from a class or interface using the options provided to the serializer.
        /// Any non-static field or property encountered in the class will be considered <c>null</c> if no object instance is provided in <paramref name="typeInstance"/>
        /// </summary>
        /// <param name="objectType">The type of the class or interface to serialize method definitions from.</param>
        /// <param name="typeInstance">
        /// An optional instance of the type, which instance field/property values can be taken from.
        /// Any non-static field or property encountered in the class will be considered <c>null</c> if no object instance is provided.
        /// </param>
        /// <returns>An enumerable of de-serialized <see cref="LSLLibraryConstantSignature"/> generated from the object type's fields and properties.</returns>
        public IEnumerable<LSLLibraryConstantSignature> DeSerializeConstants(Type objectType,
            object typeInstance = null)
        {
            var classConstantTypeConverter =
                LSLLibraryDataSerializableAttribute.GetConstantTypeConverter(objectType);

            var classValueStringConverter =
                LSLLibraryDataSerializableAttribute.GetValueStringConverter(objectType);


            var props =
                objectType.GetProperties(PropertyBindingFlags)
                    .Where(
                        x =>
                            FilterCompilerGenerated(x) && (ConstantFilter == null || !ConstantFilter.PreFilter(this, x)))
                    .Select(
                        x =>
                            _DoDeSerializeConstant(x, classConstantTypeConverter,
                                classValueStringConverter, typeInstance))
                    .Where(x => x != null);


            return objectType.GetFields(FieldBindingFlags)
                .Where(x => FilterCompilerGenerated(x) && (ConstantFilter == null || !ConstantFilter.PreFilter(this, x)))
                .Select(
                    x =>
                        _DoDeSerializeConstant(x, classConstantTypeConverter, classValueStringConverter,
                            typeInstance))
                .Where(x => x != null)
                .Concat(props);
        }


        /// <summary>
        /// de-serialize <see cref="LSLLibraryConstantSignature"/>'s from a object instance using the options provided to the serializer.
        /// </summary>
        /// <param name="fieldValueInstance">The object instance to use, instance field and property values will be able to be retrieved from this instance.</param>
        /// <returns>An enumerable of de-serialized <see cref="LSLLibraryConstantSignature"/> generated from the object instances fields and properties.</returns>

        public IEnumerable<LSLLibraryConstantSignature> DeSerializeConstants(object fieldValueInstance)
        {
            return DeSerializeConstants(fieldValueInstance.GetType(), fieldValueInstance);
        }
    }
}