#region FileInfo
// 
// File: UniqueValueDictionary.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#endregion

namespace LibLSLCC.Collections
{
    public class UniqueValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly HashSet<TValue> _usedValues = new HashSet<TValue>();
        private Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue>();

        public Dictionary<TKey, TValue> Items
        {
            get { return _items; }
            private set { _items = value; }
        }


        public ReadOnlyHashSet<TValue> ValueSet
        {
            get { return new ReadOnlyHashSet<TValue>(_usedValues); }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Items).GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _usedValues.Add(item.Value);
            Items.Add(item.Key, item.Value);
            InvokeOnAdd(item.Key, item.Value);
        }

        public void Clear()
        {
            _usedValues.Clear();
            Items.Clear();
            InvokeOnClear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Items.ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            _usedValues.Remove(item.Value);
            var removed = Items.Remove(item.Key);

            if (removed)
            {
                InvokeOnRemove(item.Key, item.Value);
            }

            return removed;
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary<TKey, TValue>) Items).IsReadOnly; }
        }

        public bool ContainsKey(TKey key)
        {
            return Items.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _usedValues.Add(value);
            Items.Add(key, value);
            InvokeOnAdd(key, value);
        }

        public bool Remove(TKey key)
        {
            TValue itemValue;
            if (!Items.TryGetValue(key, out itemValue))
            {
                return false;
            }


            var removed = Items.Remove(key);
            if (removed)
            {
                _usedValues.Remove(itemValue);
                InvokeOnRemove(key, itemValue);
            }

            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Items.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return Items[key]; }
            set
            {
                TValue itemValue;
                if (Items.TryGetValue(key, out itemValue))
                {
                    _usedValues.Remove(itemValue);
                    _usedValues.Add(value);
                }
                else
                {
                    _usedValues.Add(value);
                }
                Items[key] = value;
            }
        }

        public ICollection<TKey> Keys
        {
            get { return Items.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return Items.Values; }
        }

        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<object, UniqueValueDictionaryEventArgs> OnAdd;

        protected virtual void InvokeOnAdd(TKey arg1, TValue arg2)
        {
            var handler = OnAdd;
            if (handler != null) handler(this, new UniqueValueDictionaryEventArgs(arg1, arg2));
        }

        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<object, UniqueValueDictionaryEventArgs> OnRemove;

        protected virtual void InvokeOnRemove(TKey arg1, TValue arg2)
        {
            var handler = OnRemove;
            if (handler != null) handler(this, new UniqueValueDictionaryEventArgs(arg1, arg2));
        }

        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<object, EventArgs> OnClear;

        protected virtual void InvokeOnClear()
        {
            var handler = OnClear;
            if (handler != null) handler(this, new EventArgs());
        }

        public class UniqueValueDictionaryEventArgs : EventArgs
        {
            public UniqueValueDictionaryEventArgs(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }

            public TKey Key { get; private set; }
            public TValue Value { get; private set; }
        }
    }
}