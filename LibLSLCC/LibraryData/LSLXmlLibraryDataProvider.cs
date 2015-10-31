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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#endregion

namespace LibLSLCC.LibraryData
{
    /// <summary>
    /// Indicates to the XML library data provider what library data elements should be loaded.
    /// </summary>
    [Flags]
    public enum LSLLibraryDataLoadOptions
    {
        /// <summary>
        /// Constants, Functions and Event handler definitions should all be loaded.
        /// </summary>
        All = Functions|Constants|Events,

        /// <summary>
        /// Load Function definitions.
        /// </summary>
        Functions = 1,

        /// <summary>
        /// Load Constant definitions.
        /// </summary>
        Constants = 2,

        /// <summary>
        /// Load Event definitions.
        /// </summary>
        Events = 4
    }

    /// <summary>
    ///     A library data provider that reads LSL library data from XML
    /// </summary>
    public class LSLXmlLibraryDataProvider : LSLLibraryDataProvider,
        IXmlSerializable
    {

        /// <summary>
        /// The root element name used for LSL Library Data XML
        /// </summary>
        public static readonly string RootElementName = LSLLibraryDataXmlSerializer.RootElementName;


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
        /// <exception cref="LSLLibraryDataXmlSyntaxException">If a syntax error was detected in the Library Data XML.</exception>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            _ReadXml(reader);
        }

        private void _ReadXml(XmlReader reader, LSLLibraryDataLoadOptions loadOptions = LSLLibraryDataLoadOptions.All)
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

                if((loadOptions & LSLLibraryDataLoadOptions.Functions) == LSLLibraryDataLoadOptions.Functions)
                {
                    serializer.ReadLibraryFunctionDefinition += signature =>
                    {
                        lineInfo = serializer.CurrentLineInfo;
                        DefineFunction(signature);
                    };
                }

                if ((loadOptions & LSLLibraryDataLoadOptions.Events) == LSLLibraryDataLoadOptions.Events)
                {
                    serializer.ReadLibraryEventHandlerDefinition += signature =>
                    {
                        lineInfo = serializer.CurrentLineInfo;
                        DefineEventHandler(signature);
                    };
                }

                if ((loadOptions & LSLLibraryDataLoadOptions.Constants) == LSLLibraryDataLoadOptions.Constants)
                {
                    serializer.ReadLibraryConstantDefinition += signature =>
                    {
                        lineInfo = serializer.CurrentLineInfo;
                        DefineConstant(signature);
                    };
                }

