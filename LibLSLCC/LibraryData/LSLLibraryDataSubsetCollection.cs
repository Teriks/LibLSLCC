#region FileInfo
// 
// File: LSLLibraryDataSubsetCollection.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LibLSLCC.Collections;

namespace LibLSLCC.LibraryData
{

    /// <summary>
    /// EventArgs for <see cref="LSLLibraryDataSubsetCollection.OnSubsetAdded"/>
    /// </summary>
    public class LibraryDataSubsetAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs the event arguments using the name of the subset added.
        /// </summary>
        /// <param name="subsetName">The name of the subset added.</param>
        public LibraryDataSubsetAddedEventArgs(string subsetName)
        {
            SubsetName = subsetName;
        }

        /// <summary>
        /// The subset name added.
        /// </summary>
        public string SubsetName { get; private set; }
    }

    /// <summary>
    /// EventArgs for <see cref="LSLLibraryDataSubsetCollection.OnSubsetRemoved"/>
    /// </summary>
    public class LibraryDataSubsetRemovedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs the event arguments using the name of the subset removed.
        /// </summary>
        /// <param name="subsetName">The name of the subset removed.</param>
        public LibraryDataSubsetRemovedEventArgs(string subsetName)
        {
            SubsetName = subsetName;
        }

        /// <summary>
        /// The subset name removed.
        /// </summary>
        public string SubsetName { get; private set; }
    }

    /// <summary>
    /// EventArgs for <see cref="LSLLibraryDataSubsetCollection.OnSubsetsChanged"/>
    /// </summary>
    public class LibraryDataSubsetsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="LSLLibraryDataSubsetCollection"/> that was changed.
        /// </summary>
        public LSLLibraryDataSubsetCollection Collection { get; private set; }


        /// <summary>
        /// Constructs the event arguments using a reference to the collection that was changed.
        /// </summary>
        /// <param name="collection">The collection that was changed.</param>
        public LibraryDataSubsetsChangedEventArgs(LSLLibraryDataSubsetCollection collection)
        {
            Collection = collection;
        }
    }

    /// <summary>
    /// EventArgs for <see cref="LSLLibraryDataSubsetCollection.OnSubsetsCleared"/>
    /// </summary>
    public class LibraryDataSubsetsClearedEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="LSLLibraryDataSubsetCollection"/> that was cleared.
        /// </summary>
        public LSLLibraryDataSubsetCollection Collection { get; private set; }

        /// <summary>
        /// Constructs the event arguments using a reference to the collection that was cleared.
        /// </summary>
        /// <param name="collection">The collection that was cleared.</param>
        public LibraryDataSubsetsClearedEventArgs(LSLLibraryDataSubsetCollection collection)
        {
            Collection = collection;
        }
    }


    /// <summary>
    /// Collection wrapper for library subset strings
    /// </summary>
    public class LSLLibraryDataSubsetCollection : ISet<string>, IReadOnlyHashedSet<string>
    {
        private readonly HashSet<string> _subsets;


        /// <summary>
        /// Fired when a subset string that does not already exist is added.
        /// </summary>
        public event EventHandler<LibraryDataSubsetAddedEventArgs> OnSubsetAdded;

        /// <summary>
        /// Fired when an existing subset string is removed. This is not called
        /// when clear is called.
        /// </summary>
        public event EventHandler<LibraryDataSubsetRemovedEventArgs> OnSubsetRemoved;

        /// <summary>
        /// Fired when the collection changes at all. Including cleared,
        /// but only when the collection was not already empty.
        /// 
        /// </summary>
        public event EventHandler<LibraryDataSubsetsChangedEventArgs> OnSubsetsChanged;

        /// <summary>
        /// Fired when the subset collection is cleared, but only if the collection
        /// was not empty when it was cleared.
        /// </summary>
        public event EventHandler<LibraryDataSubsetsClearedEventArgs> OnSubsetsCleared;


        /// <summary>
        /// Construct a subsets collection out of an existing enumerable of strings
        /// </summary>
        /// <param name="subsets">An enumerable of subset names to initialize the subset collection from.</param>
        /// <exception cref="LSLInvalidSubsetNameException">If any of the give subset names contain invalid characters.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="subsets"/> is <c>null</c>.</exception>
        public LSLLibraryDataSubsetCollection(IEnumerable<string> subsets)
        {
            if(subsets == null) throw new ArgumentNullException("subsets");

            _subsets = new HashSet<string>(LSLLibraryDataSubsetNameParser.ThrowIfInvalid(subsets));
        }

        /// <summary>
        /// Construct an empty subsets collection
        /// </summary>
        public LSLLibraryDataSubsetCollection()
        {
            _subsets = new HashSet<string>();
        }


        void ICollection<string>.Add(string subsetName)
        {
            LSLLibraryDataSubsetNameParser.ThrowIfInvalid(subsetName);


            if (_subsets.Contains(subsetName)) return;

            _subsets.Add(subsetName);
            OnSubsetsChangedEvent();
            OnSubsetAddedEvent(subsetName);
        }

        /// <summary>
        /// Modifies the current set so that it contains all elements that are present in both the current set and in the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <exception cref="LSLInvalidSubsetNameException">
        /// Thrown if the UnionWith operation causes a subset name that does not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*) to be added to the subset collection.
        /// </exception>
        public void UnionWith(IEnumerable<string> other)
        {
            //need to check here we might be adding stuff
            foreach (var subset in other.Where(subset => !_subsets.Contains(subset)))
            {
                //will syntax check the names as they are added and trigger events
                Add(subset);
            }
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public void IntersectWith(IEnumerable<string> other)
        {
            var o = new HashSet<string>(other);
            //need to check here we might be removing stuff
            foreach (var subset in _subsets.Where(subset => !o.Contains(subset)))
            {
                //will trigger remove and changed events
                Remove(subset);
            }
        }

        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public void ExceptWith(IEnumerable<string> other)
        {
            if(other == null) throw new ArgumentNullException("other");

            //need to check here we might be removing stuff
            //have to call events if anything gets removed
            foreach (var subset in other)
            {
                //will trigger remove and changed events
                Remove(subset);
            }
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the current set or in the specified collection, but not both. 
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public void SymmetricExceptWith(IEnumerable<string> other)
        {
            //need to check here we might be removing stuff
            //have to call events if anything gets removed
            foreach (var item in new HashSet<string>(other).Where(x => _subsets.Contains(x)))
            {
                //will trigger remove and changed events
                Remove(item);
            }
        }

        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <returns>
        /// true if the current set is a subset of <paramref name="other"/>; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsSubsetOf(IEnumerable<string> other)
        {
            return _subsets.IsSubsetOf(other);
        }

        /// <summary>
        /// Determines whether the current set is a superset of a specified collection.
        /// </summary>
        /// <returns>
        /// true if the current set is a superset of <paramref name="other"/>; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsSupersetOf(IEnumerable<string> other)
        {
            return _subsets.IsSupersetOf(other);
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <returns>
        /// true if the current set is a proper superset of <paramref name="other"/>; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set. </param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsProperSupersetOf(IEnumerable<string> other)
        {
            return _subsets.IsProperSupersetOf(other);
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <returns>
        /// true if the current set is a proper subset of <paramref name="other"/>; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsProperSubsetOf(IEnumerable<string> other)
        {
            return _subsets.IsProperSubsetOf(other);
        }

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <returns>
        /// true if the current set and <paramref name="other"/> share at least one common element; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool Overlaps(IEnumerable<string> other)
        {
            return _subsets.Overlaps(other);
        }

        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <returns>
        /// true if the current set is equal to <paramref name="other"/>; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param><exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool SetEquals(IEnumerable<string> other)
        {
            return _subsets.SetEquals(other);
        }

        /// <summary>
        /// Adds a subset name to the current subset collection and returns a value to indicate if the subset name was successfully added. 
        /// </summary>
        /// <returns>
        /// true if the subset name is added to the set; false if the subset name is already in the set.
        /// </returns>
        /// <param name="subsetName">The subset to add to the set.</param>
        /// <exception cref="LSLInvalidSubsetNameException">If any of the give subset names don't match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*).</exception>
        public bool Add(string subsetName)
        {
            LSLLibraryDataSubsetNameParser.ThrowIfInvalid(subsetName);

            if (_subsets.Add(subsetName) == false) return false;

            OnSubsetsChangedEvent();
            OnSubsetAddedEvent(subsetName);

            return true;
        }

        /// <summary>
        /// Clear all subset strings from the collection
        /// </summary>
        public void Clear()
        {
            if (_subsets.Count <= 0) return;
            _subsets.Clear();
            OnSubsetsClearedEvent();
        }


        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="LSLInvalidSubsetNameException">If the given subset name was invalid.</exception>
        public bool Contains(string item)
        {
            LSLLibraryDataSubsetNameParser.ThrowIfInvalid(item);

            return _subsets.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(string[] array, int arrayIndex)
        {
            _subsets.CopyTo(array, arrayIndex);
        }


        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return _subsets.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }


        /// <summary>
        /// Add multiple subset strings at once to the collection.
        /// </summary>
        /// <param name="subsets">Enumerable of the subset strings to add</param>
        /// <exception cref="LSLInvalidSubsetNameException">Thrown if any of the given subset names do not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*).</exception>
        public void AddSubsets(IEnumerable<string> subsets)
        {
            foreach (var subset in LSLLibraryDataSubsetNameParser.ThrowIfInvalid(subsets))
            {
                Add(subset);
            }
        }


        /// <summary>
        /// Add multiple subsets at once to the collection by parsing them from CSV.
        /// </summary>
        /// <param name="subsets">CSV string of the subset names to add.</param>
        /// <exception cref="LSLInvalidSubsetNameException">Thrown if any of the given subset names in the CSV string do not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*).</exception>
        public void AddSubsets(string subsets)
        {
            AddSubsets(LSLLibraryDataSubsetNameParser.ParseSubsets(subsets));
        }


        /// <summary>
        /// Remove a subset string/name from the collection if it exist, the name you give to this function is not checked for validity.
        /// </summary>
        /// <param name="subsetName">The subset string/name to remove.</param>
        /// <returns>True if the subset existed and was removed, False if it did not exist.</returns>
        public bool Remove(string subsetName)
        {
            if (_subsets.Remove(subsetName) == false) return false;

            OnSubsetsChangedEvent();
            OnSubsetRemovedEvent(subsetName);

            return true;
        }


        /// <summary>
        /// Fired when a subset string that does not already exist is added.
        /// </summary>
        protected virtual void OnSubsetAddedEvent(string arg2)
        {
            var handler = OnSubsetAdded;
            if (handler != null) handler(this, new LibraryDataSubsetAddedEventArgs(arg2));
        }


        /// <summary>
        /// Fired when an existing subset string is removed. This is not called
        /// when clear is called.
        /// </summary>
        protected virtual void OnSubsetRemovedEvent(string arg2)
        {
            var handler = OnSubsetRemoved;
            if (handler != null) handler(this, new LibraryDataSubsetRemovedEventArgs(arg2));
        }


        /// <summary>
        /// Fired when the collection changes at all. Including cleared,
        /// but only when the collection was not already empty.
        /// </summary>
        protected virtual void OnSubsetsChangedEvent()
        {
            var handler = OnSubsetsChanged;
            if (handler != null) handler(this, new LibraryDataSubsetsChangedEventArgs(this));
        }


        /// <summary>
        /// Fired when the subset collection is cleared, but only if the collection
        /// was not empty when it was cleared.
        /// </summary>
        protected virtual void OnSubsetsClearedEvent()
        {
            var handler = OnSubsetsCleared;
            if (handler != null) handler(this, new LibraryDataSubsetsClearedEventArgs(this));
        }


        /// <summary>
        /// Clear the current subset strings/names and add all the subset strings from a given enumerable.
        /// </summary>
        /// <param name="subsets">The subset strings/names to add to the collection after clearing it.</param>
        /// <exception cref="LSLInvalidSubsetNameException">Thrown if any of the given subset names do not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*).</exception>
        public void SetSubsets(IEnumerable<string> subsets)
        {
            Clear();
            AddSubsets(subsets);
        }


        /// <summary>
        /// Clear the current subset strings/names and add all the subset strings parsed from CSV.
        /// </summary>
        /// <param name="subsets">The subset strings/names to add to the collection after clearing it.</param>
        /// <exception cref="LSLInvalidSubsetNameException">Thrown if any of the given subset names in the CSV string do not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*).</exception>
        public void SetSubsets(string subsets)
        {
            Clear();
            AddSubsets(LSLLibraryDataSubsetNameParser.ParseSubsets(subsets));
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<string> GetEnumerator()
        {
            return _subsets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _subsets.GetEnumerator();
        }
    }
}