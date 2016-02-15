#region FileInfo
// 
// File: LSLAutoCompleteGlobalFunction.cs
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

using System.Collections.Generic;
using System.Linq;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    /// Represents a global function declaration that was parsed by the auto complete parser
    /// </summary>
    public sealed class LSLAutoCompleteGlobalFunction
    {
        internal LSLAutoCompleteGlobalFunction(string name, string returnTypeString, LSLSourceCodeRange range, LSLSourceCodeRange typeRange,
            LSLSourceCodeRange nameRange, IList<LSLAutoCompleteLocalParameter> parameters)
        {
            Parameters = parameters.WrapWithGenericArray();
            Name = name;
            ReturnTypeString = returnTypeString;
            SourceCodeRange = range;

            NameSourceCodeRange = nameRange;

            TypeSourceCodeRange = typeRange;

            HasReturnType = true;
        }


        internal LSLAutoCompleteGlobalFunction(string name, LSLSourceCodeRange range, LSLSourceCodeRange nameRange,
            IList<LSLAutoCompleteLocalParameter> parameters)
        {
            Parameters = parameters.WrapWithGenericArray();
            Name = name;
            SourceCodeRange = range;

            NameSourceCodeRange = nameRange;

            TypeSourceCodeRange = null;

            HasReturnType = false;
        }

        /// <summary>
        /// Gets a value indicating whether the function declaration has a return type.
        /// </summary>
        /// <value>
        /// <c>true</c> if the declaration has a return type; otherwise, <c>false</c>.
        /// </value>
        public bool HasReturnType { get; private set; }

        /// <summary>
        /// Gets the <see cref="LSLSourceCodeRange"/> of the function name.
        /// </summary>
        public LSLSourceCodeRange NameSourceCodeRange { get; private set; }

        /// <summary>
        /// Gets the <see cref="LSLSourceCodeRange"/> of the functions return type if it exists, otherwise <c>null</c>.
        /// </summary>
        public LSLSourceCodeRange TypeSourceCodeRange { get; private set; }


        /// <summary>
        /// Gets the name used in the function declaration.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the string representing the return type used in the function declaration.  
        /// If none exists (<see cref="HasReturnType"/> if <c>false</c>), then it will be <c>null</c>.
        /// </summary>
        /// <value>
        /// The return type string if a return type is specified, otherwise <c>null</c>
        /// </value>
        public string ReturnTypeString { get; private set; }

        /// <summary>
        /// The full signature of the global function declaration.
        /// 
        /// Defined as: <c>ReturnTypeString+" "+Name+ParameterSignature</c> if there is a return type specified.
        /// If there is no return type then it's: <c>Name+ParameterSignature</c>
        /// </summary>
        public string FullSignature
        {
            get
            {
                var sig = "";
                if (!string.IsNullOrEmpty(ReturnTypeString))
                {
                    sig += ReturnTypeString + " ";
                }

                

                sig += Name + ParametersSignature + ";";

                return sig;
            }
        }

        /// <summary>
        /// Returns a string representing the parameter signature used in the function declaration, this includes the parentheses.
        /// </summary>
        public string ParametersSignature
        {
            get
            {
                var sig = "()";


                if (Parameters.Count > 0)
                {
                    sig = string.Join(", ", Parameters.Select(x => x.TypeString + " " + x.Name));
                }

                return sig;
            }
        }

        /// <summary>
        /// Gets a read only array of <see cref="LSLAutoCompleteLocalParameter"/> objects representing the parameters used in the function declaration.
        /// </summary>
        /// <value>
        /// The parameters used in the function declaration.
        /// </value>
        public IReadOnlyGenericArray<LSLAutoCompleteLocalParameter> Parameters { get; private set; }

        /// <summary>
        /// Gets the source code range that encompasses the entire function declaration.
        /// </summary>
        /// <value>
        /// The source code range of the entire function declaration.
        /// </value>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }
    }
}