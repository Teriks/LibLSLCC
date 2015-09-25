#region FileInfo

// 
// File: ILSLExpressionValidator.cs
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