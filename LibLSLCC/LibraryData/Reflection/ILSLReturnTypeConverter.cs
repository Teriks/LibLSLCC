using System;
using System.Reflection;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Interface for converting .NET runtime <see cref="MethodInfo"/>'s return type into its equivalent <see cref="LSLType"/>.
    /// This is used to convert the return types encountered in reflected .NET methods.
    /// This interface is used with <see cref="LSLLibraryDataReflectionSerializer"/> and the library data attributes.
    /// </summary>
    /// <seealso cref="LSLLibraryDataSerializableAttribute.ConstantTypeConverter"/>
    /// <seealso cref="LSLLibraryDataSerializableAttribute.ReturnTypeConverter"/>
    /// <seealso cref="LSLLibraryDataSerializableAttribute.ParamTypeConverter"/>
    /// <seealso cref="LSLConstantAttribute.TypeConverter"/>
    /// <seealso cref="LSLFunctionAttribute.ReturnTypeConverter"/>
    /// <seealso cref="LSLFunctionAttribute.ParamTypeConverter"/>
    public interface ILSLReturnTypeConverter
    {
        /// <summary>
        /// Converts the specified <see cref="MethodInfo"/> return type into its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="methodInfo">Runtime <see cref="Type"/> to convert.</param>
        /// <param name="outType">Resulting <see cref="LSLType"/> from the conversion.</param>
        /// <returns><c>true</c> if the conversion succeeded, <c>false</c> if it failed.</returns>
        bool ConvertReturn(MethodInfo methodInfo, out LSLType outType);
    }
}