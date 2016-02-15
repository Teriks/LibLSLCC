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
using LibLSLCC.CodeValidator.Primitives;

#endregion

namespace LibLSLCC.LibraryData
{
    public class SerializeConstantSignatureEventArgs : EventArgs

    {
        public LSLLibraryConstantSignature Signature { get; private set; }


        public SerializeConstantSignatureEventArgs(LSLLibraryConstantSignature signature)
        {
            Signature = signature;
        }
    }


    public class SerializeEventHandlerSignatureEventArgs : EventArgs

    {
        public SerializeEventHandlerSignatureEventArgs(LSLLibraryEventSignature eventSignature)
        {
            EventSignature = eventSignature;
        }


        public LSLLibraryEventSignature EventSignature { get; private set; }
    }


    public class SerializeFunctionSignatureEventArgs : EventArgs

    {
        public SerializeFunctionSignatureEventArgs(LSLLibraryFunctionSignature functionSignature)
        {
            FunctionSignature = functionSignature;
        }


        public LSLLibraryFunctionSignature FunctionSignature { get; private set; }
    }


    public class SerializeSubsetDescriptionEventArgs : EventArgs

    {
        public SerializeSubsetDescriptionEventArgs(LSLLibrarySubsetDescription subsetDescription)
        {
            SubsetDescription = subsetDescription;
        }


        public LSLLibrarySubsetDescription SubsetDescription { get; private set; }
    }


    /// <summary>
    /// An event driven SAX parser for LSL Library data
    /// </summary>
    public class LSLLibraryDataXmlSerializer
    {
        /// <summary>
        /// Info about the current XML line as parsing in progress.
        /// </summary>
        public IXmlLineInfo CurrentLineInfo { get; private set; }

        /// <summary>
        /// The root element name used for LSL Library Data XML
        /// </summary>
        public const string RootElementName = "LSLLibraryData";


        /// <summary>
        /// This event is fired when a function definition has been retrieved from XML markup.
        /// </summary>
        public event EventHandler<SerializeFunctionSignatureEventArgs> ReadLibraryFunctionDefinition;

        /// <summary>
        /// This event is fired when a constant definition has been retrieved from XML markup.
        /// </summary>
        public event EventHandler<SerializeConstantSignatureEventArgs> ReadLibraryConstantDefinition;

        /// <summary>
        /// This event is fired when an event handler definition has been retrieved from XML markup.
        /// </summary>
        public event EventHandler<SerializeEventHandlerSignatureEventArgs> ReadLibraryEventHandlerDefinition;


        /// <summary>
        /// This event is fired when a library subset description has been retrieved from XML markup.
        /// </summary>
        public event EventHandler<SerializeSubsetDescriptionEventArgs> ReadLibrarySubsetDescription;


        /// <summary>
        /// This event is fired when a function definition has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibraryFunctionDefinition(LSLLibraryFunctionSignature sig)
        {
            var handler = ReadLibraryFunctionDefinition;
            if (handler != null) handler(this, new SerializeFunctionSignatureEventArgs(sig));
        }


        /// <summary>
        /// This event is fired when an event handler definition has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibraryEventHandlerDefinition(LSLLibraryEventSignature sig)
        {
            var handler = ReadLibraryEventHandlerDefinition;
            if (handler != null) handler(this, new SerializeEventHandlerSignatureEventArgs(sig));
        }


        /// <summary>
        /// This event is fired when a constant definition has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibraryConstantDefinition(LSLLibraryConstantSignature sig)
        {
            var handler = ReadLibraryConstantDefinition;
            if (handler != null) handler(this, new SerializeConstantSignatureEventArgs(sig));
        }


        /// <summary>
        /// This event is fired when a library subset description has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibrarySubsetDescription(LSLLibrarySubsetDescription desc)
        {
            var handler = ReadLibrarySubsetDescription;
            if (handler != null) handler(this, new SerializeSubsetDescriptionEventArgs(desc));
        }


        /// <summary>
        /// Starts a parse at the current node in the given XmlReader, the default element name to consume the content of is 'LSLLibraryData'
        /// </summary>
        /// <param name="reader">The XmlReader object to read XML from.</param>
        /// <exception cref="LSLLibraryDataXmlSyntaxException">If a syntax error was detected in the Library Data XML.</exception>
        public void Parse(XmlReader reader)
        {
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
        /// Serialize library signature definitions to an XmlWriter object
        /// </summary>
        /// <param name="dataProvider">The data provider to serialize.</param>
        /// <param name="writer">The XmlWriter object to serialize to.</param>
        /// <param name="writeRootElement">Boolean defining whether or not to write a root element to the stream that houses the signatures, or to just write the signatures without putting them in a root element.</param>
        public static void WriteXml(ILSLLibraryDataProvider dataProvider,
            XmlWriter writer,
            bool writeRootElement = true)
        {
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