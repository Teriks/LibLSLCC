using System;
using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    /// <summary>
    /// Extensions for <see cref="HashMap{TKey,TValue}"/> objects.
    /// </summary>
    public static class HashMapExtensions
    {

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> object into a <see cref="HashMap{TKey,TSource}"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to convert.</param>
        /// <param name="keySelector">The key selector function.</param>
        /// <typeparam name="TSource"> </typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns>The generated <see cref="HashMap{TKey,TValue}"/> object.</returns>
        public static HashMap<TKey, TSource> ToHashMap<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.ToHashMap(keySelector, x => x, EqualityComparer<TKey>.Default);
        }


        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> object into a <see cref="HashMap{TKey,TSource}"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to convert.</param>
        /// <param name="keySelector">The key selector function.</param>
        /// <param name="elementSelector">The element selector function.</param>
        /// <typeparam name="TSource"> </typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <returns>The generated <see cref="HashMap{TKey,TValue}"/> object.</returns>
        public static HashMap<TKey, TElement> ToHashMap<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            return source.ToHashMap(keySelector, elementSelector, EqualityComparer<TKey>.Default);
        }


        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> object into a <see cref="HashMap{TKey,TSource}"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to convert.</param>
        /// <param name="keySelector">The key selector function.</param>
        /// <param name="comparer">The key comparer function.</param>
        /// <typeparam name="TSource"> </typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns>The generated <see cref="HashMap{TKey,TValue}"/> object.</returns>
        public static HashMap<TKey, TSource> ToHashMap<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            return source.ToHashMap(keySelector, x => x, comparer);
        }


        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> object into a <see cref="HashMap{TKey,TSource}"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to convert.</param>
        /// <param name="keySelector">The key selector function.</param>
        /// <param name="elementSelector">The element selector function.</param>
        /// <param name="comparer">The key comparer function.</param>
        /// <typeparam name="TSource"> </typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <returns>The generated <see cref="HashMap{TKey,TValue}"/> object.</returns>
        public static HashMap<TKey, TElement> ToHashMap<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (elementSelector == null)
            {
                throw new ArgumentNullException("elementSelector");
            }
            comparer = comparer ?? EqualityComparer<TKey>.Default;
            ICollection<TSource> list = source as ICollection<TSource>;
            var ret = list == null ? new HashMap<TKey, TElement>(comparer) : new HashMap<TKey, TElement>(list.Count, comparer);
            foreach (TSource item in source)
            {
                ret.Add(keySelector(item), elementSelector(item));
            }
            return ret;
        }

    }
}