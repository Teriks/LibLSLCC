﻿#region FileInfo
// 
// File: LSLLibraryDataProvider.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.ThreadSafeEnumeration;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    public class LSLLibraryDataProvider : ILSLMainLibraryDataProvider
    {
        private readonly List<LSLLibraryConstantSignature> _duplicateConstantsDefined =
            new List<LSLLibraryConstantSignature>();

        private readonly List<LSLLibraryEventSignature> _duplicateEventsDefined = new List<LSLLibraryEventSignature>();

        private readonly List<LSLLibraryFunctionSignature> _duplicateFunctionsDefined =
            new List<LSLLibraryFunctionSignature>();

        private readonly Dictionary<string, LSLLibraryConstantSignature> _validConstants
            = new Dictionary<string, LSLLibraryConstantSignature>();

        private readonly Dictionary<string, LSLLibraryEventSignature> _validEventHandlers =
            new Dictionary<string, LSLLibraryEventSignature>();

        private readonly Dictionary<string, List<LSLLibraryFunctionSignature>> _validLibraryFunctions
            = new Dictionary<string, List<LSLLibraryFunctionSignature>>();

        /// <summary>
        ///     Only useful when Subsets contains the "all" keyword
        /// </summary>
        public IEnumerable<LSLLibraryEventSignature> DuplicateEventsDefined
        {
            get { return _duplicateEventsDefined; }
        }

        /// <summary>
        ///     Only useful when Subsets contains the "all" keyword
        /// </summary>
        public IEnumerable<LSLLibraryConstantSignature> DuplicateConstantsDefined
        {
            get { return _duplicateConstantsDefined; }
        }

        /// <summary>
        ///     Only useful when Subsets contains the "all" keyword
        /// </summary>
        public IEnumerable<LSLLibraryFunctionSignature> DuplicateFunctionsDefined
        {
            get { return _duplicateFunctionsDefined; }
        }

        public bool AccumulateDuplicates { get; protected set; }

        public virtual IEnumerable<LSLLibraryEventSignature> SupportedEventHandlers
        {
            get { return _validEventHandlers.Values.AsLocked(_validEventHandlers); }
        }

        public virtual IEnumerable<IReadOnlyList<LSLLibraryFunctionSignature>> LibraryFunctions
        {
            get { return _validLibraryFunctions.Values.AsLocked(_validLibraryFunctions); }
        }

        public virtual IEnumerable<LSLLibraryConstantSignature> LibraryConstants
        {
            get { return _validConstants.Values.AsLocked(_validLibraryFunctions); }
        }

        /// <summary>
        ///     Return true if an event handler with the given name exists in the default library.
        /// </summary>
        /// <param name="name">Name of the event handler.</param>
        /// <returns>True if the event handler with given name exists.</returns>
        public virtual bool EventHandlerExist(string name)
        {
            return _validEventHandlers.ContainsKey(name);
        }

        /// <summary>
        ///     Return an LSLEventHandlerSignature object describing an event handler signature;
        ///     if the event handler with the given name exists, otherwise null.
        /// </summary>
        /// <param name="name">Name of the event handler</param>
        /// <returns>
        ///     An LSLEventHandlerSignature object describing the given event handlers signature,
        ///     or null if the event handler does not exist.
        /// </returns>
        public virtual LSLLibraryEventSignature GetEventHandlerSignature(string name)
        {
            LSLLibraryEventSignature result;

            if (_validEventHandlers.TryGetValue(name, out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        ///     Return true if a library function with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>True if the library function with given name exists.</returns>
        public virtual bool LibraryFunctionExist(string name)
        {
            return _validLibraryFunctions.ContainsKey(name);
        }

        /// <summary>
        ///     Return an LSLFunctionSignature list object describing the function call signatures of a library function;
        ///     if the function with the given name exists as a singular or overloaded function, otherwise null.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>
        ///     An LSLFunctionSignature list object describing the given library functions signatures,
        ///     or null if the library function does not exist.
        /// </returns>
        public virtual IReadOnlyList<LSLLibraryFunctionSignature> GetLibraryFunctionSignatures(string name)
        {
            List<LSLLibraryFunctionSignature> result;

            if (_validLibraryFunctions.TryGetValue(name, out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        ///     Return true if a library constant with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library constant.</param>
        /// <returns>True if a library constant with the given name exists.</returns>
        public virtual bool LibraryConstantExist(string name)
        {
            return _validConstants.ContainsKey(name);
        }

        /// <summary>
        ///     Return the library constant if it exists, otherwise null.
        /// </summary>
        /// <param name="name">Name of the library constant.</param>
        /// <returns>
        ///     The library constants signature
        /// </returns>
        public virtual LSLLibraryConstantSignature GetLibraryConstantSignature(string name)
        {
            LSLLibraryConstantSignature result;

            if (_validConstants.TryGetValue(name, out result))
            {
                return result;
            }
            return null;
        }

        protected void ClearLibraryConstants()
        {
            _validConstants.Clear();
        }

        protected void ClearLibraryFunctions()
        {
            _validLibraryFunctions.Clear();
        }

        protected void ClearEventHandlers()
        {
            _validEventHandlers.Clear();
        }

        public virtual void AddValidConstant(LSLLibraryConstantSignature signature)
        {
            if (_validConstants.ContainsKey(signature.Name))
            {
                if (!AccumulateDuplicates)
                {
                    throw new LSLDuplicateSignatureException("Constant with name \"" + signature.Name +
                                                             "\" is already defined");
                }
                _duplicateConstantsDefined.Add(signature);
            }
            else
            {
                _validConstants.Add(signature.Name, signature);
            }
        }

        public virtual void AddValidEventHandler(LSLLibraryEventSignature signature)
        {
            if (_validEventHandlers.ContainsKey(signature.Name))
            {
                if (!AccumulateDuplicates)
                {
                    throw new LSLDuplicateSignatureException("Event handler with name \"" + signature.Name +
                                                             "\" is already defined");
                }
                _duplicateEventsDefined.Add(signature);
            }
            else
            {
                _validEventHandlers.Add(signature.Name, signature);
            }
        }

        public virtual void AddValidLibraryFunction(LSLLibraryFunctionSignature signature)
        {
            if (_validLibraryFunctions.ContainsKey(signature.Name))
            {
                if (_validLibraryFunctions[signature.Name].Any(y => y.SignatureMatches(signature)))
                {
                    if (!AccumulateDuplicates)
                    {
                        throw new LSLDuplicateSignatureException("Library function with name \"" + signature.Name +
                                                                 "\" is already defined");
                    }
                    _duplicateFunctionsDefined.Add(signature);
                }
                else
                {
                    _validLibraryFunctions[signature.Name].Add(signature);
                }
            }
            else
            {
                _validLibraryFunctions.Add(signature.Name, new List<LSLLibraryFunctionSignature> {signature});
            }
        }

        public virtual void AddValidConstant(LSLType type, string name)
        {
            AddValidConstant(new LSLLibraryConstantSignature(type, name));
        }

        public virtual void AddValidEventHandler(string name, IEnumerable<LSLParameter> parameters)
        {
            AddValidEventHandler(new LSLLibraryEventSignature(name, parameters));
        }

        public virtual void AddValidLibraryFunction(LSLType returnType, string name,
            IEnumerable<LSLParameter> parameters)
        {
            AddValidLibraryFunction(new LSLLibraryFunctionSignature(returnType, name, parameters));
        }
    }

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