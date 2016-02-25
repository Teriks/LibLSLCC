#region FileInfo

// 
// File: GenericArrayExtensions.cs
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

using System.Collections.Generic;

#endregion

namespace LibLSLCC.Collections
{
    /// <summary>
    ///     Extensions for The <seealso cref="GenericArray{T}" /> collection.
    /// </summary>
    public static class GenericArrayExtensions
    {
        /// <summary>
        ///     Converts an <see cref="IEnumerable{T}" /> to a <seealso cref="GenericArray{T}" /> object.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}" /> to convert.</param>
        /// <typeparam name="T">The type contained in the <see cref="IEnumerable{T}" /> object.</typeparam>
        /// <returns>A <see cref="GenericArray{T}" /> object filled with the contents of the given <see cref="IEnumerable{T}" />.</returns>
        public static GenericArray<T> ToGenericArray<T>(this IEnumerable<T> enumerable)
        {
            return new GenericArray<T>(enumerable);
        }


        /// <summary>
        ///     Converts an <see cref="IList{T}" /> to a <seealso cref="GenericArray{T}" /> object by wrapping it.
        /// </summary>
        /// <param name="list">The <see cref="IList{T}" /> to convert.</param>
        /// <typeparam name="T">The type contained in the <see cref="IList{T}" /> object.</typeparam>
        /// <returns>A <see cref="GenericArray{T}" /> object filled with the contents of the given <see cref="IEnumerable{T}" />.</returns>
        public static GenericArray<T> WrapWithGenericArray<T>(this IList<T> list)
        {
            return GenericArray<T>.CreateWrapper(list);
        }
    }
}