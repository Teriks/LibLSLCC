namespace LibLSLCC.AutoCompleteParser
{
    public class LSLCommentStringSkipper
    {
        private bool _inBlockComment;
        private bool _inLineComment;
        private bool _inString;
        private int _lastStringStart;

        // ReSharper disable once MemberCanBePrivate.Global
        public bool InBlockComment
        {
            get { return _inBlockComment; }
        }

        // ReSharper disable once MemberCanBePrivate.Global
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
            get { return InLineComment || InBlockComment; }
        }

        public LSLCommentStringSkipper()
        {
            
        }

        public LSLCommentStringSkipper(string text, int parseUpTo)
        {
            this.ParseUpTo(text,parseUpTo);
        }

        public void Reset()
        {
            _inBlockComment = false;
            _inLineComment = false;
            _inString = false;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void ParseUpTo(string text, int offset)
        {
            for (int i = 0; i < offset; i++)
            {
                this.FeedChar(text, i, offset);
            }
        }
        public void FeedChar(string text, int i, int offset)
        {
            int lookAhead = i + 1;
            int lookBehind = i - 1;
            int lookBehindTwo = i - 2;
            int offsetMOne = offset - 1;

            if (!_inLineComment && !_inString)
            {
                if (text[i] == '/' && i < offsetMOne && text[lookAhead] == '*')
                {
                    _inBlockComment = true;
                }
                else if (_inBlockComment && (lookBehindTwo > 0) && text[lookBehindTwo] == '*' && text[lookBehind] == '/')
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
                else if (_inLineComment && lookBehind > 0 && text[lookBehind] == '\n')
                {
                    _inLineComment = false;
                }
            }
            if (!_inLineComment && !_inBlockComment)
            {
                if (!_inString && text[i] == '"')
                {
                    _lastStringStart = i;
                    _inString = true;
                }
                else if (_inString && lookBehind > 0 && text[lookBehind] == '"' && lookBehind != _lastStringStart)
                {

                    if ((lookBehind - _lastStringStart) > 2)
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
                    else
                    {
                        _inString = false;
                    }
                }
            }
        }
    }
}