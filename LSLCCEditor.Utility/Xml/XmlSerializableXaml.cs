#region FileInfo
// 
// File: XmlSerializableXaml.cs
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
using System.Windows.Markup;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibLSLCC.Settings;

namespace LSLCCEditor.Utility.Xml
{
    [Serializable]
    public class XmlSerializableXaml<T> : SettingsBaseClass<XmlSerializableXaml<T>>, IXmlSerializable
    {
        private T _content;

        [XmlIgnore]
        public T Content
        {
            get { return _content; }
            set { SetField(ref _content,value, "Content"); }
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Content = (T)XamlReader.Parse(reader.ReadInnerXml());
        }

        public void WriteXml(XmlWriter writer)
        {

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            settings.ConformanceLevel = ConformanceLevel.Fragment;

            XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(writer);
            manager.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(Content, manager);
        }

        public XmlSerializableXaml()
        {
        }

        public XmlSerializableXaml(T content)
        {
            Content = content;
        }

        public static implicit operator
            XmlSerializableXaml<T>(T content)
        {
            return new XmlSerializableXaml<T>(content);
        }

        public static implicit operator
            T(XmlSerializableXaml<T> xmlBrush)
        {
            return xmlBrush.Content;
        }
    }
}