#region FileInfo
// 
// File: ThreadSafeEnumerator.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 
#endregion
#region Imports

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace LibLSLCC.ThreadSafeEnumeration
{
    /// <summary>
    /// A thread safe wrapper for objects that derive from IEnumerator
    /// </summary>
    /// <typeparam name="T">The type the enumerator is to enumerate over.</typeparam>
    public class ThreadSafeEnumerator<T> : IEnumerator<T>
    {
        // this is the (thread-unsafe)
        // enumerator of the underlying collection
        private readonly IEnumerator<T> _mInner;
        // this is the object we shall lock on. 
        private readonly object _mLock;


        /// <summary>
        /// Create a thread safe enumerator wrapper, using an object as a lock.
        /// </summary>
        /// <param name="inner">The enumerator to wrap.</param>
        /// <param name="lockObject">The object to use as a lock.</param>
        public ThreadSafeEnumerator(IEnumerator<T> inner, object lockObject)
        {
            _mInner = inner;
            _mLock = lockObject;
            // entering lock in constructor
            Monitor.Enter(_mLock);
        }

        #region Implementation of IDisposable


        /// <summary>
        /// Dispose the enumerator, Calls Monitor.Exit() and GC.SuppressFinalize(true);
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the enumerator, Calls Monitor.Exit();
        /// </summary>
        /// <param name="disposing">True if the object is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            // .. and exiting lock on Dispose()
            // This will be called when for-each loop finishes
            Monitor.Exit(_mLock);
        }

        #endregion

        #region Implementation of IEnumerator

        // we just delegate actual implementation
        // to the inner enumerator, that actually iterates
        // over some collection


        /// <summary>
        /// Move to the next item.
        /// </summary>
        /// <returns>True if more items exist</returns>
        public bool MoveNext()
        {
            return _mInner.MoveNext();
        }


        /// <summary>
        /// Reset enumerator.
        /// </summary>
        public void Reset()
        {
            _mInner.Reset();
        }

        /// <summary>
        /// Current item.
        /// </summary>
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