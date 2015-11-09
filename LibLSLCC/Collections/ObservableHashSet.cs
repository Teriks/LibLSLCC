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
