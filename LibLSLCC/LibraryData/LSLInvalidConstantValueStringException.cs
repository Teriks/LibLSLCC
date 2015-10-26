using System;
using System.Runtime.Serialization;

namespace LibLSLCC.LibraryData
{
    /// <summary>
    /// Thrown by functions that deal with subset names, when an invalid format for a subset name is encountered
    /// </summary>
    [Serializable]
    public class LSLInvalidConstantValueStringException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LSLInvalidConstantValueStringException()
        {
        }

        /// <summary>
        /// Construct with message.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public LSLInvalidConstantValueStringException(string message) : base(message)
        {
        }

        /// <summary>
        /// Construct with message and inner exception.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        public LSLInvalidConstantValueStringException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected LSLInvalidConstantValueStringException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
