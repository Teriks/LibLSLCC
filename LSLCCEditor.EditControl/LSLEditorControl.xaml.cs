#region FileInfo
// 
// File: LSLEditorControl.xaml.cs
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

//#define DEBUG_AUTOCOMPLETE_PARSER

#endregion

#region Imports

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation;
using LibLSLCC.AutoComplete;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.Collections;
using LibLSLCC.LibraryData;
using LibLSLCC.Settings;
using LibLSLCC.Utility;
using LSLCCEditor.Utility;
using LSLCCEditor.Utility.Binding;
using LSLCCEditor.Utility.Xml;
using CompletionWindow = LSLCCEditor.CompletionUI.CompletionWindow;

#endregion



namespace LSLCCEditor.EditControl
{
    /// <summary>
    ///     Interaction logic for Test.xaml
    /// </summary>
    public partial class LSLEditorControl : UserControl
    {
#if DEBUG_AUTOCOMPLETE_PARSER
        private readonly DebugObjectView _debugObjectView = new DebugObjectView();
#endif



        public TextEditor Editor
        {
            get { return AvalonEditor; }
        }



        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(
            "Theme", typeof (LSLEditorControlTheme), typeof (LSLEditorControl),
            new PropertyMetadata(default(LSLEditorControlTheme), ThemePropertyChangedCallback));



        private static void ThemePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var self = (LSLEditorControl)dependencyObject;

            if (dependencyPropertyChangedEventArgs.OldValue != null)
            {
                var old = (LSLEditorControlTheme)dependencyPropertyChangedEventArgs.OldValue;

                old.HighlightingColors.UnSubscribePropertyChangedRecursive(dependencyObject);

                old.BackgroundColor.UnSubscribePropertyChanged(dependencyObject);

                old.ForegroundColor.UnSubscribePropertyChanged(dependencyObject);

                old.SelectionForegroundColor.UnSubscribePropertyChanged(dependencyObject);

                old.SelectionColor.UnSubscribePropertyChanged(dependencyObject);

                old.SelectionBorderColor.UnSubscribePropertyChanged(dependencyObject);

                old.UnSubscribePropertyChanged(dependencyObject);
            }


            if (dependencyPropertyChangedEventArgs.NewValue == null) return;

            var n = (LSLEditorControlTheme)dependencyPropertyChangedEventArgs.NewValue;

            n.HighlightingColors.SubscribePropertyChangedRecursive(dependencyObject, HighlightingSettingsPropertyChanged);

            self.Editor.Foreground = new SolidColorBrush(n.ForegroundColor.Content);
            self.Editor.Background = new SolidColorBrush(n.BackgroundColor.Content);

            self.Editor.TextArea.SelectionBorder = new Pen(new SolidColorBrush(n.SelectionBorderColor), 1);
            self.Editor.TextArea.SelectionBrush = new SolidColorBrush(n.SelectionColor);
            self.Editor.TextArea.SelectionForeground = new SolidColorBrush(n.SelectionForegroundColor);

            n.BackgroundColor.SubscribePropertyChanged(dependencyObject, BackgroundColorSettingPropertyChanged);

            n.ForegroundColor.SubscribePropertyChanged(dependencyObject, BasicTextColorSettingChanged);

            n.SelectionColor.SubscribePropertyChanged(dependencyObject, SelectionColorThemeSettingChanged);

            n.SelectionBorderColor.SubscribePropertyChanged(dependencyObject, SelectionBorderColorThemeSettingChanged);

            n.SelectionForegroundColor.SubscribePropertyChanged(dependencyObject, SelectionForegroundColorThemeSettingChanged);

            n.SubscribePropertyChanged(dependencyObject, EditorThemePropertyChanged);


            self.UpdateHighlightingColorsFromSettings(true);
        }


        private static void SelectionBorderColorThemeSettingChanged(SettingsPropertyChangedEventArgs<XmlSerializableXaml<Color>> settingsPropertyChangedEventArgs)
        {
            var suber = (LSLEditorControl)settingsPropertyChangedEventArgs.Subscriber;

            suber.AvalonEditor.TextArea.SelectionBorder.Brush = new SolidColorBrush(settingsPropertyChangedEventArgs.PropertyOwner.Content);
        }


        private static void SelectionForegroundColorThemeSettingChanged(SettingsPropertyChangedEventArgs<XmlSerializableXaml<Color>> settingsPropertyChangedEventArgs)
        {
            var suber = (LSLEditorControl)settingsPropertyChangedEventArgs.Subscriber;

            suber.AvalonEditor.TextArea.SelectionForeground = new SolidColorBrush(settingsPropertyChangedEventArgs.PropertyOwner.Content);
        }


        private static void SelectionColorThemeSettingChanged(SettingsPropertyChangedEventArgs<XmlSerializableXaml<Color>> settingsPropertyChangedEventArgs)
        {
            var suber = (LSLEditorControl)settingsPropertyChangedEventArgs.Subscriber;

            suber.AvalonEditor.TextArea.SelectionBrush = new SolidColorBrush(settingsPropertyChangedEventArgs.PropertyOwner.Content);
        }


        private static void EditorThemePropertyChanged(SettingsPropertyChangedEventArgs<LSLEditorControlTheme> settingsPropertyChangedEventArgs)
        {
            var suber = (LSLEditorControl)settingsPropertyChangedEventArgs.Subscriber;

            switch (settingsPropertyChangedEventArgs.PropertyName)
            {
                case "HighlightingColors":
                    suber.UpdateHighlightingColorsFromSettings(true);
                    break;
                case "BackgroundColor":
                    suber.Editor.Background = new SolidColorBrush((XmlColor)settingsPropertyChangedEventArgs.NewValue);
                    break;
                case "ForegroundColor":
                    suber.Editor.Foreground = new SolidColorBrush((XmlColor)settingsPropertyChangedEventArgs.NewValue);
                    break;
                case "SelectionBorderColor":
                    suber.Editor.TextArea.SelectionBorder.Brush = new SolidColorBrush((XmlColor)settingsPropertyChangedEventArgs.NewValue);
                    break;
                case "SelectionForegroundColor":
                    suber.Editor.TextArea.SelectionForeground = new SolidColorBrush((XmlColor)settingsPropertyChangedEventArgs.NewValue);
                    break;
                case "SelectionColor":
                    suber.Editor.TextArea.SelectionBrush = new SolidColorBrush((XmlColor)settingsPropertyChangedEventArgs.NewValue);
                    break; 
            }
        }

        private static void HighlightingSettingsPropertyChanged(SettingsPropertyChangedEventArgs<object> settingsPropertyChangedEventArgs)
        {
            var suber = (LSLEditorControl)settingsPropertyChangedEventArgs.Subscriber;

            suber.UpdateHighlightingColorsFromSettings(true);


        }


        private static void BasicTextColorSettingChanged(SettingsPropertyChangedEventArgs<XmlSerializableXaml<Color>> settingsPropertyChangedEventArgs)
        {
            var suber = (LSLEditorControl)settingsPropertyChangedEventArgs.Subscriber;

            suber.Editor.Foreground = new SolidColorBrush(settingsPropertyChangedEventArgs.PropertyOwner.Content);
        }


        private static void BackgroundColorSettingPropertyChanged(SettingsPropertyChangedEventArgs<XmlSerializableXaml<Color>> settingsPropertyChangedEventArgs)
        {
            var suber = (LSLEditorControl)settingsPropertyChangedEventArgs.Subscriber;

            suber.Editor.Background = new SolidColorBrush(settingsPropertyChangedEventArgs.PropertyOwner.Content);
        }



