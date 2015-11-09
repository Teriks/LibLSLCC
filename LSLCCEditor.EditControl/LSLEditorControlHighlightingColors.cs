using System.Windows.Media;
using LibLSLCC.Utility;
using LSLCCEditor.Utility;

namespace LSLCCEditor.EditControl
{
    public class LSLEditorControlHighlightingColors : SettingsBaseClass
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

        public XmlColor ConstantColor
        {
            get { return _constantColor; }
            set { SetField(ref _constantColor, value, "ConstantColor"); }
        }

        public XmlColor StringColor
        {
            get { return _stringColor; }
            set { SetField(ref _stringColor, value, "StringColor"); }
        }

        public XmlColor ControlFlowColor
        {
            get { return _controlFlowColor; }
            set { SetField(ref _controlFlowColor, value, "ControlFlowColor"); }
        }

        public XmlColor CommentColor
        {
            get { return _commentColor; }
            set { SetField(ref _commentColor, value, "CommentColor"); }
        }

        public XmlColor TypeColor
        {
            get { return _typeColor; }
            set { SetField(ref _typeColor, value, "TypeColor"); }
        }

        public XmlColor EventColor
        {
            get { return _eventColor; }
            set { SetField(ref _eventColor, value, "EventColor"); }
        }

        public XmlColor LibraryFunctionDeprecatedColor
        {
            get { return _libraryFunctionDeprecatedColor; }
            set { SetField(ref _libraryFunctionDeprecatedColor, value, "LibraryFunctionDeprecatedColor"); }
        }

        public XmlColor LibraryFunctionColor
        {
            get { return _libraryFunctionColor; }
            set { SetField(ref _libraryFunctionColor, value, "LibraryFunctionColor"); }
        }

        public XmlColor StateKeywordColor
        {
            get { return _stateKeywordColor; }
            set { SetField(ref _stateKeywordColor, value, "StateKeywordColor"); }
        }
    }
}