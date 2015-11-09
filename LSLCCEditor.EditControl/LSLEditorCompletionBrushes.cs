using System.Windows.Media;
using LSLCCEditor.Utility;

namespace LSLCCEditor.EditControl
{
    public class LSLEditorCompletionBrushes : SettingsClassBase
    {
        private XmlSolidBrush _typeBrush = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private XmlSolidBrush _eventHandlerBrush = new SolidColorBrush(Color.FromRgb(0, 76, 127));
        private XmlSolidBrush _globalFunctionBrush = new SolidColorBrush(Color.FromRgb(153, 0, 204));
        private XmlSolidBrush _globalVariableBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private XmlSolidBrush _libraryConstantBrush = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private XmlSolidBrush _libraryFunctionBrush = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private XmlSolidBrush _libraryFunctionDeprecatedBrush = new SolidColorBrush(Color.FromRgb(232, 19, 174));
        private XmlSolidBrush _localParameterBrush = new SolidColorBrush(Color.FromRgb(0, 102, 0));
        private XmlSolidBrush _localVariableBrush = new SolidColorBrush(Color.FromRgb(0, 102, 255));
        private XmlSolidBrush _labelNameDefinitionBrush = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private XmlSolidBrush _stateNameBrush = new SolidColorBrush(Colors.Black);
        private XmlSolidBrush _labelNameJumpTargetBrush = new SolidColorBrush(Colors.Black);
        private XmlSolidBrush _controlStatementBrush = new SolidColorBrush(Colors.Black);




        public XmlSolidBrush TypeBrush
        {
            get { return _typeBrush; }
            set { SetField(ref _typeBrush, value, "TypeBrush"); }
        }

        public XmlSolidBrush EventHandlerBrush
        {
            get { return _eventHandlerBrush; }
            set { SetField(ref _eventHandlerBrush, value, "EventHandlerBrush"); }
        }

        public XmlSolidBrush GlobalFunctionBrush
        {
            get { return _globalFunctionBrush; }
            set { SetField(ref _globalFunctionBrush, value, "GlobalFunctionBrush"); }
        }

        public XmlSolidBrush GlobalVariableBrush
        {
            get { return _globalVariableBrush; }
            set { SetField(ref _globalVariableBrush, value, "GlobalVariableBrush"); }
        }

        public XmlSolidBrush LibraryConstantBrush
        {
            get { return _libraryConstantBrush; }
            set { SetField(ref _libraryConstantBrush, value, "LibraryConstantBrush"); }
        }

        public XmlSolidBrush LibraryFunctionBrush
        {
            get { return _libraryFunctionBrush; }
            set { SetField(ref _libraryFunctionBrush, value, "LibraryFunctionBrush"); }
        }

        public XmlSolidBrush LibraryFunctionDeprecatedBrush
        {
            get { return _libraryFunctionDeprecatedBrush; }
            set { SetField(ref _libraryFunctionDeprecatedBrush, value, "LibraryFunctionDeprecatedBrush"); }
        }

        public XmlSolidBrush LocalParameterBrush
        {
            get { return _localParameterBrush; }
            set { SetField(ref _localParameterBrush, value, "LocalParameterBrush"); }
        }

        public XmlSolidBrush LocalVariableBrush
        {
            get { return _localVariableBrush; }
            set { SetField(ref _localVariableBrush, value, "LocalVariableBrush"); }
        }

        public XmlSolidBrush LabelNameDefinitionBrush
        {
            get { return _labelNameDefinitionBrush; }
            set { SetField(ref _labelNameDefinitionBrush, value, "LabelNameDefinitionBrush"); }
        }

        public XmlSolidBrush StateNameBrush
        {
            get { return _stateNameBrush; }
            set { SetField(ref _stateNameBrush, value, "StateNameBrush"); }
        }

        public XmlSolidBrush LabelNameJumpTargetBrush
        {
            get { return _labelNameJumpTargetBrush; }
            set { SetField(ref _labelNameJumpTargetBrush, value, "LabelNameJumpTargetBrush"); }
        }

        public XmlSolidBrush ControlStatementBrush
        {
            get { return _controlStatementBrush; }
            set { SetField(ref _controlStatementBrush, value, "ControlStatementBrush"); }
        }
    }
}