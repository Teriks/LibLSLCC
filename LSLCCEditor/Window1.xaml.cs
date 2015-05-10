using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using FindReplace;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Exceptions;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.Compilers;
using LibLSLCC.Formatter.Visitor;
using LSLCCEditor.EditorTabUI;
using Microsoft.Win32;

// ReSharper disable LocalizableElement

namespace LSLCCEditor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private ObservableCollection<EditorTab> _tabs = new ObservableCollection<EditorTab>();

        private readonly LSLCustomValidatorServiceProvider _validatorServices;


        private class WindowSyntaxWarningListener : LSLDefaultSyntaxWarningListener
        {
            private readonly Window1 _parent;



            public WindowSyntaxWarningListener(Window1 parent)
            {
                _parent = parent;
            }



            public override void OnWarning(LSLSourceCodeRange location, string message)
            {
                var tab = (EditorTab) _parent.TabControl.SelectedItem;
                tab.CompilerMessages.Add(new CompilerMessage("Warning", location, message));
            }
        }

        private class WindowSyntaxErrorListener : LSLDefaultSyntaxErrorListener
        {
            private readonly Window1 _parent;



            public WindowSyntaxErrorListener(Window1 parent)
            {
                _parent = parent;
            }



            public override void OnError(LSLSourceCodeRange location, string message)
            {
                var tab = (EditorTab) _parent.TabControl.SelectedItem;
                tab.CompilerMessages.Add(new CompilerMessage("Error", location, message));
            }
        }


        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            string details = "";
            Exception i = e.Exception;

            while (i != null)
            {
                details += i.Message + "\n\n";
                i = i.InnerException;
            }

            MessageBox.Show("An unexpected error has occurred.  The progam will need to exit.\n" +
                            "Error details:\n\n" + details,
                "Unexpected error", MessageBoxButton.OK);

            Application.Current.Shutdown();


        }

        public Window1()
        {
            Application.Current.DispatcherUnhandledException += App_DispatcherUnhandledException;

            InitializeComponent();

            _validatorServices = new LSLCustomValidatorServiceProvider
            {
                ExpressionValidator = new LSLDefaultExpressionValidator(),
                StringLiteralPreProcessor = new LSLDefaultStringPreProcessor(),
                SyntaxErrorListener = new WindowSyntaxErrorListener(this),
                SyntaxWarningListener = new WindowSyntaxWarningListener(this)
            };
        }



        private EditorTab CreateEditorTab()
        {
            var t = new EditorTab(TabControl, EditorTabs);

            t.Content = new EditorTabContent(t);

            return t;
        }



        public ObservableCollection<EditorTab> EditorTabs
        {
            get { return _tabs; }
            set { _tabs = value; }
        }



        private void CompileForOpenSim_OnClick(object sender, RoutedEventArgs e)
        {
            if (TabControl.SelectedItem == null) return;

            var tab = (EditorTab) TabControl.SelectedItem;

            var suggestedFileName = "LSLScript.cs";


            if (!string.IsNullOrWhiteSpace(tab.FilePath))
            {
                suggestedFileName = Path.GetFileName(tab.FilePath) + ".cs";
            }


            var saveDialog = new SaveFileDialog
            {
                FileName = suggestedFileName,
                DefaultExt = ".cs",
                Filter = "CSharp Code (*.cs) | *.cs"
            };

            try
            {
                var showDialog = saveDialog.ShowDialog();
                if (showDialog != null && showDialog.Value)
                {
                    if (!tab.MemoryOnly)
                    {
                        try
                        {
                            tab.SaveTabToFile();
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.Message, "Could Not Save Before Compiling",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                    }
                    CompileCurrentEditorText(saveDialog.FileName);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Could Not Compile",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }



        private void NewFile_OnClick(object sender, RoutedEventArgs e)
        {
            EditorTabs.Add(CreateEditorTab());
            TabControl.SelectedIndex = (EditorTabs.Count - 1);
        }



        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null)
            {
                return;
            }

            tab.SaveTabToFileInteractive();
        }



        private void CompileCurrentEditorText(string destinationFile)
        {
            if (TabControl.SelectedItem == null) return;

            var tab = (EditorTab) TabControl.SelectedItem;

            tab.CompilerMessages.Clear();

            var validated = ValidateCurrentEditorText();

            if (validated == null)
            {
                return;
            }


            if (File.Exists(destinationFile))
            {
                File.Delete(destinationFile);
            }

            using (var outfile = File.OpenWrite(destinationFile))
            {
                var compiler = new LSLOpenSimCSCompiler(LSLOpenSimCSCompilerSettings
                    .OpenSimClientUploadable(_validatorServices.MainLibraryDataProvider));
                try
                {
                    try
                    {
                        compiler.Compile(validated, new StreamWriter(outfile, Encoding.UTF8));
                    }
                    catch (LSLCompilerInternalException error)
                    {
                        MessageBox.Show("Please report this message with the code that caused it: " + error.Message,
                            "Internal Compiler Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        throw;
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("Please report this message with the code that caused it: " + error.Message,
                        "Unknown Compiler Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw;
                }
            }

            tab.CompilerMessages.Add(new CompilerMessage("Notice", "Program compiled successfully"));
        }



        private void SaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null)
            {
                return;
            }


            tab.SaveTabToNewFileInteractive();
        }



        private ILSLCompilationUnitNode ValidateCurrentEditorText()
        {
            if (TabControl.SelectedItem == null) return null;

            var tab = (EditorTab) TabControl.SelectedItem;

            if (tab.SourceCode == null) tab.SourceCode = "";

            _validatorServices.MainLibraryDataProvider = tab.LibraryDataProvider;


            var validator = new LSLCodeValidator(_validatorServices);

            ILSLCompilationUnitNode validated;


            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(tab.SourceCode));


            try
            {
                try
                {
                    using (var infile = new StreamReader(memoryStream, Encoding.UTF8))
                    {
                        validated = validator.Validate(infile);
                    }
                }
                catch (LSLCodeValidatorInternalError error)
                {
                    MessageBox.Show("Please report this message with the code that caused it: " + error.Message,
                        "Internal Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    validated = null;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Please report this message with the code that caused it: " + error.Message,
                    "Unknown Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                validated = null;
            }
            finally
            {
                memoryStream.Close();
            }
            return validated;
        }



        private void CheckSyntax_OnClick(object sender, RoutedEventArgs e)
        {
            if (TabControl.SelectedItem == null) return;

            var tab = (EditorTab) TabControl.SelectedItem;


            tab.CompilerMessages.Clear();

            var validated = ValidateCurrentEditorText();


            if (validated != null)
            {
                tab.CompilerMessages.Add(new CompilerMessage("Notice", "No Syntax errors detected.")
                {
                    Clickable = false
                });
            }
        }



        private void Format_OnClick(object sender, RoutedEventArgs e)
        {
            if (TabControl.SelectedItem == null) return;

            var tab = (EditorTab) TabControl.SelectedItem;


            tab.CompilerMessages.Clear();

            var validated = ValidateCurrentEditorText();

            if (validated == null)
            {
                return;
            }

            var formatter = new LSLCodeFormatterVisitor();

            StringWriter str = new StringWriter();
            formatter.WriteAndFlush(tab.SourceCode, validated, str);

            string st = str.ToString();
            if (tab.SourceCode != st)
            {
                tab.ChangesPending = true;
                if (!tab.MemoryOnly)
                {
                    tab.TabHeader = Path.GetFileName(tab.FilePath) + "*";
                }
            }

            tab.SourceCode = st;


            tab.CompilerMessages.Clear();

            validated = ValidateCurrentEditorText();


            if (validated != null)
            {
                tab.CompilerMessages.Add(new CompilerMessage("Notice", "No Syntax errors detected.")
                {
                    Clickable = false
                });
            }
        }



        private void OpenInNewTab()
        {
            var openDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "LSL Scripts (*.lsl *.txt)|*.lsl;*.txt"
            };


            var showDialog = openDialog.ShowDialog();
            if (showDialog != null && showDialog.Value)
            {
                var tab = CreateEditorTab();

                if (tab.OpenFileInteractive(openDialog.FileName))
                {
                    EditorTabs.Add(tab);
                    TabControl.SelectedIndex = EditorTabs.Count - 1;
                }
            }
        }



        private void Open_OnClick(object sender, RoutedEventArgs e)
        {
            OpenInNewTab();
        }



        private void OpenInCurrentTab()
        {
            var tab = TabControl.SelectedItem as EditorTab;

            if (tab == null) return;


            if (tab.ChangesPending)
            {
                var r = MessageBox.Show(tab.MemoryOnly ? 
                      "Would you like to save the changes in this tab to a new file before opening a another file in this tab?" 
                    : "Would you like to save the changes to this tab before opening another file in it?", 
                    
                    "Save Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                switch (r)
                {
                    case MessageBoxResult.Yes:
                        if (!tab.SaveTabToFileInteractive())
                        {
                            return;
                        }
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }


            var openDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "LSL Scripts (*.lsl *.txt)|*.lsl;*.txt"
            };


            var showDialog = openDialog.ShowDialog();
            if (showDialog != null && showDialog.Value)
            {


                tab.OpenFileInteractive(openDialog.FileName);
            }
        }



        private void OpenInThisTab_OnClick(object sender, RoutedEventArgs e)
        {
            if (TabControl.SelectedItem != null)
            {
                
                OpenInCurrentTab();
            }
            else
            {
                OpenInNewTab();
            }
        }



        private void Window1_OnClosing(object sender, CancelEventArgs e)
        {
            for (var i = 0; i < EditorTabs.Count; i++)
            {
                var tab = EditorTabs[i];

                if (tab.ChangesPending)
                {
                    TabControl.SelectedIndex = i;
                    if (!tab.Close())
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }
        }


        private FindReplaceMgr FindDialogManager { get; set; }


        private void Window1_OnLoaded(object sender, RoutedEventArgs e)
        {

            FindDialogManager = new FindReplaceMgr
            {
                InterfaceConverter = new IEditorConverter(),
                ShowSearchIn = false
            };



            var args = Environment.GetCommandLineArgs();


            if (args.Length > 1)
            {
                for (int i = 1; i < args.Length; i++)
                {
                    var tab = CreateEditorTab();
                    tab.OpenFileInteractive(args[i]);
                    EditorTabs.Add(tab);
                }
            }
            else
            {
                EditorTabs.Add(CreateEditorTab());
            }

            TabControl.SelectedIndex = 0;
        }



        private void Search_OnClick(object sender, RoutedEventArgs e)
        {
            FindDialogManager.ShowAsFind();
        }

        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var i in e.RemovedItems)
            {
                var t = i as EditorTab;
                if (t != null)
                {
                    t.IsSelected = false;
                }
            }

            foreach (var i in e.AddedItems)
            {
                var t = i as EditorTab;
                if (t != null)
                {
                    t.Content.Editor.Editor.Unloaded += (o, args) =>
                    {
                        if (ReferenceEquals(FindDialogManager.CurrentEditor, t.Content.Editor.Editor) && _droppingTab)
                        {
                            FindDialogManager.CurrentEditor = null;
                        }
                    };

                    FindDialogManager.CurrentEditor = t.Content.Editor.Editor;

                    t.IsSelected = true;
                    t.CheckExternalChanges();
                }
            }
        }



        private void TabStackPanelPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            var textBlock = e.Source as TextBlock;

            if (textBlock == null) return;

            var stackPanel = textBlock.Parent as StackPanel;

            if (stackPanel == null) return;

            var contentPresenter = stackPanel.TemplatedParent as ContentPresenter;

            if (contentPresenter == null) return;


            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(contentPresenter, contentPresenter, DragDropEffects.All);
            }
        }



        private bool _droppingTab;

        private void TabOnDrop(object sender, DragEventArgs e)
        {

            var textBlock = e.Source as TextBlock;

            if (textBlock == null) return;

            var stackPanel = textBlock.Parent as StackPanel;

            if (stackPanel == null) return;

            var contentPresenter = stackPanel.TemplatedParent as ContentPresenter;

            if (contentPresenter == null) return;


            var tabItemTarget = contentPresenter.Content as EditorTab;

            if (tabItemTarget == null) return;

            var tabItemSourceContentPresenter = e.Data.GetData(typeof(ContentPresenter)) as ContentPresenter;


            if (tabItemSourceContentPresenter == null) return;

            var tabItemSource = tabItemSourceContentPresenter.Content as EditorTab;

            if (tabItemSource == null) return;


            if (!tabItemTarget.Equals(tabItemSource))
            {
                _droppingTab = true;
                int sourceIndex = EditorTabs.IndexOf(tabItemSource);
                int targetIndex = EditorTabs.IndexOf(tabItemTarget);

                EditorTabs.Remove(tabItemSource);
                EditorTabs.Insert(targetIndex, tabItemSource);

                EditorTabs.Remove(tabItemTarget);
                EditorTabs.Insert(sourceIndex, tabItemTarget);

                TabControl.SelectedIndex = targetIndex;
                _droppingTab = false;
            }
            
        }



        private void ClearCompilerMessages_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = TabControl.SelectedItem as EditorTab;

            if (tab != null)
            {
                tab.CompilerMessages.Clear();
            }
        }
    }
}