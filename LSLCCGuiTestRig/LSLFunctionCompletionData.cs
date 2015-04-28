using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace LSLCCEditor
{
    public class LSLFunctionCompletionData : ICompletionData
    {
        private readonly string _description;
        private readonly double _priority;
        private readonly string _text;


        public LSLFunctionCompletionData(string text, string description, double priority)
        {
            _description = description;
            _priority = priority;
            _text = text;
            ColorBrush = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        }


        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public string Text { get { return _text.Substring(1); } }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get
            {
                var x= new TextBlock();
                x.Text = _text;
                x.TextAlignment=TextAlignment.Left;
                x.TextTrimming=TextTrimming.CharacterEllipsis;
                x.Foreground=ColorBrush;
                x.FontWeight = FontWeights.Bold;
                return x;
            }
        }

        public object Description
        {
            get { return _description; }
        }


        public SolidColorBrush ColorBrush
        {
            get;
            set;
        }

        public double Priority
        {
            get { return _priority; }
        }


        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text + "(");
        }
    }
}
