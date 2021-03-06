﻿#region FileInfo

// 
// File: LSLCodeSegment.cs
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
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    /// Code segments are used to describe a sequential set of code statement nodes that exist in the same scope.
    /// </summary>
    public class LSLCodeSegment : ILSLCodeSegment
    {
        private readonly GenericArray<ILSLReadOnlyCodeStatement> _statementNodes;


        /// <summary>
        ///     Construct an empty code segment.
        /// </summary>
        public LSLCodeSegment()
        {
            _statementNodes = new GenericArray<ILSLReadOnlyCodeStatement>();
            StartNode = null;
            EndNode = null;
            SourceRange = new LSLSourceCodeRange();
        }


        /// <summary>
        ///     The source code range that encompasses all <see cref="ILSLReadOnlyCodeStatement" /> objects in the <see cref="LSLCodeSegment"/>.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> for any added statements, this property will be <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRange { get; private set; }


        /// <summary>
        /// <c>true</c> if <see cref="SourceRange"/> could be calculated for this segment and is non <c>null</c>.
        /// </summary>
        public bool SourceRangeAvailable { get; private set; }


        /// <summary>
        ///     The <see cref="ILSLReadOnlyCodeStatement" /> at the start of the code segment.
        /// </summary>
        public ILSLReadOnlyCodeStatement StartNode { get; private set; }

        /// <summary>
        ///     The <see cref="ILSLReadOnlyCodeStatement" /> at the end of the code segment.
        /// </summary>
        public ILSLReadOnlyCodeStatement EndNode { get; private set; }

        /// <summary>
        ///     All <see cref="ILSLReadOnlyCodeStatement" /> in the code segment, in order of definition.
        /// </summary>
        public IReadOnlyGenericArray<ILSLReadOnlyCodeStatement> StatementNodes
        {
            get { return _statementNodes; }
        }


        /// <summary>
        ///     Adds an <see cref="ILSLReadOnlyCodeStatement" /> to the <see cref="LSLCodeSegment" /> object.
        /// </summary>
        /// <param name="statement">The <see cref="ILSLReadOnlyCodeStatement" /> to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="statement"/> is <c>null</c>.</exception>
        public virtual void AddStatement(ILSLReadOnlyCodeStatement statement)
        {
            if (statement == null) throw new ArgumentNullException("statement");

            EndNode = statement;
            if (StartNode == null)
            {
                StartNode = statement;

                if (statement.SourceRangesAvailable)
                {
                    SourceRange = new LSLSourceCodeRange(statement);
                    SourceRangeAvailable = true;
                }
                else
                {
                    SourceRangeAvailable = false;
                }


                _statementNodes.Add(statement);
            }
            else
            {
                _statementNodes.Add(statement);

                if (SourceRangeAvailable)
                {
                    SourceRange = new LSLSourceCodeRange(SourceRange, EndNode.SourceRange);
                }
            }
        }
    }
}