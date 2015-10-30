using System;
using System.Runtime.Serialization;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Thrown when a problem with a library data reflection attribute is discovered.
    /// </summary>
    [Serializable]
    public class LSLLibraryDataAttributeException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataAttributeException"/> class.
        /// </summary>
        public LSLLibraryDataAttributeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataAttributeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LSLLibraryDataAttributeException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataAttributeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public LSLLibraryDataAttributeException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataAttributeException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected LSLLibraryDataAttributeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}