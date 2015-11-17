#region FileInfo
// 
// File: ILSLLibraryDataProvider.cs
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

using System.Collections.Generic;
using LibLSLCC.CodeValidator;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.LibraryData
{
    /// <summary>
    /// An interface for providing data about the standard LSL library to <see cref="LSLCodeValidator"/>.
    /// </summary>
    public interface ILSLLibraryDataProvider
    {
        /// <summary>
        /// Gets a map of subset descriptions for subset's used by signatures in this data provider.
        /// </summary>
        /// <value>
        /// The subset description map.
        /// </value>
        IReadOnlyHashMap<string, LSLLibrarySubsetDescription> SubsetDescriptions { get; }


        /// <summary>
        ///     Enumerable of <see cref="LSLLibraryEventSignature"/> describing the event handlers supported according to this data provider
        /// </summary>
        IEnumerable<LSLLibraryEventSignature> LibraryEvents { get; }

        /// <summary>
        ///     Enumerable of the <see cref="LSLLibraryFunctionSignature"/> overloads defined according to this data provider
        /// </summary>
        IEnumerable<IReadOnlyGenericArray<LSLLibraryFunctionSignature>> LibraryFunctions { get; }

        /// <summary>
        ///     Enumerable of the <see cref="LibraryConstants"/> defined according to this data provider
        /// </summary>
        IEnumerable<LSLLibraryConstantSignature> LibraryConstants { get; }

        /// <summary>
        ///     Return true if an event handler with the given name exists in the default library.
        /// </summary>
        /// <param name="name">Name of the event handler.</param>
        /// <returns>True if the event handler with given name exists.</returns>
        bool EventHandlerExist(string name);

        /// <summary>
        ///     Return an <see cref="LSLLibraryEventSignature"/> object describing an event handler signature;
        ///     if the event handler with the given name exists, otherwise null.
        /// </summary>
        /// <param name="name">Name of the event handler</param>
        /// <returns>
        ///     An <see cref="LSLLibraryEventSignature"/> object describing the given event handlers signature,
        ///     or null if the event handler does not exist.
        /// </returns>
        LSLLibraryEventSignature GetEventHandlerSignature(string name);

        /// <summary>
        /// Return true if a library function with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>True if the library function with given name exists.</returns>
        bool LibraryFunctionExist(string name);


        /// <summary>
        /// Returns true if the given signature would be considered an overload to an existing function according to this library data provider.
        /// </summary>
        /// <param name="signatureToTest">The function signature to test.</param>
        /// <returns>Boolean representing if the given signature would be considered an overload to an existing function.</returns>
        bool IsConsideredOverload(LSLFunctionSignature signatureToTest);



        /// <summary>
        /// Returns true if the library data provider contains a function signature where <see cref="LSLFunctionSignature.DefinitionIsDuplicate"/> is true for the given <see cref="LSLFunctionSignature"/>
        /// </summary>
        /// <param name="signatureToTest">The signature to use as search criteria.</param>
        /// <returns>True if the library data provider contains a function signature where existingSignature.DefinitionIsDuplicate(signature) is true</returns>
        bool LibraryFunctionExist(LSLFunctionSignature signatureToTest);



        /// <summary>
        ///     Return an <see cref="LSLFunctionSignature"/> list with the overload signatures of a function with the given name.
        ///     If the function does not exist, null is returned.  If the function exist but is not overloaded only a single item will be returned.
        /// </summary>
        /// <param name="name">Name of the library function.</param>
        /// <returns>
        ///     An <see cref="LSLFunctionSignature"/> list object describing the given library functions signatures,
        ///     or null if the library function does not exist.
        /// </returns>
        IReadOnlyGenericArray<LSLLibraryFunctionSignature> GetLibraryFunctionSignatures(string name);



        /// <summary>
        /// Return a <see cref="LSLLibraryFunctionSignature"/> from this object where <see cref="LSLFunctionSignature.SignatureEquivalent"/> is true for the given <see cref="LSLFunctionSignature"/>,
        /// or null if no such <see cref="LSLLibraryFunctionSignature"/> exists in this provider.
        /// </summary>
        /// <remarks>
        /// see: <see cref="LSLFunctionSignature.SignatureEquivalent"/>
        /// </remarks>
        /// <param name="signatureToTest">The signature to use as search criteria.</param>
        /// <returns>
        /// An <see cref="LSLFunctionSignature"/> which has the same signature of signatureToTest, or null if none exist.
        /// </returns>
        LSLLibraryFunctionSignature GetLibraryFunctionSignature(LSLFunctionSignature signatureToTest);


        /// <summary>
        ///     Return true if a library constant with the given name exists.
        /// </summary>
        /// <param name="name">Name of the library constant.</param>
        /// <returns>True if a library constant with the given name exists.</returns>
        bool LibraryConstantExist(string name);

        /// <summary>
        ///     Return an <see cref="LSLLibraryConstantSignature"/> object describing the signature of a library constant
        /// </summary>
        /// <param name="name">Name of the library constant</param>
        /// <returns>
        ///     An <see cref="LSLLibraryConstantSignature"/> object describing the given constants signature,
        ///     or null if the constant is not defined
        /// </returns>
        LSLLibraryConstantSignature GetLibraryConstantSignature(string name);
    }
}