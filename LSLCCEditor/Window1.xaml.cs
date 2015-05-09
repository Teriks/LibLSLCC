using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Exceptions;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.Compilers;
using LibLSLCC.Formatter.Visitor;
using LSLCCEditor.LSLEditor;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

// ReSharper disable LocalizableElement

namespace LSLCCEditor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        ObservableCollection<EditorTab> _tabs = new ObservableCollection<EditorTab>();

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
                var tab = (EditorTab)_parent.TabControl.SelectedItem;
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
                var tab = (EditorTab)_parent.TabControl.SelectedItem;
                tab.CompilerMessages.Add(new CompilerMessage("Error", location, message));
            }
        }


        public Window1()
        {
            InitializeComponent();

            _tabs.Add(new EditorTab());

            _validatorServices = new LSLCustomValidatorServiceProvider
            {
                ExpressionValidator = new LSLDefaultExpressionValidator(),
                StringLiteralPreProcessor = new LSLDefaultStringPreProcessor(),
                SyntaxErrorListener = new WindowSyntaxErrorListener(this),
                SyntaxWarningListener = new WindowSyntaxWarningListener(this)
            };

        }

        public ObservableCollection<EditorTab> EditorTabs
        {
            get { return _tabs; }
            set { _tabs = value; }
        }






        private void CompileForOpenSim_OnClick(object sender, RoutedEventArgs e)
        {

            if (this.TabControl.SelectedContent == null) return;

            var tab = (EditorTab) this.TabControl.SelectedContent;

            var suggestedFileName = "LSLScript.cs";


            if (!string.IsNullOrWhiteSpace(tab.FilePath))
            {
                suggestedFileName = System.IO.Path.GetFileName(tab.FilePath) + ".cs";
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
                        SaveOpenFileTab(tab);
                    }
                    CompileCurrentEditorText(saveDialog.FileName);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Could Not Compile");
                throw;
            }
        }



        private void NewFile_OnClick(object sender, RoutedEventArgs e)
        {
            EditorTabs.Add(new EditorTab());
            TabControl.SelectedIndex = (EditorTabs.Count-1);

        }




        private void LSLEditorControl_OnTextChanged(object sender, EventArgs e)
        {


            var item = TabControl.SelectedItem as EditorTab;
            if (item != null)
            {
                if (!item.MemoryOnly && !item.ChangesPending)
                {
                    item.Header = item.Header + "*";
                }
                item.ChangesPending = true;
            }
        }



        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null)
            {
                return;
            }

            if (tab.MemoryOnly)
            {
                SaveMemoryOnlyTab(tab);
            }
            else
            {
                SaveOpenFileTab(tab);
            }
        }



        private void SaveOpenFileTab(EditorTab tab)
        {
            try
            {
                tab.Header = System.IO.Path.GetFileName(tab.FilePath);
                tab.ChangesPending = false;
                tab.MemoryOnly = false;
                File.WriteAllText(tab.FilePath, tab.SourceCode);
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not write file: "+e.Message);
            }
        }



        private void CompileCurrentEditorText(string destinationFile)
        {
            if (this.TabControl.SelectedItem == null) return;

            var tab = (EditorTab)this.TabControl.SelectedItem;

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
                            "Internal Compiler Error");
                        throw;
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("Please report this message with the code that caused it: " + error.Message,
                        "Unknown Compiler Error");
                    throw;
                }
            }

            tab.CompilerMessages.Add(new CompilerMessage("Notice", "Program compiled successfully"));
        }




        private void SaveMemoryOnlyTab(EditorTab tab)
        {

            var saveDialog = new SaveFileDialog
            {
                FileName = "LSLScript.lsl",
                DefaultExt = ".lsl",
                Filter = "LSL Script (*.lsl *.txt)|*.lsl;*.txt"
            };


            var showDialog = saveDialog.ShowDialog();
            if (showDialog != null && showDialog.Value)
            {
                try
                {
                    File.WriteAllText(saveDialog.FileName, tab.SourceCode);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Could not write file: " + e.Message);
                }

                tab.ChangesPending = false;
                tab.MemoryOnly = false;
                tab.FilePath = saveDialog.FileName;
                tab.Header = System.IO.Path.GetFileName(saveDialog.FileName);
            }
        }



        private void SaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = TabControl.SelectedItem as EditorTab;
            if (tab == null)
            {
                return;
            }


            var saveDialog = new SaveFileDialog
            {
                FileName = "LSLScript.lsl",
                DefaultExt = ".lsl",
                Filter = "LSL Script (*.lsl *.txt)|*.lsl;*.txt"
            };


            var showDialog = saveDialog.ShowDialog();
            if (showDialog != null && showDialog.Value)
            {
                try
                {
                    File.WriteAllText(saveDialog.FileName, tab.SourceCode);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not write file: " + ex.Message);
                }

                tab.ChangesPending = false;
                tab.MemoryOnly = false;
                tab.FilePath = saveDialog.FileName;
                tab.Header = System.IO.Path.GetFileName(saveDialog.FileName);
            }
        }






        private void CompilerMessageItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_editorInstance != null)
            {

                

                var message = (CompilerMessage)((ListViewItem)sender).Content;

                if (!message.Clickable)
                {
                    return;
                }


                var location = message.CodeLocation;

                var line = location.LineStart == 0 ? 1 : location.LineStart;

                var lineend = location.LineEnd == 0 ? 1 : location.LineEnd;


                _editorInstance.Editor.ScrollToLine(line);


                if (message.CodeLocation.HasIndexInfo && message.CodeLocation.IsSingleLine)
                {
                    _editorInstance.Editor.Select(message.CodeLocation.StartIndex,
                        (message.CodeLocation.StopIndex + 1) - message.CodeLocation.StartIndex);

                    return;
                }


                int l = 0;
                for (int i = line; i <= lineend; i++)
                {
                    l += _editorInstance.Editor.Document.GetLineByNumber(i).TotalLength;
                }

                var linestart = _editorInstance.Editor.Document.GetLineByNumber(line);


                _editorInstance.Editor.Select(linestart.Offset, l);
            }
            
           
        }


        private ILSLCompilationUnitNode ValidateCurrentEditorText()
        {

            if (this.TabControl.SelectedItem == null) return null;

            var tab = (EditorTab) this.TabControl.SelectedItem;

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
                        "Internal Validation Error");
                    validated = null;
                }
            }
            catch (Exception error)
            {
                
                MessageBox.Show("Please report this message with the code that caused it: " + error.Message,
                    "Unknown Validation Error");
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
            if (this.TabControl.SelectedItem == null) return;

            var tab = (EditorTab)this.TabControl.SelectedItem;


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
            if (this.TabControl.SelectedContent == null) return;

            var tab = (EditorTab)this.TabControl.SelectedContent;


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
                    tab.Header = System.IO.Path.GetFileName(tab.FilePath)+"*";
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



        private LSLEditorControl _editorInstance;
        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            _editorInstance = (LSLEditorControl)sender;
        }



        void OpenInNewTab()
        {
            var openDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "LSL Scripts (*.lsl *.txt)|*.lsl;*.txt"
            };

            try
            {
                var showDialog = openDialog.ShowDialog();
                if (showDialog != null && showDialog.Value)
                {

                    if (this.EditorTabs.Any(x => x.FilePath == openDialog.FileName))
                    {
                        MessageBox.Show("File: \"" + openDialog.FileName + "\", Is already open in another tab.", "File already open");
                        return;
                    }


                    var tab = new EditorTab();

                    tab.SourceCode = File.ReadAllText(openDialog.FileName);


                    this.EditorTabs.Add(tab);
                    TabControl.SelectedIndex = (EditorTabs.Count - 1);

                    tab.MemoryOnly = false;
                    tab.ChangesPending = false;
                    tab.Header = System.IO.Path.GetFileName(openDialog.FileName);
                    tab.FilePath = openDialog.FileName;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Could Not Read File: " + err.Message);
            }
        }


        private void Open_OnClick(object sender, RoutedEventArgs e)
        {

            OpenInNewTab();
        }



        void OpenInCurrentTab()
        {
            var openDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "LSL Scripts (*.lsl *.txt)|*.lsl;*.txt"
            };

            try
            {
                var showDialog = openDialog.ShowDialog();
                if (showDialog != null && showDialog.Value)
                {

                    if (this.EditorTabs.Any(x => x.FilePath == openDialog.FileName))
                    {
                        MessageBox.Show("File: \"" + openDialog.FileName + "\", Is already open in another tab.", "File already open");
                        return;
                    }


                    if (this.TabControl.SelectedItem == null) return;

                    var tab = (EditorTab)this.TabControl.SelectedItem;

                    tab.SourceCode = File.ReadAllText(openDialog.FileName);

                    tab.ChangesPending = false;
                    tab.MemoryOnly = false;

                    tab.Header = System.IO.Path.GetFileName(openDialog.FileName);
                    tab.FilePath = openDialog.FileName;


                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Could Not Read File: " + err.Message);
            }
        }

        private void OpenInThisTab_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.TabControl.SelectedItem != null)
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
            foreach (var tab in EditorTabs.Where(x=>x.ChangesPending))
            {
                if (!string.IsNullOrWhiteSpace(tab.FilePath))
                {
                    var result = MessageBox.Show("Do you want to save: \"" + tab.FilePath + "\"?", "Save Changed File",
                        MessageBoxButtons.YesNo);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        try
                        {
                            File.WriteAllText(tab.FilePath, tab.SourceCode);
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Unable to save file: " + err.Message, "Error");
                        }
                    }
                }
            }
        }



        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (System.Windows.Controls.Button) sender;
            var stackPanel = (StackPanel) button.Parent;
            var tab = (EditorTab)((ContentPresenter)stackPanel.TemplatedParent).Content;

            if (tab.ChangesPending)
            {
                if (!string.IsNullOrWhiteSpace(tab.FilePath))
                {
                    var result = MessageBox.Show("Do you want to save: \"" + tab.FilePath + "\"?", "Save Changed File",
                        MessageBoxButtons.YesNo);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        try
                        {
                            File.WriteAllText(tab.FilePath, tab.SourceCode);
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Unable to save file: " + err.Message, "Error");
                        }
                    }
                }
            }

            this.EditorTabs.Remove(tab);

        }
    }




    public class EditorTab : DependencyObject
    {

        private ObservableCollection<CompilerMessage> _compilerMessages = new ObservableCollection<CompilerMessage>(); 


        public static readonly DependencyProperty LibraryDataProviderProperty = DependencyProperty.Register(
            "LibraryDataProvider", typeof (ILSLMainLibraryDataProvider), typeof (EditorTab), new PropertyMetadata(default(ILSLMainLibraryDataProvider)));

        public ILSLMainLibraryDataProvider LibraryDataProvider
        {
            get { return (ILSLMainLibraryDataProvider) GetValue(LibraryDataProviderProperty); }
            set { SetValue(LibraryDataProviderProperty, value); }
        }


        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof(string), typeof(EditorTab), new PropertyMetadata(default(string)));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        public static readonly DependencyProperty SourceCodeProperty = DependencyProperty.Register("SourceCode", typeof(string), typeof(EditorTab), new PropertyMetadata(default(string)));

        public string SourceCode
        {
            get { return (string)GetValue(SourceCodeProperty); }
            set { SetValue(SourceCodeProperty, value); }
        }


        public bool ChangesPending { get; set; }

        public bool MemoryOnly { get; set; }

        public string FilePath { get; set; }

        public ObservableCollection<CompilerMessage> CompilerMessages
        {
            get { return _compilerMessages; }
            set { _compilerMessages = value; }
        }



        public EditorTab()
        {
            MemoryOnly = true;
            ChangesPending = false;
            LibraryDataProvider = new LSLDefaultLibraryDataProvider(LSLLibraryBaseData.StandardLsl);
            Header = "New (Unsaved)";
        }
    }
}
