using LibLSLCC.CodeValidator.Nodes.Interfaces;

namespace LibLSLCC.CodeValidator.Enums
{
    /// <summary>
    /// Represents a parameter list type of a <see cref="ILSLParameterListNode"/>
    /// </summary>
    public enum LSLParameterListType
    {
        /// <summary>
        /// Parameter list belongs to a function declaration.
        /// </summary>
        FunctionParameters,

        /// <summary>
        /// Parameter list belongs to an event handler.
        /// </summary>
        EventParameters,
    }
}