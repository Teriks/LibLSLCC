#region FileInfo
// 
// File: LSLEditorControlSettings.cs
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
    public class LSLEditorControlSettings : SettingsBaseClass<LSLEditorControlSettings>
    {


        private LSLHighlightingColors _highlightingColors = new LSLHighlightingColors();


        private LSLCompletionWindowItemBrushes _completionWindowItemBrushes = new LSLCompletionWindowItemBrushes();

        private bool _caseInsensitiveAutoCompleteMatching = true;
        private bool _camelCaseAutoCompleteMatching;
        private bool _substringSearchAutoCompleteMatching;
        private bool _constantCompletionFirstCharIsCaseSensitive = true;
        private XmlColor _backgroundColor = new XmlColor(Colors.White);
        private XmlColor _foregroundColor = new XmlColor(Colors.Black);
        private XmlColor _completionWindowSelectionBackgroundColor = new XmlColor(Colors.AliceBlue);
        private XmlColor _completionWindowSelectionBorderColor = new XmlColor(Colors.RoyalBlue);
        private XmlColor _completionWindowBackgroundColor = new XmlColor(Colors.White);
        private XmlColor _completionWindowBorderColor = new XmlColor(Colors.DimGray);


        private XmlColor _toolTipBackground = new XmlColor((Color)ColorConverter.ConvertFromString("#F1F2F7"));
        private XmlColor _toolTipForeground = new XmlColor((Color)ColorConverter.ConvertFromString("#FF575757"));
        private XmlColor _toolTipBorderColor = new XmlColor((Color)ColorConverter.ConvertFromString("#FF767676"));
        

        private class DefaultsFactory : CloningDefaultValueFactory<LSLEditorControlSettings>
        {
            public DefaultsFactory()
            {

            }
        }


        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor ForegroundColor
        {
            get { return _foregroundColor; }
            set { SetField(ref _foregroundColor, value, "ForegroundColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor BackgroundColor
        {
            get { return _backgroundColor; }
            set { SetField(ref _backgroundColor, value, "BackgroundColor"); }
        }


        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor CompletionWindowSelectionBackgroundColor
        {
            get { return _completionWindowSelectionBackgroundColor; }
            set { SetField(ref _completionWindowSelectionBackgroundColor, value, "CompletionWindowSelectionBackgroundColor"); }
        }


        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor CompletionWindowSelectionBorderColor
        {
            get { return _completionWindowSelectionBorderColor; }
            set { SetField(ref _completionWindowSelectionBorderColor, value, "CompletionWindowSelectionBorderColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor CompletionWindowBackgroundColor
        {
            get { return _completionWindowBackgroundColor; }
            set { SetField(ref _completionWindowBackgroundColor, value, "CompletionWindowBackgroundColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public bool CaseInsensitiveAutoCompleteMatching
        {
            get { return _caseInsensitiveAutoCompleteMatching; }
            set {SetField(ref _caseInsensitiveAutoCompleteMatching,value, "CaseInsensitiveAutoCompleteMatching"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public bool CamelCaseAutoCompleteMatching
        {
            get { return _camelCaseAutoCompleteMatching; }
            set {SetField(ref _camelCaseAutoCompleteMatching,value, "CamelCaseAutoCompleteMatching"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public bool SubstringSearchAutoCompleteMatching
        {
            get { return _substringSearchAutoCompleteMatching; }
            set {SetField(ref _substringSearchAutoCompleteMatching,value, "SubstringSearchAutoCompleteMatching"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public bool ConstantCompletionFirstCharIsCaseSensitive
        {
            get { return _constantCompletionFirstCharIsCaseSensitive; }
            set {SetField(ref _constantCompletionFirstCharIsCaseSensitive,value, "ConstantCompletionFirstCharIsCaseSensitive"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public LSLHighlightingColors HighlightingColors
        {
            get { return _highlightingColors; }
            set { SetField(ref _highlightingColors, value, "HighlightingColors"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public LSLCompletionWindowItemBrushes CompletionWindowItemBrushes
        {
            get { return _completionWindowItemBrushes; }
            set { SetField(ref _completionWindowItemBrushes, value, "CompletionWindowItemBrushes"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor ToolTipBackground
        {
            get { return _toolTipBackground; }
            set { SetField(ref _toolTipBackground,value, "ToolTipBackground"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor ToolTipForeground
        {
            get { return _toolTipForeground; }
            set { SetField(ref _toolTipForeground,value, "ToolTipForeground"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor ToolTipBorderColor
        {
            get { return _toolTipBorderColor; }
            set { SetField(ref _toolTipBorderColor,value, "ToolTipBorderColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor CompletionWindowBorderColor
        {
            get { return _completionWindowBorderColor; }
            set { SetField(ref _completionWindowBorderColor, value, "CompletionWindowBorderColor"); }
        }
    }
}