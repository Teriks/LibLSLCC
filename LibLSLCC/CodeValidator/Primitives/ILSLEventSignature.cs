#region FileInfo
// 
// File: ILSLEventSignature.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2016, Teriks
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
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     A read only interface for <see cref="LSLEventSignature" />.
    /// </summary>
    public interface ILSLEventSignature
    {
        /// <summary>
        ///     The number of parameters the event handler signature has
        /// </summary>
        int ParameterCount { get; }

        /// <summary>
        ///     The event handlers name, must follow LSL symbol naming conventions
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Indexable list of objects describing the event handlers parameters
        /// </summary>
        IReadOnlyGenericArray<LSLParameterSignature> Parameters { get; }

        /// <summary>
        ///     Returns a formated signature string for the <see cref="ILSLEventSignature" />.  This does not include a trailing
        ///     semi-colon.
        ///     An example would be: listen(integer channel, string name, key id, string message)
        /// </summary>
        string SignatureString { get; }


        /// <summary>
        ///     Delegates to <see cref="SignatureString" />
        /// </summary>
        /// <returns>
        ///     <see cref="SignatureString" />
        /// </returns>
        string ToString();


        /// <summary>
        ///     Determines if two event handler signatures match exactly, parameter names do not matter but parameter
        ///     types do.
        /// </summary>
        /// <param name="otherSignature">The other event handler signature to compare to.</param>
        /// <returns>True if the two signatures are identical.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="otherSignature" /> is <c>null</c>.</exception>
        bool SignatureMatches(ILSLEventSignature otherSignature);


        /// <summary>
        ///     <see cref="GetHashCode" /> uses the name of the <see cref="ILSLEventSignature" /> and the LSL Types of the
        ///     parameters.
        ///     <para />
        ///     This means the Hash Code is determined by the event name, and the Types of all its parameters.
        ///     <para />
        ///     Inherently, uniqueness is also determined by the number of parameters.
        /// </summary>
        /// <returns>Hash code for this <see cref="ILSLEventSignature" /></returns>
        int GetHashCode();


        /// <summary>
        ///     <see cref="Equals(object)" /> delegates to <see cref="ILSLEventSignature.SignatureMatches" />
        /// </summary>
        /// <param name="obj">The other event signature</param>
        /// <returns>Equality</returns>
        bool Equals(object obj);
    }
}