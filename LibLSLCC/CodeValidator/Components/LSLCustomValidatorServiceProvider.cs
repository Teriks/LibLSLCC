using LibLSLCC.CodeValidator.Components.Interfaces;

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    ///     An ILSLValidatorServiceProvider implementation that allows you to assign values
    ///     to members which will be used directly as part of LSLCodeValidator's implementation
    /// </summary>
    public class LSLCustomValidatorServiceProvider : ILSLValidatorServiceProvider
    {
        public ILSLExpressionValidator ExpressionValidator { get; set; }

        public ILSLMainLibraryDataProvider MainLibraryDataProvider { get; set; }

        public ILSLStringPreProcessor StringLiteralPreProcessor { get; set; }

        public ILSLSyntaxErrorListener SyntaxErrorListener { get; set; }

        public ILSLSyntaxWarningListener SyntaxWarningListener { get; set; }
    }
}