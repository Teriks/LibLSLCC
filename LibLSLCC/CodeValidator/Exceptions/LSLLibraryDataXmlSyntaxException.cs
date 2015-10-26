using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using LibLSLCC.CodeValidator.Components;

namespace LibLSLCC.CodeValidator.Exceptions
{
    /// <summary>
    /// Exception thrown by <see cref="LSLLibraryDataXmlSerializer.Parse"/> and the XML reading methods of <see cref="LSLXmlLibraryDataProvider"/>
    /// when there are syntax errors in XML library data.
    /// </summary>
    [Serializable]
    public class LSLLibraryDataXmlSyntaxException : Exception
    {

        /// <summary>
        /// The line number on which the exception occured.
        /// </summary>
        public int LineNumber { get; private set; }

        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException"/> class.
        /// </summary>
        public LSLLibraryDataXmlSyntaxException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LSLLibraryDataXmlSyntaxException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public LSLLibraryDataXmlSyntaxException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException"/> class.
        /// </summary>
        /// <param name="lineNumber">The line number the syntax error occurred on.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public LSLLibraryDataXmlSyntaxException(int lineNumber, string message, Exception inner) : base("[Library Data XML Error Line # "+lineNumber+"]: "+message, inner)
        {
            this.LineNumber = lineNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected LSLLibraryDataXmlSyntaxException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException"/> class.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="message">The message.</param>
        public LSLLibraryDataXmlSyntaxException(int lineNumber, string message) : base("[Library Data XML Error Line # " + lineNumber + "]: " + message)
        {
            LineNumber = lineNumber;
        }
    }
}
