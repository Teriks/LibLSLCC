using System;
using System.Collections.Generic;
using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    /// Collection wrapper for library subset strings
    /// </summary>
    public class LSLLibraryDataSubsetCollection
    {
        private readonly HashSet<string> _subsets;

        /// <summary>
        /// Retrieve a read only set of the subsets in this collection
        /// </summary>
        public IReadOnlySet<string> Subsets
        {
            get { return _subsets.AsReadOnly(); }
        }

        /// <summary>
        /// Fired when a subset string that does not already exist is added.
        /// </summary>
        public event Action<object, string> OnSubsetAdded;

        /// <summary>
        /// Fired when an existing subset string is removed. This is not called
        /// when clear is called.
        /// </summary>
        public event Action<object, string> OnSubsetRemoved;

        /// <summary>
        /// Fired when the collection changes at all. Including cleared,
        /// but only when the collection was not already empty.
        /// 
        /// </summary>
        public event Action<object, string> OnSubsetsChanged;

        /// <summary>
        /// Fired when the subset collection is cleared, but only if the collection
        /// was not empty when it was cleared.
        /// </summary>
        public event Action<object> OnSubsetsCleared;

        /// <summary>
        /// Construct a subsets collection out of an exiting enumerable of strings
        /// </summary>
        /// <param name="subsets"></param>
        public LSLLibraryDataSubsetCollection(IEnumerable<string> subsets)
        {
            _subsets = new HashSet<string>(subsets);
        }

        /// <summary>
        /// Construct an empty subsets collection
        /// </summary>
        public LSLLibraryDataSubsetCollection()
        {
            _subsets = new HashSet<string>();
        }

        /// <summary>
        /// Clear all subset strings from the collection
        /// </summary>
        public void Clear()
        {
            if (_subsets.Count <= 0) return;
            _subsets.Clear();
            OnSubsetsClearedEvent(this);
        }


        /// <summary>
        /// Add a subset to the collection if it does not already exist
        /// </summary>
        /// <param name="subsetName">The subset string/name to add</param>
        /// <returns>True if a unique new subset was added,  False if it already existed.</returns>
        public bool AddSubset(string subsetName)
        {
            if (_subsets.Contains(subsetName)) return false;

            _subsets.Add(subsetName);
            OnSubsetsChangedEvent(this, subsetName);
            OnSubsetAddedEvent(this, subsetName);
            return true;
        }

        /// <summary>
        /// Add multiple subset strings at once to the collection.
        /// </summary>
        /// <param name="subsets">Enumerable of the subset strings to add</param>
        public void AddSubsets(IEnumerable<string> subsets)
        {
            foreach (var subset in subsets)
            {
                AddSubset(subset);
            }
        }

        /// <summary>
        /// Remove a subset string/name from the collection if it exist
        /// </summary>
        /// <param name="subsetName">The subset string/name to remove.</param>
        /// <returns>True if the subset existed and was removed, False if it did not exist.</returns>
        public bool RemoveSubset(string subsetName)
        {
            if (!_subsets.Contains(subsetName)) return false;

            _subsets.Remove(subsetName);
            OnSubsetsChangedEvent(this, subsetName);
            OnSubsetRemovedEvent(this, subsetName);

            return true;
        }


        /// <summary>
        /// Fired when a subset string that does not already exist is added.
        /// </summary>
        protected virtual void OnSubsetAddedEvent(object arg1, string arg2)
        {
            var handler = OnSubsetAdded;
            if (handler != null) handler(arg1, arg2);
        }


        /// <summary>
        /// Fired when an existing subset string is removed. This is not called
        /// when clear is called.
        /// </summary>
        protected virtual void OnSubsetRemovedEvent(object arg1, string arg2)
        {
            var handler = OnSubsetRemoved;
            if (handler != null) handler(arg1, arg2);
        }


        /// <summary>
        /// Fired when the collection changes at all. Including cleared,
        /// but only when the collection was not already empty.
        /// </summary>
        protected virtual void OnSubsetsChangedEvent(object arg1, string arg2)
        {
            var handler = OnSubsetsChanged;
            if (handler != null) handler(arg1, arg2);
        }


        /// <summary>
        /// Fired when the subset collection is cleared, but only if the collection
        /// was not empty when it was cleared.
        /// </summary>
        protected virtual void OnSubsetsClearedEvent(object arg1)
        {
            var handler = OnSubsetsCleared;
            if (handler != null) handler(arg1);
        }


        /// <summary>
        /// Clear the current subset strings/names and add all the subset strings
        /// from a given enumerable.
        /// </summary>
        /// <param name="subsets">The subset strings/names to add to the collection after clearing it.</param>
        public void SetSubsets(IEnumerable<string> subsets)
        {
            Clear();
            AddSubsets(subsets);
        }
    }
}