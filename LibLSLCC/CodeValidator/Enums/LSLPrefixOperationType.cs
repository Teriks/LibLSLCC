using System;

namespace LibLSLCC.CodeValidator.Enums
{
    public enum LSLPrefixOperationType 
    {
        Increment = 6,
        Decrement = 5,
        Negative = 4,
        Positive = 3,
        BooleanNot = 2,
        BitwiseNot = 1,
        Error = 0
    }


    public static class LSLPrefixOperationTypeTools
    {
        public static string ToOperatorString(this LSLPrefixOperationType type)
        {
            switch (type)
            {
                case LSLPrefixOperationType.Decrement:
                    return "--";
                case LSLPrefixOperationType.Increment:
                    return "++";
                case LSLPrefixOperationType.Negative:
                    return "-";
                case LSLPrefixOperationType.Positive:
                    return "+";
                case LSLPrefixOperationType.BooleanNot:
                    return "!";
                case LSLPrefixOperationType.BitwiseNot:
                    return "~";
            }

            throw new ArgumentException(
                string.Format("Could not convert LSLPrefixOperationType.{0} enum value to operator string", type),
                "type");
        }



        public static LSLPrefixOperationType ParseFromOperator(string operationString)
        {
            if (string.IsNullOrEmpty(operationString))
            {
                throw new ArgumentNullException("operationString");
            }

            switch (operationString)
            {
                case "--":
                    return LSLPrefixOperationType.Decrement;
                case "++":
                    return LSLPrefixOperationType.Increment;
                case "-":
                    return LSLPrefixOperationType.Negative;
                case "+":
                    return LSLPrefixOperationType.Positive;
                case "!":
                    return LSLPrefixOperationType.BooleanNot;
                case "~":
                    return LSLPrefixOperationType.BitwiseNot;
            }

            throw new ArgumentException(
                string.Format("Could not parse \"{0}\" into an LSLPrefixOperationType, invalid operator string",
                    operationString), "operationString");
        }
    }
}