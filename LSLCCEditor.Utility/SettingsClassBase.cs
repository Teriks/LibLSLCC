using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LSLCCEditor.Utility
{
    public class SettingsClassBase
    {

        private Dictionary<object, Action<object, object, PropertyChangedEventArgs>> _subscribed = new Dictionary<object, Action<object, object, PropertyChangedEventArgs>>();

        public event PropertyChangedEventHandler PropertyChanged;



        public void SubscribePropertyChanged(object owner, Action<object,object, PropertyChangedEventArgs> handler)
        {
            _subscribed.Add(owner,handler);
        }

        public void UnSubscribePropertyChanged(object owner)
        {
            _subscribed.Remove(owner);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            foreach (var handler in _subscribed){
                handler.Value(this, handler.Key, new PropertyChangedEventArgs(propertyName));
            }
            return true;
        }
    }
}
