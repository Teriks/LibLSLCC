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

namespace FindReplace
{
    /// <summary>
    ///     Adapter for Avalonedit TextEditor
    /// </summary>
    public class TextEditorAdapter : IEditor
    {
        private readonly TextEditor te;

        public TextEditorAdapter(TextEditor editor)
        {
            te = editor;
        }

        public string Text
        {
            get { return te.Text; }
        }

        public int SelectionStart
        {
            get { return te.SelectionStart; }
        }

        public int SelectionLength
        {
            get { return te.SelectionLength; }
        }

        public void BeginChange()
        {
            te.BeginChange();
        }

        public void EndChange()
        {
            te.EndChange();
        }

        public void Select(int start, int length)
        {
            te.Select(start, length);
            var loc = te.Document.GetLocation(start);
            te.ScrollTo(loc.Line, loc.Column);
        }

        public void Replace(int start, int length, string ReplaceWith)
        {
            te.Document.Replace(start, length, ReplaceWith);
        }
    }

    /// <summary>
    ///     Adapter for WPF TextBox.
    ///     Note that this is semi-usable since the WPF Textbox does not have a HideSelection property.
    /// </summary>
    public class TextBoxAdapter : IEditor
    {
        private readonly TextBox te;

        public TextBoxAdapter(TextBox editor)
        {
            te = editor;
        }

        public string Text
        {
            get { return te.Text; }
        }

        public int SelectionStart
        {
            get { return te.SelectionStart; }
        }

        public int SelectionLength
        {
            get { return te.SelectionLength; }
        }

        public void BeginChange()
        {
            te.BeginChange();
        }

        public void EndChange()
        {
            te.EndChange();
        }

        public void Select(int start, int length)
        {
            te.Select(start, length);
        }

        public void Replace(int start, int length, string ReplaceWith)
        {
            te.Text = te.Text.Substring(0, start) + ReplaceWith + te.Text.Substring(start + length);
        }
    }

    /// <summary>
    ///     Adapter for WPF RichTextBox.
    ///     The WPF RichTextBox does not have a HideSelection property either.
    ///     Here the currently selected text is colored yellow, so that it can be seen.
    /// </summary>
    public class RichTextBoxAdapter : IEditor
    {
        private readonly RichTextBox rtb;
        private TextRange oldsel;

        public RichTextBoxAdapter(RichTextBox editor)
        {
            rtb = editor;
        }

        public string Text
        {
            get { return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text; }
        }

        public int SelectionStart
        {
            get { return GetPos(rtb.Document.ContentStart, rtb.Selection.Start); }
        }

        public int SelectionLength
        {
            get { return rtb.Selection.Text.Length; }
        }

        public void BeginChange()
        {
            rtb.BeginChange();
        }

        public void EndChange()
        {
            rtb.EndChange();
        }

        public void Select(int start, int length)
        {
            var tp = rtb.Document.ContentStart;
            rtb.Selection.Select(GetPoint(tp, start), GetPoint(tp, start + length));
            rtb.ScrollToVerticalOffset(rtb.Selection.Start.GetCharacterRect(LogicalDirection.Forward).Top);
            rtb.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
            oldsel = new TextRange(rtb.Selection.Start, rtb.Selection.End);
            rtb.SelectionChanged += rtb_SelectionChanged;
        }

        public void Replace(int start, int length, string ReplaceWith)
        {
            var tp = rtb.Document.ContentStart;
            var tr = new TextRange(GetPoint(tp, start), GetPoint(tp, start + length));
            tr.Text = ReplaceWith;
        }

        private void rtb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            oldsel.ApplyPropertyValue(TextElement.BackgroundProperty, null);
            rtb.SelectionChanged -= rtb_SelectionChanged;
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
        private readonly TextBoxBase tb;

        public WFTextBoxAdapter(TextBoxBase ttb)
        {
            tb = ttb;
        }

        public string Text
        {
            get { return tb.Text; }
        }

        public int SelectionStart
        {
            get { return tb.SelectionStart; }
        }

        public int SelectionLength
        {
            get { return tb.SelectionLength; }
        }

        public void BeginChange()
        {
        }

        public void EndChange()
        {
        }

        public void Select(int start, int length)
        {
            tb.Select(start, length);
            tb.ScrollToCaret();
        }

        public void Replace(int start, int length, string ReplaceWith)
        {
            tb.Text = tb.Text.Substring(0, start) + ReplaceWith + tb.Text.Substring(start + length);
        }
    }

    public class IEditorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TextEditor)
                return new TextEditorAdapter(value as TextEditor);
            if (value is TextBox)
                return new TextBoxAdapter(value as TextBox);
            if (value is RichTextBox)
                return new RichTextBoxAdapter(value as RichTextBox);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}