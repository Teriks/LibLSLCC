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
    public class LSLStaticXmlLibraryDataProvider : LSLStaticLibraryDataProvider,
        IXmlSerializable
    {


        private HashSet<string> _subsets = new HashSet<string>();


        public IReadOnlySet<string> Subsets
        {
            get { return new ReadOnlyHashSet<string>(_subsets); }
        }




        public bool LiveFiltering { get; protected set; }

        public IReadOnlySet<string> LiveFilteringSubsets { get; protected set; }

        /// <summary>
        /// Enumerable of the LibraryConstants defined according to this data provider
        /// </summary>
        public override IEnumerable<LSLLibraryConstantSignature> LibraryConstants
        {
            get
            {
                if (LiveFiltering)
                {
                    return base.LibraryConstants.Where(x => x.Subsets.Overlaps(LiveFilteringSubsets));
                }

                 return base.LibraryConstants;
                
            }
        }

        /// <summary>
        /// Enumerable of the LibraryFunctions defined according to this data provider
        /// </summary>
        public override IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LibraryFunctions
        {
            get
            {
                if (LiveFiltering)
                {
                    return base.LibraryFunctions.Select(x =>
                    {
                        return x.Where(y => y.Subsets.Overlaps(LiveFilteringSubsets)).ToList();

                    });
                }

                return base.LibraryFunctions;

            }
        }

        /// <summary>
        /// Enumerable of event handlers supported according to this data provider
        /// </summary>
        public override IEnumerable<LSLLibraryEventSignature> SupportedEventHandlers
        {
            get
            {
                if (LiveFiltering)
                {
                    return base.SupportedEventHandlers.Where(x => x.Subsets.Overlaps(LiveFilteringSubsets));
                }

                return base.SupportedEventHandlers;

            }
        }



        /// <summary>
        ///     Return an LSLEventHandlerSignature object describing an event handler signature;
        ///     if the event handler with the given name exists, otherwise null.
        /// </summary>
        /// <param name="name">Name of the event handler</param>
        /// <returns>
        ///     An LSLEventHandlerSignature object describing the given event handlers signature,
        ///     or null if the event handler does not exist.
        /// </returns>
        public override LSLLibraryEventSignature GetEventHandlerSignature(string name)
        {
            if (LiveFiltering)
            {
                var r = base.GetEventHandlerSignature(name);

                if (r == null) return null;

                if (r.Subsets.Overlaps(LiveFilteringSubsets))
                {
                    return r;
                }

                return null;
            }

            return base.GetEventHandlerSignature(name);
        }



        /// <summary>
        ///     Return the library constant if it exists, otherwise null.
        /// </summary>
        /// <param name="name">Name of the library constant.</param>
        /// <returns>
        ///     The library constants signature
        /// </returns>
        public override LSLLibraryConstantSignature GetLibraryConstantSignature(string name)
        {
            if (LiveFiltering)
            {
                var r = base.GetLibraryConstantSignature(name);

                if (r == null) return null;

                if (r.Subsets.Overlaps(LiveFilteringSubsets))
                {
                    return r;
                }

                return null;
            }

            return base.GetLibraryConstantSignature(name);
        }



        /// <summary>
        ///     Return an LSLFunctionSignature list object describing the function call signatures of a library function;
        ///     if the function with the given name exists as a singular or overloaded function, otherwise null.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>
        ///     An LSLFunctionSignature list object describing the given library functions signatures,
        ///     or null if the library function does not exist.
        /// </returns>
        public override IReadOnlyList<LSLLibraryFunctionSignature> GetLibraryFunctionSignatures(string name)
        {
            if (LiveFiltering)
            {
                var r = base.GetLibraryFunctionSignatures(name);

                if (r == null) return null;


                var sigs = r.Where(x => x.Subsets.Overlaps(LiveFilteringSubsets)).ToList();
                return sigs.Count == 0 ? null : sigs;
            }

            return base.GetLibraryFunctionSignatures(name);
        }



        /// <summary>
        ///     Return true if a library constant with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library constant.</param>
        /// <returns>True if a library constant with the given name exists.</returns>
        public override bool LibraryConstantExist(string name)
        {
            if (LiveFiltering)
            {
                return GetLibraryConstantSignature(name) != null;
            }
            return base.LibraryConstantExist(name);
        }



        /// <summary>
        ///     Return true if a library function with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>True if the library function with given name exists.</returns>
        public override bool LibraryFunctionExist(string name)
        {
            if (LiveFiltering)
            {
                var sigs = GetLibraryFunctionSignatures(name);
                return sigs != null;
            }

            return base.LibraryFunctionExist(name);
        }



        /// <summary>
        ///     Return true if an event handler with the given name exists in the default library.
        /// </summary>
        /// <param name="name">Name of the event handler.</param>
        /// <returns>True if the event handler with given name exists.</returns>
        public override bool EventHandlerExist(string name)
        {
            if (LiveFiltering)
            {
                return GetEventHandlerSignature(name) != null;
            }

            return base.EventHandlerExist(name);
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
            IXmlLineInfo lineInfo = (IXmlLineInfo)reader;
            try
            {

                LSLLibraryDataXmlSerializer serializer = new LSLLibraryDataXmlSerializer();

                serializer.ReadLibraryFunctionDefinition += signature =>
                {
                    if (AccumulateDuplicates || signature.Subsets.Any(subset => Subsets.Contains(subset)))
                    {
                        lineInfo = serializer.CurrentLineInfo;
                        AddValidLibraryFunction(signature);

                    }
                };

                serializer.ReadLibraryEventHandlerDefinition += signature =>
                {
                    if (AccumulateDuplicates || signature.Subsets.Any(subset => Subsets.Contains(subset)))
                    {
                        lineInfo = serializer.CurrentLineInfo;
                        AddValidEventHandler(signature);
                    }
                };


                serializer.ReadLibraryConstantDefinition += signature =>
                {
                    if (AccumulateDuplicates || signature.Subsets.Any(subset => Subsets.Contains(subset)))
                    {
                        lineInfo = serializer.CurrentLineInfo;
                        AddValidConstant(signature);
                    }
                };


                serializer.Parse(reader);

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

            LSLLibraryDataXmlSerializer.WriteXml(LibraryFunctions.SelectMany(x=>x),SupportedEventHandlers,LibraryConstants,writer,false);
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