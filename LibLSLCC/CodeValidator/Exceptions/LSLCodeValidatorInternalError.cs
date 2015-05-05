using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace LibLSLCC.CodeValidator.Exceptions
{
    [Serializable]
    public class LSLCodeValidatorInternalError : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public LSLCodeValidatorInternalError()
        {
        }



        public LSLCodeValidatorInternalError(string message) : base(message)
        {
        }



        public LSLCodeValidatorInternalError(string message, Exception inner) : base(message, inner)
        {
        }



        protected LSLCodeValidatorInternalError(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }


        public static LSLCodeValidatorInternalError VisitContextInvalidState(string visitFunctionName,
            bool prePass = false)
        {
            if (prePass)
            {
                return new LSLCodeValidatorInternalError("In visitor function: " + visitFunctionName +
                                                         ", context did not meet pre-requisites.");
            }

            return
                new LSLCodeValidatorInternalError("In visitor function: " + visitFunctionName +
                                                  ", visited context did not meet pre-requisites in pre-pass.");
        }


        public static LSLCodeValidatorInternalError ContextInInvalidState(string visitFunctionName, MemberInfo contextType,
            bool prePass = false)
        {
            if (prePass)
            {
                return new LSLCodeValidatorInternalError("In function: " + visitFunctionName + ", " + contextType.Name +
                                                         " type context did not meet pre-requisites.");
            }

            return
                new LSLCodeValidatorInternalError("In function: " + visitFunctionName + ", " + contextType.Name +
                                                  " type context did not meet pre-requisites in pre-pass.");
        }



        public static LSLCodeValidatorInternalError VisitReturnTypeException(string visitFunctionName,
            MemberInfo expectedReturnType)
        {
            return new LSLCodeValidatorInternalError(
                visitFunctionName + " did not return " + expectedReturnType.Name + " or derived");
        }
    }
}