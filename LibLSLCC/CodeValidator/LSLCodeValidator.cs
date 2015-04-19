using System;
using System.IO;
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.Visitor;

namespace LibLSLCC.CodeValidator
{
    public class LSLCodeValidator : ILSLCodeValidator
    {
        public ILSLValidatorServiceProvider ValidatorServices { get; private set; }
        private readonly LSLAntlrErrorHandler<int> _antlrLexerErrorHandler;
        private readonly LSLAntlrErrorHandler<IToken> _antlrParserErrorHandler;
        private readonly LSLCodeValidationVisitor _validationVisitor;



        public LSLCodeValidator(ILSLValidatorServiceProvider validatorServices)
        {
            ValidatorServices = validatorServices;
            if (!validatorServices.IsComplete())
            {
                throw new ArgumentException("An ILSLValidatorServiceProvider property was null", "validatorServices");
            }

            _validationVisitor = new LSLCodeValidationVisitor(validatorServices);
            _antlrLexerErrorHandler = new LSLAntlrErrorHandler<int>(validatorServices.SyntaxErrorListener);
            _antlrParserErrorHandler = new LSLAntlrErrorHandler<IToken>(validatorServices.SyntaxErrorListener);
        }



        public LSLCodeValidator()
        {
            _validationVisitor = new LSLCodeValidationVisitor();
            _antlrLexerErrorHandler = new LSLAntlrErrorHandler<int>(_validationVisitor.SyntaxErrorListener);
            _antlrParserErrorHandler = new LSLAntlrErrorHandler<IToken>(_validationVisitor.SyntaxErrorListener);
        }



        /// <summary>
        ///     Set to true if the last call to validate revealed syntax errors and returned null
        /// </summary>
        public bool HasSyntaxErrors { get; private set; }



        /// <summary>
        ///     Validates the code content of a stream and returns the top of the compilation unit syntax tree as a
        ///     LSLCompilationUnitNode object, if parsing resulted in syntax errors the result will be null
        /// </summary>
        /// <param name="stream">The TextReader to parse code from</param>
        /// <returns>Top level node of an LSL syntax tree</returns>
        public ILSLCompilationUnitNode Validate(TextReader stream)
        {
            HasSyntaxErrors = false;
            var inputStream = new AntlrInputStream(stream);

            var lexer = new LSLLexer(inputStream);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(_antlrLexerErrorHandler);

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new LSLParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(_antlrParserErrorHandler);

            var parseTree = parser.compilationUnit();


            if (parser.NumberOfSyntaxErrors > 0)
            {
                HasSyntaxErrors = true;
                return null;
            }

            var r = _validationVisitor.ValidateAndBuildTree(parseTree);

            if (r.HasErrors)
            {
                HasSyntaxErrors = true;
                return null;
            }

            r.Comments = lexer.Comments;

            return r;
        }
    }
}