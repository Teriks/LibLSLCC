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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LSLCCEditor.Settings;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for CompilerPane.xaml
    /// </summary>
    public partial class CompilerPane : UserControl, ISettingsPane
    {

        public ObservableCollection<string> CompilerConfigurations { get; set; }


        public static readonly DependencyProperty SelectedCompilerConfigurationProperty = DependencyProperty.Register(
            "SelectedCompilerConfiguration", typeof (string), typeof (CompilerPane), new PropertyMetadata(default(string), SelectedCompilerConfigurationChanged));

        private static void SelectedCompilerConfigurationChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {


            var pane = ((CompilerPane) dependencyObject);


            pane.CurrentCompilerConfiguration =
                (CompilerSettingsNode)AppSettings.Settings.CompilerConfigurations[dependencyPropertyChangedEventArgs.NewValue.ToString()].Clone();



        }


        public static readonly DependencyProperty CurrentCompilerConfigurationProperty = DependencyProperty.Register(
            "CurrentCompilerConfiguration", typeof (CompilerSettingsNode), typeof (CompilerPane), new PropertyMetadata(default(CompilerSettingsNode)));

        public CompilerSettingsNode CurrentCompilerConfiguration
        {
            get { return (CompilerSettingsNode) GetValue(CurrentCompilerConfigurationProperty); }
            set { SetValue(CurrentCompilerConfigurationProperty, value); }
        }

        public string SelectedCompilerConfiguration
        {
            get { return (string) GetValue(SelectedCompilerConfigurationProperty); }
            set { SetValue(SelectedCompilerConfigurationProperty, value); }
        }


        public CompilerPane()
        {
            this.DataContext = this;

            Title = "Compiler Settings";

            CompilerConfigurations = new ObservableCollection<string>(ApplicationSettings.CompilerConfigurations.Keys);


            InitializeComponent();


            

            SelectedCompilerConfiguration = CompilerConfigurations.First();




        }

        public SettingsNode ApplicationSettings
        {
            get { return AppSettings.Settings; }
        }

        public int Priority { get { return 1; } }

        public string Title { get; private set; }

        public void Init(SettingsWindow window)
        {
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            AppSettings.Settings.CompilerConfigurations[SelectedCompilerConfiguration] = CurrentCompilerConfiguration;
            AppSettings.Save();
        }
    }
}
