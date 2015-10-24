#region FileInfo
// 
// File: IReadOnlyHashedSet.cs
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
    /// Read Set interface.
    /// </summary>
    /// <typeparam name="T">The type the set is to contain.</typeparam>
    public interface IReadOnlyHashedSet<T> : IReadOnlyContainer<T>
    {
        /// <summary>
        /// Determines if this ICollection is read only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        new IEnumerator<T> GetEnumerator();

        /// <summary>
        /// Determine if the set contains the given object.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>True if the set contains the given item.</returns>
        bool Contains(T item);

        /// <summary>
        /// Copies the elements of the <see cref="IReadOnlyHashedSet{T}"/> to an array, starting at arrayIndex in the target array.
        /// </summary>
        /// <param name="array">The array to copy the items to.</param>
        /// <param name="arrayIndex">The array index to start at in the target array.</param>
        void CopyTo(T[] array, int arrayIndex);

        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if this set is a subset of the other collection.</returns>
        bool IsSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Determines whether the current set is a superset of the specified collection.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if this set is a superset of the specified collection.</returns>
        bool IsSupersetOf(IEnumerable<T> other);


        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if the current set is a proper (string) superset of a specified collection.</returns>
        bool IsProperSupersetOf(IEnumerable<T> other);

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if the current set is a proper (string) subset of a specified collection.</returns>
        bool IsProperSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if the current set overlaps with the specified collection.</returns>
        bool Overlaps(IEnumerable<T> other);

        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if the current set and the specified collection contain the same elements.</returns>
        bool SetEquals(IEnumerable<T> other);
    }
}