using System;
using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    public static class HashMapExtensions
    {
        public static HashMap<TKey, TSource> ToHashMap<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.ToHashMap(keySelector, x => x, EqualityComparer<TKey>.Default);
        }

        public static HashMap<TKey, TElement> ToHashMap<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            return source.ToHashMap(keySelector, elementSelector, EqualityComparer<TKey>.Default);
        }

        public static HashMap<TKey, TSource> ToHashMap<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            return source.ToHashMap(keySelector, x => x, comparer);
        }

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
            var ret = list == null ? new HashMap<TKey, TElement>(comparer)
                : new HashMap<TKey, TElement>(list.Count, comparer);
            foreach (TSource item in source)
            {
                ret.Add(keySelector(item), elementSelector(item));
            }
            return ret;
        }

    }
}