using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    ///     A library data provider that reads LSL library data from XML, if you derive from this
    ///     class, the new class must have the LSLXmlLibraryDataRoot attribute
    /// </summary>
    [LSLXmlLibraryDataRoot]
    public class LSLXmlLibraryDataProvider : LSLLibraryDataProvider,
        IXmlSerializable
    {


        private HashSet<string> _subsets = new HashSet<string>();


        public IReadOnlySet<string> Subsets
        {
            get { return new ReadOnlyHashSet<string>(_subsets); }
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
        public XmlSchema GetSchema()
        {
            return null;
        }




        /// <summary>
        ///     Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized. </param>
        public void ReadXml(XmlReader reader)
        {
            var lineInfo = (IXmlLineInfo) reader;
            try
            {
                var canRead = reader.Read();
                while (canRead)
                {
                    if (reader.Name == "LibraryFunction" && reader.IsStartElement())
                    {
                        var signature = LSLLibraryFunctionSignature.FromXmlFragment(reader);


                        if (AccumulateDuplicates || signature.Subsets.Any(subset => Subsets.Contains(subset)))
                        {
                            AddValidLibraryFunction(signature);

                        }


                        canRead = reader.Read();
                    }
                    else if (reader.Name == "EventHandler" && reader.IsStartElement())
                    {
                        var signature = LSLLibraryEventSignature.FromXmlFragment(reader);


                        if (AccumulateDuplicates || signature.Subsets.Any(subset => Subsets.Contains(subset)))
                        {
                            AddValidEventHandler(signature);
                        }
                        

                        canRead = reader.Read();
                    }
                    else if (reader.Name == "LibraryConstant" && reader.IsStartElement())
                    {
                        var signature = LSLLibraryConstantSignature.FromXmlFragment(reader);


                        if (AccumulateDuplicates || signature.Subsets.Any(subset => Subsets.Contains(subset)))
                        {
                            AddValidConstant(signature);
                        }
                        

                        canRead = reader.Read();
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "LSLLibraryData")
                    {
                        break;
                    }
                    else
                    {
                        canRead = reader.Read();
                    }
                }
            }
            catch (LSLDuplicateSignatureException e)
            {
                throw new XmlSyntaxException(lineInfo.LineNumber,e.Message);
            }
        }



        /// <summary>
        ///     Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
        public void WriteXml(XmlWriter writer)
        {
            foreach (var func in LibraryFunctions.SelectMany(x=>x))
            {
                writer.WriteStartElement("LibraryFunction");
                func.WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var func in SupportedEventHandlers)
            {
                writer.WriteStartElement("EventHandler");
                func.WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var func in LibraryConstants)
            {
                writer.WriteStartElement("LibraryConstant");
                func.WriteXml(writer);
                writer.WriteEndElement();
            }
        }



        /// <summary>
        ///     Fills a library data provider from an XML reader object
        /// </summary>
        /// <param name="data">The xml reader to read from</param>
        /// <param name="subsets">
        /// Data nodes must contain one of these subset strings in their Subsets property, otherwise they are discarded. 
        /// when "all" is used, all nodes are added and duplicates are accumulated into DuplicateEventsDefined, DuplicateConstantsDefined
        /// and DuplicateFunctionsDefined</param>
        /// <exception cref="ArgumentNullException">When data is null</exception>
        /// <exception cref="XmlException">When a syntax error is encountered</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void FillFromXml(XmlReader data, IReadOnlySet<string> subsets)
        {


           
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            ClearEventHandlers();
            ClearLibraryConstants();
            ClearLibraryFunctions();

            _subsets=new HashSet<string>(subsets);

            if (_subsets.Contains("all"))
            {
                AccumulateDuplicates = true;
            }

            data.ReadStartElement(LSLXmlLibraryDataRootAttribute.RootElementName);

            ReadXml(data);

            data.ReadEndElement();
        }
    }
}