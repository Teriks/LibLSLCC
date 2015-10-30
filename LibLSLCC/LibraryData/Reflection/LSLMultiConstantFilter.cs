using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Allows multiple <see cref="ILSLReflectionConstantFilter"/> objects to participate in filtering/mutating constant
    /// signatures de-serialized from runtime types using <see cref="LSLLibraryDataReflectionSerializer"/>.
    /// </summary>
    public class LSLMultiConstantFilter : ILSLReflectionConstantFilter, IEnumerable<ILSLReflectionConstantFilter>
    {

        /// <summary>
        /// A modifiable collection of all <see cref="ILSLReflectionConstantFilter"/> objects participating in filtering.
        /// </summary>
        /// <value>
        /// The <see cref="ILSLReflectionConstantFilter"/>'s being used to filter.
        /// </value>
        // ReSharper disable once CollectionNeverUpdated.Global
        public List<ILSLReflectionConstantFilter> Filters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLMultiMethodFilter"/> class.
        /// </summary>
        public LSLMultiConstantFilter()
        {
            Filters = new List<ILSLReflectionConstantFilter>();
        }


        /// <summary>
        /// Add's a filter to this multi filter, this here so list initializer syntax can be utilized to build a multi filter.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        public void Add(LSLMultiConstantFilter filter)
        {
            Filters.Add(filter);
        }


        /// <summary>
        /// Allows <see cref="PropertyInfo"/> objects to be prematurely filtered from de-serialization output.  Returns <c>true</c> if the <see cref="PropertyInfo"/> should be filtered.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="PropertyInfo"/> object we may want to filter from the output.</param>
        /// <returns><c>true</c> if the property needs to be filtered from the results.</returns>
        public bool PreFilter(LSLLibraryDataReflectionSerializer serializer, PropertyInfo info)
        {
            return Filters.Any(filter => filter.PreFilter(serializer, info));
        }

        /// <summary>
        /// Allows <see cref="FieldInfo"/> objects to be prematurely filtered from de-serialization output.  Returns <c>true</c> if the <see cref="MethodInfo"/> should be filtered.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="PropertyInfo"/> object we may want to filter from the output.</param>
        /// <returns><c>true</c> if the field needs to be filtered from the results.</returns>
        public bool PreFilter(LSLLibraryDataReflectionSerializer serializer, FieldInfo info)
        {
            return Filters.Any(filter => filter.PreFilter(serializer, info));
        }

        /// <summary>
        /// Allows modification of a constant signature after its basic information has been serialized from an objects Property, before its returned.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="PropertyInfo"/> object the library constant signature was serialized from.</param>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if the constant needs to be filtered from the results.</returns>
        public bool MutateSignature(LSLLibraryDataReflectionSerializer serializer, PropertyInfo info,
            LSLLibraryConstantSignature signature)
        {
            return Filters.Any(filter => filter.MutateSignature(serializer, info, signature));
        }

        /// <summary>
        /// Allows modification of a constant signature after its basic information has been serialized from an objects Property, before its returned.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="FieldInfo"/> object the library constant signature was serialized from.</param>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if the constant needs to be filtered from the results.</returns>
        public bool MutateSignature(LSLLibraryDataReflectionSerializer serializer, FieldInfo info,
            LSLLibraryConstantSignature signature)
        {
            return Filters.Any(filter => filter.MutateSignature(serializer, info, signature));
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ILSLReflectionConstantFilter> GetEnumerator()
        {
            return Filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}