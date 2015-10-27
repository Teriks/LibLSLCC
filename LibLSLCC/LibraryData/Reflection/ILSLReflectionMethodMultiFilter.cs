using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Allows multiple <see cref="ILSLReflectionMethodFilter"/> objects to participate in filtering/mutating method
    /// signatures de-serialized from runtime types using <see cref="LSLLibraryDataReflectionSerializer"/>.
    /// </summary>
    public class ILSLReflectionMethodMultiFilter : ILSLReflectionMethodFilter, IEnumerable<ILSLReflectionMethodFilter>
    {

        /// <summary>
        /// A modifiable collection of all <see cref="ILSLReflectionMethodFilter"/> objects participating in filtering.
        /// </summary>
        /// <value>
        /// The <see cref="ILSLReflectionMethodFilter"/>'s being used to filter.
        /// </value>
        // ReSharper disable once CollectionNeverUpdated.Global
        public List<ILSLReflectionMethodFilter> Filters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ILSLReflectionMethodMultiFilter"/> class.
        /// </summary>
        public ILSLReflectionMethodMultiFilter()
        {
            Filters= new List<ILSLReflectionMethodFilter>();
        }

        /// <summary>
        /// Allows <see cref="MethodInfo"/> objects to be prematurely filtered from de-serialization output.  Returns <c>true</c> if the <see cref="MethodInfo"/> should be filtered.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="MethodInfo"/> object we may want to filter from the output.</param>
        /// <returns><c>true</c> if the method needs to be filtered from the results.</returns>
        public bool PreFilter(LSLLibraryDataReflectionSerializer serializer, MethodInfo info)
        {
            return Filters.Any(filter => filter.PreFilter(serializer, info));
        }

        /// <summary>
        /// Allows modification a function signature after its basic information has been serialized, before its returned.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="MethodInfo"/> object the library function signature was serialized from.</param>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if the method needs to be filtered from the results.</returns>
        public bool MutateSignature(LSLLibraryDataReflectionSerializer serializer, MethodInfo info,
            LSLLibraryFunctionSignature signature)
        {
            return Filters.Any(filter => filter.MutateSignature(serializer, info, signature));
        }


        /// <summary>
        /// Add's a filter to this multi filter, this here so list initializer syntax can be utilized to build a multi filter.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        public void Add(ILSLReflectionMethodFilter filter)
        {
            Filters.Add(filter);
        }

        public IEnumerator<ILSLReflectionMethodFilter> GetEnumerator()
        {
            return Filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}