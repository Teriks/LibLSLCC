#region FileInfo

// 
// File: LSLLibraryDataReflectionSerializer.cs
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using LibLSLCC.CodeValidator;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    ///     EventArgs for FilterMethod events in <see cref="LSLLibraryDataReflectionSerializer" />.
    /// </summary>
    public sealed class AutoFilteredMethodEventArgs : EventArgs
    {
        /// <summary>
        ///     Construct a filtered method event args object given the filtered <see cref="MethodInfo" />.
        /// </summary>
        /// <param name="member">The <see cref="MethodInfo" /> that was filtered.</param>
        public AutoFilteredMethodEventArgs(MethodInfo member)
        {
            Member = member;
        }


        /// <summary>
        ///     The method that was filtered.
        /// </summary>
        public MethodInfo Member { get; private set; }
    }

    /// <summary>
    ///     EventArg for FilterConstant events in <see cref="LSLLibraryDataReflectionSerializer" />.
    /// </summary>
    public sealed class AutoFilteredConstantEventArgs : EventArgs
    {
        /// <summary>
        ///     Construct a filtered constant event args object given the filtered <see cref="MemberInfo" />.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo" /> that was filtered.</param>
        public AutoFilteredConstantEventArgs(MemberInfo member)
        {
            Member = member;
        }


        /// <summary>
        ///     The member that was filtered.
        /// </summary>
        public MemberInfo Member { get; private set; }
    }

    /// <summary>
    ///     EventArg for FilterConstantField events in <see cref="LSLLibraryDataReflectionSerializer" />.
    /// </summary>
    public sealed class AutoFilteredConstantFieldEventArgs : EventArgs
    {
        /// <summary>
        ///     Construct a filtered constant event args object given the filtered <see cref="FieldInfo" />.
        /// </summary>
        /// <param name="member">The <see cref="FieldInfo" /> that was filtered.</param>
        public AutoFilteredConstantFieldEventArgs(FieldInfo member)
        {
            Member = member;
        }


        /// <summary>
        ///     The member that was filtered.
        /// </summary>
        public FieldInfo Member { get; private set; }
    }

    /// <summary>
    ///     EventArg for FilterConstantProperty events in <see cref="LSLLibraryDataReflectionSerializer" />.
    /// </summary>
    public sealed class AutoFilteredConstantPropertyEventArgs : EventArgs
    {
        /// <summary>
        ///     Construct a filtered constant event args object given the filtered <see cref="PropertyInfo" />.
        /// </summary>
        /// <param name="member">The <see cref="PropertyInfo" /> that was filtered.</param>
        public AutoFilteredConstantPropertyEventArgs(PropertyInfo member)
        {
            Member = member;
        }


        /// <summary>
        ///     The member that was filtered.
        /// </summary>
        public PropertyInfo Member { get; private set; }
    }

    /// <summary>
    ///     Serializes library signature objects from CSharp types using runtime reflection
    /// </summary>
    public class LSLLibraryDataReflectionSerializer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLLibraryDataReflectionSerializer" /> class.
        /// </summary>
        public LSLLibraryDataReflectionSerializer()
        {
            AttributedConstantsOnly = true;
            AttributedMethodsOnly = true;
            AttributedParametersOnly = true;
        }


        /// <summary>
        ///     Gets or sets the method filter which can pre-filter <see cref="MethodInfo" /> objects from the reflection search
        ///     results.
        /// </summary>
        /// <value>
        ///     The method filter.
        /// </value>
        public ILSLMethodFilter MethodFilter { get; set; }

        /// <summary>
        ///     Gets or sets the parameter filter, which can filter <see cref="ParameterInfo" /> objects out of a function
        ///     definition generated by the serializer.
        ///     This function should return <c>true</c> if a parameter is to be filtered out of the generated
        ///     <see cref="LSLLibraryFunctionSignature" />.
        /// </summary>
        /// <value>
        ///     The parameter filter.
        /// </value>
        public Func<ParameterInfo, bool> ParameterFilter { get; set; }

        /// <summary>
        ///     Gets or sets the constant filter which can pre-filter <see cref="FieldInfo" /> and <see cref="PropertyInfo" />
        ///     objects from the reflection search results.
        /// </summary>
        /// <value>
        ///     The constant filter.
        /// </value>
        public ILSLConstantFilter ConstantFilter { get; set; }

        /// <summary>
        ///     Gets or sets the reflection <see cref="BindingFlags" /> used to search for class properties.
        /// </summary>
        /// <value>
        ///     The property binding flags.
        /// </value>
        public BindingFlags PropertyBindingFlags { get; set; }

        /// <summary>
        ///     Gets or sets the reflection <see cref="BindingFlags" /> used to search for class fields.
        /// </summary>
        /// <value>
        ///     The field binding flags.
        /// </value>
        public BindingFlags FieldBindingFlags { get; set; }

        /// <summary>
        ///     Gets or sets the reflection <see cref="BindingFlags" /> used to search for class methods.
        /// </summary>
        /// <value>
        ///     The method binding flags.
        /// </value>
        public BindingFlags MethodBindingFlags { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to filter methods lacking an <see cref="LSLFunctionAttribute" /> where the
        ///     return
        ///     type of the method cannot be converted to an <see cref="LSLType" /> by the implementation of
        ///     <see cref="ILSLReturnTypeConverter" />
        ///     assigned to <see cref="ReturnTypeConverter" />.  If set to <c>false</c> an
        ///     <see cref="LSLReflectionTypeMappingException" /> will
        ///     be thrown when a return type is found to be un-convertible.
        /// </summary>
        /// <value>
        ///     <c>true</c> to filter reflected methods lacking an <see cref="LSLFunctionAttribute" /> where the return type is
        ///     un-convertible by
        ///     <see cref="ReturnTypeConverter" />;  otherwise, <c>false</c> to throw an
        ///     <see cref="LSLReflectionTypeMappingException" />.
        /// </value>
        public bool FilterMethodsWithUnmappedReturnTypes { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to filter methods lacking an <see cref="LSLFunctionAttribute" /> where one
        ///     or more parameter
        ///     types of the method cannot be converted to an <see cref="LSLType" /> by the implementation of
        ///     <see cref="ILSLParamTypeConverter" />
        ///     assigned to <see cref="ParamTypeConverter" />.  If set to <c>false</c> an
        ///     <see cref="LSLReflectionTypeMappingException" /> will
        ///     be thrown when a parameter type is found to be un-convertible.
        /// </summary>
        /// <value>
        ///     <c>true</c> to filter reflected methods lacking an <see cref="LSLFunctionAttribute" /> where a parameter type is
        ///     un-convertible by
        ///     <see cref="ParamTypeConverter" />;  otherwise, <c>false</c> to throw an
        ///     <see cref="LSLReflectionTypeMappingException" />.
        /// </value>
        public bool FilterMethodsWithUnmappedParamTypes { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to filter field/properties lacking an <see cref="LSLConstantAttribute" />
        ///     where the declaration
        ///     type of the class member cannot be converted to an <see cref="LSLType" /> by the implementation of
        ///     <see cref="ILSLConstantTypeConverter" />
        ///     assigned to <see cref="ConstantTypeConverter" />.  If set to <c>false</c> an
        ///     <see cref="LSLReflectionTypeMappingException" /> will
        ///     be thrown when a field/property type is found to be un-convertible.
        /// </summary>
        /// <value>
        ///     <c>true</c> to filter reflected fields/properties lacking an <see cref="LSLConstantAttribute" /> where a declared
        ///     type is
        ///     un-convertible by <see cref="ConstantTypeConverter" />;  otherwise, <c>false</c> to throw an
        ///     <see cref="LSLReflectionTypeMappingException" />.
        /// </value>
        public bool FilterConstantsWithUnmappedTypes { get; set; }

        /// <summary>
        ///     Gets or sets base <see cref="ILSLValueStringConverter" /> to be used when a field or property lacks an
        ///     <see cref="LSLConstantAttribute" /> or
        ///     when <see cref="LSLLibraryDataSerializableAttribute.ValueStringConverter" /> or
        ///     <see cref="LSLConstantAttribute.ValueStringConverter" /> is not specified
        ///     to override it.
        /// </summary>
        /// <value>
        ///     The value string converter.
        /// </value>
        public ILSLValueStringConverter ValueStringConverter { get; set; }

        /// <summary>
        ///     Gets or sets base <see cref="ILSLConstantTypeConverter" /> to be used when a field or property lacks an
        ///     <see cref="LSLConstantAttribute" /> or
        ///     when <see cref="LSLLibraryDataSerializableAttribute.ConstantTypeConverter" /> or
        ///     <see cref="LSLConstantAttribute.TypeConverter" /> is not specified
        ///     to override it.
        /// </summary>
        /// <value>
        ///     The constant type converter.
        /// </value>
        public ILSLConstantTypeConverter ConstantTypeConverter { get; set; }

        /// <summary>
        ///     Gets or sets base <see cref="ILSLReturnTypeConverter" /> to be used when a method lacks an
        ///     <see cref="LSLFunctionAttribute" /> or
        ///     when <see cref="LSLLibraryDataSerializableAttribute.ReturnTypeConverter" /> or
        ///     <see cref="LSLFunctionAttribute.ReturnTypeConverter" /> is not specified
        ///     to override it.
        /// </summary>
        /// <value>
        ///     The return type converter.
        /// </value>
        public ILSLReturnTypeConverter ReturnTypeConverter { get; set; }

        /// <summary>
        ///     Gets or sets base <see cref="ILSLParamTypeConverter" /> to be used when a method parameter lacks an
        ///     <see cref="LSLParamAttribute" /> or
        ///     when <see cref="LSLLibraryDataSerializableAttribute.ParamTypeConverter" /> or
        ///     <see cref="LSLFunctionAttribute.ParamTypeConverter" /> is not specified
        ///     to override it.
        /// </summary>
        /// <value>
        ///     The parameter type converter.
        /// </value>
        public ILSLParamTypeConverter ParamTypeConverter { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to filter fields and properties that lack an
        ///     <see cref="LSLConstantAttribute" /> and are declared with/return a <c>null</c> value.
        ///     Instance fields will be considered to have <c>null</c> values if you do not provide an object instance to
        ///     <see cref="DeSerializeConstants(System.Type,object)" />.
        ///     If a <c>null</c> value is encountered in a field or property that lacks an <see cref="LSLConstantAttribute" /> and
        ///     this is <c>false</c>, an <see cref="LSLLibraryDataReflectionException" /> will be thrown.
        /// </summary>
        /// <value>
        ///     <c>true</c> to filter out null field/property values on field's/properties lacking an
        ///     <see cref="LSLConstantAttribute" />; otherwise, <c>false</c> to throw
        ///     <see cref="LSLLibraryDataReflectionException" />.
        /// </value>
        public bool FilterNullConstants { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to filter un-attributed fields and properties whose values where converted
        ///     into an invalid/un-parsable
        ///     ValueString by <see cref="ValueStringConverter" />.  This is only effective for properties/fields that lack an
        ///     <see cref="LSLConstantAttribute" />.
        ///     ValueStrings returned from converters for attributed constants are always rigorously checked for errors, an
        ///     <see cref="LSLLibraryDataReflectionException" />
        ///     will be thrown if there is a ValueString parsing error caused by an attributed constant.
        /// </summary>
        /// <value>
        ///     <c>true</c> to filter out fields/property's with invalid ValueString's when they are lacking an
        ///     <see cref="LSLConstantAttribute" />;
        ///     otherwise, <c>false</c> to throw <see cref="LSLLibraryDataReflectionException" />.
        /// </value>
        public bool FilterInvalidValueStrings { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to filter fields and properties whose values failed to convert into a
        ///     ValueString according to <see cref="ValueStringConverter" />.
        ///     This is only effective for properties/fields that lack an <see cref="LSLConstantAttribute" />.  Failed conversions
        ///     for attributed constants always throw an
        ///     <see cref="LSLLibraryDataReflectionException" />.
        /// </summary>
        /// <value>
        ///     <c>true</c> to filter out fields/property's lacking an <see cref="LSLConstantAttribute" /> where
        ///     <see cref="ValueStringConverter" />
        ///     fails to convert their value to a ValueString;  otherwise, <c>false</c> to throw
        ///     <see cref="LSLLibraryDataReflectionException" />.
        /// </value>
        public bool FilterValueStringConversionFailures { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the serializer should only de-serialize methods marked with an
        ///     <see cref="LSLFunctionAttribute" />.
        ///     The default value is <c>true</c>.  This property is taken into consideration only AFTER filtering by
        ///     <see cref="ParameterFilter" /> has taken place.
        /// </summary>
        /// <value>
        ///     <c>true</c> If the serializer should only de-serialize methods marked with an <see cref="LSLFunctionAttribute" />;
        ///     otherwise, <c>false</c>.
        /// </value>
        public bool AttributedMethodsOnly { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the serializer should only add parameters marked with an
        ///     <see cref="LSLParamAttribute" /> to de-serialized <see cref="LSLLibraryFunctionSignature" /> objects.
        ///     This option is only effective on methods that have an <see cref="LSLFunctionAttribute" />.
        ///     The default value is <c>true</c>.
        /// </summary>
        /// <value>
        ///     <c>true</c> If the serializer should only de-serialize methods marked with an <see cref="LSLFunctionAttribute" />;
        ///     otherwise, <c>false</c>.
        /// </value>
        public bool AttributedParametersOnly { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the serializer should only de-serialize fields and properties marked with
        ///     an <see cref="LSLConstantAttribute" />.
        ///     The default value is <c>true</c>.
        /// </summary>
        /// <value>
        ///     <c>true</c> If the serializer should only de-serialize fields and properties marked with an
        ///     <see cref="LSLConstantAttribute" />; otherwise, <c>false</c>.
        /// </value>
        public bool AttributedConstantsOnly { get; set; }

        /// <summary>
        ///     Occurs when <see cref="FilterMethodsWithUnmappedReturnTypes" /> is <c>true</c> and an un-attributed method is
        ///     filtered because
        ///     <see cref="ReturnTypeConverter" /> cannot successfully convert it's return type to an <see cref="LSLType" />.
        /// </summary>
        public event EventHandler<AutoFilteredMethodEventArgs> OnFilterMethodWithUnmappedReturnType;

        /// <summary>
        ///     Occurs when <see cref="FilterMethodsWithUnmappedParamTypes" /> is <c>true</c> and an un-attributed method is
        ///     filtered because
        ///     <see cref="ParamTypeConverter" /> cannot successfully convert one of it's parameter types to an
        ///     <see cref="LSLType" />.
        /// </summary>
        public event EventHandler<AutoFilteredMethodEventArgs> OnFilterMethodWithUnmappedParamType;

        /// <summary>
        ///     Occurs when <see cref="FilterConstantsWithUnmappedTypes" /> is <c>true</c> and an un-attributed constant is
        ///     filtered because
        ///     <see cref="ConstantTypeConverter" /> cannot successfully convert its type to an <see cref="LSLType" />.
        /// </summary>
        public event EventHandler<AutoFilteredConstantEventArgs> OnFilterConstantWithUnmappedType;

        /// <summary>
        ///     Occurs when <see cref="FilterConstantsWithUnmappedTypes" /> is <c>true</c> and an un-attributed constant Property
        ///     is filtered because
        ///     <see cref="ConstantTypeConverter" /> cannot successfully convert its type to an <see cref="LSLType" />.
        /// </summary>
        public event EventHandler<AutoFilteredConstantEventArgs> OnFilterConstantPropertyWithUnmappedType;

        /// <summary>
        ///     Occurs when <see cref="FilterConstantsWithUnmappedTypes" /> is <c>true</c> and an un-attributed constant Field is
        ///     filtered because
        ///     <see cref="ConstantTypeConverter" /> cannot successfully convert its type to an <see cref="LSLType" />.
        /// </summary>
        public event EventHandler<AutoFilteredConstantEventArgs> OnFilterConstantFieldWithUnmappedType;

        /// <summary>
        ///     Occurs when <see cref="FilterNullConstants" /> is <c>true</c> and an un-attributed constant is filtered for having
        ///     a null field/property value.
        /// </summary>
        public event EventHandler<AutoFilteredConstantEventArgs> OnFilterNullConstant;

        /// <summary>
        ///     Occurs when <see cref="FilterNullConstants" /> is <c>true</c> and an un-attributed constant is filtered for having
        ///     a null property value.
        /// </summary>
        public event EventHandler<AutoFilteredConstantPropertyEventArgs> OnFilterNullConstantProperty;

        /// <summary>
        ///     Occurs when <see cref="FilterNullConstants" /> is <c>true</c> and an un-attributed constant is filtered for having
        ///     a null field value.
        /// </summary>
        public event EventHandler<AutoFilteredConstantFieldEventArgs> OnFilterNullConstantField;

        /// <summary>
        ///     Occurs when <see cref="FilterInvalidValueStrings" /> is <c>true</c> and an un-attributed constant is filtered for
        ///     having
        ///     an invalid ValueString generated for it by <see cref="ValueStringConverter" />.
        /// </summary>
        public event EventHandler<AutoFilteredConstantEventArgs> OnFilterInvalidValueString;

        /// <summary>
        ///     Occurs when <see cref="FilterInvalidValueStrings" /> is <c>true</c> and an un-attributed constant Property is
        ///     filtered for having
        ///     an invalid ValueString generated for it by <see cref="ValueStringConverter" />.
        /// </summary>
        public event EventHandler<AutoFilteredConstantPropertyEventArgs> OnFilterInvalidValueStringProperty;

        /// <summary>
        ///     Occurs when <see cref="FilterInvalidValueStrings" /> is <c>true</c> and an un-attributed constant Field is filtered
        ///     for having
        ///     an invalid ValueString generated for it by <see cref="ValueStringConverter" />.
        /// </summary>
        public event EventHandler<AutoFilteredConstantFieldEventArgs> OnFilterInvalidValueStringField;

        /// <summary>
        ///     Occurs when <see cref="FilterValueStringConversionFailures" /> is <c>true</c> and an un-attributed constant is
        ///     filtered because
        ///     <see cref="ValueStringConverter" /> reported a ValueString conversion failure for the constants retrieved value.
        /// </summary>
        public event EventHandler<AutoFilteredConstantEventArgs> OnFilterValueStringConversionFailure;

        /// <summary>
        ///     Occurs when <see cref="FilterValueStringConversionFailures" /> is <c>true</c> and an un-attributed constant
        ///     Property is filtered because
        ///     <see cref="ValueStringConverter" /> reported a ValueString conversion failure for the constant Property's retrieved
        ///     value.
        /// </summary>
        public event EventHandler<AutoFilteredConstantPropertyEventArgs> OnFilterValueStringConversionFailureProperty;

        /// <summary>
        ///     Occurs when <see cref="FilterValueStringConversionFailures" /> is <c>true</c> and an un-attributed constant Field
        ///     is filtered because
        ///     <see cref="ValueStringConverter" /> reported a ValueString conversion failure for the constant Field's retrieved
        ///     value.
        /// </summary>
        public event EventHandler<AutoFilteredConstantFieldEventArgs> OnFilterValueStringConversionFailureField;


        private ValueStringConversionResult TryConvertAndAssignConstantValueString(MemberInfo info,
            LSLLibraryConstantSignature sig, object value)
        {
            var asProperty = info as PropertyInfo;
            var asField = info as FieldInfo;

            if (asProperty == null && asField == null)
            {
                throw new ArgumentException("info must be a PropertyInfo object or FieldInfo object.", "info");
            }

            bool isProperty = asProperty != null;

            Type constantMemberType = isProperty
                ? asProperty.PropertyType
                : asField.FieldType;


            string fieldDescription = isProperty ? "Property" : "Field";
            string fieldDescriptionPossessive = isProperty ? "Properties" : "Field's";


            string convertedValueString;

            bool conversionSuccess;
            if (isProperty)
            {
                conversionSuccess = ValueStringConverter.ConvertProperty(asProperty, sig.Type, value,
                    out convertedValueString);
            }
            else
            {
                conversionSuccess = ValueStringConverter.ConvertField(asField, sig.Type, value,
                    out convertedValueString);
            }

            if (!conversionSuccess)
            {
                if (!FilterValueStringConversionFailures)
                {
                    throw new LSLLibraryDataReflectionException(
                        string.Format(
                            "LSLLibraryDataReflectionSerializer.ValueStringConverter failed to convert value string " +
                            "from {0} '{1}' of type '{2}' in class of type '{3}'.  " +
                            "The retrieved {4} value was '{5}' (ToSTring'd):",
                            fieldDescription,
                            info.Name,
                            constantMemberType.Name,
                            info.DeclaringType,
                            fieldDescriptionPossessive,
                            value));
                }

                InvokeOnFilterValueStringConversionFailure(info);
                return ValueStringConversionResult.ConverterReportedFailure;
            }

            if (convertedValueString == null)
            {
                //this is always an error
                throw new LSLLibraryDataReflectionException(
                    string.Format(
                        typeof (LSLLibraryDataReflectionSerializer).Name +
                        ".ValueStringConverter returned success and a null ValueString " +
                        "from {0} '{1}' of type '{2}' in class of type '{3}'.  " +
                        "The retrieved {4} value was '{5}' (ToSTring'd):",
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
                if (!FilterInvalidValueStrings)
                {
                    throw new LSLLibraryDataReflectionException(
                        string.Format(
                            typeof (LSLLibraryDataReflectionSerializer).Name +
                            ".ValueStringConverter returned the ValueString '{0}' that " +
                            typeof (LSLLibraryConstantSignature).Name +
                            " could not parse for LSLType '{1}'. The property value given to the converter was " +
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

                InvokeOnFilterInvalidValueString(info);
                return ValueStringConversionResult.ConverterProducedUnparsableString;
            }

            return ValueStringConversionResult.Success;
        }


        private LSLLibraryConstantSignature _DoDeSerializeConstant(
            MemberInfo info,
            ILSLConstantTypeConverter optionalClassConstantTypeConverter,
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
                return new LSLLibraryConstantSignature(attributeInfo.Type, info.Name,
                    attributeInfo.RetrievedValueString)
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

            if (isProperty)
            {
                if (!ConstantTypeConverter.ConvertProperty(propertyInfo, out propertyType))
                {
                    if (FilterConstantsWithUnmappedTypes)
                    {
                        InvokeOnFilterConstantWithUnmappedType(info);
                        return null;
                    }

                    throw new LSLReflectionTypeMappingException(
                        string.Format(
                            "Class property '{0}' was declared with a Type {1} that could not be mapped by the ConstantTypeConverter of Type {2}",
                            info.Name, fieldType.FullName, ConstantTypeConverter.GetType().FullName),
                        fieldType);
                }
            }
            else
            {
                if (!ConstantTypeConverter.ConvertField(fieldInfo, out propertyType))
                {
                    if (FilterConstantsWithUnmappedTypes)
                    {
                        InvokeOnFilterConstantWithUnmappedType(info);
                        return null;
                    }

                    throw new LSLReflectionTypeMappingException(
                        string.Format(
                            "Class field '{0}' was declared with a Type {1} that could not be mapped by the ConstantTypeConverter of Type {2}",
                            info.Name, fieldType.FullName, ConstantTypeConverter.GetType().FullName),
                        fieldType);
                }
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
            LSLLibraryConstantSignature result;

            if (!fieldIsStatic)
            {
                //cant declare without a value, cant get a value without an instance, filter it
                if (fieldValueInstance == null) return null;

                if (fieldValueInstance.GetType() != info.DeclaringType)
                {
                    throw new LSLLibraryDataReflectionException(
                        string.Format(
                            "Cannot retrieve field/property '{0}''s value from 'constantValueInstance' of Type {1}.  Because the given {2}'.DeclaringType' (Type '{3}') did not " +
                            "equal 'constantValueInstance.GetType()' (Type '{1}'), and '{2}' described a non-static field which requires an object instance to retrieve a value from.",
                            info.Name,
                            fieldValueInstance.GetType().FullName,
                            info.GetType().Name,
                            info.DeclaringType.Name));
                }


                fieldValue = isProperty
                    ? propertyInfo.GetValue(fieldValueInstance, null)
                    : fieldInfo.GetValue(fieldValueInstance);
            }
            // ReSharper disable once RedundantIfElseBlock
            else
            {
                //static field/property detected

                fieldValue = isProperty ? propertyInfo.GetValue(null, null) : fieldInfo.GetValue(null);
            }


            if (fieldValue == null && FilterNullConstants)
            {
                InvokeOnFilterNullConstant(info);
                return null;
            }
            if (fieldValue != null)
            {
                result = new LSLLibraryConstantSignature(propertyType, info.Name);

                var valueStringConverterResults = TryConvertAndAssignConstantValueString(info, result, fieldValue);

                if (valueStringConverterResults != ValueStringConversionResult.Success)
                {
                    //filter, if the problem is not set to be filtered by the serializer
                    //TryConvertAndAssignConstantValueString will throw an appropriate serialization exception
                    return null;
                }
            }
            else
            {
                //throw if the field/property value was null and we asked the serializer to throw in this condition
                throw new LSLLibraryDataReflectionException(
                    string.Format(
                        "Instance field/property '{0}' belonging to type {1} returned a null field value.",
                        info.Name, info.DeclaringType.FullName));
            }

            return result;
        }


        /// <summary>
        ///     de-serialize a <see cref="LSLLibraryFunctionSignature" /> from a <see cref="MethodInfo" /> object.
        /// </summary>
        /// <param name="info">The <see cref="MethodInfo" /> object to de-serialize from.</param>
        /// <returns>The de-serialized <see cref="LSLLibraryFunctionSignature" /> or <c>null</c>.</returns>
        public LSLLibraryFunctionSignature DeSerializeMethod(MethodInfo info)
        {
            var classReturnTypeConverter = LSLLibraryDataSerializableAttribute.GetReturnTypeConverter(info.DeclaringType);
            var classParamTypeConverter = LSLLibraryDataSerializableAttribute.GetParamTypeConverter(info.DeclaringType);

            return _DoDeSerializeMethod(info, classReturnTypeConverter, classParamTypeConverter);
        }


        /// <summary>
        ///     de-serialize a <see cref="LSLLibraryConstantSignature" /> from a <see cref="PropertyInfo" /> object.
        /// </summary>
        /// <param name="info">The <see cref="PropertyInfo" /> object to de-serialize from.</param>
        /// <param name="optionalInstance">
        ///     An optional object instance to provide to serializer.
        ///     Instance properties will be considered <c>null</c> if one is not provided.
        /// </param>
        /// <returns>The de-serialized <see cref="LSLLibraryConstantSignature" /> or <c>null</c>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public LSLLibraryConstantSignature DeSerializeConstant(PropertyInfo info, object optionalInstance = null)
        {
            var constantTypeConverter = LSLLibraryDataSerializableAttribute.GetConstantTypeConverter(info.DeclaringType);
            var valueStringConverter = LSLLibraryDataSerializableAttribute.GetValueStringConverter(info.DeclaringType);

            return _DoDeSerializeConstant(info, constantTypeConverter, valueStringConverter, optionalInstance);
        }


        /// <summary>
        ///     de-serialize a <see cref="LSLLibraryConstantSignature" /> from a <see cref="MemberInfo" /> object.
        ///     The passed <see cref="MemberInfo" /> object must derive from either <see cref="PropertyInfo" /> or
        ///     <see cref="FieldInfo" />.
        /// </summary>
        /// <param name="info">The <see cref="MemberInfo" /> object to de-serialize from.</param>
        /// <param name="optionalInstance">
        ///     An optional object instance to provide to serializer.
        ///     Instance properties will be considered <c>null</c> if one is not provided.
        /// </param>
        /// <exception cref="LSLLibraryDataReflectionException">
        ///     Thrown if <paramref name="info" /> does not derive from either
        ///     <see cref="PropertyInfo" /> or <see cref="FieldInfo" />.
        /// </exception>
        /// <returns>The de-serialized <see cref="LSLLibraryConstantSignature" /> or <c>null</c>.</returns>
        public LSLLibraryConstantSignature DeSerializeConstantGeneric(MemberInfo info, object optionalInstance = null)
        {
            var prop = info as PropertyInfo;
            var field = info as FieldInfo;

            if (prop != null || field != null)
            {
                var constantTypeConverter =
                    LSLLibraryDataSerializableAttribute.GetConstantTypeConverter(info.DeclaringType);
                var valueStringConverter =
                    LSLLibraryDataSerializableAttribute.GetValueStringConverter(info.DeclaringType);

                return _DoDeSerializeConstant(info, constantTypeConverter, valueStringConverter, optionalInstance);
            }

            throw new LSLLibraryDataReflectionException(
                string.Format(
                    "DeSerializeConstant(MemberInfo,object); was passed an '{0}' in the info parameter, which derived from neither PropertyInfo or FieldInfo.",
                    info.GetType().Name));
        }


        /// <summary>
        ///     de-serialize a <see cref="LSLLibraryConstantSignature" /> from a <see cref="FieldInfo" /> object.
        /// </summary>
        /// <param name="info">The <see cref="FieldInfo" /> object to de-serialize from.</param>
        /// <param name="optionalInstance">
        ///     An optional object instance to provide to serializer.
        ///     Instance fields will be considered <c>null</c> if one is not provided.
        /// </param>
        /// <returns>The de-serialized <see cref="LSLLibraryConstantSignature" /> or <c>null</c>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public LSLLibraryConstantSignature DeSerializeConstant(FieldInfo info, object optionalInstance = null)
        {
            var constantTypeConverter = LSLLibraryDataSerializableAttribute.GetConstantTypeConverter(info.DeclaringType);
            var valueStringConverter = LSLLibraryDataSerializableAttribute.GetValueStringConverter(info.DeclaringType);

            return _DoDeSerializeConstant(info, constantTypeConverter, valueStringConverter, optionalInstance);
        }


        private LSLLibraryFunctionSignature _DoDeSerializeMethod(MethodInfo info,
            ILSLReturnTypeConverter optionalClassReturnTypeConverter,
            ILSLParamTypeConverter optionalClassParamTypeConverter)
        {
            var attributeSerializer = new LSLFunctionAttributeSerializer
            {
                //prefer the converters on the class we are serializing to ours if they are not null
                FallBackReturnTypeConverter = optionalClassReturnTypeConverter ?? ReturnTypeConverter,
                FallBackParameterTypeConverter = optionalClassParamTypeConverter ?? ParamTypeConverter,
                AttributedParametersOnly = AttributedParametersOnly,
                ParameterFilter = ParameterFilter,
            };

            var attributeInfo = attributeSerializer.GetInfo(info);

            if (attributeInfo != null)
            {
                return new LSLLibraryFunctionSignature(attributeInfo.ReturnType, info.Name, attributeInfo.Parameters)
                {
                    ModInvoke = attributeInfo.ModInvoke,
                    Deprecated = attributeInfo.Deprecated
                };
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


            if (!ReturnTypeConverter.ConvertReturn(info, out returnType))
            {
                if (FilterMethodsWithUnmappedReturnTypes)
                {
                    InvokeOnFilterMethodWithUnmappedReturnType(info);
                    return null;
                }

                throw new LSLReflectionTypeMappingException(
                    string.Format("Unmapped return type '{0}' in .NET function named '{1}': ", info.ReturnType.Name,
                        info.Name), info.ReturnType);
            }

            var parameters = new List<LSLParameter>();

            var infoParameters = info.GetParameters().AsEnumerable();

            if (ParameterFilter != null)
            {
                infoParameters = infoParameters.Where(x => !ParameterFilter(x));
            }

            foreach (var param in infoParameters)
            {
                var isVariadic =
                    param.GetCustomAttributesData()
                        .Any(x => x.Constructor.DeclaringType == typeof (ParamArrayAttribute));

                var cSharpParameterType = isVariadic ? param.ParameterType.GetElementType() : param.ParameterType;

                LSLType parameterType;
                if (!ParamTypeConverter.ConvertParameter(param, cSharpParameterType, out parameterType))
                {
                    if (FilterMethodsWithUnmappedParamTypes)
                    {
                        InvokeOnFilterMethodWithUnmappedParamType(info);
                        return null;
                    }


                    throw new LSLReflectionTypeMappingException(
                        string.Format(
                            "Unmapped parameter type '{0}' in .NET function named '{1}' at parameter index {2}: ",
                            param.ParameterType.Name, info.Name, param.Position), info.ReturnType);
                }

                var name = param.Name;

                parameters.Add(new LSLParameter(parameterType, name, isVariadic));
            }

            var signature = new LSLLibraryFunctionSignature(returnType, info.Name, parameters);

            if (MethodFilter == null) return signature;

            return MethodFilter.MutateSignature(this, info, signature) ? null : signature;
        }


        /// <summary>
        ///     de-serialize <see cref="LSLLibraryFunctionSignature" />'s from a class or interface using the options provided to
        ///     the serializer.
        /// </summary>
        /// <param name="objectType">The type of the class or interface to serialize method definitions from.</param>
        /// <returns>
        ///     An enumerable of de-serialized <see cref="LSLLibraryFunctionSignature" /> generated from the object type's
        ///     methods.
        /// </returns>
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


        private static bool FilterCompilerGenerated(MemberInfo info)
        {
            return
                info.GetCustomAttributesData()
                    .All(x => x.Constructor.DeclaringType != typeof (CompilerGeneratedAttribute));
        }


        /// <summary>
        ///     de-serialize <see cref="LSLLibraryConstantSignature" />'s from a class or interface using the options provided to
        ///     the serializer.
        ///     Any non-static field or property encountered in the class will be considered <c>null</c> if no object instance is
        ///     provided in <paramref name="typeInstance" />
        /// </summary>
        /// <param name="objectType">The type of the class or interface to serialize method definitions from.</param>
        /// <param name="typeInstance">
        ///     An optional instance of the type, which instance field/property values can be taken from.
        ///     Any non-static field or property encountered in the class will be considered <c>null</c> if no object instance is
        ///     provided.
        /// </param>
        /// <returns>
        ///     An enumerable of de-serialized <see cref="LSLLibraryConstantSignature" /> generated from the object type's
        ///     fields and properties.
        /// </returns>
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
        ///     de-serialize <see cref="LSLLibraryConstantSignature" />'s from a object instance using the options provided to the
        ///     serializer.
        /// </summary>
        /// <param name="fieldValueInstance">
        ///     The object instance to use, instance field and property values will be able to be
        ///     retrieved from this instance.
        /// </param>
        /// <returns>
        ///     An enumerable of de-serialized <see cref="LSLLibraryConstantSignature" /> generated from the object instances
        ///     fields and properties.
        /// </returns>
        public IEnumerable<LSLLibraryConstantSignature> DeSerializeConstants(object fieldValueInstance)
        {
            return DeSerializeConstants(fieldValueInstance.GetType(), fieldValueInstance);
        }


        /// <summary>
        ///     <para>
        ///         Invokes <see cref="OnFilterNullConstant" /> first.
        ///         Then Invokes <see cref="OnFilterNullConstantProperty" /> if <paramref name="member" /> is a
        ///         <see cref="PropertyInfo" /> object.
        ///         Otherwise, invokes <see cref="OnFilterNullConstantField" /> if <paramref name="member" /> is a
        ///         <see cref="FieldInfo" /> object.
        ///     </para>
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo" /> of the member being filtered.</param>
        protected virtual void InvokeOnFilterNullConstant(MemberInfo member)
        {
            var handler = OnFilterNullConstant;
            if (handler != null) handler(this, new AutoFilteredConstantEventArgs(member));

            if (OnFilterNullConstantProperty != null)
            {
                var prop = member as PropertyInfo;
                if (prop != null)
                {
                    OnFilterNullConstantProperty(this, new AutoFilteredConstantPropertyEventArgs(prop));
                }
            }

            if (OnFilterNullConstantField != null)
            {
                var field = member as FieldInfo;
                if (field != null)
                {
                    OnFilterNullConstantField(this, new AutoFilteredConstantFieldEventArgs(field));
                }
            }
        }


        /// <summary>
        ///     <para>
        ///         Invokes <see cref="OnFilterInvalidValueString" /> first.
        ///         Then Invokes <see cref="OnFilterInvalidValueStringProperty" /> if <paramref name="member" /> is a
        ///         <see cref="PropertyInfo" /> object.
        ///         Otherwise, invokes <see cref="OnFilterInvalidValueStringField" /> if <paramref name="member" /> is a
        ///         <see cref="FieldInfo" /> object.
        ///     </para>
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo" /> of the member being filtered.</param>
        protected virtual void InvokeOnFilterInvalidValueString(MemberInfo member)
        {
            var handler = OnFilterInvalidValueString;
            if (handler != null) handler(this, new AutoFilteredConstantEventArgs(member));

            if (OnFilterInvalidValueStringProperty != null)
            {
                var prop = member as PropertyInfo;
                if (prop != null)
                {
                    OnFilterInvalidValueStringProperty(this, new AutoFilteredConstantPropertyEventArgs(prop));
                }
            }

            if (OnFilterInvalidValueStringField != null)
            {
                var field = member as FieldInfo;
                if (field != null)
                {
                    OnFilterInvalidValueStringField(this, new AutoFilteredConstantFieldEventArgs(field));
                }
            }
        }


        /// <summary>
        ///     <para>
        ///         Invokes <see cref="OnFilterValueStringConversionFailure" /> first.
        ///         Then Invokes <see cref="OnFilterValueStringConversionFailureProperty" /> if <paramref name="member" /> is a
        ///         <see cref="PropertyInfo" /> object.
        ///         Otherwise, invokes <see cref="OnFilterValueStringConversionFailureField" /> if <paramref name="member" /> is a
        ///         <see cref="FieldInfo" /> object.
        ///     </para>
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo" /> of the member being filtered.</param>
        protected virtual void InvokeOnFilterValueStringConversionFailure(MemberInfo member)
        {
            var handler = OnFilterValueStringConversionFailure;
            if (handler != null) handler(this, new AutoFilteredConstantEventArgs(member));

            if (OnFilterValueStringConversionFailureProperty != null)
            {
                var prop = member as PropertyInfo;
                if (prop != null)
                {
                    OnFilterValueStringConversionFailureProperty(this, new AutoFilteredConstantPropertyEventArgs(prop));
                }
            }

            if (OnFilterValueStringConversionFailureField != null)
            {
                var field = member as FieldInfo;
                if (field != null)
                {
                    OnFilterValueStringConversionFailureField(this, new AutoFilteredConstantFieldEventArgs(field));
                }
            }
        }


        /// <summary>
        ///     <para>
        ///         Invokes <see cref="OnFilterConstantWithUnmappedType" /> first.
        ///         Then Invokes <see cref="OnFilterConstantPropertyWithUnmappedType" /> if <paramref name="member" /> is a
        ///         <see cref="PropertyInfo" /> object.
        ///         Otherwise, invokes <see cref="OnFilterConstantFieldWithUnmappedType" /> if <paramref name="member" /> is a
        ///         <see cref="FieldInfo" /> object.
        ///     </para>
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo" /> of the member being filtered.</param>
        protected virtual void InvokeOnFilterConstantWithUnmappedType(MemberInfo member)
        {
            var handler = OnFilterConstantWithUnmappedType;
            if (handler != null) handler(this, new AutoFilteredConstantEventArgs(member));

            if (OnFilterConstantPropertyWithUnmappedType != null)
            {
                var prop = member as PropertyInfo;
                if (prop != null)
                {
                    OnFilterConstantPropertyWithUnmappedType(this, new AutoFilteredConstantEventArgs(prop));
                }
            }

            if (OnFilterConstantFieldWithUnmappedType != null)
            {
                var field = member as FieldInfo;
                if (field != null)
                {
                    OnFilterConstantFieldWithUnmappedType(this, new AutoFilteredConstantEventArgs(field));
                }
            }
        }


        /// <summary>
        ///     Invokes <see cref="OnFilterMethodWithUnmappedParamType" />.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo" /> of the member being filtered.</param>
        protected virtual void InvokeOnFilterMethodWithUnmappedParamType(MethodInfo member)
        {
            var handler = OnFilterMethodWithUnmappedParamType;
            if (handler != null) handler(this, new AutoFilteredMethodEventArgs(member));
        }


        /// <summary>
        ///     Invokes <see cref="OnFilterMethodWithUnmappedReturnType" />.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo" /> of the member being filtered.</param>
        protected virtual void InvokeOnFilterMethodWithUnmappedReturnType(MethodInfo member)
        {
            var handler = OnFilterMethodWithUnmappedReturnType;
            if (handler != null) handler(this, new AutoFilteredMethodEventArgs(member));
        }


        private enum ValueStringConversionResult
        {
            Success,
            ConverterReportedFailure,
            ConverterProducedUnparsableString,
        }
    }
}