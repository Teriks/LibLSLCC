#region FileInfo
// 
// File: LSLCompletionData.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using LibLSLCC.Utility;

#endregion

namespace LSLCCEditor.LSLEditor
{
    public class LSLCompletionData : ICompletionData
    {
        private readonly string _label;
        private readonly string _text;

        public LSLCompletionData(string label, string text, double priority)
        {
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

        public bool OffsetCaretFromBegining { get; set; }
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

            bool indentBreakOccured = false;

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
                                    indentBreakOccured = true;
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

            int begining = completionSegment.Offset + IndentLevel + (indentBreakOccured ? 2 : 1);

            textArea.Document.Replace(completionSegment, text);

            if (OffsetCaretAfterInsert)
            {

                if (OffsetCaretFromBegining)
                {
                    textArea.Caret.Offset = begining + CaretOffsetAfterInsert;
                }
                else if (OffsetCaretRelativeToDocument)
                {
                    textArea.Caret.Offset = CaretOffsetAfterInsert;
                }
                else
                {
                    textArea.Caret.Offset = textArea.Caret.Offset + CaretOffsetAfterInsert;
                }

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


        public string SignatureString { get; set; }

        public string DocumentationString { get; set; }

        public object Description { get; set; }

        public double Priority { get; set; }
        public bool OffsetCaretRelativeToDocument { get; set; }
    }
}