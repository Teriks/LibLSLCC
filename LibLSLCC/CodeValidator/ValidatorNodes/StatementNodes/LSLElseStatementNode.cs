#region FileInfo

// 
// File: LSLElseStatementNode.cs
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLElseStatementNode : ILSLElseStatementNode, ILSLBranchStatementNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLElseStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLElseStatementNode(LSLParser.ElseStatementContext context, LSLCodeScopeNode code,
            bool isConstantBranch)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (code == null)
            {
                throw new ArgumentNullException("code");
            }


            ParserContext = context;
            IsConstantBranch = isConstantBranch;
            Code = code;

            code.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);

            ElseKeywordSourceCodeRange = new LSLSourceCodeRange(context.else_keyword);
        }

        public IEnumerable<LSLConstantJumpDescription> ConstantJumps
        {
            get { return Code.ConstantJumps; }
        }

        public LSLCodeScopeNode Code { get; }
        internal LSLParser.ElseStatementContext ParserContext { get; private set; }
        public LSLSourceCodeRange ElseKeywordSourceCodeRange { get; }

        #region ILSLBranchStatementNode Members

        public bool IsConstantBranch { get; }

        #endregion

        #region ILSLReturnPathNode Members

        public bool HasReturnPath
        {
            get { return Code.HasReturnPath; }
        }

        #endregion

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLCodeScopeNode ILSLElseStatementNode.Code
        {
            get { return Code; }
        }

        public static
            LSLElseStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLElseStatementNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        public ILSLSyntaxTreeNode Parent { get; set; }
        public LSLSourceCodeRange SourceCodeRange { get; }


        public bool HasErrors { get; set; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitElseStatement(this);
        }

        #endregion
    }
}