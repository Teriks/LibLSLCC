using System.Windows.Media;
using System.Xml.Serialization;

namespace LSLCCEditor.Utility
{
    [XmlRoot(ElementName = "Color")]
    public class XmlColor : XmlSerializableXaml<Color>
    {
        public XmlColor()
        {
        }

        public XmlColor(Color content) : base(content)
        {
        }

        public static implicit operator XmlColor(Color color)
        {
            return new XmlColor(color);
        }

        public static implicit operator Color(XmlColor xmlBrush)
        {
            return xmlBrush.Content;
        }
    }
}