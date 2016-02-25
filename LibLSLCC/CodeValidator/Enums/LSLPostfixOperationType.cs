#region FileInfo

// 
// File: LSLPostfixOperationType.cs
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
    ///     Represents different types of LSL postfix operators.
    /// </summary>
    public enum LSLPostfixOperationType
    {
        /// <summary>
        ///     Postfix Increment.
        /// </summary>
        Increment = 2,

        /// <summary>
        ///     Postfix Decrement.
        /// </summary>
        Decrement = 1,

        /// <summary>
        ///     Unknown/Erroneous postfix operator.
        /// </summary>
        Error = 0
    }

    /// <summary>
    ///     <see cref="LSLPostfixOperationType" /> extensions for converting <see cref="LSLPostfixOperationType" /> from source
    ///     code string representation
    ///     and back.
    /// </summary>
    public static class LSLPostfixOperationTypeTools
    {
        /// <summary>
        ///     Converts the provided  <see cref="LSLPostfixOperationType" /> to its source code string representation.
        /// </summary>
        /// <param name="type">The <see cref="LSLPostfixOperationType" /> to convert to a string.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown if the <see cref="LSLPostfixOperationType" /> provided was equal to
        ///     <see cref="LSLPostfixOperationType.Error" />.
        /// </exception>
        /// <returns>The source code string representation of the <see cref="LSLPostfixOperationType" />.</returns>
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


        /// <summary>
        ///     Determines whether the given postfix operation is a modifying operation.
        ///     Both <see cref="LSLPostfixOperationType.Decrement" /> and <see cref="LSLPostfixOperationType.Increment" /> are
        ///     modifying operations.
        /// </summary>
        /// <param name="type">If the postfix operation modifies the expression to its left.</param>
        /// <returns></returns>
        public static bool IsModifying(this LSLPostfixOperationType type)
        {
            return type == LSLPostfixOperationType.Decrement || type == LSLPostfixOperationType.Increment;
        }


        /// <summary>
        ///     Parses a <see cref="LSLPostfixOperationType" /> from its source code string representation.
        /// </summary>
        /// <param name="operationString">The string to attempt to parse an <see cref="LSLPostfixOperationType" /> from.</param>
        /// <exception cref="ArgumentNullException">Thrown if 'operationString' is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if 'operationString' was not a valid source code string representation of an
        ///     LSL postfix operator.
        /// </exception>
        /// <returns>The parsed <see cref="LSLPostfixOperationType" />.</returns>
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