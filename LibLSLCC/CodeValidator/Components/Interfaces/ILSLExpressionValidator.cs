#region FileInfo
// 
// 
// File: ILSLExpressionValidator.cs
// 
// Last Compile: 25/09/2015 @ 5:46 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// ============================================================
// ============================================================
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// Copyright (c) 2015, Teriks
// All rights reserved.
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

using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    public struct LSLExpressionValidatorResult
    {
        public static readonly LSLExpressionValidatorResult Error = new LSLExpressionValidatorResult(LSLType.Void, false);
        public static readonly LSLExpressionValidatorResult List = new LSLExpressionValidatorResult(LSLType.List, true);

        public static readonly LSLExpressionValidatorResult String = new LSLExpressionValidatorResult(LSLType.String,
            true);

        public static readonly LSLExpressionValidatorResult Vector = new LSLExpressionValidatorResult(LSLType.Vector,
            true);

        public static readonly LSLExpressionValidatorResult Rotation = new LSLExpressionValidatorResult(
            LSLType.Rotation,
            true);

        public static readonly LSLExpressionValidatorResult Key = new LSLExpressionValidatorResult(LSLType.Key, true);

        public static readonly LSLExpressionValidatorResult Integer = new LSLExpressionValidatorResult(LSLType.Integer,
            true);

        public static readonly LSLExpressionValidatorResult Float = new LSLExpressionValidatorResult(LSLType.Float, true);

        public LSLExpressionValidatorResult(LSLType resultType, bool isValid) : this()
        {
            IsValid = isValid;
            ResultType = resultType;
        }

        public LSLType ResultType { get; }
        public bool IsValid { get; }

        public bool Equals(LSLExpressionValidatorResult other)
        {
            return ResultType == other.ResultType && IsValid.Equals(other.IsValid);
        }

        /// <summary>
        ///     Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        ///     true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LSLExpressionValidatorResult && Equals((LSLExpressionValidatorResult) obj);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) ResultType*397) ^ IsValid.GetHashCode();
            }
        }

        public static bool operator ==(LSLExpressionValidatorResult left, LSLExpressionValidatorResult right)
        {
            return left.IsValid == right.IsValid && left.ResultType == right.ResultType;
        }

        public static bool operator !=(LSLExpressionValidatorResult left, LSLExpressionValidatorResult right)
        {
            return left.IsValid != right.IsValid && left.ResultType != right.ResultType;
        }
    }


    /// <summary>
    ///     An interface for a strategy used by LSLCodeValidator to validate and retrieve the return type of
    ///     expressions such as binary expressions, as well as other expressions that return a type.
    ///     this interface also defines type validation for expressions in boolean condition areas, vectors, rotations, lists
    ///     and function call parameters
    /// </summary>
    public interface ILSLExpressionValidator
    {
        LSLExpressionValidatorResult ValidatePostfixOperation(ILSLExprNode left, LSLPostfixOperationType operation);
        LSLExpressionValidatorResult ValidatePrefixOperation(LSLPrefixOperationType operation, ILSLExprNode right);
        LSLExpressionValidatorResult ValidateCastOperation(LSLType castTo, ILSLExprNode from);

        LSLExpressionValidatorResult ValidateBinaryOperation(ILSLExprNode left, LSLBinaryOperationType operation,
            ILSLExprNode right);

        bool ValidateReturnTypeMatch(LSLType returnType, ILSLExprNode returnedExpression);
        bool ValidVectorContent(ILSLExprNode type);
        bool ValidRotationContent(ILSLExprNode type);
        bool ValidListContent(ILSLExprNode type);
        bool ValidBooleanConditional(ILSLExprNode type);

        bool ValidFunctionParameter(LSLFunctionSignature functionSignature, int parameterNumber,
            ILSLExprNode parameterExpressionPassed);
    }
}