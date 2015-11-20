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

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace LibLSLCC.Collections
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged,
        INotifyPropertyChanged
    {
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

        private readonly KeyedDictCollection _keyed;

        public ObservableDictionary()
        {
            _keyed = new KeyedDictCollection();
        }


        public ObservableDictionary(IDictionary<TKey, TValue> other)
        {
            _keyed = new KeyedDictCollection(other);
        }

        public ObservableDictionary(IDictionary<TKey, TValue> other, IEqualityComparer<TKey> comparer)
        {
            _keyed = new KeyedDictCollection(other, comparer);
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        public ICollection<TKey> Keys
        {
            get { return _keyed.Items.Select(x => x.Key).ToList(); }
        }

        public ICollection<TValue> Values
        {
            get { return _keyed.Items.Select(x => x.Value).ToList(); }
        }


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

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return _keyed.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _keyed.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_keyed.Remove(item))
            {
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnPropertyChanged("Values");
                OnPropertyChanged("Keys");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                return true;
            }
            return false;
        }

        public int Count
        {
            get { return _keyed.Count; }
        }

        public bool ContainsKey(TKey key)
        {
            return _keyed.Contains(key);
        }

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

        public TValue this[TKey key]
        {
            get { return _keyed[key].Value; }
            set
            {
                var oldItem = _keyed[key];
                var newItem = new KeyValuePair<TKey, TValue>(key, value);
                var index = _keyed.IndexOf(oldItem);

                _keyed.Dictionary[key] = newItem;


                if (!_IsEqual(oldItem.Value, value))
                {
                    OnPropertyChanged("Item[]");
                    OnPropertyChanged("Values");

                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                        oldItem, newItem, index));
                }
            }
        }

        private bool _IsEqual(TValue left, TValue right)
        {
            return EqualityComparer<TValue>.Default.Equals(left, right);
        }

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged.Invoke(this, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}