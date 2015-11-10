#region FileInfo
// 
// File: SettingsBaseClass.cs
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
