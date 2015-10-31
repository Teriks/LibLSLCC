using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    /// <summary>
    /// Extensions for The <seealso cref="GenericArray{T}"/> collection.
    /// </summary>
    public static class GenericArrayExtensions
    {
        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> to a <seealso cref="GenericArray{T}"/> object.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to convert.</param>
        /// <typeparam name="T">The type contained in the <see cref="IEnumerable{T}"/> object.</typeparam>
        /// <returns>A <see cref="GenericArray{T}"/> object filled with the contents of the given <see cref="IEnumerable{T}"/>.</returns>
        public static GenericArray<T> ToGenericArray<T>(this IEnumerable<T> enumerable)
        {
            return new GenericArray<T>(enumerable);
        }


        /// <summary>
        /// Converts an <see cref="IList{T}"/> to a <seealso cref="GenericArray{T}"/> object by wrapping it.
        /// </summary>
        /// <param name="list">The <see cref="IList{T}"/> to convert.</param>
        /// <typeparam name="T">The type contained in the <see cref="IList{T}"/> object.</typeparam>
        /// <returns>A <see cref="GenericArray{T}"/> object filled with the contents of the given <see cref="IEnumerable{T}"/>.</returns>
        public static GenericArray<T> WrapWithGenericArray<T>(this IList<T> list)
        {
            return GenericArray<T>.CreateWrapper(list);
        }
    }
}