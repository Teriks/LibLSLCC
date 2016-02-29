#region FileInfo

// 
// File: LSLDefaultExpressionValidator.cs
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

using System;
using System.Collections.Generic;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     The default expression validator can validate and return results for all possible binary operations, unary
    ///     operations
    ///     etc.. in standard LSL
    ///     validations for expression types in lists/vectors/rotations and function call parameters match that of standard LSL
    /// </summary>
    public sealed class LSLDefaultExpressionValidator : ILSLExpressionValidator
    {
        private readonly Dictionary<string, LSLType> _operations = new Dictionary<string, LSLType>();


        /// <summary>
        ///     Constructs the expression validator
        /// </summary>
        public LSLDefaultExpressionValidator()
        {
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Add, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Add, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Add, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Add, LSLType.Integer, LSLType.Float);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Add, LSLType.Vector, LSLType.Vector);
            AddBinaryOperation(LSLType.Rotation, LSLBinaryOperationType.Add, LSLType.Rotation, LSLType.Rotation);
            AddBinaryOperation(LSLType.String, LSLBinaryOperationType.Add, LSLType.String, LSLType.String);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Subtract, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Subtract, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Subtract, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Subtract, LSLType.Integer, LSLType.Float);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Subtract, LSLType.Vector, LSLType.Vector);
            AddBinaryOperation(LSLType.Rotation, LSLBinaryOperationType.Subtract, LSLType.Rotation, LSLType.Rotation);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Divide, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Divide, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Divide, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Divide, LSLType.Integer, LSLType.Float);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Divide, LSLType.Integer, LSLType.Vector);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Divide, LSLType.Float, LSLType.Vector);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Divide, LSLType.Rotation, LSLType.Vector);
            AddBinaryOperation(LSLType.Rotation, LSLBinaryOperationType.Divide, LSLType.Rotation, LSLType.Rotation);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Multiply, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Multiply, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Multiply, LSLType.Vector, LSLType.Vector);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Multiply, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Multiply, LSLType.Integer, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Multiply, LSLType.Vector, LSLType.Vector);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Multiply, LSLType.Vector, LSLType.Float);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Multiply, LSLType.Integer, LSLType.Vector);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Multiply, LSLType.Float, LSLType.Vector);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Multiply, LSLType.Rotation, LSLType.Vector);
            AddBinaryOperation(LSLType.Rotation, LSLBinaryOperationType.Multiply, LSLType.Rotation, LSLType.Rotation);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Modulus, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Modulus, LSLType.Vector, LSLType.Vector);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.BitwiseOr, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.RightShift, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.LeftShift, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.BitwiseXor, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.BitwiseAnd, LSLType.Integer, LSLType.Integer);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.LogicalAnd, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.LogicalOr, LSLType.Integer, LSLType.Integer);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Equals, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Equals, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Equals, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Equals, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Equals, LSLType.Vector, LSLType.Integer);
            AddBinaryOperation(LSLType.Rotation, LSLBinaryOperationType.Equals, LSLType.Rotation, LSLType.Integer);
            AddBinaryOperation(LSLType.String, LSLBinaryOperationType.Equals, LSLType.String, LSLType.Integer);
            AddBinaryOperation(LSLType.Key, LSLBinaryOperationType.Equals, LSLType.Key, LSLType.Integer);


            AddBinaryOperation(LSLType.String, LSLBinaryOperationType.NotEquals, LSLType.Key, LSLType.Integer);
            AddBinaryOperation(LSLType.Key, LSLBinaryOperationType.NotEquals, LSLType.String, LSLType.Integer);

            AddBinaryOperation(LSLType.String, LSLBinaryOperationType.Equals, LSLType.Key, LSLType.Integer);
            AddBinaryOperation(LSLType.Key, LSLBinaryOperationType.Equals, LSLType.String, LSLType.Integer);


            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.NotEquals, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.NotEquals, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.NotEquals, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.NotEquals, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.NotEquals, LSLType.Vector, LSLType.Integer);
            AddBinaryOperation(LSLType.Rotation, LSLBinaryOperationType.NotEquals, LSLType.Rotation, LSLType.Integer);
            AddBinaryOperation(LSLType.String, LSLBinaryOperationType.NotEquals, LSLType.String, LSLType.Integer);
            AddBinaryOperation(LSLType.Key, LSLBinaryOperationType.NotEquals, LSLType.Key, LSLType.Integer);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.GreaterThan, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.GreaterThan, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.GreaterThan, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.GreaterThan, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.LessThan, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.LessThan, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.LessThan, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.LessThan, LSLType.Integer, LSLType.Integer);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.GreaterThanEqual, LSLType.Integer,
                LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.GreaterThanEqual, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.GreaterThanEqual, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.GreaterThanEqual, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.LessThanEqual, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.LessThanEqual, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.LessThanEqual, LSLType.Float, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.LessThanEqual, LSLType.Integer, LSLType.Integer);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.Assign, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Assign, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.Assign, LSLType.Integer, LSLType.Float);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.Assign, LSLType.Vector, LSLType.Vector);
            AddBinaryOperation(LSLType.Rotation, LSLBinaryOperationType.Assign, LSLType.Rotation, LSLType.Rotation);
            AddBinaryOperation(LSLType.String, LSLBinaryOperationType.Assign, LSLType.String, LSLType.String);
            AddBinaryOperation(LSLType.Key, LSLBinaryOperationType.Assign, LSLType.Key, LSLType.Key);

            AddBinaryOperation(LSLType.String, LSLBinaryOperationType.Assign, LSLType.Key, LSLType.String);
            AddBinaryOperation(LSLType.Key, LSLBinaryOperationType.Assign, LSLType.String, LSLType.Key);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.AddAssign, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.AddAssign, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.AddAssign, LSLType.Integer, LSLType.Float);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.AddAssign, LSLType.Vector, LSLType.Vector);
            AddBinaryOperation(LSLType.Rotation, LSLBinaryOperationType.AddAssign, LSLType.Rotation, LSLType.Rotation);
            AddBinaryOperation(LSLType.String, LSLBinaryOperationType.AddAssign, LSLType.String, LSLType.String);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.SubtractAssign, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.SubtractAssign, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.SubtractAssign, LSLType.Integer, LSLType.Float);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.SubtractAssign, LSLType.Vector, LSLType.Vector);
            AddBinaryOperation(LSLType.Rotation, LSLBinaryOperationType.SubtractAssign, LSLType.Rotation,
                LSLType.Rotation);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.DivideAssign, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.DivideAssign, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.DivideAssign, LSLType.Integer, LSLType.Float);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.DivideAssign, LSLType.Integer, LSLType.Vector);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.DivideAssign, LSLType.Float, LSLType.Vector);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.DivideAssign, LSLType.Rotation, LSLType.Vector);
            AddBinaryOperation(LSLType.Rotation, LSLBinaryOperationType.DivideAssign, LSLType.Rotation, LSLType.Rotation);


            //causes the linden compiler to crash if in a list expression, but works stand alone
            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.MultiplyAssign, LSLType.Float, LSLType.Integer);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.MultiplyAssign, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.MultiplyAssign, LSLType.Float, LSLType.Float);
            AddBinaryOperation(LSLType.Float, LSLBinaryOperationType.MultiplyAssign, LSLType.Integer, LSLType.Float);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.MultiplyAssign, LSLType.Integer, LSLType.Vector);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.MultiplyAssign, LSLType.Float, LSLType.Vector);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.MultiplyAssign, LSLType.Rotation, LSLType.Vector);
            AddBinaryOperation(LSLType.Rotation, LSLBinaryOperationType.MultiplyAssign, LSLType.Rotation,
                LSLType.Rotation);

            AddBinaryOperation(LSLType.Integer, LSLBinaryOperationType.ModulusAssign, LSLType.Integer, LSLType.Integer);
            AddBinaryOperation(LSLType.Vector, LSLBinaryOperationType.ModulusAssign, LSLType.Vector, LSLType.Vector);


            //yes strange, but it's in LSL. this cast results in a list containing the item on the right.
            //Like this: (list)"I am a list element now";
            AddCastOperation(LSLType.List, LSLType.String, LSLType.List);
            AddCastOperation(LSLType.List, LSLType.Key, LSLType.List);
            AddCastOperation(LSLType.List, LSLType.Integer, LSLType.List);
            AddCastOperation(LSLType.List, LSLType.Float, LSLType.List);
            AddCastOperation(LSLType.List, LSLType.Vector, LSLType.List);
            AddCastOperation(LSLType.List, LSLType.Rotation, LSLType.List);


            AddCastOperation(LSLType.List, LSLType.List, LSLType.List);


            AddCastOperation(LSLType.String, LSLType.List, LSLType.String);

            AddCastOperation(LSLType.Integer, LSLType.Integer, LSLType.Integer);
            AddCastOperation(LSLType.Integer, LSLType.Float, LSLType.Integer);
            AddCastOperation(LSLType.Integer, LSLType.String, LSLType.Integer);
            AddCastOperation(LSLType.Float, LSLType.Float, LSLType.Float);
            AddCastOperation(LSLType.Float, LSLType.Integer, LSLType.Float);
            AddCastOperation(LSLType.Float, LSLType.String, LSLType.Float);
            AddCastOperation(LSLType.Vector, LSLType.Vector, LSLType.Vector);
            AddCastOperation(LSLType.Vector, LSLType.String, LSLType.Vector);
            AddCastOperation(LSLType.Rotation, LSLType.Rotation, LSLType.Rotation);
            AddCastOperation(LSLType.Rotation, LSLType.String, LSLType.Rotation);
            AddCastOperation(LSLType.String, LSLType.String, LSLType.String);
            AddCastOperation(LSLType.String, LSLType.Integer, LSLType.String);
            AddCastOperation(LSLType.String, LSLType.Float, LSLType.String);
            AddCastOperation(LSLType.String, LSLType.Vector, LSLType.String);
            AddCastOperation(LSLType.String, LSLType.Rotation, LSLType.String);
            AddCastOperation(LSLType.String, LSLType.Key, LSLType.String);
            AddCastOperation(LSLType.Key, LSLType.Key, LSLType.Key);
            AddCastOperation(LSLType.Key, LSLType.String, LSLType.Key);


            AddPrefixOperation(LSLPrefixOperationType.BooleanNot, LSLType.Integer, LSLType.Integer);
            AddPrefixOperation(LSLPrefixOperationType.BitwiseNot, LSLType.Integer, LSLType.Integer);

            AddPrefixOperation(LSLPrefixOperationType.Increment, LSLType.Integer, LSLType.Integer);
            AddPrefixOperation(LSLPrefixOperationType.Decrement, LSLType.Integer, LSLType.Integer);


            //yes, you can negate vectors, try it
            AddPrefixOperation(LSLPrefixOperationType.Negative, LSLType.Rotation, LSLType.Rotation);
            AddPrefixOperation(LSLPrefixOperationType.Negative, LSLType.Vector, LSLType.Vector);


            AddPrefixOperation(LSLPrefixOperationType.Negative, LSLType.Integer, LSLType.Integer);
            AddPrefixOperation(LSLPrefixOperationType.Negative, LSLType.Float, LSLType.Float);

            AddPrefixOperation(LSLPrefixOperationType.Increment, LSLType.Float, LSLType.Float);
            AddPrefixOperation(LSLPrefixOperationType.Decrement, LSLType.Float, LSLType.Float);

            AddPostfixOperation(LSLType.Integer, LSLPostfixOperationType.Increment, LSLType.Integer);
            AddPostfixOperation(LSLType.Integer, LSLPostfixOperationType.Decrement, LSLType.Integer);
            AddPostfixOperation(LSLType.Float, LSLPostfixOperationType.Increment, LSLType.Float);
            AddPostfixOperation(LSLType.Float, LSLPostfixOperationType.Decrement, LSLType.Float);
        }


        /// <summary>
        ///     Validates that an expression of some type can be returned from a function given the
        ///     functions return type.
        /// </summary>
        /// <param name="returnType">The return type of the function the expression is being returned from.</param>
        /// <param name="returnedExpression">The expression attempting to be returned.</param>
        /// <returns>True if the expression is allowed to be returned from the function given the expression type and return type.</returns>
        public bool ValidateReturnTypeMatch(LSLType returnType, ILSLReadOnlyExprNode returnedExpression)
        {
            var left = new LSLDummyExpr
            {
                Type = returnType,
                ExpressionType = LSLExpressionType.LocalVariable
            };

            return
                ValidateBinaryOperation(left, LSLBinaryOperationType.Assign, returnedExpression).IsValid;
        }


        /// <summary>
        ///     Determines if an expression can be used to initialize a vector literal
        /// </summary>
        /// <param name="type">The type of expression that is attempting to be used.</param>
        /// <returns>True if the expression can be inside of a vector literal.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null" />.</exception>
        public bool ValidateVectorContent(ILSLReadOnlyExprNode type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return !type.HasErrors && type.Type == LSLType.Float || type.Type == LSLType.Integer;
        }


        /// <summary>
        ///     Determines if an expression can be used to initialize a rotation literal
        /// </summary>
        /// <param name="type">The type of expression that is attempting to be used.</param>
        /// <returns>True if the expression can be inside of a rotation literal.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null" />.</exception>
        public bool ValidateRotationContent(ILSLReadOnlyExprNode type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return !type.HasErrors && (type.Type == LSLType.Float || type.Type == LSLType.Integer);
        }


        /// <summary>
        ///     Determines if an expression can be used to initialize a list literal
        /// </summary>
        /// <param name="type">The type of expression that is attempting to be used.</param>
        /// <returns>True if the expression can be inside of a list literal.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null" />.</exception>
        public bool ValidateListContent(ILSLReadOnlyExprNode type)
        {
            if (type == null) throw new ArgumentNullException("type");

            //check for void required, we do not want functions returning void in a list
            return !type.HasErrors && type.Type != LSLType.List && type.Type != LSLType.Void;
        }


        /// <summary>
        ///     Determines if an expression is valid inside of a boolean condition area, such as an if statement
        ///     or other control statement including loops.
        /// </summary>
        /// <param name="type">The type of expression that is attempting to be used.</param>
        /// <returns>True if the expression can be used inside of boolean condition area.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null" />.</exception>
        public bool ValidBooleanConditional(ILSLReadOnlyExprNode type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return
                (type.Type == LSLType.Key) ||
                (type.Type == LSLType.Integer) ||
                (type.Type == LSLType.Vector) ||
                (type.Type == LSLType.Rotation) ||
                (type.Type == LSLType.List) ||
                (type.Type == LSLType.String) ||
                (type.Type == LSLType.Float);
        }


        /// <summary>
        ///     Validates that an expression can be passed into the parameter slot of a function.
        ///     IE: That the passed expression matches up with or can be converted to the parameter type.
        /// </summary>
        /// <param name="parameter">The parameter definition.</param>
        /// <param name="parameterExpressionPassed">The expression the user has attempting to pass into the parameter.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> or <paramref name="parameterExpressionPassed"/> is <see langword="null" />.</exception>
        public bool ValidateFunctionParameter(
            LSLParameter parameter,
            ILSLReadOnlyExprNode parameterExpressionPassed)
        {
            if (parameter == null) throw new ArgumentNullException("parameter");
            if (parameterExpressionPassed == null) throw new ArgumentNullException("parameterExpressionPassed");

            if (parameterExpressionPassed.HasErrors)
            {
                return false;
            }

            if (parameter.Variadic && parameter.Type == LSLType.Void)
            {
                return true;
            }


            var left = new LSLDummyExpr
            {
                Type = parameter.Type,
                ExpressionType = LSLExpressionType.ParameterVariable
            };


            return
                ValidateBinaryOperation(left, LSLBinaryOperationType.Assign,
                    parameterExpressionPassed).IsValid;
        }


        /// <summary>
        ///     Validates and returns the type resulting from a postfix operation.
        /// </summary>
        /// <param name="left">The expression to preform the postfix operation on.</param>
        /// <param name="operation">The postfix operation preformed.</param>
        /// <returns>An <see cref="LSLExpressionValidatorResult" /> object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> is <see langword="null" />.</exception>
        public LSLExpressionValidatorResult ValidatePostfixOperation(ILSLReadOnlyExprNode left,
            LSLPostfixOperationType operation)
        {
            if (left == null) throw new ArgumentNullException("left");

            if (left.HasErrors)
            {
                return LSLExpressionValidatorResult.Error;
            }

            LSLType t;
            if (_operations.TryGetValue(left.Type + operation.ToOperatorString(), out t))
            {
                return new LSLExpressionValidatorResult(t, true);
            }

            return LSLExpressionValidatorResult.Error;
        }


        /// <summary>
        ///     Validates and returns the type resulting from a prefix operation.
        /// </summary>
        /// <param name="right">The expression to preform the prefix operation on.</param>
        /// <param name="operation">The prefix operation preformed.</param>
        /// <returns>An <see cref="LSLExpressionValidatorResult" /> object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="right"/> is <see langword="null" />.</exception>
        public LSLExpressionValidatorResult ValidatePrefixOperation(LSLPrefixOperationType operation, ILSLReadOnlyExprNode right)
        {
            if (right == null) throw new ArgumentNullException("right");

            if (right.HasErrors)
            {
                return LSLExpressionValidatorResult.Error;
            }

            LSLType t;
            if (_operations.TryGetValue(operation.ToOperatorString() + right.Type, out t))
            {
                return new LSLExpressionValidatorResult(t, true);
            }

            return LSLExpressionValidatorResult.Error;
        }


        /// <summary>
        ///     Validates and returns the type resulting from a cast operation.
        /// </summary>
        /// <param name="castedExpression">The expression to preform the cast on.</param>
        /// <param name="castTo">The type that is being casted to.</param>
        /// <returns>An <see cref="LSLExpressionValidatorResult" /> object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="castedExpression"/> is <see langword="null" />.</exception>
        public LSLExpressionValidatorResult ValidateCastOperation(LSLType castTo, ILSLReadOnlyExprNode castedExpression)
        {
            if (castedExpression == null) throw new ArgumentNullException("castedExpression");

            if (castedExpression.HasErrors)
            {
                return LSLExpressionValidatorResult.Error;
            }


            LSLType t;
            if (_operations.TryGetValue("(" + castTo + ")" + castedExpression.Type, out t))
            {
                return new LSLExpressionValidatorResult(t, true);
            }

            return LSLExpressionValidatorResult.Error;
        }


        /// <summary>
        ///     Validates and returns the type resulting from a binary operation.
        /// </summary>
        /// <param name="left">The expression to on the left of the binary operation.</param>
        /// <param name="operation">The binary operation to preform.</param>
        /// <param name="right">The expression to on the right of the binary operation.</param>
        /// <returns>An <see cref="LSLExpressionValidatorResult" /> object</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is <see langword="null" />.</exception>
        public LSLExpressionValidatorResult ValidateBinaryOperation(ILSLReadOnlyExprNode left, LSLBinaryOperationType operation,
            ILSLReadOnlyExprNode right)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");

            if (left.HasErrors || right.HasErrors)
            {
                return LSLExpressionValidatorResult.Error;
            }


            if (left.Type == LSLType.List && operation == LSLBinaryOperationType.AddAssign)
            {
                return LSLExpressionValidatorResult.List;
            }


            if (left.Type == LSLType.List || right.Type == LSLType.List)
            {
                if (operation == LSLBinaryOperationType.Add)
                {
                    return LSLExpressionValidatorResult.List;
                }
            }


            if (left.Type == LSLType.List && right.Type == LSLType.List)
            {
                if (operation == LSLBinaryOperationType.Equals || operation == LSLBinaryOperationType.NotEquals)
                {
                    return LSLExpressionValidatorResult.Integer;
                }
                if (operation == LSLBinaryOperationType.Assign)
                {
                    return LSLExpressionValidatorResult.List;
                }
            }


            LSLType t;
            if (_operations.TryGetValue(left.Type + operation.ToOperatorString() + right.Type, out t))
            {
                return new LSLExpressionValidatorResult(t, true);
            }

            return new LSLExpressionValidatorResult(t, false);
        }


        private void AddPostfixOperation(LSLType left, LSLPostfixOperationType operation, LSLType result)
        {
            _operations.Add(left + operation.ToOperatorString(), result);
        }


        private void AddPrefixOperation(LSLPrefixOperationType operation, LSLType right, LSLType result)
        {
            _operations.Add(operation.ToOperatorString() + right, result);
        }


        private void AddBinaryOperation(LSLType left, LSLBinaryOperationType operation, LSLType right, LSLType result)
        {
            _operations.Add(left + operation.ToOperatorString() + right, result);
        }


        private void AddCastOperation(LSLType castTo, LSLType from, LSLType result)
        {
            _operations.Add("(" + castTo + ")" + @from, result);
        }
    }
}