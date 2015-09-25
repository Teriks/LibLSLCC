#region FileInfo
// 
// 
// File: LSLSemiColonStatement.cs
// 
// Last Compile: 25/09/2015 @ 11:47 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLSemiColonStatement : ILSLSemiColonStatement, ILSLCodeStatement
    {
        internal LSLSemiColonStatement(LSLParser.CodeStatementContext context, bool isSingleBlockStatement)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParserContext = context;
            IsSingleBlockStatement = isSingleBlockStatement;
        }

        internal LSLParser.CodeStatementContext ParserContext { get; }
        public bool IsSingleBlockStatement { get; private set; }
        public ILSLCodeStatement ReturnPath { get; set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public ulong ScopeId { get; set; }
        public bool IsDeadCode { get; set; }
        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange
        {
            get { return new LSLSourceCodeRange(ParserContext); }
        }

        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitSemiColonStatement(this);
        }

        public bool HasReturnPath
        {
            get { return false; }
        }

        public int StatementIndex { get; set; }
        public bool IsLastStatementInScope { get; set; }
    }
}