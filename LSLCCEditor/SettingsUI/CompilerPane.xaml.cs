#region FileInfo

// 
// File: CompilerPane.xaml.cs
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
using System.Windows;
using System.Windows.Forms;
using LibLSLCC.CSharp;
using LibLSLCC.Settings;
using LSLCCEditor.Settings;
using LSLCCEditor.Utility.Validation;
using MessageBox = System.Windows.Forms.MessageBox;
using ThicknessConverter = Xceed.Wpf.DataGrid.Converters.ThicknessConverter;
using UserControl = System.Windows.Controls.UserControl;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for CompilerPane.xaml
    /// </summary>
    public partial class CompilerPane : UserControl, ISettingsPane
    {
        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register(
            "PropertyType", typeof (ObservableCollection<string>), typeof (CompilerPane), new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> CompilerConfigurationNames
        {
            get { return (ObservableCollection<string>) GetValue(PropertyTypeProperty); }
            set { SetValue(PropertyTypeProperty, value); }
        }


        public static readonly DependencyProperty SelectedCompilerConfigurationNameProperty = DependencyProperty
            .Register(
                "SelectedCompilerConfigurationName", typeof (string), typeof (CompilerPane),
                new PropertyMetadata(default(string), SelectedCompilerConfigurationNameChanged));

        private static void SelectedCompilerConfigurationNameChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var pane = dependencyObject as CompilerPane;

            var newValue = dependencyPropertyChangedEventArgs.NewValue as string;
            var oldValue = dependencyPropertyChangedEventArgs.OldValue as string;

            if (oldValue != null)
            {
                pane.CurrentCompilerConfiguration.UnSubscribePropertyChangedAll(pane);
            }


            if (pane == null || newValue == null) return;


            pane.CurrentConfigIsEdited = false;
            pane.CurrentCompilerConfiguration = AppSettings.Settings.CompilerConfigurations[newValue].Clone();

            var compilerConfig = pane.CurrentCompilerConfiguration.OpenSimCompilerSettings;


            pane.GeneratedClassName = compilerConfig.GeneratedClassName != null ? compilerConfig.GeneratedClassName.FullSignature : "LSLScript";

            pane.GeneratedClassNamespace = compilerConfig.GeneratedClassNamespace != null ? compilerConfig.GeneratedClassNamespace.Name : "";


            pane.CurrentCompilerConfiguration.SubscribePropertyChangedAll(pane,
                AnyCurrentCompilerConfigSubPropertyChanged);

            var bindingExpression = pane.GetBindingExpression(GeneratedClassNameProperty);
            if (bindingExpression != null)
                bindingExpression.UpdateSource();
        }

        private static void AnyCurrentCompilerConfigSubPropertyChanged(
            SettingsPropertyChangedEventArgs<object> settingsPropertyChangedEventArgs)
        {
            var me = settingsPropertyChangedEventArgs.Subscriber as CompilerPane;


            if (AppSettings.Settings.CompilerConfigurations.ContainsKey(me.SelectedCompilerConfigurationName))
            {
                var settingsConfig = AppSettings.Settings.CompilerConfigurations[me.SelectedCompilerConfigurationName];

                me.CurrentConfigIsEdited = !me.CurrentCompilerConfiguration.Equals(settingsConfig);
            }
        }


        public static readonly DependencyProperty CurrentConfigIsEditedProperty = DependencyProperty.Register(
            "CurrentConfigIsEdited", typeof (bool), typeof (CompilerPane), new PropertyMetadata(default(bool)));

        public bool CurrentConfigIsEdited
        {
            get { return (bool) GetValue(CurrentConfigIsEditedProperty); }
            set { SetValue(CurrentConfigIsEditedProperty, value); }
        }

        public string SelectedCompilerConfigurationName
        {
            get { return (string) GetValue(SelectedCompilerConfigurationNameProperty); }
            set { SetValue(SelectedCompilerConfigurationNameProperty, value); }
        }


        public static readonly DependencyProperty CurrentCompilerConfigurationProperty = DependencyProperty.Register(
            "CurrentCompilerConfiguration", typeof (CompilerConfigurationNode), typeof (CompilerPane),
            new PropertyMetadata(default(CompilerConfigurationNode)));

        public CompilerConfigurationNode CurrentCompilerConfiguration
        {
            get { return (CompilerConfigurationNode) GetValue(CurrentCompilerConfigurationProperty); }
            set { SetValue(CurrentCompilerConfigurationProperty, value); }
        }


        public static readonly DependencyProperty GeneratedClassNamespaceProperty = DependencyProperty.Register(
            "GeneratedClassNamespace", typeof (string), typeof (CompilerPane),
            new PropertyMetadata(default(string), GeneratedClassNamespaceChanged));

        private static void GeneratedClassNamespaceChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var pane = dependencyObject as CompilerPane;

            var newValue = dependencyPropertyChangedEventArgs.NewValue as string;

            if (pane != null && !string.IsNullOrWhiteSpace(newValue))
            {

                var validation = CSharpNamespaceNameValidator.Validate(newValue);
                if (!validation.Success)
                {
                    throw new Exception(validation.ErrorDescription);
                }

                pane.CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedClassNamespace =
                    new CSharpNamespace(newValue);
            }
        }

        public string GeneratedClassNamespace
        {
            get { return (string) GetValue(GeneratedClassNamespaceProperty); }
            set { SetValue(GeneratedClassNamespaceProperty, value); }
        }


        public static readonly DependencyProperty GeneratedClassNameProperty = DependencyProperty.Register(
            "GeneratedClassName", typeof (string), typeof (CompilerPane),
            new PropertyMetadata("LSLScript", GeneratedClassNameChanged));

        private static void GeneratedClassNameChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var pane = dependencyObject as CompilerPane;

            var newValue = dependencyPropertyChangedEventArgs.NewValue as string;


            if (pane != null && !string.IsNullOrWhiteSpace(newValue))
            {

                var validation = CSharpClassNameValidator.ValidateDeclaration(newValue);
                if (!validation.Success)
                {
                    throw new Exception(validation.ErrorDescription);
                }

                pane.CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedClassName =
                    new CSharpClassName(newValue);
            }
        }


        public string GeneratedClassName
        {
            get { return (string) GetValue(GeneratedClassNameProperty); }
            set { SetValue(GeneratedClassNameProperty, value); }
        }


        public CompilerPane()
        {
            CompilerConfigurationNames =
                new ObservableCollection<string>(AppSettings.Settings.CompilerConfigurations.Keys);


            InitializeComponent();

            
            Title = "Compiler Settings";
            

            if (CompilerConfigurationNames.Any())
            {
                SelectedCompilerConfigurationName = CompilerConfigurationNames.First();
            }

            
        }

        public string Title { get; private set; }
        public SettingsWindow OwnerSettingsWindow { get; set; }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            var currentlySelected = SelectedCompilerConfigurationName;

            int newIndex = 0;

            if ((CompilerConfigurationNames.Count-1) > 0)
            {
                newIndex = CompilerConfigurationCombobox.SelectedIndex - 1;
            }

            

            AppSettings.Settings.RemoveCompilerConfiguration(currentlySelected, CompilerConfigurationNames[newIndex]);

            CompilerConfigurationNames.Remove(SelectedCompilerConfigurationName);

            CompilerConfigurationCombobox.SelectedIndex = newIndex;
        }

        private void Revert_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentCompilerConfiguration =
                AppSettings.Settings.CompilerConfigurations[SelectedCompilerConfigurationName];
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.IsValid())
            {


                AppSettings.Settings.CompilerConfigurations[SelectedCompilerConfigurationName] =
                    CurrentCompilerConfiguration;

                this.CurrentConfigIsEdited = false;
            }
            else
            {
                MessageBox.Show(
                    "Could not save due to invalid settings, please correct them before attempting to save.", 
                    "Invalid Settings Detected",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void New_OnClick(object sender, RoutedEventArgs e)
        {
            var name = new UniqueNamerWindow(AppSettings.Settings.CompilerConfigurations.Keys, "My Configuration")
            {
                Owner = OwnerSettingsWindow
            };
            name.ShowDialog();

            if (name.Canceled) return;

  
            AppSettings.Settings.AddCompilerConfiguration(name.ChosenName);

            CompilerConfigurationNames.Add(name.ChosenName);

            CompilerConfigurationCombobox.SelectedIndex = CompilerConfigurationNames.Count-1;
        }

        private void GenerateClass_OnUnchecked(object sender, RoutedEventArgs e)
        {

                ClassOptionsExpander.IsExpanded = false;
                ClassOptionsExpander.IsEnabled = false;
                ImportsExpander.IsExpanded = false;
                ImportsExpander.IsEnabled = false;
            
        }

        private void GenerateClass_OnChecked(object sender, RoutedEventArgs e)
        {

                ClassOptionsExpander.IsEnabled = true;
                ImportsExpander.IsEnabled = true;
 
        }

        private void InsertCoOpTerminationCalls_Checked(object sender, RoutedEventArgs e)
        {
            CoOpTerminationOptionsExpander.IsEnabled = true;
        }

        private void InsertCoOpTerminationCalls_Unchecked(object sender, RoutedEventArgs e)
        {
            CoOpTerminationOptionsExpander.IsEnabled = false;
        }



        public static readonly DependencyProperty NamespaceImportAddTextProperty = DependencyProperty.Register(
            "NamespaceImportAddText", typeof (string), typeof (CompilerPane), new PropertyMetadata(default(string), NamespaceImportAddTextChangedCallback));

        private static void NamespaceImportAddTextChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var pane = (CompilerPane)dependencyObject;


            var val = dependencyPropertyChangedEventArgs.NewValue as string;

            if (val == null) return;

            var validation=CSharpNamespaceNameValidator.Validate(val);
            if (!validation.Success)
            {
                throw new Exception(validation.ErrorDescription);
            }
        }

        public string NamespaceImportAddText
        {
            get { return (string) GetValue(NamespaceImportAddTextProperty); }
            set { SetValue(NamespaceImportAddTextProperty, value); }
        }
    }
}