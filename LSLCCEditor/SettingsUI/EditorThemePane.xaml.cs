﻿#region FileInfo
// 
// File: EditorThemePane.xaml.cs
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
using MessageBox = System.Windows.MessageBox;
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
            SelectedEditorThemeName = AppSettings.Settings.CurrentEditorControlThemeName;
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

            AppSettings.Settings.CurrentEditorControlThemeName = newValue;

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

            if (bindingExpression == null) return;

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

            if (bindingExpression == null) return;

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
            ImportExportTools.DoImportSettingsWindow(OwnerSettingsWindow, "Highlighting Colors (*.xml)|*.xml;", ".xml",
                reader =>
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
            ImportExportTools.DoExportSettingsWindow(OwnerSettingsWindow, "Highlighting Colors (*.xml)|*.xml;",
                "LSLCCEditor_HighlightingColors.xml",
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


        private void ImportCompletionWindowColors_OnClick(object sender, RoutedEventArgs e)
        {
            ImportExportTools.DoImportSettingsWindow(OwnerSettingsWindow, "Completion Window Colors (*.xml)|*.xml;",
                ".xml", reader =>
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
            ImportExportTools.DoExportSettingsWindow(OwnerSettingsWindow, "Completion Window Colors (*.xml)|*.xml;",
                "LSLCCEditor_CompletionWindowColors.xml",
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
            public XmlColor DeprecationMarkerColor { get; set; }

            public XmlColor BackgroundColor { get; set; }


            public XmlColor ForegroundColor { get; set; }


            public XmlColor BorderColor { get; set; }
        }


        private void ExportToolTipColors_OnClick(object sender, RoutedEventArgs e)
        {
            ImportExportTools.DoExportSettingsWindow(OwnerSettingsWindow, "Tool Tip Colors (*.xml)|*.xml;",
                "LSLCCEditor_ToolTipColors.xml",
                writer =>
                {
                    var x = new XmlSerializer(typeof (ToolTipColorSettings));

                    var settings = new ToolTipColorSettings
                    {
                        DeprecationMarkerColor = EditorControlTheme.ToolTipDeprecationMarkerColor,
                        BackgroundColor = EditorControlTheme.ToolTipBackground,
                        ForegroundColor = EditorControlTheme.ForegroundColor,
                        BorderColor = EditorControlTheme.BackgroundColor
                    };

                    x.Serialize(writer, settings);
                });
        }


        private void ImportToolTipColors_OnClick(object sender, RoutedEventArgs e)
        {
            ImportExportTools.DoImportSettingsWindow(OwnerSettingsWindow, "Tool Tip Colors (*.xml)|*.xml;", ".xml",
                reader =>
                {
                    var x = new XmlSerializer(typeof (ToolTipColorSettings));

                    var settings = (ToolTipColorSettings) x.Deserialize(reader);

                    EditorControlTheme.ToolTipDeprecationMarkerColor = settings.DeprecationMarkerColor;
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


        private void ResetSelectionColorBox(UIElement colorBox)
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
            var dialogResult = MessageBox.Show(OwnerSettingsWindow,
                "Are you sure you want to overwrite the currently selected theme by importing one over it?",
                "Overwrite Selected Theme?",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (dialogResult != MessageBoxResult.Yes) return;


            ImportExportTools.DoImportSettingsWindow(OwnerSettingsWindow, "Editor Theme (*.xml)|*.xml;", ".xml",
                reader =>
                {
                    var x = new XmlSerializer(typeof (LSLEditorControlTheme));

                    var settings = (LSLEditorControlTheme) x.Deserialize(reader);

                    EditorControlTheme.MemberwiseAssign(settings);
                });
        }


        private void ResetAll_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var color in ToolTipColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox = ((Border) color.Children[1]).Child;
                ResetCompletionBrushColorBox(colorBox);
            }

            foreach (var color in CompletionWindowColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox = ((Border) color.Children[1]).Child;
                ResetCompletionBrushColorBox(colorBox);
            }

            foreach (var color in HighlightingColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox = ((Border) color.Children[1]).Child;
                ResetHighlightingColorBox(colorBox);
            }

            foreach (var color in SelectionColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox = ((Border) color.Children[1]).Child;
                ResetSelectionColorBox(colorBox);
            }
        }


        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            ImportExportTools.DoExportSettingsWindow(OwnerSettingsWindow, "Editor Theme (*.xml)|*.xml;",
                "LSLCCEditor_EditorTheme.xml",
                writer =>
                {
                    var x = new XmlSerializer(typeof (LSLEditorControlTheme));

                    x.Serialize(writer, EditorControlTheme);
                });
        }


        private void SelectionColorReset_OnClick(object sender, RoutedEventArgs e)
        {
            var colorBox = ((Border) ((StackPanel) ((FrameworkElement) sender).Parent).Children[1]).Child;

            ResetSelectionColorBox(colorBox);
        }


        private void ResetAllSelectionColors_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var color in SelectionColorsListView.Items.Cast<StackPanel>())
            {
                var colorBox = ((Border) color.Children[1]).Child;
                ResetSelectionColorBox(colorBox);
            }
        }


        public class SelectionColorSettings
        {
            public XmlColor SelectionColor { get; set; }


            public XmlColor SelectionForegroundColor { get; set; }


            public XmlColor SelectionBorderColor { get; set; }


            public XmlColor SymbolSelectionColor { get; set; }


            public XmlColor SymbolSelectionForegroundColor { get; set; }


            public XmlColor SymbolSelectionBorderColor { get; set; }
        }


        private void ExportSelectionColors_OnClick(object sender, RoutedEventArgs e)
        {
            ImportExportTools.DoExportSettingsWindow(OwnerSettingsWindow, "Selection Colors (*.xml)|*.xml;",
                "LSLCCEditor_ToolTipColors.xml",
                writer =>
                {
                    var x = new XmlSerializer(typeof (SelectionColorSettings));

                    var settings = new SelectionColorSettings
                    {
                        SymbolSelectionForegroundColor = EditorControlTheme.SymbolSelectionForegroundColor,
                        SymbolSelectionColor = EditorControlTheme.SymbolSelectionColor,
                        SymbolSelectionBorderColor = EditorControlTheme.SymbolSelectionBorderColor,
                        SelectionColor = EditorControlTheme.SelectionColor,
                        SelectionBorderColor = EditorControlTheme.SelectionBorderColor,
                        SelectionForegroundColor = EditorControlTheme.SelectionForegroundColor
                    };

                    x.Serialize(writer, settings);
                });
        }


        private void ImportSelectionColors_OnClick(object sender, RoutedEventArgs e)
        {
            ImportExportTools.DoImportSettingsWindow(OwnerSettingsWindow, "Selection Colors (*.xml)|*.xml;", ".xml",
                reader =>
                {
                    var x = new XmlSerializer(typeof (SelectionColorSettings));

                    var settings = (SelectionColorSettings) x.Deserialize(reader);

                    EditorControlTheme.SymbolSelectionForegroundColor = settings.SymbolSelectionForegroundColor;
                    EditorControlTheme.SymbolSelectionColor = settings.SymbolSelectionColor;
                    EditorControlTheme.SymbolSelectionBorderColor = settings.SymbolSelectionBorderColor;

                    EditorControlTheme.SelectionColor = settings.SelectionColor;
                    EditorControlTheme.SelectionBorderColor = settings.SymbolSelectionBorderColor;
                    EditorControlTheme.SelectionForegroundColor = settings.SelectionForegroundColor;
                });
        }
    }
}