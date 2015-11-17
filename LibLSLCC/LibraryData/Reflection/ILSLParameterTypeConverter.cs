using System;
using System.Reflection;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Interface for converting .NET runtime <see cref="ParameterInfo"/>'s into their equivalent <see cref="LSLType"/>.
    /// This is used to convert the parameter types encountered in reflected .NET methods.
    /// This interface is used with <see cref="LSLLibraryDataReflectionSerializer"/> and the library data attributes.
    /// </summary>
    /// <seealso cref="LSLLibraryDataSerializableAttribute.ConstantTypeConverter"/>
    /// <seealso cref="LSLLibraryDataSerializableAttribute.ReturnTypeConverter"/>
    /// <seealso cref="LSLLibraryDataSerializableAttribute.ParamTypeConverter"/>
    /// <seealso cref="LSLConstantAttribute.TypeConverter"/>
    /// <seealso cref="LSLFunctionAttribute.ReturnTypeConverter"/>
    /// <seealso cref="LSLFunctionAttribute.ParamTypeConverter"/>
    public interface ILSLParameterTypeConverter
    {
        /// <summary>
        /// Converts the specified <see cref="ParameterInfo"/> into its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="parameterInfo">Runtime <see cref="ParameterInfo"/> for converting to an <see cref="LSLType"/>.</param>
        /// <param name="basicType">The basic type of the parameter, this will be the parameter arrays base type if the parameter is variadic.</param>
        /// <param name="outType">Resulting <see cref="LSLType"/> from the conversion.</param>
        /// <returns><c>true</c> if the conversion succeeded, <c>false</c> if it failed.</returns>
        bool ConvertParameter(ParameterInfo parameterInfo, Type basicType, out LSLType outType);
    }
}