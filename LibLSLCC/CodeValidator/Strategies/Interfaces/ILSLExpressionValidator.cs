#region FileInfo

// 
// File: ILSLExpressionValidator.cs
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



#endregion

namespace LibLSLCC.CodeValidator.Strategies
{
    /// <summary>
    ///     Represents the result of an expression/type validation preformed by an ILSLExpressionValidator object
    /// </summary>
    public struct LSLExpressionValidatorResult
    {
        /// <summary>
        ///     Represents a validation error.
        /// </summary>
        public static readonly LSLExpressionValidatorResult Error = new LSLExpressionValidatorResult(LSLType.Void, false);

        /// <summary>
        ///     Represents a list return type for an expression validation.
        /// </summary>
        public static readonly LSLExpressionValidatorResult List = new LSLExpressionValidatorResult(LSLType.List, true);

        /// <summary>
        ///     Represents a string return type for an expression validation.
        /// </summary>
        public static readonly LSLExpressionValidatorResult String = new LSLExpressionValidatorResult(LSLType.String,
            true);

        /// <summary>
        ///     Represents a vector return type for an expression validation.
        /// </summary>
        public static readonly LSLExpressionValidatorResult Vector = new LSLExpressionValidatorResult(LSLType.Vector,
            true);

        /// <summary>
        ///     Represents a rotation return type for an expression validation.
        /// </summary>
        public static readonly LSLExpressionValidatorResult Rotation = new LSLExpressionValidatorResult(
            LSLType.Rotation,
            true);

        /// <summary>
        ///     Represents a key return type for an expression validation.
        /// </summary>
        public static readonly LSLExpressionValidatorResult Key = new LSLExpressionValidatorResult(LSLType.Key, true);

        /// <summary>
        ///     Represents an integer return type for an expression validation.
        /// </summary>
        public static readonly LSLExpressionValidatorResult Integer = new LSLExpressionValidatorResult(LSLType.Integer,
            true);

        /// <summary>
        ///     Represents a float return type for an expression validation.
        /// </summary>
        public static readonly LSLExpressionValidatorResult Float = new LSLExpressionValidatorResult(LSLType.Float, true);


        /// <summary>
        ///     Constructs an expression validation result.
        /// </summary>
        /// <param name="resultType">The return type of the expression that was validated.</param>
        /// <param name="isValid">True specifies that the validation was a success, false specifies that there was an error.</param>
        public LSLExpressionValidatorResult(LSLType resultType, bool isValid) : this()
        {
            IsValid = isValid;
            ResultType = resultType;
        }


        /// <summary>
        ///     Contains the resulting type of an expression validation.
        /// </summary>
        public LSLType ResultType { get; private set; }

        /// <summary>
        ///     True if the expression passed validation, false if it did not.
        /// </summary>
        public bool IsValid { get; private set; }


        /// <summary>
        ///     Equates <see cref="LSLExpressionValidatorResult" /> objects using equality between their ResultType property and
        ///     IsValid property.
        /// </summary>
        /// <param name="other">The other  <see cref="LSLExpressionValidatorResult" /> to compare this one to.</param>
        /// <returns>
        ///     True if both  <see cref="LSLExpressionValidatorResult" /> objects have the same  <see cref="ResultType" /> and
        ///     <see cref="IsValid" /> values.
        /// </returns>
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


        /// <summary>
        ///     Equality operator that checks if <see cref="ResultType" /> and <see cref="IsValid" /> are the same in both objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>
        ///     True if both <see cref="LSLExpressionValidatorResult" /> objects have the same <see cref="ResultType" /> and
        ///     <see cref="IsValid" /> values.
        /// </returns>
        public static bool operator ==(LSLExpressionValidatorResult left, LSLExpressionValidatorResult right)
        {
            return left.IsValid == right.IsValid && left.ResultType == right.ResultType;
        }


        /// <summary>
        ///     In-Equality operator that checks if <see cref="ResultType" /> or <see cref="IsValid" /> are different in both
        ///     objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>
        ///     True if both <see cref="LSLExpressionValidatorResult" /> objects  <see cref="ResultType" /> or
        ///     <see cref="IsValid" /> property values differ from each others.
        /// </returns>
        public static bool operator !=(LSLExpressionValidatorResult left, LSLExpressionValidatorResult right)
        {
            return left.IsValid != right.IsValid || left.ResultType != right.ResultType;
        }
    }


    /// <summary>
    ///     An interface used by <see cref="LSLCodeValidator" /> to validate and retrieve the return type of
    ///     expressions such as binary expressions, as well as other expressions that return a type.
    ///     this interface also defines type validation for expressions in boolean condition areas, vectors, rotations, lists
    ///     and function call parameters.
    /// </summary>
    public interface ILSLExpressionValidator
    {
        /// <summary>
        ///     Validates the types involved in a postfix operation and returns an object describing whether or not
        ///     the postfix operation expression was valid for the involved types.
        ///     The return type of the postfix operation is also evaluated and returned in the result.
        /// </summary>
        /// <param name="left">The expression on the left of the postfix operation.</param>
        /// <param name="operation">The postfix operation preformed.</param>
        /// <returns>An <see cref="LSLExpressionValidatorResult" /> containing the validation results.</returns>
        LSLExpressionValidatorResult ValidatePostfixOperation(ILSLExprNode left, LSLPostfixOperationType operation);


