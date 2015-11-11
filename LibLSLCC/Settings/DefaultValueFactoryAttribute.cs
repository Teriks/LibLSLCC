using System;

namespace LibLSLCC.Settings
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DefaultValueFactoryAttribute : Attribute
    {
        public uint InitOrder { get; private set; }

        public IDefaultSettingsValueFactory Factory { get; private set; }

        public DefaultValueFactoryAttribute(Type factoryType, uint initOrder)
        {
            InitOrder = initOrder;
            Factory = Activator.CreateInstance(factoryType) as IDefaultSettingsValueFactory;

            if (Factory == null)
            {
                throw new Exception(string.Format("Cannot use '{0}' as a default value factory as it does not implement IDefaultSettingsValueFactory.", factoryType.FullName));
            }
        }

    }
}