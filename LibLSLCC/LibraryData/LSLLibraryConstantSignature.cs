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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator;

#endregion

namespace LibLSLCC.LibraryData
{
    /// <summary>
    ///     Represents the signature of a constant provided from an <see cref="ILSLLibraryDataProvider" /> implementation
    /// </summary>
    [XmlRoot("LibraryConstant")]
    public sealed class LSLLibraryConstantSignature : LSLConstantSignature, IXmlSerializable, ILSLLibrarySignature
    {
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private LSLLibraryDataSubsetCollection _subsets = new LSLLibraryDataSubsetCollection();


        private LSLLibraryConstantSignature()
        {
            DocumentationString = "";
        }


        /// <summary>
        ///     Construct the <see cref="LSLLibraryConstantSignature" /> by cloning another one.
        /// </summary>
        /// <param name="other">The other <see cref="LSLLibraryConstantSignature" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLLibraryConstantSignature(LSLLibraryConstantSignature other) : base(other)
        {
            DocumentationString = "";
        }



        /// <summary>
        ///     Construct the LSLLibraryConstantSignature from a given <see cref="LSLType" /> and constant name.
        ///     <see cref="LSLConstantSignature.ValueString" /> is given the default
        ///     value for the given <see cref="LSLType" /> passed in <paramref name="type" />
        /// </summary>
        /// <param name="type">The constant type.</param>
        /// <param name="name">The constant name.</param>
        /// <exception cref="LSLInvalidSymbolNameException">If <paramref name="name" /> is an invalid LSL ID token.</exception>
        /// <exception cref="LSLInvalidConstantTypeException">
        ///     if <paramref name="type" /> is
        ///     <see cref="LSLType.Void" />.
        /// </exception>
        public LSLLibraryConstantSignature(LSLType type, string name) : base(type, name)
        {
            DocumentationString = "";
        }


        /// <summary>
        ///     Construct the LSLLibraryConstantSignature from a given <see cref="LSLType" />, constant name, and Value string.
        /// </summary>
        /// <param name="type">The constant type.</param>
        /// <param name="name">The constant name.</param>
        /// <param name="valueString">
        ///     The string value that represents the constant.  Must be appropriate for
        ///     <paramref name="type" />.
        /// </param>
        /// <exception cref="LSLInvalidSymbolNameException">If <paramref name="name" /> is an invalid LSL ID token.</exception>
        /// <exception cref="LSLInvalidConstantTypeException">
        ///     if <paramref name="type" /> is
        ///     <see cref="LSLType.Void" />.
        /// </exception>
        /// <exception cref="LSLInvalidConstantValueStringException">
        ///     If <paramref name="valueString" /> is an invalid value for a float and <paramref name="type" /> is set to
        ///     <see cref="LSLType.Float" />
        ///     or
        ///     If <paramref name="valueString" /> is an invalid value for an integer and <paramref name="type" /> is set to
        ///     <see cref="LSLType.Integer" />
        ///     or
        ///     If <paramref name="valueString" /> is an invalid value for a vector and <paramref name="type" /> is set to
        ///     <see cref="LSLType.Vector" />
        ///     or
        ///     If <paramref name="valueString" /> is an invalid value for a rotation and <paramref name="type" /> is set to
        ///     <see cref="LSLType.Rotation" />
        /// </exception>
        public LSLLibraryConstantSignature(LSLType type, string name, string valueString)
            : base(type, name, valueString)
        {
            DocumentationString = "";
        }


