using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator.Exceptions;

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    /// Represents a description for a named library subset.
    /// </summary>
    public class LSLLibrarySubsetDescription : IXmlSerializable
    {
        private string _subset;

        /// <summary>
        /// Construct a description for a subset by giving the subset name, and a user friendly name associated with the subset.
        /// Optionally you can provide a description string describing the subset.
        /// </summary>
        /// <param name="subsetName">The subset name.</param>
        /// <param name="friendlyName">A user friendly name for the subset.</param>
        /// <param name="description">Optional description string.</param>
        /// <exception cref="LSLInvalidSubsetNameException">If the given subset name does not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*).</exception>
        public LSLLibrarySubsetDescription(string subsetName, string friendlyName, string description = "")
        {
            Subset = subsetName;
            FriendlyName = friendlyName;
            Description = description;
        }



        private LSLLibrarySubsetDescription()
        {
            Description = "";
        }


        /// <summary>
        /// The name of the subset this subset description contains information about.
        /// </summary>
        /// <exception cref="LSLInvalidSubsetNameException">If the given subset name does not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*).</exception>
        public string Subset
        {
            get { return _subset; }
            private set
            {
                LSLLibraryDataSubsetNameParser.ThrowIfInvalid(value);
                _subset = value;
            }
        }

        /// <summary>
        /// The friendly name to associate with the subset.
        /// </summary>
        public string FriendlyName { get; private set; }

        /// <summary>
        /// The description string for the subset.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Implementors of IXmlSerializable should return null from this function.
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }



        /// <summary>
        /// Reads a library subset description object from an XML fragment.
        /// </summary>
        /// <param name="fragment">The XML reader containing the fragment to read.</param>
        /// <returns>The parsed LSLLibrarySubsetDescription object.</returns>
        public static LSLLibrarySubsetDescription FromXmlFragment(XmlReader fragment)
        {
            var ev = new LSLLibrarySubsetDescription();
            IXmlSerializable x = ev;
            x.ReadXml(fragment);
            return ev;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var lineNumberInfo = (IXmlLineInfo)reader;

            reader.MoveToContent();

            bool hasSubset = false;
            bool hasFriendlyName = false;

            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "Subset")
                {
                    Subset = reader.Value;
                    hasSubset = true;
                }
                else if (reader.Name == "FriendlyName")
                {
                    FriendlyName = reader.Value;
                    hasFriendlyName = true;
                }
                else
                {
                    throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                        string.Format("LibrarySubsetDescription{0}: Unknown attribute '{1}'.",
                        hasSubset ? (" '" + Subset + "'") : "", reader.Name));
                }
            }

            if (!hasSubset || string.IsNullOrWhiteSpace(Subset))
            {
                throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber, 
                    string.Format("LibrarySubsetDescription '{0}': Missing Subset Attribute.", Subset));
            }

            if (!hasFriendlyName || string.IsNullOrWhiteSpace(FriendlyName))
            {
                throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber, 
                    string.Format("LibrarySubsetDescription '{0}': Missing FriendlyName Attribute.", Subset));
            }

            bool canRead = reader.Read();
            while (canRead)
            {
                if (reader.Name == "Description" && reader.IsStartElement())
                {
                    Description = reader.ReadElementContentAsString();
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "SubsetDescription")
                {
                    break;
                }
                else
                {
                    canRead = reader.Read();
                }
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                Description = "";
            }
        }


        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Subset", Subset);
            writer.WriteAttributeString("FriendlyName", FriendlyName);
            writer.WriteStartElement("Description");
            writer.WriteString(Description);
            writer.WriteEndElement();
        }
    }
}
