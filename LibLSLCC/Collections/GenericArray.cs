using System;
using System.Collections;
using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    /// <summary>
    /// Generic Array class, it is equivalent to <see cref="List{TValue}"/>.
    /// </summary>
    /// <remarks>
    /// This class supports a read only covariant interface in NET 4.0 via <see cref="IReadOnlyGenericArray{T}"/>.
    /// </remarks>
    /// <typeparam name="T">The type contained by the <see cref="GenericArray{T}"/></typeparam>
    public class GenericArray<T> : IReadOnlyGenericArray<T>, IList<T>
    {
        private readonly IList<T> _data;


        /// <summary>
        /// Initializes a new instance of the <see cref="GenericArray{T}"/> class.
        /// </summary>
        public GenericArray()
        {
            _data = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericArray{T}"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public GenericArray(IList<T> list)
        {
            _data = list;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericArray{T}"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public GenericArray(IEnumerable<T> list)
        {
            _data = new List<T>(list);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GenericArray{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public GenericArray(int capacity)
        {
            _data = new List<T>(capacity);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(T item)
        {
            _data.Add(item);
        }

        /// <summary>
        /// Adds a range of elements to the array from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(T item)
        {
            return _data.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(T item)
        {
            return _data.Remove(item);
        }

        int ICollection<T>.Count { get { return _data.Count; } }

        /// <summary>
        /// Gets the number of items in the array.
        /// </summary>
        /// <value>
        /// The number of items in the array.
        /// </value>
        public int Count { get { return _data.Count; } }


        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly { get { return false; } }
        int IReadOnlyGenericArray<T>.Count { get { return _data.Count; } }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public int IndexOf(T item)
        {
            return _data.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void Insert(int index, T item)
        {
            _data.Insert(index, item);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void RemoveAt(int index)
        {
            _data.RemoveAt(index);
        }

        T IList<T>.this[int index]
        {
            get { return _data[index]; }
            set { _data[index] = value; }
        }

        public T this[int index]
        {
            get { return _data[index]; }
            set { _data[index] = value; }
        }

        T IReadOnlyGenericArray<T>.this[int index]
        {
            get { return _data[index]; }
        }


        /// <summary>
        /// Performs an implicit conversion from <see cref="List{T}"/> to <see cref="GenericArray{T}"/>.
        /// </summary>
        /// <param name="other">The <see cref="List{T}"/> to convert from.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator GenericArray<T>(List<T> other)
        {
            return new GenericArray<T>(other);
        }
    }
}
