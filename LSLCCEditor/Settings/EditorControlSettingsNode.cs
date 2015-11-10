using LibLSLCC.Utility;
using LSLCCEditor.EditControl;

namespace LSLCCEditor.Settings
{
    public class EditorControlSettingsNode :  SettingsBaseClass
    {
        private LSLEditorControlSettings _editorControlSettings;


        private class EditorControlSettingsValueFactory : IDefaultSettingsValueFactory
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
                return new LSLEditorControlSettings();
            }
        }


        [DefaultValueFactory(typeof(EditorControlSettingsValueFactory))]
        public LSLEditorControlSettings EditorControlSettings
        {
            get { return _editorControlSettings; }
            set { SetField(ref _editorControlSettings, value,"EditorControlSettings"); }
        }
    }
}