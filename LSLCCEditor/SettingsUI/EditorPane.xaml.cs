using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Serialization;
using LibLSLCC.Settings;
using LSLCCEditor.EditControl;
using LSLCCEditor.Settings;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.Forms.MessageBox;

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
            CurrentEditorConfigurationName = AppSettings.Settings.CurrentEditorControlConfiguration;
            EditorConfigurationNames = new ObservableCollection<string>(AppSettings.Settings.EditorControlConfigurations.Keys);
        }


        public static readonly DependencyProperty EditorConfigurationNamesProperty = DependencyProperty.Register(
            "EditorConfigurationNames", typeof (ObservableCollection<string>), typeof (EditorPane), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> EditorConfigurationNames
        {
            get { return (ObservableCollection<string>) GetValue(EditorConfigurationNamesProperty); }
            set { SetValue(EditorConfigurationNamesProperty, value); }
        }

        public string Title { get; private set; }
        public SettingsWindow OwnerSettingsWindow { get; set; }


        public static readonly DependencyProperty CurrentEditorConfigurationNameProperty = DependencyProperty.Register(
            "CurrentEditorConfigurationName", typeof (string), typeof (EditorPane), new PropertyMetadata(default(string), CurrentEditorConfigurationChangedCallback));

        private static void CurrentEditorConfigurationChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var editorPane = (EditorPane) dependencyObject;

            var newValue = dependencyPropertyChangedEventArgs.NewValue.ToString();

            AppSettings.Settings.CurrentEditorControlConfiguration = newValue;

            editorPane.EditorControlSettings =
                AppSettings.Settings.EditorControlConfigurations[newValue].EditorControlSettings;

        }

        public static readonly DependencyProperty EditorControlSettingsNodeProperty = DependencyProperty.Register(
            "EditorControlSettings", typeof (LSLEditorControlSettings), typeof (EditorPane), new PropertyMetadata(default(LSLEditorControlSettings)));

        public LSLEditorControlSettings EditorControlSettings
        {
            get { return (LSLEditorControlSettings) GetValue(EditorControlSettingsNodeProperty); }
            set { SetValue(EditorControlSettingsNodeProperty, value); }
        }


        public string CurrentEditorConfigurationName
        {
            get { return (string) GetValue(CurrentEditorConfigurationNameProperty); }
            set { SetValue(CurrentEditorConfigurationNameProperty, value); }
        }

        private void ResetHighlightingColor_OnClick(object sender, RoutedEventArgs e)
        {
            var colorBox = ((Border) ((StackPanel) ((FrameworkElement) sender).Parent).Children[1]).Child;

            var bindingExpression = BindingOperations.GetBindingExpression((FrameworkElement)colorBox, ColorPicker.SelectedColorProperty);
            var propNames = bindingExpression.ParentBinding.Path.Path.Split('.');

            var propName = propNames[propNames.Length - 2];
            var context = propNames[1];
            if (context == "HighlightingColors")
            {
                DefaultValueInitializer.SetToDefault(EditorControlSettings.HighlightingColors, propName);
            }
            else
            {
                DefaultValueInitializer.SetToDefault(EditorControlSettings, propName);
            }
        }

        private void ResetAllHighlightingColors_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var color in HighlightingColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox=((Border) color.Children[1]).Child;
                var bindingExpression = BindingOperations.GetBindingExpression((FrameworkElement)colorBox, ColorPicker.SelectedColorProperty);
                var propNames = bindingExpression.ParentBinding.Path.Path.Split('.');

                var propName = propNames[propNames.Length - 2];

                var context = propNames[1];

                if (context == "HighlightingColors")
                {
                    DefaultValueInitializer.SetToDefault(EditorControlSettings.HighlightingColors, propName);
                }
                else
                {
                    DefaultValueInitializer.SetToDefault(EditorControlSettings, propName);
                }
            }
        }

        private void NewConfiguration_OnClick(object sender, RoutedEventArgs e)
        {
            var name = new UniqueNamerWindow(AppSettings.Settings.CompilerConfigurations.Keys, "My Configuration")
            {
                Owner = OwnerSettingsWindow
            };
            name.ShowDialog();

            if (name.Canceled) return;


            AppSettings.Settings.AddEditorConfiguration(name.ChosenName);

            EditorConfigurationNames.Add(name.ChosenName);

            EditorConfigurationCombobox.SelectedIndex = EditorConfigurationNames.Count - 1;
        }

        private void DeleteConfiguration_OnClick(object sender, RoutedEventArgs e)
        {
            var currentlySelected = CurrentEditorConfigurationName;

            int newIndex = 0;

            if ((EditorConfigurationNames.Count - 1) > 0 && EditorConfigurationCombobox.SelectedIndex > 0)
            {
                newIndex = EditorConfigurationCombobox.SelectedIndex - 1;
            }


            AppSettings.Settings.RemoveEditorConfiguration(currentlySelected, EditorConfigurationNames[newIndex]);

            EditorConfigurationNames.Remove(CurrentEditorConfigurationName);

            EditorConfigurationCombobox.SelectedIndex = newIndex;
        }

        private void ImportHighlightingColors_OnClick(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Highlighting Colors (*.xml)|*.xml;",
                AddExtension = true,
                DefaultExt = ".xml"
 
            };



            if (!openDialog.ShowDialog(OwnerSettingsWindow).Value)
            {
                return;
            }

            try
            {
                using (var file = new XmlTextReader(openDialog.OpenFile()))
                {
                    var x = new XmlSerializer(typeof (LSLEditorControlHighlightingColors));

                    EditorControlSettings.HighlightingColors = (LSLEditorControlHighlightingColors) x.Deserialize(file);
                }
            }
            catch (XmlSyntaxException ex)
            {
                MessageBox.Show("An XML syntax error was encountered while loading the file, settings could not be applied: "
                    + Environment.NewLine + Environment.NewLine + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an unknown error while loading the settings file, settings could not be applied: "
                    + Environment.NewLine+Environment.NewLine+ex.Message);
            }
        }



        private void ExportHighlightingColors_OnClick(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Highlighting Colors (*.xml)|*.xml;",
                FileName = "LSLCCEditor_HighlightingColors.xml"
            };


            if (!saveDialog.ShowDialog(OwnerSettingsWindow).Value)
            {
                return;
            }

            try
            {
                using (var file = new XmlTextWriter(saveDialog.OpenFile(), Encoding.Unicode))
                {
                    file.Formatting = Formatting.Indented;

                    var x = new XmlSerializer(typeof (LSLEditorControlHighlightingColors));

                    x.Serialize(file, EditorControlSettings.HighlightingColors);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected problem occurred while trying to save the file: " 
                    + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }
    }
}
