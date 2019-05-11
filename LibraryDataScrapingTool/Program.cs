#region FileInfo
// 
// File: Program.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using LibLSLCC.CodeValidator;
using LibLSLCC.Collections;
using LibLSLCC.LibraryData;
using LibraryDataScrapingTool.LibraryDataScrapers;
using LibraryDataScrapingTool.LibraryDataScrapers.FirestormLibraryDataDom;
using LibraryDataScrapingTool.OpenSimLibraryReflection;
using LibraryDataScrapingTool.ScraperInterfaces;
using LibraryDataScrapingTool.ScraperProxys;

#endregion

namespace LibraryDataScrapingTool
{
    internal class Program
    {

        public static readonly List<LSLLibrarySubsetDescription> SubsetDescriptions = new List<LSLLibrarySubsetDescription>
        {
            new LSLLibrarySubsetDescription("lsl", "Linden LSL","The standard library functions supported by Linden Lab's SecondLife servers."),
            new LSLLibrarySubsetDescription("os-lsl", "OpenSim LSL","The subset of standard library functions from LSL supported by OpenSim SecondLife servers."),
            new LSLLibrarySubsetDescription("ossl", "OpenSim OSSL","An extended set of functions provided by OpenSim servers that have OSSL enabled."),
            new LSLLibrarySubsetDescription("os-bullet-physics", "OS Bullet Physics","A set of functions for interacting with bullet physics constraints on OpenSim servers which have extended physics enabled."),
            new LSLLibrarySubsetDescription("os-mod-api", "OS Mod Invoke","A set of functions from OpenSim's JsonStore region module for manipulating JSON objects stored in script memory."),
            new LSLLibrarySubsetDescription("os-json-store","OS Json Store","A set of functions for invoking add-on script methods defined in loaded region modules on OpenSim servers."),
            new LSLLibrarySubsetDescription("os-lightshare","OS Light Share","A set of functions from OpenSim's LightShare region module for manipulating a regions shared WindLight settings."),
            new LSLLibrarySubsetDescription("os-attach-temp","OS Attach Temp","An optional OpenSim module that adds llAttachToAvatarTemp to the OpenSim script library."),
        };

