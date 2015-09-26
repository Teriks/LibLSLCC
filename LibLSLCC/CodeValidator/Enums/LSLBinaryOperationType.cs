#region FileInfo
// 
// File: LSLBinaryOperationType.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 
#endregion
#region Imports

using System;

#endregion

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
        Error = 0
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