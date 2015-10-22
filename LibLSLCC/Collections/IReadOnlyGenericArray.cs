using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    public interface IReadOnlyGenericArray<out T> : IEnumerable<T>
    {

        int Count { get; }
        T this[int index] { get; }
    }
}