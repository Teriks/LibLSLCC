#region FileInfo
// 
// File: ILSLCodeValidatorStrategiesExtensions.cs
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
using System;

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    /// <summary>
    /// Extensions for <see cref="ILSLCodeValidatorStrategies"/>
    /// </summary>
    public static class ILSLCodeValidatorStrategiesExtensions
    {
        /// <summary>
        /// Returns true if all strategy properties are non null.
        /// </summary>
        /// <param name="strategies">The <see cref="ILSLCodeValidatorStrategies"/> to check.</param>
        /// <returns>True if all properties are initialized.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="strategies"/> is <see langword="null" />.</exception>
        public static bool IsComplete(this ILSLCodeValidatorStrategies strategies)
        {
            if (strategies == null)
            {
                throw new ArgumentNullException("strategies");
            }


            return strategies.ExpressionValidator != null
                   && strategies.LibraryDataProvider != null
                   && strategies.StringLiteralPreProcessor != null
                   && strategies.SyntaxErrorListener != null
                   && strategies.SyntaxWarningListener != null;
        }


        /// <summary>
        /// Returns true if all strategy properties are non null.
        /// </summary>
        /// <param name="strategies">The <see cref="ILSLCodeValidatorStrategies"/> to check.</param>
        /// <param name="describeNulls">A string describing which properties are null if <see cref="IsComplete(ILSLCodeValidatorStrategies, out string)"/> returns <c>false</c></param>
        /// <returns>True if all properties are initialized.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="strategies"/> is <see langword="null" />.</exception>
        public static bool IsComplete(this ILSLCodeValidatorStrategies strategies, out string describeNulls)
        {
            if (strategies == null)
            {
                throw new ArgumentNullException("strategies");
            }


            string nullProps = string.Empty;

            if (strategies.ExpressionValidator == null)
            {
                nullProps += typeof(ILSLCodeValidatorStrategies).Name+".ExpressionValidator is null." + Environment.NewLine;
            }
            if (strategies.LibraryDataProvider == null)
            {
                nullProps += typeof(ILSLCodeValidatorStrategies).Name + ".LibraryDataProvider is null." + Environment.NewLine;
            }
            if (strategies.SyntaxErrorListener == null)
            {
                nullProps += typeof(ILSLCodeValidatorStrategies).Name + ".SyntaxErrorListener is null." + Environment.NewLine;
            }
            if (strategies.SyntaxWarningListener == null)
            {
                nullProps += typeof(ILSLCodeValidatorStrategies).Name + ".SyntaxWarningListener is null.";
            }

            describeNulls = string.IsNullOrEmpty(nullProps) ? null : nullProps;

            return describeNulls == null;
        }
    }
}