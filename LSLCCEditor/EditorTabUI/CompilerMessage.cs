#region FileInfo

// 
// File: CompilerMessage.cs
// 
// Author/Copyright:  Teriks
// 
// Last Compile: 24/09/2015 @ 9:26 PM
// 
// Creation Date: 03/09/2015 @ 12:50 AM
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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using LibLSLCC.CodeValidator.Primitives;
using LSLCCEditor.Annotations;

#endregion

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
        private Brush _backgroundColor;
        private bool _clickable;
        private LSLSourceCodeRange _codeLocation;
        private string _lineText;
        private string _messageHeader;
        private string _messageText;
        private Brush _messageTypeColor;

        public CompilerMessage(CompilerMessageType type, string header, LSLSourceCodeRange location, string message)
        {
            CodeLocation = location;
            MessageText = message;
            MessageHeader = header;
            LineText = "(" + CodeLocation.LineStart + ", " + CodeLocation.ColumnStart + ")";
            Clickable = true;
            SetColors(type);
        }

        public CompilerMessage(CompilerMessageType type, string header, string message)
        {
            CodeLocation = new LSLSourceCodeRange(0, 0);
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

                if (old != value) OnPropertyChanged();
            }
        }

        public string LineText
        {
            get { return _lineText; }
            set
            {
                var old = _lineText;

                _lineText = value;

                if (old != value) OnPropertyChanged();
            }
        }

        public string MessageText
        {
            get { return _messageText; }
            set
            {
                var old = _messageText;

                _messageText = value;

                if (old != value) OnPropertyChanged();
            }
        }

        public Brush MessageTypeColor
        {
            get { return _messageTypeColor; }
            set
            {
                var old = _messageTypeColor;

                _messageTypeColor = value;

                if (!Equals(old, value)) OnPropertyChanged();
            }
        }

        public Brush BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                var old = _backgroundColor;

                _backgroundColor = value;

                if (!Equals(old, value)) OnPropertyChanged();
            }
        }

        public bool Clickable
        {
            get { return _clickable; }
            set
            {
                var old = _clickable;

                _clickable = value;

                if (old != value) OnPropertyChanged();
            }
        }

        public LSLSourceCodeRange CodeLocation
        {
            get { return _codeLocation; }
            set
            {
                var old = _codeLocation;

                _codeLocation = value;

                if (!Equals(old, value)) OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}