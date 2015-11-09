using System.Windows.Media;
using LSLCCEditor.Utility;

namespace LSLCCEditor.EditControl
{
    public class LSLEditorControlSettings
    {
        private XmlBrush _builtInTypeCompleteBrush = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private XmlBrush _eventHandlerCompleteBrush = new SolidColorBrush(Color.FromRgb(0, 76, 127));
        private XmlBrush _globalFunctionCompleteBrush = new SolidColorBrush(Color.FromRgb(153, 0, 204));
        private XmlBrush _globalVariableCompleteBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private XmlBrush _libraryConstantCompleteBrush = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private XmlBrush _libraryFunctionCompleteBrush = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private XmlBrush _libraryFunctionDeprecatedCompleteBrush = new SolidColorBrush(Color.FromRgb(232, 19, 174));
        private XmlBrush _localParameterCompleteBrush = new SolidColorBrush(Color.FromRgb(0, 102, 0));
        private XmlBrush _localVariableCompleteBrush = new SolidColorBrush(Color.FromRgb(0, 102, 255));
        private XmlBrush _labelNameDefinitionCompleteBrush = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private XmlBrush _stateNameCompleteBrush = new SolidColorBrush(Colors.Black);
        private XmlBrush _labelNameJumpTargetCompleteBrush = new SolidColorBrush(Colors.Black);
        private XmlBrush _controlStatementCompleteBrush = new SolidColorBrush(Colors.Black);

        private bool _caseInsensitiveAutoCompleteMatching = true;
        private bool _camelCaseAutoCompleteMatching = false;
        private bool _substringSearchAutoCompleteMatching = false;
        private bool _constantCompletionFirstCharIsCaseSensitive = true;


        public XmlBrush BuiltInTypeCompleteBrush
        {
            get { return _builtInTypeCompleteBrush; }
            set { _builtInTypeCompleteBrush = value; }
        }

        public XmlBrush EventHandlerCompleteBrush
        {
            get { return _eventHandlerCompleteBrush; }
            set { _eventHandlerCompleteBrush = value; }
        }

        public XmlBrush GlobalFunctionCompleteBrush
        {
            get { return _globalFunctionCompleteBrush; }
            set { _globalFunctionCompleteBrush = value; }
        }

        public XmlBrush GlobalVariableCompleteBrush
        {
            get { return _globalVariableCompleteBrush; }
            set { _globalVariableCompleteBrush = value; }
        }

        public XmlBrush LibraryConstantCompleteBrush
        {
            get { return _libraryConstantCompleteBrush; }
            set { _libraryConstantCompleteBrush = value; }
        }

        public XmlBrush LibraryFunctionCompleteBrush
        {
            get { return _libraryFunctionCompleteBrush; }
            set { _libraryFunctionCompleteBrush = value; }
        }

        public XmlBrush LibraryFunctionDeprecatedCompleteBrush
        {
            get { return _libraryFunctionDeprecatedCompleteBrush; }
            set { _libraryFunctionDeprecatedCompleteBrush = value; }
        }

        public XmlBrush LocalParameterCompleteBrush
        {
            get { return _localParameterCompleteBrush; }
            set { _localParameterCompleteBrush = value; }
        }

        public XmlBrush LocalVariableCompleteBrush
        {
            get { return _localVariableCompleteBrush; }
            set { _localVariableCompleteBrush = value; }
        }

        public XmlBrush LabelNameDefinitionCompleteBrush
        {
            get { return _labelNameDefinitionCompleteBrush; }
            set { _labelNameDefinitionCompleteBrush = value; }
        }

        public XmlBrush StateNameCompleteBrush
        {
            get { return _stateNameCompleteBrush; }
            set { _stateNameCompleteBrush = value; }
        }

        public XmlBrush LabelNameJumpTargetCompleteBrush
        {
            get { return _labelNameJumpTargetCompleteBrush; }
            set { _labelNameJumpTargetCompleteBrush = value; }
        }

        public XmlBrush ControlStatementCompleteBrush
        {
            get { return _controlStatementCompleteBrush; }
            set { _controlStatementCompleteBrush = value; }
        }

        public bool CaseInsensitiveAutoCompleteMatching
        {
            get { return _caseInsensitiveAutoCompleteMatching; }
            set { _caseInsensitiveAutoCompleteMatching = value; }
        }

        public bool CamelCaseAutoCompleteMatching
        {
            get { return _camelCaseAutoCompleteMatching; }
            set { _camelCaseAutoCompleteMatching = value; }
        }

        public bool SubstringSearchAutoCompleteMatching
        {
            get { return _substringSearchAutoCompleteMatching; }
            set { _substringSearchAutoCompleteMatching = value; }
        }

        public bool ConstantCompletionFirstCharIsCaseSensitive
        {
            get { return _constantCompletionFirstCharIsCaseSensitive; }
            set { _constantCompletionFirstCharIsCaseSensitive = value; }
        }
    }
}