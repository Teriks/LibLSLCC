#region

using System;
using System.Collections.Generic;
using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    [XmlRoot("LibraryFunction")]
    public sealed class LSLLibraryFunctionSignature : LSLFunctionSignature, IXmlSerializable
    {
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private HashSet<string> _subsets = new HashSet<string>();

        private LSLLibraryDataSubsetsAttributeRegex _subsetsRegex = new
            LSLLibraryDataSubsetsAttributeRegex();


        public LSLLibraryFunctionSignature(LSLFunctionSignature other) : base(other)
        {
            DocumentationString = "";
        }


        private LSLLibraryFunctionSignature()
        {
        }

        public LSLLibraryFunctionSignature(LSLLibraryFunctionSignature other)
            : base(other)
        {
            DocumentationString = other.DocumentationString;
            _subsets = new HashSet<string>(other._subsets);
        }

        public LSLLibraryFunctionSignature(LSLType returnType, string name, IEnumerable<LSLParameter> parameters) :
            base(returnType, name, parameters)
        {
            DocumentationString = "";
        }

        public IReadOnlySet<string> Subsets
        {
            get { return new ReadOnlyHashSet<string>(_subsets); }
        }

        public IReadOnlyDictionary<string, string> Properties
        {
            get { return _properties; }
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
            var parameterNames = new HashSet<string>();

            var lineNumberInfo = (IXmlLineInfo) reader;

            reader.MoveToContent();
            bool hasReturnType = false;
            bool hasSubsets = false;
            bool hasName = false;
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "Subsets")
                {
                    SetSubsets(reader.Value);
                    hasSubsets = true;
                }
                else if (reader.Name == "ReturnType")
                {
                    LSLType type;
                    if (Enum.TryParse(reader.Value, out type))
                    {
                        ReturnType = type;
                        hasReturnType = true;
                    }
                    else
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            GetType().Name + ", ReturnType attribute invalid");
                    }
                }
                else if (reader.Name == "Name")
                {
                    if (string.IsNullOrWhiteSpace(reader.Value))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            GetType().Name + ", Name attribute invalid");
                    }
                    hasName = true;
                    Name = reader.Value;
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

            if (!hasReturnType)
            {
                throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                    "Missing ReturnType attribute");
            }

            if (!hasSubsets)
            {
                throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                    "Missing Subsets attribute");
            }

            bool isVariadic = false;

            var canRead = reader.Read();
            while (canRead)
            {
                if ((reader.Name == "Parameter") && reader.IsStartElement())
                {
                    if (isVariadic)
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "{0}, Library function {1} cannot more parameters after a Variadic parameter is defined",
                                GetType().Name, Name));
                    }


                    LSLType pType;

                    if (!Enum.TryParse(reader.GetAttribute("Type"), out pType))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            GetType().Name + ", Parameter Type attribute invalid");
                    }

                    var pName = reader.GetAttribute("Name");

                    if (string.IsNullOrWhiteSpace(pName))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            GetType().Name + ", Parameter Name attribute invalid");
                    }

                    if (parameterNames.Contains(pName))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            GetType().Name + ", Parameter Name already used");
                    }

                    var variadic = reader.GetAttribute("Variadic");


                    if (!string.IsNullOrWhiteSpace(variadic))
                    {
                        if (variadic.ToLower() == "true")
                        {
                            isVariadic = true;

                            if (pType != LSLType.Void)
                            {
                                throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                                    string.Format(
                                        "{0}, Variadic parameter {1} in Function {2} must have a Type equal to Void",
                                        GetType().Name, pName, Name));
                            }
                        }
                        else if (variadic.ToLower() != "false")
                        {
                            throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                                string.Format(
                                    "{0}, Variadic attribute in parameter {1} of Function {2} must equal True or False",
                                    GetType().Name, pName, Name));
                        }
                    }


                    parameterNames.Add(pName);

                    AddParameter(new LSLParameter(pType, pName, isVariadic));

                    canRead = reader.Read();
                }
                else if ((reader.Name == "DocumentationString") && reader.IsStartElement())
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
                                "{0}, Function {1}: Property element's Name attribute cannot be empty",
                                GetType().Name, Name));
                    }

                    var value = reader.GetAttribute("Value");

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "{0}, Function {1}: Property element's Value attribute cannot be empty",
                                GetType().Name, Name));
                    }

                    if (_properties.ContainsKey(name))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "{0}, Function {1}: Property name {2} has already been used",
                                GetType().Name, Name, name));
                    }

                    _properties.Add(name, value);

                    canRead = reader.Read();
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "LibraryFunction")
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
            writer.WriteAttributeString("ReturnType", ReturnType.ToString());
            writer.WriteAttributeString("Subsets", string.Join(",", _subsets));

            foreach (var param in Parameters)
            {
                writer.WriteStartElement("Parameter");
                writer.WriteAttributeString("Name", param.Name);
                writer.WriteAttributeString("Type", param.Type.ToString());

                if (param.Variadic)
                {
                    writer.WriteAttributeString("Variadic", "true");
                }

                writer.WriteEndElement();
            }

            writer.WriteStartElement("DocumentationString");
            writer.WriteString(DocumentationString);
            writer.WriteEndElement();
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

        public void SetSubsets(IEnumerable<string> subsets)
        {
            _subsets = new HashSet<string>(subsets);
        }

        public new static LSLLibraryFunctionSignature Parse(string str)
        {
            return new LSLLibraryFunctionSignature(LSLFunctionSignature.Parse(str));
        }


        public static LSLLibraryFunctionSignature FromXmlFragment(XmlReader fragment)
        {
            var x = new LSLLibraryFunctionSignature();
            x.ReadXml(fragment);
            return x;
        }
    }
}