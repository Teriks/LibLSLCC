#region FileInfo
// 
// 
// File: ThreadSafeEnumerator.cs
// 
// Last Compile: 25/09/2015 @ 11:47 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
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