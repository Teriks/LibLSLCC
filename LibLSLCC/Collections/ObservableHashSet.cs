#region FileInfo

// 
// File: ObservableHashSet.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

#endregion

namespace LibLSLCC.Collections
{
    /// <summary>
    ///     Observable and bindable HashSet
    /// </summary>
    /// <seealso cref="IObservableHashSetItem" />
    /// <typeparam name="T">The item type the hash set holds.</typeparam>
    public class ObservableHashSet<T> : ObservableCollection<T>, ICloneable
    {
        private readonly HashSet<T> _hashSet;


        /// <summary>
        ///     Construct an empty ObservableHashSet
        /// </summary>
        public ObservableHashSet()
        {
            _hashSet = new HashSet<T>();
        }


        /// <summary>
        ///     Construct an ObservableHashSet using the elements from an <see cref="IEnumerable{T}" />
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable{T}" /> to draw elements from.</param>
        public ObservableHashSet(IEnumerable<T> collection)
        {
            _hashSet = new HashSet<T>();

            foreach (var item in collection.Where(x => !_hashSet.Contains(x)))
            {
                Add(item);
            }
        }


        /// <summary>
        ///     Creates a shallow copy of this ObservableHashSet.
        /// </summary>
        public virtual object Clone()
        {
            return new ObservableHashSet<T>(_hashSet);
        }


        /// <summary>
        ///     Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            foreach (var item in _hashSet)
            {
                var observable = item as IObservableHashSetItem;

                if (observable != null)
                {
                    observable.PropertyChanging -= ObservableOnPropertyChanging;
                }
            }

            _hashSet.Clear();

            base.ClearItems();
        }


        /// <summary>
        ///     Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            var item = this[index];
            if (_hashSet.Remove(item))
            {
                var observable = item as IObservableHashSetItem;

                if (observable != null)
                {
                    observable.PropertyChanging -= ObservableOnPropertyChanging;
                }
            }

            base.RemoveItem(index);
        }


        /// <summary>
        ///     Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            if (_hashSet.Contains(item)) return;

            _hashSet.Add(item);


            var observable = item as IObservableHashSetItem;
            if (observable != null)
            {
                observable.PropertyChanging += ObservableOnPropertyChanging;
            }

            base.InsertItem(index, item);
        }


        /// <summary>
        ///     Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            if (_hashSet.Contains(item)) return;


            var observable = item as IObservableHashSetItem;
            if (observable != null)
            {
                observable.PropertyChanging += ObservableOnPropertyChanging;
            }

            _hashSet.Add(item);


            base.SetItem(index, item);
        }


        private static void ObservableOnPropertyChanging(object sender,
            PropertyChangingEventArgs propertyChangingEventArgs)
        {
            var item = sender as IObservableHashSetItem;

            if (item != null && item.HashEqualityPropertyNames.Contains(propertyChangingEventArgs.PropertyName))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Cannot change property '{0}' of type '{1}' when the object belongs to an ObservableHashSet, as it will effect the hash value of the object.",
                        propertyChangingEventArgs.PropertyName, item.GetType().Name));
            }
        }
    }
}