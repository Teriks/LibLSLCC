using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace LibLSLCC.ThreadSafeEnumeration
{
    public class ThreadSafeEnumerator<T> : IEnumerator<T>
    {
        // this is the (thread-unsafe)
        // enumerator of the underlying collection
        private readonly IEnumerator<T> _mInner;
        // this is the object we shall lock on. 
        private readonly object _mLock;



        public ThreadSafeEnumerator(IEnumerator<T> inner, object @lock)
        {
            _mInner = inner;
            _mLock = @lock;
            // entering lock in constructor
            Monitor.Enter(_mLock);
        }




        #region Implementation of IDisposable


        public void Dispose()
        {
            
            Dispose(true);
            GC.SuppressFinalize(this);

            
        }

        protected virtual void Dispose(bool disposing)
        {
            // .. and exiting lock on Dispose()
            // This will be called when foreach loop finishes
            Monitor.Exit(_mLock);
        }


        #endregion




        #region Implementation of IEnumerator


        // we just delegate actual implementation
        // to the inner enumerator, that actually iterates
        // over some collection

        public bool MoveNext()
        {
            return _mInner.MoveNext();
        }



        public void Reset()
        {
            _mInner.Reset();
        }



        public T Current
        {
            get { return _mInner.Current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }


        #endregion
    }
}