#region FileInfo

// 
// File: LSLLabelStatementNode.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:24 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
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