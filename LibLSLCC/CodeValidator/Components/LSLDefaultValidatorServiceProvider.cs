#region FileInfo
// 
// File: LSLDefaultValidatorServiceProvider.cs
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

using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.LibraryData;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    /// The default implementation of <see cref="ILSLValidatorServiceProvider"/> for the library
    /// </summary>
    public class LSLDefaultValidatorServiceProvider : ILSLValidatorServiceProvider
    {
        /// <summary>
        /// Construct the default implementation of <see cref="ILSLValidatorServiceProvider"/> for the library
        /// </summary>
        public LSLDefaultValidatorServiceProvider()
        {
            ExpressionValidator = new LSLDefaultExpressionValidator();

            LibraryDataProvider = new LSLDefaultLibraryDataProvider(false,
                LSLLibraryBaseData.StandardLsl);

            SyntaxErrorListener = new LSLDefaultSyntaxErrorListener();
            SyntaxWarningListener = new LSLDefaultSyntaxWarningListener();
            StringLiteralPreProcessor = new LSLDefaultStringPreProcessor();
        }

        /// <summary>
        /// The default implementation uses a LSLDefaultExpressionValidator(); instance.
        /// see: <see cref="LSLDefaultExpressionValidator"/>
        /// </summary>
        public ILSLExpressionValidator ExpressionValidator { get; private set; }

        /// <summary>
        /// The default implementation uses a LSLDefaultLibraryDataProvider(false, LSLLibraryBaseData.StandardLsl); instance.
        /// see: <see cref="LSLDefaultLibraryDataProvider"/>
        /// </summary>
        public ILSLLibraryDataProvider LibraryDataProvider { get; private set; }

        /// <summary>
        /// The default implementation uses a LSLDefaultStringPreProcessor(); instance.
        /// see: <see cref="LSLDefaultStringPreProcessor"/>
        /// </summary>
        public ILSLStringPreProcessor StringLiteralPreProcessor { get; private set; }

        /// <summary>
        /// The default implementation uses a LSLDefaultSyntaxErrorListener(); instance.
        /// see: <see cref="LSLDefaultSyntaxErrorListener"/>
        /// </summary>
        public ILSLSyntaxErrorListener SyntaxErrorListener { get; private set; }

        /// <summary>
        /// The default implementation uses a LSLDefaultSyntaxWarningListener(); instance.
        /// see: <see cref="LSLDefaultSyntaxWarningListener"/>
        /// </summary>
        public ILSLSyntaxWarningListener SyntaxWarningListener { get; private set; }
    }
}