using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Primitives;

namespace LibLSLCC.CodeValidator
{
    internal class LSLAntlrErrorHandler : IAntlrErrorListener<IToken>
    {
        private static readonly Regex NonLValueAssignmentError =
            new Regex("mismatched input '[*+-/%]?=' expecting {(.*?)}");

        private readonly ILSLSyntaxErrorListener _errorListener;

        public LSLAntlrErrorHandler(ILSLSyntaxErrorListener errorListener)
        {
            _errorListener = errorListener;
        }

        public bool HasErrors { get; private set; }

        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            HasErrors = true;

            var m = NonLValueAssignmentError.Match(msg);

            var expected = new HashSet<string>(m.Groups[1].ToString().Split(',').Select(x=>x.Trim('\'', ' ')));

            if (m.Success && expected.Contains("*") && !(
                   expected.Contains("TYPE")
                || expected.Contains("ID")
                || expected.Contains("INT")
                || expected.Contains("FLOAT")
                || expected.Contains("HEX_LITERAL")
                || expected.Contains("QUOTED_STRING")
                ))
            {
                _errorListener.AssignmentToUnassignableExpression(new LSLSourceCodeRange(offendingSymbol), offendingSymbol.Text);
            }
            else
            {
                _errorListener.GrammarLevelParserSyntaxError(line, charPositionInLine, new LSLSourceCodeRange(offendingSymbol), offendingSymbol.Text, msg);
            }
        }
    }
}