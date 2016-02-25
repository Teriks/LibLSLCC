#region FileInfo

// 
// File: LSLParsedEventHandlerSignature.cs
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

using System.Linq;
using LibLSLCC.CodeValidator.Nodes;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    /// <summary>
    ///     Represents and event handler signature parsed from source code.
    ///     This object derives from <see cref="LSLEventSignature " /> and adds an <see cref="LSLParameterListNode" />
    ///     property that contains a parameter list node from the syntax tree.
    /// </summary>
    public sealed class LSLParsedEventHandlerSignature : LSLEventSignature
    {
        /// <summary>
        ///     Construct an  <see cref="LSLParsedEventHandlerSignature" /> from an event handler name and a
        ///     <see cref="LSLParameterListNode" /> from
        ///     an LSL Syntax tree.
        /// </summary>
        /// <param name="name">The name of the event handler.</param>
        /// <param name="parameters">
        ///     The <see cref="LSLParameterListNode" /> from the syntax tree that represents the event
        ///     handlers parsed parameters.
        /// </param>
        public LSLParsedEventHandlerSignature(string name, LSLParameterListNode parameters) :
            base(name, parameters.Parameters.Select(x => new LSLParameter(x.Type, x.Name, false)))
        {
            ParameterListNode = parameters;
        }


        /// <summary>
        ///     A parameter list node from an LSL syntax tree that represents this event handler signatures parameters.
        /// </summary>
        public LSLParameterListNode ParameterListNode { get; private set; }
    }
}