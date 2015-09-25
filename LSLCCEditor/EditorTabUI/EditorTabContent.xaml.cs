#region FileInfo

// 
// File: EditorTabContent.xaml.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:26 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Imports

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibLSLCC.CodeValidator.Components.Interfaces;

#endregion

namespace LSLCCEditor.EditorTabUI
{
    /// <summary>
    ///     Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class EditorTabContent : UserControl
    {
        public static readonly DependencyProperty LibraryDataProviderProperty = DependencyProperty.Register(
            "LibraryDataProvider", typeof (ILSLMainLibraryDataProvider), typeof (EditorTabContent),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty SourceCodeProperty = DependencyProperty.Register("SourceCode",
            typeof (string), typeof (EditorTabContent),
            new FrameworkPropertyMetadata(default(string),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private readonly EditorTab _ownerTab;

        public EditorTabContent(EditorTab owner)
        {
            InitializeComponent();
            _ownerTab = owner;
        }

        public ILSLMainLibraryDataProvider LibraryDataProvider
        {
            get { return (ILSLMainLibraryDataProvider) GetValue(LibraryDataProviderProperty); }
            set { SetValue(LibraryDataProviderProperty, value); }
        }

        public string SourceCode
        {
            get { return (string) GetValue(SourceCodeProperty); }
            set { SetValue(SourceCodeProperty, value); }
        }

        public ObservableCollection<CompilerMessage> CompilerMessages { get; set; } =
            new ObservableCollection<CompilerMessage>();

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