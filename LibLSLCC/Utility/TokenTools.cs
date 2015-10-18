using System.Text.RegularExpressions;

namespace LibLSLCC.Utility
{
    /// <summary>
    /// Tools for dealing with LSL token strings, mostly symbol names.
    /// </summary>
    public class TokenTools
    {    

        /// <summary>
        /// This regex matches/validates LSL ID Tokens, IE: variable names, state names, label names, function names
        /// </summary>
        public static string IDRegex = LSLLexer.IDRegex;

        /// <summary>
        /// This regex matches/validates that a character is a valid starting character for an ID Token
        /// </summary>
        public static string IDStartCharRegex = LSLLexer.IDStartCharRegex;

        /// <summary>
        /// This regex matches/validates that a character is a valid trailing character after the first character of an ID Token
        /// </summary>
        public static string IDMiddleCharRegex = LSLLexer.IDMiddleCharRegex;

        /// <summary>
        /// Get a regex that will only match LSL symbol names, (ID's)
        /// </summary>
        /// <returns></returns>
        public static Regex GetIDRegex()
        {
            return new Regex("(?:"+IDRegex+")");
        }


        /// <summary>
        /// Get a regex that will only match any LSL symbol (ID) character, (starting characters as well as trailing characters)
        /// </summary>
        /// <param name="multiMatch">Whether or not the regex should match multiple sequential characters.</param>
        /// <returns>The constructed Regex.</returns>
        public static Regex GetAnyIDCharRegex(bool multiMatch = false)
        {
            return new Regex("(?:"+ IDStartCharRegex +"|"+ IDMiddleCharRegex+ ")" + (multiMatch ? "*" : ""));
        }


        /// <summary>
        /// Get a regex that will only match LSL symbol name start characters, (Starting characters for ID's)
        /// </summary>
        /// <param name="multiMatch">Whether or not the regex should match multiple sequential starting characters.</param>
        /// <returns>The constructed Regex.</returns>
        public static Regex GetIDStartCharRegex(bool multiMatch = false)
        {
            return new Regex("(?:"+IDStartCharRegex + ")"+(multiMatch?"*":""));
        }


        /// <summary>
        /// Get a regex that will only match LSL symbol name trailing characters, (Trailing characters after the first starting character for ID's)
        /// </summary>
        /// <param name="multiMatch">Whether or not the regex should match multiple sequential trailing characters.</param>
        /// <returns>The constructed Regex.</returns>
        public static Regex GetIDTrailingCharRegex(bool multiMatch = false)
        {
            return new Regex("(?:"+IDMiddleCharRegex + ")" + (multiMatch ? "*" : ""));
        }
    }
}
