using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    public static class GenericArrayExtensions
    {
        public static GenericArray<T> ToGenericArray<T>(this IEnumerable<T> enumerable)
        {
            return new GenericArray<T>(enumerable);
        }

        public static GenericArray<T> ToGenericArray<T>(this IList<T> list)
        {
            return new GenericArray<T>(list);
        }
    }
}