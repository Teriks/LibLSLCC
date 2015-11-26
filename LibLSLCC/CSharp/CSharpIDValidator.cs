using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LibLSLCC.CSharp
{
    static class CSharpIDValidator
    {
        private const string TrailingCharacter =
            @"\u0041-\u005A\u00C0-\u00DE\u0061-\u007A\u01C5\u01C8\u01CB\u01F2\u02B0-\u02EE\u01BB\u01C0\u01C1\u01C2\u01C3\u0294\u16EE\u16EF\u16F0\u2160\u2161\u2162\u2163\u2164\u2165\u2166\u2167\u2168\u2169\u216A\u216B\u216C\u216D\u216E\u216F";

        private const string StartingCharacter = "_" + TrailingCharacter;


        private static readonly Regex StartingCharacterGroup = new Regex("[" + StartingCharacter + "]");

        private static readonly Regex TrailingCharacterGroup = new Regex("[" + TrailingCharacter + "]");

        private static readonly Regex Id = new Regex("^[" + StartingCharacter + "][" + TrailingCharacter + "]*$");

        public static bool IsStartingCharacter(char c)
        {
            return StartingCharacterGroup.IsMatch("" + c);
        }

        public static bool IsTrailingCharacter(char c)
        {
            return TrailingCharacterGroup.IsMatch("" + c);
        }

        private static readonly object CompilerLock = new object();



        public static bool IsValidIdentifier(string str)
        {
            using (var compiler = CodeDomProvider.CreateProvider("CSharp"))
            {
                return compiler.IsValidIdentifier(str);
            }
        }
    }
}
