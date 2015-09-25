#region FileInfo
// 
// 
// File: LSLSourceCodeRange.cs
// 
// Last Compile: 25/09/2015 @ 5:46 AM
// 
// Creation Date: 21/08/2015 @ 12:22 AM
// 
// ============================================================
// ============================================================
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// Copyright (c) 2015, Teriks
// All rights reserved.
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

using Antlr4.Runtime;
using LibLSLCC.CodeValidator.ValidatorNodes;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLSourceCodeRange
    {
        public LSLSourceCodeRange()
        {
            LineStart = 0;
            ColumnStart = 0;

            LineEnd = 0;
            ColumnEnd = 0;
        }

        internal LSLSourceCodeRange(IToken ctx)
        {
            LineStart = ctx.Line;
            ColumnStart = ctx.Column;
            StartIndex = ctx.StartIndex;
            StopIndex = ctx.StopIndex;

            LineEnd = ctx.Line;
            ColumnEnd = ctx.Column + ctx.Text.Length;
            HasIndexInfo = true;
        }

        internal LSLSourceCodeRange(ParserRuleContext ctx)
        {
            LineStart = ctx.Start.Line;
            ColumnStart = ctx.Start.Column;
            StartIndex = ctx.Start.StartIndex;

            if (ctx.Stop != null)
            {
                StopIndex = ctx.Stop.StopIndex;
                LineEnd = ctx.Stop.Line;
                ColumnEnd = ctx.Stop.Column;
            }
            else
            {
                StopIndex = StartIndex;
                ColumnEnd = ColumnStart;
                LineEnd = LineStart;
            }
            HasIndexInfo = true;
        }

        public LSLSourceCodeRange(ILSLReadOnlySyntaxTreeNode start)
        {
            LineStart = start.SourceCodeRange.LineStart;
            ColumnStart = start.SourceCodeRange.ColumnStart;
            StartIndex = start.SourceCodeRange.StartIndex;
            StopIndex = start.SourceCodeRange.StopIndex;
            LineEnd = start.SourceCodeRange.LineEnd;
            ColumnEnd = start.SourceCodeRange.ColumnEnd;
            HasIndexInfo = true;
        }

        public LSLSourceCodeRange(ILSLReadOnlySyntaxTreeNode start, ILSLReadOnlySyntaxTreeNode end)
        {
            LineStart = start.SourceCodeRange.LineStart;
            ColumnStart = start.SourceCodeRange.ColumnStart;
            StartIndex = start.SourceCodeRange.StartIndex;
            StopIndex = end.SourceCodeRange.StopIndex;
            LineEnd = end.SourceCodeRange.LineEnd;
            ColumnEnd = end.SourceCodeRange.ColumnEnd;
            HasIndexInfo = true;
        }

        public LSLSourceCodeRange(int lineStart, int columnStart, int lineEnd, int columnEnd, int startIndex,
            int stopIndex)
        {
            LineStart = lineStart;
            ColumnStart = columnStart;
            LineEnd = lineEnd;
            ColumnEnd = columnEnd;
            StartIndex = startIndex;
            StopIndex = stopIndex;
            HasIndexInfo = true;
        }

        public LSLSourceCodeRange(int lineStart, int columnStart, int columnLength)
        {
            LineStart = lineStart;
            ColumnStart = columnStart;
            LineEnd = lineStart;
            ColumnEnd = columnStart + columnLength;
            StartIndex = 0;
            StopIndex = 0;
            HasIndexInfo = false;
        }

        public LSLSourceCodeRange(int lineStart, int columnStart)
        {
            LineStart = lineStart;
            ColumnStart = columnStart;
            LineEnd = lineStart;
            ColumnEnd = columnStart;
            StartIndex = 0;
            StopIndex = 0;
            HasIndexInfo = false;
        }

        public bool HasIndexInfo { get; }

        public bool IsSingleLine
        {
            get { return LineEnd == LineStart; }
        }

        public int LineStart { get; }
        public int LineEnd { get; private set; }
        public int ColumnStart { get; }
        public int ColumnEnd { get; private set; }
        public int StartIndex { get; }
        public int StopIndex { get; private set; }

        public int Length
        {
            get { return (StopIndex - StartIndex) + 1; }
        }

        public override bool Equals(object obj)
        {
            var x = obj as LSLSourceCodeRange;
            if (x == null) return false;

            return HasIndexInfo == x.HasIndexInfo && ColumnStart == x.ColumnStart &&
                   ColumnEnd == x.ColumnEnd && LineStart == x.LineStart && StartIndex == x.StartIndex;
        }

        public LSLSourceCodeRange GetFirstCharRange()
        {
            return new LSLSourceCodeRange(LineStart, ColumnStart, LineStart, ColumnStart, StartIndex, StartIndex);
        }

        public LSLSourceCodeRange GetLastCharRange()
        {
            return new LSLSourceCodeRange(LineEnd, ColumnEnd, LineEnd, ColumnEnd, StopIndex, StopIndex);
        }

        public void ExtendTo(ILSLReadOnlySyntaxTreeNode statement)
        {
            LineEnd = statement.SourceCodeRange.LineEnd;
            ColumnEnd = statement.SourceCodeRange.ColumnEnd;
            StopIndex = statement.SourceCodeRange.StopIndex;
        }
    }
}