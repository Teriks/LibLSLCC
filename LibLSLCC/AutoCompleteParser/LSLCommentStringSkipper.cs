#region FileInfo

// 
// File: LSLCommentStringSkipper.cs
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

namespace LibLSLCC.AutoCompleteParser
{
    public class LSLCommentStringSkipper
    {
        private int _lastStringStart;

        public LSLCommentStringSkipper()
        {
        }

        public LSLCommentStringSkipper(string text, int parseUpTo)
        {
            ParseUpTo(text, parseUpTo);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public bool InBlockComment { get; private set; }
        // ReSharper disable once MemberCanBePrivate.Global
        public bool InLineComment { get; set; }
        public bool InString { get; private set; }

        public bool InComment
        {
            get { return InLineComment || InBlockComment; }
        }

        public void Reset()
        {
            InBlockComment = false;
            InLineComment = false;
            InString = false;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void ParseUpTo(string text, int offset)
        {
            for (var i = 0; i < offset; i++)
            {
                FeedChar(text, i, offset);
            }
        }

        public void FeedChar(string text, int i, int offset)
        {
            var lookAhead = i + 1;
            var lookBehind = i - 1;
            var lookBehindTwo = i - 2;
            var offsetMOne = offset - 1;

            if (!InLineComment && !InString)
            {
                if (text[i] == '/' && i < offsetMOne && text[lookAhead] == '*')
                {
                    InBlockComment = true;
                }
                else if (InBlockComment && (lookBehindTwo > 0) && text[lookBehindTwo] == '*' && text[lookBehind] == '/')
                {
                    InBlockComment = false;
                }
            }
            if (!InBlockComment && !InString)
            {
                if (text[i] == '/' && i < offsetMOne && text[lookAhead] == '/')
                {
                    InLineComment = true;
                }
                else if (InLineComment && lookBehind > 0 && text[lookBehind] == '\n')
                {
                    InLineComment = false;
                }
            }
            if (!InLineComment && !InBlockComment)
            {
                if (!InString && text[i] == '"')
                {
                    _lastStringStart = i;
                    InString = true;
                }
                else if (InString && lookBehind > 0 && text[lookBehind] == '"' && lookBehind != _lastStringStart)
                {
                    if ((lookBehind - _lastStringStart) > 2)
                    {
                        var c = 0;
                        var s = i - 2;

                        for (var o = s; text[o] == '\\'; o--, c++)
                        {
                        }

                        if ((c%2) == 0)
                        {
                            InString = false;
                        }
                    }
                    else
                    {
                        InString = false;
                    }
                }
            }
        }
    }
}