using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;

namespace LSLCCEditor.EditorTabUI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class EditorTabContent : UserControl
    {
        private readonly EditorTabUI.EditorTab _ownerTab;
        public EditorTabContent(EditorTabUI.EditorTab owner)
        {
            InitializeComponent();
            _ownerTab = owner;
        }


        private ObservableCollection<CompilerMessage> _compilerMessages = new ObservableCollection<CompilerMessage>();


        public static readonly DependencyProperty LibraryDataProviderProperty = DependencyProperty.Register(
            "LibraryDataProvider", typeof(ILSLMainLibraryDataProvider), typeof(EditorTabContent),
            new FrameworkPropertyMetadata(null));

        public ILSLMainLibraryDataProvider LibraryDataProvider
        {
            get { return (ILSLMainLibraryDataProvider)GetValue(LibraryDataProviderProperty); }
            set { SetValue(LibraryDataProviderProperty, value); }
        }

       


        public static readonly DependencyProperty SourceCodeProperty = DependencyProperty.Register("SourceCode", typeof(string), typeof(EditorTabContent),
            new FrameworkPropertyMetadata(default(string),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public string SourceCode
        {
            get { return (string)GetValue(SourceCodeProperty); }
            set { SetValue(SourceCodeProperty, value); }
        }



        public ObservableCollection<CompilerMessage> CompilerMessages
        {
            get { return _compilerMessages; }
            set { _compilerMessages = value; }
        }


        private void CompilerMessageItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var message = (CompilerMessage)((ListViewItem)sender).Content;

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


            int l = 0;
            for (int i = line; i <= lineend; i++)
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
