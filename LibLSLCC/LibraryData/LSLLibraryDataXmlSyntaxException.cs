#region FileInfo

// 
// File: LSLLibraryDataXmlSyntaxException.cs
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
using System.Runtime.Serialization;
using System.Security.Permissions;

#endregion

namespace LibLSLCC.LibraryData
{
    /// <summary>
    ///     Exception thrown by <see cref="LSLLibraryDataXmlSerializer.Parse" /> and the XML reading methods of
    ///     <see cref="LSLXmlLibraryDataProvider" />
    ///     when there are syntax errors in XML library data.
    /// </summary>
    [Serializable]
    public class LSLLibraryDataXmlSyntaxException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException" /> class.
        /// </summary>
        public LSLLibraryDataXmlSyntaxException()
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LSLLibraryDataXmlSyntaxException(string message) : base(message)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public LSLLibraryDataXmlSyntaxException(string message, Exception inner) : base(message, inner)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException" /> class.
        /// </summary>
        /// <param name="lineNumber">The line number the syntax error occurred on.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public LSLLibraryDataXmlSyntaxException(int lineNumber, string message, Exception inner)
            : base("[Line #" + lineNumber + "]: " + message, inner)
        {
            LineNumber = lineNumber;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        protected LSLLibraryDataXmlSyntaxException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="LSLLibraryDataXmlSyntaxException" /> class.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="message">The message.</param>
        public LSLLibraryDataXmlSyntaxException(int lineNumber, string message)
            : base("[Line #" + lineNumber + "]: " + message)
        {
            LineNumber = lineNumber;
        }


        /// <summary>
        ///     The line number on which the exception occurred.
        /// </summary>
        public int LineNumber { get; private set; }


        /// <summary>
        ///     When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with
        ///     information about the exception.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the
        ///     exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="info" /> parameter is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("LineNumber", LineNumber.ToString());

            base.GetObjectData(info, context);
        }
    }
}