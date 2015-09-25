#region FileInfo
// File: LibraryDataDiff.cs
// 
// Last Compile: 25/09/2015 @ 5:43 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// Copyright (c) 2015, Teriks
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
#endregion
#region Imports

using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;

#endregion

namespace LibraryDataScrapingTools
{
    public class LibraryDataDiff
    {
        public LibraryDataDiff(ILSLMainLibraryDataProvider left, ILSLMainLibraryDataProvider right)
        {
            Left = left;
            Right = right;
            NotInLeft = new LSLXmlLibraryDataProvider();
            NotInRight = new LSLXmlLibraryDataProvider();
        }

        public ILSLMainLibraryDataProvider Left { get; set; }
        public ILSLMainLibraryDataProvider Right { get; set; }
        public LSLXmlLibraryDataProvider NotInLeft { get; }
        public LSLXmlLibraryDataProvider NotInRight { get; }

        private void DiffFunctions()
        {
            if (Left.LibraryFunctions.Count() != Right.LibraryFunctions.Count())
            {
                Log.WriteLine("Diff: Left number of functions is not equal to right number of functions");
            }


            var leftFuncs = new HashSet<LSLLibraryFunctionSignature>(Left.LibraryFunctions.SelectMany(x => x));
            var rightFuncs = new HashSet<LSLLibraryFunctionSignature>(Right.LibraryFunctions.SelectMany(x => x));

            if (!leftFuncs.SetEquals(rightFuncs))
            {
                foreach (var sig in leftFuncs.Except(rightFuncs))
                {
                    if (!Right.LibraryFunctionExist(sig.Name))
                    {
                        Log.WriteLine("Diff: Left function {0} does not exist in Right", sig.Name);
                        NotInRight.AddValidLibraryFunction(sig);
                    }
                    else
                    {
                        var overloads1 =
                            new HashSet<LSLLibraryFunctionSignature>(Left.GetLibraryFunctionSignatures(sig.Name));
                        var overloads2 =
                            new HashSet<LSLLibraryFunctionSignature>(Right.GetLibraryFunctionSignatures(sig.Name));
                        if (overloads1.SetEquals(overloads2))
                        {
                            Log.WriteLine("Diff: Left function {0} is different in Right", sig.Name);
                        }
                    }
                }

                foreach (var sig in rightFuncs.Except(leftFuncs))
                {
                    if (!Left.LibraryFunctionExist(sig.Name))
                    {
                        Log.WriteLine("Diff: Right function {0} does not exist in Left", sig.Name);
                        NotInLeft.AddValidLibraryFunction(sig);
                    }
                    else
                    {
                        var overloads1 =
                            new HashSet<LSLLibraryFunctionSignature>(Left.GetLibraryFunctionSignatures(sig.Name));
                        var overloads2 =
                            new HashSet<LSLLibraryFunctionSignature>(Right.GetLibraryFunctionSignatures(sig.Name));
                        if (overloads1.SetEquals(overloads2))
                        {
                            Log.WriteLine("Diff: Right function {0} is different in Left", sig.Name);
                        }
                    }
                }
            }
        }

        private void DiffConstants()
        {
            if (Left.LibraryConstants.Count() != Right.LibraryConstants.Count())
            {
                Log.WriteLine("Diff: Left number of constants is not equal to right number of constants");
            }


            var leftFuncs = new HashSet<LSLLibraryConstantSignature>(Left.LibraryConstants);
            var rightFuncs = new HashSet<LSLLibraryConstantSignature>(Right.LibraryConstants);

            if (!leftFuncs.SetEquals(rightFuncs))
            {
                foreach (var sig in leftFuncs.Except(rightFuncs))
                {
                    if (!Right.LibraryConstantExist(sig.Name))
                    {
                        Log.WriteLine("Diff: Left constant {0} does not exist in Right", sig.Name);
                        NotInRight.AddValidConstant(sig);
                    }
                    else
                    {
                        Log.WriteLine("Diff: Left constant {0} is different in Right", sig.Name);
                    }
                }

                foreach (var sig in rightFuncs.Except(leftFuncs))
                {
                    if (!Left.LibraryConstantExist(sig.Name))
                    {
                        Log.WriteLine("Diff: Right constant {0} does not exist in Left", sig.Name);
                        NotInLeft.AddValidConstant(sig);
                    }
                    else
                    {
                        Log.WriteLine("Diff: Right constant {0} is different in Left", sig.Name);
                    }
                }
            }
        }

        private void DiffEvents()
        {
            if (Left.SupportedEventHandlers.Count() != Right.SupportedEventHandlers.Count())
            {
                Log.WriteLine("Diff: Left number of events is not equal to right number of events");
            }


            var leftFuncs = new HashSet<LSLLibraryEventSignature>(Left.SupportedEventHandlers);
            var rightFuncs = new HashSet<LSLLibraryEventSignature>(Right.SupportedEventHandlers);

            if (!leftFuncs.SetEquals(rightFuncs))
            {
                foreach (var sig in leftFuncs.Except(rightFuncs))
                {
                    if (!Right.EventHandlerExist(sig.Name))
                    {
                        Log.WriteLine("Diff: Left event {0} does not exist in Right", sig.Name);
                        NotInRight.AddValidEventHandler(sig);
                    }
                    else
                    {
                        Log.WriteLine("Diff: Left event {0} is different in Right", sig.Name);
                    }
                }

                foreach (var sig in rightFuncs.Except(leftFuncs))
                {
                    if (!Left.EventHandlerExist(sig.Name))
                    {
                        Log.WriteLine("Diff: Right event {0} does not exist in Left", sig.Name);
                        NotInLeft.AddValidEventHandler(sig);
                    }
                    else
                    {
                        Log.WriteLine("Diff: Right event {0} is different in Left", sig.Name);
                    }
                }
            }
        }

        public void Diff()
        {
            DiffConstants();
            DiffFunctions();
            DiffEvents();
        }
    }
}