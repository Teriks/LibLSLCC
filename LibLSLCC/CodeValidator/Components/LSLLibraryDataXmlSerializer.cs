using System.Collections.Generic;
using System.Xml;

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLLibraryDataXmlSerializer
    {
        public delegate void LibraryFunctionSignatureEvent(LSLLibraryFunctionSignature sig);
        public delegate void LibraryConstantSignatureEvent(LSLLibraryConstantSignature sig);
        public delegate void LibraryEventHandlerSignatureEvent(LSLLibraryEventSignature sig);


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

        public IXmlLineInfo CurrentLineInfo { get; private set; }



        public void Parse(XmlReader reader, string rootElementName = "LSLLibraryData")
        {
            CurrentLineInfo = (IXmlLineInfo)reader;

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