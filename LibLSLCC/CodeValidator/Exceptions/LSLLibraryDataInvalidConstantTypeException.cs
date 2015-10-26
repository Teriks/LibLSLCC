using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.CodeValidator.Exceptions
{
    /// <summary>
    /// An exception thrown by <see cref="LSLLibraryConstantSignature.Type"/> when it is set to <see cref="LSLType.Void"/>.
    /// It is also thrown by <see cref="LSLLibraryConstantSignature.ValueString"/> if you try to set the property when <see cref="LSLLibraryConstantSignature.Type"/> is set to <see cref="LSLType.Void"/>.
    /// </summary>
    [Serializable]
    public class LSLLibraryDataInvalidConstantTypeException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataInvalidConstantTypeException"/> class.
        /// </summary>
        public LSLLibraryDataInvalidConstantTypeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataInvalidConstantTypeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LSLLibraryDataInvalidConstantTypeException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataInvalidConstantTypeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public LSLLibraryDataInvalidConstantTypeException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataInvalidConstantTypeException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected LSLLibraryDataInvalidConstantTypeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
