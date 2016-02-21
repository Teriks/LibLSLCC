using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;

namespace LibLSLCC.LibraryData
{
    /// <summary>
    /// A minimal interface for providing data about what exists in the standard LSL library.
    /// Used primarily by <see cref="LSLCodeValidator"/>.
    /// </summary>
    /// <seealso cref="ILSLValidatorServiceProvider"/>
    public interface ILSLBasicLibraryDataProvider
    {
        /// <summary>
        ///     Return true if an event handler with the given name exists in the default library.
        /// </summary>
        /// <param name="name">Name of the event handler.</param>
        /// <returns>True if the event handler with given name exists.</returns>
        bool EventHandlerExist(string name);


        /// <summary>
        ///     Return an <see cref="LSLLibraryEventSignature"/> object describing an event handler signature;
        ///     if the event handler with the given name exists, otherwise null.
        /// </summary>
        /// <param name="name">Name of the event handler</param>
        /// <returns>
        ///     An <see cref="LSLLibraryEventSignature"/> object describing the given event handlers signature,
        ///     or null if the event handler does not exist.
        /// </returns>
        LSLLibraryEventSignature GetEventHandlerSignature(string name);


        /// <summary>
        /// Return true if a library function with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>True if the library function with given name exists.</returns>
        bool LibraryFunctionExist(string name);


        /// <summary>
        ///     Return an <see cref="LSLFunctionSignature"/> list with the overload signatures of a function with the given name.
        ///     If the function does not exist, null is returned.  If the function exist but is not overloaded only a single item will be returned.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>
        ///     An <see cref="LSLFunctionSignature"/> list object describing the given library functions signatures,
        ///     or null if the library function does not exist.
        /// </returns>
        IReadOnlyGenericArray<LSLLibraryFunctionSignature> GetLibraryFunctionSignatures(string name);


        /// <summary>
        /// Return a <see cref="LSLLibraryFunctionSignature"/> from this object where <see cref="LSLFunctionSignature.SignatureEquivalent"/> is true for the given <see cref="LSLFunctionSignature"/>,
        /// or null if no such <see cref="LSLLibraryFunctionSignature"/> exists in this provider.
        /// </summary>
        /// <remarks>
        /// see: <see cref="LSLFunctionSignature.SignatureEquivalent"/>
        /// </remarks>
        /// <param name="signatureToTest">The signature to use as search criteria.</param>
        /// <returns>
        /// An <see cref="LSLFunctionSignature"/> which has the same signature of signatureToTest, or null if none exist.
        /// </returns>
        LSLLibraryFunctionSignature GetLibraryFunctionSignature(LSLFunctionSignature signatureToTest);


        /// <summary>
        ///     Return true if a library constant with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library constant.</param>
        /// <returns>True if a library constant with the given name exists.</returns>
        bool LibraryConstantExist(string name);


        /// <summary>
        ///     Return an <see cref="LSLLibraryConstantSignature"/> object describing the signature of a library constant
        /// </summary>
        /// <param name="name">Name of the library constant</param>
        /// <returns>
        ///     An <see cref="LSLLibraryConstantSignature"/> object describing the given constants signature,
        ///     or null if the constant is not defined
        /// </returns>
        LSLLibraryConstantSignature GetLibraryConstantSignature(string name);
    }
}