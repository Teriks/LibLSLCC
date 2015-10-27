using System;
using System.Runtime.Serialization;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Base class for exceptions thrown by the de-serialization methods in <see cref="LSLLibraryDataReflectionSerializer"/> 
    /// </summary>
    [Serializable]
    public class LSLLibraryDataReflectionException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataReflectionException"/> class.
        /// </summary>
        public LSLLibraryDataReflectionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataReflectionException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LSLLibraryDataReflectionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataReflectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public LSLLibraryDataReflectionException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataReflectionException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected LSLLibraryDataReflectionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}