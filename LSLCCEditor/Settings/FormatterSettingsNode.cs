using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LibLSLCC.Formatter;
using LibLSLCC.Settings;

namespace LSLCCEditor.Settings
{
    public class FormatterSettingsNode : SettingsBaseClass<FormatterSettingsNode>
    {

        private LSLCodeFormatterSettings _formatterSettings;


        private class FormatterSettingsValueFactory : IDefaultSettingsValueFactory
        {
            public bool CheckForNecessaryResets(MemberInfo member, object objectInstance, object settingValue)
            {
                if (settingValue == null)
                {
                    return true;
                }
                return false;
            }

            public object GetDefaultValue(MemberInfo member, object objectInstance)
            {
                return new LSLCodeFormatterSettings();
            }
        }


        [DefaultValueFactory(typeof(FormatterSettingsValueFactory), initOrder: 0)]
        public LSLCodeFormatterSettings FormatterSettings
        {
            get { return _formatterSettings; }
            set { SetField(ref _formatterSettings, value, "FormatterSettings"); }
        }
    }
}
