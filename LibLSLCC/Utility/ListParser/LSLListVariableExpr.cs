#region FileInfo
// 
// File: LSLListVariableExpr.cs
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

using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.Utility.ListParser
{
    /// <summary>
    ///     Variable reference list item.
    /// </summary>
    public sealed class LSLListVariableExpr : ILSLListExpr
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLListVariableExpr" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public LSLListVariableExpr(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Name of the variable referenced
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     True if this list item represents a variable reference.
        /// </summary>
        public bool IsVariableReference
        {
            get { return true; }
        }

        /// <summary>
        ///     The list item type, it will be void if its a variable reference
        /// </summary>
        public LSLType Type
        {
            get { return LSLType.Void; }
        }

        /// <summary>
        ///     Gets string representing the element, with quoting characters for the type.
        /// </summary>
        /// <value>
        ///     The value string.
        /// </value>
        public string ValueString
        {
            get { return Name; }
        }
    }
}