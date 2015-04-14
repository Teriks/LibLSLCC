using System.Collections.Generic;

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    /// <summary>
    ///     An interface for a strategy that provides data about the standard LSL library to LSLCodeValidator
    /// </summary>
    public interface ILSLMainLibraryDataProvider
    {
        /// <summary>
        ///     Enumerable of event handlers supported according to this data provider
        /// </summary>
        IEnumerable<LSLLibraryEventSignature> SupportedEventHandlers { get; }


        /// <summary>
        ///     Enumerable of the LibraryFunctions defined according to this data provider
        /// </summary>
        IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LibraryFunctions { get; }


        /// <summary>
        ///     Enumerable of the LibraryConstants defined according to this data provider
        /// </summary>
        IEnumerable<LSLLibraryConstantSignature> LibraryConstants { get; }



        /// <summary>
        ///     Return true if an event handler with the given name exists in the default library.
        /// </summary>
        /// <param name="name">Name of the event handler.</param>
        /// <returns>True if the event handler with given name exists.</returns>
        bool EventHandlerExist(string name);



        /// <summary>
        ///     Return an LSLEventHandlerSignature object describing an event handler signature;
        ///     if the event handler with the given name exists, otherwise null.
        /// </summary>
        /// <param name="name">Name of the event handler</param>
        /// <returns>
        ///     An LSLEventHandlerSignature object describing the given event handlers signature,
        ///     or null if the event handler does not exist.
        /// </returns>
        LSLLibraryEventSignature GetEventHandlerSignature(string name);



        /// <summary>
        ///     Return true if a library function with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>True if the library function with given name exists.</returns>
        bool LibraryFunctionExist(string name);



        /// <summary>
        ///     Return an LSLFunctionSignature list object describing the function call signatures of a library function;
        ///     if the function with the given name exists as a singular or overloaded function, otherwise null.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>
        ///     An LSLFunctionSignature list object describing the given library functions signatures,
        ///     or null if the library function does not exist.
        /// </returns>
        IReadOnlyList<LSLLibraryFunctionSignature> GetLibraryFunctionSignatures(string name);



        /// <summary>
        ///     Return true if a library constant with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library constant.</param>
        /// <returns>True if a library constant with the given name exists.</returns>
        bool LibraryConstantExist(string name);



        /// <summary>
        ///     Return an LSLLibraryConstantSignature object describing the signature of a library constant
        /// </summary>
        /// <param name="name">Name of the library constant</param>
        /// <returns>
        ///     An LSLLibraryConstantSignature object describing the given constants signature,
        ///     or null if the constant is not defined
        /// </returns>
        LSLLibraryConstantSignature GetLibraryConstantSignature(string name);
    }
}