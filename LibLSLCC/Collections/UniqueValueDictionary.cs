#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LibLSLCC.Collections
{
    public class UniqueValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue>();
        private readonly HashSet<TValue> _usedValues = new HashSet<TValue>();

        public Dictionary<TKey, TValue> Items
        {
            get { return _items; }
        }

        public ReadOnlyHashSet<TValue> ValueSet
        {
            get { return new ReadOnlyHashSet<TValue>(_usedValues); }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _items).GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _usedValues.Add(item.Value);
            _items.Add(item.Key, item.Value);
            InvokeOnAdd(item.Key, item.Value);
        }

        public void Clear()
        {
            _usedValues.Clear();
            _items.Clear();
            InvokeOnClear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _items.ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            _usedValues.Remove(item.Value);
            var removed = _items.Remove(item.Key);

            if (removed)
            {
                InvokeOnRemove(item.Key, item.Value);
            }

            return removed;
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary<TKey, TValue>) _items).IsReadOnly; }
        }

        public bool ContainsKey(TKey key)
        {
            return _items.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _usedValues.Add(value);
            _items.Add(key, value);
            InvokeOnAdd(key, value);
        }

        public bool Remove(TKey key)
        {
            TValue itemValue;
            if (!_items.TryGetValue(key, out itemValue))
            {
                return false;
            }


            var removed = _items.Remove(key);
            if (removed)
            {
                _usedValues.Remove(itemValue);
                InvokeOnRemove(key, itemValue);
            }

            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _items.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return _items[key]; }
            set
            {
                TValue itemValue;
                if (_items.TryGetValue(key, out itemValue))
                {
                    _usedValues.Remove(itemValue);
                    _usedValues.Add(value);
                }
                else
                {
                    _usedValues.Add(value);
                }
                _items[key] = value;
            }
        }

        public ICollection<TKey> Keys
        {
            get { return _items.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return _items.Values; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<object, UniqueValueDictionaryEventArgs> OnAdd;

        protected virtual void InvokeOnAdd(TKey arg1, TValue arg2)
        {
            Action<object, UniqueValueDictionaryEventArgs> handler = OnAdd;
            if (handler != null) handler(this, new UniqueValueDictionaryEventArgs(arg1, arg2));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<object, UniqueValueDictionaryEventArgs> OnRemove;

        protected virtual void InvokeOnRemove(TKey arg1, TValue arg2)
        {
            Action<object, UniqueValueDictionaryEventArgs> handler = OnRemove;
            if (handler != null) handler(this, new UniqueValueDictionaryEventArgs(arg1, arg2));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<object, EventArgs> OnClear;

        protected virtual void InvokeOnClear()
        {
            Action<object, EventArgs> handler = OnClear;
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