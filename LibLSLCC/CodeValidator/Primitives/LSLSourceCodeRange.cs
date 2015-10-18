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

using Antlr4.Runtime;
using LibLSLCC.CodeValidator.ValidatorNodes;

#endregion

namespace LibLSLCC.CodeValidator.Primitives
{

    /// <summary>
    /// Represents a range in LSL source code.
    /// </summary>
    public class LSLSourceCodeRange
    {

        /// <summary>
        /// Construct an empty source code range.
        /// </summary>
        public LSLSourceCodeRange()
        {
            LineStart = 0;
            ColumnStart = 0;

            LineEnd = 0;
            ColumnEnd = 0;
        }

        /// <summary>
        /// Internal function for creating an LSLSourceCodeRange from an ANTLR IToken.
        /// </summary>
        /// <param name="ctx">The ANTLR IToken</param>
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


        /// <summary>
        /// Internal function for creating an LSLSourceCodeRange from an ANTLR ParserRuleContext.
        /// </summary>
        /// <param name="ctx">The ANTLR ParserRuleContext</param>
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


        /// <summary>
        /// Creates a source code range from an ILSLReadOnlySyntaxTreeNode
        /// </summary>
        /// <param name="node">The syntax tree node to create the LSLSourceCodeRange from.</param>
        public LSLSourceCodeRange(ILSLReadOnlySyntaxTreeNode node)
        {
            LineStart = node.SourceCodeRange.LineStart;
            ColumnStart = node.SourceCodeRange.ColumnStart;
            StartIndex = node.SourceCodeRange.StartIndex;
            StopIndex = node.SourceCodeRange.StopIndex;
            LineEnd = node.SourceCodeRange.LineEnd;
            ColumnEnd = node.SourceCodeRange.ColumnEnd;
            HasIndexInfo = true;
        }


        /// <summary>
        /// Creates a source code range that spans two ILSLReadOnlySyntaxTreeNode objects.
        /// </summary>
        /// <param name="start">The ILSLReadOnlySyntaxTreeNode where the source code range starts.</param>
        /// <param name="end">The ILSLReadOnlySyntaxTreeNode where the source code range ends.</param>
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


        /// <summary>
        /// Manually create a source code range by providing all of the source code range details.
        /// </summary>
        /// <param name="lineStart">Line where the range starts.</param>
        /// <param name="columnStart">Character column where the range starts.</param>
        /// <param name="lineEnd">Line where the range ends.</param>
        /// <param name="columnEnd">Character column where the range ends.</param>
        /// <param name="startIndex">The index in the source code where range starts.</param>
        /// <param name="stopIndex">The index in the source code where the range ends.</param>
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


        /// <summary>
        /// Manually create a source code range by providing all of the source code range details.
        /// 
        /// StartIndex is set to: 0
        /// EndIndex is set to: 0;
        /// HasIndexInfo is set to: false;
        /// </summary>
        /// <param name="lineStart">Line where the range starts.</param>
        /// <param name="columnStart">Character column where the range starts.</param>
        /// <param name="columnLength">The length of the range in characters.</param>
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

        /// <summary>
        /// Manually create a source code range by providing all of the source code range details.
        /// LineEnd is set to: lineStart;
        /// ColumnEnd is set to: columnStart;
        /// StartIndex is set to: 0
        /// EndIndex is set to: 0;
        /// HasIndexInfo is set to: false;
        /// </summary>
        /// <param name="lineStart">Line where the range starts.</param>
        /// <param name="columnStart">Character column where the range starts.</param>
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

        /// <summary>
        /// True if both StartIndex and StopIndex have been set to relevant values.
        /// </summary>
        public bool HasIndexInfo { get; private set; }


        /// <summary>
        /// True if the source code range exists on a single source code line.
        /// </summary>
        public bool IsSingleLine
        {
            get { return LineEnd == LineStart; }
        }

        /// <summary>
        /// Line where the source code range starts.
        /// </summary>
        public int LineStart { get; private set; }

        /// <summary>
        /// Line where the source code range ends.
        /// </summary>
        public int LineEnd { get; private set; }

        /// <summary>
        /// Character column where the source code range starts.
        /// </summary>
        public int ColumnStart { get; private set; }

        /// <summary>
        /// Character column where the source code range ends.
        /// </summary>
        public int ColumnEnd { get; private set; }

        /// <summary>
        /// Character index in the source code where the source code range starts.
        /// This value is only relevant if HasIndexInfo is set to true.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Character index in the source code where the source code range ends.
        /// This value is only relevant if HasIndexInfo is set to true.
        /// </summary>
        public int StopIndex { get; private set; }


        /// <summary>
        /// The length of the source code range in characters.  Calculated as ((StopIndex - StartIndex) + 1).
        /// This value is only relevant if HasIndexInfo is set to true.
        /// </summary>
        public int Length
        {
            get { return (StopIndex - StartIndex) + 1; }
        }

        /// <summary>
        /// Determines if this source code range object is exactly equal to another.  (All properties are equal)
        /// If 'obj' is not an LSLSourceCodeRange object this function always returns false.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var x = obj as LSLSourceCodeRange;
            if (x == null) return false;

            return HasIndexInfo == x.HasIndexInfo && ColumnStart == x.ColumnStart &&
                   ColumnEnd == x.ColumnEnd && LineStart == x.LineStart && StartIndex == x.StartIndex;
        }


        /// <summary>
        /// Derives a hash code for this LSLSourceCodeRange by using the HasIndexInfo, ColumnStart, LineStart and StartIndex properties.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (((HasIndexInfo.GetHashCode() * 251) + ColumnStart) * 251 + LineStart) * 251 + StartIndex;
        }


        /// <summary>
        /// Get a source code range for the first character in this LSLSourceCodeRange.
        /// The return value is only relevant if HasIndexInfo is set to true.
        /// </summary>
        /// <returns>The source code range of the first character in this LSLSourceCodeRange.</returns>
        public LSLSourceCodeRange GetFirstCharRange()
        {
            return new LSLSourceCodeRange(LineStart, ColumnStart, LineStart, ColumnStart, StartIndex, StartIndex);
        }


        /// <summary>
        /// Get a source code range for the last character in this LSLSourceCodeRange.
        /// The return value is only relevant if HasIndexInfo is set to true.
        /// </summary>
        /// <returns>The source code range of the last character in this LSLSourceCodeRange.</returns>
        public LSLSourceCodeRange GetLastCharRange()
        {
            return new LSLSourceCodeRange(LineEnd, ColumnEnd, LineEnd, ColumnEnd, StopIndex, StopIndex);
        }


        /// <summary>
        /// Extends this LSLSourceCodeRange to encompass another ILSLReadOnlySyntaxTreeNode.
        /// This operation is only meaningful if ILSLReadOnlySyntaxTreeNode exists farther along in the source
        /// code than the area that this LSLSourceCodeRange represents.
        /// </summary>
        /// <param name="statement">The ILSLReadOnlySyntaxTreeNode to extend this LSLSourceCodeRange to.</param>
        public void ExtendTo(ILSLReadOnlySyntaxTreeNode statement)
        {
            LineEnd = statement.SourceCodeRange.LineEnd;
            ColumnEnd = statement.SourceCodeRange.ColumnEnd;
            StopIndex = statement.SourceCodeRange.StopIndex;
        }
    }
}