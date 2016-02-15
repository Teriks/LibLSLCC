using LibLSLCC.CodeValidator.Nodes.Interfaces;

namespace LibLSLCC.CodeValidator.Enums
{
    /// <summary>
    /// Represents the different types of expression lists that an <see cref="ILSLExpressionListNode"/> can represent.
    /// </summary>
    public enum LSLExpressionListType
    {
        /// <summary>
        /// The expression list is a list literal initializer.
        /// </summary>
        ListInitializer,

        /// <summary>
        /// The expression list represents the parameters used to call a user defined function.
        /// </summary>
        UserFunctionCallParameters,

        /// <summary>
        /// The expression list represents the parameters used to call a library defined function.
        /// </summary>
        LibraryFunctionCallParameters,

        /// <summary>
        /// The expression list represents the expression list used in a for-loops afterthoughts clause.
        /// </summary>
        ForLoopAfterthoughts,

        /// <summary>
        /// The expression list represents the expression list used in a for-loops initialization clause.
        /// </summary>
        ForLoopInitExpressions
    }
}