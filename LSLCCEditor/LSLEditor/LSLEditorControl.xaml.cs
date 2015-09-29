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

//#define DEBUG_FASTPARSER

#endregion

#region Imports

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation;
using LibLSLCC.AutoCompleteParser;
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LSLCCEditor.Utility;

#endregion

namespace LSLCCEditor.LSLEditor
{
    /// <summary>
    ///     Interaction logic for Test.xaml
    /// </summary>
    public partial class LSLEditorControl : UserControl
    {
#if DEBUG_FASTPARSER
        private readonly DebugObjectView _debugObjectView = new DebugObjectView();
#endif


        public delegate void TextChangedEventHandler(object sender, EventArgs e);

        private readonly SolidColorBrush _builtInTypeCompleteColor = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private readonly object _completionLock = new object();
        private readonly ToolTip _symbolHoverToolTip = new ToolTip();

        private readonly Brush _eventHandlerCompleteColor = new SolidColorBrush(Color.FromRgb(0, 76, 127));
        private readonly Brush _globalFunctionCompleteColor = new SolidColorBrush(Color.FromRgb(153, 0, 204));
        private readonly Brush _globalVariableCompleteColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        
        private readonly Brush _libraryConstantCompleteColor = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private readonly Brush _libraryFunctionCompleteColor = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private readonly Brush _localParameterCompleteColor = new SolidColorBrush(Color.FromRgb(0, 102, 0));
        private readonly Brush _localVariableCompleteColor = new SolidColorBrush(Color.FromRgb(0, 102, 255));

        private readonly Brush _labelNameDefinitionCompleteColor = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private readonly Brush _stateNameCompleteColor = new SolidColorBrush(Colors.Black);
        private readonly Brush _labelNameJumpTargetCompleteColor = new SolidColorBrush(Colors.Black);
        private readonly Brush _controlStatementCompleteColor = new SolidColorBrush(Colors.Black);

        private readonly object _propertyChangingLock = new object();

        private readonly HashSet<char> _stateAutocompleteIndentBreakCharacters = new HashSet<char>
        {
            '{',
            '}'
        };

        private readonly HashSet<char> _controlStatementAutocompleteIndentBreakCharacters = new HashSet<char>
        {
            ')',
            ';',
            '{',
            '}' 
        };

        private readonly object _userChangingTextLock = new object();

        private readonly HashSet<string> _validSuggestionPrefixes = new HashSet<string>
        {
            "\t",
            "\r",
            "\n",
            " ",
            "{",
            "}",
            "[",
            "(",
            ")",
            "<",
            ",",
            ";",
            "=",
            "+",
            "-",
            "*",
            "/",
            "%",
            "@",
            ""
        };

        private bool _textPropertyChangingText;
        private bool _userChangingText;


        private class LSLIndentStrategy :
            IIndentationStrategy
        {
            public void IndentLine(TextDocument document, DocumentLine line)
            {
                if (document == null)
                    throw new ArgumentNullException(nameof(document));
                if (line == null)
                    throw new ArgumentNullException(nameof(line));
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

#if DEBUG_FASTPARSER
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
            get { return LibraryDataProvider.SupportedEventHandlers; }
        }

        public IEnumerable<string> LibraryFunctionNames
        {
            get { return LibraryDataProvider.LibraryFunctions.Where(x => x.Count > 0).Select(x => x.First().Name); }
        }

        public ILSLMainLibraryDataProvider LibraryDataProvider
        {
            get { return (ILSLMainLibraryDataProvider) GetValue(LibraryDataProviderProperty); }
            set { SetValue(LibraryDataProviderProperty, value); }
        }


        private void AutoCompleteLibraryConstantsCommand(object o)
        {
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
            SuggestLibraryFunctions();
        }


        private static void TextPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var t = (LSLEditorControl) dependencyObject;
            if (!t._userChangingText && t.Editor.Document != null)
            {
                t._textPropertyChangingText = true;
                t.Editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue == null
                    ? ""
                    : dependencyPropertyChangedEventArgs.NewValue.ToString();
                t._textPropertyChangingText = false;
            }
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
                ((LSLEditorControl) dependencyObject).UpdateHighlightingFromDataProvider(
                    (ILSLMainLibraryDataProvider) dependencyPropertyChangedEventArgs.NewValue);
            }
        }


        private readonly Regex _idCharacterRegex = new Regex("[_a-zA-Z0-9]");
        private readonly Regex _idRegex = new Regex("^[_a-zA-Z]+[_a-zA-Z0-9]*$");


