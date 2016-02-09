#region FileInfo
// 
// File: FormatterPane.xaml.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 
#endregion
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Serialization;
using LibLSLCC.Formatter;
using LSLCCEditor.Settings;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

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

        private void Import_OnClick(object sender, RoutedEventArgs e)
        {

            var dialogResult = MessageBox.Show(OwnerSettingsWindow,
            "Are you sure you want to overwrite the currently selected configuration by importing one over it?", "Overwrite Selected Configuration?",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dialogResult != MessageBoxResult.Yes) return;

            ImportExportTools.DoImportSettingsWindow(OwnerSettingsWindow, "Formatter Configuration (*.xml)|*.xml", ".xml",
            reader =>
            {
                var x = new XmlSerializer(typeof(LSLCodeFormatterSettings));
                CurrentFormatterSettings.MemberwiseAssign((LSLCodeFormatterSettings)x.Deserialize(reader));
            });
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            ImportExportTools.DoExportSettingsWindow(OwnerSettingsWindow, "Formatter Configuration (*.xml)|*.xml", "Formatter_Configuration.xml",
            writer =>
            {
                var x = new XmlSerializer(typeof(LSLCodeFormatterSettings));
                x.Serialize(writer,CurrentFormatterSettings);
            });
        }
    }
}
