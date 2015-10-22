using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    public interface IReadOnlyContainer<T> : IEnumerable<T>
    {
        int Count { get; }
    }
}