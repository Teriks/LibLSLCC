#region FileInfo

// 
// File: ILSLCodeStatement.cs
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

using LibLSLCC.CodeValidator.Enums;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLReadOnlyCodeStatement : ILSLReadOnlySyntaxTreeNode, ILSLReturnPathNode
    {
        /// <summary>
        ///     The index of this statement in its scope
        /// </summary>
        int StatementIndex { get; }

        /// <summary>
        ///     Is this statement the last statement in its scope
        /// </summary>
        bool IsLastStatementInScope { get; }

        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        LSLDeadCodeType DeadCodeType { get; }

        /// <summary>
        ///     Is this statement dead code
        /// </summary>
        bool IsDeadCode { get; }

        /// <summary>
        ///     If the scope has a return path, this is set to the node that causes the function to return.
        ///     it may be a return statement, or a control chain node.
        /// </summary>
        ILSLReadOnlyCodeStatement ReturnPath { get; }

        /// <summary>
        ///     Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        ///     this is not the scopes level.
        /// </summary>
        ulong ScopeId { get; }
    }


    public interface ILSLCodeStatement : ILSLReadOnlyCodeStatement, ILSLSyntaxTreeNode
    {
        /// <summary>
        ///     The index of this statement in its scope
        /// </summary>
        new int StatementIndex { get; set; }

        /// <summary>
        ///     Is this statement the last statement in its scope
        /// </summary>
        new bool IsLastStatementInScope { get; set; }

        /// <summary>
        ///     The type of dead code that this statement is considered to be, if it is dead
        /// </summary>
        new LSLDeadCodeType DeadCodeType { get; set; }

        /// <summary>
        ///     Is this statement dead code
        /// </summary>
        new bool IsDeadCode { get; set; }

        /// <summary>
        ///     If the scope has a return path, this is set to the node that causes the function to return.
        ///     it may be a return statement, or a control chain node.
        /// </summary>
        new ILSLCodeStatement ReturnPath { get; set; }

        /// <summary>
        ///     Represents an ID number for the scope this code statement is in, they are unique per-function/event handler.
        ///     this is not the scopes level.
        /// </summary>
        new ulong ScopeId { get; set; }
    }
}