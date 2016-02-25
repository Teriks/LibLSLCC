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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator;
using LibLSLCC.Utility;
using LibLSLCC.Utility.ListParser;

#endregion

namespace LibLSLCC.LibraryData
{
    /// <summary>
    ///     Represents the signature of a constant provided from an <see cref="ILSLLibraryDataProvider" /> implementation
    /// </summary>
    [XmlRoot("LibraryConstant")]
    public sealed class LSLLibraryConstantSignature : IXmlSerializable, ILSLLibrarySignature
    {
        private static string _floatRegexString = "([-+]?[0-9]*(?:\\.[0-9]*))";

        private static Regex _vectorValidationRegex =
            new Regex("^" + _floatRegexString + "\\s*,\\s*" + _floatRegexString + "\\s*,\\s*" + _floatRegexString + "$");

        private static Regex _rotationValidationRegex =
            new Regex("^" + _floatRegexString + "\\s*,\\s*" + _floatRegexString + "\\s*,\\s*" + _floatRegexString +
                      "\\s*,\\s*" + _floatRegexString + "$");

        private string _name;
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private LSLLibraryDataSubsetCollection _subsets = new LSLLibraryDataSubsetCollection();
        private LSLType _type;
        private string _valueString;


        private LSLLibraryConstantSignature()
        {
            DocumentationString = "";
        }


        /// <summary>
        ///     Construct the <see cref="LSLLibraryConstantSignature" /> by cloning another one.
        /// </summary>
        /// <param name="other">The other <see cref="LSLLibraryConstantSignature" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        public LSLLibraryConstantSignature(LSLLibraryConstantSignature other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            DocumentationString = other.DocumentationString;
            _subsets = new LSLLibraryDataSubsetCollection(other._subsets);
            _properties = other._properties.ToDictionary(x => x.Key, y => y.Value);
            _type = other.Type;
            _valueString = other.ValueString;
        }


