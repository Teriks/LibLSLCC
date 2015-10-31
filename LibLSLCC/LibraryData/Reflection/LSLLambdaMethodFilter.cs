#region FileInfo
// 
// File: LSLLambdaMethodFilter.cs
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
using System;
using System.Reflection;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Implements <see cref="ILSLMethodFilter"/> using function objects.
    /// </summary>
    public class LSLLambdaMethodFilter : ILSLMethodFilter
    {

        /// <summary>
        /// The function used to implement <see cref="PreFilter"/>.  If <c>null</c> no filtering will occur (everything will be allowed).
        /// </summary>
        public Func<LSLLibraryDataReflectionSerializer, MethodInfo, bool> PreFilterFunction { get; set; }


        /// <summary>
        /// The function used to implement <see cref="MutateSignature"/>.  If <c>null</c> no filtering will occur (everything will be allowed) and no de-serialized method signatures will be mutated.
        /// </summary>
        public Func<LSLLibraryDataReflectionSerializer, MethodInfo, LSLLibraryFunctionSignature, bool> MutateSignatureFunction { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLambdaMethodFilter"/> class.
        /// </summary>
        public LSLLambdaMethodFilter()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLambdaMethodFilter"/> class.
        /// </summary>
        /// <param name="preFilterFunction">The pre filter function.</param>
        public LSLLambdaMethodFilter(Func<LSLLibraryDataReflectionSerializer, MethodInfo, bool> preFilterFunction)
        {
            PreFilterFunction = preFilterFunction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLambdaMethodFilter"/> class.
        /// </summary>
        /// <param name="mutateSignatureFunction">The mutate signature function.</param>
        public LSLLambdaMethodFilter(Func<LSLLibraryDataReflectionSerializer, MethodInfo, LSLLibraryFunctionSignature, bool> mutateSignatureFunction)
        {
            MutateSignatureFunction = mutateSignatureFunction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLambdaMethodFilter"/> class.
        /// </summary>
        /// <param name="preFilterFunction">The pre filter function.</param>
        /// <param name="mutateSignatureFunction">The mutate signature function.</param>
        public LSLLambdaMethodFilter(Func<LSLLibraryDataReflectionSerializer, MethodInfo, bool> preFilterFunction, Func<LSLLibraryDataReflectionSerializer, MethodInfo, LSLLibraryFunctionSignature, bool> mutateSignatureFunction) : this(preFilterFunction)
        {
            MutateSignatureFunction = mutateSignatureFunction;
        }

        /// <summary>
        /// Allows <see cref="MethodInfo"/> objects to be prematurely filtered from de-serialization output.  Returns <c>true</c> if the <see cref="MethodInfo"/> should be filtered.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="MethodInfo"/> object we may want to filter from the output.</param>
        /// <returns><c>true</c> if the method needs to be filtered from the results.</returns>
        public bool PreFilter(LSLLibraryDataReflectionSerializer serializer, MethodInfo info)
        {
            return PreFilterFunction != null && PreFilterFunction(serializer,info);
        }

        /// <summary>
        /// Allows modification a function signature after its basic information has been serialized, before its returned.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="MethodInfo"/> object the library function signature was serialized from.</param>
        /// <param name="signature">The signature.</param>
        /// <returns><c>true</c> if the method needs to be filtered from the results.</returns>
        public bool MutateSignature(LSLLibraryDataReflectionSerializer serializer, MethodInfo info,
            LSLLibraryFunctionSignature signature)
        {
            return MutateSignatureFunction != null && MutateSignatureFunction(serializer, info, signature);
        }
    }
}