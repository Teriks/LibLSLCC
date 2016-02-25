#region FileInfo

// 
// File: ObservableDictionary.cs
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

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

#endregion

namespace LibLSLCC.Collections
{
    /// <summary>
    ///     Observable and bindable dictionary implementation.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        private readonly KeyedDictCollection _keyed;


        /// <summary>
        ///     Construct an empty ObservableDictionary.
        /// </summary>
        public ObservableDictionary()
        {
            _keyed = new KeyedDictCollection();
        }


        /// <summary>
        ///     Construct an observable dictionary from an existing dictionary.
        /// </summary>
        /// <param name="other">The other dictionary.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> other)
        {
            _keyed = new KeyedDictCollection(other);
        }


        /// <summary>
        ///     Construct an observable dictionary from an existing dictionary and a key comparer.
        /// </summary>
        /// <param name="other">The other dictionary.</param>
        /// <param name="comparer">A comparer.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> other, IEqualityComparer<TKey> comparer)
        {
            _keyed = new KeyedDictCollection(other, comparer);
        }


        /// <summary>
        ///     Whether or not this collection is read only.  (<c>false</c>)
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     The keys currently in use in this dictionary.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return _keyed.Items.Select(x => x.Key).ToList(); }
        }

        /// <summary>
        ///     The values currently in use in this dictionary
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return _keyed.Items.Select(x => x.Value).ToList(); }
        }


        /// <summary>
        ///     Returns an enumerator over the key value pairs in this dictionary.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _keyed.Select(x => x).GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return _keyed.Select(x => x).GetEnumerator();
        }


        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            _keyed.Add(item);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnPropertyChanged("Values");
            OnPropertyChanged("Keys");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }


        /// <summary>
        ///     Clear the dictionary.
        /// </summary>
        public void Clear()
        {
            int cnt = _keyed.Count;
            _keyed.Clear();
            if (cnt > 0)
            {
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnPropertyChanged("Values");
                OnPropertyChanged("Keys");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }


        /// <summary>
        ///     Check if this dictionary contains a specific key value pair.
        /// </summary>
        /// <param name="item">The pair to check for.</param>
        /// <returns><c>true</c> if this dictionary contains the specified pair.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _keyed.Contains(item);
        }


        /// <summary>
        ///     Copies the key value pairs in this dictionary to an array starting at <paramref name="arrayIndex" />.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index in <paramref name="arrayIndex" /> at which to start copying</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _keyed.CopyTo(array, arrayIndex);
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!_keyed.Remove(item)) return false;

            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnPropertyChanged("Values");
            OnPropertyChanged("Keys");

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));

            return true;
        }


        /// <summary>
        ///     The number of items currently in this dictionary.
        /// </summary>
        public int Count
        {
            get { return _keyed.Count; }
        }


        /// <summary>
        ///     Check if this dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <returns><c>true</c> if this dictionary contains the specified key.</returns>
        public bool ContainsKey(TKey key)
        {
            return _keyed.Contains(key);
        }


        /// <summary>
        ///     Add a value with the given key to this dictionary.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <param name="value">The value to associate with the key.</param>
        public void Add(TKey key, TValue value)
        {
            _keyed.Add(new KeyValuePair<TKey, TValue>(key, value));

            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnPropertyChanged("Values");
            OnPropertyChanged("Keys");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                new KeyValuePair<TKey, TValue>(key, value)));
        }


        /// <summary>
        ///     Remove a value from this dictionary given its key.
        /// </summary>
        /// <param name="key">The key the value is associated with.</param>
        /// <returns><c>true</c> if the key value pair was removed, <c>false</c> if the key was not found.</returns>
        public bool Remove(TKey key)
        {
            if (_keyed.Contains(key))
            {
                var item = _keyed[key];
                var index = _keyed.IndexOf(item);

                _keyed.Remove(key);

                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnPropertyChanged("Values");
                OnPropertyChanged("Keys");

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item,
                    index));
                return true;
            }
            return false;
        }


        /// <summary>
        ///     Attempt to get the value associated with a given key.
        /// </summary>
        /// <param name="key">The key to try to get the value for.</param>
        /// <param name="value">The found value is put here, otherwise this 'out' variable is assigned <c>null</c>.</param>
        /// <returns><c>true</c> if the key existed and the value was found.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_keyed.Contains(key))
            {
                value = _keyed[key].Value;
                return true;
            }
            value = default(TValue);
            return false;
        }


        /// <summary>
        ///     Return a given value by its key.
        /// </summary>
        /// <param name="key">The key that is associated with the value.</param>
        /// <returns>The value if it exists.</returns>
        public TValue this[TKey key]
        {
            get { return _keyed[key].Value; }
            set
            {
                var oldItem = _keyed[key];
                var newItem = new KeyValuePair<TKey, TValue>(key, value);
                var index = _keyed.IndexOf(oldItem);

                _keyed.Dictionary[key] = newItem;


                if (_IsEqual(oldItem.Value, value)) return;

                OnPropertyChanged("Item[]");
                OnPropertyChanged("Values");

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                    oldItem, newItem, index));
            }
        }

        /// <summary>
        ///     This event is raised when this dictionary's item collection is changed.
        /// </summary>
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        ///     This event is raised when a property of this dictionary changes. (such as Count, Item[], Values, or Keys)
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        private static bool _IsEqual(TValue left, TValue right)
        {
            return EqualityComparer<TValue>.Default.Equals(left, right);
        }


        /// <summary>
        ///     This event is raised when this dictionary's item collection is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged.Invoke(this, e);
            }
        }


        /// <summary>
        ///     This event is raised when a property of this dictionary changes. (such as Count, Item[], Values, or Keys)
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        private class KeyedDictCollection : KeyedCollection<TKey, KeyValuePair<TKey, TValue>>
        {
            public KeyedDictCollection()
            {
            }


            public KeyedDictCollection(IDictionary<TKey, TValue> other) : base(EqualityComparer<TKey>.Default, 0)
            {
                foreach (var i in other)
                {
                    Add(i);
                }
            }


            public KeyedDictCollection(IDictionary<TKey, TValue> other, IEqualityComparer<TKey> comparer)
                : base(comparer, 0)
            {
                foreach (var i in other)
                {
                    Add(i);
                }
            }


            public new IDictionary<TKey, KeyValuePair<TKey, TValue>> Dictionary
            {
                get { return base.Dictionary; }
            }

            public new ICollection<KeyValuePair<TKey, TValue>> Items
            {
                get { return base.Items; }
            }


            protected override TKey GetKeyForItem(KeyValuePair<TKey, TValue> item)
            {
                return item.Key;
            }
        }
    }
}