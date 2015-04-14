using System.Collections;
using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    public class ReadOnlyHashSet<T> : IReadOnlySet<T>
    {
        private readonly ISet<T> _items;

        public ReadOnlyHashSet(ISet<T> items)
        {
            _items = items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _items).GetEnumerator();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return _items.IsReadOnly; }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _items.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _items.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _items.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _items.IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _items.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _items.SetEquals(other);
        }
    }
}