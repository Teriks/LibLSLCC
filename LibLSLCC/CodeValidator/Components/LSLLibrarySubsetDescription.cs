﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    /// Represents a description for a named library subset.
    /// </summary>
    public class LSLLibrarySubsetDescription : IXmlSerializable
    {
        /// <summary>
        /// Construct a description for a subset by giving the subset name, and a user friendly name associated with the subset.
        /// Optionally you can provide a description string describing the subset.
        /// </summary>
        /// <param name="subsetName">The subset name.</param>
        /// <param name="friendlyName">A user friendly name for the subset.</param>
        /// <param name="description">Optional description string.</param>
        public LSLLibrarySubsetDescription(string subsetName, string friendlyName, string description = "")
        {
            Subset = subsetName;
            FriendlyName = friendlyName;
            Description = description;
        }


        /// <summary>
        /// Construct an empty LSLLibraryDataSubsetDescription()
        /// </summary>
        protected LSLLibrarySubsetDescription()
        {
            
        }


        /// <summary>
        /// The name of the subset this subset description contains information about.
        /// </summary>
        public string Subset { get; set; }

        /// <summary>
        /// The friendly name to associate with the subset.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// The description string for the subset.
        /// </summary>
        public string Description { get; set; }

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
                    throw new XmlSyntaxException(lineNumberInfo.LineNumber, GetType().Name + ", unknown attribute " + reader.Name);
                }
            }

            if (!hasSubset || string.IsNullOrWhiteSpace(Subset))
            {
                throw new XmlSyntaxException(lineNumberInfo.LineNumber, GetType().Name + ", Missing Subset Attribute");
            }

            if (!hasFriendlyName || string.IsNullOrWhiteSpace(FriendlyName))
            {
                throw new XmlSyntaxException(lineNumberInfo.LineNumber, GetType().Name + ", Missing FriendlyName Attribute");
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