using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibLSLCC.Collections
{
    /*
    public interface IReadOnlyList<T> : IEnumerable<T>
    {
        bool Contains(T item);

        void CopyTo(T[] array, int arrayIndex);


        int Count { get;  }

        bool IsReadOnly { get; }

        int IndexOf(T item);



        T this[int index] { get; }
    }*/


    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private IList<T> _data;


        public ReadOnlyList(IEnumerable<T> data)
        {
            _data = new List<T>(data);
        }


        public ReadOnlyList(IList<T> data)
        {
            _data = data;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return _data.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public int Count { get { return _data.Count; } }
        public bool IsReadOnly { get { return true; } }
        public int IndexOf(T item)
        {
            return _data.IndexOf(item);
        }

        public T this[int index]
        {
            get { return _data[index]; }
        }
    }
}
