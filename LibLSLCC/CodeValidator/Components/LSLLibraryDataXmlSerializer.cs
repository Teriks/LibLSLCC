#region FileInfo

// 
// File: LSLLibraryDataXmlSerializer.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:24 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
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