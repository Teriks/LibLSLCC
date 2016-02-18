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
using LibLSLCC.Collections;

namespace LibLSLCC.Settings
{
    /// <summary>
    /// A static class with utilities for initializing an objects field/property's with default values.
    /// </summary>
    /// <seealso cref="IDefaultSettingsValueFactory"/>
    /// <seealso cref="DefaultValueFactoryAttribute"/>
    /// <seealso cref="DefaultValueAttribute"/>
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
                throw new ArgumentException(typeof (DefaultValueInitializer).FullName +
                                                    ".SetValue(): field was not a FieldInfo or PropertyInfo derivative of MemberInfo.");
            }
        }


        /// <summary>
        /// Get the default value for a given public instance field/property in an object instance.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <param name="memberName">The field/property name.</param>
        /// <typeparam name="T">The <see cref="Type"/> of <paramref name="instance"/></typeparam>
        /// <returns>The default value for the public field/property specified by <paramref name="memberName"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>
        /// If <paramref name="instance"/> or <paramref name="memberName"/> are <c>null</c>.
        /// </para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>
        /// If <paramref name="memberName"/> is a property that is not both readable and writable.
        /// </para>
        /// <para>
        /// If <paramref name="memberName"/> is all whitespace.
        /// </para>
        /// <para>
        /// If <paramref name="memberName"/> does not exist in <paramref name="instance"/>.
        /// </para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// If the field/property does not possess a <see cref="DefaultValueFactoryAttribute"/> or <see cref="DefaultValueAttribute"/> and
        /// its declared type possesses no default constructor.
        /// </para>
        /// <para>
        /// If the field/property possesses both <see cref="DefaultValueFactoryAttribute"/> and <see cref="DefaultValueAttribute"/> at once.
        /// </para>
        /// </exception>
        /// <seealso cref="IDefaultSettingsValueFactory"/>
        /// <seealso cref="DefaultValueFactoryAttribute"/>
        /// <seealso cref="DefaultValueAttribute"/>
        public static object GetDefaultValue<T>(T instance, string memberName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }

            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentException("memberName must not be whitespace.", "memberName");
            }

            var settingsProperty =
                instance.GetType()
                    .GetProperty(memberName, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            var settingsField =
                instance.GetType()
                    .GetField(memberName, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);


            if (settingsProperty == null && settingsField == null)
            {

                throw new ArgumentException(typeof(DefaultValueInitializer).FullName +
                                    string.Format(".GetDefaultValue(): field/property with the name of \"{0}\" did not exist in the given type \"{1}\".", memberName, typeof(T).FullName), "memberName");
            }


            const BindingFlags constructorBindingFlags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;


            var member = settingsField == null ? settingsProperty : (MemberInfo)settingsField;
            

            var asField = member as FieldInfo;
            var asProp = member as PropertyInfo;

            var isField = asField != null;

            if (!isField && !(asProp.CanRead && asProp.CanWrite))
            {
                string verbs;
                if (!asProp.CanRead && !asProp.CanWrite)
                {
                    verbs = "readable or writeable";
                }
                else if (!asProp.CanRead)
                {
                    verbs = "readable";
                }
                else
                {
                    verbs = "writeable";
                }

                throw new ArgumentException(typeof(DefaultValueInitializer).FullName +
                                    string.Format(".GetDefaultValue(): field/property with the name of \"{0}\" in type \"{1}\" is not \"{2}\".",
                                    memberName, typeof(T).FullName, verbs), "memberName");
            }


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



        /// <summary>
        /// Sets a field/property of <paramref name="instance"/> to its default value.
        /// </summary>
        /// <param name="instance">The instance of the object containing the field/property.</param>
        /// <param name="memberName">The member name of the field/property.</param>
        /// <typeparam name="T">The <see cref="Type"/> of <paramref name="instance"/></typeparam>
        /// <returns><paramref name="instance"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="instance"/> or <paramref name="memberName"/> are <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <para>
        /// If <paramref name="memberName"/> is a property that is not both readable and writable.
        /// </para>
        /// <para>
        /// If <paramref name="memberName"/> is all whitespace.
        /// </para>
        /// <para>
        /// If <paramref name="memberName"/> does not exist in <paramref name="instance"/>.
        /// </para>
        /// </exception>
        /// <exception cref="InvalidOperationException"> 
        /// <para>
        /// If the field/property does not possess a <see cref="DefaultValueFactoryAttribute"/> or <see cref="DefaultValueAttribute"/> and
        /// its declared type possesses no default constructor.
        /// </para>
        /// <para>
        /// If the field/property possesses both <see cref="DefaultValueFactoryAttribute"/> and <see cref="DefaultValueAttribute"/> at once.
        /// </para>
        /// </exception>
        /// <seealso cref="IDefaultSettingsValueFactory"/>
        /// <seealso cref="DefaultValueFactoryAttribute"/>
        /// <seealso cref="DefaultValueAttribute"/>
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


        /// <summary>
        /// <para>
        /// Checks for necessary resets on fields/properties of an object using <see cref="IDefaultSettingsValueFactory.CheckForNecessaryResets"/>,
        /// then resets them to their default value using <see cref="IDefaultSettingsValueFactory.GetDefaultValue"/> if necessary.
        /// </para>
        /// <para>
        /// This only affects fields/properties possessing a <see cref="DefaultValueFactoryAttribute"/>.
        /// </para>
        /// </summary>
        /// <param name="instance">The object instance to check for and preform necessary field/property resets on.</param>
        /// <typeparam name="T">The <see cref="Type"/> of <paramref name="instance"/>.</typeparam>
        /// <returns><paramref name="instance"/></returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a field/property possesses both <see cref="DefaultValueFactoryAttribute"/> and <see cref="DefaultValueAttribute"/> at once.
        /// </exception>
        /// <seealso cref="IDefaultSettingsValueFactory"/>
        /// <seealso cref="DefaultValueFactoryAttribute"/>
        /// <seealso cref="DefaultValueAttribute"/>
        public static T DoNeccessaryResets<T>(T instance)
        {
            var settingsProperties =
                instance.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                    .ToArray();

            var settingsFields =
                instance.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                    .ToArray();


            var valueFactoryInitQueue = new PriorityQueue<int, Action>();


            foreach (var member in settingsProperties.Concat<MemberInfo>(settingsFields))
            {
                var asField = member as FieldInfo;
                var asProp = member as PropertyInfo;

                var isField = asField != null;

                if (!isField && !(asProp.CanRead && asProp.CanWrite)) continue;

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

                if (!factoryAttribute.Any()) continue;

                var factory = ((DefaultValueFactoryAttribute)factoryAttribute.First());

                //copy the for-each item reference into a local
                //to avoid a possible compiler portability issue when using it in a lambda enclosure
                var localMember = member;
                valueFactoryInitQueue.Enqueue(factory.InitOrder, () =>
                {
                    if (factory.Factory.CheckForNecessaryResets(localMember, instance, fieldValue))
                    {
                        SetValue(localMember, instance, factory.Factory.GetDefaultValue(localMember, instance));
                    }
                });
            }

            while (!valueFactoryInitQueue.IsEmpty)
            {
                //invoke the queued up actions for DefaultValueFactoryAttribute in order after all the other fields have been initialized
                valueFactoryInitQueue.Dequeue().Value();
            }

            return instance;
        }




        /// <summary>
        /// Initializes all fields/properties in a given object to their default values.
        /// </summary>
        /// <param name="instance">The object instance do the field/property reset on.</param>
        /// <typeparam name="T">The <see cref="Type"/> of <paramref name="instance"/>.</typeparam>
        /// <returns><paramref name="instance"/></returns>
        /// <exception cref="InvalidOperationException"> 
        /// Thrown if the field/property possesses both <see cref="DefaultValueFactoryAttribute"/> and <see cref="DefaultValueAttribute"/> at once.
        /// </exception>
        /// <seealso cref="IDefaultSettingsValueFactory"/>
        /// <seealso cref="DefaultValueFactoryAttribute"/>
        /// <seealso cref="DefaultValueAttribute"/>
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

                if (!isField && !(asProp.CanRead && asProp.CanWrite)) continue;

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
                        if (factory.Factory.CheckForNecessaryResets(localMember, instance, fieldValue))
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
                else
                {
                    var constructors =
                        fieldType.GetConstructors(constructorBindingFlags).Where(x => !x.GetParameters().Any()).ToList();

                    if (!constructors.Any()) continue;

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