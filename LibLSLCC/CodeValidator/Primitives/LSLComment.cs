#region FileInfo

// 
// File: LSLComment.cs
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

#endregion

using System;

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     A container for LSL source code comment strings.
    /// </summary>
    public struct LSLComment : IEquatable<LSLComment>
    {
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(LSLComment other)
        {
            return _type == other._type && _sourceRange.Equals(other._sourceRange) && string.Equals(_text, other._text);
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
            return obj is LSLComment && Equals((LSLComment) obj);
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
                var hashCode = (int) _type;
                hashCode = (hashCode*397) ^ _sourceRange.GetHashCode();
                hashCode = (hashCode*397) ^ _text.GetHashCode();
                return hashCode;
            }
        }


        /// <summary>
        /// Compare comments for equality.
        /// </summary>
        /// <param name="left">The comment object on the left.</param>
        /// <param name="right">The comment object on the right.</param>
        /// <returns><c>true</c> if equal.</returns>
        public static bool operator ==(LSLComment left, LSLComment right)
        {
            return left.Equals(right);
        }


        /// <summary>
        /// Compare comments for inequality.
        /// </summary>
        /// <param name="left">The comment object on the left.</param>
        /// <param name="right">The comment object on the right.</param>
        /// <returns><c>true</c> if inequal.</returns>
        public static bool operator !=(LSLComment left, LSLComment right)
        {
            return !left.Equals(right);
        }


        private readonly string _text;
        private readonly LSLSourceCodeRange _sourceRange;
        private readonly LSLCommentType _type;


        /// <summary>
        ///     Construct a comment object from comment text, type and source code range.
        /// </summary>
        /// <param name="text">The text that up the entire comment, including the special comment start/end sequences.</param>
        /// <param name="type">The comment type.  <see cref="LSLCommentType" /></param>
        /// <param name="sourceRange">The source code range that the comment occupies.</param>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> or <paramref name="sourceRange"/> is <c>null</c>.</exception>
        public LSLComment(string text, LSLCommentType type, LSLSourceCodeRange sourceRange)
        {
            if (text == null) throw new ArgumentNullException("text");
            if (sourceRange == null) throw new ArgumentNullException("sourceRange");

            _text = text;
            _type = type;
            _sourceRange = sourceRange;
        }


        /// <summary>
        ///     The raw comment text.
        /// </summary>
        public string Text
        {
            get { return _text; }
        }

        /// <summary>
        ///     The source code range which the comment occupies.
        /// </summary>
        public LSLSourceCodeRange SourceRange
        {
            get { return _sourceRange; }
        }

        /// <summary>
        ///     The LSLCommentType type of the comment.
        /// </summary>
        public LSLCommentType Type
        {
            get { return _type; }
        }


        /// <summary>
        ///     Returns the comment text.
        /// </summary>
        /// <seealso cref="Text" />
        /// <returns>
        ///     <see cref="Text" />
        /// </returns>
        public override string ToString()
        {
            return Text;
        }
    }
}