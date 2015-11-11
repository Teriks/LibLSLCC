using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace LSLCCEditor.Utility.Validation
{
    public class RegexValidationRule : ValidationRule
    {

        private string _errorMessage;
        private RegexOptions _regexOptions = RegexOptions.None;
        private string _regex;

        public RegexValidationRule()
        {
        }

        public RegexValidationRule(string regex)
        {
            Regex = regex;
        }

        public RegexValidationRule(string regex, string errorMessage)
            : this(regex)
        {
            RegexOptions = _regexOptions;
        }

        public RegexValidationRule(string regex, string errorMessage, RegexOptions regexOptions)
            : this(regex)
        {
            RegexOptions = regexOptions;
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public RegexOptions RegexOptions
        {
            get { return _regexOptions; }
            set { _regexOptions = value; }
        }

        public string Regex
        {
            get { return _regex; }
            set { _regex = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;

            if (!String.IsNullOrEmpty(Regex))
            {

                string text = value as string ?? String.Empty;

                if (!System.Text.RegularExpressions.Regex.IsMatch(text, Regex, RegexOptions))
                    result = new ValidationResult(false, ErrorMessage);
            }

            return result;
        }
    }
}
