using System;
using System.Collections.Generic;
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
    public class LSLDefaultLibraryDataProvider : LSLXmlLibraryDataProvider
    {
        protected LSLDefaultLibraryDataProvider()
        {
            
        }



        private HashSet<string> GetSubsets(LSLLibraryBaseData baseDataLSLLibraryBaseData, LSLLibraryDataAdditions dataAdditions)
        {
            var subsets=new HashSet<string>();
            if (baseDataLSLLibraryBaseData == LSLLibraryBaseData.All)
            {
                subsets.Add("all");
                AccumulateDuplicates = true;
                return subsets;
            }

            subsets.Add(baseDataLSLLibraryBaseData == LSLLibraryBaseData.OpensimLsl ? "os-lsl" : "lsl");

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

            return subsets;
        }


        public LSLDefaultLibraryDataProvider(LSLLibraryBaseData libraryBaseData, 
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

                var subsets = GetSubsets(libraryBaseData, dataAdditions);

                FillFromXml(reader, subsets.AsReadOnly());





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