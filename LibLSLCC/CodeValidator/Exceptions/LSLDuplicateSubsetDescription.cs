using System;
using System.Runtime.Serialization;

namespace LibLSLCC.CodeValidator.Exceptions
{
    /// <summary>
    /// This exception is thrown by LSLLibraryDataProvider when a subset description for the same subset is added more than once.
    /// </summary>
    [Serializable]
    public class LSLDuplicateSubsetDescription : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LSLDuplicateSubsetDescription()
        {
        }

        /// <summary>
        /// Construct with a message.
        /// </summary>
        /// <param name="message"></param>
        public LSLDuplicateSubsetDescription(string message)
            : base(message)
        {
        }


        /// <summary>
        /// Construct with a message and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public LSLDuplicateSubsetDescription(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected LSLDuplicateSubsetDescription(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}