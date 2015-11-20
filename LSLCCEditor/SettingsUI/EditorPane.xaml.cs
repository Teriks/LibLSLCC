using System.Windows.Controls;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for EditorPane.xaml
    /// </summary>
    public partial class EditorPane : UserControl, ISettingsPane
    {
        public EditorPane()
        {
            InitializeComponent();

            Title = "Editor Settings";
        }

        public string Title { get; private set; }
        public SettingsWindow OwnerSettingsWindow { get; set; }

    }
}
