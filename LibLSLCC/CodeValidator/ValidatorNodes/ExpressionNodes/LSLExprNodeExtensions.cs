#region FileInfo

// 
// File: LSLExprNodeExtensions.cs
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
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes
{
    public static class LSLExprNodeExtensions
    {
        public static bool IsLiteral(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.Literal;
        }

        public static bool IsCompoundExpression(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.BinaryExpression ||
                   node.ExpressionType == LSLExpressionType.ParenthesizedExpression ||
                   node.ExpressionType == LSLExpressionType.PostfixExpression ||
                   node.ExpressionType == LSLExpressionType.PrefixExpression ||
                   node.ExpressionType == LSLExpressionType.TypecastExpression;
        }

        public static bool IsFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.LibraryFunction ||
                   node.ExpressionType == LSLExpressionType.UserFunction;
        }

        public static bool IsLibraryFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.LibraryFunction;
        }

        public static bool IsUserFunctionCall(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.UserFunction;
        }

        public static bool IsVariable(this ILSLReadOnlyExprNode node)
        {
            return node.ExpressionType == LSLExpressionType.GlobalVariable ||
                   node.ExpressionType == LSLExpressionType.LocalVariable;
        }

        public static bool IsLocalVariable(this ILSLReadOnlyExprNode node)
        {
            return
                node.ExpressionType == LSLExpressionType.LocalVariable;
        }

        public static bool IsGlobalVariable(this ILSLReadOnlyExprNode node)
        {
            return
                node.ExpressionType == LSLExpressionType.GlobalVariable;
        }
    }
}