using System;
using System.Runtime.Serialization;

namespace LibLSLCC.Compilers
{
    [Serializable]
    public class LSLCompilerInternalException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public LSLCompilerInternalException()
        {
        }


        public LSLCompilerInternalException(string message) : base(message)
        {
        }


        public LSLCompilerInternalException(string message, Exception inner) : base(message, inner)
        {
        }


        protected LSLCompilerInternalException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}