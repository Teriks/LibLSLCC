using System.Collections.ObjectModel;
using System.Windows;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly ObservableCollection<ISettingsPane> _settingPanes = new ObservableCollection<ISettingsPane>();

        public SettingsWindow()
        {

            InitializeComponent();

            var myType = typeof(SettingsWindow);
            //var panes = myType.Assembly.GetTypes().Where(t => string.Equals(t.Namespace, myType.Namespace, StringComparison.Ordinal) && t.GetInterfaces().Any(i => i == typeof(ISettingsPane)));

            var compilerPane = new CompilerPane {OwnerSettingsWindow = this};
            _settingPanes.Add(compilerPane);

           
            var editorPane = new EditorPane { OwnerSettingsWindow = this };
            _settingPanes.Add(editorPane);

           

            SettingsPagesList.SelectedItem = compilerPane; 
        }

        public ObservableCollection<ISettingsPane> SettingPanes
        {
            get { return _settingPanes; }
        }
    }
}