        public LSLEditorControlTheme Theme
        {
            get { return (LSLEditorControlTheme) GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
            "Settings", typeof (LSLEditorControlSettings), typeof (LSLEditorControl), new PropertyMetadata(default(LSLEditorControlSettings)));




        public LSLEditorControlSettings Settings
        {
            get { return (LSLEditorControlSettings) GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }



        public delegate void TextChangedEventHandler(object sender, EventArgs e);


        
        private readonly object _completionLock = new object();
        private readonly ToolTip _symbolHoverToolTip = new ToolTip();



        private readonly object _propertyChangingLock = new object();


        private readonly HashSet<string> _eventIndentBreakTriggers = new HashSet<string>
        {
            "{",
            "}"
        };

        private readonly HashSet<string> _controlStatementIndentBreakTriggers = new HashSet<string>
        {
            ";",
            "{",
            "}"
        };

        private readonly HashSet<string> _singleBlockControlStatementIndentBreakTriggers = new HashSet<string>
        {
            "do", 
            "else", 
            ")", //after a control statement condition
        };

        private readonly object _userChangingTextLock = new object();


        //suggestion prefixes and suffixes are used to rule out the need for a parse
        //up to the caret.  In some situations we can determine that auto-complete is not
        //needed or will not produce results.

        //An auto complete parse only takes place if the character preceding the inserted text
        //matches one of these
        private readonly HashSet<string> _validSuggestionPrefixes = new HashSet<string>
        {
            "\t", //after a word break
            "\r", // ^
            "\n", // ^
            " ",  // ^
            //".",  //member accessor (not used yet)
            "{", //after the start of a scope
            "}", //after a scope
            "[", //after the start of a list literal
            "(", //after the start of a parenthesized expression group, cast, parameter list, ect..
            ")", //after a single statement code scope starts
            "<", //after comparison, vectors start and left shift operator
            ">", //after right shift, and comparison
            "&", //after logical and bitwise and
            "^", //after xor
            "|", //after logical and bitwise or
            "~", //after bitwise not
            "!", //after logical not
            ",", //after a comma in an expression list
            ";", //after a statement terminator
            "=", //after all types of assignment
            "+", //after addition operator or prefix increment
            "-", //after negation, subtraction operator or prefix decrement
            "*", //after multiplication operator
            "/", //after division operator
            "%", //after modulus operator
            "@", //after a label name prefix
            ""   //at the beginning of the file
        };

        //Used when something is being inserted over a selection.
        //an auto complete parse only takes place if character after the end of the selection
        //matches one of these.
        private readonly HashSet<string> _validSuggestionSuffixes = new HashSet<string>
        {
            "\t", //before a word break
            "\r", // ^
            "\n", // ^
            " ",  // ^
            "{", //ending point is at the start of an anonymous scope
            "}", //for when a selection ending point is at the end of a scope
            "]", //before the end of a list literal
            "(", //before a cast or parenthesized expression
            ")", //at end of function parameters, cast, parenthesized expr, etc..
            "<", //before comparison or left shift
            ">", //before comparison or right shift
            "&", //before logical or bitwise and
            "^", //before xor
            "|", //before or or bitwise or
            ",", //before a comma in an expression list
            ";", //before a statement terminator
            "=", //before direct assignment
            "+", //before addition, add assign or postfix increment
            "-", //before subtraction, subtract assign or postfix decrement
            "*", //before multiply or multiply assign
            "/", //before divide or divide assign
            "%", //before modulus or modulus assign
            "" //before the end of the file
        };


        private bool _textPropertyChangingText;
        private bool _userChangingText;


        private class LSLIndentStrategy :
            IIndentationStrategy
        {
            public void IndentLine(TextDocument document, DocumentLine line)
            {
                if (document == null)
                    throw new ArgumentNullException("document");
                if (line == null)
                    throw new ArgumentNullException("line");
                var previousLine = line.PreviousLine;
                if (previousLine != null)
                {
                    var indentationSegment = TextUtilities.GetWhitespaceAfter(document, previousLine.Offset);
                    var indentation = document.GetText(indentationSegment);
                    var offset = line.Offset - 1;
                    while (offset > 0 && offset >= previousLine.Offset)
                    {
                        var lastChar = document.GetText(offset, 1);
                        if (lastChar == "{")
                        {
                            indentation += "\t";
                            break;
                        }
                        if (!string.IsNullOrWhiteSpace(lastChar))
                        {
                            break;
                        }
                        offset--;
                    }

                    // copy indentation to line
                    indentationSegment = TextUtilities.GetWhitespaceAfter(document, line.Offset);

                    document.Replace(indentationSegment, indentation);
                }
            }

            public void IndentLines(TextDocument document, int beginLine, int endLine)
            {
            }
        }


        public LSLEditorControl()
        {
            AutoCompleteUserDefined = new RelayCommand(AutoCompleteUserDefinedCommand);
            AutoCompleteLibraryFunctions = new RelayCommand(AutoCompleteLibraryFunctionsCommand);
            AutoCompleteLibraryConstants = new RelayCommand(AutoCompleteLibraryConstantsCommand);

            InitializeComponent();


            Editor.TextArea.TextEntering += TextArea_TextEntering;
            Editor.MouseHover += TextEditor_MouseHover;
            Editor.MouseHover += TextEditor_MouseHoverStopped;

            Editor.TextArea.IndentationStrategy =
                new LSLIndentStrategy();


            Settings = new LSLEditorControlSettings();

            Theme = new LSLEditorControlTheme();

            Settings.CaseInsensitiveAutoCompleteMatching = true;

            Settings.ConstantCompletionFirstCharIsCaseSensitive = true;

            Editor.TextArea.Options.EnableRectangularSelection = true;



#if DEBUG_AUTOCOMPLETE_PARSER
            _debugObjectView.Show();
#endif
        }


        public CompletionWindow CurrentCompletionWindow { get; private set; }
        public ICommand AutoCompleteUserDefined { get; set; }
        public ICommand AutoCompleteLibraryConstants { get; set; }
        public ICommand AutoCompleteLibraryFunctions { get; set; }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public IEnumerable<LSLLibraryConstantSignature> ConstantSignatures
        {
            get { return LibraryDataProvider.LibraryConstants; }
        }

        public IEnumerable<LSLLibraryEventSignature> EventSignatures
        {
            get { return LibraryDataProvider.LibraryEvents; }
        }

        public IEnumerable<string> LibraryFunctionNames
        {
            get { return LibraryDataProvider.LibraryFunctions.Where(x => x.Count > 0).Select(x => x.First().Name); }
        }

        public ILSLLibraryDataProvider LibraryDataProvider
        {
            get { return (ILSLLibraryDataProvider) GetValue(LibraryDataProviderProperty); }
            set { SetValue(LibraryDataProviderProperty, value); }
        }


        private void AutoCompleteLibraryConstantsCommand(object o)
        {
            if (CurrentCompletionWindow != null)
            {
                CurrentCompletionWindow.CompletionList.RequestInsertion(EventArgs.Empty);
                return;
            }

            SuggestLibraryConstants();
        }


        private void AutoCompleteUserDefinedCommand(object o)
        {
            if (CurrentCompletionWindow != null)
            {
                CurrentCompletionWindow.CompletionList.RequestInsertion(EventArgs.Empty);
                return;
            }


            SuggestUserDefinedOrEvent();
        }


        private void AutoCompleteLibraryFunctionsCommand(object o)
        {
            if (CurrentCompletionWindow != null)
            {
                CurrentCompletionWindow.CompletionList.RequestInsertion(EventArgs.Empty);
                return;
            }

            SuggestLibraryFunctions();
        }


        private static void TextPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var editor = (LSLEditorControl) dependencyObject;

            if (editor._userChangingText || editor.Editor.Document == null) return;

            editor._textPropertyChangingText = true;
            editor.Editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue == null
                ? ""
                : dependencyPropertyChangedEventArgs.NewValue.ToString();
            editor._textPropertyChangingText = false;
        }


        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            OnTextChanged();
            lock (_propertyChangingLock)
            {
                if (_textPropertyChangingText)
                {
                    return;
                }
            }
            lock (_userChangingTextLock)
            {
                var editor = (TextEditor) sender;
                _userChangingText = true;
                Text = editor.Text;
                _userChangingText = false;
                OnUserChangedText();
            }
        }


        public event TextChangedEventHandler UserChangedText;
        public event TextChangedEventHandler TextChanged;


        protected virtual void OnTextChanged()
        {
            var handler = TextChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        protected virtual void OnUserChangedText()
        {
            var handler = UserChangedText;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        private static void LibraryDataProviderPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyPropertyChangedEventArgs.NewValue != null)
            {
                var control = (LSLEditorControl) dependencyObject;
                control.UpdateHighlightingFromDataProvider(
                    (ILSLLibraryDataProvider) dependencyPropertyChangedEventArgs.NewValue, true);
            }
        }



        private TextSegment _GetIDSegmentUnderMouse(TextDocument document, TextViewPosition position)
        {
            var line = position.Line;
            var column = position.Column;

            var mouseOffset = document.GetOffset(line, column);



            var parser = new LSLCommentStringSkipper();
            parser.ParseUpTo(Editor.Text, mouseOffset);
            if (parser.InComment || parser.InString)
            {
                return null;
            }

            if (mouseOffset >= document.TextLength)
                mouseOffset--;

            var textAtOffset = document.GetText(mouseOffset, 1);

            if (string.IsNullOrWhiteSpace(textAtOffset)) return null;

            var startOffset = mouseOffset;
            var endOffset = mouseOffset;

            // Get text backward of the mouse position, until the first space
            while (!(string.IsNullOrWhiteSpace(textAtOffset) || !LSLTokenTools.IDAnyCharRegex.Match(textAtOffset).Success))
            {
                //wordHovered = textAtOffset + wordHovered;

                startOffset--;

                if (startOffset < 0)
                    break;


                textAtOffset = document.GetText(startOffset, 1);
            }

            // Get text forward the mouse position, until the first space
            if (endOffset < document.TextLength - 1)
            {
                endOffset++;

                textAtOffset = document.GetText(endOffset, 1);


                while (!(string.IsNullOrWhiteSpace(textAtOffset) || !LSLTokenTools.IDAnyCharRegex.Match(textAtOffset).Success))
                {
                    //wordHovered = wordHovered + textAtOffset;

                    endOffset++;
                    if (endOffset >= document.TextLength)
                        break;


                    textAtOffset = document.GetText(endOffset, 1);
                }
            }

            if (startOffset == endOffset) return null;

            var wordHovered = "";
            var length = 0;

            if (startOffset == -1)
            {
                startOffset = 0;
                length = endOffset - startOffset;
                wordHovered = document.GetText(0, length);
            }
            else if (startOffset < endOffset)
            {
                startOffset = startOffset + 1;
                length = (endOffset - startOffset);
                wordHovered = document.GetText(startOffset, length);
            }
            else if (endOffset == 0)
            {
                startOffset = startOffset + 1;
                length = 1;
                wordHovered = document.GetText(startOffset, length);
            }

            if (LSLTokenTools.IDRegexAnchored.Match(wordHovered).Success)
            {
                return new TextSegment
                {
                    Length = length,
                    StartOffset = startOffset,
                    EndOffset = startOffset + length
                };
            }

            return null;
        }


        private TextSegment GetIdSegmentUnderMouse(TextDocument document, TextViewPosition position)
        {
            try
            {
                return _GetIDSegmentUnderMouse(document, position);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        private string GetIdUnderMouse(TextDocument document, TextViewPosition position)
        {
            var sec = GetIdSegmentUnderMouse(document, position);

            if (sec == null) return "";

            var text = document.GetText(sec);
            return text;
        }


        private void TextEditor_MouseHover(object sender, MouseEventArgs e)
        {
            var pos = Editor.GetPositionFromPoint(e.GetPosition(Editor));

            if (pos != null)
            {
                var hoveredSegment = GetIdSegmentUnderMouse(Editor.Document, pos.Value);
                if (hoveredSegment == null)
                {
                    e.Handled = true;
                    _symbolHoverToolTip.IsOpen = false;
                    return;
                }

                var wordHovered = Editor.Document.GetText(hoveredSegment);


                TextBlock hoverText = null;


                if (LibraryDataProvider.LibraryFunctionExist(wordHovered))
                {
                    hoverText =
                        CreateDescriptionTextBlock_LibraryFunction(
                            LibraryDataProvider.GetLibraryFunctionSignatures(wordHovered));
                }
                else if (LibraryDataProvider.EventHandlerExist(wordHovered))
                {
                    hoverText =
                        CreateDescriptionTextBlock_EventHandler(LibraryDataProvider.GetEventHandlerSignature(wordHovered));
                }
                else if (LibraryDataProvider.LibraryConstantExist(wordHovered))
                {
                    hoverText =
                        CreateDescriptionTextBlock_LibraryConstant(
                            LibraryDataProvider.GetLibraryConstantSignature(wordHovered));
                }
                else
                {
                    var parser = new LSLAutoCompleteParser();
                    parser.Parse(new StringReader(Editor.Text), hoveredSegment.EndOffset);


                    LSLAutoCompleteGlobalVariable globalVariable;
                    LSLAutoCompleteGlobalFunction globalFunction;
                    LSLAutoCompleteLocalParameter localParameter;

                    if (parser.GlobalFunctionsDictionary.TryGetValue(wordHovered, out globalFunction))
                    {
                        hoverText = CreateDescriptionTextBlock_GlobalUserFunction(globalFunction);
                    }
                    else if (parser.GlobalVariablesDictionary.TryGetValue(wordHovered, out globalVariable))
                    {
                        hoverText = CreateGlobalVariableDescriptionTextBlock(globalVariable);
                    }
                    else if (parser.LocalParametersDictionary.TryGetValue(wordHovered, out localParameter))
                    {
                        hoverText = CreateDescriptionTextBlock_LocalParameter(localParameter);
                    }
                    else
                    {
                        var localVar = parser.LocalVariables.LastOrDefault(x => x.Name == wordHovered);
                        if (localVar != null)
                        {
                            hoverText = CreateDescriptionTextBlock_LocalVariable(localVar);
                        }
                    }
                }

                if (hoverText == null)
                {
                    e.Handled = true;
                    _symbolHoverToolTip.IsOpen = false;
                    return;
                }

                _symbolHoverToolTip.IsOpen = false;

                _symbolHoverToolTip.PlacementTarget = this; // required for property inheritance
                _symbolHoverToolTip.Content = hoverText;
                _symbolHoverToolTip.Placement = PlacementMode.Mouse;
                _symbolHoverToolTip.IsOpen = true;
                _symbolHoverToolTip.Foreground = new SolidColorBrush(Theme.ToolTipForeground);
                _symbolHoverToolTip.Background = new SolidColorBrush(Theme.ToolTipBackground);
                _symbolHoverToolTip.BorderBrush = new SolidColorBrush(Theme.ToolTipBorderColor);

                e.Handled = true;
            }
        }


        private void TextEditor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            _symbolHoverToolTip.IsOpen = false;
            e.Handled = true;
        }




        private CompletionWindow CreateNewCompletionWindow()
        {
            var c = new CompletionWindow(Editor.TextArea);


            c.ToolTipHorizontalOffset = 5;

            c.SizeToCompletionTextBlockContent = true;
            c.ResizeMode = ResizeMode.NoResize;

            c.Closed += (sender, args) => { CloseCurrentCompletionWindow(); };

            c.CloseWhenCaretAtBeginning = true;

            c.CompletionList.SubstringSearchWhileFiltering = Settings.SubstringSearchAutoCompleteMatching;

            c.CompletionList.CamelCaseMatching = Settings.CamelCaseAutoCompleteMatching;

            c.CompletionList.CaseInsensitiveMatching = Settings.CaseInsensitiveAutoCompleteMatching;

            c.CompletionList.ListBox.Background = new SolidColorBrush(Theme.CompletionWindowBackgroundColor);

            c.SelectedItemBackground = new SolidColorBrush(Theme.CompletionWindowSelectionBackgroundColor);

            
            var borderBrush = (DrawingBrush)Resources["CompletionWindowSelectionBorderBrush"];

            ((GeometryDrawing) ((DrawingGroup) borderBrush.Drawing).Children[0]).Brush =
                    new SolidColorBrush(Theme.CompletionWindowSelectionBorderColor);

            c.SelectedItemBorderBrush = borderBrush;

            c.ToolTipBackground = new SolidColorBrush(Theme.ToolTipBackground);
            c.ToolTipBorderBrush = new SolidColorBrush(Theme.ToolTipBorderColor);
            c.ToolTipForeground = new SolidColorBrush(Theme.ToolTipForeground);

            c.Background = c.CompletionList.ListBox.Background;
            c.BorderBrush = new SolidColorBrush(Theme.CompletionWindowBorderColor);
            c.BorderThickness = new Thickness(1);

            c.CompletionList.ListBox.BorderThickness= new Thickness(0);

            return c;
        }


        public void CloseCurrentCompletionWindow()
        {
            lock (_completionLock)
            {
                if (CurrentCompletionWindow == null) return;

                CurrentCompletionWindow.Close();
                CurrentCompletionWindow = null;
            }
        }

        private CompletionWindow LazyInitCompletionWindow()
        {
            lock (_completionLock)
            {
                if (CurrentCompletionWindow != null) return CurrentCompletionWindow;

                return CreateNewCompletionWindow();
            }
        }


        private bool DoAutoDedentOnTextEntering(TextCompositionEventArgs enteringArgs, int caretOffset)
        {
            var textArea = Editor.TextArea;

            if (enteringArgs.Text == "}" && textArea.Document.GetText(caretOffset - 1, 1) == "\t")
            {
                var offset = caretOffset - 1;
                while (offset > 0)
                {
                    var text = textArea.Document.GetText(offset, 1);
                    if (text == ";" || text == "{" || text == "}")
                    {
                        textArea.Document.Remove(caretOffset - 1, 1);
                        return true;
                    }
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        break;
                    }

                    offset--;
                }
            }
            return false;
        }


