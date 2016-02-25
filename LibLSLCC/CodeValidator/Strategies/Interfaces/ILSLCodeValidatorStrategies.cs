#region FileInfo

// 
// File: ILSLCodeValidatorStrategies.cs
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

#endregion

using LibLSLCC.CodeValidator.Nodes;

namespace LibLSLCC.CodeValidator.Strategies
{
    /// <summary>
    ///     Represents various sub strategies and listeners that are used in the <see cref="LSLCodeValidator" /> implementation
    ///     of <see cref="ILSLCodeValidator" />.
    /// </summary>
    public interface ILSLCodeValidatorStrategies
    {
        /// <summary>
        ///     The expression validator is in charge of determining if two types are valid
        ///     in a binary expression.  It also does several other things, one being checking if an expression
        ///     with a certain return type can be passed into a function parameter.
        /// </summary>
        ILSLExpressionValidator ExpressionValidator { get; }

        /// <summary>
        ///     The library data provider gives the code validator information about standard library functions,
        ///     constants and events that exist in the LSL namespace.
        /// </summary>
        ILSLBasicLibraryDataProvider LibraryDataProvider { get; }

        /// <summary>
        ///     The string literal pre-processor is in charge of pre-processing string literals
        ///     from source code before the value is assigned to an <see cref="ILSLStringLiteralNode.PreProcessedText" />.
        /// </summary>
        ILSLStringPreProcessor StringLiteralPreProcessor { get; }

        /// <summary>
        ///     The syntax error listener is an interface that listens for syntax
        ///     errors from the code validator
        /// </summary>
        ILSLSyntaxErrorListener SyntaxErrorListener { get; }

        /// <summary>
        ///     The syntax error listener is an interface that listens for syntax
        ///     warnings from the code validator
        /// </summary>
        ILSLSyntaxWarningListener SyntaxWarningListener { get; }
    }
}