                serializer.Parse(reader);
            }
            //LSLLibraryDataXmlSerializer.Parse() handles XmlSyntaxExceptions by throwing an LSLLibraryDataXmlSyntaxException.
            catch (LSLMissingSubsetDescriptionException e)
            {
                throw new LSLLibraryDataXmlSyntaxException(lineInfo.LineNumber, e.Message, e);
            }
            catch (LSLDuplicateSubsetDescriptionException e)
            {
                throw new LSLLibraryDataXmlSyntaxException(lineInfo.LineNumber, e.Message, e);
            }
            catch (LSLDuplicateSignatureException e)
            {
                throw new LSLLibraryDataXmlSyntaxException(lineInfo.LineNumber, e.Message, e);
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
        public void WriteXml(XmlWriter writer, bool writeRootElement)
        {
            LSLLibraryDataXmlSerializer.WriteXml(SubsetDescriptions.Values, LibraryFunctions.SelectMany(x => x), SupportedEventHandlers,
                LibraryConstants, writer, writeRootElement);
        }


        /// <summary>
        /// Fills a library data provider from an XML reader object, the data provider is cleared of all definitions first.
        /// </summary>
        /// <param name="data">The XML reader to read from.</param>
        /// <param name="loadOptions">Optionally specifies what type's of library definitions will be loaded, defaults to <see cref="LSLLibraryDataLoadOptions.All"/></param>
        /// <exception cref="ArgumentNullException">When the 'data' parameter is null.</exception>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">If a syntax error was detected in the XML (Attribute value did not pass pattern validation.. etc..)</exception>
        /// <exception cref="XmlException">If incorrect XML was encountered in the input stream.</exception>
        public void FillFromXml(XmlReader data, LSLLibraryDataLoadOptions loadOptions = LSLLibraryDataLoadOptions.All)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            ClearLibraryData();

            _ReadXml(data, loadOptions);

            data.ReadEndElement();
        }


        /// <summary>
        /// Adds additional library data provider from an XML reader object, the data provider is not cleared first.
        /// </summary>
        /// <param name="data">The XML reader to read from.</param>
        /// <param name="loadOptions">Optionally specifies what type's of library definitions will be loaded, defaults to <see cref="LSLLibraryDataLoadOptions.All"/></param>
        /// <exception cref="ArgumentNullException">When the 'data' parameter is null.</exception>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">If a syntax error was detected in the XML (Attribute value did not pass pattern validation.. etc..)</exception>
        /// <exception cref="XmlException">If incorrect XML was encountered in the input stream.</exception>
        public void AddFromXml(XmlReader data, LSLLibraryDataLoadOptions loadOptions = LSLLibraryDataLoadOptions.All)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            data.ReadStartElement(RootElementName);

            _ReadXml(data, loadOptions);

            data.ReadEndElement();
        }


        /// <summary>
        /// Adds additional library data provider from an XML file, the data provider is not cleared first.
        /// Encoding is detected using the BOM (Byte Order Mark) of the file.
        /// </summary>
        /// <param name="filename">The XML file to read library data from.</param>
        /// <param name="loadOptions">Optionally specifies what type's of library definitions will be loaded, defaults to <see cref="LSLLibraryDataLoadOptions.All"/></param>
        /// <exception cref="ArgumentException">When the 'filename' parameter is whitespace.</exception>
        /// <exception cref="ArgumentNullException">When the 'filename' parameter is null.</exception>
        /// <exception cref="FileNotFoundException">When the file in the 'filename' parameter could not be found.</exception>
        /// <exception cref="DirectoryNotFoundException">When the path in the 'filename' parameter is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="IOException">When the path in the 'filename' parameter includes an incorrect or invalid syntax for a file name, directory name, or volume label.</exception>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">If a syntax error was detected in the XML (Attribute value did not pass pattern validation.. etc..)</exception>
        /// <exception cref="XmlException">If incorrect XML was encountered in the input stream.</exception>
        public void AddFromXml(string filename, LSLLibraryDataLoadOptions loadOptions = LSLLibraryDataLoadOptions.All)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("filename");
            }

            using (var reader = new XmlTextReader(new StreamReader(filename, true)))
            {

                reader.ReadStartElement(RootElementName);

                _ReadXml(reader, loadOptions);

                reader.ReadEndElement();
            }
        }


        /// <summary>
        /// Adds additional library data by parsing every XML file in a given directory, the data provider is not cleared first.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <param name="loadOptions">Optionally specifies what type's of library definitions will be loaded, defaults to <see cref="LSLLibraryDataLoadOptions.All"/></param>
        /// <exception cref="System.ArgumentNullException">path</exception>
        /// <exception cref="System.ArgumentException">path</exception>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">If a syntax error was detected in an XML file (Attribute value did not pass pattern validation.. etc..)</exception>
        /// <exception cref="DirectoryNotFoundException">When the path in the 'path' parameter is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="IOException">If an IOException occurs while reading a file.</exception>
        public void AddFromXmlDirectory(string path, LSLLibraryDataLoadOptions loadOptions = LSLLibraryDataLoadOptions.All)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("path");
            }

            foreach (var file in Directory.EnumerateFiles(path, "*.xml"))
            {
                try
                {
                    AddFromXml(file, loadOptions);
                }
                catch (LSLLibraryDataXmlSyntaxException e)
                {
                    throw new LSLLibraryDataXmlSyntaxException(string.Format("Error Parsing File {0}: {1}", file, e.Message), e);
                }
            }
        }


        /// <summary>
        /// Fills a library data provider by parsing every XML file in a given directory, the data provider is cleared of all definitions first.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <param name="loadOptions">Optionally specifies what type's of library definitions will be loaded, defaults to <see cref="LSLLibraryDataLoadOptions.All"/></param>
        /// <exception cref="System.ArgumentNullException">path</exception>
        /// <exception cref="System.ArgumentException">path</exception>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">If a syntax error was detected in an XML file (Attribute value did not pass pattern validation.. etc..)</exception>
        /// <exception cref="DirectoryNotFoundException">When the path in the 'path' parameter is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="IOException">If an IOException occurs while reading a file.</exception>
        public void FillFromXmlDirectory(string path, LSLLibraryDataLoadOptions loadOptions = LSLLibraryDataLoadOptions.All)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("path");
            }

            ClearLibraryData();

            foreach (var file in Directory.EnumerateFiles(path, "*.xml"))
            {
                try
                {
                    AddFromXml(file, loadOptions);
                }
                catch (LSLLibraryDataXmlSyntaxException e)
                {
                    throw new LSLLibraryDataXmlSyntaxException(string.Format("Error Parsing File '{0}': {1}", file, e.Message), e);
                }
            }
        }

        /// <summary>
        /// Fills a library data provider from an XML reader object, the data provider is cleared of all definitions first.
        /// Encoding is detected using the BOM (Byte Order Mark) of the file.
        /// </summary>
        /// <param name="filename">The XML file to read library data from.</param>
        /// <param name="loadOptions">Optionally specifies what type's of library definitions will be loaded, defaults to <see cref="LSLLibraryDataLoadOptions.All"/></param>
        /// <exception cref="ArgumentException">When the 'filename' parameter is whitespace.</exception>
        /// <exception cref="ArgumentNullException">When the 'filename' parameter is null.</exception>
        /// <exception cref="FileNotFoundException">When the file in the 'filename' parameter could not be found.</exception>
        /// <exception cref="DirectoryNotFoundException">When the path in the 'filename' parameter is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="IOException">When the path in the 'filename' parameter includes an incorrect or invalid syntax for a file name, directory name, or volume label.</exception>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">If a syntax error was detected in the XML (Attribute value did not pass pattern validation.. etc..)</exception>
        /// <exception cref="XmlException">If incorrect XML was encountered in the input stream.</exception>
        public void FillFromXml(string filename, LSLLibraryDataLoadOptions loadOptions = LSLLibraryDataLoadOptions.All)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("filename");
            }

            ClearLibraryData();

            using (var reader = new XmlTextReader(new StreamReader(filename, true)))
            {

                reader.ReadStartElement(RootElementName);

                _ReadXml(reader, loadOptions);

                reader.ReadEndElement();
            }
        }


    }
}