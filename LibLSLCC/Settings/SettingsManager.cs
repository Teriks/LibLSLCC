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

#region Imports

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace LibLSLCC.Settings
{
    /// <summary>
    ///     Manages loading a serializable settings object to and from disk.  <see cref="SettingsBaseClass{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SettingsManager<T> where T : new()
    {
        /// <summary>
        ///     Create a <see cref="SettingsManager{T}" /> around an initial settings object.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="settings" /> is <c>null</c>.</exception>
        public SettingsManager(T settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            Settings = settings;
        }


        /// <summary>
        ///     Create a <see cref="SettingsManager{T}" /> with no initial settings object.
        /// </summary>
        public SettingsManager()
        {
        }


        /// <summary>
        ///     The settings object being managed.
        /// </summary>
        public T Settings { get; private set; }


        /// <summary>
        ///     Save the settings object to disk.
        /// </summary>
        /// <param name="file">The name of the file name to save to.</param>
        /// <exception cref="UnauthorizedAccessException">
        ///     The caller does not have the required permission.-or-
        ///     <paramref name="file" /> specified a file that is read-only.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="IOException">An I/O error occurred while creating the file. </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="file" /> is a zero-length string, contains only white space, or
        ///     contains one or more invalid characters as defined by <see cref="System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="file" /> is <c>null</c>. </exception>
        /// <exception cref="PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length.
        ///     For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than
        ///     260 characters.
        /// </exception>
        /// <exception cref="NotSupportedException"><paramref name="file" /> is in an invalid path format. </exception>
        /// <exception cref="InvalidOperationException">
        ///     An error occurred during serialization. The original exception is available
        ///     using the <see cref="P:System.Exception.InnerException" /> property.
        /// </exception>
        public void Save(string file)
        {
            var serializer = new XmlSerializer(typeof (T));

            var writerSettings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.Unicode,
                NewLineHandling = NewLineHandling.Entitize,
                CloseOutput = true,
            };

            using (var writer = XmlWriter.Create(File.Create(file), writerSettings))
            {
                serializer.Serialize(writer, Settings);
            }
        }


        /// <summary>
        ///     Save the settings object to a stream.
        /// </summary>
        /// <param name="stream">The stream to save the settings to.</param>
        /// <param name="closeOutput"><c>true</c> if <paramref name="stream"/> should be closed by this function, default is <c>false</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="stream" /> value is null.</exception>
        public void Save(Stream stream, bool closeOutput = false)
        {
            var serializer = new XmlSerializer(typeof(T));

            var writerSettings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.Unicode,
                NewLineHandling = NewLineHandling.Entitize,
                CloseOutput = closeOutput
            };

            using (var writer = XmlWriter.Create(stream, writerSettings))
            {
                serializer.Serialize(writer, Settings);
            }
        }


        /// <summary>
        ///     Reset all settings properties to default in the settings managed settings object.
        /// </summary>
        public void ApplyDefaults()
        {
            Settings = new T();

            DefaultValueInitializer.Init(Settings);
        }


        /// <summary>
        ///     Load the settings object from the specified file.
        /// </summary>
        /// <param name="file">The file to load the managed settings object from.</param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="file" /> is a zero-length string, contains only white space, or
        ///     contains one or more invalid characters as defined by <see cref="System.IO.Path.InvalidPathChars" />.
        /// </exception>
        /// <exception cref="NotSupportedException"><paramref name="file" /> is in an invalid path format. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="file" /> is <c>null</c>. </exception>
        /// <exception cref="PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length.
        ///     For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than
        ///     260 characters.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="file" /> was not found. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive). </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     <paramref name="file" /> specified a directory.-or- The caller does not
        ///     have the required permission.
        /// </exception>
        /// <exception cref="InvalidOperationException">An error occurred during deserialization. The original exception is available using the <see cref="P:System.Exception.InnerException" /> property. </exception>
        public void Load(string file)
        {
            var serializer = new XmlSerializer(typeof (T));

            var settings = new XmlReaderSettings()
            {
                IgnoreWhitespace = false,
                CloseInput = true
            };

            using (var reader = XmlReader.Create(File.OpenRead(file), settings))
            {
                Settings = (T) serializer.Deserialize(reader);
            }

            DefaultValueInitializer.Init(Settings);
        }


        /// <summary>
        ///     Load the settings object from the specified file.
        /// </summary>
        /// <param name="input">The stream to read the managed settings object from.</param>
        /// <param name="closeInput">whether or not this function should close <paramref name="input"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="input" /> is null. </exception>
        /// <exception cref="InvalidOperationException">An error occurred during deserialization. The original exception is available using the <see cref="P:System.Exception.InnerException" /> property. </exception>
        public void Load(Stream input, bool closeInput = false)
        {
            var serializer = new XmlSerializer(typeof(T));

            var settings = new XmlReaderSettings()
            {
                IgnoreWhitespace = false,
                CloseInput = closeInput
            };

            using (var reader = XmlReader.Create(input, settings))
            {
                Settings = (T)serializer.Deserialize(reader);
            }

            DefaultValueInitializer.Init(Settings);
        }
    }
}