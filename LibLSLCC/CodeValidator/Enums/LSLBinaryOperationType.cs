using System;

namespace LibLSLCC.CodeValidator.Enums
{
    public enum LSLBinaryOperationType 
    {
        Add = 24,
        AddAssign = 23,
        Subtract = 22,
        SubtractAssign = 21,
        Multiply = 20,
        MultiplyAssign = 19,
        Divide = 18,
        DivideAssign = 17,
        Modulus = 16,
        ModulusAssign = 15,
        Assign = 14,
        BitwiseXor = 13,
        BitwiseAnd = 12,
        BitwiseOr = 11,
        LogicalOr = 10,
        LogicalAnd = 9,
        LessThan = 8,
        LessThanEqual = 7,
        GreaterThan = 6,
        GreaterThanEqual = 5,
        LeftShift = 4,
        RightShift = 3,
        Equals = 2,
        NotEquals = 1,
        Error = 0,
    }


    public static class LSLBinaryOperationTypeTools
    {
        public static bool IsModifyAssign(this LSLBinaryOperationType type)
        {
            return type == LSLBinaryOperationType.AddAssign ||
                   type == LSLBinaryOperationType.DivideAssign ||
                   type == LSLBinaryOperationType.ModulusAssign ||
                   type == LSLBinaryOperationType.MultiplyAssign ||
                   type == LSLBinaryOperationType.SubtractAssign;
        }



        public static bool IsAssignOrModifyAssign(this LSLBinaryOperationType type)
        {
            return type == LSLBinaryOperationType.Assign || IsModifyAssign(type);
        }

        public static LSLBinaryOperationType ParseFromOperator(string operationString)
        {
            if (operationString == "=")
            {
                return LSLBinaryOperationType.Assign;
            }
            if (operationString == "+")
            {
                return LSLBinaryOperationType.Add;
            }
            if (operationString == "-")
            {
                return LSLBinaryOperationType.Subtract;
            }
            if (operationString == "/")
            {
                return LSLBinaryOperationType.Divide;
            }
            if (operationString == "%")
            {
                return LSLBinaryOperationType.Modulus;
            }
            if (operationString == "*")
            {
                return LSLBinaryOperationType.Multiply;
            }
            if (operationString == "^")
            {
                return LSLBinaryOperationType.BitwiseXor;
            }
            if (operationString == "|")
            {
                return LSLBinaryOperationType.BitwiseOr;
            }
            if (operationString == "&")
            {
                return LSLBinaryOperationType.BitwiseAnd;
            }
            if (operationString == "<")
            {
                return LSLBinaryOperationType.LessThan;
            }
            if (operationString == ">")
            {
                return LSLBinaryOperationType.GreaterThan;
            }
            if (operationString == "==")
            {
                return LSLBinaryOperationType.Equals;
            }
            if (operationString == "!=")
            {
                return LSLBinaryOperationType.NotEquals;
            }
            if (operationString == "+=")
            {
                return LSLBinaryOperationType.AddAssign;
            }
            if (operationString == "-=")
            {
                return LSLBinaryOperationType.SubtractAssign;
            }
            if (operationString == "/=")
            {
                return LSLBinaryOperationType.DivideAssign;
            }
            if (operationString == "%=")
            {
                return LSLBinaryOperationType.ModulusAssign;
            }
            if (operationString == "*=")
            {
                return LSLBinaryOperationType.MultiplyAssign;
            }
            if (operationString == "||")
            {
                return LSLBinaryOperationType.LogicalOr;
            }
            if (operationString == "&&")
            {
                return LSLBinaryOperationType.LogicalAnd;
            }
            if (operationString == "<=")
            {
                return LSLBinaryOperationType.LessThanEqual;
            }
            if (operationString == ">=")
            {
                return LSLBinaryOperationType.GreaterThanEqual;
            }
            if (operationString == "<<")
            {
                return LSLBinaryOperationType.LeftShift;
            }
            if (operationString == ">>")
            {
                return LSLBinaryOperationType.RightShift;
            }
            throw new ArgumentException(
                string.Format("Could not convert LSLBinaryOperationType.{0} enum value to operator string",
                    operationString), "operationString");
        }



        public static string ToOperatorString(this LSLBinaryOperationType type)
        {
            switch (type)
            {
                case LSLBinaryOperationType.Assign:
                    return "=";

                case LSLBinaryOperationType.Add:
                    return "+";


                case LSLBinaryOperationType.Subtract:
                    return "-";


                case LSLBinaryOperationType.Divide:
                    return "/";


                case LSLBinaryOperationType.Modulus:
                    return "%";


                case LSLBinaryOperationType.Multiply:
                    return "*";

                case LSLBinaryOperationType.BitwiseXor:
                    return "^";

                case LSLBinaryOperationType.BitwiseOr:
                    return "|";

                case LSLBinaryOperationType.BitwiseAnd:
                    return "&";

                case LSLBinaryOperationType.LessThan:
                    return "<";

                case LSLBinaryOperationType.GreaterThan:
                    return ">";

                case LSLBinaryOperationType.Equals:
                    return "==";

                case LSLBinaryOperationType.NotEquals:
                    return "!=";

                case LSLBinaryOperationType.AddAssign:
                    return "+=";

                case LSLBinaryOperationType.SubtractAssign:
                    return "-=";

                case LSLBinaryOperationType.DivideAssign:
                    return "/=";

                case LSLBinaryOperationType.ModulusAssign:
                    return "%=";

                case LSLBinaryOperationType.MultiplyAssign:
                    return "*=";

                case LSLBinaryOperationType.LogicalOr:
                    return "||";

                case LSLBinaryOperationType.LogicalAnd:
                    return "&&";

                case LSLBinaryOperationType.LessThanEqual:
                    return "<=";

                case LSLBinaryOperationType.GreaterThanEqual:
                    return ">=";

                case LSLBinaryOperationType.LeftShift:
                    return "<<";

                case LSLBinaryOperationType.RightShift:
                    return ">>";
            }


            throw new ArgumentException(
                string.Format("Could not convert LSLBinaryOperationType.{0} enum value to operator string", type),
                "type");
        }
    }
}