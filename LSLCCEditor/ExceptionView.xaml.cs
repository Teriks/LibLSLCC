#region FileInfo

// 
// File: ExceptionView.xaml.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2016, Teriks
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
using System.IO;
using System.Windows;
using Microsoft.Win32;

#endregion

namespace LSLCCEditor
{
    /// <summary>
    ///     Interaction logic for ExceptionView.xaml
    /// </summary>
    public partial class ExceptionView : Window
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof (string), typeof (ExceptionView), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty StackTraceProperty = DependencyProperty.Register(
            "StackTrace", typeof (string), typeof (ExceptionView), new PropertyMetadata(default(string)));

        private static readonly string TwoNewLines = Environment.NewLine + Environment.NewLine;


        public ExceptionView(Exception error)
        {
            InitializeComponent();
            Exception = error;
            Message = error.Message;
            StackTrace = error.StackTrace;

            Loaded += ThisOnLoaded;
        }


        public string Message
        {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string StackTrace
        {
            get { return (string) GetValue(StackTraceProperty); }
            set { SetValue(StackTraceProperty, value); }
        }

        public Exception Exception { get; set; }


        private void ThisOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            System.Media.SystemSounds.Hand.Play();
        }


        private void Okay_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            tryAgain:

            var saveFile = new SaveFileDialog
            {
                CreatePrompt = true,
                OverwritePrompt = true,
                FileName = "error_report.txt",
                AddExtension = true,
                DefaultExt = ".txt",
                Filter = "Text File (*.txt) | *.txt"
            };


            var r = saveFile.ShowDialog(this);

            if (!r.Value) return;

            try
            {
                var inner = Exception.InnerException;

                var innerText = "";
                var i = 1;
                while (inner != null)
                {
                    innerText +=
                        TwoNewLines +
                        "====================================================================="
                        + TwoNewLines + string.Format("Inner Exception Type {0}:", i) + TwoNewLines
                        + inner.GetType().AssemblyQualifiedName + TwoNewLines
                        + string.Format("Inner Message {0}:", i) + TwoNewLines
                        + inner.Message
                        + TwoNewLines
                        + string.Format("Inner Stack Trace {0}:", i)
                        + TwoNewLines
                        + inner.StackTrace
                        ;

                    inner = inner.InnerException;
                    i++;
                }

                File.WriteAllText(saveFile.FileName,
                    "Exception Type:" + TwoNewLines
                    + Exception.GetType().AssemblyQualifiedName
                    + TwoNewLines
                    + "Message:" + TwoNewLines
                    + Message
                    + TwoNewLines
                    + "Stack Trace:"
                    + TwoNewLines
                    + StackTrace + innerText);
            }
            catch (Exception err)
            {
                var result = MessageBox.Show(this,
                    "Could not save file, reason:" + TwoNewLines + err.Message +
                    TwoNewLines + "Try Again?", "Error saving file",
                    MessageBoxButton.YesNo, MessageBoxImage.Error);

                if (result == MessageBoxResult.Yes)
                {
                    goto tryAgain;
                }
            }
        }
    }
}