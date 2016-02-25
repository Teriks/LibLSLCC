#region FileInfo

// 
// File: LSLDeadCodeSegment.cs
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

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     A code segment type for housing a range of statement nodes that are considered to be dead.
    ///     a DeadCodeType enum property is provided to describe what caused the code to be dead
    /// </summary>
    public sealed class LSLDeadCodeSegment : LSLCodeSegment
    {
        /// <summary>
        ///     Construct an <see cref="LSLDeadCodeSegment" /> with the given <see cref="LSLDeadCodeType" />
        /// </summary>
        /// <param name="deadCodeType"></param>
        public LSLDeadCodeSegment(LSLDeadCodeType deadCodeType)
        {
            DeadCodeType = deadCodeType;
        }


        /// <summary>
        ///     The type of dead code that this <see cref="LSLDeadCodeSegment" /> represents.
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; private set; }
    }
}