using System;
using System.Runtime.Serialization;
using LibLSLCC.CodeValidator.Components;

namespace LibLSLCC.CodeValidator.Exceptions
{
    /// <summary>
    /// This exception is thrown by <see cref="LSLLibraryDataProvider"/> when a subset description for the same subset is added more than once.
    /// </summary>
    [Serializable]
    public class LSLDuplicateSubsetDescriptionException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LSLDuplicateSubsetDescriptionException()
        {
        }

        /// <summary>
        /// Construct with a message.
        /// </summary>
        /// <param name="message"></param>
        public LSLDuplicateSubsetDescriptionException(string message)
            : base(message)
        {
        }


        /// <summary>
        /// Construct with a message and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public LSLDuplicateSubsetDescriptionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected LSLDuplicateSubsetDescriptionException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}