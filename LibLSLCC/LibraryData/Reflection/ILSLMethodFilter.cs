using System.Reflection;
using LibLSLCC.CodeValidator.Nodes.Interfaces;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Allows modification of a function signature after its basic information has been serialized, before its returned.
    /// </summary>
    public interface ILSLMethodFilter
    {
        /// <summary>
        /// Allows <see cref="MethodInfo"/> objects to be prematurely filtered from de-serialization output.  Returns <c>true</c> if the <see cref="MethodInfo"/> should be filtered.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="MethodInfo"/> object we may want to filter from the output.</param>
        /// <returns><c>true</c> if the method needs to be filtered from the results.</returns>
        bool PreFilter(LSLLibraryDataReflectionSerializer serializer, MethodInfo info);


        /// <summary>
        /// Allows modification a function signature after its basic information has been serialized, before its returned.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="MethodInfo"/> object the library function signature was serialized from.</param>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if the method needs to be filtered from the results.</returns>
        bool MutateSignature(LSLLibraryDataReflectionSerializer serializer, MethodInfo info, LSLLibraryFunctionSignature signature);
    }
}