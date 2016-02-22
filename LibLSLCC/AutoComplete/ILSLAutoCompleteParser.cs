using System.Collections.Generic;
using System.IO;

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    /// Interface for autocomplete parsers.
    /// </summary>
    public interface ILSLAutoCompleteParser : ILSLAutoCompleteParserState
    {
        /// <summary>
        ///     Get an enumerable of <see cref="LSLAutoCompleteLocalLabel" /> objects representing local labels
        ///     that are currently accessible at <see cref="LSLAutoCompleteParser.ParseToOffset" />.
        ///     <param name="sourceCode">The source code of the entire script.</param>
        /// </summary>
        IEnumerable<LSLAutoCompleteLocalLabel> GetLocalLabels(string sourceCode);


        /// <summary>
        ///     Get an enumerable of <see cref="LSLAutoCompleteLocalJump" /> objects representing local jump statements
        ///     that are currently accessible at <see cref="LSLAutoCompleteParser.ParseToOffset" />.
        ///     <param name="sourceCode">The source code of the entire script.</param>
        /// </summary>
        IEnumerable<LSLAutoCompleteLocalJump> GetLocalJumps(string sourceCode);


        /// <summary>
        ///     Preforms an auto-complete parse on the specified stream of LSL source code, up to an arbitrary offset.
        /// </summary>
        /// <param name="stream">The input source code stream.</param>
        /// <param name="toOffset">To offset to parse up to (the cursor offset).</param>
        void Parse(TextReader stream, int toOffset);
    }
}