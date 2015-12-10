using System;
using System.Collections.ObjectModel;
using System.Linq;
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


            var compilerPane = new CompilerPane {OwnerSettingsWindow = this};
            _settingPanes.Add(compilerPane);

            var formatterPane = new FormatterPane { OwnerSettingsWindow = this };
            _settingPanes.Add(formatterPane);


            var editorPane = new EditorThemePane { OwnerSettingsWindow = this };
            _settingPanes.Add(editorPane);



            SettingsPagesList.SelectedItem = compilerPane; 
        }

        public ObservableCollection<ISettingsPane> SettingPanes
        {
            get { return _settingPanes; }
        }
    }
}
