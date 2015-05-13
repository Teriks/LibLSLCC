using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
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
    public partial class TabbedMainWindow : Window
    {
        private ObservableCollection<EditorTab> _tabs = new ObservableCollection<EditorTab>();

        private readonly LSLCustomValidatorServiceProvider _validatorServices;


        private readonly LSLDefaultLibraryDataProvider _libraryDataProvider = new LSLDefaultLibraryDataProvider(true, LSLLibraryBaseData.StandardLsl);



        public static RoutedCommand FileNew = new RoutedCommand();
        public static RoutedCommand FileOpen = new RoutedCommand();
        public static RoutedCommand FileOpenNewTab = new RoutedCommand();
        public static RoutedCommand FileSave = new RoutedCommand();
        public static RoutedCommand FileSaveAs = new RoutedCommand();

        public static RoutedCommand ToolsFormat = new RoutedCommand();
        public static RoutedCommand ToolsSyntaxCheck = new RoutedCommand();
        public static RoutedCommand ToolsClearMessages = new RoutedCommand();

        public static RoutedCommand CompileOpenSim = new RoutedCommand();

        public static RoutedCommand SearchFind = new RoutedCommand();
        public static RoutedCommand SearchReplace = new RoutedCommand();



        private class WindowSyntaxWarningListener : LSLDefaultSyntaxWarningListener
        {
            private readonly TabbedMainWindow _parent;



            public WindowSyntaxWarningListener(TabbedMainWindow parent)
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
            private readonly TabbedMainWindow _parent;



            public WindowSyntaxErrorListener(TabbedMainWindow parent)
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

        public TabbedMainWindow()
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
            const string code = "default\n{\n\tstate_entry()\n\t{\n\t\tllSay(0, \"Hello World!\");\n\t}\n}";

            var t = new EditorTab(TabControl, EditorTabs, _libraryDataProvider)
            {
                SourceCode = code,
                ChangesPending = false
            };


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
                var alreadyOpen = EditorTabs.FirstOrDefault(x => x.FilePath == openDialog.FileName);
                if (alreadyOpen != null)
                {
                    TabControl.SelectedIndex = EditorTabs.IndexOf(alreadyOpen);
                    return;
                }


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
                OwnerWindow = this,
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



        private void Find_OnClick(object sender, RoutedEventArgs e)
        {

            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            FindDialogManager.TextToFind = tab.Content.Editor.Editor.SelectedText;

            FindDialogManager.ShowAsFind();
        }

        private void Replace_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            FindDialogManager.TextToFind = tab.Content.Editor.Editor.SelectedText;

            FindDialogManager.ShowAsReplace();
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

                    t.LibraryDataProvider.LiveFilteringBaseLibraryData = t.BaseLibraryDataCache;
                    t.LibraryDataProvider.LiveFilteringLibraryDataAdditions = t.LibraryDataAdditionsCache;
                    t.Content.Editor.UpdateHighlightingFromDataProvider();

                    SetLibraryMenuFromTab(t);


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



        private Timer _tabDragTimer;
        private void TabStackPanelPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(EditorTabs.Count<=1) return;

            var textBlock = e.Source as TextBlock;

            if (textBlock == null) return;

            var stackPanel = textBlock.Parent as StackPanel;

            if (stackPanel == null) return;

            var contentPresenter = stackPanel.TemplatedParent as ContentPresenter;

            if (contentPresenter == null) return;

            _tabDragTimer = new Timer(500);
            _tabDragTimer.Elapsed += (o, args) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (Mouse.PrimaryDevice.LeftButton != MouseButtonState.Pressed) return;

                    _tabDragTimer.Stop();
                    _tabDragTimer.Dispose();
                    _tabDragTimer = null;
                    DragDrop.DoDragDrop(contentPresenter, contentPresenter, DragDropEffects.All);
                });
            };
            _tabDragTimer.Start();
        }


        private void TabStackPanelOnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_tabDragTimer != null)
            {
                _tabDragTimer.Stop();
                _tabDragTimer.Dispose();
                _tabDragTimer = null;
            }
            
        }


        private bool _droppingTab;
        private bool _settingLibraryMenuFromTab;
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



        private void SetLibraryMenuFromTab(EditorTab tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            _settingLibraryMenuFromTab = true;

            LindenLsl.IsChecked = tab.BaseLibraryDataCache == LSLLibraryBaseData.StandardLsl;
            OpenSimLsl.IsChecked = !LindenLsl.IsChecked;

            OsBulletPhysics.IsChecked = ((tab.LibraryDataAdditionsCache & LSLLibraryDataAdditions.OpenSimBulletPhysics) ==
                                               LSLLibraryDataAdditions.OpenSimBulletPhysics);
            

            OsModInvoke.IsChecked = ((tab.LibraryDataAdditionsCache & LSLLibraryDataAdditions.OpenSimModInvoke) ==
                LSLLibraryDataAdditions.OpenSimModInvoke);


            OsslFunctions.IsChecked = ((tab.LibraryDataAdditionsCache & LSLLibraryDataAdditions.OpenSimOssl) ==
                                            LSLLibraryDataAdditions.OpenSimOssl);


            OsWindlight.IsChecked = ((tab.LibraryDataAdditionsCache & LSLLibraryDataAdditions.OpenSimWindlight) ==
                                          LSLLibraryDataAdditions.OpenSimWindlight);



            _settingLibraryMenuFromTab = false;
        }



        private void LindenLsl_OnChecked(object sender, RoutedEventArgs e)
        {
            if (TabControl == null || _settingLibraryMenuFromTab) return;

            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;


            

            if (OpenSimLsl != null && OpenSimLsl.IsChecked)
            {
                OpenSimLsl.IsChecked = false;
                tab.BaseLibraryDataCache = LSLLibraryBaseData.StandardLsl;
                tab.LibraryDataProvider.LiveFilteringBaseLibraryData = tab.BaseLibraryDataCache;
                tab.Content.Editor.UpdateHighlightingFromDataProvider();
            }
        }



        private void OpenSimLsl_OnChecked(object sender, RoutedEventArgs e)
        {
            if(TabControl == null || _settingLibraryMenuFromTab) return;

            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;



            if (LindenLsl != null && LindenLsl.IsChecked)
            {
                LindenLsl.IsChecked = false;
                tab.BaseLibraryDataCache = LSLLibraryBaseData.OpensimLsl;
                tab.LibraryDataProvider.LiveFilteringBaseLibraryData = tab.BaseLibraryDataCache;
                tab.Content.Editor.UpdateHighlightingFromDataProvider();
            }
        }



        private void LindenLsl_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if(TabControl == null || _settingLibraryMenuFromTab) return;

            if (OpenSimLsl != null && !OpenSimLsl.IsChecked)
            {
                LindenLsl.IsChecked = true;
            }
        }



        private void OpenSimLsl_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if(TabControl == null || _settingLibraryMenuFromTab) return;

            if (LindenLsl != null && !LindenLsl.IsChecked)
            {
                OpenSimLsl.IsChecked = true;
            }
        }


        private void OsslFunctions_OnChecked(object sender, RoutedEventArgs e)
        {
            if(TabControl == null || _settingLibraryMenuFromTab) return;

            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            tab.LibraryDataAdditionsCache |= LSLLibraryDataAdditions.OpenSimOssl;
            tab.LibraryDataProvider.LiveFilteringLibraryDataAdditions = tab.LibraryDataAdditionsCache;
            tab.Content.Editor.UpdateHighlightingFromDataProvider();
            
        }



        private void OsslFunctions_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if(TabControl == null || _settingLibraryMenuFromTab) return;

            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            tab.LibraryDataAdditionsCache &= ~LSLLibraryDataAdditions.OpenSimOssl;
            tab.LibraryDataProvider.LiveFilteringLibraryDataAdditions = tab.LibraryDataAdditionsCache;
            tab.Content.Editor.UpdateHighlightingFromDataProvider();
        }



        private void OsWindlight_OnChecked(object sender, RoutedEventArgs e)
        {
            if(TabControl == null || _settingLibraryMenuFromTab) return;

            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            tab.LibraryDataAdditionsCache |= LSLLibraryDataAdditions.OpenSimWindlight;
            tab.LibraryDataProvider.LiveFilteringLibraryDataAdditions = tab.LibraryDataAdditionsCache;
            tab.Content.Editor.UpdateHighlightingFromDataProvider();
        }



        private void OsWindlight_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if(TabControl == null || _settingLibraryMenuFromTab) return;

            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            tab.LibraryDataAdditionsCache &= ~LSLLibraryDataAdditions.OpenSimWindlight;
            tab.LibraryDataProvider.LiveFilteringLibraryDataAdditions = tab.LibraryDataAdditionsCache;
            tab.Content.Editor.UpdateHighlightingFromDataProvider();
        }



        private void OsBulletPhysics_OnChecked(object sender, RoutedEventArgs e)
        {
            if(TabControl == null || _settingLibraryMenuFromTab) return;

            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            tab.LibraryDataAdditionsCache |= LSLLibraryDataAdditions.OpenSimBulletPhysics;
            tab.LibraryDataProvider.LiveFilteringLibraryDataAdditions = tab.LibraryDataAdditionsCache;
            tab.Content.Editor.UpdateHighlightingFromDataProvider();
        }



        private void OsBulletPhysics_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if(TabControl == null || _settingLibraryMenuFromTab) return;

            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            tab.LibraryDataAdditionsCache &= ~LSLLibraryDataAdditions.OpenSimBulletPhysics;
            tab.LibraryDataProvider.LiveFilteringLibraryDataAdditions = tab.LibraryDataAdditionsCache;
            tab.Content.Editor.UpdateHighlightingFromDataProvider();
        }



        private void OsModInvoke_OnChecked(object sender, RoutedEventArgs e)
        {
            if(TabControl == null || _settingLibraryMenuFromTab) return;

            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            tab.LibraryDataAdditionsCache |= LSLLibraryDataAdditions.OpenSimModInvoke;
            tab.LibraryDataProvider.LiveFilteringLibraryDataAdditions = tab.LibraryDataAdditionsCache;
            tab.Content.Editor.UpdateHighlightingFromDataProvider();
        }



        private void OsModInvoke_OnUnChecked(object sender, RoutedEventArgs e)
        {
            if(TabControl == null || _settingLibraryMenuFromTab) return;

            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            tab.LibraryDataAdditionsCache &= ~LSLLibraryDataAdditions.OpenSimModInvoke;
            tab.LibraryDataProvider.LiveFilteringLibraryDataAdditions = tab.LibraryDataAdditionsCache;
            tab.Content.Editor.UpdateHighlightingFromDataProvider();
        }



        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            System.Windows.Forms.MessageBox.Show(tab.Content.Editor.Editor.CaretOffset.ToString());
        }
    }
}