        /// <summary>
        ///     Construct the LSLLibraryConstantSignature from a given <see cref="LSLType" /> and constant name.
        ///     <see cref="ValueString" /> is given the default
        ///     value for the given <see cref="LSLType" /> passed in <paramref name="type" />
        /// </summary>
        /// <param name="type">The constant type.</param>
        /// <param name="name">The constant name.</param>
        /// <exception cref="LSLInvalidSymbolNameException">If <paramref name="name" /> is an invalid LSL ID token.</exception>
        /// <exception cref="LSLLibraryDataInvalidConstantTypeException">
        ///     if <paramref name="type" /> is
        ///     <see cref="LSLType.Void" />.
        /// </exception>
        public LSLLibraryConstantSignature(LSLType type, string name)
        {
            DocumentationString = "";
            Name = name;
            Type = type;

            //use _valueString to bypass validation, since its faster
            //and we know what is required by the class.
            switch (type)
            {
                case LSLType.Key:
                    _valueString = "00000000-0000-0000-0000-000000000000";
                    break;
                case LSLType.Integer:
                    _valueString = "0";
                    break;
                case LSLType.String:
                    _valueString = "";
                    break;
                case LSLType.Float:
                    _valueString = "0.0";
                    break;
                case LSLType.List:
                    _valueString = ""; //empty list
                    break;
                case LSLType.Vector:
                    _valueString = "0,0,0";
                    break;
                case LSLType.Rotation:
                    _valueString = "0,0,0,0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
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
        /// <exception cref="LSLLibraryDataInvalidConstantTypeException">
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
        {
            DocumentationString = "";
            Name = name;
            Type = type;
            ValueString = valueString;
        }


        /// <summary>
        ///     Returns a formated signature string for the constant, in the form:  NAME = ValueStringAsCodeLiteral
        ///     Without a trailing semi-colon character.
        /// </summary>
        public string SignatureString
        {
            get { return Type.ToLSLTypeName() + " " + Name + " = " + ValueStringAsCodeLiteral; }
        }

        /// <summary>
        ///     The <see cref="LSLType" /> that the library constant is defined with.
        /// </summary>
        /// <exception cref="LSLLibraryDataInvalidConstantTypeException" accessor="set">
        ///     if <paramref name="value" /> is
        ///     <see cref="LSLType.Void" />.
        /// </exception>
        public LSLType Type
        {
            get { return _type; }
            set
            {
                if (value == LSLType.Void)
                {
                    throw new LSLLibraryDataInvalidConstantTypeException(
                        "Library Constant's Type may not be set to Void.");
                }
                _type = value;
            }
        }

        /// <summary>
        ///     The name of the library constant, must abide by LSL symbol naming rules or an exception will be thrown.
        /// </summary>
        /// <exception cref="LSLInvalidSymbolNameException" accessor="set">If <paramref name="value" /> is an invalid LSL ID token.</exception>
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new LSLInvalidSymbolNameException(
                        GetType().Name + ": Constant name was null or whitespace.");
                }

                if (!LSLTokenTools.IDRegexAnchored.IsMatch(value))
                {
                    throw new LSLInvalidSymbolNameException(
                        string.Format(
                            GetType().Name + ": Constant name '{0}' contained invalid characters or formatting.",
                            value));
                }

                _name = value;
            }
        }

        /// <summary>
        ///     The value string of the library constant, you must set <see cref="Type" /> first to a value that is not
        ///     <see cref="LSLType.Void" /> or an exception will be thrown.
        /// </summary>
        /// <value>
        ///     The value string.
        /// </value>
        /// <exception cref="LSLInvalidConstantValueStringException" accessor="set">
        ///     If the Value is an invalid value for a float and <see cref="Type" /> is set to <see cref="LSLType.Float" />
        ///     or
        ///     If the Value is an invalid value for an integer and <see cref="Type" /> is set to <see cref="LSLType.Integer" />
        ///     or
        ///     If the Value is an invalid value for a vector and <see cref="Type" /> is set to <see cref="LSLType.Vector" />
        ///     or
        ///     If the Value is an invalid value for a rotation and <see cref="Type" /> is set to <see cref="LSLType.Rotation" />
        /// </exception>
        /// <exception cref="LSLLibraryDataInvalidConstantTypeException" accessor="set">
        ///     If you try to set this value and
        ///     <see cref="Type" /> is equal to <see cref="LSLType.Void" />.
        /// </exception>
        /// <remarks>
        ///     Only integral or hexadecimal values are allowed when <see cref="Type" /> is set to <see cref="LSLType.Integer" />
        ///     Only floating point or hexadecimal values are allowed when <see cref="Type" /> is set to
        ///     <see cref="LSLType.Float" />
        ///     The enclosing less than and greater than symbols will be removed when <see cref="Type" /> is set to
        ///     <see cref="LSLType.Vector" /> or <see cref="LSLType.Rotation" />.
        /// </remarks>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <c>null</c>.</exception>
        public string ValueString
        {
            get { return _valueString; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", GetType().Name + ".ValueString cannot be set to null.");
                }

                switch (Type)
                {
                    case LSLType.Float:
                        SetFloatValueString(value);
                        return;
                    case LSLType.Integer:
                        SetIntegerValueString(value);
                        return;
                    case LSLType.List:
                        SetListValueString(value);
                        return;
                    case LSLType.Vector:
                        SetVectorValueString(value);
                        return;
                    case LSLType.Rotation:
                        SetRotationValueString(value);
                        return;
                    case LSLType.Void:
                        throw new LSLLibraryDataInvalidConstantTypeException(
                            "Could not set ValueString because the 'Type' Properties value is set to Void.");
                }

                _valueString = value;
            }
        }

        /// <summary>
        ///     Returns a string which represents what this Constant would look like
        ///     if it were expanded into an LSL code literal.  This takes the Type and contents
        ///     of ValueString into account.
        /// </summary>
        public string ValueStringAsCodeLiteral
        {
            get
            {
                if (Type == LSLType.Key || Type == LSLType.String)
                {
                    return "\"" + LSLFormatTools.ShowControlCodeEscapes(ValueString) + "\"";
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
        /// <exception cref="LSLLibraryDataInvalidConstantTypeException">if 'Type' is <see cref="LSLType.Void" />.</exception>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">
        ///     On missing or unknown attributes.
        ///     If the constant 'Type' is <see cref="LSLType.Void" />.
        ///     If the constant 'Type' does not correspond to an <see cref="LSLType" /> enumeration member.
        ///     If a 'Properties' node 'Name' is <c>null</c> or whitespace.
        ///     If a 'Properties' node 'Name' is used more than once.
        ///     If a 'Properties' node 'Value' is <c>null</c> or whitespace.
        /// </exception>
        /// <exception cref="LSLInvalidConstantValueStringException">
        ///     If 'Value' is an invalid value for a float and <see cref="Type" /> is set to <see cref="LSLType.Float" />
        ///     or
        ///     If 'Value' is an invalid value for an integer and <see cref="Type" /> is set to <see cref="LSLType.Integer" />
        ///     or
        ///     If 'Value' is an invalid value for a vector and <see cref="Type" /> is set to <see cref="LSLType.Vector" />
        ///     or
        ///     If 'Value' is an invalid value for a rotation and <see cref="Type" /> is set to <see cref="LSLType.Rotation" />
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
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to the <see cref="LSLType" /> passed in <paramref name="type" />.
        /// </summary>
        /// <param name="type">The <see cref="LSLType" /> <paramref name="valueString" /> must be valid for.</param>
        /// <param name="valueString">The value string to validate.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="valueString" /> can successfully be parsed for the given <see cref="LSLType" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentException">if <paramref name="type" /> is <see cref="LSLType.Void" />.</exception>
        public static bool ValidateValueString(LSLType type, string valueString)
        {
            string discard;
            return TryParseValueString(type, valueString, out discard);
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to the <see cref="LSLType" /> passed in <paramref name="type" />.
        /// </summary>
        /// <param name="type">The <see cref="LSLType" /> <paramref name="valueString" /> must be valid for.</param>
        /// <param name="valueString">The value string to validate.</param>
        /// <param name="formated">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="valueString" /> can successfully be parsed for the given <see cref="LSLType" />
        ///     .
        /// </returns>
        /// <exception cref="ArgumentException">if <paramref name="type" /> is <see cref="LSLType.Void" />.</exception>
        public static bool TryParseValueString(LSLType type, string valueString, out string formated)
        {
            if (type == LSLType.Void)
            {
                throw new ArgumentException("type must not be LSLType.Void", "type");
            }

            formated = null;


            switch (type)
            {
                case LSLType.Float:
                    return TryParseFloatValueString(valueString, out formated);
                case LSLType.Integer:
                    return TryParseIntegerValueString(valueString, out formated);
                case LSLType.List:
                    return TryParseListValueString(valueString, out formated);
                case LSLType.Vector:
                    return TryParseVectorValueString(valueString, out formated);
                case LSLType.Rotation:
                    return TryParseRotationValueString(valueString, out formated);
                case LSLType.String:
                case LSLType.Key:
                    formated = valueString;
                    return true;
            }

            return false;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Float" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <param name="errMessage">An error message describing why the parse failed if this function returns <c>false</c>.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Float" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseFloatValueString(string value, out string valueString, out string errMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null.");
            }

            valueString = null;

            string stripSpecifiers = value.TrimEnd('f', 'F', 'd', 'D');

            double f;
            if (!double.TryParse(stripSpecifiers, out f))
            {
                int i;
                if (!int.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out i))
                {
                    errMessage =
                        string.Format("Float Constant ValueString:  Given string '{0}' is not a valid float value.",
                            value);

                    return false;
                }
                valueString = value;
            }
            else
            {
                valueString = stripSpecifiers;
            }

            errMessage = null;
            return true;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Float" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Float" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseFloatValueString(string value, out string valueString)
        {
            string discard;
            return TryParseFloatValueString(value, out valueString, out discard);
        }


        private void SetFloatValueString(string value)
        {
            string msg;
            if (!TryParseFloatValueString(value, out _valueString, out msg))
            {
                throw new LSLInvalidConstantValueStringException(msg);
            }
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.String" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <param name="errMessage">An error message describing why the parse failed if this function returns <c>false</c>.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.String" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseIntegerValueString(string value, out string valueString, out string errMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null.");
            }

            valueString = null;

            bool b;
            if (!bool.TryParse(value, out b))
            {
                int i;
                if (!int.TryParse(value, out i) &&
                    !int.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out i))
                {
                    errMessage =
                        string.Format(
                            "Integer Constant ValueString:  Given value '{0}' is not a valid integer, hexadecimal or boolean value.",
                            value);

                    return false;
                }

                valueString = value;
            }
            else
            {
                valueString = b ? "1" : "0";
            }

            errMessage = null;
            return true;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.String" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.String" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseIntegerValueString(string value, out string valueString)
        {
            string discard;
            return TryParseIntegerValueString(value, out valueString, out discard);
        }


        private void SetIntegerValueString(string value)
        {
            string msg;
            if (!TryParseIntegerValueString(value, out _valueString, out msg))
            {
                throw new LSLInvalidConstantValueStringException(msg);
            }
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.List" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <param name="errMessage">An error message describing why the parse failed if this function returns <c>false</c>.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.List" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseListValueString(string value, out string valueString, out string errMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null.");
            }

            valueString = null;

            try
            {
                string s = value.Trim(' ');

                if (string.IsNullOrWhiteSpace(s))
                {
                    errMessage =
                        "List Constant ValueString Invalid: May not be whitespace.";
                    return false;
                }


                char firstChar = s[0];
                char lastChar = s[s.Length - 1];

                if ((firstChar == '[' || lastChar == ']') &&
                    (firstChar != '[' || lastChar != ']'))
                {
                    errMessage =
                        "List Constant ValueString '{0}' Invalid: If brackets are used for the List value string, both brackets must be present.";
                    return false;
                }
                if (firstChar == '[')
                {
                    s = s.Substring(1, s.Length - 2);
                }

                valueString = string.IsNullOrWhiteSpace(s) ? "" : LSLListParser.Format("[" + s + "]", false);
            }
            catch (LSLListParserSyntaxException e)
            {
                errMessage = "List Constant ValueString Invalid: " + e.Message;
                return false;
            }

            errMessage = null;
            return true;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.List" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.List" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseListValueString(string value, out string valueString)
        {
            string discard;
            return TryParseListValueString(value, out valueString, out discard);
        }


        private void SetListValueString(string value)
        {
            string msg;
            if (!TryParseListValueString(value, out _valueString, out msg))
            {
                throw new LSLInvalidConstantValueStringException(msg);
            }
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Rotation" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <param name="errMessage">An error message describing why the parse failed if this function returns <c>false</c>.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Rotation" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseRotationValueString(string value, out string valueString, out string errMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null.");
            }

            valueString = null;

            string s = value.Trim(' ');


            if (string.IsNullOrWhiteSpace(s))
            {
                errMessage =
                    "Rotation Constant ValueString Invalid: May not be whitespace.";
                return false;
            }


            char firstChar = s[0];
            char lastChar = s[s.Length - 1];

            if ((firstChar == '<' || lastChar == '>') &&
                (firstChar != '<' || lastChar != '>'))
            {
                errMessage =
                    "Rotation Constant ValueString '{0}' Invalid: If rotation quotes are used for a Rotation value string, both '<' and '>' must be present.";
                return false;
            }

            if (firstChar == '<')
            {
                s = s.Substring(1, s.Length - 2);
            }


            var match = _rotationValidationRegex.Match(s);
            if (!match.Success)
            {
                errMessage =
                    string.Format("Rotation Constant ValueString: '{0}' could not be parsed and formated.", value);
                return false;
            }

            valueString = match.Groups[1] + ", " + match.Groups[2] + ", " + match.Groups[3] + ", " + match.Groups[4];

            errMessage = null;
            return true;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Rotation" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Rotation" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseRotationValueString(string value, out string valueString)
        {
            string discard;
            return TryParseRotationValueString(value, out valueString, out discard);
        }


        private void SetRotationValueString(string value)
        {
            string msg;
            if (!TryParseRotationValueString(value, out _valueString, out msg))
            {
                throw new LSLInvalidConstantValueStringException(msg);
            }
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Vector" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <param name="errMessage">An error message describing why the parse failed if this function returns <c>false</c>.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Vector" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseVectorValueString(string value, out string valueString, out string errMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value cannot be null.");
            }

            valueString = null;

            string s = value.Trim(' ');


            if (string.IsNullOrWhiteSpace(s))
            {
                errMessage =
                    "Vector Constant ValueString Invalid: May not be null or whitespace.";

                return false;
            }

            char firstChar = s[0];
            char lastChar = s[s.Length - 1];

            if ((firstChar == '<' || lastChar == '>') &&
                (firstChar != '<' || lastChar != '>'))
            {
                errMessage =
                    "Vector Constant ValueString '{0}' Invalid: If vector quotes are used for a Vector value string, both '<' and '>' must be present.";
                return false;
            }

            if (firstChar == '<')
            {
                s = s.Substring(1, s.Length - 2);
            }


            var match = _vectorValidationRegex.Match(s);
            if (!match.Success)
            {
                errMessage =
                    string.Format("Vector Constant ValueString: '{0}' could not be parsed and formated.", value);

                return false;
            }

            valueString = match.Groups[1] + ", " + match.Groups[2] + ", " + match.Groups[3] + ", " + match.Groups[4];

            errMessage = null;
            return true;
        }


        /// <summary>
        ///     Validates the format of a string is acceptable for assigning to <see cref="ValueString" /> when <see cref="Type" />
        ///     is equal to <see cref="LSLType.Vector" />.
        /// </summary>
        /// <param name="value">The value string to validate.</param>
        /// <param name="valueString">The re-formated version of <paramref name="valueString" /> if the parse was successful.</param>
        /// <returns><c>true</c> if <paramref name="valueString" /> can successfully be parsed for <see cref="LSLType.Vector" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        public static bool TryParseVectorValueString(string value, out string valueString)
        {
            string discard;
            return TryParseVectorValueString(value, out valueString, out discard);
        }


        private void SetVectorValueString(string value)
        {
            string msg;
            if (!TryParseVectorValueString(value, out _valueString, out msg))
            {
                throw new LSLInvalidConstantValueStringException(msg);
            }
        }


        /// <summary>
        ///     Delegates to the SignatureString Property.
        /// </summary>
        /// <returns>
        ///     The SignatureString Property.
        /// </returns>
        public override string ToString()
        {
            return SignatureString;
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
        /// <exception cref="LSLLibraryDataInvalidConstantTypeException">if 'Type' is <see cref="LSLType.Void" />.</exception>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">
        ///     On missing or unknown attributes.
        ///     If the constant 'Type' is <see cref="LSLType.Void" />.
        ///     If the constant 'Type' does not correspond to an <see cref="LSLType" /> enumeration member.
        ///     If a 'Properties' node 'Name' is <c>null</c> or whitespace.
        ///     If a 'Properties' node 'Name' is used more than once.
        ///     If a 'Properties' node 'Value' is <c>null</c> or whitespace.
        /// </exception>
        /// <exception cref="LSLInvalidConstantValueStringException">
        ///     If 'Value' is an invalid value for a float and <see cref="Type" /> is set to <see cref="LSLType.Float" />
        ///     or
        ///     If 'Value' is an invalid value for an integer and <see cref="Type" /> is set to <see cref="LSLType.Integer" />
        ///     or
        ///     If 'Value' is an invalid value for a vector and <see cref="Type" /> is set to <see cref="LSLType.Vector" />
        ///     or
        ///     If 'Value' is an invalid value for a rotation and <see cref="Type" /> is set to <see cref="LSLType.Rotation" />
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


        /// <summary>
        ///     Returns the hash code of the LSLConstantSignature object.  The Type and Name properties are used to generate the
        ///     hash code.
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
        ///     Determines whether the Type and Name properties of another LSLLibraryConstantSignature equal the Type and Name
        ///     properties of this object.
        ///     If the passed object is not an LSLLibraryConstantSignature object then the result will always be false.
        /// </summary>
        /// <param name="obj">The object to compare this object with.</param>
        /// <returns>
        ///     True if the object is an LSLLibraryConstantSignature object and the Name and Type properties of both objects
        ///     are equal to each other.
        /// </returns>
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