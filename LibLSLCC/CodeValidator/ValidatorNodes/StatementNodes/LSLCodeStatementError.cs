#region FileInfo

// 
// File: LSLCodeStatementError.cs
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
    public interface ILSLCodeStatementError : ILSLReadOnlyCodeStatement
    {
    }

    public class LSLCodeStatementError : ILSLCodeStatementError, ILSLCodeStatement
    {
        public LSLCodeStatementError(LSLParser.CodeStatementContext parserContext, bool isSingleBlockStatement)
        {
            ParserContext = parserContext;
            IsSingleBlockStatement = isSingleBlockStatement;
        }

        internal LSLParser.CodeStatementContext ParserContext { get; }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public bool HasReturnStatementNode
        {
            get { return false; }
        }

        public bool IsSingleBlockStatement { get; set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public ILSLCodeStatement ReturnPath { get; set; }
        public LSLDeadCodeType DeadCodeType { get; set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public bool HasErrors
        {
            get { return true; }
        }

        public LSLSourceCodeRange SourceCodeRange
        {
            get { return new LSLSourceCodeRange(ParserContext); }
        }

        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            throw new NotImplementedException("Visited LSLCodeStatementError");
        }

        public bool HasReturnPath
        {
            get { return false; }
        }

        public int StatementIndex { get; set; }
        public bool IsLastStatementInScope { get; set; }
        public bool IsDeadCode { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public ulong ScopeId { get; set; }
    }
}