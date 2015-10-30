using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Optional Attribute for exposing method parameters to <see cref="LSLLibraryDataReflectionSerializer"/> without having to map types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class LSLParamAttribute : Attribute
    {
        /// <summary>
        /// Gets the <see cref="LSLType"/> of the parameter as specified by the attribute.
        /// </summary>
        /// <value>
        /// The <see cref="LSLType"/> of the parameter.
        /// </value>
        public LSLType Type { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="LSLParamAttribute"/> class.
        /// </summary>
        /// <param name="type">The <see cref="LSLType"/> of the parameter, variadic status is detected automagically.</param>
        public LSLParamAttribute(LSLType type)
        {
            Type = type;
        }
    }
}