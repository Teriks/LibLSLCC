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
using Antlr4.Runtime;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    ///     An LSL parser that can help with implementing context aware auto-complete inside of code editors.
    /// </summary>
    public sealed class LSLAutoCompleteParser : LSLAutoCompleteParserBase
    {
        /// <summary>
        ///     Preforms an auto-complete parse on the specified stream of LSL source code, up to an arbitrary offset.
        /// </summary>
        /// <param name="stream">The input source code stream.</param>
        /// <param name="toOffset">To offset to parse up to (the cursor offset).</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="toOffset"/> is not greater than zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null" />.</exception>
        public override void Parse(TextReader stream, int toOffset)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (toOffset <= 0)
            {
                throw new ArgumentOutOfRangeException("toOffset", "toOffset must be greater than zero.");
            }


            var inputStream = new AntlrInputStream(stream);

            var lexer = new LSLLexer(inputStream);


            var tokenStream = new CommonTokenStream(lexer);

            var parser = new LSLParser(tokenStream);


            parser.RemoveErrorListeners();
            lexer.RemoveErrorListeners();

            var visitor = new LSLAutoCompleteVisitor(toOffset);

            ParserState = visitor;

            visitor.Parse(parser.compilationUnit());
        }
    }
}