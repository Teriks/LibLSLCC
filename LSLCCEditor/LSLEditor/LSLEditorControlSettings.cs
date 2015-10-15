using System.Windows.Media;

namespace LSLCCEditor.LSLEditor
{
    public class LSLEditorControlSettings
    {
        private SolidColorBrush _builtInTypeCompleteColor = new SolidColorBrush(Color.FromRgb(50, 52, 138));

        private Brush _eventHandlerCompleteColor = new SolidColorBrush(Color.FromRgb(0, 76, 127));

        private Brush _globalFunctionCompleteColor = new SolidColorBrush(Color.FromRgb(153, 0, 204));

        private Brush _globalVariableCompleteColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        private Brush _libraryConstantCompleteColor = new SolidColorBrush(Color.FromRgb(50, 52, 138));

        private Brush _libraryFunctionCompleteColor = new SolidColorBrush(Color.FromRgb(127, 0, 38));

        private Brush _libraryFunctionDeprecatedCompleteColor = new SolidColorBrush(Color.FromRgb(232, 19, 174));

        private Brush _localParameterCompleteColor = new SolidColorBrush(Color.FromRgb(0, 102, 0));

        private Brush _localVariableCompleteColor = new SolidColorBrush(Color.FromRgb(0, 102, 255));

        private Brush _labelNameDefinitionCompleteColor = new SolidColorBrush(Color.FromRgb(127, 0, 38));

        private Brush _stateNameCompleteColor = new SolidColorBrush(Colors.Black);

        private Brush _labelNameJumpTargetCompleteColor = new SolidColorBrush(Colors.Black);

        private Brush _controlStatementCompleteColor = new SolidColorBrush(Colors.Black);
        private bool _caseInsensitiveAutoCompleteMatching;


        public SolidColorBrush BuiltInTypeCompleteColor
        {
            get { return _builtInTypeCompleteColor; }
            set { _builtInTypeCompleteColor = value; }
        }

        public Brush EventHandlerCompleteColor
        {
            get { return _eventHandlerCompleteColor; }
            set { _eventHandlerCompleteColor = value; }
        }

        public Brush GlobalFunctionCompleteColor
        {
            get { return _globalFunctionCompleteColor; }
            set { _globalFunctionCompleteColor = value; }
        }

        public Brush GlobalVariableCompleteColor
        {
            get { return _globalVariableCompleteColor; }
            set { _globalVariableCompleteColor = value; }
        }

        public Brush LibraryConstantCompleteColor
        {
            get { return _libraryConstantCompleteColor; }
            set { _libraryConstantCompleteColor = value; }
        }

        public Brush LibraryFunctionCompleteColor
        {
            get { return _libraryFunctionCompleteColor; }
            set { _libraryFunctionCompleteColor = value; }
        }

        public Brush LibraryFunctionDeprecatedCompleteColor
        {
            get { return _libraryFunctionDeprecatedCompleteColor; }
            set { _libraryFunctionDeprecatedCompleteColor = value; }
        }

        public Brush LocalParameterCompleteColor
        {
            get { return _localParameterCompleteColor; }
            set { _localParameterCompleteColor = value; }
        }

        public Brush LocalVariableCompleteColor
        {
            get { return _localVariableCompleteColor; }
            set { _localVariableCompleteColor = value; }
        }

        public Brush LabelNameDefinitionCompleteColor
        {
            get { return _labelNameDefinitionCompleteColor; }
            set { _labelNameDefinitionCompleteColor = value; }
        }

        public Brush StateNameCompleteColor
        {
            get { return _stateNameCompleteColor; }
            set { _stateNameCompleteColor = value; }
        }

        public Brush LabelNameJumpTargetCompleteColor
        {
            get { return _labelNameJumpTargetCompleteColor; }
            set { _labelNameJumpTargetCompleteColor = value; }
        }

        public Brush ControlStatementCompleteColor
        {
            get { return _controlStatementCompleteColor; }
            set { _controlStatementCompleteColor = value; }
        }

        public bool CaseInsensitiveAutoCompleteMatching
        {
            get { return _caseInsensitiveAutoCompleteMatching; }
            set { _caseInsensitiveAutoCompleteMatching = value; }
        }

        public bool CamelCaseAutoCompleteMatching { get; set; }
        public bool SubstringSearchAutoCompleteMatching { get; set; }
    }
}