#region FileInfo
// 
// 
// File: ILSLStringPreProcessor.cs
// 
// Last Compile: 25/09/2015 @ 11:47 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
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

#endregion

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    /// <summary>
    ///     Represents a character error for the ILSLStringPreProcessor type
    /// </summary>
    public struct LSLStringCharacterError
    {
        public LSLStringCharacterError(char causingCharacter, int stringIndex) : this()
        {
            StringIndex = stringIndex;
            CausingCharacter = causingCharacter;
        }

        public char CausingCharacter { get; }
        public int StringIndex { get; }

        public bool Equals(LSLStringCharacterError other)
        {
            return CausingCharacter == other.CausingCharacter && StringIndex == other.StringIndex;
        }

        /// <summary>
        ///     Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        ///     true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LSLStringCharacterError && Equals((LSLStringCharacterError) obj);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (CausingCharacter.GetHashCode()*397) ^ StringIndex;
            }
        }

        public static bool operator ==(LSLStringCharacterError left, LSLStringCharacterError right)
        {
            return left.CausingCharacter != right.CausingCharacter && left.StringIndex != right.StringIndex;
        }

        public static bool operator !=(LSLStringCharacterError left, LSLStringCharacterError right)
        {
            return left.CausingCharacter != right.CausingCharacter && left.StringIndex != right.StringIndex;
        }
    }


    /// <summary>
    ///     An interface for a strategy in LSLCodeValidator that pre-processes string literals encountered
    ///     in source code before the text is stored in the PreProccessedText property of the LSLStringLiteral
    ///     validator node
    /// </summary>
    public interface ILSLStringPreProcessor
    {
        bool HasErrors { get; }
        IEnumerable<LSLStringCharacterError> InvalidEscapeCodes { get; }
        IEnumerable<LSLStringCharacterError> IllegalCharacters { get; }
        string Result { get; }

        /// <summary>
        ///     Process the string and place descriptions of invalid escape codes in the InvalidEscapeCodes enumerable,
        ///     Place illegal character errors in the IllegalCharacters enumerable
        /// </summary>
        /// <param name="stringLiteral">The string literal to be processed, with quotes still at the ends</param>
        void ProcessString(string stringLiteral);

        /// <summary>
        ///     Reset the pre processor so it can process another string
        /// </summary>
        void Reset();
    }
}