#region


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator.Components;
using LibraryDataScrapingTools.LibraryDataScrapers;
using LibraryDataScrapingTools.LibraryDataScrapers.FirestormLibraryDataDom;
using LibraryDataScrapingTools.OpenSimLibraryReflection;
using LibraryDataScrapingTools.ScraperInterfaces;
using LibraryDataScrapingTools.ScraperProxys;


#endregion

namespace LibraryDataScrapingTools
{
    public static class Log
    {
        public static List<TextWriter> LogWriters = new List<TextWriter>();

        public static void WriteLine(string format, params object[] args)
        {
            foreach (var textWriter in LogWriters)
            {
                textWriter.WriteLine(format, args);
            }
        }

        public static void Write(string format, params object[] args)
        {
            foreach (var textWriter in LogWriters)
            {
                textWriter.Write(format, args);
            }
        }
    }

    internal class Program
    {
        public static LSLXmlLibraryDataProvider ScrapeData(IDocumentationProvider documentation,
            OpenSimLibraryReflectedTypeData openSimScriptFramework)
        {
            var data = new LSLXmlLibraryDataProvider();

            var osslLibraryWikiData = new OsslWikiLibraryDataScraper(
                documentation, new[] { "ossl" });



            var lslLibraryData = new SecondlifeWikiLibraryData(documentation,
                new[] {"lsl"});

            


            var openSimLightShare = new OpenSimDirectLibraryData(openSimScriptFramework, documentation);

            openSimLightShare.IncludeFunctionContainingInterface("ILS_Api",
                new[] {"os-lightshare"});


            var openSimLindenSubset = new OpenSimDirectLibraryData(openSimScriptFramework, documentation);


            openSimLindenSubset.IncludeScriptConstantContainerClass(openSimScriptFramework.ScriptBaseClass,
                new[] {"os-lsl"});
            openSimLindenSubset.IncludeFunctionContainingInterface("ILSL_Api",
                new[] {"os-lsl"});


            var openSimOsslSubset = new OpenSimDirectLibraryData(openSimScriptFramework, documentation);

            openSimOsslSubset.IncludeScriptConstantContainerClass(openSimScriptFramework.ScriptBaseClass,
                new[] {"ossl"});
            openSimOsslSubset.IncludeFunctionContainingInterface("IOSSL_Api",
                new[] {"ossl"});


            var extendedPhysicsSubset = new OpenSimDirectLibraryData(openSimScriptFramework, documentation);

            extendedPhysicsSubset.IncludeAttributedModuleClass("ExtendedPhysics",
                new[] {"os-bullet-physics"});

            var modInvokeSubset = new OpenSimDirectLibraryData(openSimScriptFramework, documentation);

            modInvokeSubset.IncludeFunctionContainingInterface("IMOD_Api",
                new[] { "os-mod-api" });


            var allData = new CompoundLibraryData(
                lslLibraryData, osslLibraryWikiData,
                openSimLightShare, extendedPhysicsSubset, 
                modInvokeSubset);


            foreach (var f in allData.LSLConstants())
            {
                if (f.Name.StartsWith("WL"))
                {
                    f.SetSubsets("os-lightshare");
                    Log.WriteLine(
                        "General: Constant {0} is light share specific, setting subset to os-lightshare",
                        f.Name);
                }
                else if (openSimLindenSubset.LSLConstantExist(f.Name))
                {
                    if (!f.Subsets.Contains("ossl"))
                    {
                        f.AddSubsets("os-lsl");
                    }
                    Log.WriteLine(
                        "General: Linden constant {0} exist in the OpenSim codebase, adding os-lsl subset",
                        f.Name);
                }

                else
                {
                    if (f.Subsets.Contains("ossl") && openSimOsslSubset.LSLConstantExist(f.Name))
                    {
                        Log.WriteLine(
                            "General: Opensim constant {0} exist in the OpenSim codebase",
                            f.Name);
                    }
                    else
                    {
                        Log.WriteLine(
                            "General: Linden constant {0} DOES NOT exist in the OpenSim codebase",
                            f.Name);
                    }
                }
                data.AddValidConstant(f);
            }

            foreach (var f in allData.LSLFunctions())
            {
                if (openSimLindenSubset.LSLFunctionExist(f.Name))
                {
                    f.AddSubsets("os-lsl");
                    Log.WriteLine(
                        "General: Linden function {0} exist in the OpenSim codebase, adding os-lsl subset",
                        f.Name);
                }
                else
                {
                    if (f.Subsets.Contains("ossl") && openSimOsslSubset.LSLFunctionExist(f.Name))
                    {
                        Log.WriteLine(
                            "General: Opensim function {0} exist in the OpenSim codebase",
                            f.Name);
                    }
                    else
                    {
                        Log.WriteLine(
                            "General: Linden function {0} DOES NOT exist in the OpenSim codebase",
                            f.Name);
                    }
                }
                data.AddValidLibraryFunction(f);
            }


            foreach (var f in lslLibraryData.LSLEvents())
            {
                f.AddSubsets("os-lsl");

                data.AddValidEventHandler(f);
            }


            return data;
        }


