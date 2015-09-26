#region FileInfo
// 
// File: LSLCodeValidatorInternalError.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
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