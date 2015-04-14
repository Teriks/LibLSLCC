using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Components.Interfaces;

namespace LibLSLCC.CodeValidator
{
    internal class LSLAntlrErrorHandler<T> : IAntlrErrorListener<T>
    {
        private readonly ILSLSyntaxErrorListener _errorListener;



        public LSLAntlrErrorHandler(ILSLSyntaxErrorListener errorListener)
        {
            _errorListener = errorListener;
        }



        public void SyntaxError(IRecognizer recognizer, T offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            _errorListener.GrammarLevelSyntaxError(line,charPositionInLine, msg);
        }
    }
}