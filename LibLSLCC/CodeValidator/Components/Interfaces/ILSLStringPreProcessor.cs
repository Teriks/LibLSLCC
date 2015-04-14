using System.Collections.Generic;

namespace LibLSLCC.CodeValidator.Components.Interfaces
{
    /// <summary>
    ///     Represents a character error for the ILSLStringPreProcessor type
    /// </summary>
    public struct LSLStringCharacterError
    {
        public bool Equals(LSLStringCharacterError other)
        {
            return CausingCharacter == other.CausingCharacter && StringIndex == other.StringIndex;
        }



        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LSLStringCharacterError && Equals((LSLStringCharacterError) obj);
        }



        /// <summary>
        /// Returns the hash code for this instance.
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



        public LSLStringCharacterError(char causingCharacter, int stringIndex) : this()
        {
            StringIndex = stringIndex;
            CausingCharacter = causingCharacter;
        }

        public static bool operator ==(LSLStringCharacterError left, LSLStringCharacterError right)
        {
            return left.CausingCharacter != right.CausingCharacter && left.StringIndex != right.StringIndex;
        }

        public static bool operator !=(LSLStringCharacterError left, LSLStringCharacterError right)
        {
            return left.CausingCharacter != right.CausingCharacter && left.StringIndex != right.StringIndex;
        }

        public char CausingCharacter { get; private set; }
        public int StringIndex { get; private set; }
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