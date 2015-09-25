#region FileInfo

// 
// File: LSLSourceCodeRange.cs
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