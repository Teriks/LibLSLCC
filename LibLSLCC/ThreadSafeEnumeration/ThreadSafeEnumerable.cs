using System.Collections;
using System.Collections.Generic;

namespace LibLSLCC.ThreadSafeEnumeration
{
    public class ThreadSafeEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _mInner;
        private readonly object _mLock;



        public ThreadSafeEnumerable(IEnumerable<T> inner, object @lock)
        {
            _mLock = @lock;
            _mInner = inner;
        }




        #region Implementation of IEnumerable


        public IEnumerator<T> GetEnumerator()
        {
            return new ThreadSafeEnumerator<T>(_mInner.GetEnumerator(), _mLock);
        }



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        #endregion
    }
}