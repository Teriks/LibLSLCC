#region FileInfo

// 
// File: LSLListVectorExpr.cs
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
using LibLSLCC.CodeValidator;

#endregion

namespace LibLSLCC.Utility.ListParser
{
    /// <summary>
    ///     Vector list item.
    /// </summary>
    public sealed class LSLListVectorExpr : ILSLListExpr
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLListVectorExpr" /> class.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        /// <exception cref="System.ArgumentException">
        ///     X is not an LSLFloat or LSLVariable.;x
        ///     or
        ///     Y is not an LSLFloat or LSLVariable.;y
        ///     or
        ///     Z is not an LSLFloat or LSLVariable.;z
        /// </exception>
        public LSLListVectorExpr(ILSLListExpr x, ILSLListExpr y, ILSLListExpr z)
        {
            var intX = x as LSLListIntegerExpr;
            if (!(x is LSLListFloatExpr || x is LSLListVariableExpr || intX != null))
            {
                throw new ArgumentException("X is not an LSLFloat or LSLVariable.", "x");
            }

            var intY = y as LSLListIntegerExpr;
            if (!(y is LSLListFloatExpr || y is LSLListVariableExpr || intY != null))
            {
                throw new ArgumentException("Y is not an LSLFloat or LSLVariable.", "y");
            }

            var intZ = z as LSLListIntegerExpr;
            if (!(z is LSLListFloatExpr || z is LSLListVariableExpr || intZ != null))
            {
                throw new ArgumentException("Z is not an LSLFloat or LSLVariable.", "z");
            }

            X = intX != null ? new LSLListFloatExpr(intX.ValueString) : x;
            Y = intY != null ? new LSLListFloatExpr(intY.ValueString) : y;
            Z = intZ != null ? new LSLListFloatExpr(intZ.ValueString) : z;
        }


        /// <summary>
        ///     The X component.
        /// </summary>
        public ILSLListExpr X { get; private set; }

        /// <summary>
        ///     The Y component.
        /// </summary>
        public ILSLListExpr Y { get; private set; }

        /// <summary>
        ///     The Z component.
        /// </summary>
        public ILSLListExpr Z { get; private set; }

        /// <summary>
        ///     True if this list item represents a variable reference.
        /// </summary>
        public bool IsVariableReference
        {
            get { return false; }
        }

        /// <summary>
        ///     The list item type, it will be void if its a variable reference
        /// </summary>
        public LSLType Type
        {
            get { return LSLType.Vector; }
        }

        /// <summary>
        ///     Gets string representing the element, with quoting characters for the type.
        /// </summary>
        /// <value>
        ///     The value string.
        /// </value>
        public string ValueString
        {
            get { return "<" + X.ValueString + ", " + Y.ValueString + ", " + Z.ValueString + ">"; }
        }
    }
}