        private TextSegment _GetIDSegmentUnderMouse(TextDocument document, TextViewPosition position)
        {
            var line = position.Line;
            var column = position.Column;

            var offset = document.GetOffset(line, column);


            var parser = new LSLCommentStringSkipper();
            parser.ParseUpTo(Editor.Text, offset);
            if (parser.InComment || parser.InString)
            {
                return null;
            }

            if (offset >= document.TextLength)
                offset--;

            var textAtOffset = document.GetText(offset, 1);

            var startOffset = 0;
            var endOffset = 0;

            // Get text backward of the mouse position, until the first space
            while (!(string.IsNullOrWhiteSpace(textAtOffset) || !_idCharacterRegex.Match(textAtOffset).Success))
            {
                //wordHovered = textAtOffset + wordHovered;

                offset--;

                startOffset = offset;
                if (offset < 0)
                    break;


                textAtOffset = document.GetText(offset, 1);
            }

            // Get text forward the mouse position, until the first space
            offset = document.GetOffset(line, column);
            if (offset < document.TextLength - 1)
            {
                offset++;

                textAtOffset = document.GetText(offset, 1);

                while (!(string.IsNullOrWhiteSpace(textAtOffset) || !_idCharacterRegex.Match(textAtOffset).Success))
                {
                    //wordHovered = wordHovered + textAtOffset;

                    offset++;
                    endOffset = offset;
                    if (offset >= document.TextLength)
                        break;


                    textAtOffset = document.GetText(offset, 1);
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

            if (_idRegex.Match(wordHovered).Success)
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


                var hoverText = "";
                if (LibraryDataProvider.LibraryFunctionExist(wordHovered))
                {
                    hoverText =
                        string.Join(Environment.NewLine + Environment.NewLine,
                            LibraryDataProvider.GetLibraryFunctionSignatures(wordHovered)
                                .Select(x => x.SignatureAndDocumentation));
                }
                if (LibraryDataProvider.EventHandlerExist(wordHovered))
                {
                    hoverText = LibraryDataProvider.GetEventHandlerSignature(wordHovered).SignatureAndDocumentation;
                }
                if (LibraryDataProvider.LibraryConstantExist(wordHovered))
                {
                    hoverText = LibraryDataProvider.GetLibraryConstantSignature(wordHovered).SignatureAndDocumentation;
                }

                if (string.IsNullOrWhiteSpace(hoverText))
                {
                    e.Handled = true;
                    _symbolHoverToolTip.IsOpen = false;
                    return;
                }

                _symbolHoverToolTip.PlacementTarget = this; // required for property inheritance
                _symbolHoverToolTip.Content = new TextBlock
                {
                    MaxWidth = 500,
                    TextWrapping = TextWrapping.Wrap,
                    Text = hoverText
                };
                _symbolHoverToolTip.IsOpen = true;
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


            c.Width = c.Width + 160;


            c.Closed += (sender, args) => { CloseCurrentCompletionWindow(); };

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


            if (string.IsNullOrWhiteSpace(e.Text))
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


            if (DoAutoDedentOnTextEntering(e, caretOffset)) return;


            if (e.Text != "@" && _validSuggestionPrefixes.Contains(e.Text))
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


            if (!_validSuggestionPrefixes.Contains(behind)) return;


            lock (_completionLock)
            {
                var commentSkipper = new LSLCommentStringSkipper(Editor.Text, caretOffset);


                if (commentSkipper.InComment || commentSkipper.InString || KeywordPriorBlocksCompletion(caretOffset))
                {
                    return;
                }

                var fastVarParser = new LSLAutoCompleteParser();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);

#if DEBUG_FASTPARSER
    _debugObjectView.ViewObject("", fastVarParser);
#endif

                IList<ICompletionData> data = null;


                if (TryCompletionForEventHandler(e.Text, fastVarParser, ref data)) return;


                if (TryCompletionForStateName(fastVarParser, ref data)) return;


                if (TryCompletionForLabelNameJumpTarget(fastVarParser, ref data)) return;


                if (TryCompletionForLabelNameDefinition(fastVarParser, ref data)) return;


                var possibleType = TryCompletionForTypeName(e.Text, fastVarParser, ref data);


                var possibleControlStruct = TryCompletionForControlStatement(e.Text, fastVarParser, commentSkipper,
                    ref data);


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


        private bool TryCompletionForLibraryFunction(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestFunction) return false;

            var possibleLibraryFunction = false;

            CurrentCompletionWindow = LazyInitCompletionWindow();
            data = CurrentCompletionWindow.CompletionList.CompletionData;


            var functionSuggestions = LibraryFunctionNames.Where(x => x.StartsWith(insertedText));

            foreach (var func in functionSuggestions)
            {
                var completionData = CreateLSLLibraryFunctionCompletionData(func, fastVarParser);

                data.Add(completionData);


                possibleLibraryFunction = true;
            }
            return possibleLibraryFunction;
        }


        private bool TryCompletionForLibraryFunction(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestFunction) return false;

            var possibleGlobalFunctions = false;

            foreach (var func in LibraryFunctionNames)
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();

                CurrentCompletionWindow.CompletionList.CompletionData.Add(CreateLSLLibraryFunctionCompletionData(func, fastVarParser));

                possibleGlobalFunctions = true;
            }
            return possibleGlobalFunctions;
        }