        /// <summary>
        ///     A hint to compilers, if Expand is true then the constant's value should be expanded and placed
        ///     into the generated code, otherwise if Expand is false the constants value should be retrieved
        ///     by referencing the constant by name in the current object or some other object where constants
        ///     are stored.
        /// </summary>
        public bool Expand
        {
            get
            {
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


        /// <summary>
        ///     The library subsets this signature belongs to/is shared among.
        /// </summary>
        public LSLLibraryDataSubsetCollection Subsets
        {
            get { return _subsets; }
        }

        /// <summary>
        ///     Additional dynamic property values that can be attached to the constant signature and parsed from XML
        /// </summary>
        public IDictionary<string, string> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        ///     Gets or sets the documentation string attached to this signature.
        /// </summary>
        public string DocumentationString { get; set; }

        /// <summary>
        ///     Combines the SignatureString and Documentation string.  The signature will
        ///     have a trailing semi-colon, and if there is a documentation string a new-line will
        ///     be inserted between the signature and documentation string.
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
        ///     Whether or not this constant is marked as deprecated.
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
        ///     Fills a constant signature object from an XML fragment.
        /// </summary>
        /// <param name="reader">The XML reader containing the fragment to read.</param>
        /// <exception cref="LSLInvalidSymbolNameException">
        ///     Thrown if the constants name does not abide by LSL symbol naming
        ///     conventions.
        /// </exception>
        /// <exception cref="LSLInvalidSubsetNameException">
        ///     Thrown if any of the given subset names in the CSV 'Subsets' string do
        ///     not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*).
        /// </exception>
        /// <exception cref="LSLInvalidConstantTypeException">if 'Type' is <see cref="LSLType.Void" />.</exception>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">
        ///     On missing or unknown attributes.
        ///     If the constant 'Type' is <see cref="LSLType.Void" />.
        ///     If the constant 'Type' does not correspond to an <see cref="LSLType" /> enumeration member.
        ///     If a 'Properties' node 'Name' is <c>null</c> or whitespace.
        ///     If a 'Properties' node 'Name' is used more than once.
        ///     If a 'Properties' node 'Value' is <c>null</c> or whitespace.
        /// </exception>
        /// <exception cref="LSLInvalidConstantValueStringException">
        ///     If 'Value' is an invalid value for a float and <see cref="LSLConstantSignature.Type" /> is set to <see cref="LSLType.Float" />
        ///     or
        ///     If 'Value' is an invalid value for an integer and <see cref="LSLConstantSignature.Type" /> is set to <see cref="LSLType.Integer" />
        ///     or
        ///     If 'Value' is an invalid value for a vector and <see cref="LSLConstantSignature.Type" /> is set to <see cref="LSLType.Vector" />
        ///     or
        ///     If 'Value' is an invalid value for a rotation and <see cref="LSLConstantSignature.Type" /> is set to <see cref="LSLType.Rotation" />
        /// </exception>
        /// <exception cref="XmlException">Incorrect XML encountered in the input stream. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="reader" /> is <c>null</c>.</exception>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            reader.MoveToContent();

            var hasSubsets = false;
            var hasType = false;
            var hasName = false;
            var hasValue = false;

            string valueString = null;

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

                    //need to set the type first, defer this until later.
                    valueString = val;
                }
                else if (reader.Name == "Subsets")
                {
                    Subsets.SetSubsets(reader.Value);
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
                        throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format("LibraryConstantSignature{0}: Type attribute invalid.",
                                hasName ? (" '" + Name + "'") : ""));
                    }
                }
                else if (reader.Name == "Name")
                {
                    Name = reader.Value;
                    hasName = true;
                }
                else
                {
                    throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                        string.Format("LibraryConstantSignature{0}: Unknown attribute '{1}'.",
                            hasName ? (" '" + Name + "'") : "", reader.Name));
                }
            }

            if (!hasName)
            {
                throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                    "LibraryConstantSignature: Missing Name attribute.");
            }

            if (!hasType)
            {
                throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                    string.Format("LibraryConstantSignature '{0}': Missing Type attribute.", Name));
            }

            if (!hasValue)
            {
                throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                    string.Format("LibraryConstantSignature '{0}': Missing Value attribute.", Name));
            }

            if (!hasSubsets)
            {
                throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                    string.Format("LibraryConstantSignature '{0}': Missing Subsets attribute.", Name));
            }

            //Set the value string, this can possibly throw an LSLInvalidConstantValueStringException
            //The Type property needs to be set first above for validation to occur.
            ValueString = valueString;


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
                    var pName = reader.GetAttribute("Name");

                    if (string.IsNullOrWhiteSpace(pName))
                    {
                        throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "LibraryConstantSignature '{0}': Property element's Name attribute cannot be empty.",
                                Name));
                    }

                    var value = reader.GetAttribute("Value");

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "LibraryConstantSignature '{0}': Property element's Value attribute cannot be empty.",
                                Name));
                    }

                    if (_properties.ContainsKey(pName))
                    {
                        throw new LSLLibraryDataXmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "LibraryConstantSignature '{0}': Property name '{1}' has already been used.", Name,
                                pName));
                    }

                    _properties.Add(pName, value);

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
        ///     Converts this <see cref="LSLLibraryConstantSignature" /> into its XML representation.
        ///     The root element name is not written, this is due to <see cref="IXmlSerializable" /> implementation requirements.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter" /> stream to which the object is serialized. </param>
        /// <exception cref="InvalidOperationException"><paramref name="writer" /> is closed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="writer" /> is <c>null</c>.</exception>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Type", Type.ToString());
            writer.WriteAttributeString("Value", ValueString);
            writer.WriteAttributeString("Subsets", string.Join(",", _subsets));

            writer.WriteStartElement("DocumentationString");
            writer.WriteString(DocumentationString);
            writer.WriteEndElement();

            foreach (var prop in Properties)
            {
                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Name", prop.Key);
                writer.WriteAttributeString("Value", prop.Value);
                writer.WriteEndElement();
            }
        }


        /// <summary>
        ///     Creates a constant signature object from an XML fragment.
        /// </summary>
        /// <param name="reader">The XML reader containing the fragment to read.</param>
        /// <exception cref="LSLInvalidSymbolNameException">
        ///     Thrown if the constants name does not abide by LSL symbol naming
        ///     conventions.
        /// </exception>
        /// <exception cref="LSLInvalidSubsetNameException">
        ///     Thrown if any of the given subset names in the CSV 'Subsets' string do
        ///     not match the pattern ([a-zA-Z]+[a-zA-Z_0-9\\-]*).
        /// </exception>
        /// <exception cref="LSLInvalidConstantTypeException">if 'Type' is <see cref="LSLType.Void" />.</exception>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">
        ///     On missing or unknown attributes.
        ///     If the constant 'Type' is <see cref="LSLType.Void" />.
        ///     If the constant 'Type' does not correspond to an <see cref="LSLType" /> enumeration member.
        ///     If a 'Properties' node 'Name' is <c>null</c> or whitespace.
        ///     If a 'Properties' node 'Name' is used more than once.
        ///     If a 'Properties' node 'Value' is <c>null</c> or whitespace.
        /// </exception>
        /// <exception cref="LSLInvalidConstantValueStringException">
        ///     If 'Value' is an invalid value for a float and <see cref="LSLConstantSignature.Type" /> is set to <see cref="LSLType.Float" />
        ///     or
        ///     If 'Value' is an invalid value for an integer and <see cref="LSLConstantSignature.Type" /> is set to <see cref="LSLType.Integer" />
        ///     or
        ///     If 'Value' is an invalid value for a vector and <see cref="LSLConstantSignature.Type" /> is set to <see cref="LSLType.Vector" />
        ///     or
        ///     If 'Value' is an invalid value for a rotation and <see cref="LSLConstantSignature.Type" /> is set to <see cref="LSLType.Rotation" />
        /// </exception>
        /// <exception cref="XmlException">Incorrect XML encountered in the input stream. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="reader" /> is <c>null</c>.</exception>
        /// <returns>The parsed <see cref="LSLLibraryConstantSignature" /> object.</returns>
        public static LSLLibraryConstantSignature FromXmlFragment(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            var con = new LSLLibraryConstantSignature();
            IXmlSerializable x = con;
            x.ReadXml(reader);
            return con;
        }
    }
}