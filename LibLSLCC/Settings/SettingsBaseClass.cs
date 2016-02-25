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

#region Imports

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.Settings
{
    /// <summary>
    ///     A base class for settings objects.
    /// </summary>
    /// <typeparam name="TSetting"></typeparam>
    public abstract class SettingsBaseClass<TSetting> : ICloneable, INotifyPropertyChanged, INotifyPropertyChanging
        where TSetting : class
    {
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


        /// <summary>
        ///     Deep clones this settings object.
        /// </summary>
        /// <returns>A deep clone of this settings object.</returns>
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
                var clonerAttribute = fieldInfo.GetCustomAttributes(typeof (MemberClonerAttribute), true);

                var child = fieldInfo.GetValue(this);
                if (child == null) continue;

                if (clonerAttribute.Any())
                {
                    var clonerAttr = (MemberClonerAttribute) clonerAttribute.First();
                    cloner = Activator.CreateInstance(clonerAttr.ClonerType) as ICloner;

                    if (cloner == null)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "Field '{0}' has a [MemberClonerAttribute] with a cloner type that does not implement ICloner.",
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
                var clonerAttribute = propInfo.GetCustomAttributes(typeof (MemberClonerAttribute), true);

                var child = propInfo.GetValue(this, null);
                if (child == null) continue;

                if (clonerAttribute.Any())
                {
                    var clonerAttr = (MemberClonerAttribute) clonerAttribute.First();
                    cloner = Activator.CreateInstance(clonerAttr.ClonerType) as ICloner;

                    if (cloner == null)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "Property '{0}' has a [MemberClonerAttribute] with a cloner type that does not implement ICloner.",
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


        /// <summary>
        ///     The property changed event, fired when a settings property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     The property changing event, fired when a settings property is about to change, but has not changed yet.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;


        /// <summary>
        ///     Occurs when a property is about to change but has not changed yet.
        /// </summary>
        /// <param name="propertyName">The name of the property that is fixing to change.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
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


        /// <summary>
        ///     Subscribe to the property changed event on this settings object and all child settings objects recursively.
        /// </summary>
        /// <param name="owner">The object that is subscribing to the changed event.</param>
        /// <param name="handler">The changed event handler.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public void SubscribePropertyChangedRecursive(object owner,
            Action<SettingsPropertyChangedEventArgs<object>> handler)
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


        /// <summary>
        ///     Subscribe to the property changed event on this settings object.
        /// </summary>
        /// <param name="owner">The object that is subscribing to the changed event.</param>
        /// <param name="handler">The changed event handler.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
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


        /// <summary>
        ///     Subscribe to the property changed event on this settings object for a specific property.
        /// </summary>
        /// <param name="owner">The object that is subscribing to the changed event.</param>
        /// <param name="propertyName">The name of the property to subscribe to.</param>
        /// <param name="handler">The changed event handler.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public void SubscribePropertyChanged(object owner, string propertyName,
            Action<SettingsPropertyChangedEventArgs<TSetting>> handler)
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


        /// <summary>
        ///     Remove all property changed subscriptions belonging to an object from this settings object and all child settings
        ///     objects recursively.
        /// </summary>
        /// <param name="owner">The subscriber to remove subscriptions for.</param>
        public void UnSubscribePropertyChangedRecursive(object owner)
        {
            _subscribedChanged.Remove(owner);

            foreach (var child in GetAllNonNullSettingsBaseChildren())
            {
                var unSubscribeMethod = child.GetType().GetMethod("UnSubscribePropertyChangedRecursive");

                unSubscribeMethod.Invoke(child, new[] {owner});
            }
        }


        /// <summary>
        ///     Remove a property changed subscription from this settings object given the subscriber object.
        /// </summary>
        /// <param name="owner">The subscriber to remove the subscription for.</param>
        public void UnSubscribePropertyChanged(object owner)
        {
            _subscribedChanged.Remove(owner);
        }


        /// <summary>
        ///     Remove a specific property changed subscription from this settings object given the subscriber object and the
        ///     property name.
        /// </summary>
        /// <param name="owner">The subscriber to remove the subscription for.</param>
        /// <param name="propertyName">The property name to remove the subscription for.</param>
        public void UnSubscribePropertyChanged(object owner, string propertyName)
        {
            var dictRef = _subscribedChanged[owner];
            dictRef.Remove(propertyName);
            if (dictRef.Count == 0)
            {
                _subscribedChanged.Remove(owner);
            }
        }


        /// <summary>
        ///     Subscribe to the property changing event on this settings object and all child settings objects recursively.
        /// </summary>
        /// <param name="owner">The object that is subscribing to the changing event.</param>
        /// <param name="handler">The changing event handler.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public void SubscribePropertyChangingRecursive(object owner,
            Action<SettingsPropertyChangingEventArgs<object>> handler)
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


        /// <summary>
        ///     Subscribe to the property changing event on this settings object.
        /// </summary>
        /// <param name="owner">The object that is subscribing to the changing event.</param>
        /// <param name="handler">The changing event handler.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
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


        /// <summary>
        ///     Subscribe to the property changing event on this settings object for a specific property.
        /// </summary>
        /// <param name="owner">The object that is subscribing to the changing event.</param>
        /// <param name="propertyName">The name of the property to subscribe to.</param>
        /// <param name="handler">The changing event handler.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public void SubscribePropertyChanging(object owner, string propertyName,
            Action<SettingsPropertyChangingEventArgs<TSetting>> handler)
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


        /// <summary>
        ///     Remove all property changing subscriptions belonging to an object from this settings object and all child settings
        ///     objects recursively.
        /// </summary>
        /// <param name="owner">The subscriber to remove subscriptions for.</param>
        public void UnSubscribePropertyChangingRecursive(object owner)
        {
            _subscribedChanging.Remove(owner);

            foreach (var child in GetAllNonNullSettingsBaseChildren())
            {
                var subscribeMethod = child.GetType().GetMethod("UnSubscribePropertyChangingRecursive");

                subscribeMethod.Invoke(child, new[] {owner});
            }
        }


        /// <summary>
        ///     Remove a property changing subscription from this settings object given the subscriber object.
        /// </summary>
        /// <param name="owner">The subscriber to remove the subscription for.</param>
        public void UnSubscribePropertyChanging(object owner)
        {
            _subscribedChanging.Remove(owner);
        }


        /// <summary>
        ///     Remove a specific property changing subscription from this settings object given the subscriber object and the
        ///     property name.
        /// </summary>
        /// <param name="owner">The subscriber to remove the subscription for.</param>
        /// <param name="propertyName">The property name to remove the subscription for.</param>
        public void UnSubscribePropertyChanging(object owner, string propertyName)
        {
            var dictRef = _subscribedChanging[owner];
            dictRef.Remove(propertyName);
            if (dictRef.Count == 0)
            {
                _subscribedChanging.Remove(owner);
            }
        }


        /// <summary>
        ///     Occurs when after a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
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


        /// <summary>
        ///     A tool for implementing the property changed/changing interface that goes in a public properties set handler.
        ///     This should be used in properties that wish to abide by the property changed/changing interface.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The backing field of the property.</param>
        /// <param name="value">The new value the property is being set to.</param>
        /// <param name="propertyName">Name of the property being set.</param>
        /// <returns></returns>
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
            if (newVal == null || oldVal == null) return;

            const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                           | BindingFlags.Static;


            var subscribedChanging = baseType.GetField("_subscribedChanging", bindFlags);
            var subscribedChanged = baseType.GetField("_subscribedChanged", bindFlags);

            if (subscribedChanged == null || subscribedChanging == null)
            {
                throw new ArgumentException(
                    "baseType was expected to have the private properties _subscribedChanged and _subscribedChanging, but did not",
                    "baseType");
            }


            var changing = subscribedChanging.GetValue(oldVal) as ICloneable;
            var changed = subscribedChanged.GetValue(oldVal) as ICloneable;

            if (changing == null)
            {
                throw new InvalidOperationException(
                    "SettingsBaseClass.TransferObservers: baseType._subscribedChanging does not implement ICloneable.");
            }

            if (changed == null)
            {
                throw new InvalidOperationException(
                    "SettingsBaseClass.TransferObservers: baseType._subscribedChanged does not implement ICloneable.");
            }

            subscribedChanging.SetValue(newVal, changing.Clone());
            subscribedChanged.SetValue(newVal, changed.Clone());
        }


        /// <summary>
        ///     Returns a hash code for this settings object.  This uses the hash code all public instance fields and properties to
        ///     generate a hash.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
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


        /// <summary>
        ///     Determines whether the specified <see cref="SettingsBaseClass{T}" />, is equal to this instance by comparing its
        ///     public instance fields and properties.
        /// </summary>
        /// <param name="other">The <see cref="SettingsBaseClass{T}" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="SettingsBaseClass{T}" /> is equal to this instance; otherwise, <c>false</c>
        ///     .
        /// </returns>
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


        /// <summary>
        ///     Deep clones this settings object.
        /// </summary>
        /// <returns>A deep clone of this settings object.</returns>
        public TSetting Clone()
        {
            ICloneable i = this;
            return (TSetting) i.Clone();
        }


        /// <summary>
        ///     Assign all settings properties
        /// </summary>
        /// <param name="other"></param>
        public void MemberwiseAssign(TSetting other)
        {
            var myType = GetType();

            var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var props = myType.GetProperties(BindingFlags.Instance | BindingFlags.Public);


            foreach (var field in fields)
            {
                field.SetValue(this, field.GetValue(other));
            }

            foreach (var prop in props.Where(x => x.CanRead && x.CanWrite))
            {
                prop.SetValue(this, prop.GetValue(other, null), null);
            }
        }


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
    }
}