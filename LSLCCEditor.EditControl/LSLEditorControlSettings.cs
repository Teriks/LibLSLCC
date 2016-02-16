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

using LibLSLCC.Settings;

namespace LSLCCEditor.EditControl
{
    public class LSLEditorControlSettings : SettingsBaseClass<LSLEditorControlSettings>
    {
        private bool _caseInsensitiveAutoCompleteMatching = true;
        private bool _camelCaseAutoCompleteMatching;
        private bool _substringSearchAutoCompleteMatching;
        private bool _constantCompletionFirstCharIsCaseSensitive = true;
        private bool _showSpaces;
        private bool _showEndOfLine;
        private bool _showTabs;


        public LSLEditorControlSettings()
        {
            
        }


        private class DefaultsFactory : CloningDefaultValueFactory<LSLEditorControlSettings>
        {
            public DefaultsFactory()
            {

            }
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
        public bool ShowSpaces
        {
            get { return _showSpaces; }
            set { SetField(ref _showSpaces, value, "ShowSpaces"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public bool ShowEndOfLine
        {
            get { return _showEndOfLine; }
            set { SetField(ref _showEndOfLine, value, "ShowEndOfLine"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public bool ShowTabs
        {
            get { return _showTabs; }
            set { SetField(ref _showTabs, value, "ShowTabs"); }
        }
    }
}