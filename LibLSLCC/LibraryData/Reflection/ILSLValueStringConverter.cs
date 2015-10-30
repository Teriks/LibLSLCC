using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Interface for converting property/field values into strings assignable to <see cref="LSLLibraryConstantSignature.ValueString"/>.
    /// This interface is for types that are assigned to the attribute property <see cref="LSLConstantAttribute.ValueStringConverter"/> using the <c>typeof</c> operator.
    /// </summary>
    public interface ILSLValueStringConverter
    {
        /// <summary>
        /// Convert the value taken from a property or field with the <see cref="LSLConstantAttribute"/> into
        /// something that is valid to assign to <see cref="LSLLibraryConstantSignature.ValueString"/> given the specified
        /// <see cref="LSLType"/> that is to be assigned to <see cref="LSLLibraryConstantSignature.Type"/>.
        /// </summary>
        /// <param name="constantType">The <see cref="LSLType"/> being assigned to <see cref="LSLLibraryConstantSignature.Type"/>.</param>
        /// <param name="fieldValue">The value taking from the property or field with an <see cref="LSLConstantAttribute"/>.</param>
        /// <param name="valueString">
        /// The string to assign to <see cref="LSLLibraryConstantSignature.ValueString"/>.
        /// this should be a string that <see cref="LSLLibraryConstantSignature"/> is able to parse for the given <see cref="LSLType"/>.
        /// You should not assign <c>null</c> to <paramref name="valueString"/> if you intend to return <c>true</c>, this is invalid and the serializer will throw an exception.
        /// </param>
        /// <returns>
        /// True if the conversion succeeded, false if it did not.
        /// </returns>
        bool Convert(LSLType constantType, object fieldValue, out string valueString);
    }
}