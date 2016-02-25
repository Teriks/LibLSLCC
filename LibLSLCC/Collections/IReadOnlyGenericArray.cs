#region FileInfo

// 
// File: IReadOnlyGenericArray.cs
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
    ///     Interface for read only Generic Arrays, used by <see cref="GenericArray{T}" />.
    /// </summary>
    /// <remarks>
    ///     This interface supports covariance.
    /// </remarks>
    /// <typeparam name="T">The type contained in the Generic Array.</typeparam>
    public interface IReadOnlyGenericArray<out T> : IEnumerable<T>
    {
        /// <summary>
        ///     Gets the number of items in the array.
        /// </summary>
        /// <value>
        ///     The number of items in the array.
        /// </value>
        int Count { get; }

        /// <summary>
        ///     Gets the array element at the specified index.
        /// </summary>
        /// <value>
        ///     The array element at the specified index.
        /// </value>
        /// <param name="index">The index to retrieve the array element from.</param>
        /// <returns>The array element at the specified index.</returns>
        T this[int index] { get; }
    }
}