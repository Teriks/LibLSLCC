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

using System.Collections.Generic;
using System.Security;
using System.Xml;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator.Exceptions;

#endregion

namespace LibLSLCC.CodeValidator.Components
{

    /// <summary>
    /// An event driven SAX parser for LSL Library data
    /// </summary>
    public class LSLLibraryDataXmlSerializer
    {
        /// <summary>
        /// Delegate for the ReadLibraryConstantDefinition event.
        /// </summary>
        /// <param name="sig">The signature that gets passed into the event when its read from XML.</param>
        public delegate void LibraryConstantSignatureEvent(LSLLibraryConstantSignature sig);


        /// <summary>
        /// Delegate for the ReadLibraryEventHandlerDefinition event.
        /// </summary>
        /// <param name="sig">The signature that gets passed into the event when its read from XML.</param>
        public delegate void LibraryEventHandlerSignatureEvent(LSLLibraryEventSignature sig);


        /// <summary>
        /// Delegate for the ReadLibraryFunctionDefinition event.
        /// </summary>
        /// <param name="sig">The signature that gets passed into the event when its read from XML.</param>
        public delegate void LibraryFunctionSignatureEvent(LSLLibraryFunctionSignature sig);


        /// <summary>
        /// Delegate for the ReadLibrarySubsetDescription event.
        /// </summary>
        /// <param name="desc">The description object that gets passed into the event when its read from XML.</param>
        public delegate void LibrarySubsetDescriptionEvent(LSLLibrarySubsetDescription desc);


        /// <summary>
        /// Info about the current XML line as parsing in progress.
        /// </summary>
        public IXmlLineInfo CurrentLineInfo { get; private set; }


        /// <summary>
        /// This event is fired when a function definition has been retrieved from XML markup.
        /// </summary>
        public event LibraryFunctionSignatureEvent ReadLibraryFunctionDefinition;

        /// <summary>
        /// This event is fired when a constant definition has been retrieved from XML markup.
        /// </summary>
        public event LibraryConstantSignatureEvent ReadLibraryConstantDefinition;

        /// <summary>
        /// This event is fired when an event handler definition has been retrieved from XML markup.
        /// </summary>
        public event LibraryEventHandlerSignatureEvent ReadLibraryEventHandlerDefinition;


        /// <summary>
        /// This event is fired when a library subset description has been retrieved from XML markup.
        /// </summary>
        public event LibrarySubsetDescriptionEvent ReadLibrarySubsetDescription;


        /// <summary>
        /// This event is fired when a function definition has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibraryFunctionDefinition(LSLLibraryFunctionSignature sig)
        {
            var handler = ReadLibraryFunctionDefinition;
            if (handler != null) handler(sig);
        }


        /// <summary>
        /// This event is fired when an event handler definition has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibraryEventHandlerDefinition(LSLLibraryEventSignature sig)
        {
            var handler = ReadLibraryEventHandlerDefinition;
            if (handler != null) handler(sig);
        }

        /// <summary>
        /// This event is fired when a constant definition has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibraryConstantDefinition(LSLLibraryConstantSignature sig)
        {
            var handler = ReadLibraryConstantDefinition;
            if (handler != null) handler(sig);
        }



        /// <summary>
        /// This event is fired when a library subset description has been retrieved from XML markup.
        /// </summary>
        protected virtual void OnReadLibrarySubsetDescription(LSLLibrarySubsetDescription desc)
        {
            var handler = ReadLibrarySubsetDescription;
            if (handler != null) handler(desc);
        }


        /// <summary>
        /// Starts a parse at the current node in the given XmlReader, the default element name to consume the content of is 'LSLLibraryData'
        /// </summary>
        /// <param name="reader">The XmlReader object to read XML from.</param>
        /// <param name="rootElementName">The name of the root element, it will be consumed first and the content will be read from it.</param>
        /// <exception cref="XmlSyntaxException">If a syntax error was detected in the XML.</exception>
        public void Parse(XmlReader reader, string rootElementName = "LSLLibraryData")
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
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == rootElementName)
                    {
                        break;
                    }
                    else
                    {
                        canRead = reader.Read();
                    }
                }
            }
            catch (LSLInvalidSymbolNameException e)
            {
                throw new XmlSyntaxException(CurrentLineInfo.LineNumber, e.Message);
            }
            catch (LSLInvalidSubsetNameException e)
            {
                throw new XmlSyntaxException(CurrentLineInfo.LineNumber, e.Message);
            }
        }


        /// <summary>
        /// Serialize library signature definitions to an XmlWriter object/
        /// </summary>
        /// <param name="librarySubsetDescriptions">The library subset descriptions to serialize.</param>
        /// <param name="libraryFunctions">The library function signatures to serialize.</param>
        /// <param name="libraryEventSignatures">The library event handler signatures to serialize.</param>
        /// <param name="libraryConstants">The library constant signatures to serialize</param>
        /// <param name="writer">The XmlWriter object to serialize to.</param>
        /// <param name="writeRootElement">Boolean defining whether or not to write a root element to the stream that houses the signatures, or to just write the signatures without putting them in a root element.</param>
        /// <param name="rootElementName">The name of the root element, which is houses the serialized library signature definitions.  The default name is 'LSLLibraryData'</param>
        public static void WriteXml(
            IEnumerable<LSLLibrarySubsetDescription> librarySubsetDescriptions,
            IEnumerable<LSLLibraryFunctionSignature> libraryFunctions,
            IEnumerable<LSLLibraryEventSignature> libraryEventSignatures,
            IEnumerable<LSLLibraryConstantSignature> libraryConstants,
            XmlWriter writer,
            bool writeRootElement = true,
            string rootElementName = "LSLLibraryData")
        {
            if (writeRootElement)
            {
                writer.WriteStartElement(rootElementName);
            }


            foreach (var func in librarySubsetDescriptions)
            {
                writer.WriteStartElement("SubsetDescription");
                ((IXmlSerializable)func).WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var func in libraryFunctions)
            {
                writer.WriteStartElement("LibraryFunction");
                ((IXmlSerializable)func).WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var ev in libraryEventSignatures)
            {
                writer.WriteStartElement("EventHandler");
                ((IXmlSerializable)ev).WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var con in libraryConstants)
            {
                writer.WriteStartElement("LibraryConstant");
                ((IXmlSerializable)con).WriteXml(writer);
                writer.WriteEndElement();
            }

            if (writeRootElement)
            {
                writer.WriteEndElement();
            }
        }

    }
}