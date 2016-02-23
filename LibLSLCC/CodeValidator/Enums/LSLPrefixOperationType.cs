#region FileInfo
// 
// File: LSLPrefixOperationType.cs
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

    /// <summary>
    /// Represents different types of LSL prefix operators.
    /// </summary>
    public enum LSLPrefixOperationType
    {
        /// <summary>
        /// Prefix increment.
        /// </summary>
        Increment = 6,

        /// <summary>
        /// Prefix decrement.
        /// </summary>
        Decrement = 5,

        /// <summary>
        /// Prefix negation.
        /// </summary>
        Negative = 4,

        /// <summary>
        /// Positive number prefix.
        /// </summary>
        Positive = 3,

        /// <summary>
        /// Boolean not prefix operator.
        /// </summary>
        BooleanNot = 2,

        /// <summary>
        /// Bitwise not prefix operator. (~)
        /// </summary>
        BitwiseNot = 1,

        /// <summary>
        /// Unknown/Erroneous prefix operator.
        /// </summary>
        Error = 0
    }

    /// <summary>
    /// <see cref="LSLPrefixOperationType"/> extensions for converting <see cref="LSLPrefixOperationType"/> from source code string representation
    /// and back.
    /// </summary>
    public static class LSLPrefixOperationTypeTools
    {

        /// <summary>
        /// Converts the provided <see cref="LSLPrefixOperationType"/> to its source code string representation.
        /// </summary>
        /// <param name="type">The <see cref="LSLPrefixOperationType"/> to convert to a string.</param>
        /// <exception cref="ArgumentException">Thrown if the <see cref="LSLPrefixOperationType"/> provided was equal to <see cref="LSLPrefixOperationType.Error"/>.</exception>
        /// <returns>The source code string representation of the <see cref="LSLPrefixOperationType"/>.</returns>
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



        /// <summary>
        /// Determines whether the given prefix operation is a modifying operation.
        /// Currently only <see cref="LSLPrefixOperationType.Decrement"/> and <see cref="LSLPrefixOperationType.Increment"/> are considered modifying operations.
        /// </summary>
        /// <param name="type">If the prefix operation modifies the expression to its right.</param>
        /// <returns></returns>
        public static bool IsModifying(this LSLPrefixOperationType type)
        {
            return type == LSLPrefixOperationType.Decrement || type == LSLPrefixOperationType.Increment;
        }


        /// <summary>
        /// Parses a <see cref="LSLPrefixOperationType"/> from its source code string representation.
        /// </summary>
        /// <param name="operationString">The string to attempt to parse an <see cref="LSLPrefixOperationType"/> from.</param>
        /// <exception cref="ArgumentNullException">Thrown if 'operationString' is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if 'operationString' was not a valid source code string representation of an LSL prefix operator.</exception>
        /// <returns>The parsed <see cref="LSLPrefixOperationType"/>.</returns>
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