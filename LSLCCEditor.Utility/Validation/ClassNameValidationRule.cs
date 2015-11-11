using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using LibLSLCC.CSharp;
using LibLSLCC.Utility;

namespace LSLCCEditor.Utility.Validation
{
    public class ClassNameValidationRule : ValidationRule
    {
        public bool AllowBlank { get; set; }

        public bool AllowGenerics { get; set; }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {



            var input = value as string;

            if (string.IsNullOrWhiteSpace(input))
            {
                return AllowBlank ? ValidationResult.ValidResult : new ValidationResult(false, "you must supply a class name.");
            }

            var result = CSharpClassNameValidator.Validate(input);

            if (result.IsGeneric && !AllowGenerics)
            {
                return new ValidationResult(false, "generic class names are not allowed.");
            }

            return new ValidationResult(result.Success, result.ErrorDescription);


        }

        
    }
}