#region FileInfo
// 
// File: EditorTabContent.xaml.cs
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
#region Imports

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.LibraryData;
using LSLCCEditor.Settings;

#endregion

namespace LSLCCEditor.EditorTabUI
{
    /// <summary>
    ///     Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class EditorTabContent : UserControl
    {
        public static readonly DependencyProperty LibraryDataProviderProperty = DependencyProperty.Register(
            "LibraryDataProvider", typeof (ILSLLibraryDataProvider), typeof (EditorTabContent),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty SourceCodeProperty = DependencyProperty.Register("SourceCode",
            typeof (string), typeof (EditorTabContent),
            new FrameworkPropertyMetadata(default(string),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private readonly EditorTab _ownerTab;
        private ObservableCollection<CompilerMessage> _compilerMessages = new ObservableCollection<CompilerMessage>();

        public EditorTabContent(EditorTab owner)
        {
            InitializeComponent();
            _ownerTab = owner;


            Editor.Settings = AppSettings.Settings.EditorControlConfigurations[AppSettings.Settings.CurrentEditorControlConfiguration];
        }

        public ILSLLibraryDataProvider LibraryDataProvider
        {
            get { return (ILSLLibraryDataProvider) GetValue(LibraryDataProviderProperty); }
            set { SetValue(LibraryDataProviderProperty, value); }
        }

        public string SourceCode
        {
            get { return (string) GetValue(SourceCodeProperty); }
            set { SetValue(SourceCodeProperty, value); }
        }

        public ObservableCollection<CompilerMessage> CompilerMessages
        {
            get { return _compilerMessages; }
            set { _compilerMessages = value; }
        }

        private void CompilerMessageItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var message = (CompilerMessage) ((ListViewItem) sender).Content;

            if (!message.Clickable)
            {
                return;
            }


            var location = message.CodeLocation;

            var line = location.LineStart == 0 ? 1 : location.LineStart;

            var lineend = location.LineEnd == 0 ? 1 : location.LineEnd;


            Editor.Editor.ScrollToLine(line);


            if (message.CodeLocation.HasIndexInfo && message.CodeLocation.IsSingleLine)
            {
                Editor.Editor.Select(message.CodeLocation.StartIndex,
                    (message.CodeLocation.StopIndex + 1) - message.CodeLocation.StartIndex);

                return;
            }


            var l = 0;
            for (var i = line; i <= lineend; i++)
            {
                l += Editor.Editor.Document.GetLineByNumber(i).TotalLength;
            }

            var linestart = Editor.Editor.Document.GetLineByNumber(line);


            Editor.Editor.Select(linestart.Offset, l);
        }

        private void Editor_OnTextChanged(object sender, EventArgs e)
        {
            _ownerTab.ChangesPending = true;
        }
    }
}