        private bool TryCompletionForLibraryConstant(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestLibraryConstant) return false;

            var possibleConstant = false;

            CurrentCompletionWindow = LazyInitCompletionWindow();
            data = CurrentCompletionWindow.CompletionList.CompletionData;


            foreach (var sig in ConstantSignatures.Where(x => x.Name.StartsWith(insertedText)))
            {
                data.Add(CreateLSLConstantCompletionData(sig, fastVarParser));

                possibleConstant = true;
            }
            return possibleConstant;
        }

        private bool TryCompletionForLocalVariableOrParameter(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestLocalVariableOrParameter) return false;

            var possibleUserDefinedItem = false;

            CurrentCompletionWindow = LazyInitCompletionWindow();
            data = CurrentCompletionWindow.CompletionList.CompletionData;


            foreach (var v in fastVarParser.LocalParameters.Where(x => x.Name.StartsWith(insertedText)))
            {
                data.Add(CreateLSLLocalParameterCompletionData(v, fastVarParser));
                possibleUserDefinedItem = true;
            }

            foreach (var v in fastVarParser.LocalVariables
                .Where(x => x.Name.StartsWith(insertedText)))
            {
                data.Add(CreateLSLLocalVariableCompletionData(v, fastVarParser));

                possibleUserDefinedItem = true;
            }
            return possibleUserDefinedItem;
        }

        private bool TryCompletionForUserDefinedFunction(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestFunction) return false;

            var possibleUserDefinedItem = false;

            CurrentCompletionWindow = LazyInitCompletionWindow();
            data = CurrentCompletionWindow.CompletionList.CompletionData;


            foreach (var func in fastVarParser.GlobalFunctions.Where(x => x.Name.StartsWith(insertedText)))
            {
                data.Add(CreateLSLGlobalUserFunctionCompletionData(func, fastVarParser));
                possibleUserDefinedItem = true;
            }
            return possibleUserDefinedItem;
        }

        private bool TryCompletionForUserGlobalVariable(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestGlobalVariable) return false;

            var possibleUserDefinedItem = false;

            CurrentCompletionWindow = LazyInitCompletionWindow();
            data = CurrentCompletionWindow.CompletionList.CompletionData;

            foreach (var v in fastVarParser.GlobalVariables.Where(x => x.Name.StartsWith(insertedText)))
            {
                data.Add(CreateLSLGlobalUserVariableCompletionData(v, fastVarParser));
                possibleUserDefinedItem = true;
            }
            return possibleUserDefinedItem;
        }

        private bool TryCompletionForControlStatement(string insertedText, LSLAutoCompleteParser fastVarParser,
            LSLCommentStringSkipper commentSkipper, ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestControlStatement) return false;

            var possibleControlStruct = false;

            if (insertedText.StartsWith("i"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLIfStatementCompletionData(fastVarParser.ScopeAddressAtOffset.ScopeLevel,  fastVarParser));
                possibleControlStruct = true;
            }
            else if (insertedText.StartsWith("e") && fastVarParser.AfterIfOrElseIfStatement)
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLElseStatementCompletionData(fastVarParser.ScopeAddressAtOffset.ScopeLevel,  fastVarParser));
                data.Add(CreateLSLElseIfStatementCompletionData(fastVarParser.ScopeAddressAtOffset.ScopeLevel,   fastVarParser));
                possibleControlStruct = true;
            }
            else if (insertedText.StartsWith("w"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLWhileStatementCompletionData(fastVarParser.ScopeAddressAtOffset.ScopeLevel, fastVarParser));
                possibleControlStruct = true;
            }
            else if (insertedText.StartsWith("d"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLDoStatementCompletionData(fastVarParser.ScopeAddressAtOffset.ScopeLevel, fastVarParser));
                possibleControlStruct = true;
            }
            else if (insertedText.StartsWith("f"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLForStatementCompletionData(fastVarParser.ScopeAddressAtOffset.ScopeLevel, fastVarParser));
                possibleControlStruct = true;
            }
            else if (insertedText.StartsWith("j"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLJumpStatementCompletionData(fastVarParser.ScopeAddressAtOffset.ScopeLevel, fastVarParser));
                possibleControlStruct = true;
            }
            else if (insertedText.StartsWith("r"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;
                data.Add(CreateLSLReturnStatementCompletionData(fastVarParser.ScopeAddressAtOffset.ScopeLevel, fastVarParser));
                possibleControlStruct = true;
            }
            else if (insertedText.StartsWith("s"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;
                data.Add(CreateLSLStateChangeStatementCompletionData(fastVarParser.ScopeAddressAtOffset.ScopeLevel, fastVarParser));
                possibleControlStruct = true;
            }
            else if (insertedText.StartsWith("@") && !fastVarParser.InSingleStatementCodeScopeTopLevel)
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;
                data.Add(CreateLSLLabelStatementCompletionData(fastVarParser.ScopeAddressAtOffset.ScopeLevel, fastVarParser));
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

                data.Add(CreateLSLTypeCompletionData(LSLType.Integer, fastVarParser));
                possibleType = true;
            }
            else if (insertedText.StartsWith("s"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLTypeCompletionData(LSLType.String, fastVarParser));
                possibleType = true;
            }
            else if (insertedText.StartsWith("v"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLTypeCompletionData(LSLType.Vector, fastVarParser));
                possibleType = true;
            }
            else if (insertedText.StartsWith("r"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLTypeCompletionData(LSLType.Rotation, fastVarParser));
                possibleType = true;
            }
            else if (insertedText.StartsWith("k"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLTypeCompletionData(LSLType.Key, fastVarParser));
                possibleType = true;
            }
            else if (insertedText.StartsWith("f"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLTypeCompletionData(LSLType.Float, fastVarParser));
                possibleType = true;
            }
            else if (insertedText.StartsWith("l"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLTypeCompletionData(LSLType.List, fastVarParser));
                possibleType = true;
            }
            return possibleType;
        }

        private bool TryCompletionForLabelNameDefinition(LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestLabelNameDefinition) return false;

            CurrentCompletionWindow = LazyInitCompletionWindow();
            data = CurrentCompletionWindow.CompletionList.CompletionData;


            var possibleLabelName = false;
            foreach (var label in fastVarParser.GetLocalJumps(Editor.Text))
            {
                data.Add(CreateLSLLabelDefinitionCompletionData(label, fastVarParser));
                possibleLabelName = true;
            }

            if (!possibleLabelName)
            {
                CurrentCompletionWindow = null;
                return true;
            }

            CurrentCompletionWindow.Show();
            return true;
        }

        private bool TryCompletionForLabelNameJumpTarget(LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestLabelNameJumpTarget) return false;


            CurrentCompletionWindow = LazyInitCompletionWindow();
            data = CurrentCompletionWindow.CompletionList.CompletionData;


            var possibleLabelName = false;
            foreach (var label in fastVarParser.GetLocalLabels(Editor.Text))
            {
                data.Add(CreateLSLLabelJumpTargetCompletionData(label, fastVarParser));
                possibleLabelName = true;
            }

            if (!possibleLabelName)
            {
                CurrentCompletionWindow = null;
                return true;
            }

            CurrentCompletionWindow.Show();

            return true;
        }

        private bool TryCompletionForStateName(LSLAutoCompleteParser fastVarParser, ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestStateName) return false;


            CurrentCompletionWindow = LazyInitCompletionWindow();
            data = CurrentCompletionWindow.CompletionList.CompletionData;


            data.Add(CreateLSLDefaultStateNameCompletionData(fastVarParser));

            foreach (var state in fastVarParser.StateBlocks)
            {
                data.Add(CreateLSLStateNameCompletionData(state, fastVarParser));
            }

            CurrentCompletionWindow.Show();
            return true;
        }

        private bool TryCompletionForEventHandler(string insertedText, LSLAutoCompleteParser fastVarParser,
            ref IList<ICompletionData> data)
        {
            if (!fastVarParser.CanSuggestEventHandler) return false;


            CurrentCompletionWindow = LazyInitCompletionWindow();
            data = CurrentCompletionWindow.CompletionList.CompletionData;


            var possibleEventName = false;
            foreach (var eventHandler in EventSignatures.Where(x => x.Name.StartsWith(insertedText)))
            {
                data.Add(CreateLSLEventCompletionData(eventHandler, fastVarParser));
                possibleEventName = true;
            }

            if (!possibleEventName)
            {
                CurrentCompletionWindow = null;
                return true;
            }

            CurrentCompletionWindow.Show();
            return true;
        }

        private LSLCompletionData CreateLSLDefaultStateNameCompletionData(LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData("default", "default", "Default script state", 0)
            {
                AppendOnInsert = ";",
                ColorBrush = _stateNameCompleteColor
            };

            int offset = autoCompleteParser.ParseToOffset;

            while (true)
            {
                var c = Editor.Text[offset];
                var b = (char.IsWhiteSpace(c) || _idCharacterRegex.IsMatch(c.ToString())) && c != '\n' && c != '\r';

                if (!b && c == ';')
                {
                    data.AppendOnInsert = "";
                    data.OffsetCaretRelativeToDocument = true;
                    data.OffsetCaretAfterInsert = true;
                    data.CaretOffsetAfterInsert = offset+8;
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

        private LSLCompletionData CreateLSLStateNameCompletionData(LSLAutoCompleteParser.StateBlock state, LSLAutoCompleteParser autoCompleteParser)
        {
            var data= new LSLCompletionData(state.Name, state.Name, "Script state", 0)
            {
                AppendOnInsert = ";",
                ColorBrush = _stateNameCompleteColor
            };


            int offset = autoCompleteParser.ParseToOffset;


            while (true)
            {
                var c = Editor.Text[offset];
                var b = (char.IsWhiteSpace(c) || _idCharacterRegex.IsMatch(c.ToString())) && c != '\n' && c != '\r';

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

        private LSLCompletionData CreateLSLLabelDefinitionCompletionData(LSLAutoCompleteParser.LocalJump label, LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(label.Target, label.Target, "In scope jump", 0)
            {
                AppendOnInsert = ";",
                ColorBrush = _labelNameDefinitionCompleteColor
            };

            int offset = autoCompleteParser.ParseToOffset;

            while (true)
            {
                var c = Editor.Text[offset];
                var b = (char.IsWhiteSpace(c) || _idCharacterRegex.IsMatch(c.ToString())) && c != '\n' && c != '\r';

                if (!b && c == ';')
                {
                    data.AppendOnInsert = "";
                    data.OffsetCaretRelativeToDocument = true;
                    data.OffsetCaretAfterInsert = true;
                    data.CaretOffsetAfterInsert = offset + label.Target.Length + 1;
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

        private LSLCompletionData CreateLSLLabelJumpTargetCompletionData(LSLAutoCompleteParser.LocalLabel label, LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(label.Name, label.Name, "In scope label", 0)
            {
                AppendOnInsert = ";",
                ColorBrush = _labelNameJumpTargetCompleteColor
            };

            int offset = autoCompleteParser.ParseToOffset;

            while (true)
            {
                var c = Editor.Text[offset];
                var b = (char.IsWhiteSpace(c) || _idCharacterRegex.IsMatch(c.ToString())) && c != '\n' && c != '\r';

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



        private LSLCompletionData CreateLSLGlobalUserVariableCompletionData(LSLAutoCompleteParser.GlobalVariable v, LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(v.Name, v.Name,
                "Global variable:\n" + v.Type + " " + v.Name + ";", 1)
            {
                ColorBrush = _globalVariableCompleteColor
            };

            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }

        private LSLCompletionData CreateLSLLibraryFunctionCompletionData(string func, LSLAutoCompleteParser autoCompleteParser)
        {
            var docs = string.Join(Environment.NewLine + Environment.NewLine,
                LibraryDataProvider.GetLibraryFunctionSignatures(func)
                    .Select(x => x.SignatureAndDocumentation));

            var data = new LSLCompletionData(func, func, docs, 6)
            {
                AppendOnInsert = "()",
                ColorBrush = _libraryFunctionCompleteColor,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = -1,
            };



            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }

        private LSLCompletionData CreateLSLConstantCompletionData(LSLLibraryConstantSignature sig, LSLAutoCompleteParser autoCompleteParser)
        {
            return new LSLCompletionData(sig.Name, sig.Name,
                sig.SignatureAndDocumentation, 5)
            {
                ColorBrush = _libraryConstantCompleteColor
            };
        }

        private LSLCompletionData CreateLSLLocalVariableCompletionData(LSLAutoCompleteParser.LocalVariable v, LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(v.Name, v.Name, "Local variable:\n" + v.Type + " " + v.Name + ";", 4)
            {
                ColorBrush = _localVariableCompleteColor
            };


            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }

        private LSLCompletionData CreateLSLLocalParameterCompletionData(LSLAutoCompleteParser.LocalParameter v, LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(v.Name, v.Name, "Local parameter:\n" + v.Type + " " + v.Name + ";", 3)
            {
                ColorBrush = _localParameterCompleteColor
            };

            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }

        private LSLCompletionData CreateLSLGlobalUserFunctionCompletionData(LSLAutoCompleteParser.GlobalFunction func, LSLAutoCompleteParser autoCompleteParser)
        {
            var data= new LSLCompletionData(func.Name, func.Name, "Global function:\n" + func.Signature, 2)
            {
                AppendOnInsert = "()",
                ColorBrush = _globalFunctionCompleteColor,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = -1,
            };



            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }






        private LSLCompletionData CreateLSLForStatementCompletionData(int scopeLevel, LSLAutoCompleteParser autoCompleteParser)
        {
            return new LSLCompletionData("for", "for(;;)", "else if statement", 0)
            {
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}"),
                ColorBrush = _controlStatementCompleteColor,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 4,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters
            };
        }

        private LSLCompletionData CreateLSLWhileStatementCompletionData(int scopeLevel, LSLAutoCompleteParser autoCompleteParser)
        {
            return new LSLCompletionData("while", "while()", "else if statement", 0)
            {
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}"),
                ColorBrush = _controlStatementCompleteColor,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 6,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters
            };
        }



        private LSLCompletionData CreateLSLDoStatementCompletionData(int scopeLevel, LSLAutoCompleteParser autoCompleteParser)
        {
            return new LSLCompletionData("do", "do", "do statement", 0)
            {
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}"),
                ColorBrush = _controlStatementCompleteColor,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 4 + scopeLevel,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters
            };
        }

        private LSLCompletionData CreateLSLIfStatementCompletionData(int scopeLevel, LSLAutoCompleteParser autoCompleteParser)
        {
            return new LSLCompletionData("if", "if()", "if statement", 0)
            {
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}"),
                ColorBrush = _controlStatementCompleteColor,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 3,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters
            };
        }

        private LSLCompletionData CreateLSLElseIfStatementCompletionData(int scopeLevel, LSLAutoCompleteParser autoCompleteParser)
        {
            return new LSLCompletionData("else if", "else if()", "else if statement", 0)
            {
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}"),
                ColorBrush = _controlStatementCompleteColor,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 8,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters
            };
        }

        private LSLCompletionData CreateLSLElseStatementCompletionData(int scopeLevel, LSLAutoCompleteParser autoCompleteParser)
        {
            return new LSLCompletionData("else", "else", "else statement", 0)
            {
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}"),
                ColorBrush = _controlStatementCompleteColor,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 6 + scopeLevel,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters
            };
        }


        private LSLCompletionData CreateLSLReturnStatementCompletionData(int scopeLevel, LSLAutoCompleteParser autoCompleteParser)
        {
            var inReturningFunction = autoCompleteParser.InFunctionCodeBody &&
                                      autoCompleteParser.CurrentFunctionReturnType != LSLType.Void;

            var data = new LSLCompletionData("return", "return", "return statement", 0)
            {
                AppendOnInsert = inReturningFunction ? " ;" : ";",
                ColorBrush = _controlStatementCompleteColor,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 6,
                InsertTextAtCaretAfterOffset = false,
            };

            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;
            data.CaretOffsetAfterInsert = data.CaretOffsetAfterInsert + 1;

            return data;
        }




        private LSLCompletionData CreateLSLLabelStatementCompletionData(int scopeLevel, LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData("@", "@", "label statement", 0)
            {
                AppendOnInsert = ";",
                ColorBrush = _controlStatementCompleteColor,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 0,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters
            };

            return data;
        }


        private LSLCompletionData CreateLSLJumpStatementCompletionData(int scopeLevel, LSLAutoCompleteParser autoCompleteParser)
        {

            


            var data = new LSLCompletionData("jump", "jump", "jump statement", 0)
            {
                AppendOnInsert = " ;",
                ColorBrush = _controlStatementCompleteColor,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 4,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters
            };


            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;
            data.CaretOffsetAfterInsert = data.CaretOffsetAfterInsert + 1;

            return data;
        }


        private LSLCompletionData CreateLSLStateChangeStatementCompletionData(int scopeLevel, LSLAutoCompleteParser autoCompleteParser)
        {
            var data =  new LSLCompletionData("state", "state", "state change statement", 0)
            {
                AppendOnInsert = " ;",
                ColorBrush = _controlStatementCompleteColor,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 5,
                InsertTextAtCaretAfterOffset = false
            };


            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;
            data.CaretOffsetAfterInsert = data.CaretOffsetAfterInsert + 1;

            return data;
        }




        private LSLCompletionData CreateLSLTypeCompletionData(LSLType type, LSLAutoCompleteParser autoCompleteParser)
        {
            var name = LSLTypeTools.ToLSLTypeString(type);
            return new LSLCompletionData(name, name,
                name + "type", 0)
            {
                ColorBrush = _builtInTypeCompleteColor
            };
        }





        private LSLCompletionData CreateLSLEventCompletionData(LSLLibraryEventSignature eventHandler, LSLAutoCompleteParser autoCompleteParser)
        {
            var parameters = eventHandler.SignatureString.Substring(eventHandler.Name.Length);

            var stateCompletionData = new LSLCompletionData(
                eventHandler.Name,
                eventHandler.Name, eventHandler.SignatureAndDocumentation, 0)
            {
                AppendOnInsert = parameters + "\n{\n\t\n}",
                ColorBrush = _eventHandlerCompleteColor,
                ForceIndent = true,
                IndentLevel = 1,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 1 + eventHandler.Name.Length + parameters.Length + 4,
                InsertTextAtCaretAfterOffset = true,
                CaretOffsetInsertionText = "\t",
                IndentBreakCharacters = _stateAutocompleteIndentBreakCharacters
            };
            return stateCompletionData;
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

            CurrentCompletionWindow = LazyInitCompletionWindow();
            var data = CurrentCompletionWindow.CompletionList.CompletionData;

            foreach (var i in fastVarParser.LocalParameters)
            {
                data.Add(CreateLSLLocalParameterCompletionData(i, fastVarParser));
                possibleUserDefinedItem = true;
            }

            foreach (var i in fastVarParser.LocalVariables)
            {
                data.Add(CreateLSLLocalVariableCompletionData(i, fastVarParser));
                possibleUserDefinedItem = true;
            }

            return possibleUserDefinedItem;
        }


        private bool TryCompletionForUserDefinedFunction(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestFunction) return false;

            var possibleUserDefinedItem = false;

            CurrentCompletionWindow = LazyInitCompletionWindow();
            var data = CurrentCompletionWindow.CompletionList.CompletionData;

            foreach (var i in fastVarParser.GlobalFunctions)
            {
                data.Add(CreateLSLGlobalUserFunctionCompletionData(i, fastVarParser));
                possibleUserDefinedItem = true;
            }
            return possibleUserDefinedItem;
        }

        private bool TryCompletionForUserGlobalVariable(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestGlobalVariable) return false;

            var possibleUserDefinedItem = false;

            CurrentCompletionWindow = LazyInitCompletionWindow();
            var data = CurrentCompletionWindow.CompletionList.CompletionData;

            foreach (var i in fastVarParser.GlobalVariables)
            {
                data.Add(CreateLSLGlobalUserVariableCompletionData(i, fastVarParser));
                possibleUserDefinedItem = true;
            }
            return possibleUserDefinedItem;
        }


        private bool TryCompletionForLabelNameDefinition(LSLAutoCompleteParser fastVarParser)
        {
            if (!fastVarParser.CanSuggestLabelNameDefinition) return false;


            CurrentCompletionWindow = LazyInitCompletionWindow();
            var data = CurrentCompletionWindow.CompletionList.CompletionData;

            var possibleLabelName = false;
            foreach (var label in fastVarParser.GetLocalJumps(Editor.Text))
            {
                data.Add(CreateLSLLabelDefinitionCompletionData(label, fastVarParser));
                possibleLabelName = true;
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
            if (fastVarParser.CanSuggestLabelNameJumpTarget)
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                var data = CurrentCompletionWindow.CompletionList.CompletionData;

                var possibleLabelName = false;
                foreach (var label in fastVarParser.GetLocalLabels(Editor.Text))
                {
                    data.Add(CreateLSLLabelJumpTargetCompletionData(label, fastVarParser));
                    possibleLabelName = true;
                }

                if (!possibleLabelName)
                {
                    CurrentCompletionWindow = null;
                    return true;
                }

                CurrentCompletionWindow.Show();
                return true;
            }
            return false;
        }

        private bool TryCompletionForStateName(LSLAutoCompleteParser fastVarParser)
        {
            if (fastVarParser.CanSuggestStateName)
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                var data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateLSLDefaultStateNameCompletionData(fastVarParser));

                foreach (var state in fastVarParser.StateBlocks)
                {
                    data.Add(CreateLSLStateNameCompletionData(state, fastVarParser));
                }

                CurrentCompletionWindow.Show();
                return true;
            }
            return false;
        }

        private bool TryCompletionForEventHandler(LSLAutoCompleteParser fastVarParser)
        {
            if (fastVarParser.CanSuggestEventHandler)
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                var data = CurrentCompletionWindow.CompletionList.CompletionData;

                var possibleEventName = false;

                foreach (var eventHandler in EventSignatures)
                {
                    data.Add(CreateLSLEventCompletionData(eventHandler, fastVarParser));
                    possibleEventName = true;
                }

                if (!possibleEventName)
                {
                    CurrentCompletionWindow = null;
                    return true;
                }

                CurrentCompletionWindow.Show();
                return true;
            }
            return false;
        }


        private string LookBehindCaretOffset(int caretOffset, int behindOffset, int length)
        {
            return (caretOffset - behindOffset) > 0 ? Editor.Document.GetText(caretOffset - behindOffset, length) : "";
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

#if DEBUG_FASTPARSER
            var P = new LSLAutoCompleteParser();
            P.Parse(new StringReader(Editor.Text), caretOffset);
            _debugObjectView.ViewObject("", P);
#endif

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
            bool possibleLibraryConstants = false;

            CurrentCompletionWindow = LazyInitCompletionWindow();


            foreach (var con in ConstantSignatures)
            {
                CurrentCompletionWindow.CompletionList.CompletionData.Add(
                    CreateLSLConstantCompletionData(con, fastVarParser));

                possibleLibraryConstants = true;
            }

            return possibleLibraryConstants;
        }


        public void UpdateHighlightingFromDataProvider()
        {
            UpdateHighlightingFromDataProvider(LibraryDataProvider);
        }


        public void UpdateHighlightingFromDataProvider(ILSLMainLibraryDataProvider provider)
        {
            using (var resourceStream = GetType().Assembly.GetManifestResourceStream("LSLCCEditor.LSLEditor.LSL.xshd"))
            {
                if (resourceStream != null)
                {
                    var reader = new XmlTextReader(resourceStream);

                    Editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);


                    foreach (
                        var func in
                            (from s in provider.LibraryFunctions.Where(x => x.Count > 0).Select(x => x.First().Name)
                                orderby s.Length descending
                                select s))
                    {
                        var rule = new HighlightingRule
                        {
                            Regex = new Regex("\\b" + func + "\\b"),
                            Color = Editor.SyntaxHighlighting.GetNamedColor("Functions")
                        };
                        Editor.SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
                    }

                    foreach (var cnst in (from s in provider.LibraryConstants orderby s.Name.Length descending select s)
                        )
                    {
                        var rule = new HighlightingRule
                        {
                            Regex = new Regex("\\b" + cnst.Name + "\\b"),
                            Color = Editor.SyntaxHighlighting.GetNamedColor("Constants")
                        };
                        Editor.SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
                    }

                    foreach (
                        var evnt in
                            (from s in provider.SupportedEventHandlers orderby s.Name.Length descending select s))
                    {
                        var rule = new HighlightingRule
                        {
                            Regex = new Regex("\\b" + evnt.Name + "\\b"),
                            Color = Editor.SyntaxHighlighting.GetNamedColor("Events")
                        };
                        Editor.SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Could not open manifest resource stream LSLCCEditor.LSL.xshd");
                }
            }
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
            "LibraryDataProvider", typeof (ILSLMainLibraryDataProvider), typeof (LSLEditorControl),
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
        private LSLAutoCompleteParser.GlobalFunction _contextMenuFunction;
        private LSLAutoCompleteParser.GlobalVariable _contextMenuVar;
        private LSLAutoCompleteParser.LocalVariable _contextMenuLocalVar;
        private LSLAutoCompleteParser.LocalParameter _contextMenuLocalParam;
        


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


            Editor.TextArea.SelectionBorder = new Pen(SystemColors.HighlightBrush, 1);
            Editor.TextArea.SelectionBrush = new SolidColorBrush(SystemColors.HighlightColor) {Opacity = 0.7};
            Editor.TextArea.SelectionForeground = SystemColors.HighlightTextBrush;

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

                Editor.TextArea.SelectionBorder = new Pen(Brushes.Black, 1)
                {
                    DashStyle = new DashStyle(new double[] {2, 2}, 2.0)
                };

                Editor.TextArea.SelectionBrush = Brushes.Transparent;
                Editor.TextArea.SelectionForeground = Brushes.Red;
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


            Editor.TextArea.SelectionBorder = new Pen(SystemColors.HighlightBrush, 1);
            Editor.TextArea.SelectionBrush = new SolidColorBrush(SystemColors.HighlightColor) {Opacity = 0.7};
            Editor.TextArea.SelectionForeground = SystemColors.HighlightTextBrush;

            _contextMenuFunction = null;
            _contextMenuVar = null;
            _contextMenuLocalVar = null;
            _contextMenuLocalParam = null;
        }
    }
}