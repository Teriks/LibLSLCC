using System.CodeDom.Compiler;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using LibLSLCC.CSharp;
using LibLSLCC.Utility;

namespace LSLCCEditor.Utility.Validation
{
    public class NamespaceValidationRule : ValidationRule
    {
        public bool AllowBlank { get; set; }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            if (string.IsNullOrWhiteSpace(input))
            {
                return AllowBlank ? ValidationResult.ValidResult : new ValidationResult(true, "namespace name must be provided.");
            }

            var validate = CSharpNamespaceNameValidator.Validate(input);
            return  new ValidationResult(validate.Success, validate.ErrorDescription);
        }

    }
}