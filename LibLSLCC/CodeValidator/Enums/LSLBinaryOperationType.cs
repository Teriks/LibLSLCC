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

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Describes different types of binary operations.
    /// </summary>
    public enum LSLBinaryOperationType
    {
        /// <summary>
        ///     Addition.
        /// </summary>
        Add = 24,

        /// <summary>
        ///     Compound Add and assign.
        /// </summary>
        AddAssign = 23,

        /// <summary>
        ///     Subtraction.
        /// </summary>
        Subtract = 22,


        /// <summary>
        ///     Compound Subtract and assign.
        /// </summary>
        SubtractAssign = 21,

        /// <summary>
        ///     Multiplication.
        /// </summary>
        Multiply = 20,

        /// <summary>
        ///     Compound Multiply and assign.
        /// </summary>
        MultiplyAssign = 19,

        /// <summary>
        ///     Division.
        /// </summary>
        Divide = 18,

        /// <summary>
        ///     Compound Divide and assign.
        /// </summary>
        DivideAssign = 17,


        /// <summary>
        ///     Modulus.
        /// </summary>
        Modulus = 16,

        /// <summary>
        ///     Compound Modulus and assign.
        /// </summary>
        ModulusAssign = 15,

        /// <summary>
        ///     Direct assignment.
        /// </summary>
        Assign = 14,

        /// <summary>
        ///     Bitwise XOR operator.
        /// </summary>
        BitwiseXor = 13,

        /// <summary>
        ///     Bitwise AND operator.
        /// </summary>
        BitwiseAnd = 12,

        /// <summary>
        ///     Bitwise OR operator.
        /// </summary>
        BitwiseOr = 11,

        /// <summary>
        ///     Logical OR operator.
        /// </summary>
        LogicalOr = 10,

        /// <summary>
        ///     Logical AND operator.
        /// </summary>
        LogicalAnd = 9,


        /// <summary>
        ///     Less than comparison operator.
        /// </summary>
        LessThan = 8,

        /// <summary>
        ///     Less than or equal to comparison operator.
        /// </summary>
        LessThanEqual = 7,

        /// <summary>
        ///     Greater than comparison operator.
        /// </summary>
        GreaterThan = 6,


        /// <summary>
        ///     Greater than or equal to comparison operator.
        /// </summary>
        GreaterThanEqual = 5,


        /// <summary>
        ///     Bitwise left shift operator.
        /// </summary>
        LeftShift = 4,

        /// <summary>
        ///     Bitwise right shift operator.
        /// </summary>
        RightShift = 3,

        /// <summary>
        ///     Equality comparison operator.
        /// </summary>
        Equals = 2,

        /// <summary>
        ///     In-Equality comparison operator.
        /// </summary>
        NotEquals = 1,

        /// <summary>
        ///     Undefined operator.
        /// </summary>
        Error = 0
    }

    /// <summary>
    ///     <see cref="LSLBinaryOperationType" /> extensions for parsing <see cref="LSLBinaryOperationType" />'s from strings.
    ///     As well as converting them into strings.
    ///     This class also contains some methods for group classifications of <see cref="LSLBinaryOperationType" />.
    ///     Such as IsModifyAssign();
    /// </summary>
    public static class LSLBinaryOperationTypeTools
    {
        /// <summary>
        ///     Determines if the <see cref="LSLBinaryOperationType" /> is a form of modifying assignment that is not just a plain
        ///     assign operation.
        /// </summary>
        /// <param name="type">The <see cref="LSLBinaryOperationType" /> to test.</param>
        /// <returns>
        ///     True if the provided <see cref="LSLBinaryOperationType" /> is a ModifyAssignOperation such as (+=).  False if
        ///     it is a plain assignment operator, or other type of operator.
        /// </returns>
        public static bool IsModifyAssign(this LSLBinaryOperationType type)
        {
            return type == LSLBinaryOperationType.AddAssign ||
                   type == LSLBinaryOperationType.DivideAssign ||
                   type == LSLBinaryOperationType.ModulusAssign ||
                   type == LSLBinaryOperationType.MultiplyAssign ||
                   type == LSLBinaryOperationType.SubtractAssign;
        }


        /// <summary>
        ///     Determines if the <see cref="LSLBinaryOperationType" /> is a direct assignment, or a modifying assignment
        ///     operation.
        ///     Effectively: (type == <see cref="LSLBinaryOperationType.Assign" /> ||
        ///     <see cref="IsModifyAssign(LSLBinaryOperationType)" />)
        /// </summary>
        /// <param name="type">The <see cref="LSLBinaryOperationType" /> to test.</param>
        /// <returns>
        ///     True if the provided <see cref="LSLBinaryOperationType" /> is either a direct assignment operation, or is a
        ///     modifying assignment operation.  False if otherwise.
        /// </returns>
        public static bool IsAssignOrModifyAssign(this LSLBinaryOperationType type)
        {
            return type == LSLBinaryOperationType.Assign || IsModifyAssign(type);
        }


        /// <summary>
        ///     Parses an <see cref="LSLBinaryOperationType" /> from a given string.  Accepted strings are any of LSL's binary
        ///     operators, without any whitespace characters added.
        /// </summary>
        /// <param name="operationString">The operation string to turn into an <see cref="LSLBinaryOperationType" />.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown when 'operationString' does not contain a valid string representation of a
        ///     binary operator that exist's in LSL.
        /// </exception>
        /// <returns>The parsed <see cref="LSLBinaryOperationType" />.</returns>
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


        /// <summary>
        ///     Converts an <see cref="LSLBinaryOperationType" /> into is source code equivalent string.
        /// </summary>
        /// <param name="type">The <see cref="LSLBinaryOperationType" /> to convert to a string.</param>
        /// <returns>The source code equivalent string representation of the provided <see cref="LSLBinaryOperationType" />.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown if the <see cref="LSLBinaryOperationType" /> provided is equal to
        ///     <see cref="LSLBinaryOperationType.Error" />
        /// </exception>
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