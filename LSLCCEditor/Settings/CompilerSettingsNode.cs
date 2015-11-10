using LibLSLCC.Compilers;
using LibLSLCC.Utility;

namespace LSLCCEditor.Settings
{
    public class CompilerSettingsNode : SettingsBaseClass
    {
        private LSLOpenSimCompilerSettings _openSimCompilerSettings;


        private class CompilerSettingsDefaultValueFactory : IDefaultSettingsValueFactory
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
                return new LSLOpenSimCompilerSettings();
            }
        }


        [DefaultValueFactory(typeof(CompilerSettingsDefaultValueFactory))]
        public LSLOpenSimCompilerSettings OpenSimCompilerSettings
        {
            get { return _openSimCompilerSettings; }
            set { SetField(ref _openSimCompilerSettings,value,"OpenSimCompilerSettings"); }
        }
    }
}