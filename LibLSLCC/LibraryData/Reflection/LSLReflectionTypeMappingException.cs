using System;
using System.Runtime.Serialization;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{
    /// <summary>
    /// Thrown by de-serialization methods in <see cref="LSLLibraryDataReflectionSerializer"/> 
    /// </summary>
    [Serializable]
    public class LSLReflectionTypeMappingException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// The type that no mapping existed for.
        /// </summary>
        public Type MissingType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLReflectionTypeMappingException"/> class.
        /// </summary>
        public LSLReflectionTypeMappingException(Type missingType)
        {
            MissingType = missingType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLReflectionTypeMappingException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="missingType">The type that was missing a mapping to convert it into an <see cref="LSLType"/></param>
        public LSLReflectionTypeMappingException(string message, Type missingType) : base(message)
        {
            MissingType = missingType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLReflectionTypeMappingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="missingType">The type that was missing a mapping to convert it into an <see cref="LSLType"/></param>
        public LSLReflectionTypeMappingException(string message, Exception inner, Type missingType) : base(message, inner)
        {
            MissingType = missingType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLReflectionTypeMappingException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected LSLReflectionTypeMappingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}