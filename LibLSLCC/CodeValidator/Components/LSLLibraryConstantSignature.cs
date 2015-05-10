#region


using System;
using System.Collections.Generic;
using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.Collections;


#endregion

namespace LibLSLCC.CodeValidator.Components
{
    [XmlRoot("LibraryConstant")]
    public sealed class LSLLibraryConstantSignature : IXmlSerializable
    {
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private HashSet<string> _subsets = new HashSet<string>();

        private LSLLibraryDataSubsetsAttributeRegex _subsetsRegex = new
            LSLLibraryDataSubsetsAttributeRegex();

        private LSLLibraryConstantSignature()
        {
            ValueString = "";
            DocumentationString = "";
            Name = "";
            Type = LSLType.Void;
        }

        public LSLLibraryConstantSignature(LSLLibraryConstantSignature other)
        {
            ValueString = other.ValueString;
            DocumentationString = other.DocumentationString;
            _subsets = new HashSet<string>(other._subsets);
            Type = other.Type;
        }

        public LSLLibraryConstantSignature(LSLType type, string name)
        {
            DocumentationString = "";
            Name = name;
            Type = type;
            ValueString = "";
        }

        public LSLLibraryConstantSignature(LSLType type, string name, string valueString)
        {
            DocumentationString = "";
            Name = name;
            ValueString = valueString;
            Type = type;
        }

        public IReadOnlySet<string> Subsets
        {
            get { return new ReadOnlyHashSet<string>(_subsets); }
        }

        public IReadOnlyDictionary<string, string> Properties
        {
            get { return _properties; }
        }


        public string SignatureString
        {
            get { return LSLTypeTools.ToLSLTypeString(Type) + " " + Name + " = " + ValueString; }
        }

        public string DocumentationString { get; set; }


        public string SignatureAndDocumentation
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DocumentationString))
                {
                    return SignatureString + ";";
                }
                return SignatureString + ";" +
                       Environment.NewLine +
                       DocumentationString;
            }
        }


        public LSLType Type { get; private set; }
        public string Name { get; private set; }
        public string ValueString { get; private set; }


        /// <summary>
        ///     This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return
        ///     null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the
        ///     <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is
        ///     produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method
        ///     and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" />
        ///     method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }


        /// <summary>
        ///     Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized. </param>
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            bool hasSubsets = false;
            bool hasType = false;
            bool hasName = false;
            bool hasValue = false;

            var lineNumberInfo = (IXmlLineInfo) reader;

            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "Value")
                {
                    string val = reader.Value;
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        hasValue = true;
                    }
                    ValueString = val;
                }
                else if (reader.Name == "Subsets")
                {
                    SetSubsets(reader.Value);
                    hasSubsets = true;
                }
                else if (reader.Name == "Type")
                {
                    LSLType type;
                    if (Enum.TryParse(reader.Value, out type))
                    {
                        Type = type;
                        hasType = true;
                    }
                    else
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            GetType().Name + ", Type attribute invalid");
                    }
                }
                else if (reader.Name == "Name")
                {
                    if (string.IsNullOrWhiteSpace(reader.Value))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            GetType().Name + ", Name attribute invalid");
                    }
                    Name = reader.Value;
                    hasName = true;
                }
                else
                {
                    throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                        GetType().Name + ", unknown attribute " + reader.Name);
                }
            }

            if (!hasName)
            {
                throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                    "Missing Name attribute");
            }

            if (!hasType)
            {
                throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                    "Missing Type attribute");
            }

            if (!hasValue)
            {
                throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                    "Missing Value attribute");
            }

            if (!hasSubsets)
            {
                throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                    "Missing Subsets attribute");
            }


            var canRead = reader.Read();
            while (canRead)
            {
                if ((reader.Name == "DocumentationString") && reader.IsStartElement())
                {
                    DocumentationString = reader.ReadElementContentAsString();
                    canRead = reader.Read();
                }
                else if ((reader.Name == "Property") && reader.IsStartElement())
                {
                    var name = reader.GetAttribute("Name");

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "{0}, Event {1}: Property element's Name attribute cannot be empty",
                                GetType().Name, Name));
                    }

                    var value = reader.GetAttribute("Value");

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "{0}, Event {1}: Property element's Value attribute cannot be empty",
                                GetType().Name, Name));
                    }

                    if (_properties.ContainsKey(name))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "{0}, Event {1}: Property name {2} has already been used",
                                GetType().Name, Name, name));
                    }

                    _properties.Add(name, value);

                    canRead = reader.Read();
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "LibraryConstant")
                {
                    break;
                }
                else
                {
                    canRead = reader.Read();
                }
            }
        }


        /// <summary>
        ///     Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Type", Type.ToString());
            writer.WriteAttributeString("Value", ValueString);
            writer.WriteAttributeString("Subsets", string.Join(",", _subsets));

            writer.WriteStartElement("DocumentationString");
            writer.WriteString(DocumentationString);
            writer.WriteEndElement();
        }

        public override string ToString()
        {
            return SignatureString;
        }


        public void SetSubsets(IEnumerable<string> subsets)
        {
            _subsets = new HashSet<string>(subsets);
        }

        public void SetSubsets(string subsets)
        {
            _subsets = new HashSet<string>(_subsetsRegex.ParseSubsets(subsets));
        }

        public void AddSubsets(string subsets)
        {
            _subsets.UnionWith(_subsetsRegex.ParseSubsets(subsets));
        }

        public void AddSubsets(IEnumerable<string> subsets)
        {
            _subsets.UnionWith(subsets);
        }


        public static LSLLibraryConstantSignature FromXmlFragment(XmlReader reader)
        {
            var x = new LSLLibraryConstantSignature();
            x.ReadXml(reader);
            return x;
        }


        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash*31 + Type.GetHashCode();
            hash = hash*31 + Name.GetHashCode();

            return hash;
        }


        public override bool Equals(object obj)
        {
            var o = obj as LSLLibraryConstantSignature;
            if (o == null)
            {
                return false;
            }

            return o.Name == Name && o.Type == Type;
        }
    }
}