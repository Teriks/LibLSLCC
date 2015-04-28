namespace LibLSLCC.FastEditorParse
{
    public class LSLCommentStringSkipper
    {
        private bool _inBlockComment;
        private bool _inLineComment;
        private bool _inString;

        public bool InBlockComment
        {
            get { return _inBlockComment; }
        }

        public bool InLineComment
        {
            get { return _inLineComment; }
            set { _inLineComment = value; }
        }

        public bool InString
        {
            get { return _inString; }
        }

        public bool InComment
        {
            get { return _inLineComment || _inBlockComment; }
        }

        public void Reset()
        {
            _inBlockComment = false;
            _inLineComment = false;
            _inString = false;
        }

        public void FeedChar(string text, int i, int offset)
        {
            int lookAhead = i + 1;
            int lookBehind = i - 1;
            int offsetMOne = offset - 1;

            if (!_inLineComment && !_inString)
            {
                if (text[i] == '/' && i < offsetMOne && text[lookAhead] == '*')
                {
                    _inBlockComment = true;
                }
                else if (_inBlockComment && text[i - 2] == '*' && text[lookBehind] == '/')
                {
                    _inBlockComment = false;
                }
            }
            if (!_inBlockComment && !_inString)
            {
                if (text[i] == '/' && i < offsetMOne && text[lookAhead] == '/')
                {
                    _inLineComment = true;
                }
                else if (_inLineComment && text[lookBehind] == '\n')
                {
                    _inLineComment = false;
                }
            }
            if (!_inLineComment && !_inBlockComment)
            {
                if (!_inString && text[i] == '"')
                {
                    _inString = true;
                }
                else if (_inString && text[lookBehind] == '"')
                {
                    int c = 0;
                    int s = i - 2;

                    for (int o = s; text[o] == '\\'; o--, c++)
                    {
                    }

                    if ((c%2) == 0)
                    {
                        _inString = false;
                    }
                }
            }
        }
    }
}