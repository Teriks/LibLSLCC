using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using LibLSLCC.CodeValidator.Primitives;
using LSLCCEditor.Annotations;

namespace LSLCCEditor
{
    public enum CompilerMessageType
    {
        Warning,
        General,
        Error
    }


    public class CompilerMessage : INotifyPropertyChanged
    {
        private Brush _messageTypeColor;
        private Brush _backgroundColor;
        private string _messageText;
        private string _lineText;
        private string _messageHeader;
        private bool _clickable;
        private LSLSourceCodeRange _codeLocation;

        public CompilerMessage(CompilerMessageType type, string header, LSLSourceCodeRange location, string message)
        {
            CodeLocation = location;
            MessageText = message;
            MessageHeader = header;
            LineText = "("+ CodeLocation.LineStart + ", " + CodeLocation.ColumnStart+")";
            Clickable = true;
            SetColors(type);


        }

        private void SetColors(CompilerMessageType type)
        {
            switch (type)
            {
                case CompilerMessageType.Error:
                    MessageTypeColor = Brushes.Red;
                    BackgroundColor = Brushes.LavenderBlush;
                    break;
                case CompilerMessageType.General:
                    MessageTypeColor = Brushes.Blue;
                    BackgroundColor = Brushes.White;
                    break;
                case CompilerMessageType.Warning:
                    MessageTypeColor = Brushes.DimGray;
                    BackgroundColor = Brushes.Cornsilk;
                    break;
            }
        }

        public CompilerMessage(CompilerMessageType type, string header,  string message)
        {
            CodeLocation = new LSLSourceCodeRange(0,0);
            MessageText = message;
            MessageHeader = header;
            LineText = "(" + CodeLocation.LineStart + ", " + CodeLocation.ColumnStart + ")";
            Clickable = true;
            SetColors(type);
        }


        public string MessageHeader
        {
            get { return _messageHeader; }
            set
            {
                var old = _messageHeader;

                _messageHeader = value;

                if (old != value) this.OnPropertyChanged();
            }
        }

        public string LineText
        {
            get { return _lineText; }
            set
            {
                var old = _lineText;

                _lineText = value;

                if (old != value) this.OnPropertyChanged();
            }
        }

        public string MessageText
        {
            get { return _messageText; }
            set
            {
                var old = _messageText;

                _messageText = value;

                if (old != value) this.OnPropertyChanged();
            }
        }

        public Brush MessageTypeColor
        {
            get { return _messageTypeColor; }
            set
            {
                var old = _messageTypeColor;

                _messageTypeColor = value;

                if (!Equals(old, value)) this.OnPropertyChanged();
            }
        }

        public Brush BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                var old = _backgroundColor;

                _backgroundColor = value;

                if (!Equals(old, value)) this.OnPropertyChanged();
            }
        }

        public bool Clickable
        {
            get { return _clickable; }
            set
            {
                var old = _clickable;

                _clickable = value;

                if(old != value) this.OnPropertyChanged();
            }
        }

        public LSLSourceCodeRange CodeLocation
        {
            get { return _codeLocation; }
            set
            {
                var old = _codeLocation;

                _codeLocation = value;

                if (!Equals(old, value)) this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}