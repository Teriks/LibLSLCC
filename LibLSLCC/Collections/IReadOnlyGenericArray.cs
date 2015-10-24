using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    /// <summary>
    /// Interface for read only Generic Arrays, used by <see cref="GenericArray{T}"/>.
    /// </summary>
    /// <remarks>
    /// This interface supports covariance.
    /// </remarks>
    /// <typeparam name="T">The type contained in the Generic Array.</typeparam>
    public interface IReadOnlyGenericArray<out T> : IEnumerable<T>
    {

        /// <summary>
        /// Gets the number of items in the array.
        /// </summary>
        /// <value>
        /// The number of items in the array.
        /// </value>
        int Count { get; }

        /// <summary>
        /// Gets the array element at the specified index.
        /// </summary>
        /// <value>
        /// The array element at the specified index.
        /// </value>
        /// <param name="index">The index to retrieve the array element from.</param>
        /// <returns>The array element at the specified index.</returns>
        T this[int index] { get; }
    }
}