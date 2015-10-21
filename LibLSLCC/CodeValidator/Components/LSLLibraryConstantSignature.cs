#region FileInfo
// 
// File: LSLLibraryConstantSignature.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Exceptions;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    /// Represents the signature of a constant provided from an ILSLLibraryDataProvider implementation
    /// </summary>
    [XmlRoot("LibraryConstant")]
    public sealed class LSLLibraryConstantSignature : IXmlSerializable, ILSLLibrarySignature
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

        /// <summary>
        /// Construct the LSLLibraryConstantSingature by cloning another one.
        /// </summary>
        /// <param name="other"></param>
        public LSLLibraryConstantSignature(LSLLibraryConstantSignature other)
        {
            ValueString = other.ValueString;
            DocumentationString = other.DocumentationString;
            _subsets = new HashSet<string>(other._subsets);
            Type = other.Type;
            _properties = other._properties.ToDictionary(x => x.Key, y => y.Value);
        }


        /// <summary>
        /// Construct the LSLLibraryConstantSignature from a given LSLType and constant name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public LSLLibraryConstantSignature(LSLType type, string name)
        {
            DocumentationString = "";
            Name = name;
            Type = type;
            ValueString = "";
        }


        /// <summary>
        /// Construct the LSLLibraryConstantSignature from a given LSLType, constant name, and Value string.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="valueString"></param>
        public LSLLibraryConstantSignature(LSLType type, string name, string valueString)
        {
            DocumentationString = "";
            Name = name;
            ValueString = valueString;
            Type = type;
        }

        /// <summary>
        /// The library subsets this signature belongs to/is shared among.
        /// </summary>
        public IReadOnlySet<string> Subsets
        {
            get { return new ReadOnlyHashSet<string>(_subsets); }
        }


        /// <summary>
        /// Additional dynamic property values that can be attached to the constant signature and parsed from XML
        /// </summary>
        public IDictionary<string, string> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Returns a formated signature string for the constant, in the form:  NAME = ValueStringAsCodeLiteral
        /// Without a trailing semi-colon character.
        /// </summary>
        public string SignatureString
        {
            get { return Type.ToLSLTypeString() + " " + Name + " = " + ValueStringAsCodeLiteral; }
        }

        /// <summary>
        /// Gets or sets the documentation string attached to this signature.
        /// </summary>
        public string DocumentationString { get; set; }


        /// <summary>
        /// Combines the SignatureString and Documentation string.  The signature will
        /// have a trailing semi-colon, and if there is a documentation string a new-line will
        /// be inserted between the signature and documentation string.
        /// </summary>
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
        /// The LSLType that the library constant is defined with.
        /// </summary>
        public LSLType Type { get; set; }

        /// <summary>
        /// The name of the library constant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The raw value string of the library constant.
        /// </summary>
        public string ValueString { get; private set; }


        /// <summary>
        /// Returns the ValueString replacing any control code characters with symbolic string escapes.
        /// </summary>
        public string ValueStringWithControlCodeEscapes
        {
            get { return StringTools.ShowControlCodeEscapes(ValueString); }
        }

        /// <summary>
        /// Returns a string which represents what this Constant would look like
        /// if it were expanded into an LSL code literal.  This takes the Type and contents
        /// of ValueString into account.
        /// </summary>
        public string ValueStringAsCodeLiteral
        {
            get
            {
                if (Type == LSLType.Key || Type == LSLType.String)
                {
                    return "\"" + ValueStringWithControlCodeEscapes + "\"";
                }
                if (Type == LSLType.Vector || Type == LSLType.Rotation)
                {
                    return "<" + ValueString + ">";
                }
                if (Type == LSLType.List)
                {
                    return "[" + ValueString + "]";
                }

                return ValueString;
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
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Fills a constant signature object from an XML fragment.
        /// </summary>
        /// <param name="reader">The XML reader containing the fragment to read.</param>
        /// <exception cref="LSLInvalidSymbolNameException">Thrown if the constants name does not abide by LSL symbol naming conventions.</exception>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            var hasSubsets = false;
            var hasType = false;
            var hasName = false;
            var hasValue = false;

            var lineNumberInfo = (IXmlLineInfo) reader;

            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "Value")
                {
                    var val = reader.Value;
                    //The value is only truly missing if its entirely devoid of character data
                    //some LSL constants like EOF are nothing but whitespace characters
                    if (val.Length != 0)
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
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Type", Type.ToString());
            writer.WriteAttributeString("Value", ValueString);
            writer.WriteAttributeString("Subsets", string.Join(",", _subsets));

            foreach (var prop in Properties)
            {
                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Name", prop.Key);
                writer.WriteAttributeString("Value", prop.Value);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("DocumentationString");
            writer.WriteString(DocumentationString);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Delegates to the SignatureString Property.
        /// </summary>
        /// <returns>
        /// The SignatureString Property.
        /// </returns>
        public override string ToString()
        {
            return SignatureString;
        }

        /// <summary>
        /// Sets the library subsets this LSLLibraryConstantSignature belongs to.
        /// </summary>
        /// <param name="subsets">An enumerable of subset name strings</param>
        public void SetSubsets(IEnumerable<string> subsets)
        {
            _subsets = new HashSet<string>(subsets);
        }

        /// <summary>
        /// Sets the library subsets this LSLLibraryConstantSignature belongs to by parsing them out of a comma separated string of names.
        /// </summary>
        /// <param name="subsets">A comma separated list of subset names in a string.</param>
        public void SetSubsets(string subsets)
        {
            _subsets = new HashSet<string>(_subsetsRegex.ParseSubsets(subsets));
        }


        /// <summary>
        /// Adds to the current library subsets this LSLLibraryConstantSignature belongs to by parsing them out of a comma separated string of names.
        /// </summary>
        /// <param name="subsets">A comma separated list of subset names in a string to add.</param>
        public void AddSubsets(string subsets)
        {
            _subsets.UnionWith(_subsetsRegex.ParseSubsets(subsets));
        }


        /// <summary>
        /// Adds to the current library subsets this LSLLibraryConstantSignature belongs to.
        /// </summary>
        /// <param name="subsets">An enumerable of subset name strings to add.</param>
        public void AddSubsets(IEnumerable<string> subsets)
        {
            _subsets.UnionWith(subsets);
        }


        /// <summary>
        /// Reads a constant signature object from an XML fragment.
        /// </summary>
        /// <param name="reader">The XML reader containing the fragment to read.</param>
        /// <exception cref="LSLInvalidSymbolNameException">Thrown if the constants name does not abide by LSL symbol naming conventions.</exception>
        /// <returns>The parsed LSLLibraryConstantSignature object.</returns>
        public static LSLLibraryConstantSignature FromXmlFragment(XmlReader reader)
        {
            var con = new LSLLibraryConstantSignature();
            IXmlSerializable x = con;
            x.ReadXml(reader);
            return con;
        }


        /// <summary>
        /// Returns the hash code of the LSLConstantSignature object.  The Type and Name properties are used to generate the hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hash = 17;
            


            hash = hash*31 + Type.GetHashCode();
            hash = hash*31 + Name.GetHashCode();


            return hash;
        }


        /// <summary>
        /// Determines whether the Type and Name properties of another LSLLibraryConstantSignature equal the Type and Name properties of this object.
        /// If the passed object is not an LSLLibraryConstantSignature object then the result will always be false.
        /// </summary>
        /// <param name="obj">The object to compare this object with.</param>
        /// <returns>True if the object is an LSLLibraryConstantSignature object and the Name and Type properties of both objects are equal to each other.</returns>
        public override bool Equals(object obj)
        {
            var o = obj as LSLLibraryConstantSignature;
            if (o == null)
            {
                return false;
            }

            return o.Name == Name && o.Type == Type;
        }


        /// <summary>
        /// Whether or not this constant is marked as deprecated.
        /// </summary>
        public bool Deprecated
        {
            get
            {
                string deprecatedStatus;
                return (Properties.TryGetValue("Deprecated", out deprecatedStatus) &&
                        deprecatedStatus.ToLower() == "true");
            }
            set
            {
                if (value == false)
                {
                    if (Properties.ContainsKey("Deprecated"))
                    {
                        Properties.Remove("Deprecated");
                    }
                }
                else
                {
                    Properties["Deprecated"] = "true";
                }
            }
        }

        /// <summary>
        /// A hint to compilers, if Expand is true then the constant's value should be expanded and placed
        /// into the generated code, otherwise if Expand is false the constants value should be retrieved
        /// by referencing the constant by name in the current object or some other object where constants
        /// are stored.
        /// </summary>
        public bool Expand
        {
            get {
                string expand;
                return (Properties.TryGetValue("Expand", out expand) && expand.ToLower() == "true");
            }
            set
            {
                if (value == false)
                {
                    if (Properties.ContainsKey("Expand"))
                    {
                        Properties.Remove("Expand");
                    }
                }
                else
                {
                    Properties["Expand"] = "true";
                }
            }
        }
    }
}