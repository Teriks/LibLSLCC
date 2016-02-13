#region FileInfo
// 
// File: SettingsManager.cs
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
using System.Xml;
using System.Xml.Serialization;

namespace LibLSLCC.Settings
{
    public enum SettingsErrorType
    {
        FileMissing,
        FileUnreadable,
        SyntaxError
    }

    public sealed class SettingsManagerConfigErrorEventArgs : EventArgs
    {
        public SettingsManagerConfigErrorEventArgs(SettingsErrorType errorType, bool settingsReset)
        {
            ErrorType = errorType;
            SettingsReset = settingsReset;
        }

        public bool SettingsReset { get; private set; }

        public SettingsErrorType ErrorType { get; private set; }
    }


    public sealed class SettingsManager<T> where T : new()
    {
        private readonly bool _resetOnConfigError;
        public T Settings { get; private set; }


        public event EventHandler<SettingsManagerConfigErrorEventArgs> ConfigError;


        public SettingsManager(bool resetSettingsOnConfigError = true)
        {
            _resetOnConfigError = resetSettingsOnConfigError;
        }


        public void Save(string file)
        {
            var serializer = new XmlSerializer(typeof (T));

            var writerSettings = new XmlWriterSettings
            {
                Indent = true,
                NewLineHandling = NewLineHandling.Entitize,
                CloseOutput = true
            };

            using (var writer = XmlWriter.Create(File.Create(file), writerSettings))
            {
                serializer.Serialize(writer, Settings);
            }
        }


        public void ApplyDefaults()
        {
            Settings = new T();

            DefaultValueInitializer.Init(Settings);
        }

        private void HandleLoadError(SettingsErrorType type)
        {
            if (_resetOnConfigError)
            {
                ApplyDefaults();
            }

            OnConfigError(type);
        }

        public void Load(string file)
        {
            if (File.Exists(file))
            {
                var serializer = new XmlSerializer(typeof (T));

                try
                {
                    using (var reader = new XmlTextReader(File.OpenRead(file)))
                    {
                        reader.WhitespaceHandling = WhitespaceHandling.All;
                        Settings = (T) serializer.Deserialize(reader);
                    }
                }
                catch (IOException)
                {
                    HandleLoadError(SettingsErrorType.FileUnreadable);
                }
                catch (Exception)
                {
                    HandleLoadError(SettingsErrorType.SyntaxError);
                }

                DefaultValueInitializer.Init(Settings);
            }
            else
            {
                HandleLoadError(SettingsErrorType.FileMissing);
            }
        }


        private void OnConfigError(SettingsErrorType type)
        {
            var handler = ConfigError;
            if (handler != null) handler(this, new SettingsManagerConfigErrorEventArgs(type, _resetOnConfigError));
        }
    }
}