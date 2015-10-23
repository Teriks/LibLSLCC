using System.Text.RegularExpressions;
using LibLSLCC.Parser;

namespace LibLSLCC.Utility
{
    /// <summary>
    /// Tools for dealing with LSL token strings, mostly symbol names.
    /// </summary>
    public class LSLTokenTools
    {    

        /// <summary>
        /// This regex matches/validates LSL ID Tokens, IE: variable names, state names, label names, function names
        /// </summary>
        public static Regex IDRegex = new Regex("(?:" + LSLLexer.IDRegex + ")");

        /// <summary>
        /// This regex matches/validates LSL ID Tokens, IE: variable names, state names, label names, function names
        /// It is anchored with ^ and $ at the beginning and end respectively.
        /// </summary>
        public static Regex IDRegexAnchored = new Regex("^(?:" + LSLLexer.IDRegex + ")$");

        /// <summary>
        /// This regex matches/validates that a character is a valid starting character for an ID Token
        /// </summary>
        public static Regex IDStartCharRegex = new Regex("(?:" + LSLLexer.IDStartCharRegex +")");

        /// <summary>
        /// This regex matches/validates that a character is a valid trailing character after the first character of an ID Token
        /// </summary>
        public static Regex IDTrailingCharRegex = new Regex("(?:" + LSLLexer.IDTrailingCharRegex + ")");



        /// <summary>
        /// This regex matches/validates that a character is either a valid starting OR trailing character in an ID token
        /// </summary>
        public static Regex IDAnyCharRegex = new Regex("(?:" + LSLLexer.IDStartCharRegex + "|"+ LSLLexer.IDTrailingCharRegex +")");


    }
}
