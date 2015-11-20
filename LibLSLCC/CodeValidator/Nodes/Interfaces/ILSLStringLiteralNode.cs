#region FileInfo
// 
// File: ILSLStringLiteralNode.cs
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

using LibLSLCC.CodeValidator.Components.Interfaces;

namespace LibLSLCC.CodeValidator.Nodes.Interfaces
{

    /// <summary>
    /// AST token interface for string literal nodes.
    /// </summary>
    public interface ILSLStringLiteralNode : ILSLReadOnlyExprNode
    {
        /// <summary>
        /// The pre-processed text of the string literal.
        /// 
        /// <see cref="LSLCodeValidator"/> relies on an implementation of <see cref="ILSLStringPreProcessor"/> to fill this value out by passing <see cref="ILSLStringPreProcessor"/>
        /// the raw text for the string literal and assigning the string it produces to this property.
        /// <see cref="ILSLStringPreProcessor"/>
        /// </summary>
        string PreProccessedText { get; }

        /// <summary>
        /// The raw text for the string literal from the source code, this should include the quote characters that surround the string.
        /// Any escape codes used in the source code string should be double escaped.
        /// </summary>
        string RawText { get; }
    }
}