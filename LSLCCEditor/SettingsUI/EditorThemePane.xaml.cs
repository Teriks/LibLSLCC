using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using LibLSLCC.Settings;
using LSLCCEditor.EditControl;
using LSLCCEditor.Settings;
using LSLCCEditor.Utility.Xml;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using UserControl = System.Windows.Controls.UserControl;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for EditorThemePane.xaml
    /// </summary>
    public partial class EditorThemePane : UserControl, ISettingsPane
    {
        public EditorThemePane()
        {
            InitializeComponent();

            Title = "Editor Theme";
            SelectedEditorThemeName = AppSettings.Settings.CurrentEditorControlTheme;
            EditorThemeNames =
                new ObservableCollection<string>(AppSettings.Settings.EditorControlThemes.Keys);
        }


        public static readonly DependencyProperty EditorThemeNamesProperty = DependencyProperty.Register(
            "EditorThemeNames", typeof (ObservableCollection<string>), typeof (EditorThemePane),
            new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> EditorThemeNames
        {
            get { return (ObservableCollection<string>) GetValue(EditorThemeNamesProperty); }
            set { SetValue(EditorThemeNamesProperty, value); }
        }



        public string Title { get; private set; }
        public SettingsWindow OwnerSettingsWindow { get; set; }



        public static readonly DependencyProperty SelectedEditorThemeNameProperty = DependencyProperty.Register(
            "SelectedEditorThemeName", typeof (string), typeof (EditorThemePane),
            new PropertyMetadata(default(string), SelectedEditorThemeChangedCallback));



        private static void SelectedEditorThemeChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var editorPane = (EditorThemePane) dependencyObject;

            var newValue = dependencyPropertyChangedEventArgs.NewValue.ToString();

            AppSettings.Settings.CurrentEditorControlTheme = newValue;

            editorPane.EditorControlTheme =
                AppSettings.Settings.EditorControlThemes[newValue].Theme;
        }


        public static readonly DependencyProperty EditorControlThemeProperty = DependencyProperty.Register(
            "EditorControlTheme", typeof (LSLEditorControlTheme), typeof (EditorThemePane),
            new PropertyMetadata(default(LSLEditorControlTheme)));



        public LSLEditorControlTheme EditorControlTheme
        {
            get { return (LSLEditorControlTheme) GetValue(EditorControlThemeProperty); }
            set { SetValue(EditorControlThemeProperty, value); }
        }


        public string SelectedEditorThemeName
        {
            get { return (string) GetValue(SelectedEditorThemeNameProperty); }
            set { SetValue(SelectedEditorThemeNameProperty, value); }
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
                DefaultValueInitializer.SetToDefault(EditorControlTheme.HighlightingColors, propName);
            }
            else
            {
                DefaultValueInitializer.SetToDefault(EditorControlTheme, propName);
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
                        DefaultValueInitializer.GetDefaultValue(EditorControlTheme.CompletionWindowItemBrushes,
                            propName);

                var property = EditorControlTheme.CompletionWindowItemBrushes.GetType()
                    .GetProperty(propName);

                property.SetValue(EditorControlTheme.CompletionWindowItemBrushes, deflt);
            }
            else
            {
                var propName = propNames[propNames.Length - 2];
                DefaultValueInitializer.SetToDefault(EditorControlTheme, propName);
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
            var name = new UniqueNamerWindow(AppSettings.Settings.EditorControlThemes.Keys, "My Theme")
            {
                Owner = OwnerSettingsWindow
            };
            name.ShowDialog();

            if (name.Canceled) return;


            AppSettings.Settings.AddEditorControlTheme(name.ChosenName);

            EditorThemeNames.Add(name.ChosenName);

            EditorThemeCombobox.SelectedIndex = EditorThemeNames.Count - 1;
        }


        private void DeleteConfiguration_OnClick(object sender, RoutedEventArgs e)
        {
            var currentlySelected = SelectedEditorThemeName;

            int newIndex = 0;

            if ((EditorThemeNames.Count - 1) > 0 && EditorThemeCombobox.SelectedIndex > 0)
            {
                newIndex = EditorThemeCombobox.SelectedIndex - 1;
            }


            AppSettings.Settings.RemoveEditorControlTheme(currentlySelected, EditorThemeNames[newIndex]);

            EditorThemeNames.Remove(SelectedEditorThemeName);

            EditorThemeCombobox.SelectedIndex = newIndex;
        }


        
        public class HighlightingSettings
        {
            
            public XmlColor BasicTextColor { get; set; }

            
            public XmlColor BackgroundColor { get; set; }

            
            public LSLHighlightingColors HighlightingColors { get; set; }
        }



        private void ImportHighlightingColors_OnClick(object sender, RoutedEventArgs e)
        {
            DoImportSettingsWindow("Highlighting Colors (*.xml)|*.xml;", ".xml", reader =>
            {
                var x = new XmlSerializer(typeof (HighlightingSettings));

                var settings = (HighlightingSettings) x.Deserialize(reader);

                EditorControlTheme.ForegroundColor = settings.BasicTextColor;
                EditorControlTheme.BackgroundColor = settings.BackgroundColor;
                EditorControlTheme.HighlightingColors = settings.HighlightingColors;
            });
        }


        private void ExportHighlightingColors_OnClick(object sender, RoutedEventArgs e)
        {
            DoExportSettingsWindow("Highlighting Colors (*.xml)|*.xml;", "LSLCCEditor_HighlightingColors.xml",
                writer =>
                {
                    var x = new XmlSerializer(typeof (HighlightingSettings));

                    var settings = new HighlightingSettings
                    {
                        BackgroundColor = EditorControlTheme.BackgroundColor,
                        BasicTextColor = EditorControlTheme.ForegroundColor,
                        HighlightingColors = EditorControlTheme.HighlightingColors
                    };

                    x.Serialize(writer, settings);
                });
        }


        public class CompletionWindowColorSettings
        {
            
            public XmlColor CompletionWindowBackgroundColor { get; set; }

            
            public XmlColor CompletionWindowSelectionBackgroundColor { get; set; }

            
            public XmlColor CompletionWindowSelectionBorderColor { get; set; }

            
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
                var x = new XmlSerializer(typeof (CompletionWindowColorSettings));

                var settings = (CompletionWindowColorSettings) x.Deserialize(reader);

                EditorControlTheme.CompletionWindowItemBrushes = settings.CompletionWindowItemBrushes;
                EditorControlTheme.CompletionWindowBackgroundColor = settings.CompletionWindowBackgroundColor;

                EditorControlTheme.CompletionWindowSelectionBackgroundColor =
                    settings.CompletionWindowSelectionBackgroundColor;

                EditorControlTheme.CompletionWindowSelectionBorderColor =
                    settings.CompletionWindowSelectionBorderColor;
            });
        }


        private void ExportCompletionWindowColors_OnClick(object sender, RoutedEventArgs e)
        {
            DoExportSettingsWindow("Completion Window Colors (*.xml)|*.xml;", "LSLCCEditor_CompletionWindowColors.xml",
                writer =>
                {
                    var x = new XmlSerializer(typeof (CompletionWindowColorSettings));

                    var settings = new CompletionWindowColorSettings
                    {
                        CompletionWindowItemBrushes = EditorControlTheme.CompletionWindowItemBrushes,
                        CompletionWindowSelectionBorderColor =
                            EditorControlTheme.CompletionWindowSelectionBorderColor,
                        CompletionWindowBackgroundColor = EditorControlTheme.CompletionWindowBackgroundColor,
                        CompletionWindowSelectionBackgroundColor =
                            EditorControlTheme.CompletionWindowSelectionBackgroundColor
                    };

                    x.Serialize(writer, settings);
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



        
        public class ToolTipColorSettings
        {
            
            public XmlColor BackgroundColor { get; set; }

            
            public XmlColor ForegroundColor { get; set; }

            
            public XmlColor BorderColor { get; set; }
        }


        private void ExportToolTipColors_OnClick(object sender, RoutedEventArgs e)
        {
            DoExportSettingsWindow("Tool Tip Colors (*.xml)|*.xml;", "LSLCCEditor_ToolTipColors.xml",
                writer =>
                {
                    var x = new XmlSerializer(typeof (ToolTipColorSettings));

                    var settings = new ToolTipColorSettings
                    {
                        BackgroundColor = EditorControlTheme.ToolTipBackground,
                        ForegroundColor = EditorControlTheme.ForegroundColor,
                        BorderColor = EditorControlTheme.BackgroundColor
                    };

                    x.Serialize(writer, settings);
                });
        }


        private void ImportToolTipColors_OnClick(object sender, RoutedEventArgs e)
        {
            DoImportSettingsWindow("Tool Tip Colors (*.xml)|*.xml;", ".xml", reader =>
            {
                var x = new XmlSerializer(typeof (ToolTipColorSettings));

                var settings = (ToolTipColorSettings) x.Deserialize(reader);

                EditorControlTheme.ToolTipBackground = settings.BackgroundColor;
                EditorControlTheme.ToolTipForeground = settings.ForegroundColor;
                EditorControlTheme.ToolTipBorderColor = settings.BorderColor;
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

            DefaultValueInitializer.SetToDefault(EditorControlTheme, propName);
        }


        private void RenameConfiguration_OnClick(object sender, RoutedEventArgs e)
        {
            var x = new UniqueNamerWindow(AppSettings.Settings.EditorControlThemes.Keys, SelectedEditorThemeName,
                false);

            x.ShowDialog();

            if (x.Canceled) return;

            AppSettings.Settings.RenameEditorControlTheme(SelectedEditorThemeName, x.ChosenName);


            EditorThemeNames.Clear();

            foreach (var v in AppSettings.Settings.EditorControlThemes.Keys)
            {
                EditorThemeNames.Add(v);
            }

            SelectedEditorThemeName = x.ChosenName;
        }


        private void Import_OnClick(object sender, RoutedEventArgs e)
        {
            var dialogResult = MessageBox.Show(
                "Are you sure you want to overwrite the currently selected theme by importing one over it?","Overwrite Selected Theme?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes) return;


            DoImportSettingsWindow("Editor Theme (*.xml)|*.xml;", ".xml", reader =>
            {
                var x = new XmlSerializer(typeof(LSLEditorControlTheme));

                var settings = (LSLEditorControlTheme)x.Deserialize(reader);

                EditorControlTheme.MemberwiseAssign(settings);
                AppSettings.Settings.EditorControlThemes[AppSettings.Settings.CurrentEditorControlTheme].Theme.MemberwiseAssign(settings);
            });
        }


        private void ResetAll_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var color in ToolTipColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox = ((Border)color.Children[1]).Child;
                ResetCompletionBrushColorBox(colorBox);
            }

            foreach (var color in CompletionWindowColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox = ((Border)color.Children[1]).Child;
                ResetCompletionBrushColorBox(colorBox);
            }

            foreach (var color in HighlightingColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox = ((Border)color.Children[1]).Child;
                ResetHighlightingColorBox(colorBox);
            }
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            DoExportSettingsWindow("Editor Theme (*.xml)|*.xml;", "LSLCCEditor_EditorTheme.xml",
            writer =>
            {
                var x = new XmlSerializer(typeof(LSLEditorControlTheme));

                x.Serialize(writer, EditorControlTheme);
            });
        }
    }
}