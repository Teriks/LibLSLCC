using LibLSLCC.CodeValidator.Components.Interfaces;

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLDefaultValidatorServiceProvider : ILSLValidatorServiceProvider
    {
        public LSLDefaultValidatorServiceProvider()
        {
            ExpressionValidator = new LSLDefaultExpressionValidator();

            MainLibraryDataProvider = new LSLDefaultLibraryDataProvider(false,
                LSLLibraryBaseData.StandardLsl, 
                LSLLibraryDataAdditions.None);

            SyntaxErrorListener = new LSLDefaultSyntaxErrorListener();
            SyntaxWarningListener = new LSLDefaultSyntaxWarningListener();
            StringLiteralPreProcessor = new LSLDefaultStringPreProcessor();
        }



        public ILSLExpressionValidator ExpressionValidator { get; private set; }

        public ILSLMainLibraryDataProvider MainLibraryDataProvider { get; private set; }

        public ILSLStringPreProcessor StringLiteralPreProcessor { get; private set; }

        public ILSLSyntaxErrorListener SyntaxErrorListener { get; private set; }

        public ILSLSyntaxWarningListener SyntaxWarningListener { get; private set; }
    }
}