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
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.Collections;
using LibraryDataScrapingTools.LibraryDataScrapers;
using LibraryDataScrapingTools.LibraryDataScrapers.FirestormLibraryDataDom;
using LibraryDataScrapingTools.OpenSimLibraryReflection;
using LibraryDataScrapingTools.ScraperInterfaces;
using LibraryDataScrapingTools.ScraperProxys;

#endregion

namespace LibraryDataScrapingTools
{
    internal class Program
    {

        public static GenericArray<LSLLibrarySubsetDescription> SubsetDescriptions = new GenericArray<LSLLibrarySubsetDescription>
        {
            new LSLLibrarySubsetDescription("lsl", "Linden LSL","The standard library functions supported by Linden Lab's SecondLife servers."),
            new LSLLibrarySubsetDescription("os-lsl", "OpenSim LSL","The subset of standard library functions from LSL supported by OpenSim SecondLife servers."),
            new LSLLibrarySubsetDescription("ossl", "OpenSim OSSL","An extended set of functions provided by OpenSim servers that have OSSL enabled."),
            new LSLLibrarySubsetDescription("os-bullet-physics", "OS Bullet Physics","A set of functions for interacting with bullet physics constraints on OpenSim servers which have extended physics enabled."),
            new LSLLibrarySubsetDescription("os-mod-api", "OS Mod Invoke","A set of functions from OpenSim's JsonStore region module for manipulating JSON objects stored in script memory."),
            new LSLLibrarySubsetDescription("os-json-store","OS Json Store","A set of functions for invoking add-on script methods defined in loaded region modules on OpenSim servers."),
            new LSLLibrarySubsetDescription("os-lightshare","OS Light Share","A set of functions from OpenSim's LightShare region module for manipulating a regions shared WindLight settings.")
        };

        private static void Run(TextWriter logFile)
        {
            Log.LogWriters.Add(Console.Out);

            if (logFile != null)
            {
                Log.LogWriters.Add(logFile);
            }


            MessageBox.Show("Select the OpenSim the binary 'bin' folder of your OpenSim build so library data can be gathered from it.  (Must be Built First)",
                "Select OpenSim Binary Directory", MessageBoxButtons.OK, MessageBoxIcon.Information);

            retrySelectOpenSimBinDirectory:

            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Select OpenSim bin directory to gather OpenSim library data from."
            };

            var result = folderBrowserDialog.ShowDialog();


            if (result != DialogResult.OK)
            {
                var dialogResult = MessageBox.Show("Selecting an OpenSim bin directory is required, "+
                    "click Retry to select again or Cancel to exit the program.", "Directory Required",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);

                if (dialogResult == DialogResult.Retry)
                {
                    goto retrySelectOpenSimBinDirectory;
                }
                return;
            }


            GenericArray<ScriptLibrary> firestormScriptLibraries = new GenericArray<ScriptLibrary>();


            while (
                MessageBox.Show("Select additional firestorm scriptlibrary_* data to draw documentation from or click no to continue.",
                    "Select Firestorm Library Data", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Filter = "Firestorm Highlight File (*.xml) | *.xml",
                    CheckFileExists = true,
                    Multiselect = true
                };

                dialog.ShowDialog();

                foreach (var filename in dialog.FileNames)
                {
                    using (var reader = new XmlTextReader(File.OpenRead(filename)))
                    {
                        firestormScriptLibraries.Add(ScriptLibrary.Read(reader));
                    }
                }
            }


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




            MessageBox.Show("Select the keywords_lsl_default.xml from an up to date viewers program folder under the app_settings folder.",
                "Select LLSD File", MessageBoxButtons.OK, MessageBoxIcon.Information);

            retryOpenLLSD:

            OpenFileDialog openLLSDDialog = new OpenFileDialog
            {
                Filter = "LLSD File (*.xml) | *.xml",
                CheckFileExists = true,
                Title = "Select the keywords_lsl_default.xml or other library data LLSD file"
            };



