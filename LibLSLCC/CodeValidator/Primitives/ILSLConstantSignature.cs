using System;
using LibLSLCC.LibraryData;

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    /// Read only interface for <see cref="LSLConstantSignature"/>.
    /// </summary>
    public interface ILSLConstantSignature 
    {
        /// <summary>
        ///     Returns a formated signature string for the constant, in the form:  <see cref="Name"/> = <see cref="ValueStringAsCodeLiteral"/>
        ///     Without a trailing semi-colon character.
        /// </summary>
        string SignatureString { get; }

        /// <summary>
        ///     The <see cref="LSLType" /> that the library constant is defined with.
        /// </summary>
        LSLType Type { get;}

        /// <summary>
        ///     The name of the library constant, must abide by LSL symbol naming rules or an exception will be thrown.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The value string for the library constant.
        /// </summary>
        /// <value>
        ///     The value string.
        /// </value>
        string ValueString { get; }

        /// <summary>
        ///     Returns a string which represents what this Constant would look like
        ///     if it were expanded into an LSL code literal.  This takes the Type and contents
        ///     of <see cref="ValueString"/> into account.
        /// </summary>
        string ValueStringAsCodeLiteral { get; }


        /// <summary>
        ///     Delegates to the <see cref="SignatureString"/> Property.
        /// </summary>
        /// <returns>
        ///     The <see cref="SignatureString"/> Property.
        /// </returns>
        string ToString();


        /// <summary>
        ///     Returns the hash code of the <see cref="ILSLConstantSignature"/> object.  <para/>
        ///     The <see cref="Type"/> and <see cref="Name"/> properties are used to generate the hash code.
        /// </summary>
        /// <returns>The generated hash code.</returns>
        int GetHashCode();


        /// <summary>
        ///     Determines whether the Type and Name properties of another <see cref="ILSLConstantSignature"/> equal the Type and Name
        ///     properties of this object. <para/>
        ///     If the passed object is not an <see cref="ILSLConstantSignature"/> object then the result will always be false.
        /// </summary>
        /// <param name="obj">The object to compare this object with.</param>
        /// <returns>
        ///     True if the object is an <see cref="ILSLConstantSignature"/> object and the Name and Type properties of both objects
        ///     are equal to each other.
        /// </returns>
        bool Equals(object obj);
    }
}