using System;
using System.Runtime.Serialization;

namespace LibLSLCC.LibraryData
{

    /// <summary>
    /// This exception is thrown by LSLLibraryDataProvider when a duplicate signature is added to 
    /// a subset.
    /// </summary>
    [Serializable]
    public class LSLDuplicateSignatureException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LSLDuplicateSignatureException()
        {
        }

        /// <summary>
        /// Construct with a message.
        /// </summary>
        /// <param name="message"></param>
        public LSLDuplicateSignatureException(string message)
            : base(message)
        {
        }


        /// <summary>
        /// Construct with a message and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public LSLDuplicateSignatureException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected LSLDuplicateSignatureException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}