            if (openLLSDDialog.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(openLLSDDialog.FileName))
            {
                var dialogResult = MessageBox.Show(
                    "Selecting a keywords_lsl_default.xml file from your viewer install is required, "+
                    "click Retry to select again or Cancel to exit the program.", "File Required", 
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);

                if (dialogResult == DialogResult.Retry)
                {
                    goto retryOpenLLSD;
                }

                return;
            }



            if (docProvider == null)
            {
                docProvider = new LLSDDocumentationScraper(openLLSDDialog.FileName);
            }
            else
            {
                docProvider = new CompoundDocumentationScraper(docProvider, new LLSDDocumentationScraper(openLLSDDialog.FileName));
            }


            var llsdData = new LLSDLibraryData(openLLSDDialog.FileName, new[] { "lsl" });




            var openSimLibraryReflectedTypeData = new OpenSimLibraryReflectedTypeData(folderBrowserDialog.SelectedPath);

            var openSimData = new OpenSimDirectLibraryData(openSimLibraryReflectedTypeData);


            openSimData.IncludeScriptConstantContainerClass(openSimLibraryReflectedTypeData.ScriptBaseClass, new[] { "os-lsl" });
            openSimData.IncludeFunctionContainingInterface("ILS_Api", new[] { "os-lightshare" });
            openSimData.IncludeFunctionContainingInterface("ILSL_Api", new[] { "os-lsl" });
            openSimData.IncludeFunctionContainingInterface("IOSSL_Api", new[] { "ossl" });
            openSimData.IncludeFunctionContainingInterface("IMOD_Api", new[] { "os-mod-api" });
            openSimData.IncludeAttributedModuleClass("ExtendedPhysics", new[] { "os-bullet-physics" });
            openSimData.IncludeAttributedModuleClass("JsonStoreScriptModule", new[] { "os-json-store" });



            var openSim = new LibraryDataSet(openSimData);


            GenericArray<string> activeLibrarySubsets  = SubsetDescriptions.Select(x => x.Subset).ToList();


            var provider = new LSLXmlLibraryDataProvider(activeLibrarySubsets);
            provider.AddSubsetDescriptions(SubsetDescriptions);

            foreach (var c in llsdData.LSLConstants())
            {
                if (provider.LibraryConstantExist(c.Name)) continue;

                if (openSim.LSLConstantExist(c.Name))
                {
                    var constant = openSim.LSLConstant(c.Name);
                    c.AddSubsets(constant.Subsets);
                }

                provider.DefineConstant(c);
            }

            foreach (var c in llsdData.LSLFunctions())
            {
                if (provider.GetLibraryFunctionSignature(c) != null) continue;

                if (openSim.LSLFunctionExist(c.Name))
                {
                    var overloads = openSim.LSLFunctionOverloads(c.Name);
                    c.AddSubsets(overloads.First().Subsets);
                }

                provider.DefineFunction(c);
            }

            foreach (var c in llsdData.LSLEvents())
            {
                if (provider.EventHandlerExist(c.Name)) continue;

                if (openSimLibraryReflectedTypeData.EventNames.Contains(c.Name))
                {
                    c.AddSubsets("os-lsl");
                }

                provider.DefineEventHandler(c);
            }


            SecondlifeWikiLibraryData wikiData = new SecondlifeWikiLibraryData(new[] { "lsl" });