        private static void Run(TextWriter logFile)
        {
            Log.LogWriters.Add(Console.Out);

            if (logFile != null)
            {
                Log.LogWriters.Add(logFile);
            }


            var openSimBinPath = OpenOpenSimBinPath();


            var firestormScriptLibraries = OpenFirestormScriptLibrarys();


            IDocumentationProvider docProvider = null;


            foreach (var lib in firestormScriptLibraries)
            {
                var p = new FirestormDocumentationScraper(lib);
                if (docProvider == null)
                {
                    docProvider = p;
                }
                else
                {
                    docProvider = new CompoundDocumentationScraper(docProvider, p);
                }
            }


            var llsdKeywordsFile = OpenKeywordsFile();


            if (docProvider == null)
            {
                docProvider = new LLSDDocumentationScraper(llsdKeywordsFile);
            }
            else
            {
                docProvider = new CompoundDocumentationScraper(docProvider, new LLSDDocumentationScraper(llsdKeywordsFile));
            }


           var llsdData = new LLSDLibraryData(llsdKeywordsFile, new[] { "lsl" });




            var openSimLibraryReflectedTypeData = new OpenSimLibraryReflectedTypeData(openSimBinPath);

            var openSimData = new OpenSimDirectLibraryData(openSimLibraryReflectedTypeData);


            openSimData.IncludeScriptConstantContainerClass(openSimLibraryReflectedTypeData.ScriptBaseClass, new[] { "os-lsl" });
            openSimData.IncludeFunctionContainingInterface("ILS_Api", new[] { "os-lightshare" });
            openSimData.IncludeFunctionContainingInterface("ILSL_Api", new[] { "os-lsl" });
            openSimData.IncludeFunctionContainingInterface("IOSSL_Api", new[] { "ossl" });
            openSimData.IncludeFunctionContainingInterface("IMOD_Api", new[] { "os-mod-api" });
            openSimData.IncludeAttributedModuleClass("ExtendedPhysics", new[] { "os-bullet-physics" });
            openSimData.IncludeAttributedModuleClass("JsonStoreScriptModule", new[] { "os-json-store" });

            //no longer has a script invocation attribute, so its there but wont be detected.
            //stuff like this is why the scraper program is so ugly and I don't spend to much time improving it.
            //openSimData.IncludeAttributedModuleClass("TempAttachmentsModule", new[] { "os-attach-temp" });



            var openSim = new LibraryDataSet(openSimData);

            //meh
            openSim.FunctionSet.Add(new LSLLibraryFunctionSignature(LSLType.Integer, "llAttachToAvatarTemp",
                new[] {new LSLParameterSignature(LSLType.Integer, "attachmentPoint", false)}));


            var activeLibrarySubsets  = SubsetDescriptions.Select(x => x.Subset).ToList();


            var provider = new LSLXmlLibraryDataProvider(activeLibrarySubsets);
            provider.AddSubsetDescriptions(SubsetDescriptions);




            var wikiData = new SecondlifeWikiLibraryData(new[] { "lsl" });


            foreach (var func in wikiData.LSLFunctions())
            {
                Log.WriteLineWithHeader("[NOTICE, LSL WIKI FUNCTION ADDED]:",
                    "The function {0}; was found on the LSL Wiki that was in not in the current set of functions, adding it.",
                    func.SignatureString);


                IReadOnlyGenericArray<LSLLibraryFunctionSignature> overloads;
                if (openSim.OverloadsHashMap.TryGetValue(func.Name, out overloads))
                {
                    if (overloads.Any(x=>x.SignatureEquivalent(func)))
                    {
                        func.Subsets.Add("os-lsl");
                    }
                }

                func.DocumentationString = docProvider.DocumentFunction(func);
                provider.DefineFunction(func);
            }


            foreach (var con in openSim.LSLFunctions())
            {
                var x = provider.GetLibraryFunctionSignatures(con.Name);

                if (x!=null && x.Any(y=>y.SignatureEquivalent(con))) continue;

                con.DocumentationString = docProvider.DocumentFunction(con);


                provider.DefineFunction(con);
            }

            //the wiki is just wrong, the constant values are wrong there to much for comfort.
            foreach (var con in llsdData.LSLConstants())
            {
                if (openSim.LSLConstantExist(con.Name))
                {
                    var consant = openSim.LSLConstant(con.Name);

                    if (consant.Type == con.Type && consant.ValueString == con.ValueString)
                    {
                        con.Subsets.Add("os-lsl");
                    }
                }

                Log.WriteLineWithHeader("[NOTICE, LSL WIKI CONSTANT ADDED]:",
                    "The constant {0}; was found on the LSL Wiki that was not in the current set of constants, adding it.",
                    con.SignatureString);

                con.DocumentationString = docProvider.DocumentConstant(con);
                provider.DefineConstant(con);
            }


            foreach (var con in openSim.LSLConstants())
            {
                var consant = provider.GetLibraryConstantSignature(con.Name);
                if (consant != null)
                {
                    if (consant.Type == con.Type && consant.ValueString == con.ValueString)
                    {
                       continue;
                    }
                }

                con.DocumentationString = docProvider.DocumentConstant(con);
                provider.DefineConstant(con);
            }



            foreach (var ev in wikiData.LSLEvents())
            {
                Log.WriteLineWithHeader("[NOTICE, LSL WIKI EVENT ADDED]:",
                    "The event {0}; was found on the LSL Wiki that was not in the current set of events, adding it.",
                    ev.SignatureString);


                if (openSimLibraryReflectedTypeData.EventNames.Contains(ev.Name))
                {
                    ev.Subsets.AddSubsets("os-lsl");
                }


                ev.DocumentationString = docProvider.DocumentEvent(ev);
                provider.DefineEventHandler(ev);
            }


            foreach (var c in llsdData.LSLFunctions())
            {
                if (provider.LibraryFunctionExist(c.Name)) continue;

                if (openSim.LSLFunctionExist(c.Name))
                {
                    var overloads = openSim.LSLFunctionOverloads(c.Name).Where(x => x.ParameterCount == c.ParameterCount).ToList();

                    if (overloads.Any())
                    {
                        var osFunc = overloads.First();

                        c.Subsets.AddSubsets(osFunc.Subsets);
                        c.ModInvoke = osFunc.ModInvoke;
                    }
                }

                provider.DefineFunction(c);
            }


            foreach (var c in llsdData.LSLEvents())
            {
                if (provider.EventHandlerExist(c.Name)) continue;

                if (openSimLibraryReflectedTypeData.EventNames.Contains(c.Name))
                {
                    c.Subsets.AddSubsets("os-lsl");
                }

                provider.DefineEventHandler(c);
            }




            foreach (var c in openSim.LSLConstants().Where(x => !provider.LibraryConstantExist(x.Name)))
            {
                Log.WriteLineWithHeader("[NOTICE, OPENSIM CONSTANT ADDED]:",
                    "The constant {0}; was found in the OpenSim binaries but was not in the current set of constants, adding it to the current set of data.",
                    c.SignatureString);

                c.DocumentationString = docProvider.DocumentConstant(c);
                provider.DefineConstant(c);
            }



            MessageBox.Show("Select a place to save the generated LibLSLCC library data.",
                "Select Library Data Save Path", MessageBoxButtons.OK, MessageBoxIcon.Information);

            retrySave:



            var libraryDataWritterSettings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.Unicode,
                CloseOutput = true
            };


