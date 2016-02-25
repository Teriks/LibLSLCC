#region FileInfo

// 
// File: LSLCodeValidationVisitorSubClasses.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 

#endregion

#region Imports

using Antlr4.Runtime;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Internal
{
    internal sealed partial class LSLCodeValidatorVisitor
    {
        #region InternalClasses

        /// <summary>
        ///     Represents a generic binary expression operation.
        ///     I made alternative grammar label names for each different type of expression operation so that I could
        ///     differentiate between postfix, casts etc.. unfortunately each type of binary operation must be named
        ///     uniquely or ANTLR will complain, so the different binary expression visitor functions build this object
        ///     and pass it to VisitBinaryExpression.  All binary expression contexts have the same properties,
        ///     but they cannot be made to derive from one type as ANTLR generates them and it does not have a feature to
        ///     allow it
        /// </summary>
        private struct BinaryExpressionContext
        {
            public readonly LSLParser.ExpressionContext LeftContext;
            public readonly IToken OperationToken;
            public readonly LSLParser.ExpressionContext OriginalContext;
            public readonly LSLParser.ExpressionContext RightContext;


            public BinaryExpressionContext(LSLParser.ExpressionContext exprLvalue, IToken operationToken,
                LSLParser.ExpressionContext exprRvalue, LSLParser.ExpressionContext originalContext)
            {
                LeftContext = exprLvalue;
                OperationToken = operationToken;
                RightContext = exprRvalue;
                OriginalContext = originalContext;
            }
        }


        /// <summary>
        ///     Represents a generic assignment expression operation.
        ///     Similar use to the struct above, except for assignment and modifying assignment operators.
        /// </summary>
        private struct AssignmentExpressionContext
        {
            public readonly ILSLExprNode LeftExpr;
            public readonly IToken OperationToken;
            public readonly LSLParser.ExpressionContext OriginalContext;
            public readonly LSLParser.ExpressionContext RightContext;


            public AssignmentExpressionContext(ILSLExprNode exprLvalue, IToken operationToken,
                LSLParser.ExpressionContext exprRvalue, LSLParser.ExpressionContext originalContext)
            {
                LeftExpr = exprLvalue;
                OperationToken = operationToken;
                RightContext = exprRvalue;
                OriginalContext = originalContext;
            }
        }

        #endregion
    }
}