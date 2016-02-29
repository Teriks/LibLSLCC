#region FileInfo
// 
// File: ImportExportTools.cs
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

using System;
using System.Security;
using System.Text;
using System.Windows;
using System.Xml;
using Microsoft.Win32;

namespace LSLCCEditor.SettingsUI
{
    internal static class ImportExportTools
    {
        public static void DoImportSettingsWindow(Window owner, string fileFilter, string fileExt, Action<XmlReader> serialize)
        {
            var openDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = fileFilter,
                AddExtension = true,
                DefaultExt = fileExt
            };
            if (!openDialog.ShowDialog(owner).Value)
            {
                return;
            }

            var settings = new XmlReaderSettings()
            {
                CloseInput = true,
                IgnoreWhitespace = false
            };

            try
            {
                using (var file = XmlReader.Create(openDialog.OpenFile(), settings))
                {
                    serialize(file);
                }
            }
            catch (XmlSyntaxException ex)
            {
                MessageBox.Show(owner, "An XML syntax error was encountered while loading the file, settings could not be applied: "
                                + Environment.NewLine + Environment.NewLine + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "There was an unknown error while loading the settings file, settings could not be applied: "
                                + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }


        public static void DoExportSettingsWindow(Window owner, string fileFilter, string fileName, Action<XmlWriter> serialize)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = fileFilter,
                FileName = fileName
            };


            if (!saveDialog.ShowDialog(owner).Value)
            {
                return;
            }

            var settings = new XmlWriterSettings()
            {
                CloseOutput = true,
                Encoding = Encoding.Unicode,
                Indent = true,
                NewLineHandling = NewLineHandling.Entitize
            };

            try
            {
                using (var file = XmlWriter.Create(saveDialog.OpenFile(), settings))
                {
                    serialize(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "An unexpected problem occurred while trying to save the file: "
                                + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }
    }
}
