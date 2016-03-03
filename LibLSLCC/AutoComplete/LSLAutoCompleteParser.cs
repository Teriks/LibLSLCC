#region FileInfo

// 
// File: LSLAutoCompleteParser.cs
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
using System.IO;
using System.Runtime.Remoting;
using Antlr4.Runtime;
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.AutoComplete
{
    internal class LimitedTextReader : TextReader
    {
        private readonly TextReader _baseReader;
        private readonly int _maxOffset;
        private int _position;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseReader"></param>
        /// <param name="maxOffset"></param>
        public LimitedTextReader(TextReader baseReader, int maxOffset)
        {
            _baseReader = baseReader;
            _maxOffset = maxOffset;
        }


        /// <summary>
        /// Reads the next character from the input stream and advances the character position by one character.
        /// </summary>
        /// <returns>
        /// The next character from the input stream, or -1 if no more characters are available. The default implementation returns -1.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextReader"/> is closed. </exception><exception cref="T:System.IO.IOException">An I/O error occurs. </exception><filterpriority>1</filterpriority>
        public override int Read()
        {


            if (_position > _maxOffset) return -1;

            

            _position++;
            return _baseReader.Read();
        }


        /// <summary>
        /// Reads the next character without changing the state of the reader or the character source. Returns the next available character without actually reading it from the input stream.
        /// </summary>
        /// <returns>
        /// An integer representing the next character to be read, or -1 if no more characters are available or the stream does not support seeking.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextReader"/> is closed. </exception><exception cref="T:System.IO.IOException">An I/O error occurs. </exception><filterpriority>1</filterpriority>
        public override int Peek()
        {
            if (_position > _maxOffset) return -1;

            return _baseReader.Peek();
        }
    }


    /// <summary>
    ///     An LSL parser that can help with implementing context aware auto-complete inside of code editors.
    /// </summary>
    public sealed class LSLAutoCompleteParser : LSLAutoCompleteParserBase
    {
        /// <summary>
        ///     Preforms an auto-complete parse on the specified stream of LSL source code, up to an arbitrary offset.
        /// </summary>
        /// <param name="code">The input source code.</param>
        /// <param name="toOffset">To offset to parse up to (the cursor offset).</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="toOffset" /> is not greater than zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="code" /> is <c>null</c>.</exception>
        public override void Parse(string code, int toOffset)
        {
            if (code == null)
            {
                throw new ArgumentNullException("code");
            }

            if (toOffset < 0)
            {
                throw new ArgumentOutOfRangeException("toOffset", "toOffset cannot be less than zero.");
            }

            var detectInStringOrComment = new LSLCommentAndStringDetector(code, toOffset);

            var visitor = new LSLAutoCompleteVisitor(toOffset);

            ParserState = visitor;

            if (detectInStringOrComment.InComment || detectInStringOrComment.InString)
            {
                visitor.InString = detectInStringOrComment.InString;
                visitor.InComment = detectInStringOrComment.InComment;
                visitor.InLineComment = detectInStringOrComment.InLineComment;
                visitor.InBlockComment = detectInStringOrComment.InBlockComment;
                return;
            }


            var inputStream = new AntlrInputStream(new StringReader(code), toOffset);

            var lexer = new LSLLexer(inputStream);

            var tokenStream = new CommonTokenStream(lexer);

            var parser = new LSLParser(tokenStream);


            parser.RemoveErrorListeners();
            lexer.RemoveErrorListeners();

            visitor.Parse(parser.compilationUnit());
        }
    }
}