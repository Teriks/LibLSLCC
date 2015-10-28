using System;
using System.Collections.Generic;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Implements <see cref="ILSLReflectionTypeConverter"/> in memory using two dictionary objects.
    /// </summary>
    public class LSLReflectionTypeConversionMap : ILSLReflectionTypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LSLReflectionTypeConversionMap"/> class.
        /// </summary>
        public LSLReflectionTypeConversionMap()
        {
            MethodReturnTypeMap = new Dictionary<Type, LSLType>();
            MethodParameterTypeMap = new Dictionary<Type, LSLType>();

            PropertyTypeMap = new Dictionary<Type, LSLType>();
            FieldTypeMap = new Dictionary<Type, LSLType>();
        }


        /// <summary>
        /// Map's <see cref="Type"/>'s used to declare method return types to <see cref="LSLType"/>'s used to define LSL library function return types.
        /// <c>null</c> is a valid value for this property, it means there are no mappings and no conversions will succeed in <see cref="ConvertReturnType"/>.
        /// The map is initialized by the class, you can use it immediately after constructing the class.
        /// </summary>
        /// 
        public Dictionary<Type, LSLType> MethodReturnTypeMap { get; set; }


        /// <summary>
        /// Map's <see cref="Type"/>'s used to declare method parameters to <see cref="LSLType"/>'s used to define LSL library function parameters.
        /// <c>null</c> is a valid value for this property, it means there are no mappings and no conversions will succeed in <see cref="ConvertParameterType"/>.
        /// The map is initialized by the class, you can use it immediately after constructing the class.
        /// </summary>
        public Dictionary<Type, LSLType> MethodParameterTypeMap { get; set; }


        /// <summary>
        /// Map's <see cref="Type"/>'s used to declare class properties to <see cref="LSLType"/>'s used to define LSL library constants.
        /// <c>null</c> is a valid value for this property, it means there are no mappings and no conversions will succeed in <see cref="ConvertPropertyType"/>.
        /// The map is initialized by the class, you can use it immediately after constructing the class.
        /// </summary>
        public Dictionary<Type, LSLType> PropertyTypeMap { get; set; }


        /// <summary>
        /// Map's <see cref="Type"/>'s used to declare class fields to <see cref="LSLType"/>'s used to define LSL library constants.
        /// <c>null</c> is a valid value for this property, it means there are no mappings and no conversions will succeed in <see cref="ConvertFieldType"/>.
        /// The map is initialized by the class, you can use it immediately after constructing the class.
        /// </summary>
        public Dictionary<Type, LSLType> FieldTypeMap { get; set; }


        /// <summary>
        /// Convert a runtime method return <see cref="Type"/> to its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="type">The runtime return <see cref="Type"/> to convert.</param>
        /// <param name="lslType">Output <see cref="LSLType"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        public bool ConvertReturnType(Type type, out LSLType lslType)
        {
            if (MethodReturnTypeMap == null)
            {
                lslType = LSLType.Void;
                return false;
            }
            return MethodReturnTypeMap.TryGetValue(type, out lslType);
        }

        /// <summary>
        /// Convert a runtime method parameter <see cref="Type"/> to its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="type">The runtime parameter <see cref="Type"/> to convert.</param>
        /// <param name="lslType">Output <see cref="LSLType"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        public bool ConvertParameterType(Type type, out LSLType lslType)
        {
            if (MethodParameterTypeMap == null)
            {
                lslType = LSLType.Void;
                return false;
            }
            return MethodParameterTypeMap.TryGetValue(type, out lslType);
        }

        /// <summary>
        /// Convert a runtime property declaration <see cref="Type"/> to its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="type">The runtime property declaration <see cref="Type"/> to convert.</param>
        /// <param name="lslType">Output <see cref="LSLType"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        public bool ConvertPropertyType(Type type, out LSLType lslType)
        {
            if (PropertyTypeMap == null)
            {
                lslType = LSLType.Void;
                return false;
            }
            return PropertyTypeMap.TryGetValue(type, out lslType);
        }

        /// <summary>
        /// Convert a runtime field declaration <see cref="Type"/> to its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="type">The runtime field declaration <see cref="Type"/> to convert.</param>
        /// <param name="lslType">Output <see cref="LSLType"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        public bool ConvertFieldType(Type type, out LSLType lslType)
        {
            if (FieldTypeMap == null)
            {
                lslType = LSLType.Void;
                return false;
            }
            return FieldTypeMap.TryGetValue(type, out lslType);
        }
    }
}