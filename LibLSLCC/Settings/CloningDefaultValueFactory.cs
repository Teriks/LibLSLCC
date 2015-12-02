using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LibLSLCC.Settings
{
    public class CloningDefaultValueFactory<T> : IDefaultSettingsValueFactory where T : new()
    {
        private static readonly T Default = new T();
        private static readonly DefaultCloner Cloner = new DefaultCloner();
        public bool CheckForNecessaryResets(MemberInfo member, object objectInstance, object settingValue)
        {
            return settingValue == null;
        }

        public object GetDefaultValue(MemberInfo member, object objectInstance)
        {
            var prop = member as PropertyInfo;
            var field = member as FieldInfo;

            var defaultValue = prop != null ? prop.GetValue(Default, null) : field.GetValue(Default);

            var clone = Cloner.Clone(defaultValue);

            return clone ?? defaultValue;
        }
    }
}
