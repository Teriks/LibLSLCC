using System.Windows.Media;
using LibLSLCC.Settings;
using LSLCCEditor.Utility.Xml;

namespace LSLCCEditor.EditControl
{
    public class LSLEditorControlTheme : SettingsBaseClass<LSLEditorControlTheme>
    {
        private class DefaultsFactory : CloningDefaultValueFactory<LSLEditorControlTheme>
        {
            public DefaultsFactory()
            {
            }
        }


        public LSLEditorControlTheme()
        {
        }


        private XmlColor _foregroundColor = new XmlColor(Colors.Black);
        private XmlColor _backgroundColor = new XmlColor(Colors.White);
        private XmlColor _completionWindowSelectionBackgroundColor = new XmlColor(Colors.AliceBlue);
        private XmlColor _completionWindowSelectionBorderColor = new XmlColor(Colors.RoyalBlue);
        private XmlColor _completionWindowBackgroundColor = new XmlColor(Colors.White);
        private LSLHighlightingColors _highlightingColors = new LSLHighlightingColors();
        private LSLCompletionWindowItemBrushes _completionWindowItemBrushes = new LSLCompletionWindowItemBrushes();
        private XmlColor _toolTipBackground = new XmlColor((Color) ColorConverter.ConvertFromString("#F1F2F7"));
        private XmlColor _toolTipForeground = new XmlColor((Color) ColorConverter.ConvertFromString("#FF575757"));
        private XmlColor _toolTipBorderColor = new XmlColor((Color) ColorConverter.ConvertFromString("#FF767676"));
        private XmlColor _completionWindowBorderColor = new XmlColor(Colors.DimGray);


        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor ForegroundColor
        {
            get { return _foregroundColor; }
            set { SetField(ref _foregroundColor, value, "ForegroundColor"); }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor BackgroundColor
        {
            get { return _backgroundColor; }
            set { SetField(ref _backgroundColor, value, "BackgroundColor"); }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor CompletionWindowSelectionBackgroundColor
        {
            get { return _completionWindowSelectionBackgroundColor; }
            set
            {
                SetField(ref _completionWindowSelectionBackgroundColor, value,
                    "CompletionWindowSelectionBackgroundColor");
            }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor CompletionWindowSelectionBorderColor
        {
            get { return _completionWindowSelectionBorderColor; }
            set { SetField(ref _completionWindowSelectionBorderColor, value, "CompletionWindowSelectionBorderColor"); }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor CompletionWindowBackgroundColor
        {
            get { return _completionWindowBackgroundColor; }
            set { SetField(ref _completionWindowBackgroundColor, value, "CompletionWindowBackgroundColor"); }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public LSLHighlightingColors HighlightingColors
        {
            get { return _highlightingColors; }
            set { SetField(ref _highlightingColors, value, "HighlightingColors"); }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public LSLCompletionWindowItemBrushes CompletionWindowItemBrushes
        {
            get { return _completionWindowItemBrushes; }
            set { SetField(ref _completionWindowItemBrushes, value, "CompletionWindowItemBrushes"); }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor ToolTipBackground
        {
            get { return _toolTipBackground; }
            set { SetField(ref _toolTipBackground, value, "ToolTipBackground"); }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor ToolTipForeground
        {
            get { return _toolTipForeground; }
            set { SetField(ref _toolTipForeground, value, "ToolTipForeground"); }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor ToolTipBorderColor
        {
            get { return _toolTipBorderColor; }
            set { SetField(ref _toolTipBorderColor, value, "ToolTipBorderColor"); }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor CompletionWindowBorderColor
        {
            get { return _completionWindowBorderColor; }
            set { SetField(ref _completionWindowBorderColor, value, "CompletionWindowBorderColor"); }
        }
    }
}