            foreach (var func in wikiData.LSLFunctions())
            {

                var signatures = provider.GetLibraryFunctionSignatures(func.Name);
                //found existing overloads of this function
                if (signatures != null)
                {
                    //found an exact match
                    var sigMatch = signatures.FirstOrDefault(x => x.SignatureEquivalent(func));

                    if (sigMatch != null)
                    {
                        if (func.Deprecated && !sigMatch.Deprecated)
                        {
                            Log.WriteLineWithHeader("[NOTICE, LSL WIKI FUNCTION DEPRECATED]:",
                                "The function {0} is stated to be deprecated on the LSL Wiki and not set to deprecated in the current set of functions, making it deprecated.",
                                func.SignatureString);

                            sigMatch.Deprecated = true;
                        }
                        else
                        {
                            Log.WriteLineWithHeader("[NOTICE, LSL WIKI FUNCTION ALREADY DEFINED]:",
                                "The function {0} is scraped from the wiki was already accurately defined in the current set of functions, discarding it.",
                                func.SignatureString);
                        }

                        continue;
                    }

                    var duplicate = signatures.FirstOrDefault(x => x.DefinitionIsDuplicate(func));
                    //found a signature that would cause a duplicate definition error
                    if (duplicate != null)
                    {
                        Log.WriteLineWithHeader("[WARNING, LSL WIKI FUNCTION DUPLICATE DEFINITION]:",
                            "The function {0}; was found on the LSL Wiki is considered a duplicate definition to {1}; which already exists in the current set of functions, discarding it." +
                            " Fix this manually if this is not right.",
                            func.SignatureString, duplicate.SignatureString);

                        continue;
                    }

                    Log.WriteLineWithHeader("[WARNING, LSL WIKI FUNCTION OVERLOAD ADDED]:",
                        "The function {0}; was found on the LSL Wiki and has a different signature than the one in the current set of functions, CREATING OVERLOAD" +
                        " Fix manually if this is not right.",
                        func.SignatureString);

                }
                else
                {
                    Log.WriteLineWithHeader("[NOTICE, LSL WIKI FUNCTION ADDED]:",
                        "The function {0}; was found on the LSL Wiki that was in not in the current set of functions, adding it.",
                        func.SignatureString);
                }


                if (openSim.LSLFunctionExist(func.Name))
                {
                    func.AddSubsets("os-lsl");
                }

                provider.DefineFunction(func);
            }


            foreach (var con in wikiData.LSLConstants())
            {
                var signature = provider.GetLibraryConstantSignature(con.Name);
                if (signature != null)
                {

                    if (con.Type != signature.Type)
                    {
                        Log.WriteLineWithHeader("[WARNING, LSL WIKI CONSTANT MISMATCHED TYPE]:",
                            "The constant {0}; was found on the LSL Wiki but has a different TYPE than the version in the current set of constants, the current signature {1}" +
                            Environment.NewLine + "will be kept and the Wiki version discarded, you should check manually that the signature is correct.",
                            con.SignatureString, signature.SignatureString);
                    }


                    if (con.ValueString != signature.ValueString)
                    {
                        Log.WriteLineWithHeader("[WARNING, LSL WIKI CONSTANT MISMATCHED VALUE]:",
                            "The constant {0}; was found on the LSL Wiki but has a different VALUE than the version in the current set of constants, the current signature {1}" +
                            Environment.NewLine + "will be kept and the Wiki version discarded, you should check manually that the signature is correct.",
                            con.SignatureString, signature.SignatureString);
                    }

                    if (con.Deprecated && !signature.Deprecated)
                    {
                        Log.WriteLineWithHeader("[NOTICE, LSL WIKI CONSTANT DEPRECATED]:",
                            "The constant {0}; is stated to be deprecated on the LSL Wiki and not set to deprecated in the current set of constants, making it deprecated.", 
                            con.SignatureString);

                        signature.Deprecated = true;
                    }

                    continue;
                }

                Log.WriteLineWithHeader("[NOTICE, LSL WIKI CONSTANT ADDED]:",
                    "The constant {0}; was found on the LSL Wiki that was not in the current set of constants, adding it.",
                    con.SignatureString);

                if (openSim.LSLConstantExist(con.Name))
                {
                    con.AddSubsets("os-lsl");
                }

                provider.DefineConstant(con);
            }


            foreach (var ev in wikiData.LSLEvents())
            {
                var signature = provider.GetEventHandlerSignature(ev.Name);
                if (signature != null)
                {

                    if (!ev.SignatureMatches(signature))
                    {
                        Log.WriteLineWithHeader("[WARNING, LSL WIKI EVENT SIGNATURE MISMATCH]:",
                            "The event {0}; was found on the LSL Wiki but has a different call signature than the version in the current set of events, the current signature" 
                            + Environment.NewLine+
                            "will be kept and the Wiki version {1} discarded, you should check manually that the signature is correct.",
                            ev.SignatureString, signature.SignatureString);
                    }

                    if (ev.Deprecated && !signature.Deprecated)
                    {
                        Log.WriteLineWithHeader("[NOTICE, LSL WIKI EVENT DEPRECATED]:",
                            "The event {0}; is stated to be deprecated on the LSL Wiki and not set to deprecated in the current set of events, making it deprecated.", 
                            ev.SignatureString);

                        signature.Deprecated = true;
                    }

                    continue;
                }

                

                Log.WriteLineWithHeader("[NOTICE, LSL WIKI EVENT ADDED]:",
                    "The event {0}; was found on the LSL Wiki that was not in the current set of events, adding it.",
                    ev.SignatureString);


                if (openSimLibraryReflectedTypeData.EventNames.Contains(ev.Name))
                {
                    ev.AddSubsets("os-lsl");
                }

                provider.DefineEventHandler(ev);
            }




