#region FileInfo
// 
// File: HashedSet.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LibLSLCC.Collections
{

    /// <summary>
    /// A hashed set collection meant to contain unique items.  It is equivalent to <see cref="HashSet{T}"/>
    /// </summary>
    /// <remarks>
    /// This collection supports a read only interface in NET 4.0 via <see cref="IReadOnlyHashedSet{T}"/>.
    /// </remarks>
    /// <typeparam name="T">The type of object(s) that the <see cref="HashSet{T}"/> can contain.</typeparam>
    public class HashedSet<T> : IReadOnlyHashedSet<T>, ISet<T>, ICloneable
    {
        private readonly ISet<T> _items;



        /// <summary>
        /// Initializes a new instance of the <see cref="HashedSet{T}"/> classby wrapping an <see cref="ISet{T}"/>.
        /// </summary>
        /// <param name="items">The items.</param>
        private HashedSet(ISet<T> items)
        {
            _items = items;
        }


        /// <summary>
        /// Create a new instance of an <see cref="HashedSet{T}"/> class by wrapping an <see cref="ISet{T}"/>.
        /// </summary>
        /// <param name="items">The items.</param>
        public static ISet<T> CreateWrapper(ISet<T> items)
        {
            return new HashedSet<T>(items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedSet{T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="comparer">The comparer.</param>
        public HashedSet(IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            _items = new HashSet<T>(items, comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedSet{T}"/> class.
        /// </summary>
        public HashedSet()
        {
            _items = new HashSet<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedSet{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public HashedSet(IEqualityComparer<T> comparer)
        {
            _items = new HashSet<T>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedSet{T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public HashedSet(IEnumerable<T> items)
        {
            _items = new HashSet<T>(items);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _items).GetEnumerator();
        }

        public bool Add(T item)
        {
            return _items.Add(item);
        }

        bool ISet<T>.Add(T item)
        {
            return _items.Add(item);
        }

        /// <summary>
        /// Modifies the current set so that it contains all elements that are present in both the current set and in the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
        public void UnionWith(IEnumerable<T> other)
        {
            _items.UnionWith(other);
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
        public void IntersectWith(IEnumerable<T> other)
        {
            _items.IntersectWith(other);
        }

        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
        public void ExceptWith(IEnumerable<T> other)
        {
            _items.ExceptWith(other);
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the current set or in the specified collection, but not both. 
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is null.</exception>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _items.SymmetricExceptWith(other);
        }


        void ICollection<T>.Add(T item)
        {
            _items.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Determine if the set contains the given object.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>True if the set contains the given item.</returns>
        public bool Contains(T item)
        {
            return _items.Contains(item);
        }


        /// <summary>
        /// Copies the elements of the <see cref="IReadOnlyHashedSet{T}"/> to an array, starting at arrayIndex in the target array.
        /// </summary>
        /// <param name="array">The array to copy the items to.</param>
        /// <param name="arrayIndex">The array index to start at in the target array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Determines if this ICollection is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _items.IsReadOnly; }
        }

        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if this set is a subset of the other collection.</returns>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _items.IsSubsetOf(other);
        }


        /// <summary>
        /// Determines whether the current set is a superset of the specified collection.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if this set is a superset of the specified collection.</returns>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _items.IsSupersetOf(other);
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if the current set is a proper (string) superset of a specified collection.</returns>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _items.IsProperSupersetOf(other);
        }


        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if the current set is a proper (string) subset of a specified collection.</returns>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _items.IsProperSubsetOf(other);
        }


        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if the current set overlaps with the specified collection.</returns>
        public bool Overlaps(IEnumerable<T> other)
        {
            return _items.Overlaps(other);
        }

        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The other collection.</param>
        /// <returns>True if the current set and the specified collection contain the same elements.</returns>
        public bool SetEquals(IEnumerable<T> other)
        {
            return _items.SetEquals(other);
        }


        public object Clone()
        {
            return new HashSet<T>(this.AsEnumerable());
        }
    }
}