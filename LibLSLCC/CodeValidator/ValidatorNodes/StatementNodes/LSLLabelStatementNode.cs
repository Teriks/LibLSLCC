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
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLLabelStatementNode : ILSLLabelStatementNode, ILSLCodeStatement
    {
        private readonly List<LSLJumpStatementNode> _jumpsToHere = new List<LSLJumpStatementNode>();
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
            SourceCodeRange = new LSLSourceCodeRange(context);

            LabelNameSourceCodeRange = new LSLSourceCodeRange(context.label_name);
            LabelPrefixSourceCodeRange = new LSLSourceCodeRange(context.label_prefix);
            SemiColonSourceCodeRange = new LSLSourceCodeRange(context.semi_colon);

            SourceCodeRangesAvailable = true;
        }

        internal LSLParser.LabelStatementContext ParserContext { get; private set; }
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

        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        public LSLDeadCodeType DeadCodeType { get; set; }


        /// <summary>
        /// The name of the label.
        /// </summary>
        public string LabelName
        {
            get { return ParserContext.label_name.Text; }
        }

        /// <summary>
        ///     Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        ///     this is not the scopes level.
        /// </summary>
        public ulong ScopeId { get; set; }


        /// <summary>
        /// The source code range of the '@' symbol that prefixes the label name.
        /// </summary>
        public LSLSourceCodeRange LabelPrefixSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the label's name.
        /// </summary>
        public LSLSourceCodeRange LabelNameSourceCodeRange { get; private set; }


        /// <summary>
        /// The source code range of the semi-colon that follows the label definition.
        /// </summary>
        public LSLSourceCodeRange SemiColonSourceCodeRange { get; private set; }



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


        /// <summary>
        /// True if this statement belongs to a single statement code scope.
        /// A single statement code scope is a brace-less code scope that can be used in control or loop statements.
        /// </summary>
        public bool IsSingleBlockStatement { get; private set; }

        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }


        /// <summary>
        ///     The index of this statement in its scope
        /// </summary>
        public int StatementIndex { get; set; }


        /// <summary>
        /// True if the node represents a return path out of its ILSLCodeScopeNode parent, False otherwise.
        /// </summary>
        public bool HasReturnPath
        {
            get { return false; }
        }

        /// <summary>
        ///     Is this statement the last statement in its scope
        /// </summary>
        public bool IsLastStatementInScope { get; set; }


        /// <summary>
        ///     Is this statement dead code
        /// </summary>
        public bool IsDeadCode { get; set; }


        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceCodeRangesAvailable { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of ILSLValidatorNodeVisitor
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitLabelStatement(this);
        }

        #endregion
    }
}