        private string FindKeywordWordBehindOffset(string text, int caretOffset)
        {
            var behindOffset = caretOffset > 1 ? caretOffset - 1 : -1;

            var inWord = false;
            var word = "";
            while (behindOffset >= 0)
            {
                var c = text[behindOffset];

                if (char.IsWhiteSpace(c) && inWord == false)
                {
                    behindOffset--;
                    continue;
                }


                inWord = true;


                //take advantage of the fact LSL keywords have no special symbols in them
                if (char.IsLetter(c))
                {
                    word = c + word;
                }
                else
                {
                    return word == "" ? null : word;
                }

                behindOffset--;
            }

            return word == "" ? null : word;
        }


        private bool KeywordPriorBlocksCompletion(int caretOffset)
        {
            var keywordBehindOffset = FindKeywordWordBehindOffset(Editor.Text, caretOffset);

            if (keywordBehindOffset == null) return false;

            //prune out a full parse up to the cursor using context clues, the word behind the cursor.
            //right now, typing stuff after these keywords should not result in a suggestion
            switch (keywordBehindOffset)
            {
                case "integer":
                case "float":
                case "string":
                case "vector":
                case "rotation":
                case "key":
                case "list":
                case "default":
                    return true;

                default:
                    return false;
            }
        }


        // ReSharper disable once FunctionComplexityOverflow
        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (_symbolHoverToolTip.IsOpen)
            {
                _symbolHoverToolTip.IsOpen = false;
            }

 
            if (string.IsNullOrWhiteSpace(e.Text) || e.Text == ".")
            {
                lock (_completionLock)
                {
                    if (CurrentCompletionWindow == null) return;

                    CurrentCompletionWindow.Close();
                    CurrentCompletionWindow = null;
                }
                return;
            }


            var textArea = Editor.TextArea;
            var caretOffset = textArea.Caret.Offset;

            

            if (textArea.Selection.Length > 0)
            {
                int selectionStartOffset = textArea.Document.GetOffset(textArea.Selection.StartPosition.Location);
                int selectionEndOffset = textArea.Document.GetOffset(textArea.Selection.EndPosition.Location);

                var characterAfterSelection= LookAheadCaretOffset(Math.Max(selectionStartOffset,selectionEndOffset), 0, 1);

                if (!_validSuggestionSuffixes.Contains(characterAfterSelection))
                {
                    return;
                }

                if (selectionStartOffset < caretOffset)
                {
                    caretOffset = selectionStartOffset;
                }

            }


            if (DoAutoDedentOnTextEntering(e, caretOffset)) return;


            if (_validSuggestionPrefixes.Contains(e.Text))
            {
                lock (_completionLock)
                {
                    if (CurrentCompletionWindow == null) return;

                    CurrentCompletionWindow.Close();
                    CurrentCompletionWindow = null;
                }
                return;
            }


            lock (_completionLock)
            {
                if (CurrentCompletionWindow != null)
                {
                    return;
                }
            }


            var behind = LookBehindCaretOffset(caretOffset, 1, 1);


            if (!_validSuggestionPrefixes.Contains(behind))
            {
                return;
            }


            lock (_completionLock)
            {
                var commentSkipper = new LSLCommentStringSkipper(Editor.Text, caretOffset);


                if (commentSkipper.InComment || commentSkipper.InString || KeywordPriorBlocksCompletion(caretOffset))
                {
                    return;
                }

                var fastVarParser = new LSLAutoCompleteParser();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset-1);

#if DEBUG_AUTOCOMPLETE_PARSER
    _debugObjectView.ViewObject("", fastVarParser);
#endif

                IList<ICompletionData> data = null;





                if (TryCompletionForEventHandler(e.Text, fastVarParser, ref data)) return;


                if (TryCompletionForStateName(e.Text, fastVarParser, ref data)) return;


                if (TryCompletionForLabelNameJumpTarget(e.Text, fastVarParser, ref data)) return;


                if (TryCompletionForLabelNameDefinition(e.Text, fastVarParser, ref data)) return;


                var possibleType = TryCompletionForTypeName(e.Text, fastVarParser, ref data);


                var possibleControlStruct = TryCompletionForControlStatement(e.Text, fastVarParser, ref data);


                var possibleUserDefinedItem = TryCompletionForUserGlobalVariable(e.Text, fastVarParser, ref data);


                possibleUserDefinedItem |= TryCompletionForUserDefinedFunction(e.Text, fastVarParser, ref data);


                possibleUserDefinedItem |= TryCompletionForLocalVariableOrParameter(e.Text, fastVarParser, ref data);


                var possibleConstant = TryCompletionForLibraryConstant(e.Text, fastVarParser, ref data);


                var possibleLibraryFunction = TryCompletionForLibraryFunction(e.Text, fastVarParser, ref data);


                if (!possibleConstant
                    && !possibleLibraryFunction
                    && !possibleType
                    && !possibleUserDefinedItem
                    && !possibleControlStruct)
                {
                    CurrentCompletionWindow = null;
                    return;
                }

