using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using LibLSLCC.Collections;

namespace LibLSLCC.Settings
{
    public static class DefaultValueInitializer
    {
        public static void Init(object instance)
        {
            var settingsProperties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).ToArray();


            const BindingFlags constructorBindingFlags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;


            PriorityQueue<uint, Action> valueFactoryInitQueue = new PriorityQueue<uint, Action>();


            foreach (var field in settingsProperties)
            {

                var fieldValue = field.GetValue(instance, null);

                var factoryAttribute = field.GetCustomAttributes(typeof(DefaultValueFactoryAttribute), true).ToList();

                var defaultAttribute = field.GetCustomAttributes(typeof(DefaultValueAttribute), true).ToList();

                if (defaultAttribute.Any() && factoryAttribute.Any())
                {
                    throw new InvalidOperationException(string.Format("Property: '{0} {1}.{2}' uses both a [DefaultValueFactoryAttribute] and a[ DefaultValueAttribute].", 
                        field.PropertyType.FullName, 
                        field.DeclaringType.FullName, 
                        field.Name));
                }

                if (factoryAttribute.Any())
                {
                    var factory = ((DefaultValueFactoryAttribute)factoryAttribute.First());

                    //copy the foreach item reference into a local
                    //to avoid a possible compiler portability issue when using it in a lambda enclosure
                    var localField = field;
                    valueFactoryInitQueue.Enqueue(factory.InitOrder, () =>
                    {

                        if (factory.Factory.CheckForNecessaryResets(instance, fieldValue))
                        {
                            localField.SetValue(instance, factory.Factory.GetDefaultValue(instance), null);
                        }

                    });

                }
                else if (defaultAttribute.Any())
                {
                    var defaultValue = ((DefaultValueAttribute)factoryAttribute.First());

                    field.SetValue(instance, defaultValue.Value, null);

                }
                else if (fieldValue == null)
                {

                    var constructors = field.PropertyType.GetConstructors(constructorBindingFlags).Where(x => !x.GetParameters().Any()).ToList();
                    if (!constructors.Any())
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "{0}.Init(object instance):  Property '{1} {2}.{3};' is defined with a type that has no parameterless constructor," 
                                + " use the [DefaultValueFactory] attribute on the property.",
                                typeof(DefaultValueInitializer).FullName,
                                field.PropertyType.FullName,
                                field.DeclaringType.FullName,
                                field.Name));
                    }

                    var constructor = constructors.First();

                    object newInstance = constructor.Invoke(null);
                    Init(newInstance);
                    field.SetValue(instance, constructor.Invoke(null), null);
                    
                }
            }



            while (!valueFactoryInitQueue.IsEmpty)
            {
                //invoke the queued up actions for DefaultValueFactoryAttribute in order after all the other fields have been initialized
                valueFactoryInitQueue.Dequeue().Value();
            }
        }
    }
}