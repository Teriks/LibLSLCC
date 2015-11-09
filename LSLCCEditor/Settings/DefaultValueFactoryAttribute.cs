using System;

namespace LSLCCEditor.Settings
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DefaultValueFactoryAttribute : Attribute
    {
        
        public IDefaultSettingsValueFactory Factory { get; private set; }

        public DefaultValueFactoryAttribute(Type factoryType)
        {
            Factory = Activator.CreateInstance(factoryType) as IDefaultSettingsValueFactory;

            if (Factory == null)
            {
                throw new Exception(string.Format("Cannot use '{0}' as a default value factory as it does not implement IDefaultSettingsValueFactory.", factoryType.FullName));
            }
        }

    }
}