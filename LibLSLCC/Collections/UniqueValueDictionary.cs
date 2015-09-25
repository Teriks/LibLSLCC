#region FileInfo

// 
// File: UniqueValueDictionary.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:25 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
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
        public Dictionary<TKey, TValue> Items { get; } = new Dictionary<TKey, TValue>();

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