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
    public class LSLLibraryDataSerializableAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the <see cref="ILSLTypeConverter"/> used to convert the return types of methods in a given class to <see cref="LSLType"/>'s.
        /// The return type converter defined in the class attribute can be overridden per method by <see cref="LSLFunctionAttribute.ReturnTypeConverter"/>.
        /// This property will override <see cref="LSLLibraryDataReflectionSerializer.ReturnTypeConverter"/> if it is set.
        /// </summary>
        /// <value>
        /// The return type converter, should derive from <see cref="ILSLTypeConverter"/>.
        /// </value>
        public Type ReturnTypeConverter { get; set; }


        /// <summary>
        /// Gets or sets the <see cref="ILSLTypeConverter"/> used to convert the parameter types of methods in a given class to <see cref="LSLType"/>'s.
        /// The parameter type converter defined in the class attribute can be overridden per method by <see cref="LSLFunctionAttribute.ParamTypeConverter"/>.
        /// This property will override <see cref="LSLLibraryDataReflectionSerializer.ParamTypeConverter"/> if it is set.
        /// </summary>
        /// <value>
        /// The parameter type converter, should derive from <see cref="ILSLTypeConverter"/>.
        /// </value>
        public Type ParamTypeConverter { get; set; }


        /// <summary>
        /// Gets or sets the <see cref="ILSLTypeConverter"/> used to convert the types of fields/properties in a given class to <see cref="LSLType"/>'s.
        /// The constant type converter defined in the class attribute can be overridden per method by <see cref="LSLConstantAttribute.TypeConverter"/>.
        /// This property will override <see cref="LSLLibraryDataReflectionSerializer.ConstantTypeConverter"/> if it is set.
        /// </summary>
        /// <value>
        /// The parameter type converter, should derive from <see cref="ILSLTypeConverter"/>.
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
            if (!Attribute.IsDefined(fromClass, typeof (LSLLibraryDataSerializableAttribute))) return null;

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





        internal static ILSLTypeConverter GetReturnTypeConverter(Type fromClass)
        {
            return _GetTypeConverter<ILSLTypeConverter>(fromClass, "ReturnTypeConverter");
        }

        internal static ILSLTypeConverter GetParamTypeConverter(Type fromClass)
        {
            return _GetTypeConverter<ILSLTypeConverter>(fromClass, "ParamTypeConverter");
        }

        internal static ILSLTypeConverter GetConstantTypeConverter(Type fromClass)
        {
            return _GetTypeConverter<ILSLTypeConverter>(fromClass, "ConstantTypeConverter");
        }

        internal static ILSLValueStringConverter GetValueStringConverter(Type fromClass)
        {
            return _GetTypeConverter<ILSLValueStringConverter>(fromClass, "ValueStringConverter");
        }
    }
}