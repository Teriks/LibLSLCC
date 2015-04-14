using System;

namespace LibLSLCC.CodeValidator.Enums
{
    public enum LSLPostfixOperationType 
    {
        Increment = 2,
        Decrement = 1,
        Error = 0
    }

    public static class LSLPostfixOperationTypeTools
    {
        public static string ToOperatorString(this LSLPostfixOperationType type)
        {
            switch (type)
            {
                case LSLPostfixOperationType.Decrement:
                    return "--";
                case LSLPostfixOperationType.Increment:
                    return "++";
            }

            throw new ArgumentException(
                string.Format("Could not convert LSLPostfixOperationType.{0} enum value to operator string", type),
                "type");
        }



        public static LSLPostfixOperationType ParseFromOperator(string operationString)
        {
            if (string.IsNullOrEmpty(operationString))
            {
                throw new ArgumentNullException("operationString");
            }

            switch (operationString)
            {
                case "--":
                    return LSLPostfixOperationType.Decrement;
                case "++":
                    return LSLPostfixOperationType.Increment;
            }

            throw new ArgumentException(
                string.Format("Could not parse \"{0}\" into an LSLPostfixOperationType, invalid operator string",
                    operationString), "operationString");
        }
    }
}