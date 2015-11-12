using System.Windows.Forms;

namespace LSLCCEditor.SettingsUI
{
    public interface ISettingsPane
    {

        string Title { get; }


        SettingsWindow Owner { get; set; }
    }
}