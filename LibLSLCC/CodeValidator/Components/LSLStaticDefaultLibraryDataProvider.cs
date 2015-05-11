using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    ///     The LSLDefaultLibraryDataProvider reads XML from the embedded resource
    ///     LibLSLCC.CodeValidator.Components.LibraryData.StandardLSL.xml
    ///     to define its data
    /// </summary>
    [LSLXmlLibraryDataRoot]
    public class LSLStaticDefaultLibraryDataProvider : LSLStaticXmlLibraryDataProvider
    {
        private LSLLibraryBaseData _liveFilteringBaseLibraryData;
        private LSLLibraryDataAdditions _liveFilteringLibraryDataAdditions;



        protected LSLStaticDefaultLibraryDataProvider()
        {
            
        }



        public LSLLibraryBaseData LiveFilteringBaseLibraryData
        {
            get { return _liveFilteringBaseLibraryData; }
            set
            {
                if (value != _liveFilteringBaseLibraryData)
                {
                    LiveFilteringSubsets = GetSubsets(value, LiveFilteringLibraryDataAdditions);

                    _liveFilteringBaseLibraryData = value;
                }
            }
        }

        public LSLLibraryDataAdditions LiveFilteringLibraryDataAdditions
        {
            get { return _liveFilteringLibraryDataAdditions; }
            set
            {
                if (value != _liveFilteringLibraryDataAdditions)
                {
                    LiveFilteringSubsets = GetSubsets(LiveFilteringBaseLibraryData, value);

                    _liveFilteringLibraryDataAdditions = value;
                }
            }
        }



        private ReadOnlyHashSet<string> GetSubsets(LSLLibraryBaseData libraryBaseData,
            LSLLibraryDataAdditions dataAdditions)
        {
            var subsets = new HashSet<string>();

            subsets.Add(libraryBaseData == LSLLibraryBaseData.OpensimLsl ? "os-lsl" : "lsl");

            if ((dataAdditions & LSLLibraryDataAdditions.OpenSimOssl) == LSLLibraryDataAdditions.OpenSimOssl)
            {
                subsets.Add("ossl");
            }

            if ((dataAdditions & LSLLibraryDataAdditions.OpenSimWindlight) == LSLLibraryDataAdditions.OpenSimWindlight)
            {
                subsets.Add("os-lightshare");
            }

            if ((dataAdditions & LSLLibraryDataAdditions.OpenSimBulletPhysics) == LSLLibraryDataAdditions.OpenSimBulletPhysics)
            {
                subsets.Add("os-bullet-physics");
            }

            if ((dataAdditions & LSLLibraryDataAdditions.OpenSimModInvoke) == LSLLibraryDataAdditions.OpenSimModInvoke)
            {
                subsets.Add("os-mod-api");
            }

            return new ReadOnlyHashSet<string>(subsets);
        }


        public LSLStaticDefaultLibraryDataProvider(bool liveFiltering, LSLLibraryBaseData libraryBaseData, 
            LSLLibraryDataAdditions dataAdditions = LSLLibraryDataAdditions.None)
        {
            using (
                var libraryData =
                    GetType()
                        .Assembly.GetManifestResourceStream(
                            "LibLSLCC.CodeValidator.Components.LibraryData.LSLDefaultLibraryDataProvider.xml"))
            {
                if (libraryData == null)
                {
                    throw new InvalidOperationException(
                        "Could not locate manifest resource LibLSLCC.CodeValidator.Components.Resources.StandardLSLLibraryData.xml");
                }

                var reader = new XmlTextReader(libraryData);

                if (libraryBaseData == LSLLibraryBaseData.All)
                {
                    AccumulateDuplicates = true;
                    FillFromXml(reader, new HashSet<string> { "all" }.AsReadOnly());
                    return;
                }

                if (liveFiltering)
                {
                    
                    AccumulateDuplicates = true;
                    LiveFiltering = true;
                    LiveFilteringSubsets = GetSubsets(libraryBaseData, dataAdditions);
                    _liveFilteringLibraryDataAdditions = dataAdditions;
                    _liveFilteringBaseLibraryData = libraryBaseData;
                    FillFromXml(reader, new HashSet<string> {"all"}.AsReadOnly());
                }
                else
                {
                    FillFromXml(reader, GetSubsets(libraryBaseData, dataAdditions));
                } 
            }
        }
    }

    [Flags]
    public enum LSLLibraryDataAdditions
    {
        None=0,
        OpenSimOssl =1,
        OpenSimWindlight=2,
        OpenSimBulletPhysics=4,
        OpenSimModInvoke=8
    }

    public enum LSLLibraryBaseData
    {
        StandardLsl,
        OpensimLsl,
        All
    }
}