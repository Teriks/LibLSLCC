#region FileInfo
// 
// File: CompilerMessage.cs
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

        public CompilerMessage(CompilerMessageType type, string header, LSLSourceCodeRange location, string message, bool showLineInfo = true)
        {
            CodeLocation = location;
            MessageText = message;
            MessageHeader = header;

            if (showLineInfo)
            {
                LineText = "(" + CodeLocation.LineStart + ", " + CodeLocation.ColumnStart + "):";
            }
            else
            {
                LineText = ":";
            }

            Clickable = true;
            SetColors(type);
        }

        public CompilerMessage(CompilerMessageType type, string header, string message, bool showLineInfo = true)
        {
            CodeLocation = new LSLSourceCodeRange(0, 0);
            MessageText = message;
            MessageHeader = header;
            if (showLineInfo)
            {
                LineText = "(" + CodeLocation.LineStart + ", " + CodeLocation.ColumnStart + "):";
            }
            else
            {
                LineText = ":";
            }
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