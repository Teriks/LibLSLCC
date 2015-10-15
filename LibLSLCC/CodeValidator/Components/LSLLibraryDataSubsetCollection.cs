using System;
using System.Collections.Generic;
using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLLibraryDataSubsetCollection
    {
        private readonly HashSet<string> _subsets;

        public IReadOnlySet<string> Subsets
        {
            get { return _subsets.AsReadOnly(); }
        }

        public event Action<object, string> OnSubsetAdded;
        public event Action<object, string> OnSubsetRemoved;
        public event Action<object, string> OnSubsetsChanged;
        public event Action<object> OnSubsetsCleared;

        public LSLLibraryDataSubsetCollection(IEnumerable<string> subsets)
        {
            _subsets = new HashSet<string>(subsets);
        }

        public LSLLibraryDataSubsetCollection()
        {
            _subsets = new HashSet<string>();
        }

        public void Clear()
        {
            if (_subsets.Count <= 0) return;
            _subsets.Clear();
            OnOnSubsetsCleared(this);
        }

        public bool AddSubset(string subsetName)
        {
            if (_subsets.Contains(subsetName)) return false;

            _subsets.Add(subsetName);
            OnSubsetsChangedEvent(this, subsetName);
            OnSubsetAddedEvent(this, subsetName);
            return true;
        }

        public void AddSubsets(IEnumerable<string> subsets)
        {
            foreach (var subset in subsets)
            {
                AddSubset(subset);
            }
        }

        public bool RemoveSubset(string subsetName)
        {
            if (!_subsets.Contains(subsetName)) return false;

            _subsets.Remove(subsetName);
            OnSubsetsChangedEvent(this, subsetName);
            OnSubsetRemovedEvent(this, subsetName);

            return true;
        }

        protected virtual void OnSubsetAddedEvent(object arg1, string arg2)
        {
            var handler = OnSubsetAdded;
            if (handler != null) handler(arg1, arg2);
        }

        protected virtual void OnSubsetRemovedEvent(object arg1, string arg2)
        {
            var handler = OnSubsetRemoved;
            if (handler != null) handler(arg1, arg2);
        }

        protected virtual void OnSubsetsChangedEvent(object arg1, string arg2)
        {
            var handler = OnSubsetsChanged;
            if (handler != null) handler(arg1, arg2);
        }

        protected virtual void OnOnSubsetsCleared(object arg1)
        {
            var handler = OnSubsetsCleared;
            if (handler != null) handler(arg1);
        }

        public void SetSubsets(IEnumerable<string> subsets)
        {
            Clear();
            AddSubsets(subsets);
        }
    }
}