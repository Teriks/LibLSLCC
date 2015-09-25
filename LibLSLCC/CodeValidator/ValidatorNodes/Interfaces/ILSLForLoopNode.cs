#region FileInfo

// 
// File: ILSLForLoopNode.cs
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

using LibLSLCC.CodeValidator.Primitives;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.Interfaces
{
    public interface ILSLForLoopNode : ILSLReadOnlyCodeStatement, ILSLLoopNode
    {
        LSLSourceCodeRange ForKeywordSourceCodeRange { get; }
        LSLSourceCodeRange OpenParenthSourceCodeRange { get; }
        ILSLExpressionListNode InitExpressionsList { get; }
        LSLSourceCodeRange FirstSemiColonSourceCodeRange { get; }
        LSLSourceCodeRange SecondSemiColonSourceCodeRange { get; }
        LSLSourceCodeRange CloseParenthSourceCodeRange { get; }
        ILSLExpressionListNode AfterthoughExpressions { get; }
        bool HasInitExpressions { get; }
        bool HasConditionExpression { get; }
        bool HasAfterthoughtExpressions { get; }
    }
}