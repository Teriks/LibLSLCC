namespace LSLCCEditor.Settings
{
    public interface IDefaultSettingsValueFactory
    {

        bool CheckForNecessaryResets(object objectInstance, object settingValue);

        object GetDefaultValue(object objectInstance);
    }
}