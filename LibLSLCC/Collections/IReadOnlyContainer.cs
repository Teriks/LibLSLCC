using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    /// <summary>
    /// Interface for read only collections/containers.
    /// </summary>
    /// <remarks>
    /// This interface supports covariance.
    /// </remarks>
    /// <typeparam name="T">The type contained by the container.</typeparam>
    public interface IReadOnlyContainer<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the number of elements contained in the container.
        /// </summary>
        int Count { get; }
    }
}