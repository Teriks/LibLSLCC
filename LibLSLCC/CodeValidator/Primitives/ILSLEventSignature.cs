using System;
using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    /// A read only interface for <see cref="LSLEventSignature"/>.
    /// </summary>
    public interface ILSLEventSignature
    {
        /// <summary>
        ///     The number of parameters the event handler signature has
        /// </summary>
        int ParameterCount { get; }

        /// <summary>
        ///     The event handlers name, must follow LSL symbol naming conventions
        /// </summary>
        /// <exception cref="LSLInvalidSymbolNameException" accessor="set">
        ///     Thrown if the event handler name does not follow LSL symbol naming conventions.
        /// </exception>
        string Name { get; }

        /// <summary>
        ///     Indexable list of objects describing the event handlers parameters
        /// </summary>
        IReadOnlyGenericArray<LSLParameterSignature> Parameters { get; }

        /// <summary>
        ///     Returns a formated signature string for the <see cref="ILSLEventSignature" />.  This does not include a trailing
        ///     semi-colon.
        ///     An example would be: listen(integer channel, string name, key id, string message)
        /// </summary>
        string SignatureString { get; }


        /// <summary>
        ///     Delegates to SignatureString
        /// </summary>
        /// <returns>SignatureString</returns>
        string ToString();


        /// <summary>
        ///     Determines if two event handler signatures match exactly, parameter names do not matter but parameter
        ///     types do.
        /// </summary>
        /// <param name="otherSignature">The other event handler signature to compare to.</param>
        /// <returns>True if the two signatures are identical.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="otherSignature"/> is <c>null</c>.</exception>
        bool SignatureMatches(ILSLEventSignature otherSignature);


        /// <summary>
        ///     <see cref="GetHashCode"/> uses the name of the <see cref="ILSLEventSignature" /> and the LSL Types of the parameters. <para/>
        ///     This means the Hash Code is determined by the event name, and the Types of all its parameters. <para/>
        ///     Inherently, uniqueness is also determined by the number of parameters.
        /// </summary>
        /// <returns>Hash code for this <see cref="ILSLEventSignature" /></returns>
        int GetHashCode();


        /// <summary>
        ///     <see cref="Equals(object)"/> delegates to <see cref="ILSLEventSignature.SignatureMatches" />
        /// </summary>
        /// <param name="obj">The other event signature</param>
        /// <returns>Equality</returns>
        bool Equals(object obj);
    }
}