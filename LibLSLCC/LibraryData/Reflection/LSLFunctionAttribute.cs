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