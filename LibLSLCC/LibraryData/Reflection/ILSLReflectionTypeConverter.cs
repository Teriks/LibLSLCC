using System;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Interface for converting <see cref="LSLType"/> to and from <see cref="Type"/>
    /// </summary>
    public interface ILSLReflectionTypeConverter
    {
        /// <summary>
        /// Convert a runtime <see cref="Type"/> to its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="type">The runtime <see cref="Type"/> to convert.</param>
        /// <param name="lslType">Output <see cref="LSLType"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        bool ConvertType(Type type, out LSLType lslType);


        /// <summary>
        /// Convert an LSL <see cref="LSLType"/> to its corresponding runtime <see cref="Type"/>
        /// </summary>
        /// <param name="lslType">The LSL <see cref="LSLType"/> to convert.</param>
        /// <param name="type">Output <see cref="Type"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        bool ConvertType(LSLType lslType, out Type type);
    }
}