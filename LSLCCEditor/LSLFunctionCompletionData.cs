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


        public ImageSource Image
        {
            get { return null; }
        }

        public string Text { get { return _text.Substring(1); } }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get
            {
                var x = new TextBlock
                {
                    Text = _text,
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
