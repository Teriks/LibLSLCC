using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public readonly ObservableCollection<ISettingsPane> _settingPanes = new ObservableCollection<ISettingsPane>();

        public SettingsWindow()
        {

            InitializeComponent();

            var myType = typeof(SettingsWindow);
            var panes = myType.Assembly.GetTypes().Where(t => string.Equals(t.Namespace, myType.Namespace, StringComparison.Ordinal) && t.GetInterfaces().Any(i => i == typeof(ISettingsPane)));

            var compilerPane = new CompilerPane() {Owner = this};
            _settingPanes.Add(compilerPane);
            

            SettingsPagesList.SelectedItem = compilerPane; 
        }

        public ObservableCollection<ISettingsPane> SettingPanes
        {
            get { return _settingPanes; }
        }
    }
}
