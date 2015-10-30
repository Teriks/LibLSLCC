using System;
using System.Collections.Generic;
using System.Net.Configuration;
using System.Text;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.Utility;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Optional attribute for exposing properties and fields to <see cref="LSLLibraryDataReflectionSerializer"/> without having to map types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LSLConstantAttribute : Attribute
    {
        /// <summary>
        /// Gets the <see cref="LSLType"/> of the constant as specified by the attribute.
        /// </summary>
        /// <value>
        /// The <see cref="LSLType"/> of the constant.
        /// </value>
        public LSLType Type { get; private set; }




        /// <summary>
        /// Gets a value indicating what serialized <see cref="LSLLibraryConstantSignature"/> objects should have <see cref="LSLLibraryConstantSignature.Expand"/> set to.
        /// </summary>
        /// <value>
        ///  The value to set <see cref="LSLLibraryConstantSignature.Expand"/> to.
        /// </value>
        public bool Expand { get; private set; }

        /// <summary>
        /// Gets a value indicating what serialized <see cref="LSLLibraryConstantSignature"/> objects should have <see cref="LSLLibraryConstantSignature.Deprecated"/> set to.
        /// </summary>
        /// <value>
        ///  The value to set <see cref="LSLLibraryConstantSignature.Deprecated"/> to.
        /// </value>
        public bool Deprecated { get; private set; }


        /// <summary>
        /// Gets or sets the value string to be assigned to <see cref="LSLLibraryConstantSignature.ValueString"/> if no <see cref="ValueStringConverter"/> type is present.
        /// You cannot have both an explicit <see cref="LSLLibraryConstantSignature.ValueString"/> and  <see cref="ValueStringConverter"/> at the same time, this will cause an exception from the serializer.
        /// </summary>
        /// <value>
        /// The value string to be assigned to <see cref="LSLLibraryConstantSignature.ValueString"/>.
        /// </value>
        public string ValueString { get; set; }


        /// <summary>
        /// <para>
        /// Gets or sets the value string converter <see cref="Type"/>, this type should derive from <see cref="ILSLValueStringConverter"/> or you will get exceptions from the serializer.
        /// The value string converter is responsible for converting field/property values into something that is parsable by <see cref="LSLLibraryConstantSignature.ValueString"/>.
        /// You cannot have both an explicit <see cref="LSLLibraryConstantSignature.ValueString"/> and  <see cref="ValueStringConverter"/> at the same time, this will cause an exception from the serializer.
        /// This property is only optional if the class is using a defined <see cref="LSLLibraryDataSerializableAttribute.ValueStringConverter"/> or <see cref="LSLLibraryDataReflectionSerializer.ValueStringConverter"/> is set.
        /// Setting this property will override both <see cref="LSLLibraryDataSerializableAttribute.ValueStringConverter"/> and <see cref="LSLLibraryDataReflectionSerializer.ValueStringConverter"/>.
        /// </para>
        /// </summary>
        /// <value>
        /// The value string converter <see cref="Type"/> which sould derive from <see cref="ILSLValueStringConverter"/>.
        /// </value>
        public Type ValueStringConverter { get; set; }


        /// <summary>
        /// <para>
        /// Gets or sets the type converter that is used in the case that no <see cref="Type"/> is given, it takes the property type and converts it to an <see cref="LSLType"/>.
        /// This property is only optional if the class is using a defined <see cref="LSLLibraryDataSerializableAttribute.ConstantTypeConverter"/> or <see cref="LSLLibraryDataReflectionSerializer.ConstantTypeConverter"/> is set.
        /// Setting this property will override both <see cref="LSLLibraryDataSerializableAttribute.ConstantTypeConverter"/> and <see cref="LSLLibraryDataReflectionSerializer.ConstantTypeConverter"/>.
        /// </para>
        /// </summary>
        /// <value>
        /// The type converter which should derive from <see cref="ILSLTypeConverter"/>.
        /// </value>
        public Type TypeConverter { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="LSLConstantAttribute"/> class.
        /// </summary>
        /// <param name="constantType">The <see cref="LSLType"/> of the constant.</param>
        public LSLConstantAttribute(LSLType constantType)
        {
            Type = constantType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLConstantAttribute"/> class.
        /// </summary>
        public LSLConstantAttribute()
        {
        }
    }
}