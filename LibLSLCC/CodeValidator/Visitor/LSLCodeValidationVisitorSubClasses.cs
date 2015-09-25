#region FileInfo

// 
// File: LSLCodeValidationVisitorSubClasses.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:25 PM
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

using Antlr4.Runtime;

#endregion

namespace LibLSLCC.CodeValidator.Visitor
{
    internal partial class LSLCodeValidationVisitor
    {
        #region InternalClasses

        /// <summary>
        ///     Represents a generic binary expression operation.
        ///     I made alternative grammar label names for each different type of expression operation so that I could
        ///     differentiate between postfix, casts etc.. unfortunately each type of binary operation must be named
        ///     uniquely or antlr will complain, so the different binary expression visitor functions build this object
        ///     and pass it it to VisitBinaryExpression.  All binary expression contexts have the same properties,
        ///     but they cannot be made to derive from one type as antlr generates them and it does not have a feature to
        ///     allow it
        /// </summary>
        private struct BinaryExpressionContext
        {
            public readonly LSLParser.ExpressionContext LeftContext;
            public readonly bool ModifiesLValue;
            public readonly IToken OperationToken;
            public readonly LSLParser.ExpressionContext OriginalContext;
            public readonly LSLParser.ExpressionContext RightContext;

            public BinaryExpressionContext(LSLParser.ExpressionContext exprLvalue, IToken operationToken,
                LSLParser.ExpressionContext exprRvalue, LSLParser.ExpressionContext originalContext,
                bool modifiesLValue = false)
            {
                LeftContext = exprLvalue;
                OperationToken = operationToken;
                RightContext = exprRvalue;
                OriginalContext = originalContext;
                ModifiesLValue = modifiesLValue;
            }
        }

        #endregion
    }
}