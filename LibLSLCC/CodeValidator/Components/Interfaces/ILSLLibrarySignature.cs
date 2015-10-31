using System.Collections.Generic;
using LibLSLCC.Collections;
using LibLSLCC.LibraryData;

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
        LSLLibraryDataSubsetCollection Subsets { get; }


    }
}