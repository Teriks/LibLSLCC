using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LibLSLCC.Collections
{
    public class ObservableSet<T> : ObservableCollection<T>
    {

        private readonly HashSet<T>  _hashSet = new HashSet<T>(); 

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
    }
}
