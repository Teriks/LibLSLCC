using System;
using System.Runtime.Serialization;

namespace LibLSLCC.LibraryData
{
    /// <summary>
    /// This exception is thrown by LSLLibraryDataProvider when a signature is added that belongs to a subset with no description
    /// a subset.
    /// </summary>
    [Serializable]
    public class LSLMissingSubsetDescriptionException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LSLMissingSubsetDescriptionException()
        {
        }

        /// <summary>
        /// Construct with a message.
        /// </summary>
        /// <param name="message"></param>
        public LSLMissingSubsetDescriptionException(string message)
            : base(message)
        {
        }


        /// <summary>
        /// Construct with a message and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public LSLMissingSubsetDescriptionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected LSLMissingSubsetDescriptionException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}