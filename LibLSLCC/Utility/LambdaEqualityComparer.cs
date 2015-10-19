using System;
using System.Collections.Generic;

namespace LibLSLCC.Utility
{

    /// <summary>
    /// Implements the generic IEqualityComparer interface by delegating comparisons and hash 
    /// code generation to function objects that have the capability of being lambdas.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LambdaEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// The function object that is used to generate hash codes.
        /// </summary>
        public  Func<T, int> Hash { get; set; }

        /// <summary>
        /// Construct a LambdaEqualityComparer using a comparison function, and an optional hash code generation function.
        /// If a hash code generation function is not provided, hash code generation is implemented by calling GetHashCode()
        /// on the given object.
        /// </summary>
        /// <param name="cmp">The comparison function to use.</param>
        /// <param name="hash">The optional hash code generation function to use.</param>
        public LambdaEqualityComparer(Func<T, T, bool> cmp, Func<T, int> hash = null)
        {
            Hash = hash;
            Cmp = cmp;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param><param name="y">The second object of type <paramref name="T"/> to compare.</param>
        public bool Equals(T x, T y)
        {
            return Cmp(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public int GetHashCode(T obj)
        {
            return Hash != null ? Hash(obj) : obj.GetHashCode();
        }

        /// <summary>
        /// The function object that is used for comparisons.
        /// </summary>
        public Func<T, T, bool> Cmp { get; set; }
    }
}