using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    public static class CollectionExtensions
    {
        public static IReadOnlyDictionary<TK,TV> AsReadOnly<TK,TV> (this IDictionary<TK, TV>  dict)
        {
            return new ReadOnlyDictionary<TK, TV>(dict);
        }

        public static IReadOnlySet<T> AsReadOnly<T>(this ISet<T> set)
        {
            return new ReadOnlyHashSet<T>(set);
        } 
    }
}