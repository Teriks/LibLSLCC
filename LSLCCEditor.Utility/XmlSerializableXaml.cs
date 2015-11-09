using System;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LSLCCEditor.Utility
{
    [Serializable]
    public class XmlSerializableXaml<T> : IXmlSerializable
    {

        [XmlIgnore]
        public T Content { get; private set; }

        public XmlSchema GetSchema()
        {
            throw new System.NotImplementedException();
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