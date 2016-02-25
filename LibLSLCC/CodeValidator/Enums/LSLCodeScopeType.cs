#region FileInfo

// 
// File: LSLCodeScopeType.cs
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

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Represents a category of code scope.
    /// </summary>
    public enum LSLCodeScopeType
    {
        /// <summary>
        ///     An event handlers code scope.
        /// </summary>
        EventHandler,

        /// <summary>
        ///     A user defined functions code scope.
        /// </summary>
        Function,

        /// <summary>
        ///     A Do-Loops code scope.
        /// </summary>
        DoLoop,

        /// <summary>
        ///     A While-Loops code scope.
        /// </summary>
        WhileLoop,

        /// <summary>
        ///     A For-Loops code scope.
        /// </summary>
        ForLoop,

        /// <summary>
        ///     An If statements code scope.
        /// </summary>
        If,

        /// <summary>
        ///     An Else-If statements code scope.
        /// </summary>
        ElseIf,

        /// <summary>
        ///     An Else statements code scope.
        /// </summary>
        Else,

        /// <summary>
        ///     An anonymous code scope defined inside of another code scope.
        ///     Basically a matching pare of curly braces inside of another code scope that does not associate with any control
        ///     statement.
        ///     This is sometimes used to create an anonymous new scope for local variables.
        /// </summary>
        AnonymousBlock
    }
}