            var saveGeneratedLibraryDataDialog = new SaveFileDialog
            {
                CreatePrompt = true,
                Filter = "Library Data (*.xml) | *.xml",
                OverwritePrompt = true,
                Title = "Select a place to save the library data",
                FileName = "LibLSLCC_LibraryData.xml"
            };

            string libraryDataOutputFilePath = null;

            if (saveGeneratedLibraryDataDialog.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(saveGeneratedLibraryDataDialog.FileName))
            {
                var dialogResult =
                    MessageBox.Show(
                        "Are you sure you wish to discard the generated library data? you did not select a file to save it in.",
                        "Discard Data?", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);

                if (dialogResult == DialogResult.No)
                {
                    goto retrySave;
                }
            }
            else
            {
                libraryDataOutputFilePath = saveGeneratedLibraryDataDialog.FileName;

                using (var file = XmlWriter.Create(saveGeneratedLibraryDataDialog.OpenFile(), libraryDataWritterSettings))
                {
                    provider.WriteXml(file, true);
                }
            }



            var runDiffDialogResult=  
                MessageBox.Show(
                "Would you like to DIFF the generated data with another set of LibLSLCC Library Data? (The data you just generated will be the 'Left' file).",
                "Run Library Data DIFF", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(runDiffDialogResult == DialogResult.No) return;




            
            MessageBox.Show("Select a LibLSLCC Library Data XML File to be on the Right side of the DIFF.", "Select the 'Right' Library File",
                MessageBoxButtons.OK, MessageBoxIcon.Information);



            retrySelectRightDiffOutput:

            var selectRightDialog = new OpenFileDialog
            {
                Filter = "LibLSLCC Library Data (*.xml) | *.xml",
                CheckFileExists = true,
                Title = "Select a LibLSLCC Library data file"
            };

            

            if (selectRightDialog.ShowDialog() !=  DialogResult.OK || string.IsNullOrWhiteSpace(selectRightDialog.FileName))
            {
                var dialogResult =
                  MessageBox.Show(
                      "You must select a LibLSLCC Library data file to be on the right side of the DIFF, "+
                       "click Retry to select again or Cancel to exit the program.",
                  "File Required", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop);

                if (dialogResult == DialogResult.Retry)
                {
                    goto retrySelectRightDiffOutput;
                }

                return;
            }


            var rightDiffLibraryDataProvider = new LSLXmlLibraryDataProvider(activeLibrarySubsets);
            rightDiffLibraryDataProvider.AddSubsetDescriptions(SubsetDescriptions);


            using (var file = XmlReader.Create(selectRightDialog.OpenFile(), 
                new XmlReaderSettings {CloseInput = true}))
            {
                rightDiffLibraryDataProvider.FillFromXml(file);
            }

            var diff = new LibraryDataDiff(provider, rightDiffLibraryDataProvider, SubsetDescriptions);

            diff.Diff();


            var saveNotRightButInLeft =
                MessageBox.Show(
                    "Would you like to save a library data file representing what was in the generated data, but not in the Right DIFF File?",
                    "Save Added Library Element Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (saveNotRightButInLeft == DialogResult.Yes)
            {
                retryNotInRightSave:

                var saveFileLibraryData = new SaveFileDialog
                {
                    CreatePrompt = true,
                    Filter = "LibLSLCC Library Data (*.xml) | *.xml",
                    OverwritePrompt = true,
                    Title = "Select a place to save the added library elements data.",
                    FileName = "GeneratedDataNotIn_"+ selectRightDialog.SafeFileName
                };

                if (libraryDataOutputFilePath != null)
                {
                    saveFileLibraryData.FileName =
                        "DataIn_"
                        + Path.GetFileNameWithoutExtension(libraryDataOutputFilePath) +
                        "_ButNotIn_"
                        + selectRightDialog.SafeFileName;
                }
                else
                {
                    saveFileLibraryData.FileName =
                        "DataIn_GeneratedData_ButNotIn_"+selectRightDialog.SafeFileName;
                }


                if (saveFileLibraryData.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(saveFileLibraryData.FileName))
                {
                    var dialogResult =
                        MessageBox.Show(
                            "Are you sure you wish to discard the DIFF file representing elements added to the generated library data? " +
                            "you did not select a file to save it in.",
                            "Discard Data?", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);

                    if (dialogResult == DialogResult.No)
                    {
                        goto retryNotInRightSave;
                    }
                }
                else
                {
                    using (var file = XmlWriter.Create(saveFileLibraryData.OpenFile(), libraryDataWritterSettings))
                    {
                        diff.NotInRight.WriteXml(file, true);
                    }
                }
            }



            var saveNotLeftButInRight =
            MessageBox.Show(
                "Would you like to save a library data file representing what was in the Right DIFF file, but not in the generated library data?",
                "Save Removed Library Element Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (saveNotLeftButInRight == DialogResult.Yes)
            {
                retryNotInLeftSave:

                var saveFileLibraryData = new SaveFileDialog
                {
                    CreatePrompt = true,
                    Filter = "LibLSLCC Library Data (*.xml) | *.xml",
                    OverwritePrompt = true,
                    Title = "Select a place to save the removed library elements data."
                    
                };

                if (libraryDataOutputFilePath != null)
                {
                    saveFileLibraryData.FileName =
                        "DataIn_"
                        + Path.GetFileNameWithoutExtension(selectRightDialog.SafeFileName) +
                        "_ButNotIn_"
                        + Path.GetFileNameWithoutExtension(libraryDataOutputFilePath) + ".xml";
                }
                else
                {
                    saveFileLibraryData.FileName =
                        "DataIn_"
                        + Path.GetFileNameWithoutExtension(selectRightDialog.FileName) +
                        "_ButNotIn_GeneratedData" + ".xml";
                }

                if (saveFileLibraryData.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(saveFileLibraryData.FileName))
                {
                    var dialogResult =
                        MessageBox.Show(
                            "Are you sure you wish to discard the DIFF file representing elements removed from the generated library data? " +
                            "you did not select a file to save it in.",
                            "Discard Data?", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);

                    if (dialogResult == DialogResult.No)
                    {
                        goto retryNotInLeftSave;
                    }
                }
                else
                {
                    using (var file = XmlWriter.Create(saveFileLibraryData.OpenFile(), libraryDataWritterSettings))
                    {
                        diff.NotInLeft.WriteXml(file, true);
                    }
                }
            }
        }

        private static string OpenOpenSimBinPath()
        {
            MessageBox.Show(
                "Select the OpenSim the binary 'bin' folder of your OpenSim build so library data can be gathered from it.  (Must be Built First)",
                "Select OpenSim Binary Directory", MessageBoxButtons.OK, MessageBoxIcon.Information);

            retrySelectOpenSimBinDirectory:

            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Select OpenSim bin directory to gather OpenSim library data from."
            };

            var result = folderBrowserDialog.ShowDialog();


            if (result != DialogResult.OK)
            {
                var dialogResult = MessageBox.Show("Selecting an OpenSim bin directory is required, " +
                                                   "click Retry to select again or Cancel to exit the program.",
                    "Directory Required",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);

                if (dialogResult == DialogResult.Retry)
                {
                    goto retrySelectOpenSimBinDirectory;
                }

                Environment.Exit(1);
            }

            return folderBrowserDialog.SelectedPath;
        }

        private static GenericArray<ScriptLibrary> OpenFirestormScriptLibrarys()
        {
            var firestormScriptLibraries = new GenericArray<ScriptLibrary>();

            while (
                MessageBox.Show(
                    "Select additional firestorm scriptlibrary_* data to draw documentation from or click no to continue.",
                    "Select Firestorm Library Data", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Firestorm Highlight File (*.xml) | *.xml",
                    CheckFileExists = true,
                    Multiselect = true
                };

                dialog.ShowDialog();

                var xmlReaderSettings = new XmlReaderSettings {CloseInput = true};
                foreach (var filename in dialog.FileNames)
                {
                    using (var reader = XmlReader.Create(File.OpenRead(filename), xmlReaderSettings))
                    {
                        firestormScriptLibraries.Add(ScriptLibrary.Read(reader));
                    }
                }
            }

            return firestormScriptLibraries;
        }



        private static string OpenKeywordsFile()
        {
            MessageBox.Show(
                "Select the keywords_lsl_default.xml from an up to date viewers program folder under the app_settings folder.",
                "Select LLSD File", MessageBoxButtons.OK, MessageBoxIcon.Information);

            retryOpenLLSD:

            var openLlsdDialog = new OpenFileDialog
            {
                Filter = "LLSD File (*.xml) | *.xml",
                CheckFileExists = true,
                Title = "Select the keywords_lsl_default.xml or other library data LLSD file"
            };


            if (openLlsdDialog.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(openLlsdDialog.FileName))
            {
                var dialogResult = MessageBox.Show(
                    "Selecting a keywords_lsl_default.xml file from your viewer install is required, " +
                    "click Retry to select again or Cancel to exit the program.", "File Required",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);

                if (dialogResult == DialogResult.Retry)
                {
                    goto retryOpenLLSD;
                }

                Environment.Exit(1);
            }

            return openLlsdDialog.FileName;
        }


        private static GenericArray<GenericArray<T>> ChunkBy<T>(GenericArray<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToGenericArray())
                .ToGenericArray();
        }

