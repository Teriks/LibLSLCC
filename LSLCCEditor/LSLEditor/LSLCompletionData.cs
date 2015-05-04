using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace LSLCCEditor.LSLEditor
{
    public class LSLCompletionData : ICompletionData
    {
        private readonly string _description;
        private readonly double _priority;
        private readonly string _text;
        private readonly string _label;


        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            
            textArea.Document.Replace(completionSegment, Text);
        }

        public LSLCompletionData(string label, string text, string description, double priority)
        {

            _description = description;
            _priority = priority;
            _text = text;
            _label = label;
            TextSubStringStart = 1;
        }

        public ImageSource Image
        {
            get { return null; }
        }

        public int TextSubStringStart { get; set; }

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