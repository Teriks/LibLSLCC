#region FileInfo
// 
// File: LambdaEqualityComparer.cs
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
using System.Collections.Generic;

namespace LibLSLCC.Utility
{

    /// <summary>
    /// Implements the generic IEqualityComparer interface by delegating comparisons and hash 
    /// code generation to function objects that have the capability of being lambdas.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LambdaEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// The function object that is used to generate hash codes.
        /// </summary>
        public  Func<T, int> Hash { get; set; }

        /// <summary>
        /// Construct a LambdaEqualityComparer using a comparison function, and an optional hash code generation function.
        /// If a hash code generation function is not provided, hash code generation is implemented by calling GetHashCode()
        /// on the given object.
        /// </summary>
        /// <param name="cmp">The comparison function to use.</param>
        /// <param name="hash">The optional hash code generation function to use.</param>
        public LambdaEqualityComparer(Func<T, T, bool> cmp, Func<T, int> hash = null)
        {
            Hash = hash;
            Cmp = cmp;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <typeparamref name="T"/> to compare.</param><param name="y">The second object of type <typeparamref name="T"/> to compare.</param>
        public bool Equals(T x, T y)
        {
            return Cmp(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public int GetHashCode(T obj)
        {
            return Hash != null ? Hash(obj) : obj.GetHashCode();
        }

        /// <summary>
        /// The function object that is used for comparisons.
        /// </summary>
        public Func<T, T, bool> Cmp { get; set; }
    }
}