#region FileInfo

// 
// File: LSLLibraryDataXmlSerializer.cs
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
using System.Linq;
using System.Security;
using System.Xml;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator;

#endregion

namespace LibLSLCC.LibraryData
{
    /// <summary>
    ///     EventArgs for <see cref="LSLLibraryDataXmlSerializer.ReadLibraryConstantDefinition" />
    /// </summary>
    public class SerializeConstantSignatureEventArgs : EventArgs

    {
        /// <summary>
        ///     Constructs the event args around a given <see cref="LSLLibraryConstantSignature" /> that was just read.
        /// </summary>
        /// <param name="signature"></param>
        public SerializeConstantSignatureEventArgs(LSLLibraryConstantSignature signature)
        {
            Signature = signature;
        }


        /// <summary>
        ///     The <see cref="LSLLibraryConstantSignature" /> node that was read.
        /// </summary>
        public LSLLibraryConstantSignature Signature { get; private set; }
    }

    /// <summary>
    ///     EventArgs for <see cref="LSLLibraryDataXmlSerializer.ReadLibraryEventHandlerDefinition" />
    /// </summary>
    public class SerializeEventHandlerSignatureEventArgs : EventArgs

    {
        /// <summary>
        ///     Constructs the event args around a given <see cref="LSLLibraryEventSignature" /> that was just read.
        /// </summary>
        public SerializeEventHandlerSignatureEventArgs(LSLLibraryEventSignature eventSignature)
        {
            EventSignature = eventSignature;
        }


        /// <summary>
        ///     The <see cref="LSLLibraryEventSignature" /> node that was read.
        /// </summary>
        public LSLLibraryEventSignature EventSignature { get; private set; }
    }

    /// <summary>
    ///     EventArgs for <see cref="LSLLibraryDataXmlSerializer.ReadLibraryFunctionDefinition" />
    /// </summary>
    public class SerializeFunctionSignatureEventArgs : EventArgs

    {
        /// <summary>
        ///     Constructs the event args around a given <see cref="LSLLibraryFunctionSignature" /> that was just read.
        /// </summary>
        public SerializeFunctionSignatureEventArgs(LSLLibraryFunctionSignature functionSignature)
        {
            FunctionSignature = functionSignature;
        }


        /// <summary>
        ///     The <see cref="LSLLibraryFunctionSignature" /> node that was read.
        /// </summary>
        public LSLLibraryFunctionSignature FunctionSignature { get; private set; }
    }

    /// <summary>
    ///     EventArgs for <see cref="LSLLibraryDataXmlSerializer.ReadLibrarySubsetDescription" />
    /// </summary>
    public class SerializeSubsetDescriptionEventArgs : EventArgs

    {
        /// <summary>
        ///     Constructs the event args around a given <see cref="LSLLibrarySubsetDescription" /> that was just read.
        /// </summary>
        public SerializeSubsetDescriptionEventArgs(LSLLibrarySubsetDescription subsetDescription)
        {
            SubsetDescription = subsetDescription;
        }


        /// <summary>
        ///     The <see cref="LSLLibrarySubsetDescription" /> node that was read.
        /// </summary>
        public LSLLibrarySubsetDescription SubsetDescription { get; private set; }
    }


    /// <summary>
    ///     An event driven SAX parser for LSL Library data
    /// </summary>
    public class LSLLibraryDataXmlSerializer
    {
        /// <summary>
        ///     The root element name used for LSL Library Data XML
        /// </summary>
        public const string RootElementName = "LSLLibraryData";

        /// <summary>
        ///     Info about the current XML line as parsing in progress.
        /// </summary>
        public IXmlLineInfo CurrentLineInfo { get; private set; }

        /// <summary>
        ///     This event is fired when a function definition has been retrieved from XML markup.
        /// </summary>
        public event EventHandler<SerializeFunctionSignatureEventArgs> ReadLibraryFunctionDefinition;

        /// <summary>
        ///     This event is fired when a constant definition has been retrieved from XML markup.
        /// </summary>
        public event EventHandler<SerializeConstantSignatureEventArgs> ReadLibraryConstantDefinition;

        /// <summary>
        ///     This event is fired when an event handler definition has been retrieved from XML markup.
        /// </summary>
        public event EventHandler<SerializeEventHandlerSignatureEventArgs> ReadLibraryEventHandlerDefinition;

        /// <summary>
        ///     This event is fired when a library subset description has been retrieved from XML markup.
        /// </summary>
        public event EventHandler<SerializeSubsetDescriptionEventArgs> ReadLibrarySubsetDescription;


        /// <summary>
        ///     This event is fired when a function definition has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibraryFunctionDefinition(LSLLibraryFunctionSignature sig)
        {
            var handler = ReadLibraryFunctionDefinition;
            if (handler != null) handler(this, new SerializeFunctionSignatureEventArgs(sig));
        }


        /// <summary>
        ///     This event is fired when an event handler definition has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibraryEventHandlerDefinition(LSLLibraryEventSignature sig)
        {
            var handler = ReadLibraryEventHandlerDefinition;
            if (handler != null) handler(this, new SerializeEventHandlerSignatureEventArgs(sig));
        }


