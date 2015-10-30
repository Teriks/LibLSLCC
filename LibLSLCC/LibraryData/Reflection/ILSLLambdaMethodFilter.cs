using System;
using System.Reflection;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Implements <see cref="ILSLMethodFilter"/> using function objects.
    /// </summary>
    public class ILSLLambdaMethodFilter : ILSLMethodFilter
    {

        /// <summary>
        /// The function used to implement <see cref="PreFilter"/>.  If <c>null</c> no filtering will occur (everything will be allowed).
        /// </summary>
        public Func<LSLLibraryDataReflectionSerializer, MethodInfo, bool> PreFilterFunction { get; set; }


        /// <summary>
        /// The function used to implement <see cref="MutateSignature"/>.  If <c>null</c> no filtering will occur (everything will be allowed) and no de-serialized method signatures will be mutated.
        /// </summary>
        public Func<LSLLibraryDataReflectionSerializer, MethodInfo, LSLLibraryFunctionSignature, bool> MutateSignatureFunction { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ILSLLambdaMethodFilter"/> class.
        /// </summary>
        public ILSLLambdaMethodFilter()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ILSLLambdaMethodFilter"/> class.
        /// </summary>
        /// <param name="preFilterFunction">The pre filter function.</param>
        public ILSLLambdaMethodFilter(Func<LSLLibraryDataReflectionSerializer, MethodInfo, bool> preFilterFunction)
        {
            PreFilterFunction = preFilterFunction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ILSLLambdaMethodFilter"/> class.
        /// </summary>
        /// <param name="mutateSignatureFunction">The mutate signature function.</param>
        public ILSLLambdaMethodFilter(Func<LSLLibraryDataReflectionSerializer, MethodInfo, LSLLibraryFunctionSignature, bool> mutateSignatureFunction)
        {
            MutateSignatureFunction = mutateSignatureFunction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ILSLLambdaMethodFilter"/> class.
        /// </summary>
        /// <param name="preFilterFunction">The pre filter function.</param>
        /// <param name="mutateSignatureFunction">The mutate signature function.</param>
        public ILSLLambdaMethodFilter(Func<LSLLibraryDataReflectionSerializer, MethodInfo, bool> preFilterFunction, Func<LSLLibraryDataReflectionSerializer, MethodInfo, LSLLibraryFunctionSignature, bool> mutateSignatureFunction) : this(preFilterFunction)
        {
            MutateSignatureFunction = mutateSignatureFunction;
        }

        /// <summary>
        /// Allows <see cref="MethodInfo"/> objects to be prematurely filtered from de-serialization output.  Returns <c>true</c> if the <see cref="MethodInfo"/> should be filtered.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="MethodInfo"/> object we may want to filter from the output.</param>
        /// <returns><c>true</c> if the method needs to be filtered from the results.</returns>
        public bool PreFilter(LSLLibraryDataReflectionSerializer serializer, MethodInfo info)
        {
            return PreFilterFunction != null && PreFilterFunction(serializer,info);
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
            return MutateSignatureFunction != null && MutateSignatureFunction(serializer, info, signature);
        }
    }
}