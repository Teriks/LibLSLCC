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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using LibLSLCC.Utility;

#endregion

namespace LSLCCEditor.EditControl
{
    public class LSLCompletionData : ICompletionData
    {
        private readonly string _label;
        private readonly string _text;
        private HashSet<string> _indentBreakCharacters = new HashSet<string>();
        private Brush _colorBrush = new SolidColorBrush(Color.FromRgb(50, 52, 138));

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

        public HashSet<string> IndentBreakCharacters
        {
            get { return _indentBreakCharacters; }
            set { _indentBreakCharacters = value; }
        }

        public Brush ColorBrush
        {
            get { return _colorBrush; }
            set { _colorBrush = value; }
        }

        private class MatchString
        {
            public readonly string Value;
            public int Indx;

            public MatchString(string value)
            {
                Value = value;
                Indx = value.Length - 1;
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var o = obj as MatchString;
                return o != null && Value.Equals(o.Value);
            }

            public char c()
            {
                return Value[Indx];
            }
        }
        private static bool MatchOneOfBackwards(HashSet<string> strings, string input, int inputOffset)
        {
            var collection = strings.Select(x => new MatchString(x)).ToArray();
            HashSet<string> matches = new HashSet<string>(strings);

            while (inputOffset > 0 && matches.Count > 0)
            {
                foreach (var v in collection)
                {
                    var a = v.c();

                    var b = input[inputOffset];

                    if (a == b)
                    {
                        v.Indx--;

                        if (v.Indx <= 0)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        matches.Remove(v.Value);
                    }
                    
                }
                inputOffset -= 1;
            }

            return false;
        }

        private string IndentString(string str, string indentString, bool indentFront, int offsetIn, out int offsetOut)
        {
            offsetOut = offsetIn;
            var s = "";

            if (indentFront)
            {
                s = indentString;
            }
            int index = 0;
            foreach (var c in str)
            {
                if (c == '\n')
                {
                    s += c + indentString;
                    if (index < offsetIn)
                    {
                        offsetOut += indentString.Length;
                    }
                }
                else
                {
                    s += c;
                }
                index++;
            }

            return s;
        }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            var prep = string.IsNullOrEmpty(PrependOnInsert) ? "" : PrependOnInsert;
            var app = string.IsNullOrEmpty(AppendOnInsert) ? "" : AppendOnInsert;
            var insertionText = string.IsNullOrWhiteSpace(CaretOffsetInsertionText) ? "" : CaretOffsetInsertionText;

            //AvalonEdit apparently MODFIES segment descriptions passed to Document.Replace. wtf.exe
            //this took me a long time to debug.
            var completionOffset = completionSegment.Offset;

            var text = prep + Text + app;

            int offsetFromBegining = CaretOffsetAfterInsert;

            bool lineBroken = false;
            if (ForceIndent)
            {
                
                var indent = LSLFormatTools.CreateTabsString(IndentLevel);

                int i;
                int length = 0;
                for (i = completionSegment.Offset - 1;
                    i >= 0 &&
                    textArea.Document.Text[i] != '\n';
                    i--)
                {
                    if (MatchOneOfBackwards(IndentBreakCharacters, textArea.Document.Text, i))
                    {

                        lineBroken = true;
                    }

                    length++;
                }

                text = "\n" + IndentString(text, indent, true, offsetFromBegining, out offsetFromBegining);

                if (!lineBroken)
                {
                    textArea.Document.Replace(i, length + completionSegment.Length + 1, text);
                    completionOffset = i + IndentLevel + 1;
                }
                else
                {
                    textArea.Document.Replace(completionOffset, completionSegment.Length, text);
                    completionOffset += IndentLevel + 1;
                }
            }
            else
            {
                textArea.Document.Replace(completionOffset, completionSegment.Length, text);
            }

            if (!OffsetCaretAfterInsert) return;

            if (OffsetCaretFromBegining)
            {
                textArea.Caret.Offset = Clamp(completionOffset + offsetFromBegining, 0, textArea.Document.TextLength);
            }
            else if (OffsetCaretRelativeToDocument)
            {
                textArea.Caret.Offset = Clamp(CaretOffsetAfterInsert,0,textArea.Document.TextLength);
            }
            else
            {
                textArea.Caret.Offset = Clamp(textArea.Caret.Offset + CaretOffsetAfterInsert, 0, textArea.Document.TextLength);
            }

            if (InsertTextAtCaretAfterOffset)
            {
                textArea.Document.Insert(textArea.Caret.Offset, insertionText);
            }
        }




        private static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            if (val.CompareTo(max) > 0) return max;
            return val;
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



        public Func<object> DescriptionFactory { get; set; } 

        public object Description
        {
            get
            {
                if (DescriptionFactory == null) return null;
                return DescriptionFactory();
            }
        }

        public double Priority { get; set; }
        public bool OffsetCaretRelativeToDocument { get; set; }
    }
}