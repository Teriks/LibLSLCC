#region FileInfo

// 
// File: LSLCompletionData.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:26 PM
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using LibLSLCC.Extensions;

#endregion

namespace LSLCCEditor.LSLEditor
{
    public class LSLCompletionData : ICompletionData
    {
        private readonly string _description;
        private readonly string _label;
        private readonly string _text;

        public LSLCompletionData(string label, string text, string description, double priority)
        {
            _description = description;
            Priority = priority;
            _text = text;
            _label = label;
            TextSubStringStart = 0;
        }

        public string PrependOnInsert { get; set; }
        public string AppendOnInsert { get; set; }
        public bool ForceIndent { get; set; }
        public int IndentLevel { get; set; }
        public int TextSubStringStart { get; set; }
        public int CaretOffsetAfterInsert { get; set; }
        public bool OffsetCaretAfterInsert { get; set; }
        public bool InsertTextAtCaretAfterOffset { get; set; }
        public string CaretOffsetInsertionText { get; set; }
        public HashSet<char> IndentBreakCharacters { get; set; } = new HashSet<char>();
        public Brush ColorBrush { get; set; } = new SolidColorBrush(Color.FromRgb(50, 52, 138));

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            var prep = string.IsNullOrWhiteSpace(PrependOnInsert) ? "" : PrependOnInsert;
            var app = string.IsNullOrWhiteSpace(AppendOnInsert) ? "" : AppendOnInsert;

            var text = prep + Text + app;

            if (ForceIndent)
            {
                var indent = StringTools.CreateTabsString(IndentLevel);

                var result = "";

                var lines = text.Split('\n').ToList();

                for (var i = 0; i < lines.Count; i++)
                {
                    string line;
                    if (i == 0)
                    {
                        var j = completionSegment.EndOffset - 1;
                        var linePrefix = "";
                        var start = j;
                        var end = 0;
                        while (j > 0)
                        {
                            var c = textArea.Document.Text[j];
                            if (c == '\n' || IndentBreakCharacters.Contains(c))
                            {
                                end = j;
                                if (c != '\n')
                                {
                                    linePrefix += c + "\n";
                                }
                                else
                                {
                                    linePrefix += c;
                                }

                                break;
                            }
                            j--;
                        }


                        textArea.Document.Remove(end, (start - end) + 1);


                        line = linePrefix + indent + lines[i].Trim();
                    }
                    else
                    {
                        line = indent + lines[i].Trim();
                    }


                    if (i != lines.Count - 1)
                    {
                        line += '\n';
                    }

                    result += line;
                }

                text = result;
            }

            textArea.Document.Replace(completionSegment, text);

            if (OffsetCaretAfterInsert)
            {
                textArea.Caret.Offset = textArea.Caret.Offset + CaretOffsetAfterInsert;

                if (InsertTextAtCaretAfterOffset)
                {
                    textArea.Document.Insert(textArea.Caret.Offset, CaretOffsetInsertionText);
                }
            }
        }

        public ImageSource Image
        {
            get { return null; }
        }

        public string Text
        {
            get
            {
                if (TextSubStringStart == 0) return _text;

                return _text.Substring(TextSubStringStart);
            }
        }

        public object Content
        {
            get
            {
                var x = new TextBlock
                {
                    Text = _label,
                    TextAlignment = TextAlignment.Left,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    Foreground = ColorBrush,
                    FontWeight = FontWeights.Bold
                };
                return x;
            }
        }

        public object Description
        {
            get { return new TextBlock {Text = _description, MaxWidth = 500, TextWrapping = TextWrapping.Wrap}; }
        }

        public double Priority { get; }
    }
}