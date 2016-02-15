using System;

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    /// <summary>
    /// Extensions for ILSLValidatorServiceProvider
    /// </summary>
    public static class ILSLValidatorServiceProvideExtensions
    {
        /// <summary>
        /// Returns true if all service provider properties are non null
        /// </summary>
        /// <param name="provider">The <see cref="ILSLValidatorServiceProvider"/> to check.</param>
        /// <returns>True if all properties are initialized.</returns>
        public static bool IsComplete(this ILSLValidatorServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }


            return provider.ExpressionValidator != null
                   && provider.LibraryDataProvider != null
                   && provider.StringLiteralPreProcessor != null
                   && provider.SyntaxErrorListener != null
                   && provider.SyntaxWarningListener != null;
        }


        /// <summary>
        /// Returns true if all service provider properties are non null
        /// </summary>
        /// <param name="provider">The <see cref="ILSLValidatorServiceProvider"/> to check.</param>
        /// <param name="describeNulls">A string describing which properties are null if <see cref="IsComplete"/> returns <c>false</c></param>
        /// <returns>True if all properties are initialized.</returns>
        public static bool IsComplete(this ILSLValidatorServiceProvider provider, out string describeNulls)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }


            string nullProps = string.Empty;

            if (provider.ExpressionValidator == null)
            {
                nullProps += "ILSLValidatorServiceProvider.ExpressionValidator is null." + Environment.NewLine;
            }
            if (provider.LibraryDataProvider == null)
            {
                nullProps += "ILSLValidatorServiceProvider.LibraryDataProvider is null." + Environment.NewLine;
            }
            if (provider.SyntaxErrorListener == null)
            {
                nullProps += "ILSLValidatorServiceProvider.SyntaxErrorListener is null." + Environment.NewLine;
            }
            if (provider.SyntaxWarningListener == null)
            {
                nullProps += "ILSLValidatorServiceProvider.SyntaxWarningListener is null.";
            }

            describeNulls = nullProps == string.Empty ? null : nullProps;

            return describeNulls == null;
        }
    }
}