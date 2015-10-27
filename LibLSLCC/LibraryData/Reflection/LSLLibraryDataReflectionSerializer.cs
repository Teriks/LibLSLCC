using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using Antlr4.Runtime.Atn;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Serializes library signature objects from CSharp types using runtime reflection
    /// </summary>
    public class LSLLibraryDataReflectionSerializer
    {

        /// <summary>
        /// Responsible for mapping a runtime <see cref="Type"/> to a corresponding <see cref="LSLType"/> and vice-versa  
        /// </summary>
        public ILSLReflectionTypeConverter Converter { get; private set; }

        /// <summary>
        /// Gets or sets the method serializer addon, it can add additional data to serialized library functions or choose to filter them.
        /// </summary>
        /// <value>
        /// The method serializer addon.
        /// </value>
        public ILSLReflectionMethodFilter MethodFilter { get; set; }

        /// <summary>
        /// Gets or sets the constant serializer addon, it can add additional data to serialized library constants or choose to filter them.
        /// </summary>
        /// <value>
        /// The constant serializer addon.
        /// </value>
        public ILSLReflectionConstantFilter ConstantFilter { get; set; }



        /// <summary>
        /// Gets or sets the property binding flags used in <see cref="DeSerializeConstants(Type,object)"/> and <see cref="DeSerializeConstants(object)"/> to discover object properties.
        /// </summary>
        /// <value>
        /// The property binding flags to use in <see cref="DeSerializeConstants(Type,object)"/> and <see cref="DeSerializeConstants(object)"/> to discover object properties.
        /// </value>
        public BindingFlags PropertyBindingFlags { get; set; }


        /// <summary>
        /// Gets or sets the field binding flags used in <see cref="DeSerializeConstants(Type,object)"/> and <see cref="DeSerializeConstants(object)"/> to discover object fields.
        /// </summary>
        /// <value>
        /// The field binding flags to use in <see cref="DeSerializeConstants(Type,object)"/> and <see cref="DeSerializeConstants(object)"/> to discover object fields.
        /// </value>
        public BindingFlags FieldBindingFlags { get; set; }

        /// <summary>
        /// Gets or sets the field binding flags used in <see cref="DeSerializeMethods(Type)"/> to discover object methods.
        /// </summary>
        /// <value>
        /// The field binding flags to use in <see cref="DeSerializeMethods(Type)"/> to discover object methods.
        /// </value>
        public BindingFlags MethodBindingFlags { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to throw an <see cref="LSLReflectionTypeMappingException"/> from <see cref="DeSerializeMethod"/> when <see cref="Converter"/> was
        /// unable to convert a <see cref="Type"/> used in a <see cref="MethodInfo"/> signature.  If this is <c>false</c> <see cref="DeSerializeMethod"/> will return <c>null</c> instead of throwing.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="DeSerializeMethod"/> should throw <see cref="LSLReflectionTypeMappingException"/> otherwise, <c>false</c> if it should return <c>null</c>.
        /// </value>
        public bool ThrowOnUnmappedTypeInMethod { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to throw an <see cref="LSLReflectionTypeMappingException"/> from <see cref="DeSerializeConstant(FieldInfo,object)"/> when <see cref="Converter"/> was
        /// unable to convert the <see cref="Type"/> used in <see cref="FieldInfo.FieldType"/>.  If this is <c>false</c> <see cref="DeSerializeConstant(FieldInfo,object)"/> will return <c>null</c> instead of throwing.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="DeSerializeConstant(FieldInfo,object)"/> should throw <see cref="LSLReflectionTypeMappingException"/> otherwise, <c>false</c> if it should return <c>null</c>.
        /// </value>
        public bool ThrowOnUnmappedTypeInField { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to throw an <see cref="LSLReflectionTypeMappingException"/> from <see cref="DeSerializeConstant(PropertyInfo,object)"/> when <see cref="Converter"/> was
        /// unable to convert the <see cref="Type"/> used in <see cref="PropertyInfo.PropertyType"/>.  If this is <c>false</c> <see cref="DeSerializeConstant(PropertyInfo,object)"/> will return <c>null</c> instead of throwing.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="DeSerializeConstant(PropertyInfo)"/> should throw <see cref="LSLReflectionTypeMappingException"/> otherwise, <c>false</c> if it should return <c>null</c>.
        /// </value>
        public bool ThrowOnUnmappedTypeInProperty { get; set; }


        /// <summary>
        /// The function used to convert property/field values to strings so that they can be assigned to <see cref="LSLLibraryConstantSignature.ValueString"/> in de-serialized library constants.
        /// If you don't set this, values taken from objects are simply discarded.
        /// </summary>
        public Func<object, string> FieldValueStringConverter { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether fields/properties who's values are null are to be filtered.
        /// If this is <c>true</c> both <see cref="DeSerializeConstant(PropertyInfo,object)"/> and <see cref="DeSerializeConstant(FieldInfo,object)"/> will return null if the value retrieved from a field/property is <c>null</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if we should filter field/properties that have a null value in the object instance they were reflected from.
        /// </value>
        public bool FilterNullFieldsAndProperties { get; set; }




        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataReflectionSerializer"/> class with a given <see cref="ILSLReflectionTypeConverter"/>
        /// </summary>
        /// <param name="converter">The conversion mapper to assign to <see cref="Converter"/>.</param>
        public LSLLibraryDataReflectionSerializer(ILSLReflectionTypeConverter converter)
        {
            Converter = converter;
        }




        /// <summary>
        /// <para>
        /// De-serializes an <see cref="LSLLibraryConstantSignature"/> from a given <see cref="PropertyInfo"/> object.
        /// If <paramref name="constantValueInstance"/> is not <c>null</c> the value for <see cref="LSLLibraryConstantSignature.ValueString"/> will be determined by the properties value <paramref name="constantValueInstance"/>.
        /// If <see cref="FilterNullFieldsAndProperties"/> is <c>true</c> and the property value retrieved from <paramref name="constantValueInstance"/> is <c>null</c> than this function returns null.
        /// The value retrieved from <paramref name="constantValueInstance"/> is passed through <see cref="FieldValueStringConverter"/> before being assigned to <see cref="LSLLibraryConstantSignature.ValueString"/>.
        /// If <paramref name="constantValueInstance"/> is <c>null</c> the default value associated with <see cref="LSLLibraryConstantSignature.Type"/> is used for <see cref="LSLLibraryConstantSignature.ValueString"/>.
        /// </para>
        /// </summary>
        /// <param name="info">The <see cref="PropertyInfo"/> to deserialize an <see cref="LSLLibraryConstantSignature"/> from.</param>
        /// <returns>
        /// An <see cref="LSLLibraryConstantSignature"/> that was deserialized from the given <see cref="PropertyInfo"/> object.  
        /// Or <c>null</c> if <see cref="PropertyInfo.PropertyType"/> was not convertible by <see cref="Converter"/> and <see cref="ThrowOnUnmappedTypeInProperty"/> is <c>false</c>.
        /// Or <c>null</c> if <see cref="PropertyInfo.CanRead"/> is <c>false</c>.
        /// Or <c>null</c> if <see cref="FilterNullFieldsAndProperties"/> is <c>true</c> and the property value retrieved from <paramref name="constantValueInstance"/> is <c>null</c>.
        /// Or <c>null</c> if the given <see cref="PropertyInfo"/> object or proposed return value was filtered by the <see cref="ILSLReflectionConstantFilter"/> implementation assigned to <see cref="ConstantFilter"/>./>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="info"/> is <c>null</c>.</exception>
        /// <exception cref="LSLReflectionTypeMappingException">Occurs if <see cref="ThrowOnUnmappedTypeInMethod" /> is <c>true</c> and an unmapped type was used in <see cref="PropertyInfo.PropertyType"/>.</exception>
        public LSLLibraryConstantSignature DeSerializeConstant(PropertyInfo info, object constantValueInstance = null)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }


            if (ConstantFilter != null && ConstantFilter.PreFilter(this, info)) return null;

            return _DoDeSerializeConstantWithNoPreChecks(info, constantValueInstance);
        }




        private LSLLibraryConstantSignature _DoDeSerializeConstantWithNoPreChecks(PropertyInfo info, object fieldValueInstance = null)
        {
            LSLType propertyType;
            if (!Converter.ConvertType(info.PropertyType, out propertyType))
            {
                if (ThrowOnUnmappedTypeInProperty)
                {
                    throw new LSLReflectionTypeMappingException(
                        string.Format("Unmapped type '{0}' in .NET property named '{1}': ", info.PropertyType.Name, info.Name),
                        info.PropertyType);
                }
                return null;
            }

            LSLLibraryConstantSignature result;

            if (fieldValueInstance != null && FieldValueStringConverter != null)
            {
                if (!info.CanRead) return null;


                var value = info.GetValue(fieldValueInstance, null);

                if (value == null && FilterNullFieldsAndProperties) return null;

                result = new LSLLibraryConstantSignature(propertyType, info.Name, FieldValueStringConverter(value));
                if (ConstantFilter == null) return result;
                return ConstantFilter.MutateSignature(this, info, result) ? null : result;
            }

            result = new LSLLibraryConstantSignature(propertyType, info.Name);
            if (ConstantFilter == null) return result;
            return ConstantFilter.MutateSignature(this, info, result) ? null : result;
        }


        /// <summary>
        /// <para>
        /// De-serializes an <see cref="LSLLibraryConstantSignature"/> from a given <see cref="FieldInfo"/> object.
        /// If <paramref name="constantValueInstance"/> is not <c>null</c> the value for <see cref="LSLLibraryConstantSignature.ValueString"/> will be determined by the fields value in <paramref name="constantValueInstance"/>.
        /// If <see cref="FilterNullFieldsAndProperties"/> is <c>true</c> and the property value retrieved from <paramref name="constantValueInstance"/> is <c>null</c> than this function returns null.
        /// The value retrieved from <paramref name="constantValueInstance"/> is passed through <see cref="FieldValueStringConverter"/> before being assigned to <see cref="LSLLibraryConstantSignature.ValueString"/>.
        /// If <paramref name="constantValueInstance"/> is <c>null</c> null the default value associated with <see cref="LSLLibraryConstantSignature.Type"/> is used for <see cref="LSLLibraryConstantSignature.ValueString"/>.
        /// </para>
        /// </summary>
        /// <param name="info">The <see cref="FieldInfo"/> to deserialize an <see cref="LSLLibraryConstantSignature"/> from.</param>
        /// <param name="constantValueInstance">The optional object instance to retrieve field values from that will be assigned to <see cref="LSLLibraryConstantSignature.ValueString"/> if <see cref="FieldValueStringConverter"/> is not <c>null</c>.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if the given <see cref="FieldInfo"/> object represents a static field when <paramref name="constantValueInstance"/> is non <c>null</c>,
        /// and has a <see cref="Type"/> that is different from the <see cref="MemberInfo.DeclaringType"/> of <paramref name="info"/>.</exception>
        /// <returns>
        /// An <see cref="LSLLibraryConstantSignature"/> that was deserialized from the given <see cref="FieldInfo"/> object.  
        /// Or <c>null</c> if <see cref="FieldInfo.FieldType"/> was not convertible by <see cref="Converter"/> and <see cref="ThrowOnUnmappedTypeInProperty"/> is <c>false</c>.
        /// Or <c>null</c> if <see cref="FilterNullFieldsAndProperties"/> is <c>true</c> and the property value retrieved from <paramref name="constantValueInstance"/> is <c>null</c>.
        /// Or <c>null</c> if the given <see cref="FieldInfo"/> object or proposed return value was filtered by the <see cref="ILSLReflectionConstantFilter"/> implementation assigned to <see cref="ConstantFilter"/>./>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="info"/> is <c>null</c>.</exception>
        /// <exception cref="LSLReflectionTypeMappingException">Occurs if <see cref="ThrowOnUnmappedTypeInMethod" /> is <c>true</c> and an unmapped type was used in <see cref="FieldInfo.FieldType"/>.</exception>
        public LSLLibraryConstantSignature DeSerializeConstant(FieldInfo info, object constantValueInstance = null)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            if (ConstantFilter != null && ConstantFilter.PreFilter(this, info)) return null;

            return _DoDeSerializeConstantWithNoPreChecks(info, constantValueInstance);
        }





        private LSLLibraryConstantSignature _DoDeSerializeConstantWithNoPreChecks(FieldInfo info, object fieldValueInstance = null)
        {
            LSLType propertyType;
            if (!Converter.ConvertType(info.FieldType, out propertyType))
            {
                if (ThrowOnUnmappedTypeInField)
                {
                    throw new LSLReflectionTypeMappingException(
                        string.Format("Unmapped type '{0}' in .NET field named '{1}': ", info.FieldType.Name, info.Name),
                        info.FieldType);
                }
                return null;
            }

            LSLLibraryConstantSignature result;

            //user has defined a way to convert field/property values into strings.
            if (FieldValueStringConverter != null)
            {
                object fieldValue;

                if (!info.IsStatic)
                {
                    if (fieldValueInstance != null)
                    {
                        if (fieldValueInstance.GetType() != info.DeclaringType)
                        {
                            throw new ArgumentException(
                                "Cannot retrieve a field value from 'constantValueInstance' because the given 'FieldInfo.DeclaringType' did not " +
                                "equal 'constantValueInstance.GetType()', and 'FieldInfo' described a non-static field which requires an object instance to retreive a value from.",
                                "info");
                        }


                        fieldValue = info.GetValue(fieldValueInstance);

                        if (fieldValue == null && FilterNullFieldsAndProperties) return null;

                        result = new LSLLibraryConstantSignature(propertyType, info.Name,
                            FieldValueStringConverter(fieldValue));

                        if (ConstantFilter == null) return result;
                        return ConstantFilter.MutateSignature(this, info, result) ? null : result;
                    }
                }


                fieldValue = info.GetValue(null);

                if (fieldValue == null && FilterNullFieldsAndProperties) return null;

                result = new LSLLibraryConstantSignature(propertyType, info.Name,
                    FieldValueStringConverter(fieldValue));

                if (ConstantFilter == null) return result;
                return ConstantFilter.MutateSignature(this, info, result) ? null : result;
            }


            //ignore any field/property value because there is no way we can convert it to a string assignable to LSLLibraryConstantSignature.ValueString

            result = new LSLLibraryConstantSignature(propertyType, info.Name);

            if (ConstantFilter == null) return result;
            return ConstantFilter.MutateSignature(this, info, result) ? null : result;
        }




        /// <summary>
        /// Serializes a matching library function signature given a <see cref="MethodInfo" /> object that describes a reflected .NET method.
        /// </summary>
        /// <param name="info">The MethodInfo object.</param>
        /// <returns>
        /// An <see cref="LSLLibraryFunctionSignature" /> serialized from the <see cref="MethodInfo" /> object.  
        /// Or <c>null</c> if the <see cref="MethodInfo"/> object utilized an type not convertible by <see cref="Converter"/> and <see cref="ThrowOnUnmappedTypeInMethod"/> was set to <c>false</c>.
        /// Or <c>null</c> if the given <see cref="MethodInfo"/> object or proposed return value was filtered by the <see cref="ILSLReflectionMethodFilter"/> implementation assigned to <see cref="MethodFilter"/>./>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="info" /> is <c>null</c>.</exception>
        /// <exception cref="LSLLibraryDataReflectionException">
        /// If <see cref="MethodBase.IsSpecialName"/> is <c>true</c> in <paramref name="info"/>.
        /// Or if <see cref="MethodBase.IsConstructor"/> is <c>true</c> in <paramref name="info"/>.
        /// Or if <see cref="MethodBase.IsGenericMethod"/> is <c>true</c> in <paramref name="info"/>.
        /// </exception>
        /// <exception cref="LSLReflectionTypeMappingException">Occurs if <see cref="ThrowOnUnmappedTypeInMethod" /> is <c>true</c> and an unmapped type is found in the <see cref="MethodInfo" /> signature.</exception>
        public LSLLibraryFunctionSignature DeSerializeMethod(MethodInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            if (info.IsGenericMethod)
            {
                throw new ArgumentException(string.Format("Cannot de-serialize because method named '{0}' is a generic method.", info.Name));
            }

            if (info.IsConstructor)
            {
                throw new ArgumentException(
                    string.Format("Cannot de-serialize because method named '{0}' is a constructor.", info.Name));
            }

            if (info.IsSpecialName)
            {
                throw new ArgumentException(
                    string.Format("Cannot de-serialize because method named '{0}' is a 'special' method such as a property indexer or array index, etc..", info.Name));
            }


            if (MethodFilter != null && MethodFilter.PreFilter(this, info)) return null;

            return _DoDeSerializeMethodWithNoPreChecks(info);
        }



        private LSLLibraryFunctionSignature _DoDeSerializeMethodWithNoPreChecks(MethodInfo info)
        {
            LSLType returnType;


            if (!Converter.ConvertType(info.ReturnType, out returnType))
            {
                if (ThrowOnUnmappedTypeInMethod)
                {
                    throw new LSLReflectionTypeMappingException(
                        string.Format("Unmapped return type '{0}' in .NET function named '{1}': ", info.ReturnType.Name,
                            info.Name), info.ReturnType);
                }
                return null;
            }

            var parameters = new GenericArray<LSLParameter>();
            foreach (var p in info.GetParameters())
            {
                var isVariadic = p.GetCustomAttributes(typeof (ParamArrayAttribute), false).Length > 0;

                var cSharpParameterType = isVariadic ? p.ParameterType.GetElementType() : p.ParameterType;

                LSLType parameterType;
                if (!Converter.ConvertType(cSharpParameterType, out parameterType))
                {
                    if (ThrowOnUnmappedTypeInMethod)
                    {
                        throw new LSLReflectionTypeMappingException(
                            string.Format(
                                "Unmapped parameter type '{0}' in .NET function named '{1}' at parameter index {2}: ",
                                p.ParameterType.Name, info.Name, p.Position), info.ReturnType);
                    }
                    return null;
                }

                var name = p.Name;

                parameters.Add(new LSLParameter(parameterType, name, isVariadic));
            }

            var signature = new LSLLibraryFunctionSignature(returnType, info.Name, parameters);

            if (MethodFilter == null) return signature;

            return MethodFilter.MutateSignature(this, info, signature) ? null : signature;
        }


        /// <summary>
        /// Serialize all methods of a type into <see cref="LSLLibraryFunctionSignature"/> objects, The <see cref="BindingFlags"/> used for method reflection are taken from <see cref="MethodBindingFlags"/>
        /// If <see cref="MethodFilter"/> is not <c>null</c> and decides to filter a field/property then that field or property is not included in the output from this function.
        /// </summary>
        /// <param name="objectType">The type of the object to obtain the method signatures from.</param>
        /// <exception cref="LSLReflectionTypeMappingException">Occurs while enumerating if <see cref="ThrowOnUnmappedTypeInMethod" /> is <c>true</c> and an unmapped type is found in one of the <see cref="MethodInfo" /> signatures belonging to the <see cref="Type"/> given in <paramref name="objectType"/>.</exception>
        /// <returns>A list of serialized <see cref="LSLLibraryFunctionSignature"/>'s.</returns>
        public IEnumerable<LSLLibraryFunctionSignature> DeSerializeMethods(Type objectType)
        {

            return
                objectType.GetMethods(MethodBindingFlags)
                .Where(x => (!x.IsSpecialName && !x.IsConstructor && !x.IsGenericMethod) && (MethodFilter == null || !MethodFilter.PreFilter(this, x)))
                    .Select(_DoDeSerializeMethodWithNoPreChecks)
                    .Where(x => x != null);

        }

        /// <summary>
        /// Serialize all properties/fields of a type into <see cref="LSLLibraryFunctionSignature"/> objects, The <see cref="BindingFlags"/> used for property/field reflection are taken from <see cref="FieldBindingFlags"/> and <see cref="PropertyBindingFlags"/> respectively.
        /// If <see cref="ConstantFilter"/> is not <c>null</c> and decides to filter a field/property then that field or property is not included in the output from this function.
        /// </summary>
        /// <param name="objectType">The type of the object to obtain the method signatures from.</param>
        /// <param name="fieldValueInstance">The optional object instance to retrieve property/field values from that will be assigned to <see cref="LSLLibraryConstantSignature.ValueString"/> if <see cref="FieldValueStringConverter"/> is not <c>null</c>.</param>
        /// <exception cref="LSLReflectionTypeMappingException">Occurs while enumerating if <see cref="ThrowOnUnmappedTypeInMethod" /> is <c>true</c> and an unmapped type is found in one of the <see cref="MethodInfo" /> signatures belonging to the <see cref="Type"/> given in <paramref name="objectType"/>.</exception>
        /// <returns>A list of <see cref="LSLLibraryConstantSignature"/>'s de-serialized from the <paramref name="objectType"/> <see cref="Type"/> with <see cref="LSLLibraryConstantSignature.ValueString"/> taken from the optional <paramref name="fieldValueInstance"/> object.</returns>
        public IEnumerable<LSLLibraryConstantSignature> DeSerializeConstants(Type objectType, object fieldValueInstance = null)
        {

            return objectType.GetFields(FieldBindingFlags)
                .Where(x => ConstantFilter == null || !ConstantFilter.PreFilter(this, x))
                .Select(x=>_DoDeSerializeConstantWithNoPreChecks(x, fieldValueInstance))
                .Where(x => x != null)
                .Concat(
                    objectType.GetProperties(PropertyBindingFlags).Select(x => _DoDeSerializeConstantWithNoPreChecks(x, fieldValueInstance)).Where(x => x != null)
                    );

        }

        /// <summary>
        /// Delegates to <see cref="DeSerializeConstants(System.Type,object)"/> with the arguments (<paramref name="fieldValueInstance"/>.GetType(), <paramref name="fieldValueInstance"/>).
        /// </summary>
        /// <param name="fieldValueInstance">The object instance to retrieve the information used for <see cref="DeSerializeConstants(System.Type,object)"/> from.</param>
        /// <returns>A list of <see cref="LSLLibraryFunctionSignature"/>'s de-serialized from the <paramref name="fieldValueInstance"/> object.</returns>
        public IEnumerable<LSLLibraryConstantSignature> DeSerializeConstants(object fieldValueInstance)
        {
            return DeSerializeConstants(fieldValueInstance.GetType(), fieldValueInstance);
        }

    }
}