        private static void CheckMissingOpensimKeywords(OpenSimLibraryReflectedTypeData openSimLibraryReflectedTypeData,
            LSLLibraryDataProvider existingData)
        {
            var openSim = new OpenSimDirectLibraryData(openSimLibraryReflectedTypeData);
            openSim.IncludeScriptConstantContainerClass(openSimLibraryReflectedTypeData.ScriptBaseClass,
                new[] {"os-lsl"});
            openSim.IncludeFunctionContainingInterface("ILS_Api", new[] {"os-lightshare"});
            openSim.IncludeFunctionContainingInterface("ILSL_Api", new[] {"os-lsl"});
            openSim.IncludeFunctionContainingInterface("IOSSL_Api", new[] {"ossl"});
            openSim.IncludeAttributedModuleClass("ExtendedPhysics", new[] {"os-bullet-physics"});

            foreach (var f in openSim.LSLFunctions().Where(x => !existingData.LibraryFunctionExist(x.Name)))
            {
                Log.WriteLine("OpensimCodeBaseCheck: Function {0} is defined in OpenSim code, but not library data",
                    f.SignatureString);
            }

            foreach (var c in openSim.LSLConstants().Where(x => !existingData.LibraryConstantExist(x.Name)))
            {
                Log.WriteLine("OpensimCodeBaseCheck: Constant {0} is defined in OpenSim code, but not library data",
                    c.SignatureString);
            }

            var subsetFilter = new HashSet<string> {"ossl", "os-lsl", "os-bullet-physics", "os-lightshare"};

            foreach (var f in existingData.LibraryFunctions.SelectMany(x => x)
                .Where(x => x.Subsets.IsSubsetOf(subsetFilter)))
            {
                if (!openSim.LSLFunctionExist(f.Name))
                {
                    Log.WriteLine("OpensimCodeBaseCheck: Function {0} is defined in Library Data, but not opensim code",
                        f.SignatureString);
                }
            }


            foreach (var c in existingData.LibraryConstants.Where(x => x.Subsets.IsSubsetOf(subsetFilter)))
            {
                if (!openSim.LSLConstantExist(c.Name))
                {
                    Log.WriteLine("OpensimCodeBaseCheck: Constant {0} is defined in Library Data, but not opensim code",
                        c.SignatureString);
                }
            }
        }


        [STAThread]
        private static void Main(string[] args)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Select OpenSim bin directory"
            };

            var result = folderBrowserDialog.ShowDialog();


            if (result != DialogResult.OK)
            {
                return;
            }


            Log.LogWriters.Add(Console.Out);


            ScriptLibrary firestormLSL;
            ScriptLibrary firestormOssl;

            using (
                Stream firestormLSLDocs =
                    typeof (Program).Assembly.GetManifestResourceStream(
                        "LibraryDataScrapingTools.FirestormDropIn.scriptlibrary_lsl.xml"))
            using (
                Stream firestormOsslDocs =
                    typeof (Program).Assembly.GetManifestResourceStream(
                        "LibraryDataScrapingTools.FirestormDropIn.scriptlibrary_ossl.xml"))
            {
                if (firestormLSLDocs == null)
                {
                    Log.WriteLine("Could not read firestorm LSL keywords file");
                    return;
                }

                if (firestormOsslDocs == null)
                {
                    Log.WriteLine("Could not read firestorm OSSL keywords file");
                    return;
                }


                firestormLSL = ScriptLibrary.Read(new XmlTextReader(firestormLSLDocs));
                firestormOssl = ScriptLibrary.Read(new XmlTextReader(firestormOsslDocs));
            }


            var compoundDocumentor = new CompoundDocumentationScraper();
            compoundDocumentor.AddProvider(new FirestormDocumentationScraper(firestormLSL));
            compoundDocumentor.AddProvider(new FirestormDocumentationScraper(firestormOssl));


            var openSimScriptFramework = new OpenSimLibraryReflectedTypeData(folderBrowserDialog.SelectedPath);


            using (FileStream logStream = File.Create("log.txt"))
            using (FileStream libraryDataStream = File.Create("LibraryData.xml.txt"))

            {
                var logWriter = new StreamWriter(logStream) {AutoFlush = true};

                Log.LogWriters.Add(logWriter);

                Log.WriteLine("");
                Log.WriteLine("");
                Log.WriteLine("Scraping library data... ");
                Log.WriteLine("");
                Log.WriteLine("");

                var data = ScrapeData(compoundDocumentor, openSimScriptFramework);

                var serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(libraryDataStream, data);


                Log.WriteLine("");
                Log.WriteLine("");
                Log.WriteLine("Checking for missing OpenSim defines... ");
                Log.WriteLine("");
                Log.WriteLine("");


                CheckMissingOpensimKeywords(openSimScriptFramework, data);

                Log.WriteLine("");
                Log.WriteLine("");
                Log.WriteLine("Doing Diff...");
                Log.WriteLine("");
                Log.WriteLine("");

                var diff = new LibraryDataDiff(data, new LSLDefaultLibraryDataProvider(
                    LSLLibraryBaseData.All));

                diff.Diff();


                using (var notInLeft = File.Create("notInLeft.xml.txt"))
                {
                    serializer.Serialize(notInLeft, diff.NotInLeft);
                    notInLeft.Flush();
                }

                using (var notInRight = File.Create("notInRight.xml.txt"))
                {
                    serializer.Serialize(notInRight, diff.NotInRight);
                    notInRight.Flush();
                }

                logWriter.Flush();

            }
            Process.Start("log.txt");
            Process.Start("LibraryData.xml.txt");
            Process.Start("notInLeft.xml.txt");
            Process.Start("notInRight.xml.txt");
        }
    }
}