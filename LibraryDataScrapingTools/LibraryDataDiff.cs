#region FileInfo

// 
// File: LibraryDataDiff.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:27 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
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