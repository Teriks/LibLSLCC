using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    public interface IReadOnlySet<T> : IReadOnlyCollection<T>
    {
        new IEnumerator<T> GetEnumerator();
        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);
        new int Count { get; }
        bool IsReadOnly { get; }
        bool IsSubsetOf(IEnumerable<T> other);
        bool IsSupersetOf(IEnumerable<T> other);
        bool IsProperSupersetOf(IEnumerable<T> other);
        bool IsProperSubsetOf(IEnumerable<T> other);
        bool Overlaps(IEnumerable<T> other);
        bool SetEquals(IEnumerable<T> other);
    }
}