using System;
using System.Runtime.Serialization;

namespace lslcc
{
    [Serializable]
    internal class ArgParseException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ArgParseException()
        {
        }


        public ArgParseException(string message) : base(message)
        {
        }


        public ArgParseException(string message, Exception inner) : base(message, inner)
        {
        }


        protected ArgParseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}