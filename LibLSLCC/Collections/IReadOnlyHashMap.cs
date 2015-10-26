using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    /// <summary>
    /// Read only HashMap interface, used by <see cref="HashMap{TKey,TValue}"/>
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IReadOnlyHashMap<TKey, TValue> : IReadOnlyContainer<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <value>
        /// The value associated with the given key.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        TValue this[TKey key] { get; }

        /// <summary>
        /// Gets the keys this <see cref="HashMap{TKey,TValue}"/> contains.
        /// </summary>
        /// <value>
        /// The keys.
        /// </value>
        IEnumerable<TKey> Keys { get; }

        /// <summary>
        /// Gets the values this <see cref="HashMap{TKey,TValue}"/> contains.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        IEnumerable<TValue> Values { get; }

        /// <summary>
        /// Determines whether this <see cref="HashMap{TKey,TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        bool ContainsKey(TKey key);

        /// <summary>
        /// Tries to put the value associated with a given key into the out <paramref name="value"/> parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value output location.</param>
        /// <returns>True if the value was found and retrieved, false if it did not exist.</returns>
        bool TryGetValue(TKey key, out TValue value);
    }
}