            foreach (var c in openSim.LSLConstants().Where(x => !provider.LibraryConstantExist(x.Name)))
            {
                Log.WriteLineWithHeader("[NOTICE, OPENSIM CONSTANT ADDED]:",
                    "The constant {0}; was found in the OpenSim binaries but was not in the current set of constants, adding it to the current set of data.",
                    c.SignatureString);

                c.DocumentationString = docProvider.DocumentConstant(c);
                provider.DefineConstant(c);
            }


            //this will work as long as open sim does not start creating function overloads
            //for standard linden functions..
            foreach (var c in openSim.LSLFunctions().Where(x => !provider.LibraryFunctionExist(x.Name)).ToList())
            {
                if(provider.LibraryFunctionExist(c.Name))
                {
                    Log.WriteLineWithHeader("[NOTICE, OPENSIM FUNCTION OVERLOADED]:",
                    "The function {0}; was found in the OpenSim binaries but already existed in the current set of functions with a different signature, creating overload.",
                    c.SignatureString);
                }
                else
                {
                    Log.WriteLineWithHeader("[NOTICE, OPENSIM FUNCTION ADDED]:",
                        "The function {0}; was found in the OpenSim binaries but was not in the current set of functions, adding it to the current set of data.",
                        c.SignatureString);
                }

                c.DocumentationString = docProvider.DocumentFunction(c);
                provider.DefineFunction(c);
            }



            MessageBox.Show("Select a place to save the generated LibLSLCC library data.",
                "Select Library Data Save Path", MessageBoxButtons.OK, MessageBoxIcon.Information);

            retrySave:

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

                using (
                    var writer = new XmlTextWriter(saveGeneratedLibraryDataDialog.OpenFile(), Encoding.Unicode)
                    {
                        Formatting = Formatting.Indented
                    })
                {

                    provider.WriteXml(writer, true);
                }
            }




            var generateConstantTestScript =
                MessageBox.Show(
                "Would you like to generate an LSL script to test the values of the generated library constants in-world?",
                "Generate Constant Test Script", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (generateConstantTestScript != DialogResult.No)
            {


                var saveFile = new SaveFileDialog
                {
                    CreatePrompt = true,
                    Filter = "LSL Script (*.lsl) | *.lsl",
                    OverwritePrompt = true,
                    Title = "Select a place to save the script",
                    FileName = "LibLSLCC_Constants_Test.lsl"
                };


                if (saveFile.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(saveFile.FileName))
                {

                    using (var writer = new StreamWriter(saveFile.OpenFile()))
                    {
                        writer.AutoFlush = true;

                        GenerateLSLConstantTestScript(provider,writer);

                        writer.Flush();
                    }
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

            OpenFileDialog selectRightDialog = new OpenFileDialog
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

            using (var file = new XmlTextReader(selectRightDialog.OpenFile()))
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
                    using (var file = new XmlTextWriter(saveFileLibraryData.OpenFile(), Encoding.Unicode))
                    {
                        file.Formatting = Formatting.Indented;
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
                    Title = "Select a place to save the removed library elements data.",
                    
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
                    using (var file = new XmlTextWriter(saveFileLibraryData.OpenFile(), Encoding.Unicode))
                    {
                        file.Formatting = Formatting.Indented;
                        diff.NotInLeft.WriteXml(file, true);
                    }
                }
            }
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


            int index = 0;


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
                SaveFileDialog saveFile = new SaveFileDialog
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