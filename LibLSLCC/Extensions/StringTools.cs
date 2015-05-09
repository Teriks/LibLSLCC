namespace LibLSLCC.Extensions
{
    public static class StringTools
    {

        /// <summary>
        /// Gets the number of spaces required to match the length of the whitespace leading up to the first non-whitespace
        /// character in a string (new line is not considered whitespace here).
        /// </summary>
        /// <param name="str">The string to consider</param>
        /// <param name="tabSize">The size of a tab character in spaces</param>
        /// <returns>The number of space characters required to match the length of all the whitespace characters at the end of the string (except newlines)</returns>
        public static int GetStringSpacesIndented(this string str, int tabSize=4)
        {
            int columns = 0;

            foreach (char t in str)
            {
                if (char.IsWhiteSpace(t))
                {
                    if (t == '\t')
                    {
                        columns += 4;
                    }
                    else if (t == ' ')
                    {
                        columns++;
                    }
                }
                else
                {
                    break;
                }
            }
            return columns;
        }

        /// <summary>
        /// Gets the number of spaces required to exactly match the length of a given string up to the first new line
        /// </summary>
        /// <param name="str">Input string to get the length in spaces of</param>
        /// <param name="tabSize">Tab size in spaces, defaults to 4</param>
        /// <returns>Number of spaces required to match the length of the string</returns>
        public static int GetStringSpacesEquivalent(this string str, int tabSize = 4)
        {
            if (str.Length == 0) return 0;

            int columns = 0;

            for (int index = 0; index < str.Length; index++)
            {
                char t = str[index];
                if (char.IsWhiteSpace(t))
                {
                    if (t == '\t')
                    {
                        columns += tabSize;
                    }
                    else if (t == ' ')
                    {
                        columns++;
                    }
                }
                else if (char.IsDigit(t) || char.IsLetter(t) || char.IsSymbol(t) || char.IsPunctuation(t))
                {
                    columns += 1;
                }
                else if (index + 1 < str.Length && char.IsHighSurrogate(t) && char.IsLowSurrogate(str[index + 1]))
                {
                    columns += 1;
                    index++;
                }
                else if (t == '\n')
                {
                    break;
                }
            }
            return columns;
        }


        /// <summary>
        /// Creates a spacer string using tabs up until spaces are required for alignment.
        /// Strings less than tabSize end up being only spaces.
        /// </summary>
        /// <param name="spaces">The number of spaces the spacer string should be equivalent to</param>
        /// <param name="tabSize">The size of a tab character in spaces, default value is 4</param>
        /// <returns>
        /// A string consisting of leading tabs and possibly trailing spaces that is equivalent in length 
        /// to the number of spaces provided in the spaces parameter</returns>
        public static string CreateTabCorrectSpaceString(int spaces, int tabSize = 4)
        {
            string space = "";
            int actual = 0;
            for (int i = 0; i < (spaces / tabSize); i++)
            {
                space += '\t';
                actual += tabSize;
            }

            while (actual < spaces)
            {
                space += ' ';
                actual++;
            }


            return space;
        }


        public static string CreateRepeatingString(int repeats, string content)
        {
            string r = "";
            for (int i = 0; i < repeats; i++) r += content;
            return r;
        }

        /// <summary>
        /// Generate a string with N number of spaces in it
        /// </summary>
        /// <param name="spaces">Number of spaces</param>
        /// <returns>A string containing 'spaces' number of spaces</returns>
        public static string CreateSpacesString(int spaces)
        {
            return CreateRepeatingString(spaces, " ");
        }


        /// <summary>
        /// Generate a string with N number of tabs in it
        /// </summary>
        /// <param name="tabs">Number of tabs</param>
        /// <returns>A string containing 'tabs' number of tabs</returns>
        public static string CreateTabsString(int tabs)
        {
            return CreateRepeatingString(tabs, "\t");
        }



        /// <summary>
        /// Generate a string with N number of newlines in it
        /// </summary>
        /// <param name="newLines">Number of newlines</param>
        /// <returns>A string containing 'newLines' number of newlines</returns>
        public static string CreateNewLinesString(int newLines)
        {
            return CreateRepeatingString(newLines, "\n");
        }
    }
}