using System.Windows.Controls;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for CompletionPane.xaml
    /// </summary>
    public partial class CompletionPane : UserControl,  ISettingsPane
    {
        public CompletionPane()
        {
            InitializeComponent();
            Title = "Completion Settings";
        }


        public string Title { get; private set; }
        public SettingsWindow OwnerSettingsWindow { get; set; }


    }
}
