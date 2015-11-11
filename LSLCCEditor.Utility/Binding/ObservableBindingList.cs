using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LSLCCEditor.Utility.Binding
{
    public class ObservableBindingList<T> : BindingList<T>
    {

        public ObservableBindingList(IList<T> list) : base(list)
        {
            
        }

        public ObservableBindingList()
        {

        }

        protected override void RemoveItem(int index)
        {

            var item = this[index];

            base.RemoveItem(index);

            if (ItemRemoved != null)
            {
                ItemRemoved(item);
            }
        }

        public event Action<T> ItemRemoved;
    }
}
