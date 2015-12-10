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
using LibLSLCC.Formatter;
using LSLCCEditor.Settings;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for FormatterPane.xaml
    /// </summary>
    public partial class FormatterPane : UserControl, ISettingsPane
    {
        public static readonly DependencyProperty FormatterConfigurationNamesProperty = DependencyProperty.Register(
            "FormatterConfigurationNames", typeof (ObservableCollection<string>), typeof (FormatterPane), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> FormatterConfigurationNames
        {
            get { return (ObservableCollection<string>) GetValue(FormatterConfigurationNamesProperty); }
            set { SetValue(FormatterConfigurationNamesProperty, value); }
        }


        public static readonly DependencyProperty SelectedFormatterConfigurationNameProperty = DependencyProperty.Register(
            "SelectedFormatterConfigurationName", typeof (string), typeof (FormatterPane), new PropertyMetadata(default(string), SelectedFormatterConfigurationNameChangedCallback));

        private static void SelectedFormatterConfigurationNameChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyPropertyChangedEventArgs.NewValue != null)
            {
                var self = (FormatterPane) dependencyObject;

                AppSettings.Settings.CurrentFormatterConfigurationName =
                    dependencyPropertyChangedEventArgs.NewValue.ToString();

                self.CurrentFormatterSettings = AppSettings.Settings.CurrentFormatterConfiguration.FormatterSettings;
            }
        }

        public string SelectedFormatterConfigurationName
        {
            get { return (string) GetValue(SelectedFormatterConfigurationNameProperty); }
            set { SetValue(SelectedFormatterConfigurationNameProperty, value); }
        }


        public static readonly DependencyProperty CurrentFormatterSettingsProperty = DependencyProperty.Register(
            "CurrentFormatterSettings", typeof (LSLCodeFormatterSettings), typeof (FormatterPane), new PropertyMetadata(default(LSLCodeFormatterSettings)));

        public LSLCodeFormatterSettings CurrentFormatterSettings
        {
            get { return (LSLCodeFormatterSettings) GetValue(CurrentFormatterSettingsProperty); }
            set { SetValue(CurrentFormatterSettingsProperty, value); }
        }


        public FormatterPane()
        {
            InitializeComponent();
            Title = "Formatter Settings";
            SelectedFormatterConfigurationName = AppSettings.Settings.CurrentFormatterConfigurationName;
            FormatterConfigurationNames = new ObservableCollection<string>(AppSettings.Settings.FormatterConfigurations.Keys);
        }

        public string Title { get; private set; }
        public SettingsWindow OwnerSettingsWindow { get; set; }



        private void New_OnClick(object sender, RoutedEventArgs e)
        {
            var name = new UniqueNamerWindow(AppSettings.Settings.FormatterConfigurations.Keys, "My Configuration")
            {
                Owner = OwnerSettingsWindow
            };
            name.ShowDialog();

            if (name.Canceled) return;


            AppSettings.Settings.AddFormatterConfiguration(name.ChosenName);

            FormatterConfigurationNames.Add(name.ChosenName);

            FormatterConfigurationNameCombobox.SelectedIndex = FormatterConfigurationNames.Count - 1;
        }



        private void Rename_OnClick(object sender, RoutedEventArgs e)
        {
            var x = new UniqueNamerWindow(AppSettings.Settings.FormatterConfigurations.Keys, SelectedFormatterConfigurationName, false);

            x.ShowDialog();

            if (x.Canceled) return;

            AppSettings.Settings.RenameFormatterConfiguration(SelectedFormatterConfigurationName, x.ChosenName);



            FormatterConfigurationNames.Clear();

            foreach (var v in AppSettings.Settings.FormatterConfigurations.Keys)
            {
                FormatterConfigurationNames.Add(v);
            }

            SelectedFormatterConfigurationName = x.ChosenName;
        }



        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            var currentlySelected = SelectedFormatterConfigurationName;

            int newIndex = 0;

            if ((FormatterConfigurationNames.Count - 1) > 0 && FormatterConfigurationNameCombobox.SelectedIndex > 0)
            {
                newIndex = FormatterConfigurationNameCombobox.SelectedIndex - 1;
            }


            AppSettings.Settings.RemoveFormatterConfiguration(currentlySelected, FormatterConfigurationNames[newIndex]);

            FormatterConfigurationNames.Remove(SelectedFormatterConfigurationName);

            FormatterConfigurationNameCombobox.SelectedIndex = newIndex;
        }
    }
}
