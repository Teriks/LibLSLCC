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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using LibLSLCC.Compilers;
using LibLSLCC.Compilers.OpenSim;
using LibLSLCC.CSharp;
using LSLCCEditor.Settings;
using LSLCCEditor.Utility.Validation;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for CompilerPane.xaml
    /// </summary>
    public partial class CompilerPane : UserControl, ISettingsPane
    {
        public ObservableCollection<string> CompilerConfigurations { get; set; }


        public static readonly DependencyProperty SelectedCompilerConfigurationProperty = DependencyProperty.Register(
            "SelectedCompilerConfiguration", typeof (string), typeof (CompilerPane),
            new PropertyMetadata(default(string), SelectedCompilerConfigurationChanged));

        private static void SelectedCompilerConfigurationChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var pane = ((CompilerPane) dependencyObject);


            pane.CurrentCompilerConfiguration =
                AppSettings.Settings.CompilerConfigurations[dependencyPropertyChangedEventArgs.NewValue.ToString()]
                    .Clone();


            pane.ConstructorAccessibilityLevel =
                pane.CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedConstructorAccessibility;




            var className = pane.CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedClassName;
            if (className != null)
            {
                pane.GeneratedClassName = className.ToString();
            }
            else
            {
                pane.GeneratedClassName = "LSLScript";
            }


            var classNamespace = pane.CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedClassNamespace;

            if (classNamespace != null)
            {
                pane.GeneratedClassNamespace = classNamespace.ToString();
            }
            else
            {
                pane.GeneratedClassNamespace = "";
            }

        }




        public static readonly DependencyProperty CurrentCompilerConfigurationProperty = DependencyProperty.Register(
            "CurrentCompilerConfiguration", typeof (CompilerSettingsNode), typeof (CompilerPane),
            new PropertyMetadata(default(CompilerSettingsNode)));


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

            SelectedCompilerConfiguration = CompilerConfigurations.First();

            this.Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Owner.MinWidth = Owner.ActualWidth+50;
            Owner.MinHeight = Owner.ActualHeight+50;
        }


        public static readonly DependencyProperty ConstructorAccessibility = DependencyProperty.Register(
            "ConstructorAccessibility", typeof (MemberAccessibilityLevel), typeof (CompilerPane),
            new PropertyMetadata(default(MemberAccessibilityLevel)));

        public MemberAccessibilityLevel ConstructorAccessibilityLevel
        {
            get { return (MemberAccessibilityLevel) GetValue(ConstructorAccessibility); }
            set { SetValue(ConstructorAccessibility, value); }
        }


        public AppSettingsNode ApplicationSettings
        {
            get { return AppSettings.Settings; }
        }

        public int Priority
        {
            get { return 1; }
        }

        public string Title { get; private set; }
        public SettingsWindow Owner { get; set; }


        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CoOpTerminationOptionsExpander.IsValid() && ClassOptionsExpander.IsValid())
            {
                AppSettings.Settings.CompilerConfigurations[SelectedCompilerConfiguration] =
                    CurrentCompilerConfiguration;
                AppSettings.Save();
            }
            else
            {
                MessageBox.Show("Could not save configuration as some settings were invalid.",
                    "Invalid Settings Detected", MessageBoxButton.OK,MessageBoxImage.Error);
            }

        }

        private void RevertButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentCompilerConfiguration =
                AppSettings.Settings.CompilerConfigurations[SelectedCompilerConfiguration].Clone();
        }



        private void NamespaceImportAdd_OnClick(object sender, RoutedEventArgs e)
        {
            if (NameSpaceAddTextbox.IsValid())
            {
                CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedNamespaceImports.Add(
                    new CSharpNamespace(NamespaceImportAddText));
            }
        }

        private void NamespaceListboxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                object[] selected = new object [NamespaceNameListBox.SelectedItems.Count];

                NamespaceNameListBox.SelectedItems.CopyTo(selected,0);

                foreach (var item in selected)
                {
                    CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedNamespaceImports.Remove(
                        (CSharpNamespace) item);
                }
            }
        }

        private void NamespaceListBoxContextMenuDelete_OnClick(object sender, RoutedEventArgs e)
        {
            object[] selected = new object[NamespaceNameListBox.SelectedItems.Count];

            NamespaceNameListBox.SelectedItems.CopyTo(selected, 0);

            foreach (var item in selected)
            {
                CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedNamespaceImports.Remove(
                    (CSharpNamespace)item);
            }
        }



        public static readonly DependencyProperty GeneratedClassNamespaceProperty = DependencyProperty.Register(
            "GeneratedClassNamespace", typeof (string), typeof (CompilerPane), new PropertyMetadata(default(string), GeneratedNamespaceNamePropertyChanged));

        private static void GeneratedNamespaceNamePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var pane = (CompilerPane) dependencyObject;
            var nVal = (string) dependencyPropertyChangedEventArgs.NewValue;

            if (string.IsNullOrWhiteSpace(nVal))
            {
                pane.CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedClassNamespace = null;
                return;
            }

            var c = CSharpNamespaceNameValidator.Validate(nVal);
            if (c.Success)
            {
                pane.CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedClassNamespace =
                    new CSharpNamespace((string) dependencyPropertyChangedEventArgs.NewValue);
            }
            else
            {
                throw new Exception(c.ErrorDescription);
            }
        }




        public string GeneratedClassNamespace
        {
            get { return (string) GetValue(GeneratedClassNamespaceProperty); }
            set
            {
                SetValue(GeneratedClassNamespaceProperty, value);
            }
        }



        public static readonly DependencyProperty GeneratedClassNameProperty = DependencyProperty.Register(
            "GeneratedClassName", typeof (string), typeof (CompilerPane), new PropertyMetadata(default(string), GeneratedClassNameChanged));

        private static void GeneratedClassNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var pane = (CompilerPane)dependencyObject;
            var nVal = (string)dependencyPropertyChangedEventArgs.NewValue;

            if (string.IsNullOrWhiteSpace(nVal))
            {
                throw new Exception("must provide a class name.");
            }

            var c = CSharpClassNameValidator.Validate(nVal);
            if (c.Success)
            {
                pane.CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedClassName =
                    new CSharpClassName((string)dependencyPropertyChangedEventArgs.NewValue);
            }
            else
            {
                throw new Exception(c.ErrorDescription);
            }
        }

        public string GeneratedClassName
        {
            get { return (string) GetValue(GeneratedClassNameProperty); }
            set { SetValue(GeneratedClassNameProperty, value); }
        }




        public static readonly DependencyProperty NamespaceImportAddTextProperty = DependencyProperty.Register(
            "NamespaceImportAddText", typeof (string), typeof (CompilerPane), new PropertyMetadata(default(string), NamespaceImportAddtext));

        private static void NamespaceImportAddtext(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var nVal = (string)dependencyPropertyChangedEventArgs.NewValue;

            if (string.IsNullOrWhiteSpace(nVal))
            {
                throw new Exception("must provide a namespace name.");
            }

            var c = CSharpNamespaceNameValidator.Validate(nVal);
            if (!c.Success)
            {
                throw new Exception(c.ErrorDescription);
            }
        }

        public string NamespaceImportAddText
        {
            get { return (string) GetValue(NamespaceImportAddTextProperty); }
            set { SetValue(NamespaceImportAddTextProperty, value); }
        }
    }





}