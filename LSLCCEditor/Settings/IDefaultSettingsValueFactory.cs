namespace LSLCCEditor.Settings
{
    public interface IDefaultSettingsValueFactory
    {

        bool NeedsToBeReset(SettingsNode settingsNode, object obj);

        object GetDefaultValue(SettingsNode settingsNode);
    }
}