using System.Reflection;
using LibLSLCC.Compilers.OpenSim;
using LibLSLCC.Settings;
using LSLCCEditor.EditControl;

namespace LSLCCEditor.Settings
{
    public class EditorControlThemeNode : SettingsBaseClass<EditorControlThemeNode>
    {
        private LSLEditorControlTheme _theme;

        private class ControlThemeDefaultValueFactory : IDefaultSettingsValueFactory
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
                return new LSLEditorControlTheme();
            }
        }


        [DefaultValueFactory(typeof(ControlThemeDefaultValueFactory), initOrder: 0)]
        public LSLEditorControlTheme Theme
        {
            get { return _theme; }
            set { SetField(ref _theme, value, "Theme"); }
        }
    }
}