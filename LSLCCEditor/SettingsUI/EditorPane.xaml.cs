using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using LibLSLCC.Settings;
using LSLCCEditor.EditControl;
using LSLCCEditor.Settings;
using LSLCCEditor.Utility.Xml;
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
            EditorConfigurationNames =
                new ObservableCollection<string>(AppSettings.Settings.EditorControlConfigurations.Keys);
        }


        public static readonly DependencyProperty EditorConfigurationNamesProperty = DependencyProperty.Register(
            "EditorConfigurationNames", typeof (ObservableCollection<string>), typeof (EditorPane),
            new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> EditorConfigurationNames
        {
            get { return (ObservableCollection<string>) GetValue(EditorConfigurationNamesProperty); }
            set { SetValue(EditorConfigurationNamesProperty, value); }
        }

        public string Title { get; private set; }
        public SettingsWindow OwnerSettingsWindow { get; set; }


        public static readonly DependencyProperty CurrentEditorConfigurationNameProperty = DependencyProperty.Register(
            "CurrentEditorConfigurationName", typeof (string), typeof (EditorPane),
            new PropertyMetadata(default(string), CurrentEditorConfigurationChangedCallback));

        private static void CurrentEditorConfigurationChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var editorPane = (EditorPane) dependencyObject;

            var newValue = dependencyPropertyChangedEventArgs.NewValue.ToString();

            AppSettings.Settings.CurrentEditorControlConfiguration = newValue;

            editorPane.EditorControlSettings =
                AppSettings.Settings.EditorControlConfigurations[newValue].EditorControlSettings;
        }

        public static readonly DependencyProperty EditorControlSettingsNodeProperty = DependencyProperty.Register(
            "EditorControlSettings", typeof (LSLEditorControlSettings), typeof (EditorPane),
            new PropertyMetadata(default(LSLEditorControlSettings)));

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

            ResetHighlightingColorBox(colorBox);
        }

        private void ResetHighlightingColorBox(UIElement colorBox)
        {
            var bindingExpression = BindingOperations.GetBindingExpression((FrameworkElement) colorBox,
                ColorPicker.SelectedColorProperty);
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


        private void ResetCompletionWindowColor_OnClick(object sender, RoutedEventArgs e)
        {
            var colorBox = ((Border) ((StackPanel) ((FrameworkElement) sender).Parent).Children[1]).Child;

            ResetCompletionBrushColorBox(colorBox);
        }


        private void ResetCompletionBrushColorBox(UIElement colorBox)
        {
            var bindingExpression = BindingOperations.GetBindingExpression((FrameworkElement) colorBox,
                ColorPicker.SelectedColorProperty);
            var propNames = bindingExpression.ParentBinding.Path.Path.Split('.');


            var context = propNames[1];
            if (context == "CompletionWindowItemBrushes")
            {
                var propName = propNames[propNames.Length - 3];

                var deflt =
                    (XmlSolidBrush)
                        DefaultValueInitializer.GetDefaultValue(EditorControlSettings.CompletionWindowItemBrushes,
                            propName);

                var property = EditorControlSettings.CompletionWindowItemBrushes.GetType()
                    .GetProperty(propName);

                property.SetValue(EditorControlSettings.CompletionWindowItemBrushes, deflt);
            }
            else
            {
                var propName = propNames[propNames.Length - 2];
                DefaultValueInitializer.SetToDefault(EditorControlSettings, propName);
            }
        }


        private void ResetAllCompletionWindowColors_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var color in CompletionWindowColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox = ((Border) color.Children[1]).Child;
                ResetCompletionBrushColorBox(colorBox);
            }
        }

        private void ResetAllHighlightingColors_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var color in HighlightingColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox = ((Border) color.Children[1]).Child;
                ResetHighlightingColorBox(colorBox);
            }
        }


        private void NewConfiguration_OnClick(object sender, RoutedEventArgs e)
        {
            var name = new UniqueNamerWindow(AppSettings.Settings.EditorControlConfigurations.Keys, "My Configuration")
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

        [DataContract]
        private class HighlightingSettings
        {
            [DataMember]
            public XmlColor BasicTextColor { get; set; }

            [DataMember]
            public XmlColor BackgroundColor { get; set; }

            [DataMember]
            public LSLHighlightingColors HighlightingColors { get; set; }
        }


        private void ImportHighlightingColors_OnClick(object sender, RoutedEventArgs e)
        {
            DoImportSettingsWindow("Highlighting Colors (*.xml)|*.xml;", ".xml", reader =>
            {
                var x = new DataContractSerializer(typeof (HighlightingSettings));

                var settings = (HighlightingSettings) x.ReadObject(reader);

                EditorControlSettings.ForegroundColor = settings.BasicTextColor;
                EditorControlSettings.BackgroundColor = settings.BackgroundColor;
                EditorControlSettings.HighlightingColors = settings.HighlightingColors;
            });
        }


        private void ExportHighlightingColors_OnClick(object sender, RoutedEventArgs e)
        {
            DoExportSettingsWindow("Highlighting Colors (*.xml)|*.xml;", "LSLCCEditor_HighlightingColors.xml",
                writer =>
                {
                    var x = new DataContractSerializer(typeof (HighlightingSettings));

                    var settings = new HighlightingSettings
                    {
                        BackgroundColor = EditorControlSettings.BackgroundColor,
                        BasicTextColor = EditorControlSettings.ForegroundColor,
                        HighlightingColors = EditorControlSettings.HighlightingColors
                    };

                    x.WriteObject(writer, settings);
                });
        }


        [DataContract]
        private class CompletionWindowColorSettings
        {
            [DataMember]
            public XmlColor CompletionWindowBackgroundColor { get; set; }

            [DataMember]
            public XmlColor CompletionWindowSelectionBackgroundColor { get; set; }

            [DataMember]
            public XmlColor CompletionWindowSelectionBorderColor { get; set; }

            [DataMember]
            public LSLCompletionWindowItemBrushes CompletionWindowItemBrushes { get; set; }
        }

        private void DoImportSettingsWindow(string fileFilter, string fileExt, Action<XmlTextReader> serialize)
        {
            var openDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = fileFilter,
                AddExtension = true,
                DefaultExt = fileExt
            };
            if (!openDialog.ShowDialog(OwnerSettingsWindow).Value)
            {
                return;
            }

            try
            {
                using (var file = new XmlTextReader(openDialog.OpenFile()))
                {
                    serialize(file);
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
                                + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }


        private void DoExportSettingsWindow(string fileFilter, string fileName, Action<XmlTextWriter> serialize)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = fileFilter,
                FileName = fileName
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
                    serialize(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected problem occurred while trying to save the file: "
                                + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }


        private void ImportCompletionWindowColors_OnClick(object sender, RoutedEventArgs e)
        {
            DoImportSettingsWindow("Completion Window Colors (*.xml)|*.xml;", ".xml", reader =>
            {
                var x = new DataContractSerializer(typeof (CompletionWindowColorSettings));

                var settings = (CompletionWindowColorSettings) x.ReadObject(reader);

                EditorControlSettings.CompletionWindowItemBrushes = settings.CompletionWindowItemBrushes;
                EditorControlSettings.CompletionWindowBackgroundColor = settings.CompletionWindowBackgroundColor;

                EditorControlSettings.CompletionWindowSelectionBackgroundColor =
                    settings.CompletionWindowSelectionBackgroundColor;

                EditorControlSettings.CompletionWindowSelectionBorderColor =
                    settings.CompletionWindowSelectionBorderColor;
            });
        }


        private void ExportCompletionWindowColors_OnClick(object sender, RoutedEventArgs e)
        {
            DoExportSettingsWindow("Completion Window Colors (*.xml)|*.xml;", "LSLCCEditor_CompletionWindowColors.xml",
                writer =>
                {
                    var x = new DataContractSerializer(typeof (CompletionWindowColorSettings));

                    var settings = new CompletionWindowColorSettings
                    {
                        CompletionWindowItemBrushes = EditorControlSettings.CompletionWindowItemBrushes,
                        CompletionWindowSelectionBorderColor =
                            EditorControlSettings.CompletionWindowSelectionBorderColor,
                        CompletionWindowBackgroundColor = EditorControlSettings.CompletionWindowBackgroundColor,
                        CompletionWindowSelectionBackgroundColor =
                            EditorControlSettings.CompletionWindowSelectionBackgroundColor
                    };

                    x.WriteObject(writer, settings);
                });
        }


        private void ResetAllToolTipColors_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var color in ToolTipColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox = ((Border) color.Children[1]).Child;
                ResetCompletionBrushColorBox(colorBox);
            }
        }


        [DataContract]
        private class ToolTipColorSettings
        {
            [DataMember]
            public XmlColor BackgroundColor { get; set; }

            [DataMember]
            public XmlColor ForegroundColor { get; set; }

            [DataMember]
            public XmlColor BorderColor { get; set; }
        }


        private void ExportToolTipColors_OnClick(object sender, RoutedEventArgs e)
        {
            DoExportSettingsWindow("Tool Tip Colors (*.xml)|*.xml;", "LSLCCEditor_ToolTipColors.xml",
                writer =>
                {
                    var x = new DataContractSerializer(typeof (ToolTipColorSettings));

                    var settings = new ToolTipColorSettings
                    {
                        BackgroundColor = EditorControlSettings.ToolTipBackground,
                        ForegroundColor = EditorControlSettings.ForegroundColor,
                        BorderColor = EditorControlSettings.BackgroundColor
                    };

                    x.WriteObject(writer, settings);
                });
        }


        private void ImportToolTipColors_OnClick(object sender, RoutedEventArgs e)
        {
            DoImportSettingsWindow("Tool Tip Colors (*.xml)|*.xml;", ".xml", reader =>
            {
                var x = new DataContractSerializer(typeof (ToolTipColorSettings));

                var settings = (ToolTipColorSettings) x.ReadObject(reader);

                EditorControlSettings.ToolTipBackground = settings.BackgroundColor;
                EditorControlSettings.ToolTipForeground = settings.ForegroundColor;
                EditorControlSettings.ToolTipBorderColor = settings.BorderColor;
            });
        }


        private void ToolTipColorReset_OnClick(object sender, RoutedEventArgs e)
        {
            var colorBox = ((Border) ((StackPanel) ((FrameworkElement) sender).Parent).Children[1]).Child;

            ResetToolTipColorBox(colorBox);
        }


        private void ResetToolTipColorBox(UIElement colorBox)
        {
            var bindingExpression = BindingOperations.GetBindingExpression((FrameworkElement) colorBox,
                ColorPicker.SelectedColorProperty);
            var propNames = bindingExpression.ParentBinding.Path.Path.Split('.');

            var propName = propNames[propNames.Length - 2];

            DefaultValueInitializer.SetToDefault(EditorControlSettings, propName);
        }
    }
}