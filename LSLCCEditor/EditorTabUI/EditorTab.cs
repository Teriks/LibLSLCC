﻿#region FileInfo
// 
// File: EditorTab.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibLSLCC.LibraryData;
using LSLCCEditor.Utility.Binding;
using Microsoft.Win32;

#endregion

// ReSharper disable LocalizableElement

namespace LSLCCEditor.EditorTabUI
{
    public class EditorTab : DependencyObject
    {
        public static readonly DependencyProperty TabHeaderProperty = DependencyProperty.Register(
            "TabHeader", typeof (string), typeof (EditorTab), new FrameworkPropertyMetadata("New (Unsaved)"));

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof (EditorTabContent), typeof (EditorTab),
            new FrameworkPropertyMetadata(default(EditorTabContent)));

        public static readonly DependencyProperty ChangesPendingProperty = DependencyProperty.Register(
            "ChangesPending", typeof (bool), typeof (EditorTab),
            new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.None,
                ChangesPendingPropertyChangedCallback));

        public static readonly DependencyProperty MemoryOnlyProperty = DependencyProperty.Register(
            "MemoryOnly", typeof (bool), typeof (EditorTab), new FrameworkPropertyMetadata(default(bool)));

        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(
            "FilePath", typeof (string), typeof (EditorTab), new FrameworkPropertyMetadata(default(string)));

        private readonly TabControl _owner;
        private bool _fileChanged;
        private bool _fileDeleted;
        private FileSystemWatcher _fileWatcher;
        private string _tabName;

        public EditorTab(TabControl owner, IList<EditorTab> ownerTabCollection, LSLLibraryDataProvider dataProvider)
        {
            _owner = owner;
            OwnerTabCollection = ownerTabCollection;

            ActiveLibraryDataSubsetsCache = new HashSet<string>(dataProvider.ActiveSubsets);

            


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



        public IList<EditorTab> OwnerTabCollection { get; private set; }

        public IEnumerable<EditorTab> OtherTabs
        {
            get { return OwnerTabCollection.Where(x => !ReferenceEquals(x, this)); }
        }

        public string TabHeader
        {
            get { return (string) GetValue(TabHeaderProperty); }
            set { SetValue(TabHeaderProperty, value); }
        }

        public EditorTabContent Content
        {
            get { return (EditorTabContent) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public string TabName
        {
            get { return _tabName; }
            set
            {
                if (_tabName == value) return;


                var editorTabs = OtherTabs as IList<EditorTab> ?? OtherTabs.ToList();

                var dups = editorTabs.Where(x => !ReferenceEquals(x, this)).Count(x => x.TabName == value);


                var newHeader = value;
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

        public bool ChangesPending
        {
            get { return (bool) GetValue(ChangesPendingProperty); }
            set { SetValue(ChangesPendingProperty, value); }
        }

        public bool MemoryOnly
        {
            get { return (bool) GetValue(MemoryOnlyProperty); }
            set { SetValue(MemoryOnlyProperty, value); }
        }

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



        public HashSet<string> ActiveLibraryDataSubsetsCache { get; private set; } 


        private static void ChangesPendingPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if ((bool) dependencyPropertyChangedEventArgs.NewValue)
            {
                var tab = (EditorTab) dependencyObject;
                tab.TabHeader = tab.TabHeader + "*";
            }
            else
            {
                var tab = (EditorTab) dependencyObject;


                var editorTabs = tab.OtherTabs as IList<EditorTab> ?? tab.OtherTabs.ToList();

                var dups = editorTabs.Count(x => x.TabName == tab.TabName);

                var newHeader = tab.TabName;

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

        public void CheckExternalChanges()
        {
            if (_fileDeleted)
            {
                Dispatcher.Invoke(() =>
                {
                    var r =
                        MessageBox.Show(Application.Current.MainWindow,
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
                    var r = MessageBox.Show(Application.Current.MainWindow, 
                        "This file was changed outside of this tab, would you like to reload it?",
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
                        MessageBox.Show(Application.Current.MainWindow, 
                            "File could not be loaded: " + e.Message, "Error");
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
                    var source = Encoding.UTF8.GetBytes(SourceCode);
                    var oldHash = md5.ComputeHash(source, 0, source.Length);
                    byte[] newHash;

                    try
                    {
                        using (var stream = File.OpenRead(FilePath))
                        {
                            newHash = md5.ComputeHash(stream);
                        }
                    }
                    catch
                    {
                        return;
                        // ignored
                    }

                    if (oldHash.SequenceEqual(newHash)) return;

                    _fileChanged = true;
                    if (IsSelected)
                    {
                        CheckExternalChanges();
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
                return SaveTabToFile();
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, 
                    e.Message, "Could Not Save File", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }


        private void OpenFile(string fileName)
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
                MessageBox.Show(Application.Current.MainWindow, 
                    e.Message, "Could Not Open File", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }


        private bool SaveTabToNewFile()
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

            if (showDialog == null || !showDialog.Value) return false;


            File.WriteAllText(saveDialog.FileName, SourceCode);

            TabName = Path.GetFileName(saveDialog.FileName);
            ChangesPending = false;
            MemoryOnly = false;
            FilePath = saveDialog.FileName;


            WatchNewFile(FilePath);

            return true;
        }

        public bool SaveTabToNewFileInteractive()
        {
            try
            {
                return SaveTabToNewFile();
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, 
                    e.Message, "Could Not Save File", MessageBoxButton.OK, MessageBoxImage.Error);
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
            catch (Exception)
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

        private void CloseAllLeftImpl(object o)
        {
            var deleting = false;
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
            var deleting = true;
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
            foreach (var t in OwnerTabCollection.ToList())
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
                throw new InvalidOperationException("EditorTab.OpenFolderImpl: Directory null");
            }

            var filePath = FilePath;
            if (!File.Exists(filePath))
            {
                return;
            }

            var argument = @"/select, " + filePath;

            Process.Start("explorer.exe", argument);
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
                        MessageBox.Show(Application.Current.MainWindow, 
                            "A file name must be provided.", "Could Not Rename File", MessageBoxButton.OK,
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
                                MessageBox.Show(Application.Current.MainWindow,
                                    e.Message, "Could Not Rename File", MessageBoxButton.OK,
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
            var removingIndex = 0;

            for (var i = 0; i < OwnerTabCollection.Count; i++)
            {
                if (ReferenceEquals(OwnerTabCollection[i], this))
                {
                    removingIndex = i;
                    break;
                }
            }

            _owner.SelectedIndex = removingIndex;

            var canceled = false;

            if (ChangesPending)
            {
                var buttons = canCancel ? MessageBoxButton.YesNoCancel : MessageBoxButton.YesNo;
                MessageBoxResult r;
                if (MemoryOnly)
                {
                    r = MessageBox.Show(Application.Current.MainWindow, 
                        "Would you like to save this tab to a file before closing?", "Save Changes",
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
                            MessageBox.Show(Application.Current.MainWindow,
                                e.Message, "Could Not Save File", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if (r == MessageBoxResult.Cancel)
                    {
                        canceled = true;
                    }
                }
                else
                {
                    r = MessageBox.Show(Application.Current.MainWindow, 
                        "Would you like to save this tab before closing?",
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
                            MessageBox.Show(Application.Current.MainWindow, 
                                e.Message, "Could Not Save File", MessageBoxButton.OK, MessageBoxImage.Error);
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