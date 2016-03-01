#region FileInfo

// 
// File: ObservableSettingsHashSet.cs
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

using System.Collections.Generic;
using System.Linq;
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.Settings
{
    /// <summary>
    ///     An observable hash set collection that is usable as a member in classes deriving from
    ///     <see cref="SettingsBaseClass{T}" />
    /// </summary>
    /// <typeparam name="T">The type of elements this <see cref="ObservableSettingsHashSet{T}" /> contains.</typeparam>
    public class ObservableSettingsHashSet<T> : ObservableHashSet<T>
    {
        /// <summary>
        ///     Construct an empty <see cref="ObservableSettingsHashSet{T}" />
        /// </summary>
        public ObservableSettingsHashSet()
        {
        }


        /// <summary>
        ///     Construct an <see cref="ObservableSettingsHashSet{T}" /> containing the elements from
        ///     <paramref name="collection" />.
        /// </summary>
        /// <param name="collection">
        ///     The <see cref="IEnumerable{T}" /> to fill the <see cref="ObservableSettingsHashSet{T}" />
        ///     with.
        /// </param>
        public ObservableSettingsHashSet(IEnumerable<T> collection) : base(collection)
        {
        }


        /// <summary>
        ///     Calculates the hash code for this <see cref="ObservableSettingsHashSet{T}" /> by considering the hash code of every
        ///     element.
        /// </summary>
        /// <returns>The generated hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return this.Aggregate(19, (current, item) => current*31 + item.GetHashCode());
            }
        }


        /// <summary>
        ///     Determines if every element in this <see cref="ObservableSettingsHashSet{T}" /> is equal to the elements in
        ///     another, using SequenceEqual.
        ///     If <paramref name="obj" /> is not an <see cref="ObservableSettingsHashSet{T}" /> object, then this function will
        ///     return <c>false</c>.
        /// </summary>
        /// <param name="obj">The object to test for equality with.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="obj" /> is an <see cref="ObservableSettingsHashSet{T}" /> where
        ///     this.SequenceEqual(obj) returns <c>true</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as ObservableSettingsHashSet<T>;
            if (other == null) return false;

            if (Count != other.Count) return false;

            return this.SequenceEqual(other);
        }


        /// <summary>
        ///     Creates a shallow copy of this <see cref="ObservableSettingsHashSet{T}"/>..
        /// </summary>
        /// <returns>A shallow copy of this <see cref="ObservableSettingsHashSet{T}"/>.</returns>
        public override object Clone()
        {
            return new ObservableSettingsHashSet<T>(this);
        }
    }
}