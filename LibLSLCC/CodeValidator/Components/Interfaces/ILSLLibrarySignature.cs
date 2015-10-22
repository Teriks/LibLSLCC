using System.Collections.Generic;
using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    /// <summary>
    /// Interface for the common properties shared by LSLLibrary*Signature objects.
    /// </summary>
    public interface ILSLLibrarySignature
    {
        /// <summary>
        /// Whether or not this library signature is marked as deprecated or not.
        /// </summary>
        bool Deprecated { get; set; }

        /// <summary>
        /// Returns the documentation string attached to this library signature.
        /// </summary>
        string DocumentationString { get; set; }

        /// <summary>
        /// Additional dynamic property values that can be attached to the constant signature and parsed from XML
        /// </summary>
        IDictionary<string, string> Properties { get; }

        /// <summary>
        /// Returns a formated string containing the signature and documentation for this library signature.
        /// It consists of the SignatureString followed by a semi-colon, and then followed by a new-line and DocumentationString
        /// if the documentation string is not null.
        /// </summary>
        string SignatureAndDocumentation { get; }


        /// <summary>
        /// The library subsets this signature belongs to/is shared among.
        /// </summary>
        IReadOnlyHashedSet<string> Subsets { get; }


        /// <summary>
        /// Adds to the current library subsets this signature belongs to by parsing them out of a comma separated string of names.
        /// </summary>
        /// <param name="subsets">A comma separated list of subset names in a string to add.</param>
        void AddSubsets(string subsets);


        /// <summary>
        /// Sets the library subsets this signature belongs to.
        /// </summary>
        /// <param name="subsets">An enumerable of subset name strings</param>
        void AddSubsets(IEnumerable<string> subsets);


        /// <summary>
        /// Sets the library subsets this signature belongs to by parsing them out of a comma separated string of names.
        /// </summary>
        /// <param name="subsets">A comma separated list of subset names in a string.</param>
        void SetSubsets(string subsets);

        /// <summary>
        /// Sets the library subsets this signature belongs to.
        /// </summary>
        /// <param name="subsets">An enumerable of subset name strings</param>
        void SetSubsets(IEnumerable<string> subsets);
    }
}