        /// <summary>
        ///     Validates the types involved in a prefix operation and returns an object describing whether or not
        ///     the prefix operation expression was valid for the involved types.
        ///     The return type of the prefix operation is also evaluated and returned in the result.
        /// </summary>
        /// <param name="right">The expression on the right of the prefix operation.</param>
        /// <param name="operation">The prefix operation preformed.</param>
        /// <returns>An <see cref="LSLExpressionValidatorResult" /> containing the validation results.</returns>
        LSLExpressionValidatorResult ValidatePrefixOperation(LSLPrefixOperationType operation, ILSLExprNode right);


        /// <summary>
        ///     Validates that a cast to a certain type can be preformed on a expression node of a certain type.
        ///     Returns an object describing whether or not the cast is valid.
        ///     The return type of the cast operation is also evaluated and returned in the result.
        /// </summary>
        /// <param name="castTo">The type being casted to.</param>
        /// <param name="castedExpression">The expression being casted.</param>
        /// <returns>An <see cref="LSLExpressionValidatorResult" /> containing the validation results.</returns>
        LSLExpressionValidatorResult ValidateCastOperation(LSLType castTo, ILSLExprNode castedExpression);


        /// <summary>
        ///     Validates the types involved in a binary operation and returns an object describing whether or not
        ///     the binary operation expression was valid for the involved types.
        ///     The return type of the binary operation is also evaluated and returned in the result.
        /// </summary>
        /// <param name="left">The expression on the left of the binary operation.</param>
        /// <param name="operation">The binary operation preformed.</param>
        /// <param name="right">The expression on the left of the binary operation.</param>
        /// <returns>An <see cref="LSLExpressionValidatorResult" /> containing the validation results.</returns>
        LSLExpressionValidatorResult ValidateBinaryOperation(ILSLExprNode left, LSLBinaryOperationType operation,
            ILSLExprNode right);


        /// <summary>
        ///     Validates that an expression matches the return type of a function, or rather if a certain expression can be
        ///     returned
        ///     from a function with the given return type.
        /// </summary>
        /// <param name="returnType">The return type of the function.</param>
        /// <param name="returnedExpression">The expression to be returned from the function.</param>
        /// <returns>
        ///     True if the given expression is of a valid type to be returned from a function with the given return type,
        ///     false if otherwise.
        /// </returns>
        bool ValidateReturnTypeMatch(LSLType returnType, ILSLExprNode returnedExpression);


        /// <summary>
        ///     Validates that a given expression is able to be used inside of a vector literal initializer list.
        /// </summary>
        /// <param name="type">
        ///     The expression the user is attempting to use to initialize one of the components of a vector
        ///     literal.
        /// </param>
        /// <returns>True if the expression can be used in a vector literal initializer list, false if otherwise.</returns>
        bool ValidateVectorContent(ILSLExprNode type);


        /// <summary>
        ///     Validates that a given expression is able to be used inside of a rotation literal initializer list.
        /// </summary>
        /// <param name="type">
        ///     The expression the user is attempting to use to initialize one of the components of a rotation
        ///     literal.
        /// </param>
        /// <returns>True if the expression can be used in a rotation literal initializer list, false if otherwise.</returns>
        bool ValidateRotationContent(ILSLExprNode type);


        /// <summary>
        ///     Validates that a given expression is able to be used inside of a list literal initializer list.
        /// </summary>
        /// <param name="type">The expression the user is attempting to use to initialize a list literal element.</param>
        /// <returns>True if the expression can be used in a list literal initializer list, false if otherwise.</returns>
        bool ValidateListContent(ILSLExprNode type);


        /// <summary>
        ///     Validates that a given expression can be used as a boolean conditional for a control or loop statement.
        /// </summary>
        /// <param name="type">
        ///     The expression the user is attempting to use as a boolean conditional in a control or loop
        ///     statement.
        /// </param>
        /// <returns>True if the expression can be used as a boolean conditional, false if otherwise.</returns>
        bool ValidBooleanConditional(ILSLExprNode type);


        /// <summary>
        ///     Validates that an expression can be passed into the parameter slot of a function.
        ///     IE: That the passed expression matches up with or can be converted to the parameter type.
        /// </summary>
        /// <param name="parameter">The parameter definition.</param>
        /// <param name="parameterExpressionPassed">The expression the user has attempting to pass into the parameter.</param>
        /// <returns></returns>
        bool ValidateFunctionParameter(LSLParameter parameter,
            ILSLExprNode parameterExpressionPassed);
    }
}