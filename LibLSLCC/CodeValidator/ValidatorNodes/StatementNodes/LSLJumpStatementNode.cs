#region FileInfo

// 
// File: LSLJumpStatementNode.cs
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

using System;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLJumpStatementNode : ILSLJumpStatementNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLJumpStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLJumpStatementNode(LSLParser.JumpStatementContext context, LSLLabelStatementNode jumpTarget,
            bool isSingleBlockStatement)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (jumpTarget == null)
            {
                throw new ArgumentNullException("jumpTarget");
            }

            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            JumpTarget = jumpTarget;
            jumpTarget.AddJumpToHere(this);
            SourceCodeRange = new LSLSourceCodeRange(context);

            LabelNameSourceCodeRange = new LSLSourceCodeRange(context.jump_target);
            JumpKeywordSourceCodeRange = new LSLSourceCodeRange(context.jump_keyword);
            SemiColonSourceCodeRange = new LSLSourceCodeRange(context.semi_colon);
        }

        internal LSLParser.JumpStatementContext ParserContext { get; }
        public LSLLabelStatementNode JumpTarget { get; set; }

        #region ILSLCodeStatement Members

        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public bool ConstantJump { get; set; }

        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public int StatementIndex { get; set; }

        public bool HasReturnPath
        {
            get { return false; }
        }

// ReSharper disable UnusedParameter.Local


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitJumpStatement(this);
        }

        #endregion

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        public ILSLCodeStatement ReturnPath { get; set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLDeadCodeType DeadCodeType { get; set; }


        public string LabelName
        {
            get { return ParserContext.jump_target.Text; }
        }

        ILSLLabelStatementNode ILSLJumpStatementNode.JumpTarget
        {
            get { return JumpTarget; }
        }


        public ulong ScopeId { get; set; }


        public static
            LSLJumpStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLJumpStatementNode(sourceRange, Err.Err);
        }


        public LSLSourceCodeRange JumpKeywordSourceCodeRange { get; }

        public LSLSourceCodeRange LabelNameSourceCodeRange { get; }

        public LSLSourceCodeRange SemiColonSourceCodeRange { get; }
    }
}