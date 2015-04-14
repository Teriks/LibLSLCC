using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    public struct LSLExpressionValidatorResult
    {
        public bool Equals(LSLExpressionValidatorResult other)
        {
            return ResultType == other.ResultType && IsValid.Equals(other.IsValid);
        }



        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LSLExpressionValidatorResult && Equals((LSLExpressionValidatorResult) obj);
        }



        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) ResultType*397) ^ IsValid.GetHashCode();
            }
        }



        public readonly static  LSLExpressionValidatorResult Error = new LSLExpressionValidatorResult(LSLType.Void, false);

        public readonly static LSLExpressionValidatorResult List = new LSLExpressionValidatorResult(LSLType.List, true);

        public readonly static LSLExpressionValidatorResult String = new LSLExpressionValidatorResult(LSLType.String, true);

        public readonly static LSLExpressionValidatorResult Vector = new LSLExpressionValidatorResult(LSLType.Vector, true);

        public readonly static LSLExpressionValidatorResult Rotation = new LSLExpressionValidatorResult(LSLType.Rotation,
            true);

        public readonly static LSLExpressionValidatorResult Key = new LSLExpressionValidatorResult(LSLType.Key, true);

        public readonly static LSLExpressionValidatorResult Integer = new LSLExpressionValidatorResult(LSLType.Integer, true);

        public readonly static LSLExpressionValidatorResult Float = new LSLExpressionValidatorResult(LSLType.Float, true);



        public static bool operator ==(LSLExpressionValidatorResult left, LSLExpressionValidatorResult right)
        {
            return left.IsValid == right.IsValid && left.ResultType == right.ResultType;
        }

        public static bool operator !=(LSLExpressionValidatorResult left, LSLExpressionValidatorResult right)
        {
            return left.IsValid != right.IsValid && left.ResultType != right.ResultType;
        }







        public LSLExpressionValidatorResult(LSLType resultType, bool isValid) : this()
        {
            IsValid = isValid;
            ResultType = resultType;
        }



        public LSLType ResultType { get; private set; }

        public bool IsValid { get; private set; }
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