using System;
using System.Runtime.Serialization;

namespace lslcc
{
    [Serializable]
    internal class LogWriteException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public LogWriteException()
        {
        }


        public LogWriteException(string message) : base(message)
        {
        }


        public LogWriteException(string message, Exception inner) : base(message, inner)
        {
        }


        protected LogWriteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}