namespace LibLSLCC.CodeValidator.Enums
{

    /// <summary>
    /// Represents different types of conditional/control statements
    /// </summary>
    public enum LSLConditionalStatementType
    {
        /// <summary>
        /// An If statement.
        /// </summary>
        If,

        /// <summary>
        /// An Else-If statement.
        /// </summary>
        ElseIf,

        /// <summary>
        /// A While loop.
        /// </summary>
        While,

        /// <summary>
        /// A Do-While loop.
        /// </summary>
        DoWhile,

        /// <summary>
        /// A For loop.
        /// </summary>
        For
    }
}