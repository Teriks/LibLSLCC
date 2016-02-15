#region FileInfo
// 
// File: LSLEditorControlTheme.cs
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

using System.Windows;
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
            var selection = SystemColors.HighlightColor;
            _selectionColor = new XmlColor(Color.FromArgb(178,selection.R,selection.G,selection.B));
            _selectionBorderColor = new XmlColor(_selectionColor.Clone());
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
        private XmlColor _symbolSelectionBorderColor = Colors.Black;
        private XmlColor _symbolSelectionColor = Colors.Transparent;
        private XmlColor _symbolSelectionForegroundColor = Colors.Red;
        private XmlColor _selectionForegroundColor = new XmlColor(Colors.White);
        private XmlColor _selectionColor;
        private XmlColor _selectionBorderColor;
        private XmlColor _toolTipDeprecationMarkerColor = Colors.Red;


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

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor SymbolSelectionBorderColor
        {
            get { return _symbolSelectionBorderColor; }
            set { SetField(ref _symbolSelectionBorderColor, value, "SymbolSelectionBorderColor"); }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor SymbolSelectionColor
        {
            get { return _symbolSelectionColor; }
            set { SetField(ref _symbolSelectionColor, value, "SymbolSelectionColor"); }
        }

        [DefaultValueFactory(typeof (DefaultsFactory))]
        public XmlColor SymbolSelectionForegroundColor
        {
            get { return _symbolSelectionForegroundColor; }
            set { SetField(ref _symbolSelectionForegroundColor, value, "SymbolSelectionForegroundColor"); }
        }




        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor SelectionColor
        {
            get { return _selectionColor; }
            set { SetField(ref _selectionColor, value, "SelectionColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor SelectionBorderColor
        {
            get { return _selectionBorderColor; }
            set { SetField(ref _selectionBorderColor, value, "SelectionBorderColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor SelectionForegroundColor
        {
            get { return _selectionForegroundColor; }
            set { SetField(ref _selectionForegroundColor, value, "SelectionForegroundColor"); }
        }

        [DefaultValueFactory(typeof(DefaultsFactory))]
        public XmlColor ToolTipDeprecationMarkerColor
        {
            get { return _toolTipDeprecationMarkerColor; }
            set { SetField(ref _toolTipDeprecationMarkerColor, value, "ToolTipDeprecationMarkerColor"); }
        }
    }
}