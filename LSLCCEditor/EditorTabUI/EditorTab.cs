using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LSLCCEditor.EditorTabUI;
using LSLCCEditor.Utility;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

// ReSharper disable LocalizableElement

namespace LSLCCEditor.EditorTabUI
{
    public class EditorTab : DependencyObject
    {
        private readonly TabControl _owner;
        public IList<EditorTab> OwnerTabCollection { get; private set; }


        public IEnumerable<EditorTab> OtherTabs
        {
            get { return OwnerTabCollection.Where(x => !ReferenceEquals(x, this)); }
        }

        public static readonly DependencyProperty TabHeaderProperty = DependencyProperty.Register(
            "TabHeader", typeof (string), typeof (EditorTab), new FrameworkPropertyMetadata("New (Unsaved)"));

        public string TabHeader
        {
            get { return (string) GetValue(TabHeaderProperty); }
            set { SetValue(TabHeaderProperty, value); }
        }


        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof (EditorTabContent), typeof (EditorTab),
            new FrameworkPropertyMetadata(default(EditorTabContent)));

        public EditorTabContent Content
        {
            get { return (EditorTabContent) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }



        public string TabName
        {
            get
            {
                return _tabName;
            }
            set
            {
                if (_tabName == value) return;



                var editorTabs = OtherTabs as IList<EditorTab> ?? OtherTabs.ToList();

                var dups = editorTabs.Where(x => !ReferenceEquals(x, this)).Count(x => x.TabName == value);


                string newHeader = value;
                if (dups > 0)
                {
                    newHeader = value + " " + dups;
                    while (editorTabs.Any(x => (x.TabHeader == newHeader) || (x.TabHeader == newHeader + "*")))
                    {
                        dups++;
                        newHeader = value + " " + dups;
                    }
                }

                if (!MemoryOnly && ChangesPending)
                {
                    TabHeader = newHeader + "*";
                }
                else
                {
                    TabHeader = newHeader;
                }

                _tabName = value;
            }
        }


        public ICommand CloseCommand { get; private set; }

        public ICommand CloseAllExceptMeCommand { get; private set; }

        public ICommand CloseAllRightCommand { get; private set; }

        public ICommand CloseAllLeftCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public ICommand SaveAsCommand { get; private set; }

        public ICommand RenameCommand { get; private set; }


        public ICommand OpenFolderCommand { get; private set; }

        public ICommand CopyFullPathCommand { get; private set; }

        public static readonly DependencyProperty ChangesPendingProperty = DependencyProperty.Register(
            "ChangesPending", typeof (bool), typeof (EditorTab), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.None, ChangesPendingPropertyChangedCallback));



