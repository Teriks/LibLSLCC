using System;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.Collections;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Implements <see cref="ILSLReflectionTypeConverter"/> in memory using two dictionary objects.
    /// </summary>
    public class ILSLReflectionTypeConversionTable : ILSLReflectionTypeConverter
    {
        readonly HashMap<LSLType,Type> _mappingsFrom = new HashMap<LSLType, Type>();
        readonly HashMap<Type, LSLType> _mappingsTo = new HashMap<Type, LSLType>();

        /// <summary>
        /// Convert a runtime <see cref="Type"/> to its corresponding <see cref="LSLType"/>
        /// </summary>
        /// <param name="type">The runtime <see cref="Type"/> to convert.</param>
        /// <param name="lslType">Output <see cref="LSLType"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        public bool ConvertType(Type type, out LSLType lslType)
        {
            return _mappingsTo.TryGetValue(type, out lslType);
        }

        /// <summary>
        /// Convert an LSL <see cref="LSLType"/> to its corresponding runtime <see cref="Type"/>
        /// </summary>
        /// <param name="lslType">The LSL <see cref="LSLType"/> to convert.</param>
        /// <param name="type">Output <see cref="Type"/>.</param>
        /// <returns><c>true</c> if the conversion succeed.</returns>
        public bool ConvertType(LSLType lslType, out Type type)
        {
            return _mappingsFrom.TryGetValue(lslType, out type);
        }

        /// <summary>
        /// Add a conversion from the given <see cref="Type"/> in <paramref name="type"/> to the given <see cref="LSLType"/> in <paramref name="lslType"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that is to be convertible to the <see cref="LSLType"/> in <paramref name="lslType"/></param>
        /// <param name="lslType">The <see cref="LSLType"/> that is to be convertible from the <see cref="Type"/> in <paramref name="type"/></param>
        public void AddToEntry(Type type, LSLType lslType)
        {
            _mappingsTo.Add(type,lslType);
        }


        /// <summary>
        /// Add a conversion from the given <see cref="LSLType"/> in <paramref name="lslType"/> to the given <see cref="Type"/> in <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that is to be convertible from the <see cref="LSLType"/> in <paramref name="lslType"/></param>
        /// <param name="lslType">The <see cref="LSLType"/> that is to be convertible to the <see cref="Type"/> in <paramref name="type"/></param>
        public void AddFromEntry(LSLType lslType, Type type)
        {
            _mappingsFrom.Add(lslType, type);
        }


        /// <summary>
        /// Add a conversion from the given <see cref="Type"/> in <paramref name="type"/> to the given <see cref="LSLType"/> in <paramref name="lslType"/> and back.
        /// The conversion will work both ways, you will get a duplicate key exception if a conversion to/from entry already exist for either parameter.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that is to be convertible to and from the <see cref="LSLType"/> in <paramref name="lslType"/></param>
        /// <param name="lslType">The <see cref="LSLType"/> that is to be convertible to and from the <see cref="Type"/> in <paramref name="type"/></param>
        public void AddEntry(Type type, LSLType lslType)
        {
            AddToEntry(type,lslType);
            AddFromEntry(lslType,type);
        }


        /// <summary>
        /// Remove all type mappings that have been added to this instance.
        /// </summary>
        public void ClearEntrys()
        {
            _mappingsTo.Clear();
            _mappingsFrom.Clear();
        }
    }
}