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
using System.Xml;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLLibraryDataXmlSerializer
    {
        public delegate void LibraryConstantSignatureEvent(LSLLibraryConstantSignature sig);

        public delegate void LibraryEventHandlerSignatureEvent(LSLLibraryEventSignature sig);

        public delegate void LibraryFunctionSignatureEvent(LSLLibraryFunctionSignature sig);

        public IXmlLineInfo CurrentLineInfo { get; private set; }
        public event LibraryFunctionSignatureEvent ReadLibraryFunctionDefinition;
        public event LibraryConstantSignatureEvent ReadLibraryConstantDefinition;
        public event LibraryEventHandlerSignatureEvent ReadLibraryEventHandlerDefinition;

        protected virtual void OnReadLibraryFunctionDefinition(LSLLibraryFunctionSignature sig)
        {
            var handler = ReadLibraryFunctionDefinition;
            if (handler != null) handler(sig);
        }

        protected virtual void OnReadLibraryEventHandlerDefinition(LSLLibraryEventSignature sig)
        {
            var handler = ReadLibraryEventHandlerDefinition;
            if (handler != null) handler(sig);
        }

        protected virtual void OnReadLibraryConstantDefinition(LSLLibraryConstantSignature sig)
        {
            var handler = ReadLibraryConstantDefinition;
            if (handler != null) handler(sig);
        }

        public void Parse(XmlReader reader, string rootElementName = "LSLLibraryData")
        {
            CurrentLineInfo = (IXmlLineInfo) reader;

            var canRead = reader.Read();
            while (canRead)
            {
                if (reader.Name == "LibraryFunction" && reader.IsStartElement())
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

        public static void WriteXml(
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

            foreach (var func in libraryFunctions)
            {
                writer.WriteStartElement("LibraryFunction");
                func.WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var func in libraryEventSignatures)
            {
                writer.WriteStartElement("EventHandler");
                func.WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var func in libraryConstants)
            {
                writer.WriteStartElement("LibraryConstant");
                func.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (writeRootElement)
            {
                writer.WriteEndElement();
            }
        }
    }
}