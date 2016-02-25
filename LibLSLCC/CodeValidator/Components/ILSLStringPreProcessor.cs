#region FileInfo
// 
// File: ILSLStringPreProcessor.cs
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
using LibLSLCC.CodeValidator.Nodes;
using LibLSLCC.CodeValidator.Nodes.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    /// <summary>
    ///     Represents a character error for the <see cref="ILSLStringPreProcessor"/> type.
    /// </summary>
    public struct LSLStringCharacterError
    {
        /// <summary>
        /// Construct an <see cref="LSLStringCharacterError"/> for a given character and index in the string.
        /// </summary>
        /// <param name="causingCharacter">The character that caused the error.</param>
        /// <param name="stringIndex">The index in the string the character error was encountered at.</param>
        public LSLStringCharacterError(char causingCharacter, int stringIndex) : this()
        {
            StringIndex = stringIndex;
            CausingCharacter = causingCharacter;
        }


        /// <summary>
        /// The character that caused the error.
        /// </summary>
        public char CausingCharacter { get; private set; }

        /// <summary>
        /// The index in the string at which the error occurred.
        /// </summary>
        public int StringIndex { get; private set; }


        /// <summary>
        /// Equals checks equality by determining whether or not the <see cref="CausingCharacter"/> and <see cref="StringIndex"/> are the
        /// same in both objects.
        /// </summary>
        /// <param name="other">The other <see cref="LSLStringCharacterError"/> object to test for equality with.</param>
        /// <returns>True if <see cref="CausingCharacter"/> and <see cref="StringIndex"/> are the same in both objects.</returns>
        public bool Equals(LSLStringCharacterError other)
        {
            return CausingCharacter == other.CausingCharacter && StringIndex == other.StringIndex;
        }

        /// <summary>
        /// Equals checks equality by determining whether or not the <see cref="CausingCharacter"/> and <see cref="StringIndex"/> are the
        /// same in both objects.
        /// </summary>
        /// <param name="obj">The other <see cref="LSLStringCharacterError"/> object to test for equality with.</param>
        /// <returns>True if 'obj' is a <see cref="LSLStringCharacterError"/> object, and <see cref="CausingCharacter"/> and <see cref="StringIndex"/> are the same in both objects.  False if otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LSLStringCharacterError && Equals((LSLStringCharacterError) obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.  
        /// The hash code is derived from the <see cref="CausingCharacter"/> property and the <see cref="StringIndex"/> property.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (CausingCharacter.GetHashCode()*397) ^ StringIndex;
            }
        }


        /// <summary>
        /// The equality operator checks equality by determining whether or not the <see cref="CausingCharacter"/> and <see cref="StringIndex"/> are the
        /// same in both <see cref="LSLStringCharacterError"/> objects.
        /// </summary>
        /// <param name="left">The <see cref="LSLStringCharacterError"/> on the left of the equality operator.</param>
        /// <param name="right">The <see cref="LSLStringCharacterError"/> on the right of the equality operator.</param>
        /// <returns>True if <see cref="CausingCharacter"/> and <see cref="StringIndex"/> are the same in both <see cref="LSLStringCharacterError"/> objects, false if otherwise.</returns>
        public static bool operator ==(LSLStringCharacterError left, LSLStringCharacterError right)
        {
            return left.CausingCharacter != right.CausingCharacter && left.StringIndex != right.StringIndex;
        }

        /// <summary>
        /// The in-equality operator checks in-equality by determining whether either the <see cref="CausingCharacter"/> or
        /// <see cref="StringIndex"/> are different among two <see cref="LSLStringCharacterError"/> objects.
        /// </summary>
        /// <param name="left">The <see cref="LSLStringCharacterError"/> on the left of the in-equality operator.</param>
        /// <param name="right">The <see cref="LSLStringCharacterError"/> on the right of the in-equality operator.</param>
        /// <returns>True if <see cref="CausingCharacter"/> or <see cref="StringIndex"/> are different among the two <see cref="LSLStringCharacterError"/> objects, false otherwise.</returns>
        public static bool operator !=(LSLStringCharacterError left, LSLStringCharacterError right)
        {
            return left.CausingCharacter != right.CausingCharacter || left.StringIndex != right.StringIndex;
        }
    }


    /// <summary>
    ///     An interface use by <see cref="LSLCodeValidator"/> to pre-processes string literals encountered
    ///     in source code before the text is stored in the <see cref="LSLStringLiteralNode.PreProcessedText"/> property of the <see cref="ILSLStringLiteralNode"/> node.
    /// </summary>
    public interface ILSLStringPreProcessor
    {

        /// <summary>
        /// True if the string that was just pre-processed contains invalid escape sequences or illegal character errors.
        /// </summary>
        bool HasErrors { get;  }


        /// <summary>
        /// An enumerable of all invalid escape sequences found in the string.
        /// </summary>
        IEnumerable<LSLStringCharacterError> InvalidEscapeCodes { get;  }

        /// <summary>
        /// An enumerable of all illegal characters found in the string.
        /// </summary>
        IEnumerable<LSLStringCharacterError> IllegalCharacters { get;  }

        /// <summary>
        /// The resulting string after the input string has been pre-processed.
        /// </summary>
        string Result { get;  }

        /// <summary>
        /// Process the string and place descriptions of invalid escape codes in the <see cref="InvalidEscapeCodes"/> enumerable,
        /// Place illegal character errors in the <see cref="IllegalCharacters"/> enumerable.
        /// </summary>
        /// <param name="stringLiteral">The string literal to be processed, the string is expected to be wrapped in double quote characters still.</param>
        void ProcessString(string stringLiteral);


        /// <summary>
        /// Reset the pre-processor so it can process another string
        /// </summary>
        void Reset();
    }
}