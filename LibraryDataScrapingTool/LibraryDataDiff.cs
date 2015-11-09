#region FileInfo
// 
// File: LibraryDataDiff.cs
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
using System.Linq;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.LibraryData;

#endregion

namespace LibraryDataScrapingTools
{
    public class LibraryDataDiff
    {
        public LibraryDataDiff(LSLLibraryDataProvider left, LSLLibraryDataProvider right, IEnumerable<LSLLibrarySubsetDescription> subsetDescriptions)
        {
            Left = left;
            Right = right;


            var lslLibrarySubsetDescriptions = subsetDescriptions as LSLLibrarySubsetDescription[] ?? subsetDescriptions.ToArray();

            var activeSubsets = lslLibrarySubsetDescriptions.Select(x => x.Subset).ToList();

            NotInLeft = new LSLXmlLibraryDataProvider(activeSubsets);
            NotInRight = new LSLXmlLibraryDataProvider(activeSubsets);

            NotInLeft.AddSubsetDescriptions(lslLibrarySubsetDescriptions);
            NotInRight.AddSubsetDescriptions(lslLibrarySubsetDescriptions);
        }

        public ILSLLibraryDataProvider Left { get; set; }
        public ILSLLibraryDataProvider Right { get; set; }
        public LSLXmlLibraryDataProvider NotInLeft { get; private set; }
        public LSLXmlLibraryDataProvider NotInRight { get; private set; }

        private void DiffFunctions()
        {
            if (Left.LibraryFunctions.Count() != Right.LibraryFunctions.Count())
            {
                Log.WriteLineWithHeader("[Diff]: ", "Left number of functions is not equal to right number of functions");
            }


            var leftFuncs = new HashSet<LSLLibraryFunctionSignature>(Left.LibraryFunctions.SelectMany(x => x));
            var rightFuncs = new HashSet<LSLLibraryFunctionSignature>(Right.LibraryFunctions.SelectMany(x => x));

            if (!leftFuncs.SetEquals(rightFuncs))
            {
                foreach (var sig in leftFuncs.Except(rightFuncs))
                {
                    if (!Right.LibraryFunctionExist(sig.Name))
                    {
                        Log.WriteLineWithHeader("[Diff]: ", "Left function {0} does not exist in Right", sig.Name);
                        NotInRight.DefineFunction(sig);
                    }
                    else
                    {
                        var overloads1 =
                            new HashSet<LSLLibraryFunctionSignature>(Left.GetLibraryFunctionSignatures(sig.Name));
                        var overloads2 =
                            new HashSet<LSLLibraryFunctionSignature>(Right.GetLibraryFunctionSignatures(sig.Name));
                        if (overloads1.SetEquals(overloads2))
                        {
                            Log.WriteLineWithHeader("[Diff]: ", "Left function {0} is different in Right", sig.Name);
                        }
                    }
                }

                foreach (var sig in rightFuncs.Except(leftFuncs))
                {
                    if (!Left.LibraryFunctionExist(sig.Name))
                    {
                        Log.WriteLineWithHeader("[Diff]: ", "Right function {0} does not exist in Left", sig.Name);
                        NotInLeft.DefineFunction(sig);
                    }
                    else
                    {
                        var overloads1 =
                            new HashSet<LSLLibraryFunctionSignature>(Left.GetLibraryFunctionSignatures(sig.Name));
                        var overloads2 =
                            new HashSet<LSLLibraryFunctionSignature>(Right.GetLibraryFunctionSignatures(sig.Name));
                        if (overloads1.SetEquals(overloads2))
                        {
                            Log.WriteLineWithHeader("[Diff]: ", "Right function {0} is different in Left", sig.Name);
                        }
                    }
                }
            }
        }

        private void DiffConstants()
        {
            if (Left.LibraryConstants.Count() != Right.LibraryConstants.Count())
            {
                Log.WriteLineWithHeader("[Diff]: ", "Left number of constants is not equal to right number of constants");
            }


            var leftConstants = new HashSet<LSLLibraryConstantSignature>(Left.LibraryConstants);
            var rightConstants = new HashSet<LSLLibraryConstantSignature>(Right.LibraryConstants);

            if (!leftConstants.SetEquals(rightConstants))
            {
                foreach (var sig in leftConstants.Except(rightConstants))
                {
                    if (!Right.LibraryConstantExist(sig.Name))
                    {
                        Log.WriteLineWithHeader("[Diff]: ", "Left constant {0} does not exist in Right", sig.Name);
                        NotInRight.DefineConstant(sig);
                    }
                    else
                    {
                        Log.WriteLineWithHeader("[Diff]: ", "Left constant {0} is different in Right", sig.Name);
                    }
                }

                foreach (var sig in rightConstants.Except(leftConstants))
                {
                    if (!Left.LibraryConstantExist(sig.Name))
                    {
                        Log.WriteLineWithHeader("[Diff]: ", "Right constant {0} does not exist in Left", sig.Name);
                        NotInLeft.DefineConstant(sig);
                    }
                    else
                    {
                        Log.WriteLineWithHeader("[Diff]: ", "Right constant {0} is different in Left", sig.Name);
                    }
                }
            }
        }

        private void DiffEvents()
        {
            if (Left.SupportedEventHandlers.Count() != Right.SupportedEventHandlers.Count())
            {
                Log.WriteLineWithHeader("[Diff]: ", "Left number of events is not equal to right number of events");
            }


            var leftEvents = new HashSet<LSLLibraryEventSignature>(Left.SupportedEventHandlers);
            var rightEvents = new HashSet<LSLLibraryEventSignature>(Right.SupportedEventHandlers);

            if (!leftEvents.SetEquals(rightEvents))
            {
                foreach (var sig in leftEvents.Except(rightEvents))
                {
                    if (!Right.EventHandlerExist(sig.Name))
                    {
                        Log.WriteLineWithHeader("[Diff]: ", "Left event {0} does not exist in Right", sig.Name);
                        NotInRight.DefineEventHandler(sig);
                    }
                    else
                    {
                        Log.WriteLineWithHeader("[Diff]: ", "Left event {0} is different in Right", sig.Name);
                    }
                }

                foreach (var sig in rightEvents.Except(leftEvents))
                {
                    if (!Left.EventHandlerExist(sig.Name))
                    {
                        Log.WriteLineWithHeader("[Diff]: ", "Right event {0} does not exist in Left", sig.Name);
                        NotInLeft.DefineEventHandler(sig);
                    }
                    else
                    {
                        Log.WriteLineWithHeader("[Diff]: ", "Right event {0} is different in Left", sig.Name);
                    }
                }
            }
        }

        public void Diff()
        {

            Log.WriteLine("============================");
            Log.WriteLine("Starting Library Data DIFF");
            Log.WriteLine("============================");

            DiffConstants();
            DiffFunctions();
            DiffEvents();

            Log.WriteLine("============================");
            Log.WriteLine("Library Data DIFF Finished");
            Log.WriteLine("============================");
        }
    }
}