        /// <summary>
        ///     This event is fired when a constant definition has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibraryConstantDefinition(LSLLibraryConstantSignature sig)
        {
            var handler = ReadLibraryConstantDefinition;
            if (handler != null) handler(this, new SerializeConstantSignatureEventArgs(sig));
        }


        /// <summary>
        ///     This event is fired when a library subset description has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibrarySubsetDescription(LSLLibrarySubsetDescription desc)
        {
            var handler = ReadLibrarySubsetDescription;
            if (handler != null) handler(this, new SerializeSubsetDescriptionEventArgs(desc));
        }


        /// <summary>
        ///     Starts a parse at the current node in the given XmlReader, the default element name to consume the content of is
        ///     'LSLLibraryData'
        /// </summary>
        /// <param name="reader">The XmlReader object to read XML from.</param>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">If a syntax error was detected in the Library Data XML.</exception>
        /// <exception cref="XmlException">An error occurred while parsing the XML. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is <c>null</c>.</exception>
        public void Parse(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            CurrentLineInfo = (IXmlLineInfo) reader;

            try
            {
                var canRead = reader.Read();
                while (canRead)
                {
                    if (reader.Name == "SubsetDescription" && reader.IsStartElement())
                    {
                        var desc = LSLLibrarySubsetDescription.FromXmlFragment(reader);

                        OnReadLibrarySubsetDescription(desc);

                        canRead = reader.Read();
                    }
                    else if (reader.Name == "LibraryFunction" && reader.IsStartElement())
                    {
                        var signature = LSLLibraryFunctionSignature.FromXmlFragment(reader);

                        OnReadLibraryFunctionDefinition(signature);


                        canRead = reader.Read();
                    }
                    else if (reader.Name == "EventHandler" && reader.IsStartElement())
                    {
                        var signature = LSLLibraryEventSignature.FromXmlFragment(reader);


                        OnReadLibraryEventHandlerDefinition(signature);


                        canRead = reader.Read();
                    }
                    else if (reader.Name == "LibraryConstant" && reader.IsStartElement())
                    {
                        var signature = LSLLibraryConstantSignature.FromXmlFragment(reader);


                        OnReadLibraryConstantDefinition(signature);


                        canRead = reader.Read();
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == RootElementName)
                    {
                        break;
                    }
                    else
                    {
                        canRead = reader.Read();
                    }
                }
            }
            catch (LSLLibraryDataInvalidConstantTypeException e)
            {
                throw new LSLLibraryDataXmlSyntaxException(CurrentLineInfo.LineNumber, e.Message, e);
            }
            catch (LSLInvalidConstantValueStringException e)
            {
                throw new LSLLibraryDataXmlSyntaxException(CurrentLineInfo.LineNumber, e.Message, e);
            }
            catch (LSLInvalidSymbolNameException e)
            {
                throw new LSLLibraryDataXmlSyntaxException(CurrentLineInfo.LineNumber, e.Message, e);
            }
            catch (LSLInvalidSubsetNameException e)
            {
                throw new LSLLibraryDataXmlSyntaxException(CurrentLineInfo.LineNumber, e.Message, e);
            }
            catch (XmlSyntaxException e)
            {
                throw new LSLLibraryDataXmlSyntaxException(CurrentLineInfo.LineNumber, e.Message, e);
            }
        }


        /// <summary>
        ///     Serialize library signature definitions to an XmlWriter object
        /// </summary>
        /// <param name="dataProvider">The data provider to serialize.</param>
        /// <param name="writer">The XmlWriter object to serialize to.</param>
        /// <param name="writeRootElement">
        ///     Boolean defining whether or not to write a root element to the stream that houses the
        ///     signatures, or to just write the signatures without putting them in a root element.
        /// </param>
        /// <exception cref="InvalidOperationException"><paramref name="writer" /> is closed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dataProvider"/> or <paramref see="writer"/> is <c>null</c>.</exception>
        public static void WriteXml(ILSLLibraryDataProvider dataProvider,
            XmlWriter writer,
            bool writeRootElement = true)
        {
            if (dataProvider == null) throw new ArgumentNullException("dataProvider");
            if (writer == null) throw new ArgumentNullException("writer");

            if (writeRootElement)
            {
                writer.WriteStartElement(RootElementName);
            }


            foreach (var func in dataProvider.SubsetDescriptions.Values)
            {
                writer.WriteStartElement("SubsetDescription");
                ((IXmlSerializable) func).WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var func in dataProvider.LibraryFunctions.SelectMany(x => x))
            {
                writer.WriteStartElement("LibraryFunction");
                ((IXmlSerializable) func).WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var ev in dataProvider.LibraryEvents)
            {
                writer.WriteStartElement("EventHandler");
                ((IXmlSerializable) ev).WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var con in dataProvider.LibraryConstants)
            {
                writer.WriteStartElement("LibraryConstant");
                ((IXmlSerializable) con).WriteXml(writer);
                writer.WriteEndElement();
            }

            if (writeRootElement)
            {
                writer.WriteEndElement();
            }
        }
    }
}