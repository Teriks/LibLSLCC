#region FileInfo

// 
// File: LSLCodeValidatorInternalError.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:24 PM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// 
// This file is part of LibLSLCC.
// LibLSLCC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// LibLSLCC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with LibLSLCC.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Imports

using System;
using System.Reflection;
using System.Runtime.Serialization;

#endregion

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

        public static LSLCodeValidatorInternalError ContextInInvalidState(string visitFunctionName,
            MemberInfo contextType,
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