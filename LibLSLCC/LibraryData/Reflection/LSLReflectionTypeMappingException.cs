#region FileInfo
// 
// File: LSLReflectionTypeMappingException.cs
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

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using LibLSLCC.CodeValidator.Enums;

namespace LibLSLCC.LibraryData.Reflection
{

    /// <summary>
    /// Thrown by de-serialization methods in <see cref="LSLLibraryDataReflectionSerializer" /> when a type encountered in a non-attributed member
    /// signature contains <see cref="Type" />('s) that were un-mappable to an <see cref="LSLType" />
    /// </summary>
    /// <seealso cref="LSLLibraryDataReflectionSerializer.FilterMethodsWithUnmappedReturnTypes" />
    /// <seealso cref="LSLLibraryDataReflectionSerializer.FilterMethodsWithUnmappedParamTypes" />
    /// <seealso cref="LSLLibraryDataReflectionSerializer.FilterConstantsWithUnmappedTypes"/>
    [Serializable]
    public class LSLReflectionTypeMappingException : LSLLibraryDataReflectionException
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
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. 
        /// </param><param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("MissingType", MissingType.ToString());

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Constructs a new <see cref="LSLReflectionTypeMappingException"/>
        /// </summary>
        public LSLReflectionTypeMappingException()
        {
        }



        /// <summary>
        /// Constructs a new <see cref="LSLReflectionTypeMappingException"/>
        /// </summary>
        /// <param name="message">The message to use for the exception.</param>
        public LSLReflectionTypeMappingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="LSLReflectionTypeMappingException"/>
        /// </summary>
        /// <param name="message">The message to use for the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public LSLReflectionTypeMappingException(string message, Exception innerException) : base(message, innerException)
        {
        }




        /// <summary>
        /// Initializes a new instance of the <see cref="LSLReflectionTypeMappingException"/> class.
        /// <param name="missingType">The type that was missing a mapping to convert it into an <see cref="LSLType"/></param>
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