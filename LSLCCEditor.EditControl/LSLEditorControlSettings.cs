using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using LSLCCEditor.Utility;

namespace LSLCCEditor.EditControl
{
    public class LSLEditorControlSettings : SettingsClassBase
    {


        private LSLEditorControlHighlightingColors _highlightingColors = new LSLEditorControlHighlightingColors();


        private LSLEditorCompletionBrushes _completionBrushes = new LSLEditorCompletionBrushes();

        private bool _caseInsensitiveAutoCompleteMatching = true;
        private bool _camelCaseAutoCompleteMatching = false;
        private bool _substringSearchAutoCompleteMatching = false;
        private bool _constantCompletionFirstCharIsCaseSensitive = true;


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

        public LSLEditorCompletionBrushes CompletionBrushes
        {
            get { return _completionBrushes; }
            set { SetField(ref _completionBrushes, value, "CompletionBrushes"); }
        }
    }
}