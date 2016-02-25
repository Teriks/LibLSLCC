#region FileInfo

// 
// File: LSLSourceCodeRange.cs
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

using System;
using Antlr4.Runtime;
using LibLSLCC.CodeValidator.Nodes.Interfaces;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{
    /// <summary>
    ///     Represents a range in LSL source code.
    /// </summary>
    public class LSLSourceCodeRange
    {
        /// <summary>
        ///     Construct an empty source code range.
        /// </summary>
        public LSLSourceCodeRange()
        {
            LineStart = 0;
            ColumnStart = 0;
            LineEnd = 0;
            ColumnEnd = 0;
            StartIndex = 0;
            StopIndex = 0;
            IsEmpty = true;
        }


        /// <summary>
        ///     Internal constructor for creating an <see cref="LSLSourceCodeRange" /> from an ANTLR IToken.
        /// </summary>
        /// <param name="ctx">The ANTLR IToken</param>
        /// <exception cref="ArgumentNullException"><paramref name="ctx" /> is <c>null</c>.</exception>
        internal LSLSourceCodeRange(IToken ctx)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("ctx");
            }

            LineStart = ctx.Line;
            ColumnStart = ctx.Column;
            StartIndex = ctx.StartIndex;
            StopIndex = ctx.StopIndex;

            LineEnd = ctx.Line;
            ColumnEnd = ctx.Column + ctx.Text.Length;
        }


        /// <summary>
        ///     Internal constructor for creating an <see cref="LSLSourceCodeRange" /> that encompasses two ANTLR ITokens.
        /// </summary>
        /// <param name="start">The first ANTLR IToken</param>
        /// <param name="end">The second ANTLR IToken</param>
        /// <exception cref="ArgumentNullException"><paramref name="start" /> or <paramref name="end" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If start and end overlap.</exception>
        internal LSLSourceCodeRange(IToken start, IToken end)
        {
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }

            if (end == null)
            {
                throw new ArgumentNullException("end");
            }

            if (start.StartIndex < end.StopIndex && end.StartIndex < start.StopIndex)
            {
                throw new ArgumentException(string.Format("{0}({1} start, {1} end): start and end ranges overlap.",
                    typeof (LSLSourceCodeRange).Name, typeof (IToken).Name));
            }

            LineStart = start.Line;
            ColumnStart = start.Column;
            StartIndex = start.StartIndex;
            StopIndex = end.StopIndex;

            ColumnEnd = end.Column + end.Text.Length;
            LineEnd = end.Line;
        }


        /// <summary>
        ///     Creates a source code range that spans two <see cref="LSLSourceCodeRange" /> objects.
        /// </summary>
        /// <param name="start">The <see cref="ILSLReadOnlySyntaxTreeNode" /> where the source code range starts.</param>
        /// <param name="end">The <see cref="ILSLReadOnlySyntaxTreeNode" /> where the source code range ends.</param>
        /// <exception cref="ArgumentNullException"><paramref name="start" /> or <paramref name="end" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If start and end overlap.</exception>
        public LSLSourceCodeRange(LSLSourceCodeRange start, LSLSourceCodeRange end)
        {
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }

            if (end == null)
            {
                throw new ArgumentNullException("end");
            }

            if (start.StartIndex < end.StopIndex && end.StartIndex < start.StopIndex)
            {
                throw new ArgumentException(string.Format("{0}({0} start, {0} end): start and end ranges overlap.",
                    typeof (LSLSourceCodeRange).Name));
            }

            LineStart = start.LineStart;
            ColumnStart = start.LineEnd;

            StartIndex = start.StartIndex;

            StopIndex = end.StopIndex;

            ColumnEnd = end.ColumnEnd;
            LineEnd = end.LineEnd;
        }


        /// <summary>
        ///     Internal function for creating an <see cref="LSLSourceCodeRange" /> from an ANTLR ParserRuleContext.
        /// </summary>
        /// <param name="ctx">The ANTLR ParserRuleContext</param>
        /// <exception cref="ArgumentNullException"><paramref name="ctx" /> is <c>null</c>.</exception>
        internal LSLSourceCodeRange(ParserRuleContext ctx)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("ctx");
            }

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
        }


        /// <summary>
        ///     Creates a source code range from an <see cref="ILSLReadOnlySyntaxTreeNode" />
        /// </summary>
        /// <param name="node">The syntax tree node to create the <see cref="LSLSourceCodeRange" /> from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="node" /> is <c>null</c>.</exception>
        public LSLSourceCodeRange(ILSLReadOnlySyntaxTreeNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            LineStart = node.SourceRange.LineStart;
            ColumnStart = node.SourceRange.ColumnStart;
            StartIndex = node.SourceRange.StartIndex;
            StopIndex = node.SourceRange.StopIndex;
            LineEnd = node.SourceRange.LineEnd;
            ColumnEnd = node.SourceRange.ColumnEnd;
        }


        /// <summary>
        ///     Creates a source code range that spans two <see cref="ILSLReadOnlySyntaxTreeNode" /> objects.
        /// </summary>
        /// <param name="start">The <see cref="ILSLReadOnlySyntaxTreeNode" />where the source code range starts.</param>
        /// <param name="end">The <see cref="ILSLReadOnlySyntaxTreeNode" />where the source code range ends.</param>
        /// <exception cref="ArgumentNullException"><paramref name="start" /> or <paramref name="end" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        ///     If <paramref name="start" />.SourceRangesAvailable or <paramref name="end" />
        ///     .SourceRangesAvailable are false.
        /// </exception>
        public LSLSourceCodeRange(ILSLReadOnlySyntaxTreeNode start, ILSLReadOnlySyntaxTreeNode end)
        {
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }

            if (end == null)
            {
                throw new ArgumentNullException("end");
            }

            if (!start.SourceRangesAvailable)
            {
                throw new ArgumentException("start.SourceRangesAvailable == false", "start");
            }

            if (!end.SourceRangesAvailable)
            {
                throw new ArgumentException("end.SourceRangesAvailable == false", "end");
            }

            if (start.SourceRange.StartIndex < end.SourceRange.StopIndex &&
                end.SourceRange.StartIndex < start.SourceRange.StopIndex)
            {
                throw new ArgumentException(string.Format("{0}({1} start, {1} end): start and end ranges overlap.",
                    typeof (LSLSourceCodeRange).Name, typeof (ILSLReadOnlySyntaxTreeNode).Name));
            }

            LineStart = start.SourceRange.LineStart;
            ColumnStart = start.SourceRange.ColumnStart;
            StartIndex = start.SourceRange.StartIndex;
            StopIndex = end.SourceRange.StopIndex;
            LineEnd = end.SourceRange.LineEnd;
            ColumnEnd = end.SourceRange.ColumnEnd;
        }


        /// <summary>
        ///     Manually create a source code range by providing all of the source code range details.
        /// </summary>
        /// <param name="lineStart">Line where the range starts.</param>
        /// <param name="columnStart">Character column where the range starts.</param>
        /// <param name="lineEnd">Line where the range ends.</param>
        /// <param name="columnEnd">Character column where the range ends.</param>
        /// <param name="startIndex">The index in the source code where range starts.</param>
        /// <param name="stopIndex">The index in the source code where the range ends.</param>
        /// <exception cref="ArgumentException">
        ///     if <paramref name="lineStart" /> is greater than <paramref name="lineEnd" /> or
        ///     <paramref name="startIndex" /> is greater than <paramref name="stopIndex" />.
        /// </exception>
        public LSLSourceCodeRange(int lineStart, int columnStart, int lineEnd, int columnEnd, int startIndex,
            int stopIndex)
        {
            if (lineStart > lineEnd)
            {
                throw new ArgumentException("lineStart > lineEnd", "lineStart");
            }

            if (startIndex > stopIndex)
            {
                throw new ArgumentException("startIndex > stopIndex", "startIndex");
            }

            LineStart = lineStart;
            ColumnStart = columnStart;
            LineEnd = lineEnd;
            ColumnEnd = columnEnd;
            StartIndex = startIndex;
            StopIndex = stopIndex;
        }


        /// <summary>
        ///     <c>true</c> <see cref="IsEmpty" /> is <c>false</c> and <see cref="LineEnd" /> == <see cref="LineStart" />.
        /// </summary>
        public bool IsSingleLine
        {
            get { return !IsEmpty && LineEnd == LineStart; }
        }

        /// <summary>
        ///     <c>true</c> if this <see cref="LSLSourceCodeRange" /> was constructed without parameters and represents an empty
        ///     source range.
        /// </summary>
        public bool IsEmpty { get; private set; }

        /// <summary>
        ///     Line where the source code range starts.
        /// </summary>
        public int LineStart { get; private set; }

        /// <summary>
        ///     Line where the source code range ends.
        /// </summary>
        public int LineEnd { get; private set; }

        /// <summary>
        ///     Character column where the source code range starts.
        /// </summary>
        public int ColumnStart { get; private set; }

        /// <summary>
        ///     Character column where the source code range ends.
        /// </summary>
        public int ColumnEnd { get; private set; }

        /// <summary>
        ///     Character index in the source code where the source code range starts.
        ///     This value is only relevant if HasIndexInfo is set to true.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        ///     Character index in the source code where the source code range ends.
        ///     This value is only relevant if HasIndexInfo is set to true.
        /// </summary>
        public int StopIndex { get; private set; }

        /// <summary>
        ///     The length of the source code range in characters.  Calculated as ((StopIndex - StartIndex) + 1).
        ///     This value is only relevant if HasIndexInfo is set to true.
        /// </summary>
        public int Length
        {
            get { return (StopIndex - StartIndex) + 1; }
        }

        /// <summary>
        ///     Get a source code range for the first character in this <see cref="LSLSourceCodeRange" />.
        ///     The return value is only relevant if HasIndexInfo is set to true.
        /// </summary>
        /// <value>The source code range of the first character in this <see cref="LSLSourceCodeRange" />.</value>
        public LSLSourceCodeRange FirstCharRange
        {
            get
            {
                return new LSLSourceCodeRange(LineStart, ColumnStart, LineStart, ColumnStart, StartIndex, StartIndex);
            }
        }

        /// <summary>
        ///     Get a source code range for the last character in this <see cref="LSLSourceCodeRange" />.
        ///     The return value is only relevant if HasIndexInfo is set to true.
        /// </summary>
        /// <value>The source code range of the last character in this <see cref="LSLSourceCodeRange" />.</value>
        public LSLSourceCodeRange LastCharRange
        {
            get { return new LSLSourceCodeRange(LineEnd, ColumnEnd, LineEnd, ColumnEnd, StopIndex, StopIndex); }
        }


        /// <summary>
        ///     Gets a string representation of the source code range.
        ///     In the format: "(LineStart: 0, LineEnd: 0, ColumnStart: 0, ColumnEnd: 0, StartIndex: 0, StopIndex: 0)"
        /// </summary>
        /// <returns>The string representation of the source code range.</returns>
        public override string ToString()
        {
            return string.Format(
                "(LineStart: {0}, LineEnd: {1}, ColumnStart: {2}, ColumnEnd: {3}, StartIndex: {4}, StopIndex: {5})",
                LineStart, LineEnd, ColumnStart, ColumnEnd, StartIndex, StopIndex);
        }


        /// <summary>
        ///     Determines if this source code range object is exactly equal to another.  (All properties are equal)
        ///     If 'obj' is not an <see cref="LSLSourceCodeRange" /> object this function always returns false.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var x = obj as LSLSourceCodeRange;
            if (x == null) return false;

            return ColumnStart == x.ColumnStart &&
                   ColumnEnd == x.ColumnEnd && LineStart == x.LineStart && StartIndex == x.StartIndex;
        }


        /// <summary>
        ///     Derives a hash code for this <see cref="LSLSourceCodeRange" /> by using the HasIndexInfo, ColumnStart, LineStart
        ///     and StartIndex properties.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ((ColumnStart*251) + LineStart)*251 + StartIndex;
        }
    }
}