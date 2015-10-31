#region FileInfo
// 
// File: LSLMultiMethodFilter.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Allows multiple <see cref="ILSLMethodFilter"/> objects to participate in filtering/mutating method
    /// signatures de-serialized from runtime types using <see cref="LSLLibraryDataReflectionSerializer"/>.
    /// </summary>
    public class LSLMultiMethodFilter : ILSLMethodFilter, IEnumerable<ILSLMethodFilter>
    {

        /// <summary>
        /// A modifiable collection of all <see cref="ILSLMethodFilter"/> objects participating in filtering.
        /// </summary>
        /// <value>
        /// The <see cref="ILSLMethodFilter"/>'s being used to filter.
        /// </value>
        // ReSharper disable once CollectionNeverUpdated.Global
        public List<ILSLMethodFilter> Filters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLMultiMethodFilter"/> class.
        /// </summary>
        public LSLMultiMethodFilter()
        {
            Filters= new List<ILSLMethodFilter>();
        }

        /// <summary>
        /// Allows <see cref="MethodInfo"/> objects to be prematurely filtered from de-serialization output.  Returns <c>true</c> if the <see cref="MethodInfo"/> should be filtered.
        /// </summary>
        /// <param name="serializer">The <see cref="LSLLibraryDataReflectionSerializer"/> this add-on belongs to.</param>
        /// <param name="info">The <see cref="MethodInfo"/> object we may want to filter from the output.</param>
        /// <returns><c>true</c> if the method needs to be filtered from the results.</returns>
        public bool PreFilter(LSLLibraryDataReflectionSerializer serializer, MethodInfo info)
        {
            return Filters.Any(filter => filter.PreFilter(serializer, info));
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
            return Filters.Any(filter => filter.MutateSignature(serializer, info, signature));
        }


        /// <summary>
        /// Add's a filter to this multi filter, this here so list initializer syntax can be utilized to build a multi filter.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        public void Add(ILSLMethodFilter filter)
        {
            Filters.Add(filter);
        }

        public IEnumerator<ILSLMethodFilter> GetEnumerator()
        {
            return Filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}