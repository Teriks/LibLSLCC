using System;
using System.Reflection;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Implements <see cref="ILSLReflectionConstantFilter"/> using function objects.
    /// </summary>
    public class LSLReflectionLambdaConstantFilter : ILSLReflectionConstantFilter
    {

        /// <summary>
        /// The function used to implement <see cref="PreFilter(LSLLibraryDataReflectionSerializer,PropertyInfo)"/>.  
        /// If its <c>null</c> nothing will be filtered (everything will be allowed).
        /// </summary>
        public Func<LSLLibraryDataReflectionSerializer, PropertyInfo, bool> PreFilterPropertyConstant { get; set; }

        /// <summary>
        /// The function used to implement <see cref="MutateSignature(LSLLibraryDataReflectionSerializer,PropertyInfo,LSLLibraryConstantSignature)"/>.  
        /// If its <c>null</c> nothing will be filtered (everything will be allowed) and no de-serialized constants derived from fields will be mutated.
        /// </summary>
        public Func<LSLLibraryDataReflectionSerializer, PropertyInfo, LSLLibraryConstantSignature, bool> MutatePropertyConstant { get; set; }

        /// <summary>
        /// The function used to implement <see cref="PreFilter(LSLLibraryDataReflectionSerializer,FieldInfo)"/>.  
        /// If its <c>null</c> nothing will be filtered (everything will be allowed).
        /// </summary>
        public Func<LSLLibraryDataReflectionSerializer, FieldInfo, bool> PreFilterFieldConstant { get; set; }

        /// <summary>
        /// The function used to implement <see cref="MutateSignature(LSLLibraryDataReflectionSerializer,FieldInfo,LSLLibraryConstantSignature)"/>.  
        /// If its <c>null</c> nothing will be filtered (everything will be allowed) and no de-serialized constants derived from fields will be mutated.
        /// </summary>
        public Func<LSLLibraryDataReflectionSerializer, FieldInfo, LSLLibraryConstantSignature, bool> MutateFieldConstant { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="LibLSLCC.LibraryData.Reflection.LSLReflectionLambdaConstantFilter"/> class.
        /// </summary>
        public LSLReflectionLambdaConstantFilter()
        {
        }

        /// <summary>
        /// Allows <see cref="PropertyInfo"/> objects to be prematurely filtered from de-serialization output.  Returns <c>true</c> if the <see cref="PropertyInfo"/> should be filtered.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="PropertyInfo"/> object we may want to filter from the output.</param>
        /// <returns><c>true</c> if the property needs to be filtered from the results.</returns>
        public bool PreFilter(LSLLibraryDataReflectionSerializer serializer, PropertyInfo info)
        {
            return PreFilterPropertyConstant != null && PreFilterPropertyConstant(serializer, info);
        }

        /// <summary>
        /// Allows <see cref="FieldInfo"/> objects to be prematurely filtered from de-serialization output.  Returns <c>true</c> if the <see cref="MethodInfo"/> should be filtered.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="PropertyInfo"/> object we may want to filter from the output.</param>
        /// <returns><c>true</c> if the field needs to be filtered from the results.</returns>
        public bool PreFilter(LSLLibraryDataReflectionSerializer serializer, FieldInfo info)
        {
            return PreFilterFieldConstant != null && PreFilterFieldConstant(serializer, info);
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
            return MutatePropertyConstant != null && MutatePropertyConstant(serializer, info, signature);
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
            return MutateFieldConstant != null && MutateFieldConstant(serializer, info, signature);
        }
    }
}