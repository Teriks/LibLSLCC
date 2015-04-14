using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    public static class CollectionExtensions
    {
        public static IReadOnlyDictionary<K,V> AsReadOnly<K,V> (this IDictionary<K, V>  dict)
        {
            return new ReadOnlyDictionary<K, V>(dict);
        }

        public static IReadOnlySet<T> AsReadOnly<T>(this ISet<T> set)
        {
            return new ReadOnlyHashSet<T>(set);
        } 
    }
}