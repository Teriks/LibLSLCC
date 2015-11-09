using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LibLSLCC.Utility
{
    public abstract class SettingsBaseClass : ICloneable
    {
        private readonly Dictionary<object, Action<object, object, PropertyChangedEventArgs>> _subscribed = new Dictionary<object, Action<object, object, PropertyChangedEventArgs>>();

        public event PropertyChangedEventHandler PropertyChanged;


        public void SubscribePropertyChanged(object owner, Action<object, object, PropertyChangedEventArgs> handler)
        {
            _subscribed.Add(owner, handler);
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
            foreach (var handler in _subscribed)
            {
                handler.Value(this, handler.Key, new PropertyChangedEventArgs(propertyName));
            }
            return true;
        }



        public object Clone()
        {
            var myType = this.GetType();

            var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var props = myType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var instance = Activator.CreateInstance(myType);

            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType.GetInterfaces().Any(t=>t==typeof (ICloneable)))
                {
                    var child =  fieldInfo.GetValue(this);
                    if (child == null) continue;

                    var clone = ((ICloneable)child).Clone();

                    fieldInfo.SetValue(instance, clone);
                }
                else
                {
                    fieldInfo.SetValue(instance, fieldInfo.GetValue(this));
                }
            }

            foreach (var propInfo in props)
            {
                if (propInfo.PropertyType.GetInterfaces().Any(t => t == typeof(ICloneable)))
                {
                    var child = propInfo.GetValue(this,null);
                    if(child == null) continue;

                    var clone = ((ICloneable) child).Clone();

                    propInfo.SetValue(instance, clone, null);
                }
                else
                {
                    propInfo.SetValue(instance, propInfo.GetValue(this,null),null);
                }
            }

            return instance;
        }
    }
}
