#region FileInfo

// 
// File: ILSLStringPreProcessor.cs
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