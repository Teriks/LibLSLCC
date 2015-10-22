using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    public interface IReadOnlyHashMap<TKey, TValue> : IReadOnlyContainer<KeyValuePair<TKey, TValue>>
    {
        TValue this[TKey key] { get; }
        IEnumerable<TKey> Keys { get; }
        IEnumerable<TValue> Values { get; }

        bool ContainsKey(TKey key);
        bool TryGetValue(TKey key, out TValue value);
    }
}