        private static void GenerateLSLConstantTestScript(ILSLLibraryDataProvider dataProvider, TextWriter writer)
        {

            writer.WriteLine();
            writer.WriteLine("default {");
            
            writer.WriteLine("\tstate_entry(){");


            var index = 0;


            foreach (var constantGroup in ChunkBy(dataProvider.LibraryConstants.Where(x => x.Subsets.Contains("lsl")).ToGenericArray(), 200))
            {
                writer.WriteLine("\t\t#ifdef GROUP_"+index);

                foreach (var constant in constantGroup)
                {
                    writer.WriteLine("\t\tif(" + constant.Name + " != " + constant.ValueStringAsCodeLiteral + ") llOwnerSay(\"" + constant.Name + " value is \"+(string)" + constant.Name + "+\", LibLSLCC value is \"+(string)" + constant.ValueStringAsCodeLiteral + ");");
                }
                writer.WriteLine("\t\tllOwnerSay(\"GROUP_"+index+" Test Complete.\");");
                writer.WriteLine("\t\t#endif");

                index++;
            }


            writer.WriteLine("\t}");
            writer.WriteLine("}");
        }



        [STAThread]
        private static void Main(string[] args)
        {


            if (Directory.Exists(SecondlifeWikiLibraryData.WebCacheFileDirectory))
            {
                var deleteWebCacheDialogResult = MessageBox.Show("Would you to clear the web cache for the LSL Wiki from the previous run?",
                    "Clear Web Cache", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (deleteWebCacheDialogResult == DialogResult.Yes)
                {
                    Directory.Delete(SecondlifeWikiLibraryData.WebCacheFileDirectory, true);
                }
            }





            var saveLogFileDialogResult = MessageBox.Show("Would you like to save a log file?","Save Log",MessageBoxButtons.YesNo,MessageBoxIcon.Question);


            if (saveLogFileDialogResult == DialogResult.Yes)
            {
                var saveFile = new SaveFileDialog
                {
                    CreatePrompt = true,
                    Filter = "Log File (*.txt) | *.txt",
                    OverwritePrompt = true,
                    Title = "Select a place to save the log file",
                    FileName = "library_scraper_log.txt"
                };


                if (saveFile.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(saveFile.FileName))
                {
                    Run(null);
                }
                else
                {
                    using (var log = new StreamWriter(saveFile.OpenFile(), Encoding.Unicode))
                    {
                        log.AutoFlush = true;
                        Run(log);
                        log.Flush();
                    }
                }

            }
            else
            {
                Run(null);
            }

        }
    }
}