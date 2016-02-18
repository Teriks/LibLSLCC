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
    /// <summary>
    /// Describes a settings read error type.
    /// </summary>
    public enum SettingsErrorType
    {
        /// <summary>
        /// Settings file not found.
        /// </summary>
        FileMissing,

        /// <summary>
        /// Settings file could not be read.
        /// </summary>
        FileUnreadable,

        /// <summary>
        /// Syntax error in the settings file.
        /// </summary>
        SyntaxError
    }

    /// <summary>
    /// Event arguments for the settings manager ConfigError event.  <see cref="SettingsManager{T}.ConfigError"/>
    /// </summary>
    public sealed class SettingsManagerConfigErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Construct the config error event args.
        /// </summary>
        /// <param name="errorType">The error type.</param>
        /// <param name="settingsReset">Whether or not the settings were reset to default.</param>
        public SettingsManagerConfigErrorEventArgs(SettingsErrorType errorType, bool settingsReset)
        {
            ErrorType = errorType;
            SettingsReset = settingsReset;
        }

        /// <summary>
        /// Whether or not the settings were reset to default.
        /// </summary>
        public bool SettingsReset { get; private set; }

        /// <summary>
        /// The error type.
        /// </summary>
        public SettingsErrorType ErrorType { get; private set; }
    }


    /// <summary>
    /// Manages loading a serializable settings object to and from disk.  <see cref="SettingsBaseClass{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SettingsManager<T> where T : new()
    {
        private readonly bool _resetOnConfigError;

        /// <summary>
        /// The settings object being managed.
        /// </summary>
        public T Settings { get; private set; }


        /// <summary>
        /// Occurs when there is an error reading the settings object off disk.
        /// </summary>
        public event EventHandler<SettingsManagerConfigErrorEventArgs> ConfigError;


        /// <summary>
        /// Construct the settings manager.
        /// </summary>
        /// <param name="resetSettingsOnConfigError">Whether or not settings errors reset the settings object to have default property values.</param>
        public SettingsManager(bool resetSettingsOnConfigError = true)
        {
            _resetOnConfigError = resetSettingsOnConfigError;
        }


        /// <summary>
        /// Save the settings object to disk.
        /// </summary>
        /// <param name="file">The name of the file name to save to.</param>
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


        /// <summary>
        /// Reset all settings properties to default in the settings managed settings object.
        /// </summary>
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

        /// <summary>
        /// Load the settings object from the specified file.
        /// </summary>
        /// <param name="file">The file to load the managed settings object from.</param>
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