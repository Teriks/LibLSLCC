#region FileInfo

// 
// File: ILSLCodeScopeNode.cs
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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLCodeScopeNode : ILSLReadOnlyCodeStatement
    {
        /// <summary>
        ///     Constant jump descriptors for constant jumps that occur in this scope
        ///     used only with JumpStatementAnalysis is turned on and dead code caused by
        ///     jump statements is being detected
        /// </summary>
        IEnumerable<LSLConstantJumpDescription> ConstantJumps { get; }

        /// <summary>
        ///     Is this scope a single statement scope, like a brace-less 'if' branch.
        ///     true if IsCodeScope is false
        /// </summary>
        bool IsSingleStatement { get; }

        /// <summary>
        ///     Is this a normal braced code scope.
        ///     true if IsSingleStatement is false
        /// </summary>
        bool IsCodeScope { get; }

        /// <summary>
        ///     Code statements that are children of this code scope
        /// </summary>
        IEnumerable<ILSLReadOnlyCodeStatement> CodeStatements { get; }

        /// <summary>
        ///     ReturnStatementNode != null
        /// </summary>
        bool HasReturnStatementNode { get; }

        /// <summary>
        ///     FirstDeadStatementNode != null
        /// </summary>
        bool HasDeadStatementNodes { get; }

        /// <summary>
        ///     The first statement node to be considered dead, when dead code is detected
        /// </summary>
        ILSLReadOnlyCodeStatement FirstDeadStatementNode { get; }

        /// <summary>
        ///     Returns descriptions of all dead code segments in the top level of this scope,
        ///     if there are any
        /// </summary>
        IEnumerable<LSLDeadCodeSegment> DeadCodeSegments { get; }

        /// <summary>
        ///     The top level return statement for a code scope, if one exists
        /// </summary>
        LSLReturnStatementNode ReturnStatementNode { get; }

        LSLCodeScopeType CodeScopeType { get; }
    }
}