#region FileInfo

// 
// File: IReadOnlyHashMap.cs
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
    ///     Read only HashMap interface, used by <see cref="HashMap{TKey,TValue}" />
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IReadOnlyHashMap<TKey, TValue> : IReadOnlyContainer<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <value>
        ///     The value associated with the given key.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The value associated with the specified key</returns>
        TValue this[TKey key] { get; }

        /// <summary>
        ///     Gets the keys this <see cref="HashMap{TKey,TValue}" /> contains.
        /// </summary>
        /// <value>
        ///     The keys.
        /// </value>
        IEnumerable<TKey> Keys { get; }

        /// <summary>
        ///     Gets the values this <see cref="HashMap{TKey,TValue}" /> contains.
        /// </summary>
        /// <value>
        ///     The values.
        /// </value>
        IEnumerable<TValue> Values { get; }


        /// <summary>
        ///     Determines whether this <see cref="HashMap{TKey,TValue}" /> contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if this <see cref="HashMap{TKey,TValue}" /> contains the specified <paramref name="key"/>; otherwise <c>false</c>.</returns>
        bool ContainsKey(TKey key);


        /// <summary>
        ///     Tries to put the value associated with a given key into the out <paramref name="value" /> parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value output location.</param>
        /// <returns>True if the value was found and retrieved, false if it did not exist.</returns>
        bool TryGetValue(TKey key, out TValue value);
    }
}