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

namespace LibLSLCC.Settings
{
    public abstract class SettingsBaseClass<TSetting> : ICloneable, INotifyPropertyChanged, INotifyPropertyChanging
        where TSetting : class
    {
        private readonly Dictionary<object, Action<SettingsPropertyChangedEventArgs<TSetting>>> _subscribedChanged =
            new Dictionary<object, Action<SettingsPropertyChangedEventArgs<TSetting>>>();

        private readonly Dictionary<object, Action<SettingsPropertyChangingEventArgs<TSetting>>> _subscribedChanging =
            new Dictionary<object, Action<SettingsPropertyChangingEventArgs<TSetting>>>();

        public event PropertyChangedEventHandler PropertyChanged;


        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanging(string propertyName)
        {
            var handler = PropertyChanging;
            if (handler != null) handler(this, new PropertyChangingEventArgs(propertyName));
            foreach (var subscriber in _subscribedChanging)
            {
                subscriber.Value(new SettingsPropertyChangingEventArgs<TSetting>(this as TSetting, subscriber.Key,
                    propertyName));
            }
        }

        private IEnumerable<object> GetAllNonNullSettingsBaseChildren()
        {
            var myType = GetType();

            var props = myType.GetProperties(BindingFlags.Instance | BindingFlags.Public);


            foreach (var prop in props)
            {
                var value = prop.GetValue(this, null);

                if (value == null) continue;

                var baseSearch = prop.PropertyType.BaseType;

                bool isSettingsBase = false;

                while (baseSearch != null)
                {
                    var genericTickIndex = baseSearch.Name.IndexOf("`", StringComparison.Ordinal);
                    if (genericTickIndex == -1) break;
                    var nonGenericName = baseSearch.Name.Substring(0, genericTickIndex);
                    if (nonGenericName != "SettingsBaseClass")
                    {
                        baseSearch = baseSearch.BaseType;
                        continue;
                    }

                    isSettingsBase = true;
                    break;
                }

                if (!isSettingsBase) continue;

                yield return value;
            }
        }


        public void SubscribePropertyChangedAll(object owner, Action<SettingsPropertyChangedEventArgs<object>> handler)
        {
            _subscribedChanged.Add(owner,
                args => handler(new SettingsPropertyChangedEventArgs<object>(this, owner, args.PropertyName)));

            foreach (var child in GetAllNonNullSettingsBaseChildren())
            {
                var subscribeMethod = child.GetType().GetMethod("SubscribePropertyChangedAll");

                subscribeMethod.Invoke(child, new[] {owner, handler});
            }
        }


        public void SubscribePropertyChanged(object owner, Action<SettingsPropertyChangedEventArgs<TSetting>> handler)
        {
            _subscribedChanged.Add(owner, handler);
        }


        public void UnSubscribePropertyChangedAll(object owner)
        {
            _subscribedChanged.Remove(owner);

            foreach (var child in GetAllNonNullSettingsBaseChildren())
            {
                var subscribeMethod = child.GetType().GetMethod("UnSubscribePropertyChangedAll");

                subscribeMethod.Invoke(child, new[] {owner});
            }
        }

        public void UnSubscribePropertyChanged(object owner)
        {
            _subscribedChanged.Remove(owner);
        }


        public void SubscribePropertyChangingAll(object owner, Action<SettingsPropertyChangedEventArgs<object>> handler)
        {
            _subscribedChanging.Add(owner,
                args => handler(new SettingsPropertyChangedEventArgs<object>(this, owner, args.PropertyName)));

            foreach (var child in GetAllNonNullSettingsBaseChildren())
            {
                var subscribeMethod = child.GetType().GetMethod("SubscribePropertyChangingAll");

                subscribeMethod.Invoke(child, new[] {owner, handler});
            }
        }


        public void SubscribePropertyChanging(object owner, Action<SettingsPropertyChangingEventArgs<TSetting>> handler)
        {
            _subscribedChanging.Add(owner, handler);
        }


        public void UnSubscribePropertyChangingAll(object owner)
        {
            _subscribedChanging.Remove(owner);

            foreach (var child in GetAllNonNullSettingsBaseChildren())
            {
                var subscribeMethod = child.GetType().GetMethod("UnSubscribePropertyChangingAll");

                subscribeMethod.Invoke(child, new[] {owner});
            }
        }


        public void UnSubscribePropertyChanging(object owner)
        {
            _subscribedChanging.Remove(owner);
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;


            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

            foreach (var subscriber in _subscribedChanged)
            {
                subscriber.Value(new SettingsPropertyChangedEventArgs<TSetting>(this as TSetting, subscriber.Key,
                    propertyName));
            }
        }


        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            OnPropertyChanging(propertyName);
            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        public override int GetHashCode()
        {
            var myType = GetType();

            var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var props = myType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            int hc = 0;


            foreach (var field in fields)
            {
                var val = field.GetValue(this) ?? 0;

                hc ^= val.GetHashCode();
                hc = (hc << 7) | (hc >> (32 - 7));
            }

            foreach (var prop in props)
            {
                var val = prop.GetValue(this, null) ?? 0;

                hc ^= val.GetHashCode();
                hc = (hc << 7) | (hc >> (32 - 7));
            }

            return hc;
        }


        public override bool Equals(object other)
        {
            if (other == null) return false;


            var myType = GetType();

            if (!myType.IsInstanceOfType(other))
            {
                return false;
            }

            var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var props = myType.GetProperties(BindingFlags.Instance | BindingFlags.Public);


            foreach (var field in fields)
            {
                var mval = field.GetValue(this);
                var tval = field.GetValue(other);
                if (!_Compare(mval, tval))
                {
                    return false;
                }
            }

            foreach (var prop in props)
            {
                var mval = prop.GetValue(this, null);
                var tval = prop.GetValue(other, null);
                if (!_Compare(mval, tval))
                {
                    return false;
                }
            }

            return true;
        }


        private bool _Compare(object mval, object tval)
        {
            if (mval == null && tval != null) return false;
            if (mval != null && tval == null) return false;

            if (mval == null) return true;

            if (!mval.GetType().IsInstanceOfType(tval)) return false;

            return mval.Equals(tval) && mval.GetHashCode() == tval.GetHashCode();
        }


        public TSetting Clone()
        {
            ICloneable i = this;
            return (TSetting) i.Clone();
        }


        object ICloneable.Clone()
        {
            var myType = GetType();

            var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var props = myType.GetProperties(BindingFlags.Instance | BindingFlags.Public);


            const BindingFlags constructorBindingFlags =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

            var constructors =
                myType.GetConstructors(constructorBindingFlags).Where(x => !x.GetParameters().Any()).ToList();
            if (!constructors.Any())
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0}.Init(object instance):  Type '{0}'  has no parameterless constructor.",
                        myType.FullName));
            }

            var constructor = constructors.First();

            object instance = constructor.Invoke(null);


            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType.GetInterfaces().Any(i => i == typeof (ICloneable)))
                {
                    var child = fieldInfo.GetValue(this);
                    if (child == null) continue;

                    var clone = ((ICloneable) child).Clone();

                    fieldInfo.SetValue(instance, clone);
                }
                else
                {
                    fieldInfo.SetValue(instance, fieldInfo.GetValue(this));
                }
            }

            foreach (var propInfo in props.Where(p => p.CanWrite && p.CanWrite))
            {
                if (propInfo.PropertyType.GetInterfaces().Any(i => i == typeof (ICloneable)))
                {
                    var child = propInfo.GetValue(this, null);
                    if (child == null) continue;

                    var clone = ((ICloneable) child).Clone();

                    propInfo.SetValue(instance, clone, null);
                }
                else
                {
                    propInfo.SetValue(instance, propInfo.GetValue(this, null), null);
                }
            }

            return instance;
        }
    }
}