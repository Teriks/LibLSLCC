namespace LSLCCEditor.SettingsUI
{
    public interface ISettingsPane
    {
        int Priority { get; }

        string Title { get; }

        void Init(SettingsWindow window);
    }
}