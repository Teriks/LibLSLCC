namespace LibLSLCC.CodeValidator.Enums
{
    /// <summary>
    ///     Represents the Return source of an LSLBasicExpr object
    /// </summary>
    public enum LSLExpressionType
    {
        Literal,
        LibraryFunction,
        LibraryConstant,
        UserFunction,
        LocalVariable,
        ParameterVariable,
        GlobalVariable,
        VectorComponentAccess,
        RotationComponentAccess,
        BinaryExpression,
        PostfixExpression,
        PrefixExpression,
        TypecastExpression,
        ParenthesizedExpression,
    }
}