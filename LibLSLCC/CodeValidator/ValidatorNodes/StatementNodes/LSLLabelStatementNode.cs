#region FileInfo
// 
// File: LSLLabelStatementNode.cs
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLLabelStatementNode : ILSLLabelStatementNode, ILSLCodeStatement
    {
        private readonly List<LSLJumpStatementNode> _jumpsToHere;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLLabelStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLLabelStatementNode(LSLParser.LabelStatementContext context, bool isSingleBlockStatement)
        {
            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            _jumpsToHere = new List<LSLJumpStatementNode>();
            SourceCodeRange = new LSLSourceCodeRange(context);

            LabelNameSourceCodeRange = new LSLSourceCodeRange(context.label_name);
            LabelPrefixSourceCodeRange = new LSLSourceCodeRange(context.label_prefix);
            SemiColonSourceCodeRange = new LSLSourceCodeRange(context.semi_colon);
        }

        internal LSLParser.LabelStatementContext ParserContext { get; }
        internal LSLParser.CodeScopeContext ParentScopeParserContext { get; set; }
        public LSLCodeScopeNode ParentScope { get; set; }

        public IReadOnlyList<LSLJumpStatementNode> JumpsToHere
        {
            get { return _jumpsToHere.AsReadOnly(); }
        }

        public ILSLCodeStatement ReturnPath { get; set; }

        IReadOnlyList<ILSLJumpStatementNode> ILSLLabelStatementNode.JumpsToHere
        {
            get { return JumpsToHere; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLDeadCodeType DeadCodeType { get; set; }

        public string LabelName
        {
            get { return ParserContext.label_name.Text; }
        }

        public ulong ScopeId { get; set; }
        public LSLSourceCodeRange LabelPrefixSourceCodeRange { get; }
        public LSLSourceCodeRange LabelNameSourceCodeRange { get; }
        public LSLSourceCodeRange SemiColonSourceCodeRange { get; }

        public void AddJumpToHere(LSLJumpStatementNode jump)
        {
            _jumpsToHere.Add(jump);
        }

        public static
            LSLLabelStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLLabelStatementNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLCodeStatement Members

        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public int StatementIndex { get; set; }


        public bool HasReturnPath
        {
            get { return false; }
        }

        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitLabelStatement(this);
        }

        #endregion
    }
}