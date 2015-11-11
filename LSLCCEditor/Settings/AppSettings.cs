#region FileInfo
// 
// File: AppSettings.cs
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
using System.IO;
using System.Windows.Forms;
using LibLSLCC.Settings;

namespace LSLCCEditor.Settings
{

 

    public static class AppSettings
    {
        private static readonly SettingsManager<AppSettingsNode> SettingsManager =
            new SettingsManager<AppSettingsNode>();


        public static AppSettingsNode Settings { get { return SettingsManager.Settings; } }


        public static string AppDataDir { get; private set; }

        public static string SettingsFile { get; private set; }



        static AppSettings()
        {
            AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LibLSLCC", "LSLCCEditor");

            Directory.CreateDirectory(AppDataDir);

            SettingsFile = Path.Combine(AppDataDir, "settings.xml");


            SettingsManager.ConfigError += SettingsManagerOnConfigError;

        }

        private static void SettingsManagerOnConfigError(object sender, SettingsManagerConfigErrorEventArgs settingsManagerConfigErrorEventArgs)
        {
            switch (settingsManagerConfigErrorEventArgs.ErrorType)
            {
                case SettingsErrorType.SyntaxError:
                    MessageBox.Show(
                        "There was a problem with the application settings, defaults have been re-applied",
                        "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


                    Save();
                    break;

                case SettingsErrorType.FileUnreadable:

                    MessageBox.Show(
                        "There was a problem reading the application settings file, it may be locked by another application.",
                        "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Application.Exit();
                    break;

                case SettingsErrorType.FileMissing:

                    Save();
                    break;
            }
        }


        public static void Save()
        {
            SettingsManager.Save(SettingsFile);
        }


        public static void Load()
        {
            SettingsManager.Load(SettingsFile);
        }
    }
}
