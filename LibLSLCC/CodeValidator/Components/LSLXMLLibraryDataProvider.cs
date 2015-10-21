#region FileInfo
// 
// File: LSLXMLLibraryDataProvider.cs
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
using LibLSLCC.CodeValidator.Exceptions;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    ///     A library data provider that reads LSL library data from XML
    /// </summary>
    public class LSLXmlLibraryDataProvider : LSLLibraryDataProvider,
        IXmlSerializable
    {

        /// <summary>
        /// Construct an LSLXmlLibraryDataProvider with the option to enable live filtering mode in the base class
        /// </summary>
        /// <param name="liveFiltering">Whether or not to enable live filtering mode in the base class.</param>
        public LSLXmlLibraryDataProvider(bool liveFiltering = true) : base(liveFiltering)
        {
            
        }


        /// <summary>
        /// Construct an LSLXmlLibraryDataProvider with the ability to set the active subsets in the base class, and optionally enable
        /// live filtering mode.
        /// </summary>
        /// <param name="activeSubsets">The active subsets to set in the base class.</param>
        /// <param name="liveFiltering">Whether or not to enable live filtering mode in the base class.</param>
        public LSLXmlLibraryDataProvider(IEnumerable<string> activeSubsets, bool liveFiltering = true) : base(activeSubsets, liveFiltering)
        {

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
        ///     Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized. </param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var lineInfo = (IXmlLineInfo) reader;
            try
            {
                var serializer = new LSLLibraryDataXmlSerializer();


                serializer.ReadLibrarySubsetDescription += desc =>
                {
                    lineInfo = serializer.CurrentLineInfo;
                    AddSubsetDescription(desc);
                };

                serializer.ReadLibraryFunctionDefinition += signature =>
                {
                    lineInfo = serializer.CurrentLineInfo;
                    DefineFunction(signature);
                };

                serializer.ReadLibraryEventHandlerDefinition += signature =>
                {
                    lineInfo = serializer.CurrentLineInfo;
                    DefineEventHandler(signature);
                };


                serializer.ReadLibraryConstantDefinition += signature =>
                {
                    lineInfo = serializer.CurrentLineInfo;
                    DefineConstant(signature);
                };


                serializer.Parse(reader);
            }
            catch (LSLMissingSubsetDescriptionException e)
            {
                throw new XmlSyntaxException(lineInfo.LineNumber, e.Message);
            }
            catch (LSLDuplicateSubsetDescription e)
            {
                throw new XmlSyntaxException(lineInfo.LineNumber, e.Message);
            }
            catch (LSLDuplicateSignatureException e)
            {
                throw new XmlSyntaxException(lineInfo.LineNumber, e.Message);
            }
        }

        /// <summary>
        ///     Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            LSLLibraryDataXmlSerializer.WriteXml(SubsetDescriptions.Values, LibraryFunctions.SelectMany(x => x), SupportedEventHandlers,
                LibraryConstants, writer, false);
        }


        /// <summary>
        ///     Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
        /// <param name="writeRootElement">Whether or not to write the root element for this object</param>
        /// <param name="rootElementName">The root element name to write the library data into as content.</param>
        public void WriteXml(XmlWriter writer, bool writeRootElement, string rootElementName = "LSLLibraryData")
        {
            LSLLibraryDataXmlSerializer.WriteXml(SubsetDescriptions.Values, LibraryFunctions.SelectMany(x => x), SupportedEventHandlers,
                LibraryConstants, writer, writeRootElement, rootElementName);
        }


        /// <summary>
        ///     Fills a library data provider from an XML reader object
        /// </summary>
        /// <param name="data">The XML reader to read from</param>
        /// <param name="rootElementName">The root element name of the top level XML node containing library data.</param>
        /// <exception cref="ArgumentNullException">When data is null</exception>
        /// <exception cref="XmlException">When a syntax error is encountered</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void FillFromXml(XmlReader data, string rootElementName = "LSLLibraryData")
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            ClearEventHandlers();
            ClearLibraryConstants();
            ClearLibraryFunctions();

            data.ReadStartElement(rootElementName);

            IXmlSerializable serializable = this;
            serializable.ReadXml(data);

            data.ReadEndElement();
        }

    }
}