using System;

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    /// <summary>
    ///     Represents various sub strategies and listeners that are used in LSLCodeValidator's implementation
    /// </summary>
    public interface ILSLValidatorServiceProvider
    {
        ILSLExpressionValidator ExpressionValidator { get; }
        ILSLMainLibraryDataProvider MainLibraryDataProvider { get; }
        ILSLStringPreProcessor StringLiteralPreProcessor { get; }
        ILSLSyntaxErrorListener SyntaxErrorListener { get; }
        ILSLSyntaxWarningListener SyntaxWarningListener { get; }
    }

    public static class ILSLValidatorServiceProvideExtensions
    {
        /// <summary>
        ///     Returns true if all service provider properties are non null
        /// </summary>
        /// <param name="provider">The ILSLValidatorServiceProvider to check</param>
        /// <returns>True if all properties are initialized</returns>
        public static bool IsComplete(this ILSLValidatorServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }


            return provider.ExpressionValidator != null
                   && provider.MainLibraryDataProvider != null
                   && provider.StringLiteralPreProcessor != null
                   && provider.SyntaxErrorListener != null
                   && provider.SyntaxWarningListener != null;
        }
    }
}