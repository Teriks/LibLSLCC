using System;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LSLCCEditor.EditControl
{
    [Serializable]
    [XmlRoot(ElementName = "Brush")]
    public class XmlBrush : IXmlSerializable
    {

        [XmlIgnore]
        public Brush Brush { get; private set; }

        public XmlSchema GetSchema()
        {
            throw new System.NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var color = reader.ReadContentAsString();

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(color);

            Brush = (Brush)XamlReader.Load(new XmlNodeReader(doc));
        }

        public void WriteXml(XmlWriter writer)
        {

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            settings.ConformanceLevel = ConformanceLevel.Fragment;

            XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(writer);
            manager.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(Brush, manager);
        }

        public XmlBrush()
        {
        }

        public XmlBrush(Brush brush)
        {
            Brush = brush;
        }

        public static implicit operator 
            XmlBrush(Brush brush)
        {
            return new XmlBrush(brush);
        }

        public static implicit operator
            Brush(XmlBrush xmlBrush)
        {
            return xmlBrush.Brush;
        }
    }
}