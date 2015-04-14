using System.Collections.Generic;

namespace LibLSLCC.ThreadSafeEnumeration
{
    public static class ThreadSafeEnumerableExtension
    {
        public static IEnumerable<T> AsLocked<T>(this IEnumerable<T> ie, object @lock)
        {
            return new ThreadSafeEnumerable<T>(ie, @lock);
        }
    }
}