using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibLSLCC.Collections
{
    public static class CloneableExtensions
    {
        /// <summary>
        /// Clones all elements of an <see cref="IEnumerable{T}"/> that enumerates objects implementing <see cref="ICloneable"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable of <see cref="ICloneable"/> implementors.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that enumerates over clones of every item in <paramref name="enumerable"/>.</returns>
        public static IEnumerable<T> CloneAll<T>(this IEnumerable<T> enumerable) where T : ICloneable
        {
            return enumerable.Select(x => (T)x.Clone());
        } 
    }
}
