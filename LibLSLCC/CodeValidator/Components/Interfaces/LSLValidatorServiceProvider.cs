#region FileInfo
// 
// 
// File: LSLValidatorServiceProvider.cs
// 
// Last Compile: 25/09/2015 @ 11:47 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
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

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    /// <summary>
    ///     Represents various sub strategies and listeners that are used in LSLCodeValidator's implementation
    /// </summary>
    public interface ILSLValidatorServiceProvider
    {
        ILSLExpressionValidator ExpressionValidator { get; }
        ILSLMainLibraryDataProvider MainLibraryDataProvider { get; }
        ILSLStringPreProcessor StringLiteralPreProcessor { get; }
        ILSLSyntaxErrorListener SyntaxErrorListener { get; }
        ILSLSyntaxWarningListener SyntaxWarningListener { get; }
    }

    public static class ILSLValidatorServiceProvideExtensions
    {
        /// <summary>
        ///     Returns true if all service provider properties are non null
        /// </summary>
        /// <param name="provider">The ILSLValidatorServiceProvider to check</param>
        /// <returns>True if all properties are initialized</returns>
        public static bool IsComplete(this ILSLValidatorServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }


            return provider.ExpressionValidator != null
                   && provider.MainLibraryDataProvider != null
                   && provider.StringLiteralPreProcessor != null
                   && provider.SyntaxErrorListener != null
                   && provider.SyntaxWarningListener != null;
        }
    }
}