                if (CurrentCompletionWindow != null) CurrentCompletionWindow.Show();
            }
        }



        private string ToLowerIfCaseInsensitiveComplete(string input)
        {
            return Settings.CaseInsensitiveAutoCompleteMatching ? input.ToLower() : input;
        }


        private bool TryCompletionForLibraryFunction(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestFunction) return false;
            if (insertedText.Length == 1 && !LSLTokenTools.IDStartCharRegex.IsMatch(insertedText)) return false;

            var possibleLibraryFunction = false;


            insertedText = ToLowerIfCaseInsensitiveComplete(insertedText);

            var functions =
                LibraryFunctionNames
                .Where(x => ToLowerIfCaseInsensitiveComplete(x).StartsWith(insertedText))
                .OrderBy(x => x.Length);

            foreach (var func in functions)
            {
                if (!possibleLibraryFunction)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleLibraryFunction = true;
                }


                var completionData = CreateCompletionData_LibraryFunction(func, fastVarParser);
                completionData.Priority = -data.Count;
                data.Add(completionData);
            }

            return possibleLibraryFunction;
        }


        private bool TryCompletionForLibraryFunction(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestFunction) return false;

            var possibleGlobalFunctions = false;

            IList<ICompletionData> data = null;

            foreach (var func in LibraryFunctionNames.OrderBy(x => x.Length))
            {
                if (!possibleGlobalFunctions)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleGlobalFunctions = true;
                }

                var cdata = CreateCompletionData_LibraryFunction(func, fastVarParser);

                cdata.Priority = -CurrentCompletionWindow.CompletionList.CompletionData.Count;
                data.Add(cdata);
            }
            return possibleGlobalFunctions;
        }


        private bool TryCompletionForLibraryConstant(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestLibraryConstant) return false;
            if (insertedText.Length == 1 && !LSLTokenTools.IDStartCharRegex.IsMatch(insertedText)) return false;

            var possibleConstant = false;


            IEnumerable<LSLLibraryConstantSignature> constants;

            if (!Settings.ConstantCompletionFirstCharIsCaseSensitive)
            {
                insertedText = ToLowerIfCaseInsensitiveComplete(insertedText);

                constants =
                    ConstantSignatures
                        .Where(x => ToLowerIfCaseInsensitiveComplete(x.Name).StartsWith(insertedText));
            }
            else
            {
                constants = ConstantSignatures.Where(x => x.Name.StartsWith(insertedText));
            }

            foreach (var sig in constants.OrderBy(x=>x.Name.Length))
            {
                if (!possibleConstant)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleConstant = true;
                }


                var cdata = CreateCompletionData_Constant(sig);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }
            return possibleConstant;
        }


        private bool TryCompletionForLocalVariableOrParameter(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestLocalVariableOrParameter) return false;
            if (insertedText.Length == 1 && !LSLTokenTools.IDStartCharRegex.IsMatch(insertedText)) return false;

            var possibleUserDefinedItem = false;



            insertedText = ToLowerIfCaseInsensitiveComplete(insertedText);

            var parameters = fastVarParser
                .LocalParameters
                .Where(x => ToLowerIfCaseInsensitiveComplete(x.Name).StartsWith(insertedText))
                .OrderBy(x => x.Name.Length);

            foreach (var v in parameters)
            {
                if (!possibleUserDefinedItem)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleUserDefinedItem = true;
                }


                var cdata = CreateCompletionData_LocalParameter(v, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            var variables = fastVarParser
                .LocalVariables
                .Where(x => ToLowerIfCaseInsensitiveComplete(x.Name).StartsWith(insertedText))
                .OrderBy(x => x.Name.Length);

            foreach (var v in variables)
            {
                if (!possibleUserDefinedItem)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleUserDefinedItem = true;
                }

                var cdata = CreateCompletionData_LocalVariable(v, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }


            return possibleUserDefinedItem;
        }


        private bool TryCompletionForUserDefinedFunction(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestFunction) return false;
            if (insertedText.Length == 1 && !LSLTokenTools.IDStartCharRegex.IsMatch(insertedText)) return false;

            var possibleUserDefinedItem = false;

            insertedText = ToLowerIfCaseInsensitiveComplete(insertedText);

            var functions = fastVarParser
                .GlobalFunctions
                .Where(x => ToLowerIfCaseInsensitiveComplete(x.Name).StartsWith(insertedText))
                .OrderBy(x=>x.Name.Length);

            foreach (var func in functions)
            {
                if (!possibleUserDefinedItem)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleUserDefinedItem = true;
                }

                var cdata = CreateCompletionData_GlobalUserFunction(func, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }
            return possibleUserDefinedItem;
        }



        private bool TryCompletionForUserGlobalVariable(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestGlobalVariable) return false;
            if (insertedText.Length == 1 && !LSLTokenTools.IDStartCharRegex.IsMatch(insertedText)) return false;

            var possibleUserDefinedItem = false;


            insertedText = ToLowerIfCaseInsensitiveComplete(insertedText);

            var variables = fastVarParser
                .GlobalVariables
                .Where(x => ToLowerIfCaseInsensitiveComplete(x.Name).StartsWith(insertedText))
                .OrderBy(x=>x.Name.Length);

            foreach (var v in variables)
            {
                if (!possibleUserDefinedItem)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleUserDefinedItem = true;
                }

                var cdata = CreateCompletionData_GlobalUserVariable(v, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            return possibleUserDefinedItem;
        }


        private bool TryCompletionForControlStatement(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            var possibleControlStruct = false;
            if (fastVarParser.CanSuggestControlStatement)
            {

                if (insertedText.StartsWith("i"))
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;

                    data.Add(CreateCompletionData_IfStatement(fastVarParser.ScopeAddressAtOffset.ScopeLevel,
                        fastVarParser));
                    possibleControlStruct = true;
                }
                else if (insertedText.StartsWith("e") && fastVarParser.AfterIfOrElseIfStatement)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;

                    data.Add(CreateCompletionData_ElseStatement(fastVarParser.ScopeAddressAtOffset.ScopeLevel,
                        fastVarParser));
                    data.Add(CreateCompletionData_ElseIfStatement(fastVarParser.ScopeAddressAtOffset.ScopeLevel,
                        fastVarParser));
                    possibleControlStruct = true;
                }
                else if (insertedText.StartsWith("w"))
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;

                    data.Add(CreateCompletionData_WhileStatement(fastVarParser.ScopeAddressAtOffset.ScopeLevel,
                        fastVarParser));
                    possibleControlStruct = true;
                }
                else if (insertedText.StartsWith("d"))
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;

                    data.Add(CreateCompletionData_DoStatement(fastVarParser.ScopeAddressAtOffset.ScopeLevel,
                        fastVarParser));
                    possibleControlStruct = true;
                }
                else if (insertedText.StartsWith("f"))
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;

                    data.Add(CreateCompletionData_ForStatement(fastVarParser.ScopeAddressAtOffset.ScopeLevel,
                        fastVarParser));
                    possibleControlStruct = true;
                }
            }


            if (fastVarParser.CanSuggestJumpStatement && insertedText.StartsWith("j"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateCompletionData_JumpStatement(fastVarParser));
                possibleControlStruct = true;
            }
            else if (fastVarParser.CanSuggestReturnStatement && insertedText.StartsWith("r"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;
                data.Add(CreateCompletionData_ReturnStatement(fastVarParser));
                possibleControlStruct = true;
            }
            else if (fastVarParser.CanSuggestStateChangeStatement && insertedText.StartsWith("s"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;
                data.Add(CreateCompletionData_StateChangeStatement(fastVarParser));
                possibleControlStruct = true;
            }

            return possibleControlStruct;
        }


        private bool TryCompletionForTypeName(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestTypeName) return false;

            var possibleType = false;


            if (insertedText.StartsWith("i"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateCompletionData_Type(LSLType.Integer));
                possibleType = true;
            }
            else if (insertedText.StartsWith("s"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateCompletionData_Type(LSLType.String));
                possibleType = true;
            }
            else if (insertedText.StartsWith("v"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateCompletionData_Type(LSLType.Vector));
                possibleType = true;
            }
            else if (insertedText.StartsWith("r"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateCompletionData_Type(LSLType.Rotation));
                possibleType = true;
            }
            else if (insertedText.StartsWith("k"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateCompletionData_Type(LSLType.Key));
                possibleType = true;
            }
            else if (insertedText.StartsWith("f"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateCompletionData_Type(LSLType.Float));
                possibleType = true;
            }
            else if (insertedText.StartsWith("l"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateCompletionData_Type(LSLType.List));
                possibleType = true;
            }
            else if (insertedText.StartsWith("q"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateCompletionData_Type("quaternion"));
                possibleType = true;
            }
            return possibleType;
        }


        private bool TryCompletionForLabelNameDefinition(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestLabelNameDefinition) return false;
            if (insertedText.Length == 1 && !LSLTokenTools.IDStartCharRegex.IsMatch(insertedText)) return false;


            var possibleLabelName = false;

            insertedText = ToLowerIfCaseInsensitiveComplete(insertedText);

            var localJumps = fastVarParser
                .GetLocalJumps(Editor.Text)
                .Where(x=>ToLowerIfCaseInsensitiveComplete(x.TargetName).StartsWith(insertedText))
                .OrderBy(x=>x.TargetName.Length);

            foreach (var label in localJumps)
            {
                if (!possibleLabelName)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleLabelName = true;
                }

                var cdata = CreateCompletionData_LabelDefinition(label, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            if (!possibleLabelName)
            {
                CurrentCompletionWindow = null;
                return true;
            }

            CurrentCompletionWindow.Show();
            return true;
        }



        private bool TryCompletionForLabelNameJumpTarget(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestLabelNameJumpTarget) return false;
            if (insertedText.Length == 1 && !LSLTokenTools.IDStartCharRegex.IsMatch(insertedText)) return false;


            var possibleLabelName = false;

            insertedText = ToLowerIfCaseInsensitiveComplete(insertedText);

            var localLabels = fastVarParser
                .GetLocalLabels(Editor.Text)
                .Where(x => ToLowerIfCaseInsensitiveComplete(x.Name).StartsWith(insertedText))
                .OrderBy(x => x.Name.Length);


            foreach (var label in localLabels)
            {
                if (!possibleLabelName)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleLabelName = true;
                }

                var cdata = CreateCompletionData_LabelJumpTarget(label, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            if (!possibleLabelName)
            {
                CurrentCompletionWindow = null;
                return true;
            }

            CurrentCompletionWindow.Show();

            return true;
        }


        private bool TryCompletionForStateName(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestStateName) return false;
            if (insertedText.Length == 1 && !LSLTokenTools.IDStartCharRegex.IsMatch(insertedText)) return false;


            if (fastVarParser.StateBlocks.Count == 0 && !insertedText.StartsWith("d")) return false;


            CurrentCompletionWindow = LazyInitCompletionWindow();
            data = CurrentCompletionWindow.CompletionList.CompletionData;


            data.Add(CreateCompletionData_DefaultStateName(fastVarParser));


            insertedText = ToLowerIfCaseInsensitiveComplete(insertedText);

            var states = fastVarParser
                .StateBlocks
                .Where(x => ToLowerIfCaseInsensitiveComplete(x.Name).StartsWith(insertedText))
                .OrderBy(x => x.Name.Length);

            foreach (var state in states)
            {
                var cdata = CreateCompletionData_StateName(state, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            CurrentCompletionWindow.Show();
            return true;
        }


        private bool TryCompletionForEventHandler(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestEventHandler) return false;
            if (insertedText.Length == 1 && !LSLTokenTools.IDStartCharRegex.IsMatch(insertedText)) return false;


            var possibleEventName = false;


            insertedText = ToLowerIfCaseInsensitiveComplete(insertedText);

            var events = EventSignatures
                .Where(x => ToLowerIfCaseInsensitiveComplete(x.Name).StartsWith(insertedText))
                .OrderBy(x => x.Name.Length);

            foreach (var eventHandler in events)
            {
                if (!possibleEventName)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleEventName = true;
                }

                var cdata = CreateCompletionData_EventHandler(eventHandler);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            if (!possibleEventName)
            {
                CurrentCompletionWindow = null;
                return true;
            }

            CurrentCompletionWindow.Show();
            return true;
        }


        private LSLCompletionData CreateCompletionData_DefaultStateName(LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData("default", "default", 0)
            {
                AppendOnInsert = ";",
                ColorBrush = Theme.CompletionWindowItemBrushes.StateNameBrush,
                DescriptionFactory = CreateDescriptionTextBlock_DefaultState
            };

            var offset = autoCompleteParser.ParseToOffset;

            while (true)
            {
                var c = Editor.Text[offset];
                var b = (char.IsWhiteSpace(c) || LSLTokenTools.IDAnyCharRegex.IsMatch(c.ToString())) && c != '\n' && c != '\r';

                if (!b && c == ';')
                {
                    data.AppendOnInsert = "";
                    data.OffsetCaretRelativeToDocument = true;
                    data.OffsetCaretAfterInsert = true;
                    data.CaretOffsetAfterInsert = offset + 8;
                    break;
                }
                if (!b)
                {
                    break;
                }

                offset++;
            }

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_DefaultState()
        {
            var description = new TextBlock();
            description.Inlines.Add(CreateHighlightedRunFromXshd("State", "default"));
            description.Inlines.Add(" script state");
            return description;
        }


        private TextBlock CreateDescriptionTextBlock_DefinedState(LSLAutoCompleteStateBlock state)
        {
            var description = new TextBlock();
            description.Inlines.Add(new Run(state.Name) {FontWeight = FontWeights.Bold});
            description.Inlines.Add(" script state");
            return description;
        }


        private LSLCompletionData CreateCompletionData_StateName(LSLAutoCompleteStateBlock state,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(state.Name, state.Name, 0)
            {
                AppendOnInsert = ";",
                ColorBrush = Theme.CompletionWindowItemBrushes.StateNameBrush,
                DescriptionFactory = ()=>CreateDescriptionTextBlock_DefinedState(state)
            };


            var offset = autoCompleteParser.ParseToOffset;


            while (true)
            {
                var c = Editor.Text[offset];
                var b = (char.IsWhiteSpace(c) || LSLTokenTools.IDAnyCharRegex.IsMatch(c.ToString())) && c != '\n' && c != '\r';

                if (!b && c == ';')
                {
                    data.AppendOnInsert = "";
                    data.OffsetCaretRelativeToDocument = true;
                    data.OffsetCaretAfterInsert = true;
                    data.CaretOffsetAfterInsert = offset + state.Name.Length + 1;
                    break;
                }
                if (!b)
                {
                    break;
                }

                offset++;
            }

            return data;
        }


        private LSLCompletionData CreateCompletionData_LabelDefinition(LSLAutoCompleteLocalJump label,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(label.TargetName, label.TargetName, 0)
            {
                AppendOnInsert = ";",
                ColorBrush = Theme.CompletionWindowItemBrushes.LabelNameDefinitionBrush,
                DescriptionFactory = ()=> CreateDescriptionTextBlock_LabelDefinition()
            };

            var offset = autoCompleteParser.ParseToOffset;

            while (true)
            {
                var c = Editor.Text[offset];
                var b = (char.IsWhiteSpace(c) || LSLTokenTools.IDAnyCharRegex.IsMatch(c.ToString())) && c != '\n' && c != '\r';

                if (!b && c == ';')
                {
                    data.AppendOnInsert = "";
                    data.OffsetCaretRelativeToDocument = true;
                    data.OffsetCaretAfterInsert = true;
                    data.CaretOffsetAfterInsert = offset + label.TargetName.Length + 1;
                    break;
                }
                if (!b)
                {
                    break;
                }

                offset++;
            }

            return data;
        }

        private static TextBlock CreateDescriptionTextBlock_LabelDefinition()
        {
            return new TextBlock {Text = "In scope jump"};
        }


        private LSLCompletionData CreateCompletionData_LabelJumpTarget(LSLAutoCompleteLocalLabel label,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(label.Name, label.Name, 0)
            {
                AppendOnInsert = ";",
                ColorBrush = Theme.CompletionWindowItemBrushes.LabelNameJumpTargetBrush,
                DescriptionFactory = () => CreateDescriptionTextBlock_LabelJumpTarget()
            };

            var offset = autoCompleteParser.ParseToOffset;

            while (true)
            {
                var c = Editor.Text[offset];
                var b = (char.IsWhiteSpace(c) || LSLTokenTools.IDAnyCharRegex.IsMatch(c.ToString())) && c != '\n' && c != '\r';

                if (!b && c == ';')
                {
                    data.AppendOnInsert = "";
                    data.OffsetCaretRelativeToDocument = true;
                    data.OffsetCaretAfterInsert = true;
                    data.CaretOffsetAfterInsert = offset + label.Name.Length + 1;
                    break;
                }
                if (!b)
                {
                    break;
                }

                offset++;
            }

            return data;
        }

        private static TextBlock CreateDescriptionTextBlock_LabelJumpTarget()
        {
            return new TextBlock {Text = "In scope label"};
        }


        private LSLCompletionData CreateCompletionData_GlobalUserVariable(LSLAutoCompleteGlobalVariable v,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(v.Name, v.Name, 1)
            {
                ColorBrush = Theme.CompletionWindowItemBrushes.GlobalVariableBrush,
                DescriptionFactory = () => CreateGlobalVariableDescriptionTextBlock(v)
            };

            if (!autoCompleteParser.InBracelessCodeStatementArea) return data;


            data.ForceIndent = true;

            data.IndentBreakCharacters = _singleBlockControlStatementIndentBreakTriggers;

            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }


        private TextBlock CreateGlobalVariableDescriptionTextBlock(LSLAutoCompleteGlobalVariable v)
        {
            var description = new TextBlock();

            description.Inlines.Add(new Run("Global Variable:" + LSLFormatTools.CreateNewLinesString(2))
            {
                FontWeight = FontWeights.Bold
            });

            description.Inlines.Add(CreateHighlightedRunFromXshd("Type", v.TypeString));
            description.Inlines.Add(" " + v.Name);
            return description;
        }




        private LSLCompletionData CreateCompletionData_Constant(LSLLibraryConstantSignature sig)
        {
            var data = new LSLCompletionData(sig.Name, sig.Name, 5)
            {
                ColorBrush = Theme.CompletionWindowItemBrushes.LibraryConstantBrush,
                DescriptionFactory = () => CreateDescriptionTextBlock_LibraryConstant(sig)
            };


            return data;
        }

        private TextBlock CreateDescriptionTextBlock_LibraryConstant(LSLLibraryConstantSignature sig)
        {
            var description = new TextBlock();
            description.TextWrapping = TextWrapping.Wrap;
            description.MaxWidth = 500;

            description.Inlines.Add(new Run("Library Constant:" + LSLFormatTools.CreateNewLinesString(2))
            {
                FontWeight = FontWeights.Bold
            });

            description.Inlines.Add(CreateHighlightedRunFromXshd("Type", sig.Type.ToLSLTypeString()+" "));
            description.Inlines.Add(CreateHighlightedRunFromXshd("Constant", sig.Name));
            description.Inlines.Add(" = ");
            description.Inlines.Add(sig.ValueStringAsCodeLiteral + ";");

            if (!string.IsNullOrWhiteSpace(sig.DocumentationString))
            {
                description.Inlines.Add(LSLFormatTools.CreateNewLinesString(2) + sig.DocumentationString);
            }

            return description;
        }


        private LSLCompletionData CreateCompletionData_LocalVariable(LSLAutoCompleteLocalVariable v,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(v.Name, v.Name, 4)
            {
                ColorBrush = Theme.CompletionWindowItemBrushes.LocalVariableBrush,
                DescriptionFactory = () => CreateDescriptionTextBlock_LocalVariable(v)
            };


            if (!autoCompleteParser.InBracelessCodeStatementArea) return data;

            data.ForceIndent = true;

            data.IndentBreakCharacters = _singleBlockControlStatementIndentBreakTriggers;

            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }


        private TextBlock CreateDescriptionTextBlock_LocalVariable(LSLAutoCompleteLocalVariable v)
        {
            var description = new TextBlock();

            description.Inlines.Add(new Run("Local Variable:" + LSLFormatTools.CreateNewLinesString(2))
            {
                FontWeight = FontWeights.Bold
            });

            description.Inlines.Add(CreateHighlightedRunFromXshd("Type", v.TypeString));
            description.Inlines.Add(" " + v.Name);
            return description;
        }


        private LSLCompletionData CreateCompletionData_LocalParameter(LSLAutoCompleteLocalParameter v,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(v.Name, v.Name, 3)
            {
                ColorBrush = Theme.CompletionWindowItemBrushes.LocalParameterBrush
            };


            data.DescriptionFactory = () => CreateDescriptionTextBlock_LocalParameter(v);


            if (!autoCompleteParser.InBracelessCodeStatementArea) return data;

            data.ForceIndent = true;

            data.IndentBreakCharacters = _singleBlockControlStatementIndentBreakTriggers;

            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_LocalParameter(LSLAutoCompleteLocalParameter v)
        {
            var description = new TextBlock();

            description.Inlines.Add(new Run("Local Parameter:" + LSLFormatTools.CreateNewLinesString(2))
            {
                FontWeight = FontWeights.Bold
            });

            description.Inlines.Add(CreateHighlightedRunFromXshd("Type", v.TypeString));
            description.Inlines.Add(" " + v.Name);
            return description;
        }



        private LSLCompletionData CreateCompletionData_GlobalUserFunction(LSLAutoCompleteGlobalFunction func,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var additiveEnding = autoCompleteParser.InCodeStatementArea
                ? ";"
                : "";

            var data = new LSLCompletionData(func.Name, func.Name, 2)
            {
                AppendOnInsert = "()" + additiveEnding,
                ColorBrush = Theme.CompletionWindowItemBrushes.GlobalFunctionBrush,
                DescriptionFactory = () => CreateDescriptionTextBlock_GlobalUserFunction(func)
            };


            if (func.Parameters.Count > 0)
            {
                data.OffsetCaretAfterInsert = true;
                data.CaretOffsetAfterInsert = -1 - additiveEnding.Length;
            }


            if (!autoCompleteParser.InBracelessCodeStatementArea) return data;

            data.ForceIndent = true;

            data.IndentBreakCharacters = _singleBlockControlStatementIndentBreakTriggers;

            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_GlobalUserFunction(LSLAutoCompleteGlobalFunction func)
        {
            var description = new TextBlock();

            var nameRun = new Run(func.Name)
            {
                FontWeight = FontWeights.Bold,
                Foreground = Theme.CompletionWindowItemBrushes.GlobalFunctionBrush
            };

            description.Inlines.Add(new Run("Global Function:" + LSLFormatTools.CreateNewLinesString(2))
            {
                FontWeight = FontWeights.Bold
            });

            if (func.HasReturnType)
            {
                description.Inlines.Add(CreateHighlightedRunFromXshd("Type", func.ReturnTypeString + " "));
            }

            description.Inlines.Add(nameRun);
            description.Inlines.Add(new Run("(") {FontWeight = FontWeights.Bold});

            var pIndex = 1;
            foreach (var param in func.Parameters)
            {
                description.Inlines.Add(CreateHighlightedRunFromXshd("Type", param.TypeString + " "));
                description.Inlines.Add(param.Name);
                if (pIndex < func.Parameters.Count)
                {
                    description.Inlines.Add(", ");
                }
                pIndex++;
            }

            description.Inlines.Add(new Run(")") {FontWeight = FontWeights.Bold});
            description.Inlines.Add(new Run(";"));
            return description;
        }

        private LSLCompletionData CreateCompletionData_LibraryFunction(string func, 
            LSLAutoCompleteParser autoCompleteParser)
        {
            var sigs = LibraryDataProvider.GetLibraryFunctionSignatures(func);

            var allOverloadsDeprecated = sigs.All(x => x.Deprecated);

            var colorBrush = Theme.CompletionWindowItemBrushes.LibraryFunctionBrush;

            if (allOverloadsDeprecated)
            {
                colorBrush = Theme.CompletionWindowItemBrushes.LibraryFunctionDeprecatedBrush;
            }

            var additiveEnding = autoCompleteParser.InCodeStatementArea
                ? ";"
                : "";

            var data = new LSLCompletionData(func, func, 6)
            {
                AppendOnInsert = "()" + additiveEnding,
                ColorBrush = colorBrush,
                DescriptionFactory = () => CreateDescriptionTextBlock_LibraryFunction(sigs)
            };


            if (sigs.Any(x => x.ParameterCount > 0))
            {
                data.OffsetCaretAfterInsert = true;
                data.CaretOffsetAfterInsert = -1 - additiveEnding.Length;
            }

            if (!autoCompleteParser.InBracelessCodeStatementArea) return data;

            data.ForceIndent = true;

            data.IndentBreakCharacters = _singleBlockControlStatementIndentBreakTriggers;

            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_LibraryFunction(
            IReadOnlyGenericArray<LSLLibraryFunctionSignature> funcOverloads)
        {
            var description = new TextBlock();

            description.TextWrapping = TextWrapping.Wrap;
            description.MaxWidth = 500;

            description.Inlines.Add(new Run("Library Function:" + LSLFormatTools.CreateNewLinesString(2))
            {
                FontWeight = FontWeights.Bold
            });

            var overloadCnt = 1;
            foreach (var func in funcOverloads)
            {
                var nameRun = new Run(func.Name)
                {
                    FontWeight = FontWeights.Bold
                };

                if (func.Deprecated)
                {
                    description.Inlines.Add(new Run("(DEPRECATED) ")
                    {
                        Foreground = new SolidColorBrush(Theme.ToolTipDeprecationMarkerColor),
                        FontWeight = FontWeights.Bold
                    });

                    nameRun.Foreground = new SolidColorBrush(Theme.HighlightingColors.LibraryFunctionDeprecatedColor);
                }
                else
                {
                    nameRun.Foreground = new SolidColorBrush(Theme.HighlightingColors.LibraryFunctionColor);
                }

                if (func.ReturnType != LSLType.Void)
                {
                    description.Inlines.Add(CreateHighlightedRunFromXshd("Type", func.ReturnType.ToLSLTypeString() + " "));
                }

                description.Inlines.Add(nameRun);
                description.Inlines.Add(new Run("(") {FontWeight = FontWeights.Bold});


                foreach (var param in func.ConcreteParameters)
                {
                    description.Inlines.Add(CreateHighlightedRunFromXshd("Type", param.Type.ToLSLTypeString() + " "));
                    description.Inlines.Add(param.Name);
                    if (param.ParameterIndex < func.ConcreteParameterCount-1)
                    {
                        description.Inlines.Add(", ");
                    }
                }

                if (func.HasVariadicParameter)
                {
                    if (func.ConcreteParameterCount != 0)
                    {
                        description.Inlines.Add(", ");

                    }
                    var variadicParameter = func.Parameters.Last();
                    var variadicParameterName = variadicParameter.Name;
                    var variadicParameterType = variadicParameter.Type;

                    var variadicParameterTypeName = 
                        variadicParameterType == LSLType.Void
                        ? "any"
                        : variadicParameterType.ToLSLTypeString();


                    description.Inlines.Add(new Run("params ") { FontWeight = FontWeights.Bold });
                    description.Inlines.Add(CreateHighlightedRunFromXshd("Type", variadicParameterTypeName));
                    description.Inlines.Add(new Run("[] ") { FontWeight = FontWeights.Bold });
                    description.Inlines.Add(variadicParameterName);
                }

                description.Inlines.Add(new Run(")") {FontWeight = FontWeights.Bold});
                description.Inlines.Add(new Run(";"));


                if (!string.IsNullOrWhiteSpace(func.DocumentationString))
                {
                    description.Inlines.Add(LSLFormatTools.CreateNewLinesString(2) + func.DocumentationString);
                }

                if (overloadCnt < funcOverloads.Count)
                {
                    description.Inlines.Add(LSLFormatTools.CreateNewLinesString(2));
                }

                overloadCnt++;
            }

            return description;
        }


        private LSLCompletionData CreateCompletionData_ForStatement(int scopeLevel,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData("for", "for(;;)", 0)
            {
                AppendOnInsert = (autoCompleteParser.InBracelessCodeStatementArea ? "" : "\n{\n}"),
                ColorBrush = Theme.CompletionWindowItemBrushes.ControlStatementBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 4,
                InsertTextAtCaretAfterOffset = false,
                DescriptionFactory = () => CreateDescriptionTextBlock_ForStatement()
            };

            data.IndentBreakCharacters = autoCompleteParser.InBracelessCodeStatementArea ?
                _singleBlockControlStatementIndentBreakTriggers : _controlStatementIndentBreakTriggers;

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_ForStatement()
        {
            var description = new TextBlock();

            var typeRun = CreateHighlightedRunFromXshd("ControlFlow", "for");

            description.Inlines.Add(typeRun);
            description.Inlines.Add(" statement");
            return description;
        }

        private LSLCompletionData CreateCompletionData_WhileStatement(int scopeLevel,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData("while", "while()", 0)
            {
                AppendOnInsert = (autoCompleteParser.InBracelessCodeStatementArea ? "" : "\n{\n}"),
                ColorBrush = Theme.CompletionWindowItemBrushes.ControlStatementBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 6,
                InsertTextAtCaretAfterOffset = false,
                DescriptionFactory = () => CreateDescriptionTextBlock_WhileStatement()
            };

            data.IndentBreakCharacters = autoCompleteParser.InBracelessCodeStatementArea ?
                _singleBlockControlStatementIndentBreakTriggers : _controlStatementIndentBreakTriggers;

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_WhileStatement()
        {
            var description = new TextBlock();

            var typeRun = CreateHighlightedRunFromXshd("ControlFlow", "while");

            description.Inlines.Add(typeRun);
            description.Inlines.Add(" statement");
            return description;
        }


        private LSLCompletionData CreateCompletionData_DoStatement(int scopeLevel,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData("do", "do", 0)
            {
                AppendOnInsert = (autoCompleteParser.InBracelessCodeStatementArea ? " " : "\n{\n}\nwhile();"),
                ColorBrush = Theme.CompletionWindowItemBrushes.ControlStatementBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = (autoCompleteParser.InBracelessCodeStatementArea ? 0 : -2),
                InsertTextAtCaretAfterOffset = false,
                DescriptionFactory = () => CreateDescriptionTextBlock_DoStatement()
            };

            data.IndentBreakCharacters = autoCompleteParser.InBracelessCodeStatementArea ?
                _singleBlockControlStatementIndentBreakTriggers : _controlStatementIndentBreakTriggers;

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_DoStatement()
        {
            var description = new TextBlock();

            var typeRun = CreateHighlightedRunFromXshd("ControlFlow", "do");

            description.Inlines.Add(typeRun);
            description.Inlines.Add(" statement");
            return description;
        }

        private LSLCompletionData CreateCompletionData_IfStatement(int scopeLevel,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData("if", "if()", 0)
            {
                AppendOnInsert = (autoCompleteParser.InBracelessCodeStatementArea ? "" : "\n{\n}"),
                ColorBrush = Theme.CompletionWindowItemBrushes.ControlStatementBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 3,
                InsertTextAtCaretAfterOffset = false,
                DescriptionFactory = () => CreateDescriptionTextBlock_IfStatement()
            };

            data.IndentBreakCharacters = autoCompleteParser.InBracelessCodeStatementArea ?
                _singleBlockControlStatementIndentBreakTriggers : _controlStatementIndentBreakTriggers;

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_IfStatement()
        {
            var description = new TextBlock();

            var typeRun = CreateHighlightedRunFromXshd("ControlFlow", "if");

            description.Inlines.Add(typeRun);
            description.Inlines.Add(" statement");
            return description;
        }

        private LSLCompletionData CreateCompletionData_ElseIfStatement(int scopeLevel,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData("else if", "else if()", 0)
            {
                AppendOnInsert =  "\n{\n}",
                ColorBrush = Theme.CompletionWindowItemBrushes.ControlStatementBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 8,
                InsertTextAtCaretAfterOffset = false,

                DescriptionFactory = () => CreateDescriptionTextBlock_ElseIfStatement()
            };

            data.IndentBreakCharacters =  _controlStatementIndentBreakTriggers;

            return data;
        }


        private TextBlock CreateDescriptionTextBlock_ElseIfStatement()
        {
            var description = new TextBlock();

            var typeRun = CreateHighlightedRunFromXshd("ControlFlow", "else if");

            description.Inlines.Add(typeRun);
            description.Inlines.Add(" statement");
            return description;
        }


        private LSLCompletionData CreateCompletionData_ElseStatement(int scopeLevel,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData("else", "else", 0)
            {
                AppendOnInsert = "\n{\n\t\n}",
                ColorBrush = Theme.CompletionWindowItemBrushes.ControlStatementBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 8,
                InsertTextAtCaretAfterOffset = false,
                DescriptionFactory = () => CreateDescriptionTextBlock_ElseStatement()
            };

            data.IndentBreakCharacters =  _controlStatementIndentBreakTriggers;

            return data;
        }


        private TextBlock CreateDescriptionTextBlock_ElseStatement()
        {
            var description = new TextBlock();

            var typeRun = CreateHighlightedRunFromXshd("ControlFlow", "else");

            description.Inlines.Add(typeRun);
            description.Inlines.Add(" statement");
            return description;
        }


        private LSLCompletionData CreateCompletionData_ReturnStatement(LSLAutoCompleteParser autoCompleteParser)
        {
            var inReturningFunction = autoCompleteParser.InFunctionCodeBody &&
                                      autoCompleteParser.CurrentFunctionReturnType != LSLType.Void;

            var data = new LSLCompletionData("return", "return", 0)
            {
                AppendOnInsert = inReturningFunction ? " ;" : ";",
                ColorBrush = Theme.CompletionWindowItemBrushes.ControlStatementBrush,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 7,
                InsertTextAtCaretAfterOffset = false,
                DescriptionFactory = () => CreateDescriptionTextBlock_ReturnStatement()
            };

            if (!autoCompleteParser.InBracelessCodeStatementArea) return data;

            data.ForceIndent = true;

            data.IndentBreakCharacters = _singleBlockControlStatementIndentBreakTriggers;

            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_ReturnStatement()
        {
            var description = new TextBlock();

            var typeRun = CreateHighlightedRunFromXshd("ControlFlow", "return");

            description.Inlines.Add(typeRun);
            description.Inlines.Add(" statement");
            return description;
        }


        private LSLCompletionData CreateCompletionData_JumpStatement(LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData("jump", "jump", 0)
            {
                AppendOnInsert = " ;",
                ColorBrush = Theme.CompletionWindowItemBrushes.ControlStatementBrush,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 5,
                InsertTextAtCaretAfterOffset = false,
                DescriptionFactory = () => CreateDescriptionTextBlock_JumpStatement()
            };

            if (!autoCompleteParser.InBracelessCodeStatementArea) return data;

            data.ForceIndent = true;

            data.IndentBreakCharacters = _singleBlockControlStatementIndentBreakTriggers;

            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_JumpStatement()
        {
            var description = new TextBlock();

            var typeRun = CreateHighlightedRunFromXshd("ControlFlow", "jump");

            description.Inlines.Add(typeRun);
            description.Inlines.Add(" statement");
            return description;
        }


        private LSLCompletionData CreateCompletionData_StateChangeStatement(LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData("state", "state", 0)
            {
                AppendOnInsert = " ;",
                ColorBrush = Theme.CompletionWindowItemBrushes.ControlStatementBrush,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 6,
                InsertTextAtCaretAfterOffset = false,
                DescriptionFactory = () => CreateDescriptionTextBlock_StateChangeStatment()
            };

            if (!autoCompleteParser.InBracelessCodeStatementArea) return data;

            data.ForceIndent = true;

            data.IndentBreakCharacters = _singleBlockControlStatementIndentBreakTriggers;

            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;


            return data;
        }

        private TextBlock CreateDescriptionTextBlock_StateChangeStatment()
        {
            var description = new TextBlock();

            var typeRun = CreateHighlightedRunFromXshd("State", "state");

            description.Inlines.Add(typeRun);
            description.Inlines.Add(" change statement");
            return description;
        }


        private LSLCompletionData CreateCompletionData_Type(LSLType type)
        {

            return CreateCompletionData_Type(type.ToLSLTypeString());
        }

        private LSLCompletionData CreateCompletionData_Type(string typeString)
        {

            var desc = new TextBlock();

            var typeRun = CreateHighlightedRunFromXshd("Type", typeString);


            desc.Inlines.Add(typeRun);
            desc.Inlines.Add(" type");


            return new LSLCompletionData(typeString, typeString, 0)
            {
                ColorBrush = Theme.CompletionWindowItemBrushes.TypeBrush,
                DescriptionFactory = () => desc
            };
        }


        private Run CreateHighlightedRunFromXshd(string xshdColorName, string text)
        {
            var typeHighlightColor = Editor.SyntaxHighlighting.GetNamedColor(xshdColorName);

            var typeRun = new Run(text);

            if (typeHighlightColor.FontStyle != null) typeRun.FontStyle = typeHighlightColor.FontStyle.Value;
            if (typeHighlightColor.FontWeight != null) typeRun.FontWeight = typeHighlightColor.FontWeight.Value;


            try
            {
                //it will work, the parameter is unused in Avalon edit, I have no idea why its even in the API 
                //of this function call
                typeRun.Foreground = typeHighlightColor.Foreground.GetBrush(null);
            }
            catch
            {
                // ignored
            }
            return typeRun;
        }


        private LSLCompletionData CreateCompletionData_EventHandler(LSLLibraryEventSignature eventHandler)
        {
            var parameters = eventHandler.SignatureString.Substring(eventHandler.Name.Length);

            var stateCompletionData = new LSLCompletionData(
                eventHandler.Name,
                eventHandler.Name, 0)
            {
                AppendOnInsert = parameters + "\n{\n\t\n}",
                ColorBrush = Theme.CompletionWindowItemBrushes.EventHandlerBrush,
                ForceIndent = true,
                IndentLevel = 1,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = eventHandler.Name.Length + parameters.Length + 4,
                InsertTextAtCaretAfterOffset = true,
                IndentBreakCharacters = _eventIndentBreakTriggers,
                DescriptionFactory = () => CreateDescriptionTextBlock_EventHandler(eventHandler)
            };

            return stateCompletionData;
        }


        private TextBlock CreateDescriptionTextBlock_EventHandler(LSLLibraryEventSignature eventHandler)
        {
            var description = new TextBlock();

            description.TextWrapping = TextWrapping.Wrap;
            description.MaxWidth = 500;

            description.Inlines.Add(new Run("Event Handler:" + LSLFormatTools.CreateNewLinesString(2))
            {
                FontWeight = FontWeights.Bold
            });


            var nameRun = new Run(eventHandler.Name)
            {
                FontWeight = FontWeights.Bold,
                Foreground = Theme.CompletionWindowItemBrushes.EventHandlerBrush
            };

            description.Inlines.Add(nameRun);
            description.Inlines.Add(new Run("(") {FontWeight = FontWeights.Bold});


            var pIndex = 1;
            foreach (var param in eventHandler.Parameters)
            {
                description.Inlines.Add(CreateHighlightedRunFromXshd("Type", param.Type.ToLSLTypeString() + " "));
                description.Inlines.Add(param.Name);
                if (pIndex < eventHandler.ParameterCount)
                {
                    description.Inlines.Add(", ");
                }
                pIndex++;
            }

            description.Inlines.Add(new Run(")") {FontWeight = FontWeights.Bold});
            description.Inlines.Add(new Run(";"));

            if (!string.IsNullOrWhiteSpace(eventHandler.DocumentationString))
            {
                description.Inlines.Add(LSLFormatTools.CreateNewLinesString(2) + eventHandler.DocumentationString);
            }

            return description;
        }


        private void SuggestUserDefinedOrEvent()
        {
            if (_symbolHoverToolTip.IsOpen)
            {
                _symbolHoverToolTip.IsOpen = false;
            }

            var textArea = Editor.TextArea;
            var caretOffset = textArea.Caret.Offset;


            lock (_completionLock)
            {
                if (CurrentCompletionWindow != null)
                {
                    if (CurrentCompletionWindow.CompletionList.ListBox.Items.Count == 0)
                    {
                        CurrentCompletionWindow.Close();
                        CurrentCompletionWindow = null;
                    }
                    else
                    {
                        return;
                    }
                }

                var behind = LookBehindCaretOffset(caretOffset, 1, 1);


                if (!_validSuggestionPrefixes.Contains(behind)) return;


                var commentSkipper = new LSLCommentStringSkipper(Editor.Text, caretOffset);

                if (commentSkipper.InComment || commentSkipper.InString || KeywordPriorBlocksCompletion(caretOffset))
                {
                    return;
                }


                var fastVarParser = new LSLAutoCompleteParser();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);


                if (TryCompletionForEventHandler(fastVarParser)) return;


                if (TryCompletionForStateName(fastVarParser)) return;


                if (TryCompletionForLabelNameJumpTarget(fastVarParser)) return;


                if (TryCompletionForLabelNameDefinition(fastVarParser)) return;


                var possibleUserDefinedItem = TryCompletionForUserGlobalVariable(fastVarParser);


                possibleUserDefinedItem |= TryCompletionForUserDefinedFunction(fastVarParser);

                possibleUserDefinedItem |= TryCompletionForLocalVariableOrParameter(fastVarParser);


                if (!possibleUserDefinedItem)
                {
                    CurrentCompletionWindow = null;
                    return;
                }

                if (CurrentCompletionWindow != null) CurrentCompletionWindow.Show();
            }
        }


        private bool TryCompletionForLocalVariableOrParameter(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestLocalVariableOrParameter) return false;

            var possibleUserDefinedItem = false;

            IList<ICompletionData> data = null;


            foreach (var i in fastVarParser.LocalParameters.OrderBy(x => x.Name.Length))
            {
                if (!possibleUserDefinedItem)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleUserDefinedItem = true;
                }

                var cdata = CreateCompletionData_LocalParameter(i, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            foreach (var i in fastVarParser.LocalVariables.OrderBy(x => x.Name.Length))
            {
                if (!possibleUserDefinedItem)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleUserDefinedItem = true;
                }

                var cdata = CreateCompletionData_LocalVariable(i, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            return possibleUserDefinedItem;
        }


        private bool TryCompletionForUserDefinedFunction(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestFunction) return false;

            var possibleUserDefinedItem = false;

            IList<ICompletionData> data = null;


            foreach (var i in fastVarParser.GlobalFunctions.OrderBy(x => x.Name.Length))
            {
                if (!possibleUserDefinedItem)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleUserDefinedItem = true;
                }

                var cdata = CreateCompletionData_GlobalUserFunction(i, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            return possibleUserDefinedItem;
        }

        private bool TryCompletionForUserGlobalVariable(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestGlobalVariable) return false;

            var possibleUserDefinedItem = false;

            IList<ICompletionData> data = null;

            foreach (var i in fastVarParser.GlobalVariables.OrderBy(x => x.Name.Length))
            {
                if (!possibleUserDefinedItem)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleUserDefinedItem = true;
                }


                var cdata = CreateCompletionData_GlobalUserVariable(i, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }
            return possibleUserDefinedItem;
        }


        private bool TryCompletionForLabelNameDefinition(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestLabelNameDefinition) return false;


            IList<ICompletionData> data = null;

            var possibleLabelName = false;

            foreach (var label in fastVarParser.GetLocalJumps(Editor.Text).OrderBy(x => x.TargetName.Length))
            {
                if (!possibleLabelName)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleLabelName = true;
                }


                var cdata = CreateCompletionData_LabelDefinition(label, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            if (!possibleLabelName)
            {
                CurrentCompletionWindow = null;
                return true;
            }

            CurrentCompletionWindow.Show();
            return true;
        }

        private bool TryCompletionForLabelNameJumpTarget(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestLabelNameJumpTarget) return false;

            IList<ICompletionData> data = null;

            var possibleLabelName = false;

            foreach (var label in fastVarParser.GetLocalLabels(Editor.Text).OrderBy(x => x.Name.Length))
            {
                if (!possibleLabelName)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleLabelName = true;
                }

                var cdata = CreateCompletionData_LabelJumpTarget(label, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            if (!possibleLabelName)
            {
                CurrentCompletionWindow = null;
                return true;
            }

            CurrentCompletionWindow.Show();
            return true;
        }

        private bool TryCompletionForStateName(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestStateName) return false;


            CurrentCompletionWindow = LazyInitCompletionWindow();
            var data = CurrentCompletionWindow.CompletionList.CompletionData;

            data.Add(CreateCompletionData_DefaultStateName(fastVarParser));

            foreach (var state in fastVarParser.StateBlocks.OrderBy(x => x.Name.Length))
            {
                var cdata = CreateCompletionData_StateName(state, fastVarParser);
                cdata.Priority = -data.Count;
                data.Add(cdata);

                data.Add(CreateCompletionData_StateName(state, fastVarParser));
            }

            CurrentCompletionWindow.Show();
            return true;
        }

        private bool TryCompletionForEventHandler(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestEventHandler) return false;

            IList<ICompletionData> data = null;

            var possibleEventName = false;

            foreach (var eventHandler in EventSignatures.OrderBy(x => x.Name.Length))
            {
                if (!possibleEventName)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleEventName = true;
                }

                var cdata = CreateCompletionData_EventHandler(eventHandler);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            if (!possibleEventName)
            {
                CurrentCompletionWindow = null;
                return true;
            }

            CurrentCompletionWindow.Show();
            return true;
        }


        private string LookBehindCaretOffset(int caretOffset, int behindOffset, int length)
        {
            return (caretOffset - behindOffset) > 0 ? Editor.Document.GetText(caretOffset - behindOffset, length) : "";
        }

        private string LookAheadCaretOffset(int caretOffset, int aheadOffset, int length)
        {
            return (caretOffset + aheadOffset + length) > Editor.Document.TextLength ? "" : Editor.Document.GetText(caretOffset + aheadOffset, length);
        }


        private void SuggestLibraryFunctions()
        {
            if (_symbolHoverToolTip.IsOpen)
            {
                _symbolHoverToolTip.IsOpen = false;
            }


            var textArea = Editor.TextArea;
            var caretOffset = textArea.Caret.Offset;


            lock (_completionLock)
            {
                if (CurrentCompletionWindow != null)
                {
                    if (CurrentCompletionWindow.CompletionList.ListBox.Items.Count == 0)
                    {
                        CurrentCompletionWindow.Close();
                        CurrentCompletionWindow = null;
                    }
                    else
                    {
                        return;
                    }
                }


                var behind = LookBehindCaretOffset(caretOffset, 1, 1);


                if (!_validSuggestionPrefixes.Contains(behind)) return;


                var commentSkipper = new LSLCommentStringSkipper(Editor.Text, caretOffset);


                if (commentSkipper.InComment || commentSkipper.InString || KeywordPriorBlocksCompletion(caretOffset))
                {
                    return;
                }

                var fastVarParser = new LSLAutoCompleteParser();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);


                var possibleGlobalFunctions = TryCompletionForLibraryFunction(fastVarParser);


                if (!possibleGlobalFunctions)
                {
                    CurrentCompletionWindow = null;
                    return;
                }

                if (CurrentCompletionWindow != null) CurrentCompletionWindow.Show();
            }
        }


        private void SuggestLibraryConstants()
        {
            if (_symbolHoverToolTip.IsOpen)
            {
                _symbolHoverToolTip.IsOpen = false;
            }


            var textArea = Editor.TextArea;
            var caretOffset = textArea.Caret.Offset;


            lock (_completionLock)
            {
                if (CurrentCompletionWindow != null)
                {
                    if (CurrentCompletionWindow.CompletionList.ListBox.Items.Count == 0)
                    {
                        CurrentCompletionWindow.Close();
                        CurrentCompletionWindow = null;
                    }
                    else
                    {
                        return;
                    }
                }

                var behind = LookBehindCaretOffset(caretOffset, 1, 1);


                if (!_validSuggestionPrefixes.Contains(behind)) return;


                var commentSkipper = new LSLCommentStringSkipper(Editor.Text, caretOffset);


                if (commentSkipper.InComment || commentSkipper.InString || KeywordPriorBlocksCompletion(caretOffset))
                {
                    return;
                }


                var fastVarParser = new LSLAutoCompleteParser();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);


                var possibleLibraryConstants = TryCompletionForLibraryConstant(fastVarParser);


                if (!possibleLibraryConstants)
                {
                    CurrentCompletionWindow = null;
                    return;
                }

                if (CurrentCompletionWindow != null) CurrentCompletionWindow.Show();
            }
        }


        private bool TryCompletionForLibraryConstant(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestLibraryConstant) return false;
            var possibleLibraryConstants = false;


            IList<ICompletionData> data = null;

            foreach (var con in ConstantSignatures.OrderBy(x => x.Name.Length))
            {
                if (!possibleLibraryConstants)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;
                    possibleLibraryConstants = true;
                }


                var cdata = CreateCompletionData_Constant(con);
                cdata.Priority = -data.Count;
                data.Add(cdata);
            }

            return possibleLibraryConstants;
        }



        public void UpdateHighlightingFromDataProvider()
        {
            UpdateHighlightingFromDataProvider(LibraryDataProvider, true);
        }





        private void UpdateHighlightingColorsFromSettings(bool forceVisualUpdate)
        {
            if (Editor.SyntaxHighlighting == null) return;

            
            Editor.TextArea.TextView.LinkTextForegroundBrush = new SolidColorBrush(Theme.HighlightingColors.UrlColor);


            foreach (var color in Editor.SyntaxHighlighting.NamedHighlightingColors)
            {
                switch (color.Name)
                {
                    case "String":
                        color.Foreground = new SimpleHighlightingBrush(Theme.HighlightingColors.StringColor);
                        break;
                    case "Comment":
                        color.Foreground = new SimpleHighlightingBrush(Theme.HighlightingColors.CommentColor);
                        break;
                    case "Type":
                        color.Foreground = new SimpleHighlightingBrush(Theme.HighlightingColors.TypeColor);
                    break;
                    case "Constant":
                        color.Foreground = new SimpleHighlightingBrush(Theme.HighlightingColors.ConstantColor);
                        break;
                    case "ControlFlow":
                        color.Foreground = new SimpleHighlightingBrush(Theme.HighlightingColors.ControlFlowColor);
                        break;
                    case "State":
                        color.Foreground = new SimpleHighlightingBrush(Theme.HighlightingColors.StateKeywordColor);
                        break;
                    case "Function":
                        color.Foreground = new SimpleHighlightingBrush(Theme.HighlightingColors.LibraryFunctionColor);
                        break;
                    case "Event":
                        color.Foreground = new SimpleHighlightingBrush(Theme.HighlightingColors.EventColor);
                        break;
                    case "DeprecatedFunction":
                        color.Foreground = new SimpleHighlightingBrush(Theme.HighlightingColors.LibraryFunctionDeprecatedColor);
                        break;
                }
            }

            if (!forceVisualUpdate) return;

            var oldHighlighting = Editor.SyntaxHighlighting;
            Editor.SyntaxHighlighting = null;
            Editor.SyntaxHighlighting = oldHighlighting;
        }


        private IHighlightingDefinition LoadXSHD()
        {
            using (var strm = GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".LSL.xshd"))
            {
                var reader=new XmlTextReader(strm);
                return HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

        }


        private void UpdateHighlightingFromDataProvider(ILSLLibraryDataProvider provider, bool forceVisualUpdate)
        {

            Editor.SyntaxHighlighting = LoadXSHD();
            UpdateHighlightingColorsFromSettings(false);

            foreach (
                var funcs in
                    (from s in provider.LibraryFunctions.Where(x => x.Count > 0)
                        orderby s.First().Name.Length descending
                        select s))
            {
                var name = funcs.First().Name;

                var colorName = "Function";

                if (funcs.All(f => f.Deprecated))
                {
                    colorName = "DeprecatedFunction";
                }

                var rule = new HighlightingRule
                {
                    Regex = new Regex("\\b" + name + "\\b"),
                    Color = Editor.SyntaxHighlighting.GetNamedColor(colorName)
                };


                Editor.SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
            }

            foreach (var cnst in (from s in provider.LibraryConstants orderby s.Name.Length descending select s)
                )
            {
                var rule = new HighlightingRule
                {
                    Regex = new Regex("\\b" + cnst.Name + "\\b"),
                    Color = Editor.SyntaxHighlighting.GetNamedColor("Constant")
                };
                Editor.SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
            }

            foreach (
                var evnt in
                    (from s in provider.LibraryEvents orderby s.Name.Length descending select s))
            {
                var rule = new HighlightingRule
                {
                    Regex = new Regex("\\b" + evnt.Name + "\\b"),
                    Color = Editor.SyntaxHighlighting.GetNamedColor("Event")
                };
                Editor.SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
            }

            if (!forceVisualUpdate) return;

            var oldHighlighting = Editor.SyntaxHighlighting;
            Editor.SyntaxHighlighting = null;
            Editor.SyntaxHighlighting = oldHighlighting;
        }



        private void TextEditor_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            _symbolHoverToolTip.IsOpen = false;
        }


        private void TextEditor_OnMouseLeave(object sender, MouseEventArgs e)
        {
            _symbolHoverToolTip.IsOpen = false;
        }


        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof (string), typeof (LSLEditorControl),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                TextPropertyChangedCallback));

        public static readonly DependencyProperty LibraryDataProviderProperty = DependencyProperty.Register(
            "LibraryDataProvider", typeof (ILSLLibraryDataProvider), typeof (LSLEditorControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                LibraryDataProviderPropertyChangedCallback));


        private void EditorContext_ClickSuggestUserDefinedOrEvent(object sender, RoutedEventArgs e)
        {
            SuggestUserDefinedOrEvent();
        }


        private void EditorContext_ClickSuggestLibraryFunctions(object sender, RoutedEventArgs e)
        {
            SuggestLibraryFunctions();
        }


        private void EditorContext_ClickSuggestLibraryConstants(object sender, RoutedEventArgs e)
        {
            SuggestLibraryConstants();
        }


        private TextViewPosition? _contextMenuOpenPosition;
        private LSLAutoCompleteGlobalFunction _contextMenuFunction;
        private LSLAutoCompleteGlobalVariable _contextMenuVar;
        private LSLAutoCompleteLocalVariable _contextMenuLocalVar;
        private LSLAutoCompleteLocalParameter _contextMenuLocalParam;



        private void TextArea_ContextMenu_GotoDefinitionClick(object sender, RoutedEventArgs e)
        {
            if (_contextMenuOpenPosition != null)
            {
                if (_contextMenuFunction != null)
                {
                    Editor.ScrollTo(_contextMenuFunction.SourceCodeRange.LineStart, 0);
                    Editor.Select(_contextMenuFunction.NameSourceCodeRange.StartIndex,
                        _contextMenuFunction.NameSourceCodeRange.Length);
                }
                else if (_contextMenuLocalVar != null)
                {
                    Editor.ScrollTo(_contextMenuLocalVar.SourceCodeRange.LineStart, 0);
                    Editor.Select(_contextMenuLocalVar.NameSourceCodeRange.StartIndex,
                        _contextMenuLocalVar.NameSourceCodeRange.Length);
                }
                else if (_contextMenuLocalParam != null)
                {
                    Editor.ScrollTo(_contextMenuLocalParam.SourceCodeRange.LineStart, 0);
                    Editor.Select(_contextMenuLocalParam.NameSourceCodeRange.StartIndex,
                        _contextMenuLocalParam.NameSourceCodeRange.Length);
                }
                else if (_contextMenuVar != null)
                {
                    Editor.ScrollTo(_contextMenuVar.SourceCodeRange.LineStart, 0);
                    Editor.Select(_contextMenuVar.NameSourceCodeRange.StartIndex,
                        _contextMenuVar.NameSourceCodeRange.Length);
                }
            }

            Editor.TextArea.SelectionBorder = new Pen(new SolidColorBrush(Theme.SelectionBorderColor), 1);
            Editor.TextArea.SelectionBrush = new SolidColorBrush(Theme.SelectionColor);
            Editor.TextArea.SelectionForeground = new SolidColorBrush(Theme.SelectionForegroundColor);

            _contextMenuFunction = null;
            _contextMenuVar = null;
            _contextMenuLocalVar = null;
            _contextMenuLocalParam = null;
        }


        private void TextArea_ContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            _contextMenuOpenPosition = null;
            GotoDefinitionContextMenuButton.Visibility = Visibility.Collapsed;


            _contextMenuOpenPosition = Editor.GetPositionFromPoint(Mouse.GetPosition(Editor));


            if (_contextMenuOpenPosition == null)
            {
                return;
            }

            var fastVarParser = new LSLAutoCompleteParser();
            fastVarParser.Parse(new StringReader(Editor.Text),
                Editor.Document.GetOffset(_contextMenuOpenPosition.Value.Location));

            var segment = GetIdSegmentUnderMouse(Editor.Document, _contextMenuOpenPosition.Value);
            if (segment == null)
            {
                _contextMenuOpenPosition = null;
                return;
            }

            var wordHovered = Editor.Document.GetText(segment.StartOffset, segment.Length);

            fastVarParser.GlobalFunctionsDictionary.TryGetValue(wordHovered, out _contextMenuFunction);
            _contextMenuLocalVar = fastVarParser.LocalVariables.FirstOrDefault(y => y.Name == wordHovered);

            if (_contextMenuLocalVar == null)
            {
                _contextMenuLocalParam = fastVarParser.LocalParameters.FirstOrDefault(y => y.Name == wordHovered);

                if (_contextMenuLocalParam == null)
                {
                    _contextMenuVar = null;
                    fastVarParser.GlobalVariablesDictionary.TryGetValue(wordHovered, out _contextMenuVar);
                }
            }
            else
            {
                _contextMenuLocalParam = null;
            }

            var isSymbol = (
                _contextMenuFunction != null ||
                _contextMenuVar != null ||
                _contextMenuLocalVar != null ||
                _contextMenuLocalParam != null
                );

            if (isSymbol)
            {
                Editor.Select(segment.StartOffset, segment.Length);

                Editor.TextArea.SelectionBorder = new Pen(new SolidColorBrush(Theme.SymbolSelectionBorderColor), 1)
                {
                    DashStyle = new DashStyle(new double[] {2, 2}, 2.0)
                };

                Editor.TextArea.SelectionBrush = new SolidColorBrush(Theme.SymbolSelectionColor);
                Editor.TextArea.SelectionForeground = new SolidColorBrush(Theme.SymbolSelectionForegroundColor);
            }

            GotoDefinitionContextMenuButton.Visibility =
                isSymbol
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }


        private void TextArea_ContextMenu_OnClosed(object sender, RoutedEventArgs e)
        {
            _contextMenuOpenPosition = null;
            GotoDefinitionContextMenuButton.Visibility = Visibility.Collapsed;


            Editor.TextArea.SelectionBorder = new Pen(new SolidColorBrush(Theme.SelectionBorderColor), 1);
            Editor.TextArea.SelectionBrush = new SolidColorBrush(Theme.SelectionColor);
            Editor.TextArea.SelectionForeground = new SolidColorBrush(Theme.SelectionForegroundColor);

            _contextMenuFunction = null;
            _contextMenuVar = null;
            _contextMenuLocalVar = null;
            _contextMenuLocalParam = null;
        }
    }
}