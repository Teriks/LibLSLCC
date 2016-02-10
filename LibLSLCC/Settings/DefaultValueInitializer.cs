#region FileInfo
// 
// File: DefaultValueInitializer.cs
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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using LibLSLCC.Collections;

namespace LibLSLCC.Settings
{
    public static class DefaultValueInitializer
    {
        private static void SetValue(MemberInfo field, object instance, object value)
        {
            var asField = field as FieldInfo;
            var asProp = field as PropertyInfo;

            var isProp = asProp != null;
            var isField = asField != null;

            if (isProp)
            {
                asProp.SetValue(instance, value, null);
            }
            else if (isField)
            {
                asField.SetValue(instance, value);
            }
            else
            {
                throw new InvalidOperationException(typeof (DefaultValueInitializer).FullName +
                                                    ".SetValue(): field was not a FieldInfo or PropertyInfo derivative of MemberInfo.");
            }
        }

        public static object GetDefaultValue<T>(T instance, string memberName)
        {
            var settingsProperty =
                instance.GetType()
                    .GetProperty(memberName, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            var settingsField =
                instance.GetType()
                    .GetField(memberName, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

            const BindingFlags constructorBindingFlags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            var member = settingsField == null ? settingsProperty : (MemberInfo)settingsField;
            if (member == null) return instance;

            var asField = member as FieldInfo;
            var asProp = member as PropertyInfo;

            var isField = asField != null;

            if (!isField && !(asProp.CanRead && asProp.CanWrite)) return null;

            var fieldValue = isField ? asField.GetValue(instance) : asProp.GetValue(instance, null);

            var fieldType = isField ? asField.FieldType : asProp.PropertyType;


            var factoryAttribute = member.GetCustomAttributes(typeof(DefaultValueFactoryAttribute), true).ToList();

            var defaultAttribute = member.GetCustomAttributes(typeof(DefaultValueAttribute), true).ToList();

            if (defaultAttribute.Any() && factoryAttribute.Any())
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Property: '{0} {1}.{2}' uses both a [DefaultValueFactoryAttribute] and a [DefaultValueAttribute].",
                        fieldType.FullName,
                        member.DeclaringType.FullName,
                        member.Name));
            }

            if (factoryAttribute.Any())
            {
                var factory = ((DefaultValueFactoryAttribute)factoryAttribute.First());

                return factory.Factory.GetDefaultValue(member, instance);
            }
            if (defaultAttribute.Any())
            {
                var defaultValue = ((DefaultValueAttribute)defaultAttribute.First());

                return defaultValue.Value;
            }
            if (fieldValue == null)
            {
                var constructors =
                    fieldType.GetConstructors(constructorBindingFlags).Where(x => !x.GetParameters().Any()).ToList();
                if (!constructors.Any())
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "{0}.Init(object instance):  Property '{1} {2}.{3};' is defined with a type that has no parameterless constructor,"
                            + " use the [DefaultValueFactory] attribute on the property.",
                            typeof(DefaultValueInitializer).FullName,
                            fieldType.FullName,
                            member.DeclaringType.FullName,
                            member.Name));
                }

                var constructor = constructors.First();

                object newInstance = constructor.Invoke(null);
                Init(newInstance);
                return constructor.Invoke(null);
            }

            return null;
        }



        public static T SetToDefault<T>(T instance, string memberName)
        {
            var defaultValue = GetDefaultValue(instance, memberName);

            var member = typeof (T).GetProperty(memberName) ?? (MemberInfo) typeof(T).GetField(memberName);

            if (member == null) return instance;

            var asField = member as FieldInfo;
            var asProp = member as PropertyInfo;

            var isField = asField != null;

            if (asField == null && asProp == null) return instance;


            if (!isField && !(asProp.CanRead && asProp.CanWrite)) return instance;


            if (defaultValue != null)
            {
                SetValue(member, instance, defaultValue);
            }

            return instance;
        }


        public static T Init<T>(T instance)
        {
            var settingsProperties =
                instance.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                    .ToArray();
            var settingsFields =
                instance.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                    .ToArray();


            const BindingFlags constructorBindingFlags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;


            var valueFactoryInitQueue = new PriorityQueue<int, Action>();


            foreach (var member in settingsProperties.Concat<MemberInfo>(settingsFields))
            {
                var asField = member as FieldInfo;
                var asProp = member as PropertyInfo;

                var isField = asField != null;

                if(!isField && !(asProp.CanRead && asProp.CanWrite)) continue;

                var fieldValue = isField ? asField.GetValue(instance) : asProp.GetValue(instance, null);

                var fieldType = isField ? asField.FieldType : asProp.PropertyType;


                var factoryAttribute = member.GetCustomAttributes(typeof (DefaultValueFactoryAttribute), true).ToList();

                var defaultAttribute = member.GetCustomAttributes(typeof (DefaultValueAttribute), true).ToList();

                if (defaultAttribute.Any() && factoryAttribute.Any())
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Property: '{0} {1}.{2}' uses both a [DefaultValueFactoryAttribute] and a [DefaultValueAttribute].",
                            fieldType.FullName,
                            member.DeclaringType.FullName,
                            member.Name));
                }

                if (factoryAttribute.Any())
                {
                    var factory = ((DefaultValueFactoryAttribute) factoryAttribute.First());

                    //copy the for-each item reference into a local
                    //to avoid a possible compiler portability issue when using it in a lambda enclosure
                    var localMember = member;
                    valueFactoryInitQueue.Enqueue(factory.InitOrder, () =>
                    {
                        if (factory.Factory.CheckForNecessaryResets(localMember,instance, fieldValue))
                        {
                            SetValue(localMember, instance, factory.Factory.GetDefaultValue(localMember, instance));
                        }
                    });
                }
                else if (defaultAttribute.Any())
                {
                    var defaultValue = ((DefaultValueAttribute) defaultAttribute.First());

                    SetValue(member, instance, defaultValue.Value);
                }
                else if (fieldValue == null)
                {
                    var constructors =
                        fieldType.GetConstructors(constructorBindingFlags).Where(x => !x.GetParameters().Any()).ToList();
                    if (!constructors.Any())
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "{0}.Init(object instance):  Property '{1} {2}.{3};' is defined with a type that has no parameterless constructor,"
                                + " use the [DefaultValueFactory] attribute on the property.",
                                typeof (DefaultValueInitializer).FullName,
                                fieldType.FullName,
                                member.DeclaringType.FullName,
                                member.Name));
                    }

                    var constructor = constructors.First();

                    object newInstance = constructor.Invoke(null);
                    Init(newInstance);
                    SetValue(member, instance, constructor.Invoke(null));
                }
            }

            while (!valueFactoryInitQueue.IsEmpty)
            {
                //invoke the queued up actions for DefaultValueFactoryAttribute in order after all the other fields have been initialized
                valueFactoryInitQueue.Dequeue().Value();
            }

            return instance;
        }


    }
}