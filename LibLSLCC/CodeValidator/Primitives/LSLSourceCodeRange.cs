using Antlr4.Runtime;
using LibLSLCC.CodeValidator.ValidatorNodes;

namespace LibLSLCC.CodeValidator.Primitives
{
    public class LSLSourceCodeRange
    {
        private readonly int _columnStart;
        private readonly int _lineStart;
        private int _columnEnd;
        private int _lineEnd;
        private readonly int _startIndex;
        private  int _stopIndex;


        public LSLSourceCodeRange()
        {
            _lineStart = 0;
            _columnStart = 0;

            _lineEnd = 0;
            _columnEnd = 0;
        }



        internal LSLSourceCodeRange(IToken ctx)
        {
            _lineStart = ctx.Line;
            _columnStart = ctx.Column;
            _startIndex = ctx.StartIndex;
            _stopIndex = ctx.StopIndex;

            _lineEnd = ctx.Line;
            _columnEnd = ctx.Column + ctx.Text.Length;
            HasIndexInfo = true;
        }



        internal LSLSourceCodeRange(ParserRuleContext ctx)
        {
            _lineStart = ctx.Start.Line;
            _columnStart = ctx.Start.Column;
            _startIndex = ctx.Start.StartIndex;

            if (ctx.Stop != null)
            {
                _stopIndex = ctx.Stop.StopIndex;
                _lineEnd = ctx.Stop.Line;
                _columnEnd = ctx.Stop.Column;
            }
            else
            {
                _stopIndex = _startIndex;
                _columnEnd = _columnStart;
                _lineEnd = _lineStart;
            }
            HasIndexInfo = true;
        }



        public LSLSourceCodeRange(ILSLReadOnlySyntaxTreeNode start)
        {
            _lineStart = start.SourceCodeRange.LineStart;
            _columnStart = start.SourceCodeRange.ColumnStart;
            _startIndex = start.SourceCodeRange.StartIndex;
            _stopIndex = start.SourceCodeRange.StopIndex;
            _lineEnd = start.SourceCodeRange.LineEnd;
            _columnEnd = start.SourceCodeRange.ColumnEnd;
            HasIndexInfo = true;
        }



        public LSLSourceCodeRange(ILSLReadOnlySyntaxTreeNode start, ILSLReadOnlySyntaxTreeNode end)
        {
            _lineStart = start.SourceCodeRange.LineStart;
            _columnStart = start.SourceCodeRange.ColumnStart;
            _startIndex = start.SourceCodeRange.StartIndex;
            _stopIndex = end.SourceCodeRange.StopIndex;
            _lineEnd = end.SourceCodeRange.LineEnd;
            _columnEnd = end.SourceCodeRange.ColumnEnd;
            HasIndexInfo = true;
        }



        public LSLSourceCodeRange(int lineStart, int columnStart, int lineEnd, int columnEnd, int startIndex, int stopIndex)
        {
            _lineStart = lineStart;
            _columnStart = columnStart;
            _lineEnd = lineEnd;
            _columnEnd = columnEnd;
            _startIndex = startIndex;
            _stopIndex = stopIndex;
            HasIndexInfo = true;
        }



        public LSLSourceCodeRange(int lineStart, int columnStart, int columnLength)
        {
            _lineStart = lineStart;
            _columnStart = columnStart;
            _lineEnd = lineStart;
            _columnEnd = columnStart + columnLength;
            _startIndex = 0;
            _stopIndex = 0;
            HasIndexInfo = false;
        }



        public LSLSourceCodeRange(int lineStart, int columnStart)
        {
            _lineStart = lineStart;
            _columnStart = columnStart;
            _lineEnd = lineStart;
            _columnEnd = columnStart;
            _startIndex = 0;
            _stopIndex = 0;
            HasIndexInfo = false;
        }

        public bool HasIndexInfo { get; private set; }

        public bool IsSingleLine
        {
            get { return _lineEnd == _lineStart; }
        }


        public int LineStart
        {
            get { return _lineStart; }
        }

        public int LineEnd
        {
            get { return _lineEnd; }
        }

        public int ColumnStart
        {
            get { return _columnStart; }
        }

        public int ColumnEnd
        {
            get { return _columnEnd; }
        }

        public int StartIndex
        {
            get { return _startIndex; }
        }

        public int StopIndex
        {
            get { return _stopIndex; }
        }



        public void ExtendTo(ILSLReadOnlySyntaxTreeNode statement)
        {
            _lineEnd = statement.SourceCodeRange.LineEnd;
            _columnEnd = statement.SourceCodeRange.ColumnEnd;
            _stopIndex = statement.SourceCodeRange.StopIndex;
        }
    }
}