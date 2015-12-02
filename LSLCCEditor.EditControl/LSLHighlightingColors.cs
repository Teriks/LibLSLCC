#region FileInfo
// 
// File: LSLHighlightingColors.cs
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

using System.Windows.Media;
using LibLSLCC.Settings;
using LSLCCEditor.Utility.Xml;

namespace LSLCCEditor.EditControl
{
    public class LSLHighlightingColors : SettingsBaseClass<LSLHighlightingColors>
    {
        private XmlColor _stateKeywordColor = Color.FromRgb(127, 0, 38);
        private XmlColor _libraryFunctionColor = Color.FromRgb(127, 0, 38);
        private XmlColor _libraryFunctionDeprecatedColor = Color.FromRgb(232, 19, 174);
        private XmlColor _eventColor = Color.FromRgb(0, 76, 127);
        private XmlColor _typeColor = Color.FromRgb(25, 76, 25);
        private XmlColor _commentColor = Color.FromRgb(255, 127, 80);
        private XmlColor _controlFlowColor = Color.FromRgb(0, 0, 204);
        private XmlColor _stringColor = Color.FromRgb(25, 76, 25);
        private XmlColor _constantColor = Color.FromRgb(50, 52, 138);



        private class DefaultsFactory : CloningDefaultValueFactory<LSLHighlightingColors>
        {
            public DefaultsFactory()
            {

            }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor ConstantColor
        {
            get { return _constantColor; }
            set { SetField(ref _constantColor, value, "ConstantColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor StringColor
        {
            get { return _stringColor; }
            set { SetField(ref _stringColor, value, "StringColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor ControlFlowColor
        {
            get { return _controlFlowColor; }
            set { SetField(ref _controlFlowColor, value, "ControlFlowColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor CommentColor
        {
            get { return _commentColor; }
            set { SetField(ref _commentColor, value, "CommentColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor TypeColor
        {
            get { return _typeColor; }
            set { SetField(ref _typeColor, value, "TypeColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor EventColor
        {
            get { return _eventColor; }
            set { SetField(ref _eventColor, value, "EventColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor LibraryFunctionDeprecatedColor
        {
            get { return _libraryFunctionDeprecatedColor; }
            set { SetField(ref _libraryFunctionDeprecatedColor, value, "LibraryFunctionDeprecatedColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor LibraryFunctionColor
        {
            get { return _libraryFunctionColor; }
            set { SetField(ref _libraryFunctionColor, value, "LibraryFunctionColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor StateKeywordColor
        {
            get { return _stateKeywordColor; }
            set { SetField(ref _stateKeywordColor, value, "StateKeywordColor"); }
        }
    }
}