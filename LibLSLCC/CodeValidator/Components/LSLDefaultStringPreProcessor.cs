using System.Collections.Generic;
using System.Globalization;
using System.Text;
using LibLSLCC.CodeValidator.Components.Interfaces;

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLDefaultStringPreProcessor : ILSLStringPreProcessor
    {
        private readonly List<LSLStringCharacterError> _illegalCharacters = new List<LSLStringCharacterError>();
        private readonly List<LSLStringCharacterError> _invalidEscapeCodes = new List<LSLStringCharacterError>();



        public LSLDefaultStringPreProcessor()
        {
            HasErrors = false;
            Result = "";
        }



        public bool HasErrors { get; private set; }

        public IEnumerable<LSLStringCharacterError> InvalidEscapeCodes
        {
            get { return _invalidEscapeCodes; }
        }

        public IEnumerable<LSLStringCharacterError> IllegalCharacters
        {
            get { return _illegalCharacters; }
        }

        public string Result { get; private set; }



        public void ProcessString(string stringLiteral)
        {
            var result = new StringBuilder();


            var itr = CStringUnescape(stringLiteral);


            foreach (var str in itr)
            {
                result.Append(str);
            }


            Result = HasErrors ? "" : result.ToString();
        }



        public void Reset()
        {
            _invalidEscapeCodes.Clear();
            _illegalCharacters.Clear();
            HasErrors = false;
            Result = "";
        }



        private IEnumerable<string> CStringUnescape(string str)
        {
            var sequentialEscapes = 0;

            for (var i = 0; i < str.Length;)
            {
                var chr = str[i];

                if (chr == '\\' && ((i + 1) < str.Length && (sequentialEscapes%2 == 0)))
                {
                    if (HasErrors) yield break;


                    var chr1 = str[i + 1];

                    var result = ReplaceEscapeCode(i, chr1);

                    if (HasErrors) yield break;

                    yield return result;
                    i += 2;
                }
                else if (chr == '\\')
                {
                    yield return "\\";
                    sequentialEscapes++;
                    i++;
                }
                else
                {
                    if (HasErrors) yield break;


                    var result = FilterCharacter(i, chr);

                    if (HasErrors) yield break;

                    yield return result;

                    sequentialEscapes = 0;
                    i++;
                }
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "index")]
        private static string FilterCharacter(int index, char chr)
        {
            if (chr == '\n')
            {
                return "\\n";
            }
            if (chr == '\r')
            {
                return "\\r";
            }

            return chr.ToString(CultureInfo.InvariantCulture);
        }



        private string ReplaceEscapeCode(int index, char code)
        {
            if (code == 't')
            {
                return "    "; //4 spaces
            }

            if (code == 'n' || code == '"' || code == '\\')
            {
                return "\\" + code;
            }

            if (code == 'r')
            {
                return "r"; //don't even ask, I have no fucking idea why
            }

            HasErrors = true;

            _invalidEscapeCodes.Add(new LSLStringCharacterError(code, index));

            return "";
        }
    }
}