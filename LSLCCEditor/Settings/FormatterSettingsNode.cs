using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibLSLCC.Formatter;
using LibLSLCC.Settings;

namespace LSLCCEditor.Settings
{
    public class FormatterSettingsNode : SettingsBaseClass<EditorControlSettingsNode>
    {

        private LSLCodeFormatterSettings _formatterSettings;


        private class FormatterSettingsValueFactory : IDefaultSettingsValueFactory
        {
            public bool CheckForNecessaryResets(object objectInstance, object settingValue)
            {
                if (settingValue == null)
                {
                    return true;
                }
                return false;
            }

            public object GetDefaultValue(object objectInstance)
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
