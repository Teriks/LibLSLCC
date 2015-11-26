using System;
using LibLSLCC.Collections;

namespace LibLSLCC.CSharp
{
    public static class CSharpKeywords
    {
        public static readonly IReadOnlyHashMap<string, Type> BuiltInTypeMap = new HashMap<string, Type>()
        {
            {"bool", typeof (bool)},
            {"byte", typeof (byte)},
            {"sbyte", typeof (sbyte)},
            {"char", typeof (char)},
            {"decimal", typeof (decimal)},
            {"double", typeof (double)},
            {"float", typeof (float)},
            {"int", typeof (int)},
            {"uint", typeof (uint)},
            {"long", typeof (long)},
            {"ulong", typeof (ulong)},
            {"object", typeof (object)},
            {"short", typeof (short)},
            {"ushort", typeof (ushort)},
            {"string", typeof (string)}
        };



        /// <summary>
        /// Converts a CSharp type alias such as 'int', 'float' or object (etc..) to its corresponding <see cref="Type"/>.
        /// </summary>
        /// <param name="keywordTypeName">The keyword type alias to convert to an actual <see cref="Type"/>.</param>
        /// <returns>The type if there is a corresponding <see cref="Type"/>, otherwise <c>null</c>.</returns>
        public static Type KeywordTypetoType(string keywordTypeName)
        {
            Type t;
            if (BuiltInTypeMap.TryGetValue(keywordTypeName, out t))
            {
                return t;
            }
            return null;
        }


        /// <summary>
        /// Determines if a string is a built in type alias reference such as int.
        /// </summary>
        /// <returns><c>true</c> if the given string contains a built in type alias name; otherwise <c>false</c>.</returns>
        public static bool IsTypeAliasKeyword(string keywordTypeName)
        {
            return BuiltInTypeMap.ContainsKey(keywordTypeName);
        }



        public static readonly IReadOnlyHashedSet<string> NonContextualKeywordSet = new HashedSet<string>()
        {
            "abstract",
            "as",
            "base",
            "bool",
            "break",
            "byte",
            "case",
            "catch",
            "char",
            "checked",
            "class",
            "const",
            "continue",
            "decimal",
            "default",
            "delegate",
            "do",
            "double",
            "else",
            "enum",
            "event",
            "explicit",
            "extern",
            "false",
            "finally",
            "fixed",
            "float",
            "for",
            "foreach",
            "goto",
            "if",
            "implicit",
            "in",
            "int",
            "interface",
            "internal",
            "is",
            "lock",
            "long",
            "namespace",
            "new",
            "null",
            "object",
            "operator",
            "out",
            "override",
            "params",
            "private",
            "protected",
            "public",
            "readonly",
            "ref",
            "return",
            "sbyte",
            "sealed",
            "short",
            "sizeof",
            "stackalloc",
            "static",
            "string",
            "struct",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "uint",
            "ulong",
            "unchecked",
            "unsafe",
            "ushort",
            "using",
            "virtual",
            "void",
            "volatile",
            "while"
        };

        public static bool IsNonContextualKeyword(string str)
        {
            return NonContextualKeywordSet.Contains(str);
        }


    }
}