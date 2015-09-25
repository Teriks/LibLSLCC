#region FileInfo

// 
// File: LSLStateChangeStatementNode.cs
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

using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLStateChangeStatementNode : ILSLStateChangeStatementNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLStateChangeStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLStateChangeStatementNode(LSLParser.StateChangeStatementContext context, bool isSingleBlockStatement)
        {
            ParserContext = context;
            IsSingleBlockStatement = isSingleBlockStatement;
            SourceCodeRange = new LSLSourceCodeRange(context);
            StateKeywordSourceCodeRange = new LSLSourceCodeRange(context.state_keyword);
            SemiColonSourceCodeRange = new LSLSourceCodeRange(context.semi_colon);
            StateNameSourceCodeRange = new LSLSourceCodeRange(context.state_target);
        }

        internal LSLParser.StateChangeStatementContext ParserContext { get; }
        public ILSLCodeStatement ReturnPath { get; set; }
        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public ulong ScopeId { get; set; }
        public LSLSourceCodeRange StateKeywordSourceCodeRange { get; }
        public LSLSourceCodeRange StateNameSourceCodeRange { get; }
        public LSLSourceCodeRange SemiColonSourceCodeRange { get; }

        public static
            LSLStateChangeStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLStateChangeStatementNode(sourceRange, Err.Err);
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

        public string StateTargetName
        {
            get { return ParserContext.state_target.Text; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLStateChangeStatementNode.Parent
        {
            get { return Parent; }
        }

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
            return visitor.VisitStateChangeStatement(this);
        }

        #endregion
    }
}