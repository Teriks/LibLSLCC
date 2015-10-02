using System.Text.RegularExpressions;

namespace LibLSLCC.Utility
{
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


        public static Regex GetIDRegex(bool multiMatch = false)
        {
            return new Regex("(?:"+IDRegex+")" + (multiMatch ? "*" : ""));
        }

        public static Regex GetAnyIDCharRegex(bool multiMatch = false)
        {
            return new Regex("(?:"+ IDStartCharRegex +"|"+ IDMiddleCharRegex+ ")" + (multiMatch ? "*" : ""));
        }

        public static Regex GetIDStartCharRegex(bool multiMatch = false)
        {
            return new Regex("(?:"+IDStartCharRegex + ")"+(multiMatch?"*":""));
        }

        public static Regex GetIDMiddleCharRegex(bool multiMatch = false)
        {
            return new Regex("(?:"+IDMiddleCharRegex + ")" + (multiMatch ? "*" : ""));
        }
    }
}
