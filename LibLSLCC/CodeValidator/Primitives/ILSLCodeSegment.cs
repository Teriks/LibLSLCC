#region FileInfo
// 
// File: ILSLCodeSegment.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2016, Teriks
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

using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    /// Read only interface for code segments.  <para/>
    /// Code segments are used to describe a sequential set of code statement nodes that exist in the same scope.
    /// </summary>
    public interface ILSLCodeSegment
    {
        /// <summary>
        ///     The <see cref="ILSLReadOnlyCodeStatement" /> at the end of the code segment.
        /// </summary>
        ILSLReadOnlyCodeStatement EndNode { get; }


        /// <summary>
        ///     The source code range that encompasses all <see cref="ILSLReadOnlyCodeStatement" /> objects in the <see cref="LSLCodeSegment"/>.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> for any added statements, this property will be <c>null</c>.
        /// </remarks>
        LSLSourceCodeRange SourceRange { get; }

        /// <summary>
        /// <c>true</c> if <see cref="SourceRange"/> could be calculated for this segment and is non <c>null</c>.
        /// </summary>
        bool SourceRangeAvailable { get; }


        /// <summary>
        ///     The <see cref="ILSLReadOnlyCodeStatement" /> at the start of the code segment.
        /// </summary>
        ILSLReadOnlyCodeStatement StartNode { get; }

        /// <summary>
        ///     All <see cref="ILSLReadOnlyCodeStatement" /> in the code segment, in order of definition.
        /// </summary>
        IReadOnlyGenericArray<ILSLReadOnlyCodeStatement> StatementNodes { get; }
    }
}