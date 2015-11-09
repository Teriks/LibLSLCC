#region FileInfo

// 
// File: TabbedMainWindow.xaml.cs
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Compilers;
using LibLSLCC.Formatter;
using LibLSLCC.LibraryData;
using LibLSLCC.Utility;
using LSLCCEditor.EditorTabUI;
using LSLCCEditor.FindReplaceUI;
using LSLCCEditor.Settings;
using Microsoft.Win32;

#endregion

// ReSharper disable LocalizableElement

namespace LSLCCEditor
{
    /// <summary>
    ///     Interaction logic for Window1.xaml
    /// </summary>
    public partial class TabbedMainWindow : Window
    {
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

        private bool _droppingTabAfterDragging;

        private LSLXmlLibraryDataProvider _libraryDataProvider;

        private bool _settingLibraryMenuProgrammaticallyFromTabCache;

        private bool _uncheckingLibraryDataTabItemProgrammatically;

        private bool _selectingStartupTabDuringWindowLoad;

        private Timer _tabDragTimer;

        private LSLValidatorServiceProvider _validatorServices;






        public TabbedMainWindow()
        {

            InitializeComponent();
            Initialize();

        }


        private void Initialize()
        {


            _libraryDataProvider = new LSLXmlLibraryDataProvider(new[] { "lsl" });


            try
            {
                _libraryDataProvider.FillFromXmlDirectory("library_data");

            }
            catch (LSLLibraryDataXmlSyntaxException err)
            {
                MessageBox.Show(
                    "There is a syntax error in one of your XML library data files and the application must close." 
                    + LSLFormatTools.CreateNewLinesString(2) + err.Message,
                    "Library Data Syntax Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Application.Current.Shutdown();
            }



            _validatorServices = new LSLValidatorServiceProvider
            {
                ExpressionValidator = new LSLDefaultExpressionValidator(),
                StringLiteralPreProcessor = new LSLDefaultStringPreProcessor(),
                SyntaxErrorListener = new WindowSyntaxErrorListener(this),
                SyntaxWarningListener = new WindowSyntaxWarningListener(this),
                LibraryDataProvider = _libraryDataProvider
            };


            foreach (var dataMenuItem in _libraryDataProvider.SubsetDescriptions.Select(subset => new MenuItem
            {
                IsCheckable = true,
                Header = subset.Value.FriendlyName,
                Tag = subset.Value.Subset,
                ToolTip = new ToolTip { Content = new TextBlock { Text = subset.Value.Description } }
            }))
            {
                dataMenuItem.Checked += DataMenuItemOnChecked;
                dataMenuItem.Unchecked += DataMenuItemOnUnChecked;
                TabLibraryDataMenu.Items.Add(dataMenuItem);
            }



            FindDialogManager = new FindReplaceMgr
            {
                OwnerWindow = this,
                InterfaceConverter = new IEditorConverter(),
                ShowSearchIn = false
            };


            var args = Environment.GetCommandLineArgs();


            if (args.Length > 1)
            {
                for (var i = 1; i < args.Length; i++)
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

            _selectingStartupTabDuringWindowLoad = true;

            TabControl.SelectedIndex = 0;

            var initialTab = (EditorTab) TabControl.SelectedItem;

            SetLibraryMenuFromTab(initialTab);

            FindDialogManager.CurrentEditor = initialTab.Content.Editor.Editor;

            _selectingStartupTabDuringWindowLoad = false;

        }


        private void TabbedMainWindow_OnClosing(object sender, CancelEventArgs e)
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


        public ObservableCollection<EditorTab> EditorTabs
        {
            get { return _editorTabs; }
            set { _editorTabs = value; }
        }

        private FindReplaceMgr FindDialogManager { get; set; }


        private void SetLibraryMenuFromTab(EditorTab tab)
        {
            if (tab == null)
            {
                throw new ArgumentNullException("tab");
            }

            _settingLibraryMenuProgrammaticallyFromTabCache = true;


            foreach (var menuItem in TabLibraryDataMenu.Items.Cast<MenuItem>())
            {
                menuItem.IsChecked = tab.ActiveLibraryDataSubsetsCache.Contains(menuItem.Tag.ToString());
            }


            _settingLibraryMenuProgrammaticallyFromTabCache = false;
        }

        private void DataMenuItemOnUnChecked(object sender, RoutedEventArgs e)
        {
            if (TabControl == null || _settingLibraryMenuProgrammaticallyFromTabCache) return;

            var item = sender as MenuItem;

            var tab = (EditorTab) TabControl.SelectedItem;


            if (item == null) return;

            var subsetName = item.Tag.ToString();


            var osLslMenuItem =
                TabLibraryDataMenu.Items.Cast<MenuItem>()
                    .FirstOrDefault(x => x.Tag.ToString() == LSLLibraryBaseData.OpensimLsl.ToSubsetName());
            var lslMenuItem =
                TabLibraryDataMenu.Items.Cast<MenuItem>()
                    .FirstOrDefault(x => x.Tag.ToString() == LSLLibraryBaseData.StandardLsl.ToSubsetName());

            if (subsetName == LSLLibraryBaseData.StandardLsl.ToSubsetName() && osLslMenuItem != null)
            {
                osLslMenuItem.IsChecked = true;
            }

            if (subsetName == LSLLibraryBaseData.OpensimLsl.ToSubsetName() && lslMenuItem != null)
            {
                lslMenuItem.IsChecked = true;
            }


            _libraryDataProvider.ActiveSubsets.Remove(subsetName);
            tab.ActiveLibraryDataSubsetsCache.Remove(subsetName);

            if (!_uncheckingLibraryDataTabItemProgrammatically)
            {
                tab.Content.Editor.UpdateHighlightingFromDataProvider();
            }
        }

        private void DataMenuItemOnChecked(object sender, RoutedEventArgs routedEventArgs)
        {
            if (TabControl == null || _settingLibraryMenuProgrammaticallyFromTabCache) return;

            var item = sender as MenuItem;

            var tab = (EditorTab) TabControl.SelectedItem;


            if (item == null) return;

            var subsetName = item.Tag.ToString();


            var osLslMenuItem =
                TabLibraryDataMenu.Items.Cast<MenuItem>()
                    .FirstOrDefault(x => x.Tag.ToString() == LSLLibraryBaseData.OpensimLsl.ToSubsetName());
            var lslMenuItem =
                TabLibraryDataMenu.Items.Cast<MenuItem>()
                    .FirstOrDefault(x => x.Tag.ToString() == LSLLibraryBaseData.StandardLsl.ToSubsetName());

            if (subsetName == LSLLibraryBaseData.StandardLsl.ToSubsetName() && osLslMenuItem != null)
            {
                _uncheckingLibraryDataTabItemProgrammatically = true;
                osLslMenuItem.IsChecked = false;
                _uncheckingLibraryDataTabItemProgrammatically = false;
            }

            if (subsetName == LSLLibraryBaseData.OpensimLsl.ToSubsetName() && lslMenuItem != null)
            {
                _uncheckingLibraryDataTabItemProgrammatically = true;
                lslMenuItem.IsChecked = false;
                _uncheckingLibraryDataTabItemProgrammatically = false;
            }


            _libraryDataProvider.ActiveSubsets.Add(subsetName);
            tab.ActiveLibraryDataSubsetsCache.Add(subsetName);
            tab.Content.Editor.UpdateHighlightingFromDataProvider();
        }



        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //the initial tab already has already been setup before it was selected.
            if (_selectingStartupTabDuringWindowLoad) return;


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
                var tab = i as EditorTab;
                if (tab != null)
                {
                    _libraryDataProvider.ActiveSubsets.SetSubsets(tab.ActiveLibraryDataSubsetsCache);

                    SetLibraryMenuFromTab(tab);

                    _libraryDataProvider.ActiveSubsets.Clear();
                    _libraryDataProvider.ActiveSubsets.AddSubsets(tab.ActiveLibraryDataSubsetsCache);
                    tab.Content.Editor.UpdateHighlightingFromDataProvider();


                    tab.Content.Editor.Editor.Unloaded += (o, args) =>
                    {
                        if (ReferenceEquals(FindDialogManager.CurrentEditor, tab.Content.Editor.Editor) && _droppingTabAfterDragging)
                        {
                            FindDialogManager.CurrentEditor = null;
                        }
                    };

                    FindDialogManager.CurrentEditor = tab.Content.Editor.Editor;

                    tab.IsSelected = true;
                    tab.CheckExternalChanges();
                }
            }
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

 

        private void CompileForOpenSimClickStub()
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

            //I want to be able to debug compiler errors in debug mode
            //Hence the conditional compilation.

#if !DEBUG
            try
            {
#endif
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
#if !DEBUG
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Could Not Compile",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
#endif
        }



        private LSLOpenSimCompilerSettings _openSimCompilerSettings;
        private ObservableCollection<EditorTab> _editorTabs = new ObservableCollection<EditorTab>();


        private const string _clientSideScriptCompilerHeader =
@"//c#
/** 
*  Do not remove //c# from the first line of this script.
*
*  This is OpenSim CSharp code, CSharp scripting must be enabled on the server to run.
*
*  Please note this script does not support being reset, because a constructor was not generated.
*  Compile using the server side script option to generate a script constructor.
*
*  This code will run on an unmodified OpenSim server, however script resets will not reset global variables,
*  and OpenSim will be unable to save the state of this script as its global variables are created in an object container.
*
*/ 
";


        private const string _serverSideScriptCompilerHeader =
@"//c#-raw
/** 
*  Do not remove //c#-raw from the first line of this script.
*
*  This is OpenSim CSharp code, CSharp scripting must be enabled on the server to run.
*
*  This is a server side script.  It constitutes a fully generated script class that
*  will be sent to the CSharp compiler in OpenSim.  This code supports script resets.
*
*  This script is meant to upload compatible with the LibLSLCC OpenSim fork.
*
*  If you are running a version of OpenSim with the LibLSLCC compiler enabled, you must add 'csraw'
*  to the allowed list of compiler languages under [XEngine] for this script to successfully upload.
*
*  Adding 'csraw' to your allowed language list when using the old OpenSim compiler will have no effect
*  besides an error being written to your log file.  OpenSim will run but you will not actually be able
*  to use the 'csraw' upload type.
*
*  Note that you can also set 'CreateClassWrapperForCSharpScripts' to 'false' under the [LibLCLCC]
*  OpenSim.ini config section in order to enable 'csraw' mode uploads for every CSharp script sent to the 
*  LibLSLCC compiler;  Including those marked with '//c#' if you have 'cs' in your list of allowed languages.
*
*/ 
";


        private void CompileForOpenSimServerSide_OnClick(object sender, RoutedEventArgs e)
        {
            _openSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimServerSideDefault();
            _openSimCompilerSettings.ScriptHeader = _serverSideScriptCompilerHeader;


            CompileForOpenSimClickStub();
        }


        private void CompileForOpenSimClientSide_OnClick(object sender, RoutedEventArgs e)
        {
            _openSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimClientUploadable();
            _openSimCompilerSettings.ScriptHeader = _clientSideScriptCompilerHeader;


            CompileForOpenSimClickStub();
        }

        private void CompileForOpenSimClientSideCOOP_OnClick(object sender, RoutedEventArgs e)
        {
            _openSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimClientUploadable();
            _openSimCompilerSettings.ScriptHeader = _clientSideScriptCompilerHeader;
            _openSimCompilerSettings.InsertCoOpTerminationCalls = true;
            CompileForOpenSimClickStub();
        }

        private void CompileForOpenSimServerSideCOOP_OnClick(object sender, RoutedEventArgs e)
        {
            _openSimCompilerSettings = LSLOpenSimCompilerSettings.OpenSimServerSideDefault();
            _openSimCompilerSettings.ScriptHeader = _serverSideScriptCompilerHeader;
            _openSimCompilerSettings.InsertCoOpTerminationCalls = true;
            CompileForOpenSimClickStub();
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

            //I want to be able to debug compiler errors in debug mode
            //Hence the conditional compilation.

#if !DEBUG
            bool compileSuccess = false;
#else
            bool compileSuccess = true;
#endif

            using (var outfile = File.OpenWrite(destinationFile))
            {

                var compiler = new LSLOpenSimCompiler(_libraryDataProvider, _openSimCompilerSettings);

#if !DEBUG

                try
                {
                    compiler.Compile(validated, new StreamWriter(outfile, Encoding.UTF8));
                    compileSuccess = true;
                }
                catch (LSLCompilerInternalException error)
                {
                    MessageBox.Show("Please report this message with the code that caused it: " + error.Message,
                        "Internal Compiler Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                catch (Exception error)
                {
                    MessageBox.Show("Please report this message with the code that caused it: " + error.Message,
                        "Unknown Compiler Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                }
#else
                compiler.Compile(validated, new StreamWriter(outfile, Encoding.UTF8));
#endif
            }

#if !DEBUG
            if (compileSuccess)
            {

                tab.CompilerMessages.Add(new CompilerMessage(CompilerMessageType.General, "Notice", "Program compiled successfully", false) {Clickable = false});
            }
            else
            {
                tab.CompilerMessages.Add(new CompilerMessage(CompilerMessageType.Error, "Error",
                    "An internal compiler exception occurred, please report the code that caused this.", false) { Clickable = false });
            }
#endif
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

            var validator = new LSLCodeValidator(_validatorServices);

            ILSLCompilationUnitNode validated;


            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(tab.SourceCode));

            //I want to be able to debug validator errors in debug mode
            //Hence the conditional compilation.

#if !DEBUG
            try
            {
                using (var infile = new StreamReader(memoryStream, Encoding.UTF8))
                {
                    validated = validator.Validate(infile);
                }
            }
            catch (LSLCodeValidatorInternalException error)
            {
                MessageBox.Show("Please report this message with the code that caused it: " + error.Message,
                    "Internal Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                validated = null;
            }
            catch (Exception error)
            {
                MessageBox.Show("Please report this message with the code that caused it: " + error.Message,
                    "Unknown Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                validated = null;
            }
#else
            using (var infile = new StreamReader(memoryStream, Encoding.UTF8))
            {
                validated = validator.Validate(infile);
            }
#endif

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
                tab.CompilerMessages.Add(new CompilerMessage(CompilerMessageType.General, "Notice",
                    "No Syntax errors detected.", false)
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

            var formatter = new LSLCodeFormatter();

            var str = new StringWriter();
            formatter.Format(tab.SourceCode, validated, str);

            var st = str.ToString();
            if (tab.SourceCode != st)
            {
                tab.ChangesPending = true;
            }

            tab.SourceCode = st;


            tab.CompilerMessages.Clear();

            validated = ValidateCurrentEditorText();


            if (validated != null)
            {
                tab.CompilerMessages.Add(new CompilerMessage(CompilerMessageType.General, "Notice",
                    "No Syntax errors detected.", false)
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
                var r = MessageBox.Show(tab.MemoryOnly
                    ? "Would you like to save the changes in this tab to a new file before opening a another file in this tab?"
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

 

        private void Find_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            FindDialogManager.TextToFind = tab.Content.Editor.Editor.SelectedText;

            FindDialogManager.ShowAsFind(this);
        }

        private void Replace_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null) return;

            FindDialogManager.TextToFind = tab.Content.Editor.Editor.SelectedText;

            FindDialogManager.ShowAsReplace(this);
        }

        private void TabStackPanelPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EditorTabs.Count <= 1) return;

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

            var tabItemSourceContentPresenter = e.Data.GetData(typeof (ContentPresenter)) as ContentPresenter;


            if (tabItemSourceContentPresenter == null) return;

            var tabItemSource = tabItemSourceContentPresenter.Content as EditorTab;

            if (tabItemSource == null) return;


            if (tabItemTarget.Equals(tabItemSource)) return;


            _droppingTabAfterDragging = true;

            var sourceIndex = EditorTabs.IndexOf(tabItemSource);
            var targetIndex = EditorTabs.IndexOf(tabItemTarget);

            EditorTabs.Remove(tabItemSource);
            EditorTabs.Insert(targetIndex, tabItemSource);

            EditorTabs.Remove(tabItemTarget);
            EditorTabs.Insert(sourceIndex, tabItemTarget);

            TabControl.SelectedIndex = targetIndex;

            _droppingTabAfterDragging = false;
        }

        private void ClearCompilerMessages_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = TabControl.SelectedItem as EditorTab;

            if (tab != null) tab.CompilerMessages.Clear();
        }

        private class WindowSyntaxWarningListener : LSLDefaultSyntaxWarningListener
        {
            private readonly TabbedMainWindow _parent;

            public WindowSyntaxWarningListener(TabbedMainWindow parent)
            {
                _parent = parent;
            }

            protected override void OnWarning(LSLSourceCodeRange location, string message)
            {
                var tab = (EditorTab) _parent.TabControl.SelectedItem;
                tab.CompilerMessages.Add(new CompilerMessage(CompilerMessageType.Warning, "Warning", location, message));
            }
        }

        private class WindowSyntaxErrorListener : LSLDefaultSyntaxErrorListener
        {
            private readonly TabbedMainWindow _parent;

            public WindowSyntaxErrorListener(TabbedMainWindow parent)
            {
                _parent = parent;
            }

            protected override void OnError(LSLSourceCodeRange location, string message)
            {
                var tab = (EditorTab) _parent.TabControl.SelectedItem;
                tab.CompilerMessages.Add(new CompilerMessage(CompilerMessageType.Error, "Error", location, message));
            }
        }

        private void SettingsMenu_Click(object sender, RoutedEventArgs e)
        {
            var menu = new SettingsUI.SettingsWindow {Owner = this};
            menu.ShowDialog();
        }
    }
}