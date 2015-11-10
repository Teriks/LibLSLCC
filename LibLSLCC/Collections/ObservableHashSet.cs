#region FileInfo
// 
// File: ObservableHashSet.cs
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LibLSLCC.Collections
{
    public class ObservableSet<T> : ObservableCollection<T>, ICloneable
    {

        private readonly HashSet<T>  _hashSet;


        public ObservableSet()
        {
            _hashSet = new HashSet<T>();
        }

        public ObservableSet(IEnumerable<T> collection )
        {
            _hashSet = new HashSet<T>();

            foreach (var item in collection.Where(x=>!_hashSet.Contains(x)))
            {
                this.Add(item);
            }
        }


        protected override void ClearItems()
        {
            _hashSet.Clear();
            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            _hashSet.Remove(this[index]);
            base.RemoveItem(index);
        }


        protected override void InsertItem(int index, T item)
        {
            if (_hashSet.Contains(item)) return;
            _hashSet.Add(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            if (_hashSet.Contains(item)) return;
            _hashSet.Add(item);
            base.SetItem(index, item);
        }

        public object Clone()
        {
            return new ObservableSet<T>(_hashSet);
        }
    }
}
