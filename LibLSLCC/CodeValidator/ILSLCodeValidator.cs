using System.IO;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

namespace LibLSLCC.CodeValidator
{
    public interface ILSLCodeValidator
    {
        /// <summary>
        ///     Set to true if the last call to validate revealed syntax errors and returned null
        /// </summary>
        bool HasSyntaxErrors { get; }

        /// <summary>
        ///     Validates the code content of a stream and returns the top of the compilation unit syntax tree as a
        ///     LSLCompilationUnitNode object, if parsing resulted in syntax errors the result will be null
        /// </summary>
        /// <param name="stream">The TextReader to parse code from</param>
        /// <returns>Top level node of an LSL syntax tree</returns>
        ILSLCompilationUnitNode Validate(TextReader stream);
    }
}