using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace LSLCCEditor
{
    public class LSLConstantCompletionData : ICompletionData
    {
        private readonly string _description;
        private readonly double _priority;
        private readonly string _text;


        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            
            textArea.Document.Replace(completionSegment, Text);
        }

        public LSLConstantCompletionData(string text, string description, double priority)
        {

            _description = description;
            _priority = priority;
            _text = text;
        }

        public ImageSource Image
        {
            get { return null; }
        }

        public string Text { get { return _text.Substring(1);  } }

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

        Brush _colorBrush = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        public Brush ColorBrush
        {
            get { return _colorBrush; }
            set { _colorBrush = value; }
        }
        public object Description
        {
            get { return _description; }
        }

        public double Priority
        {
            get { return _priority; }
        }
    }
}