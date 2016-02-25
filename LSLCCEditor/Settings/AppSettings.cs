#region FileInfo

// 
// File: AppSettings.cs
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
using System.Windows.Forms;
using LibLSLCC.Settings;

#endregion

namespace LSLCCEditor.Settings
{
    public static class AppSettings
    {
        private const string CurrentSettingsVersion = "{FB178F2E-D3E1-4545-B90A-193094B02F64}";

        private static readonly SettingsManager<AppSettingsNode> SettingsManager =
            new SettingsManager<AppSettingsNode>();

        private static string _appDataDir;
        private static string _settingsFile;
        private static string _settingsVersionFile;

        public static AppSettingsNode Settings
        {
            get { return SettingsManager.Settings; }
        }

        public static string AppDataDir
        {
            get
            {
                return _appDataDir ??
                       (_appDataDir =
                           Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LibLSLCC",
                               "LSLCCEditor"));
            }
        }

        public static string SettingsFile
        {
            get { return _settingsFile ?? (_settingsFile = Path.Combine(AppDataDir, "settings.xml")); }
        }

        public static string SettingsVersionFile
        {
            get { return _settingsVersionFile ?? (_settingsVersionFile = Path.Combine(AppDataDir, "settings.ver")); }
        }


        public static void Save()
        {
            SettingsManager.Save(SettingsFile);
        }


        public static void Load()
        {
            try
            {
                Directory.CreateDirectory(AppDataDir);
            }
                // ReSharper disable once CatchAllClause
            catch (Exception e)
            {
                MessageBox.Show(
                    "The application settings directory could not be created." + Environment.NewLine +
                    "The application will now exit."
                    + Environment.NewLine
                    + Environment.NewLine + "Error: " + e.Message,
                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Environment.Exit(1);
            }


            try
            {
                if (!File.Exists(SettingsVersionFile))
                {
                    File.WriteAllText(SettingsVersionFile, CurrentSettingsVersion);
                }
                else
                {
                    var versionCheck = File.ReadAllText(SettingsVersionFile);
                    if (versionCheck != CurrentSettingsVersion)
                    {
                        var r = MessageBox.Show(
                            "A version change in the application settings has been detected, default settings will be applied over your old settings."
                            + Environment.NewLine +
                            "Click cancel if you do not want this to happen and would just like to exit for now instead.", "Settings Version Change",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                        if (r == DialogResult.Cancel)
                        {
                            Environment.Exit(1);
                        }

                        if (File.Exists(SettingsFile))
                        {
                            File.Delete(SettingsFile);
                        }

                        File.WriteAllText(SettingsVersionFile, CurrentSettingsVersion);
                    }
                }
            }
                // ReSharper disable once CatchAllClause
            catch (Exception e)
            {
                MessageBox.Show(
                    "There was a problem reading/writing the application settings files, " +
                    "please make sure they are not locked by another application.  "
                    + "The application will now exit."
                    + Environment.NewLine
                    + Environment.NewLine + "Error: " + e.Message, "Settings Read/Write Error",
                    MessageBoxButtons.OK);

                Environment.Exit(1);
            }


            try
            {
                SettingsManager.Load(SettingsFile);
            }

            catch (FileNotFoundException)
            {
                MessageBox.Show(
                    "There was a problem with the application settings, the application settings file could not be found."
                    + Environment.NewLine + Environment.NewLine +
                    "The file will be created and default settings will be applied.",
                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                SettingsManager.ApplyDefaults();
                Save();
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show(
                    "There was a problem with the application settings, the applications data directory could not be found."
                    + Environment.NewLine + Environment.NewLine +
                    "The directory will be created and default settings will be applied.",
                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    Directory.CreateDirectory(AppDataDir);
                }
                    // ReSharper disable once CatchAllClause
                catch (Exception e)
                {
                    MessageBox.Show(
                        "The application settings directory could not be created." + Environment.NewLine +
                        "The application will now exit." + Environment.NewLine + Environment.NewLine +
                        "Error: " + e.Message,
                        "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Environment.Exit(1);
                }

                SettingsManager.ApplyDefaults();
                Save();
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "There was a problem reading the application settings file, it may be locked by another application." +
                    Environment.NewLine +
                    "The application will now exit.",
                    "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Environment.Exit(1);
            }
                // ReSharper disable once CatchAllClause
            catch (Exception)
            {
                var r = MessageBox.Show(
                    "There was a problem/syntax error with the application settings, default settings will be re-applied."
                    + Environment.NewLine + Environment.NewLine +
                    "Click cancel if you do not want this to happen and would just like to exit for now instead.",
                    "Configuration Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);


                if (r == DialogResult.Cancel)
                {
                    Environment.Exit(1);
                }

                SettingsManager.ApplyDefaults();
                Save();
            }
        }
    }
}