        private static void ChangesPendingPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if ((bool) dependencyPropertyChangedEventArgs.NewValue)
            {
                var tab = (EditorTab) dependencyObject;
                tab.TabHeader = tab.TabHeader + "*";
            }
            else
            {
                var tab = (EditorTab)dependencyObject;


                var editorTabs = tab.OtherTabs as IList<EditorTab> ?? tab.OtherTabs.ToList();

                var dups = editorTabs.Count(x => x.TabName == tab.TabName);

                string newHeader = tab.TabName;

                if (dups > 0)
                {
                    newHeader = tab.TabName + " " + dups;
                    while (editorTabs.Any(x => (x.TabHeader == newHeader) || (x.TabHeader == newHeader + "*")))
                    {
                        dups++;
                        newHeader = tab.TabName + " " + dups;
                    }
                }

                
                tab.TabHeader = newHeader;
                
            }
        }



        public bool ChangesPending
        {
            get { return (bool) GetValue(ChangesPendingProperty); }
            set { SetValue(ChangesPendingProperty, value); }
        }


        public static readonly DependencyProperty MemoryOnlyProperty = DependencyProperty.Register(
            "MemoryOnly", typeof (bool), typeof (EditorTab), new FrameworkPropertyMetadata(default(bool)));

        public bool MemoryOnly
        {
            get { return (bool) GetValue(MemoryOnlyProperty); }
            set { SetValue(MemoryOnlyProperty, value); }
        }


        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(
            "FilePath", typeof (string), typeof (EditorTab), new FrameworkPropertyMetadata(default(string)));




        public string FilePath
        {
            get { return (string) GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }


        public string SourceCode
        {
            get { return Content.SourceCode; }
            set { Content.SourceCode = value; }
        }

        public bool IsSelected { get; set; }

        public ObservableCollection<CompilerMessage> CompilerMessages
        {
            get { return Content.CompilerMessages; }
        }

        public LSLDefaultLibraryDataProvider LibraryDataProvider
        {
            get { return (LSLDefaultLibraryDataProvider)Content.LibraryDataProvider; }
            set
            {
                Content.LibraryDataProvider = value;
            }
        }


        public LSLLibraryBaseData BaseLibraryDataCache { get; set; }


        public LSLLibraryDataAdditions LibraryDataAdditionsCache { get; set; }


        private FileSystemWatcher _fileWatcher;



        private void RemoveFileWatcher()
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.Changed -= FileWatcherOnChanged;
                _fileWatcher.Deleted -= FileWatcherOnDeleted;
                _fileWatcher.Renamed -= FileWatcherOnRenamed;
                _fileWatcher = null;
            }
        }



        private void WatchNewFile(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");

            RemoveFileWatcher();

            var directory = Path.GetDirectoryName(fileName);
            var name = Path.GetFileName(fileName);

            if (directory == null || name == null)
            {
                throw new InvalidOperationException("File path not valid");
            }

            _fileWatcher = new FileSystemWatcher(directory, name);
            _fileWatcher.EnableRaisingEvents = true;

            _fileWatcher.Changed += FileWatcherOnChanged;
            _fileWatcher.Deleted += FileWatcherOnDeleted;
            _fileWatcher.Renamed += FileWatcherOnRenamed;
        }



        private bool _fileChanged;
        private bool _fileDeleted;
        private string _tabName;




        public void CheckExternalChanges()
        {
 
            if (_fileDeleted)
            {
                Dispatcher.Invoke(() =>
                {
                    var r =
                        MessageBox.Show(
                            "This file has been deleted outside of the editor, would you like to close this tab?",
                            "File Deleted", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (r == MessageBoxResult.Yes)
                    {
                        ChangesPending = false;
                        Close();
                    }
                    else
                    {
                        _fileWatcher.Changed -= FileWatcherOnChanged;
                        _fileWatcher.Deleted -= FileWatcherOnDeleted;
                        _fileWatcher.Renamed -= FileWatcherOnRenamed;

                        _fileWatcher = null;

                        TabName = Path.GetFileName(FilePath) + " (Old Unsaved)";
                        MemoryOnly = true;
                        FilePath = null;

                        _fileDeleted = false;
                    }
                });
            }
            else if (_fileChanged)
            {
                Dispatcher.Invoke(() =>
                {
                    var r = MessageBox.Show("This file was changed outside of this tab, would you like to reload it?",
                        "File Changed", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (r != MessageBoxResult.Yes)
                    {
                        
                        TabName = Path.GetFileName(FilePath) + " (Old Unsaved)";
                        MemoryOnly = true;
                        FilePath = null;
                        return;
                    }

                    try
                    {
                        OpenFile(FilePath);
                        _fileChanged = false;

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("File could not be loaded: " + e.Message, "Error");
                    }
                });
            }
        }



        private void FileWatcherOnRenamed(object sender, RenamedEventArgs renamedEventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                FilePath = renamedEventArgs.FullPath;

                TabName = renamedEventArgs.Name;
            });
        }



        private void FileWatcherOnDeleted(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            _fileDeleted = true;
            if (IsSelected)
            {
                CheckExternalChanges();
            }
        }



        private void FileWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                using (var md5 = MD5.Create())
                {
                    byte[] source = Encoding.UTF8.GetBytes(SourceCode);
                    var oldHash = md5.ComputeHash(source, 0, source.Length);
                    byte[] newHash;
                    using (var stream = File.OpenRead(FilePath))
                    {
                        newHash = md5.ComputeHash(stream);
                    }

                    if (!oldHash.SequenceEqual(newHash))
                    {
                        _fileChanged = true;
                        if (IsSelected)
                        {
                            CheckExternalChanges();
                        }
                    }
                }
            });


        }



        public bool SaveTabToFile()
        {
            if (MemoryOnly)
            {
                return SaveMemoryOnlyTab();
            }
            return SaveOpenFileTab();
        }



        public bool SaveTabToFileInteractive()
        {
            try
            {
                return this.SaveTabToFile();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Could Not Save File", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }



        public void OpenFile(string fileName)
        {



            Content.SourceCode = File.ReadAllText(fileName);

            TabName = Path.GetFileName(fileName);
            MemoryOnly = false;
            ChangesPending = false;
            FilePath = fileName;

            WatchNewFile(FilePath);
        }



        public bool OpenFileInteractive(string filename)
        {
            try
            {
                OpenFile(filename);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Could Not Open File", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }



        public bool SaveTabToNewFile()
        {

            var saveDialog = new SaveFileDialog
            {
                FileName = "script.lsl",
                DefaultExt = ".lsl",
                Filter = "LSL Script (*.lsl *.txt)|*.lsl;*.txt"
            };

            if (!MemoryOnly)
            {
                saveDialog.FileName = Path.GetFileName(FilePath);
                saveDialog.InitialDirectory = Path.GetDirectoryName(FilePath);
            }

            var showDialog = saveDialog.ShowDialog();
            if (showDialog != null && showDialog.Value)
            {
                File.WriteAllText(saveDialog.FileName, SourceCode);

                TabName = Path.GetFileName(saveDialog.FileName);
                ChangesPending = false;
                MemoryOnly = false;
                FilePath = saveDialog.FileName;
                

                WatchNewFile(FilePath);

                return true;
            }

            return false;
        }



        public bool SaveTabToNewFileInteractive()
        {
            try
            {
                return SaveTabToNewFile();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Could Not Save File", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }



        private bool SaveOpenFileTab()
        {
            try
            {


                if (_fileWatcher != null)
                {
                    _fileWatcher.EnableRaisingEvents = false;
                }
                File.WriteAllText(FilePath, SourceCode);

                if (_fileWatcher != null)
                {
                    _fileWatcher.EnableRaisingEvents = true;
                }

                TabName = Path.GetFileName(FilePath);
                ChangesPending = false;
                MemoryOnly = false;

                return true;
            }
            catch (Exception e)
            {
                if (_fileWatcher != null)
                {
                    _fileWatcher.EnableRaisingEvents = true;
                }

                throw;
            }
        }



        private bool SaveMemoryOnlyTab()
        {
            var saveDialog = new SaveFileDialog
            {
                FileName = "script.lsl",
                DefaultExt = ".lsl",
                Filter = "LSL Script (*.lsl *.txt)|*.lsl;*.txt"
            };


            var showDialog = saveDialog.ShowDialog();
            if (showDialog != null && showDialog.Value)
            {
                File.WriteAllText(saveDialog.FileName, SourceCode);

                TabName = Path.GetFileName(saveDialog.FileName);
                ChangesPending = false;
                MemoryOnly = false;
                FilePath = saveDialog.FileName;
                

                return true;
            }

            return false;
        }



        public EditorTab(TabControl owner, IList<EditorTab> ownerTabCollection, LSLDefaultLibraryDataProvider dataProvider, string sourceCode = "")
        {
            _owner = owner;
            OwnerTabCollection = ownerTabCollection;

            BaseLibraryDataCache = LSLLibraryBaseData.StandardLsl;
            LibraryDataAdditionsCache = LSLLibraryDataAdditions.None;

            Content = new EditorTabContent(this)
            {
                Editor =
                {
                    LibraryDataProvider = dataProvider
                }
            };

            CloseCommand = new RelayCommand(CloseCommandImpl);
            SaveCommand = new RelayCommand(SaveCommandImpl);
            SaveAsCommand = new RelayCommand(SaveAsCommandImpl);
            RenameCommand = new RelayCommand(RenameCommandImpl);
            OpenFolderCommand = new RelayCommand(OpenFolderImpl);
            CopyFullPathCommand = new RelayCommand(CopyFullFilePathImpl);
            CloseAllExceptMeCommand = new RelayCommand(CloseAllExceptMeImpl);
            CloseAllRightCommand = new RelayCommand(CloseAllRightImpl);
            CloseAllLeftCommand = new RelayCommand(CloseAllLeftImpl);


            TabName = "New Script";
            ChangesPending = false;
            MemoryOnly = true;
            FilePath = null;
        }



        private void CloseAllLeftImpl(object o)
        {
            bool deleting = false;
            for (var i = OwnerTabCollection.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(OwnerTabCollection[i], this))
                {
                    deleting = true;
                }
                else if (deleting)
                {
                    OwnerTabCollection[i].Close();
                }
            }
        }



        private void CloseAllRightImpl(object o)
        {
            bool deleting = true;
            for (var i = OwnerTabCollection.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(OwnerTabCollection[i], this))
                {
                    deleting = false;
                }
                else if (deleting)
                {
                    OwnerTabCollection[i].Close();
                }
            }
        }



        private void CloseAllExceptMeImpl(object o)
        {
            foreach (EditorTab t in OwnerTabCollection.ToList())
            {
                if (!ReferenceEquals(t, this))
                {
                    t.Close();
                }
            }
        }



        private void CopyFullFilePathImpl(object o)
        {
            if (MemoryOnly) return;

            Clipboard.SetText(FilePath);
        }



        private void OpenFolderImpl(object o)
        {
            if (MemoryOnly) return;

            var dir = Path.GetDirectoryName(FilePath);
            if (dir == null)
            {
                throw new InvalidOperationException("OpenFolderImpl: Directory null");
            }

            string filePath = FilePath;
            if (!File.Exists(filePath))
            {
                return;
            }

            string argument = @"/select, " + filePath;

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }



        private void RenameCommandImpl(object o)
        {
            if (MemoryOnly) return;

            var input = new InputDialog();
            input.Title = "Rename File";
            input.InputTextBox.Text = TabName;

            input.Closed += (sender, args) =>
            {
                if (input.Accepted)
                {
                    var newName = input.InputTextBox.Text;

                    if (string.IsNullOrWhiteSpace(newName))
                    {
                        MessageBox.Show("A file name must be provided.", "Could Not Rename File", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                        return;
                    }

                    var oldName = Path.GetFileName(FilePath);

                    if (input.InputTextBox.Text != oldName)
                    {
                        var dir = Path.GetDirectoryName(FilePath);

                        if (dir != null)
                        {
                            var newfile = Path.Combine(dir, newName);
                            try
                            {
                                
                                File.Move(FilePath, newfile);
                                TabName = newName;
                                FilePath = newfile;
                                
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message, "Could Not Rename File", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }
                        }
                    }
                }
            };

            input.Show();
        }



        private void SaveAsCommandImpl(object o)
        {
            SaveTabToNewFileInteractive();
        }



        private void SaveCommandImpl(object o)
        {
            SaveTabToFileInteractive();
        }



        public bool Close(bool canCancel = true)
        {
            var lastSelectedIndex = _owner.SelectedIndex;
            int removingIndex = 0;

            for (var i = 0; i < OwnerTabCollection.Count; i++)
            {
                if (ReferenceEquals(OwnerTabCollection[i], this))
                {
                    removingIndex = i;
                    break;
                }
            }

            _owner.SelectedIndex = removingIndex;

            bool canceled = false;

            if (ChangesPending)
            {
                var buttons = canCancel ? MessageBoxButton.YesNoCancel : MessageBoxButton.YesNo;
                MessageBoxResult r;
                if (MemoryOnly)
                {
                    r = MessageBox.Show("Would you like to save this tab to a file before closing?", "Save Changes",
                        buttons, MessageBoxImage.Question);

                    if (r == MessageBoxResult.Yes)
                    {
                        try
                        {
                            while (!SaveTabToNewFile())
                            {
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Could Not Save File", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if (r == MessageBoxResult.Cancel)
                    {
                        canceled = true;
                    }
                }
                else
                {
                    r = MessageBox.Show("Would you like to save this tab before closing?",
                        "Save Changes To \"" + FilePath + "\"",
                        buttons, MessageBoxImage.Question);

                    if (r == MessageBoxResult.Yes)
                    {
                        try
                        {
                            while (!SaveOpenFileTab())
                            {
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Could Not Save File", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if (r == MessageBoxResult.Cancel)
                    {
                        canceled = true;
                    }
                }
            }

           

            if (!canceled)
            {
                if (removingIndex > lastSelectedIndex)
                {
                    _owner.SelectedIndex = lastSelectedIndex;
                }
                else
                {
                    _owner.SelectedIndex = lastSelectedIndex - 1;
                }

                IsSelected = false;
                RemoveFileWatcher();
                OwnerTabCollection.Remove(this);
            }

            return !canceled;
        }



        private void CloseCommandImpl(object o)
        {
            Close();
        }
    }
}