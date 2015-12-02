using System.Windows.Media;
using System.Xml.Serialization;

namespace LSLCCEditor.Utility.Xml
{
    [XmlRoot(ElementName = "Brush")]
    public class XmlBrush : XmlSerializableXaml<Brush>
    {
        public XmlBrush()
        {
        }

        public XmlBrush(Brush content) : base(content)
        {
        }

        public static implicit operator XmlBrush(Brush brush)
        {
            return new XmlBrush(brush);
        }

        public static implicit operator
            Brush(XmlBrush xmlBrush)
        {
            return xmlBrush.Content;
        }
    }
}