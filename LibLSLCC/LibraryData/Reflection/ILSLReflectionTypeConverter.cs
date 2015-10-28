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
        /// Convert a runtime method return <see cref="Type"/> to its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="type">The runtime return <see cref="Type"/> to convert.</param>
        /// <param name="lslType">Output <see cref="LSLType"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        bool ConvertReturnType(Type type, out LSLType lslType);



        /// <summary>
        /// Convert a runtime method parameter <see cref="Type"/> to its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="type">The runtime parameter <see cref="Type"/> to convert.</param>
        /// <param name="lslType">Output <see cref="LSLType"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        bool ConvertParameterType(Type type, out LSLType lslType);


        /// <summary>
        /// Convert a runtime property declaration <see cref="Type"/> to its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="type">The runtime property declaration <see cref="Type"/> to convert.</param>
        /// <param name="lslType">Output <see cref="LSLType"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        bool ConvertPropertyType(Type type, out LSLType lslType);



        /// <summary>
        /// Convert a runtime field declaration <see cref="Type"/> to its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="type">The runtime field declaration <see cref="Type"/> to convert.</param>
        /// <param name="lslType">Output <see cref="LSLType"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        bool ConvertFieldType(Type type, out LSLType lslType);


    }
}