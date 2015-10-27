using System.Reflection;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Allows modification of a constant signature after its basic information has been serialized, before its returned.
    /// </summary>
    public interface ILSLReflectionConstantFilter
    {

        /// <summary>
        /// Allows <see cref="PropertyInfo"/> objects to be prematurely filtered from de-serialization output.  Returns <c>true</c> if the <see cref="PropertyInfo"/> should be filtered.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="PropertyInfo"/> object we may want to filter from the output.</param>
        /// <returns><c>true</c> if the property needs to be filtered from the results.</returns>
        bool PreFilter(LSLLibraryDataReflectionSerializer serializer, PropertyInfo info);


        /// <summary>
        /// Allows <see cref="FieldInfo"/> objects to be prematurely filtered from de-serialization output.  Returns <c>true</c> if the <see cref="MethodInfo"/> should be filtered.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="PropertyInfo"/> object we may want to filter from the output.</param>
        /// <returns><c>true</c> if the field needs to be filtered from the results.</returns>
        bool PreFilter(LSLLibraryDataReflectionSerializer serializer, FieldInfo info);


        /// <summary>
        /// Allows modification of a constant signature after its basic information has been serialized from an objects Property, before its returned.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="PropertyInfo"/> object the library constant signature was serialized from.</param>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if the constant needs to be filtered from the results.</returns>
        bool MutateSignature(LSLLibraryDataReflectionSerializer serializer, PropertyInfo info, LSLLibraryConstantSignature signature);


        /// <summary>
        /// Allows modification of a constant signature after its basic information has been serialized from an objects Property, before its returned.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="FieldInfo"/> object the library constant signature was serialized from.</param>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if the constant needs to be filtered from the results.</returns>
        bool MutateSignature(LSLLibraryDataReflectionSerializer serializer, FieldInfo info, LSLLibraryConstantSignature signature);
    }
}