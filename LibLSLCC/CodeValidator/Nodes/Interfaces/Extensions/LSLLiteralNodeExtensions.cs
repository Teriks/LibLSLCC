#region FileInfo

// 
// File: LSLLiteralNodeInterfaceExtensions.cs
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
using System.Linq;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    /// Represents the type of overflow/underflow that can occur when defining a integer/hex literal.
    /// </summary>
    public enum LSLLiteralOverflowType
    {

        /// <summary>
        /// No overflow/underflow.
        /// </summary>
        None,

        /// <summary>
        /// Overflow.
        /// </summary>
        Overflow,

        /// <summary>
        /// Underflow.
        /// </summary>
        Underflow
    }


    /// <summary>
    ///     Extensions for LSL literal nodes.
    /// </summary>
    public static class LSLLiteralNodeExtensions
    {
        /// <summary>
        ///     Determines whether the integer literal node is a literal value that overflows/underflows a 32 bit integer. <para/>
        ///     Whether or not the node is negated is determined with <see cref="LSLExprNodeExtensions.IsNegated"/>.
        /// </summary>
        /// <param name="node">The integer literal node to test.</param>
        /// <returns><see cref="LSLLiteralOverflowType"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static LSLLiteralOverflowType CheckForOverflow(this ILSLIntegerLiteralNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return CheckForOverflow(node, node.IsNegated());
        }




        /// <summary>
        ///     Determines whether the float literal node is a literal value that overflows/underflows a 32 bit float.
        /// </summary>
        /// <param name="node">The float literal node to test.</param>
        /// <returns><see cref="LSLLiteralOverflowType"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static LSLLiteralOverflowType CheckForOverflow(this ILSLFloatLiteralNode node)
        {
            if (node == null) throw new ArgumentNullException("node");


            var formatedFloatString = LSLFormatTools.FormatFloatString(node.RawText.TrimEnd('f', 'F'));

            bool nonZero = formatedFloatString.TakeWhile(c => c != 'e').Any(c => c != '0' && c != '.' && c != 'f');

            if(!nonZero) return LSLLiteralOverflowType.None;

            double val;
            try
            {
                val = double.Parse(formatedFloatString);
            }
            catch (OverflowException)
            {
                return LSLLiteralOverflowType.Overflow;
            }


            if (val > 3.402823466E+38)
            {
                return LSLLiteralOverflowType.Overflow;
            }
            if (val < 1.401298464E-45)
            {
                return LSLLiteralOverflowType.Underflow;
            }

            return LSLLiteralOverflowType.None;
        }



        /// <summary>
        ///     Determines whether the integer literal node is a literal value that overflows/underflows a 32 bit integer.
        /// </summary>
        /// <param name="node">The integer literal node to test.</param>
        /// <param name="negated">Whether or not the node is negated.</param>
        /// <returns><see cref="LSLLiteralOverflowType"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static LSLLiteralOverflowType CheckForOverflow(this ILSLIntegerLiteralNode node, bool negated)
        {
            if (node == null) throw new ArgumentNullException("node");

            long val;
            try
            {
                val = Convert.ToInt64(node.RawText);
            }
            catch (OverflowException)
            {
                return LSLLiteralOverflowType.Overflow;
            }

            if (negated && val > 2147483648)
            {
                return LSLLiteralOverflowType.Underflow;
            }
            if(!negated && val > 2147483647)
            {
                return LSLLiteralOverflowType.Overflow;
            }

            return LSLLiteralOverflowType.None;
        }


        /// <summary>
        ///     Determines whether the hex literal node is a literal value that overflows/underflows a 32 bit integer.
        /// </summary>
        /// <param name="node">The integer hex node to test.</param>
        /// <returns><see cref="LSLLiteralOverflowType"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static LSLLiteralOverflowType CheckForOverflow(this ILSLHexLiteralNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            var hexPart = node.RawText.Substring(2).TrimStart('0');

            bool negative = false;
            switch (char.ToUpper(hexPart[0]))
            {
                case '8':
                case '9':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    negative = true;
                    break;
            }

            if(hexPart.Length > 8)
            { 
                return negative ? LSLLiteralOverflowType.Underflow : LSLLiteralOverflowType.Overflow;
            }

            return LSLLiteralOverflowType.None;
        }
    }
}