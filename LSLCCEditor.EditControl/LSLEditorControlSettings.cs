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

using System.Reflection;
using System.Windows.Media;
using LibLSLCC.Settings;
using LSLCCEditor.Utility.Xml;

namespace LSLCCEditor.EditControl
{
    public class LSLEditorControlSettings : SettingsBaseClass<LSLEditorControlSettings>
    {


        private LSLEditorControlHighlightingColors _highlightingColors = new LSLEditorControlHighlightingColors();


        private LSLEditorCompletionWindowItemBrushes _completionWindowItemBrushes = new LSLEditorCompletionWindowItemBrushes();

        private bool _caseInsensitiveAutoCompleteMatching = true;
        private bool _camelCaseAutoCompleteMatching;
        private bool _substringSearchAutoCompleteMatching;
        private bool _constantCompletionFirstCharIsCaseSensitive = true;
        private XmlColor _backgroundColor = new XmlColor(Colors.White);
        private XmlColor _basicTextColor = new XmlColor(Colors.Black);
        private XmlColor _completionWindowSelectionBackgroundColor = new XmlColor(Colors.AliceBlue);
        private XmlColor _completionWindowSelectionBorderColor = new XmlColor(Colors.RoyalBlue);
        private XmlColor _completionWindowBackgroundColor = new XmlColor(Colors.White);


        private class DefaultColorFactory : IDefaultSettingsValueFactory
        {
            public bool CheckForNecessaryResets(MemberInfo member, object objectInstance, object settingValue)
            {
                if (settingValue == null)
                {
                    return true;
                }
                return false;
            }

            public object GetDefaultValue(MemberInfo member, object objectInstance)
            {
                switch (member.Name)
                {
                    case "BackgroundColor": return new XmlColor(Colors.White);
                    case "BasicTextColor": return new XmlColor(Colors.Black);
                    case "CompletionWindowSelectionBackgroundColor": return new XmlColor(Colors.AliceBlue);
                    case "CompletionWindowSelectionBorderColor": return new XmlColor(Colors.RoyalBlue);
                    case "CompletionWindowBackgroundColor": return new XmlColor(Colors.White);
                }
                return null;
            }
        }

        [DefaultValueFactory(typeof(DefaultColorFactory))]
        public XmlColor BasicTextColor
        {
            get { return _basicTextColor; }
            set { SetField(ref _basicTextColor, value, "BasicTextColor"); }
        }

        [DefaultValueFactory(typeof(DefaultColorFactory))]
        public XmlColor BackgroundColor
        {
            get { return _backgroundColor; }
            set { SetField(ref _backgroundColor, value, "BackgroundColor"); }
        }


        [DefaultValueFactory(typeof(DefaultColorFactory))]
        public XmlColor CompletionWindowSelectionBackgroundColor
        {
            get { return _completionWindowSelectionBackgroundColor; }
            set { SetField(ref _completionWindowSelectionBackgroundColor, value, "CompletionWindowSelectionBackgroundColor"); }
        }


        [DefaultValueFactory(typeof(DefaultColorFactory))]
        public XmlColor CompletionWindowSelectionBorderColor
        {
            get { return _completionWindowSelectionBorderColor; }
            set { SetField(ref _completionWindowSelectionBorderColor, value, "CompletionWindowSelectionBorderColor"); }
        }

        [DefaultValueFactory(typeof (DefaultColorFactory))]
        public XmlColor CompletionWindowBackgroundColor
        {
            get { return _completionWindowBackgroundColor; }
            set { SetField(ref _completionWindowBackgroundColor, value, "CompletionWindowBackgroundColor"); }
        }

        public bool CaseInsensitiveAutoCompleteMatching
        {
            get { return _caseInsensitiveAutoCompleteMatching; }
            set {SetField(ref _caseInsensitiveAutoCompleteMatching,value, "CaseInsensitiveAutoCompleteMatching"); }
        }


        public bool CamelCaseAutoCompleteMatching
        {
            get { return _camelCaseAutoCompleteMatching; }
            set {SetField(ref _camelCaseAutoCompleteMatching,value, "CamelCaseAutoCompleteMatching"); }
        }

        public bool SubstringSearchAutoCompleteMatching
        {
            get { return _substringSearchAutoCompleteMatching; }
            set {SetField(ref _substringSearchAutoCompleteMatching,value, "SubstringSearchAutoCompleteMatching"); }
        }

        public bool ConstantCompletionFirstCharIsCaseSensitive
        {
            get { return _constantCompletionFirstCharIsCaseSensitive; }
            set {SetField(ref _constantCompletionFirstCharIsCaseSensitive,value, "ConstantCompletionFirstCharIsCaseSensitive"); }
        }

        public LSLEditorControlHighlightingColors HighlightingColors
        {
            get { return _highlightingColors; }
            set { SetField(ref _highlightingColors, value, "HighlightingColors"); }
        }

        public LSLEditorCompletionWindowItemBrushes CompletionWindowItemBrushes
        {
            get { return _completionWindowItemBrushes; }
            set { SetField(ref _completionWindowItemBrushes, value, "CompletionWindowItemBrushes"); }
        }


    }
}