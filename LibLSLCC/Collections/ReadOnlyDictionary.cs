using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace LibLSLCC.Collections
{
    [Serializable]
    public class ReadOnlyDictionary<TKey,TValue> : IReadOnlyDictionary<TKey, TValue>, ISerializable, IDeserializationCallback
    {
        readonly IDictionary<TKey,TValue> _items;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> items)
        {
            _items = items;
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected ReadOnlyDictionary(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info,context);
        } 

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _items).GetEnumerator();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _items.ToList().CopyTo(array, arrayIndex);
        }



        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool ContainsKey(TKey key)
        {
            return _items.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _items.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return _items[key]; }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return _items.Keys; }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return _items.Values; }
        }

        public ICollection<TKey> Keys
        {
            get { return _items.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return _items.Values; }
        }



        virtual public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ((ISerializable) _items).GetObjectData(info, context);
        }

        public void OnDeserialization(object sender)
        {
            ((IDeserializationCallback) _items).OnDeserialization(sender);
        }
    }
}
