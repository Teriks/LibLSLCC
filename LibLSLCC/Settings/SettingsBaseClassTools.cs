using System;

namespace LibLSLCC.Settings
{
    public static class SettingsBaseClassTools
    {
        public static bool HasSettingsBase(Type type, out Type baseType)
        {
            var baseSearch = type.BaseType;

            bool isSettingsBase = false;

            while (baseSearch != null)
            {
                var genericTickIndex = baseSearch.FullName.IndexOf("`", StringComparison.Ordinal);
                if (genericTickIndex == -1) break;
                var nonGenericName = baseSearch.FullName.Substring(0, genericTickIndex);
                if (nonGenericName != "LibLSLCC.Settings.SettingsBaseClass")
                {
                    baseSearch = baseSearch.BaseType;
                    continue;
                }


                isSettingsBase = true;
                break;
            }

            baseType = isSettingsBase ? baseSearch : null;
            return isSettingsBase;
        }


        public static bool HasSettingsBase(Type type)
        {
            Type discard;
            return HasSettingsBase(type, out discard);
        }
    }
}