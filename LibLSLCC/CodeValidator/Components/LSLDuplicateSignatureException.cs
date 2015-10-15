using System;
using System.Runtime.Serialization;

namespace LibLSLCC.CodeValidator.Components
{
    [Serializable]
    public class LSLDuplicateSignatureException : Exception
    {
        public LSLDuplicateSignatureException()
        {
        }

        public LSLDuplicateSignatureException(string message)
            : base(message)
        {
        }

        public LSLDuplicateSignatureException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected LSLDuplicateSignatureException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}