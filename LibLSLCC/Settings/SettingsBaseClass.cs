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
using LibLSLCC.Collections;

namespace LibLSLCC.Settings
{
    public abstract class SettingsBaseClass<TSetting> : ICloneable, INotifyPropertyChanged, INotifyPropertyChanging
        where TSetting : class
    {
        private class DeepClonableValueHashMap<TKey, TValue> : HashMap<TKey, TValue>
        {
            public override object Clone()
            {
                var map = new DeepClonableValueHashMap<TKey, TValue>();

                foreach (var kvp in this)
                {
                    var clonableValue = (ICloneable) kvp.Value;
                    map.Add(kvp.Key, (TValue) clonableValue.Clone());
                }

                return map;
            }
        }


        private readonly
            DeepClonableValueHashMap<object, HashMap<string, Action<SettingsPropertyChangedEventArgs<TSetting>>>>
            _subscribedChanged =
                new DeepClonableValueHashMap
                    <object, HashMap<string, Action<SettingsPropertyChangedEventArgs<TSetting>>>>();

        private readonly
            DeepClonableValueHashMap<object, HashMap<string, Action<SettingsPropertyChangingEventArgs<TSetting>>>>
            _subscribedChanging =
                new DeepClonableValueHashMap
                    <object, HashMap<string, Action<SettingsPropertyChangingEventArgs<TSetting>>>>();


        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;


        protected virtual void OnPropertyChanging(string propertyName, object oldValue, object newValue)
        {
            var handler = PropertyChanging;
            if (handler != null) handler(this, new PropertyChangingEventArgs(propertyName));
            foreach (var subscriber in _subscribedChanging)
            {
                Action<SettingsPropertyChangingEventArgs<TSetting>> action;

                if (subscriber.Value.TryGetValue("", out action))
                {
                    action(new SettingsPropertyChangingEventArgs<TSetting>(this as TSetting, subscriber.Key,
                        propertyName, oldValue, newValue));
                }

                if (subscriber.Value.TryGetValue(propertyName, out action))
                {
                    action(new SettingsPropertyChangingEventArgs<TSetting>(this as TSetting, subscriber.Key,
                        propertyName, oldValue, newValue));
                }
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

                if (!SettingsBaseClassTools.HasSettingsBase(prop.PropertyType)) continue;

                yield return value;
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public void SubscribePropertyChangedRecursive(object owner, Action<SettingsPropertyChangedEventArgs<object>> handler)
        {
            if (!_subscribedChanged.ContainsKey(owner))
            {
                _subscribedChanged.Add(owner, new HashMap<string, Action<SettingsPropertyChangedEventArgs<TSetting>>>()
                {
                    {
                        "",
                        args =>
                            handler(new SettingsPropertyChangedEventArgs<object>(this, owner, args.PropertyName,
                                args.OldValue,
                                args.NewValue))
                    }
                });
            }
            else
            {
                _subscribedChanged[owner].Add("", args =>
                    handler(new SettingsPropertyChangedEventArgs<object>(this, owner, args.PropertyName,
                        args.OldValue,
                        args.NewValue)));
            }

            foreach (var child in GetAllNonNullSettingsBaseChildren())
            {
                var subscribeMethod = child.GetType().GetMethod("SubscribePropertyChangedRecursive");

                subscribeMethod.Invoke(child, new[] {owner, handler});
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public void SubscribePropertyChanged(object owner, Action<SettingsPropertyChangedEventArgs<TSetting>> handler)
        {
            if (!_subscribedChanged.ContainsKey(owner))
            {
                _subscribedChanged.Add(owner, new HashMap<string, Action<SettingsPropertyChangedEventArgs<TSetting>>>
                {
                    {
                        "",
                        handler
                    }
                });
            }
            else
            {
                _subscribedChanged[owner].Add("", handler);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public void SubscribePropertyChanged(object owner, string propertyName, Action<SettingsPropertyChangedEventArgs<TSetting>> handler)
        {
            if (!_subscribedChanged.ContainsKey(owner))
            {
                _subscribedChanged.Add(owner, new HashMap<string, Action<SettingsPropertyChangedEventArgs<TSetting>>>
                {
                    {
                        propertyName,
                        handler
                    }
                });
            }
            else
            {
                _subscribedChanged[owner].Add(propertyName, handler);
            }
        }


        public void UnSubscribePropertyChangedRecursive(object owner)
        {
            _subscribedChanged.Remove(owner);

            foreach (var child in GetAllNonNullSettingsBaseChildren())
            {
                var subscribeMethod = child.GetType().GetMethod("UnSubscribePropertyChangedRecursive");

                subscribeMethod.Invoke(child, new[] {owner});
            }
        }

        public void UnSubscribePropertyChanged(object owner)
        {
            _subscribedChanged.Remove(owner);
        }

        public void UnSubscribePropertyChanged(object owner, string propertyName)
        {
            var dictRef = _subscribedChanged[owner];
            dictRef.Remove(propertyName);
            if (dictRef.Count == 0)
            {
                _subscribedChanged.Remove(owner);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public void SubscribePropertyChangingRecursive(object owner, Action<SettingsPropertyChangingEventArgs<object>> handler)
        {
            if (!_subscribedChanging.ContainsKey(owner))
            {
                _subscribedChanging.Add(owner,
                    new HashMap<string, Action<SettingsPropertyChangingEventArgs<TSetting>>>()
                    {
                        {
                            "",
                            args =>
                                handler(new SettingsPropertyChangingEventArgs<object>(this, owner, args.PropertyName,
                                    args.OldValue,
                                    args.NewValue))
                        }
                    });
            }
            else
            {
                _subscribedChanging[owner].Add("", args =>
                    handler(new SettingsPropertyChangingEventArgs<object>(this, owner, args.PropertyName,
                        args.OldValue,
                        args.NewValue)));
            }

            foreach (var child in GetAllNonNullSettingsBaseChildren())
            {
                var subscribeMethod = child.GetType().GetMethod("SubscribePropertyChangingRecursive");

                subscribeMethod.Invoke(child, new[] {owner, handler});
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public void SubscribePropertyChanging(object owner, Action<SettingsPropertyChangingEventArgs<TSetting>> handler)
        {
            if (!_subscribedChanging.ContainsKey(owner))
            {
                _subscribedChanging.Add(owner, new HashMap<string, Action<SettingsPropertyChangingEventArgs<TSetting>>>
                {
                    {
                        "",
                        handler
                    }
                });
            }
            else
            {
                _subscribedChanging[owner].Add("", handler);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public void SubscribePropertyChanging(object owner, string propertyName, Action<SettingsPropertyChangingEventArgs<TSetting>> handler)
        {
            if (!_subscribedChanging.ContainsKey(owner))
            {
                _subscribedChanging.Add(owner, new HashMap<string, Action<SettingsPropertyChangingEventArgs<TSetting>>>
                {
                    {
                        propertyName,
                        handler
                    }
                });
            }
            else
            {
                _subscribedChanging[owner].Add(propertyName, handler);
            }
        }


        public void UnSubscribePropertyChangingRecursive(object owner)
        {
            _subscribedChanging.Remove(owner);

            foreach (var child in GetAllNonNullSettingsBaseChildren())
            {
                var subscribeMethod = child.GetType().GetMethod("UnSubscribePropertyChangingRecursive");

                subscribeMethod.Invoke(child, new[] {owner});
            }
        }


        public void UnSubscribePropertyChanging(object owner)
        {
            _subscribedChanging.Remove(owner);
        }

        public void UnSubscribePropertyChanging(object owner, string propertyName)
        {
            var dictRef = _subscribedChanging[owner];
            dictRef.Remove(propertyName);
            if (dictRef.Count == 0)
            {
                _subscribedChanging.Remove(owner);
            }
        }


        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            PropertyChangedEventHandler handler = PropertyChanged;


            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

            foreach (var subscriber in _subscribedChanged)
            {
                Action<SettingsPropertyChangedEventArgs<TSetting>> action;

                if (subscriber.Value.TryGetValue("", out action))
                {
                    action(new SettingsPropertyChangedEventArgs<TSetting>(this as TSetting, subscriber.Key,
                        propertyName, oldValue, newValue));
                }

                if (subscriber.Value.TryGetValue(propertyName, out action))
                {
                    action(new SettingsPropertyChangedEventArgs<TSetting>(this as TSetting, subscriber.Key,
                        propertyName, oldValue, newValue));
                }
            }
        }


        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            var curValue = field;

            Type baseType;
            if (field != null && SettingsBaseClassTools.HasSettingsBase(field.GetType(), out baseType))
            {
                TransferObservers(baseType, field, value);
            }

            OnPropertyChanging(propertyName, curValue, value);
            field = value;
            OnPropertyChanged(propertyName, curValue, value);

            return true;
        }


        private static void TransferObservers<T>(Type baseType, T oldVal, T newVal)
        {
            const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                           | BindingFlags.Static;


            var subscribedChanging = baseType.GetField("_subscribedChanging", bindFlags);
            var subscribedChanged = baseType.GetField("_subscribedChanged", bindFlags);


            var changing = subscribedChanging.GetValue(oldVal) as ICloneable;
            var changed = subscribedChanged.GetValue(oldVal) as ICloneable;


            subscribedChanging.SetValue(newVal, changing.Clone());
            subscribedChanged.SetValue(newVal, changed.Clone());
        }


        public override int GetHashCode()
        {
            var myType = GetType();

            var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var props = myType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            int hash = 0;


            foreach (var field in fields)
            {
                var val = field.GetValue(this) ?? 0;

                hash ^= val.GetHashCode();
                hash = (hash << 7) | (hash >> (32 - 7));
            }

            foreach (var prop in props)
            {
                var val = prop.GetValue(this, null) ?? 0;

                hash ^= val.GetHashCode();
                hash = (hash << 7) | (hash >> (32 - 7));
            }

            return hash;
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


        private static bool _Compare(object mval, object tval)
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


        public void MemberwiseAssign(TSetting other)
        {
            var myType = GetType();

            var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var props = myType.GetProperties(BindingFlags.Instance | BindingFlags.Public);


            foreach (var field in fields)
            {
                field.SetValue(this,field.GetValue(other));
            }

            foreach (var prop in props.Where(x=>x.CanRead && x.CanWrite))
            {
                prop.SetValue(this, prop.GetValue(other,null),null);
            }
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

            ICloner cloner = new DefaultCloner();

            foreach (var fieldInfo in fields)
            {
                var clonerAttribute = fieldInfo.GetCustomAttributes(typeof(MemberClonerAttribute), true);

                var child = fieldInfo.GetValue(this);
                if (child == null) continue;

                if (clonerAttribute.Any())
                {
                    var clonerAttr = (MemberClonerAttribute)clonerAttribute.First();
                    cloner = Activator.CreateInstance(clonerAttr.ClonerType) as ICloner;

                    if (cloner == null)
                    {
                        throw new InvalidOperationException(string.Format("Field '{0}' has a [MemberClonerAttribute] with a cloner type that does not implement ICloner.", 
                            fieldInfo.Name));
                    }

                    fieldInfo.SetValue(instance, cloner.Clone(child));
                }
                else
                {
                    
                    var clone = cloner.Clone(child);
                    fieldInfo.SetValue(instance, clone ?? fieldInfo.GetValue(this));
                }
            }

            foreach (var propInfo in props.Where(p => p.CanWrite && p.CanRead))
            {
                var clonerAttribute = propInfo.GetCustomAttributes(typeof(MemberClonerAttribute), true);
               
                var child = propInfo.GetValue(this, null);
                if (child == null) continue;

                if (clonerAttribute.Any())
                {
                    var clonerAttr = (MemberClonerAttribute)clonerAttribute.First();
                    cloner = Activator.CreateInstance(clonerAttr.ClonerType) as ICloner;

                    if (cloner == null)
                    {
                        throw new InvalidOperationException(string.Format("Property '{0}' has a [MemberClonerAttribute] with a cloner type that does not implement ICloner.",
                            propInfo.Name));
                    }

                    propInfo.SetValue(instance, cloner.Clone(child), null);
                }
                else
                {
                    var clone = cloner.Clone(child);
                    propInfo.SetValue(instance, clone ?? propInfo.GetValue(this, null), null);
                }
            }

            return instance;
        }
    }
}