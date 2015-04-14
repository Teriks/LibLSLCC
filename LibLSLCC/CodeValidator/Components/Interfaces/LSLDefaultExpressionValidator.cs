using System.Collections.Generic;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.ExpressionNodes;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    ///     The default expression validator can validate and return results for all possible binary operations, unary
    ///     operations
    ///     etc.. in standard LSL
    ///     validations for expression types in lists/vectors/rotations and function call parameters match that of standard LSL
    /// </summary>
    public class LSLDefaultExpressionValidator : ILSLExpressionValidator
    {
        private readonly Dictionary<string, LSLType> _operations = new Dictionary<string, LSLType>();



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


            //yes wtf, but its in lsl, cast results in a list containing the item on the right
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



        public bool ValidateReturnTypeMatch(LSLType returnType, ILSLExprNode returnedExpression)
        {
            var left = new LSLDummyExpr
            {
                Type = returnType,
                ExpressionType = LSLExpressionType.LocalVariable,
            };

            return
                ValidateBinaryOperation(left, LSLBinaryOperationType.Assign, returnedExpression).IsValid;
        }



        public bool ValidVectorContent(ILSLExprNode type)
        {
            return !type.HasErrors && type.Type == LSLType.Float || type.Type == LSLType.Integer;
        }



        public bool ValidRotationContent(ILSLExprNode type)
        {
            return !type.HasErrors && (type.Type == LSLType.Float || type.Type == LSLType.Integer);
        }



        public bool ValidListContent(ILSLExprNode type)
        {
            return !type.HasErrors && type.Type != LSLType.List;
        }



        public bool ValidBooleanConditional(ILSLExprNode type)
        {
            return
                (type.Type == LSLType.Key) ||
                (type.Type == LSLType.Integer) ||
                (type.Type == LSLType.Vector) ||
                (type.Type == LSLType.Rotation) ||
                (type.Type == LSLType.List) ||
                (type.Type == LSLType.String) ||
                (type.Type == LSLType.Float);
        }



        public bool ValidFunctionParameter(
            LSLFunctionSignature functionSignature,
            int parameterNumber,
            ILSLExprNode parameterExpressionPassed)
        {
            if (parameterExpressionPassed.HasErrors)
            {
                return false;
            }

            var left = new LSLDummyExpr
            {
                Type = functionSignature.Parameters[parameterNumber].Type,
                ExpressionType = LSLExpressionType.ParameterVariable
            };


            return
                ValidateBinaryOperation(left, LSLBinaryOperationType.Assign,
                    parameterExpressionPassed).IsValid;
        }



        public LSLExpressionValidatorResult ValidatePostfixOperation(ILSLExprNode left,
            LSLPostfixOperationType operation)
        {
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



        public LSLExpressionValidatorResult ValidatePrefixOperation(LSLPrefixOperationType operation, ILSLExprNode right)
        {
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



        public LSLExpressionValidatorResult ValidateCastOperation(LSLType castTo, ILSLExprNode from)
        {
            if (from.HasErrors)
            {
                return LSLExpressionValidatorResult.Error;
            }


            LSLType t;
            if (_operations.TryGetValue("(" + castTo + ")" + @from.Type, out t))
            {
                return new LSLExpressionValidatorResult(t, true);
            }

            return LSLExpressionValidatorResult.Error;
        }



        public LSLExpressionValidatorResult ValidateBinaryOperation(ILSLExprNode left, LSLBinaryOperationType operation,
            ILSLExprNode right)
        {
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