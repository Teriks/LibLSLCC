#region FileInfo
// 
// File: Adapters.cs
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
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using RichTextBox = System.Windows.Controls.RichTextBox;
using TextBox = System.Windows.Controls.TextBox;

#endregion

namespace LSLCCEditor.FindReplace
{
    /// <summary>
    ///     Adapter for Avalonedit TextEditor
    /// </summary>
    public class TextEditorAdapter : IEditor
    {
        private readonly TextEditor _te;

        public TextEditorAdapter(TextEditor editor)
        {
            _te = editor;
        }

        public string Text
        {
            get { return _te.Text; }
        }

        public int SelectionStart
        {
            get { return _te.SelectionStart; }
        }

        public int SelectionLength
        {
            get { return _te.SelectionLength; }
        }

        public void BeginChange()
        {
            _te.BeginChange();
        }

        public void EndChange()
        {
            _te.EndChange();
        }

        public void Select(int start, int length)
        {
            _te.Select(start, length);
            var loc = _te.Document.GetLocation(start);
            _te.ScrollTo(loc.Line, loc.Column);
        }

        public void Replace(int start, int length, string replaceWith)
        {
            _te.Document.Replace(start, length, replaceWith);
        }
    }

    /// <summary>
    ///     Adapter for WPF TextBox.
    ///     Note that this is semi-usable since the WPF Textbox does not have a HideSelection property.
    /// </summary>
    public class TextBoxAdapter : IEditor
    {
        private readonly TextBox _te;

        public TextBoxAdapter(TextBox editor)
        {
            _te = editor;
        }

        public string Text
        {
            get { return _te.Text; }
        }

        public int SelectionStart
        {
            get { return _te.SelectionStart; }
        }

        public int SelectionLength
        {
            get { return _te.SelectionLength; }
        }

        public void BeginChange()
        {
            _te.BeginChange();
        }

        public void EndChange()
        {
            _te.EndChange();
        }

        public void Select(int start, int length)
        {
            _te.Select(start, length);
        }

        public void Replace(int start, int length, string replaceWith)
        {
            _te.Text = _te.Text.Substring(0, start) + replaceWith + _te.Text.Substring(start + length);
        }
    }

    /// <summary>
    ///     Adapter for WPF RichTextBox.
    ///     The WPF RichTextBox does not have a HideSelection property either.
    ///     Here the currently selected text is colored yellow, so that it can be seen.
    /// </summary>
    public class RichTextBoxAdapter : IEditor
    {
        private readonly RichTextBox _rtb;
        private TextRange _oldsel;

        public RichTextBoxAdapter(RichTextBox editor)
        {
            _rtb = editor;
        }

        public string Text
        {
            get { return new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd).Text; }
        }

        public int SelectionStart
        {
            get { return GetPos(_rtb.Document.ContentStart, _rtb.Selection.Start); }
        }

        public int SelectionLength
        {
            get { return _rtb.Selection.Text.Length; }
        }

        public void BeginChange()
        {
            _rtb.BeginChange();
        }

        public void EndChange()
        {
            _rtb.EndChange();
        }

        public void Select(int start, int length)
        {
            var tp = _rtb.Document.ContentStart;
            _rtb.Selection.Select(GetPoint(tp, start), GetPoint(tp, start + length));
            _rtb.ScrollToVerticalOffset(_rtb.Selection.Start.GetCharacterRect(LogicalDirection.Forward).Top);
            _rtb.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
            _oldsel = new TextRange(_rtb.Selection.Start, _rtb.Selection.End);
            _rtb.SelectionChanged += rtb_SelectionChanged;
        }

        public void Replace(int start, int length, string replaceWith)
        {
            var tp = _rtb.Document.ContentStart;
            var tr = new TextRange(GetPoint(tp, start), GetPoint(tp, start + length));
            tr.Text = replaceWith;
        }

        private void rtb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _oldsel.ApplyPropertyValue(TextElement.BackgroundProperty, null);
            _rtb.SelectionChanged -= rtb_SelectionChanged;
        }

        /*private static TextPointer GetPointOld(TextPointer start, int x)
       {
           var ret = start;
           var i = 0;
           while (i < x && ret != null)
           {
               if (ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text || ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                   i++;              
               if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
                   return ret;
               ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
           }
           return ret;
       }*/


        private static TextPointer GetPoint(TextPointer start, int x)
        {
            var ret = start.GetPositionAtOffset(x);
            while (new TextRange(start, ret).Text.Length < x)
            {
                if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
                    return ret;
                ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
            }

            return ret;
        }

        private static int GetPos(TextPointer start, TextPointer p)
        {
            return (new TextRange(start, p)).Text.Length;
        }
    }

    internal class WFTextBoxAdapter : IEditor
    {
        private readonly TextBoxBase _tb;

        public WFTextBoxAdapter(TextBoxBase ttb)
        {
            _tb = ttb;
        }

        public string Text
        {
            get { return _tb.Text; }
        }

        public int SelectionStart
        {
            get { return _tb.SelectionStart; }
        }

        public int SelectionLength
        {
            get { return _tb.SelectionLength; }
        }

        public void BeginChange()
        {
        }

        public void EndChange()
        {
        }

        public void Select(int start, int length)
        {
            _tb.Select(start, length);
            _tb.ScrollToCaret();
        }

        public void Replace(int start, int length, string replaceWith)
        {
            _tb.Text = _tb.Text.Substring(0, start) + replaceWith + _tb.Text.Substring(start + length);
        }
    }

    public class IEditorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var editor = value as TextEditor;
            if (editor != null)
                return new TextEditorAdapter(editor);
            var box = value as TextBox;
            if (box != null)
                return new TextBoxAdapter(box);
            var textBox = value as RichTextBox;
            if (textBox != null)
                return new RichTextBoxAdapter(textBox);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}