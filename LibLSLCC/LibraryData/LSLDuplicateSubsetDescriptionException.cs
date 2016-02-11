#region FileInfo
// 
// File: LSLDuplicateSubsetDescriptionException.cs
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
using System.Runtime.Serialization;

namespace LibLSLCC.LibraryData
{
    /// <summary>
    /// This exception is thrown by <see cref="LSLLibraryDataProvider"/> when a subset description for the same subset is added more than once.
    /// </summary>
    [Serializable]
    public class LSLDuplicateSubsetDescriptionException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LSLDuplicateSubsetDescriptionException()
        {
        }

        /// <summary>
        /// Construct with a message.
        /// </summary>
        /// <param name="message"></param>
        public LSLDuplicateSubsetDescriptionException(string message)
            : base(message)
        {
        }


        /// <summary>
        /// Construct with a message and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public LSLDuplicateSubsetDescriptionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected LSLDuplicateSubsetDescriptionException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}