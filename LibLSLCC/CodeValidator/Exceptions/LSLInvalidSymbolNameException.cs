using System;
using System.Runtime.Serialization;

namespace LibLSLCC.CodeValidator.Exceptions
{
    /// <summary>
    /// Thrown when LSLConstantSignature, LSLFunctionSignature or LSLEventSignature have their Name
    /// set to a value that does not conform the LSL's symbol naming convention.
    /// </summary>
    [Serializable]
    public class LSLInvalidSymbolNameException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LSLInvalidSymbolNameException()
        {
        }

        /// <summary>
        /// Construct with message.
        /// </summary>
        /// <param name="message"></param>
        public LSLInvalidSymbolNameException(string message) : base(message)
        {
        }

        /// <summary>
        /// Construct with message and inner exception.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public LSLInvalidSymbolNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected LSLInvalidSymbolNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
