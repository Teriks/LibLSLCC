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
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.Collections;
using LibLSLCC.LibraryData;
using LibLSLCC.Utility;
using LSLCCEditor.EditControl;
using LSLCCEditor.Utility;
using CompletionWindow = LSLCCEditor.CompletionUI.CompletionWindow;

#endregion

namespace LSLCCEditor.EditControl
{
    /// <summary>
    ///     Interaction logic for Test.xaml
    /// </summary>
    public partial class LSLEditorControl : UserControl
    {
#if DEBUG_FASTPARSER
        private readonly DebugObjectView _debugObjectView = new DebugObjectView();
#endif



        public TextEditor Editor
        {
            get { return AvalonEditor; }
        }


        public LSLEditorControlSettings Settings
        {
            get { return _settings; }
            set  { _settings = value; }
        }

        private LSLEditorControlSettings _settings = new LSLEditorControlSettings();

        public delegate void TextChangedEventHandler(object sender, EventArgs e);


        
        private readonly object _completionLock = new object();
        private readonly ToolTip _symbolHoverToolTip = new ToolTip();



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

            Settings.CaseInsensitiveAutoCompleteMatching = true;

            Settings.ConstantCompletionFirstCharIsCaseSensitive = true;


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
                ((LSLEditorControl) dependencyObject).UpdateHighlightingFromDataProvider(
                    (ILSLLibraryDataProvider) dependencyPropertyChangedEventArgs.NewValue);
            }
        }



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
            while (!(string.IsNullOrWhiteSpace(textAtOffset) || !LSLTokenTools.IDAnyCharRegex.Match(textAtOffset).Success))
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

                while (!(string.IsNullOrWhiteSpace(textAtOffset) || !LSLTokenTools.IDAnyCharRegex.Match(textAtOffset).Success))
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


                    LSLAutoCompleteParser.GlobalVariable globalVariable;
                    LSLAutoCompleteParser.GlobalFunction globalFunction;
                    LSLAutoCompleteParser.LocalParameter localParameter;

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

            c.CloseWhenCaretAtBeginning = true;

            c.CompletionList.SubstringSearchWhileFiltering = Settings.SubstringSearchAutoCompleteMatching;

            c.CompletionList.CamelCaseMatching = Settings.CamelCaseAutoCompleteMatching;

            c.CompletionList.CaseInsensitiveMatching = Settings.CaseInsensitiveAutoCompleteMatching;

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
            if (!fastVarParser.CanSuggestControlStatement) return false;

            var possibleControlStruct = false;

            if (insertedText.StartsWith("i"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateCompletionData_IfStatement(fastVarParser.ScopeAddressAtOffset.ScopeLevel, fastVarParser));
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

                data.Add(CreateCompletionData_DoStatement(fastVarParser.ScopeAddressAtOffset.ScopeLevel, fastVarParser));
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
            else if (insertedText.StartsWith("j"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;

                data.Add(CreateCompletionData_JumpStatement(fastVarParser));
                possibleControlStruct = true;
            }
            else if (insertedText.StartsWith("r"))
            {
                CurrentCompletionWindow = LazyInitCompletionWindow();
                data = CurrentCompletionWindow.CompletionList.CompletionData;
                data.Add(CreateCompletionData_ReturnStatement(fastVarParser));
                possibleControlStruct = true;
            }
            else if (insertedText.StartsWith("s"))
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
                .Where(x=>ToLowerIfCaseInsensitiveComplete(x.Target).StartsWith(insertedText))
                .OrderBy(x=>x.Target.Length);

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
                ColorBrush = Settings.StateNameCompleteBrush,
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


        private TextBlock CreateDescriptionTextBlock_DefinedState(LSLAutoCompleteParser.StateBlock state)
        {
            var description = new TextBlock();
            description.Inlines.Add(new Run(state.Name) {FontWeight = FontWeights.Bold});
            description.Inlines.Add(" script state");
            return description;
        }


        private LSLCompletionData CreateCompletionData_StateName(LSLAutoCompleteParser.StateBlock state,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(state.Name, state.Name, 0)
            {
                AppendOnInsert = ";",
                ColorBrush = Settings.StateNameCompleteBrush,
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


        private LSLCompletionData CreateCompletionData_LabelDefinition(LSLAutoCompleteParser.LocalJump label,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(label.Target, label.Target, 0)
            {
                AppendOnInsert = ";",
                ColorBrush = Settings.LabelNameDefinitionCompleteBrush,
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

        private static TextBlock CreateDescriptionTextBlock_LabelDefinition()
        {
            return new TextBlock {Text = "In scope jump"};
        }


        private LSLCompletionData CreateCompletionData_LabelJumpTarget(LSLAutoCompleteParser.LocalLabel label,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(label.Name, label.Name, 0)
            {
                AppendOnInsert = ";",
                ColorBrush = Settings.LabelNameJumpTargetCompleteBrush,
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


        private LSLCompletionData CreateCompletionData_GlobalUserVariable(LSLAutoCompleteParser.GlobalVariable v,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(v.Name, v.Name, 1)
            {
                ColorBrush = Settings.GlobalVariableCompleteBrush,
                DescriptionFactory = () => CreateGlobalVariableDescriptionTextBlock(v)
            };

            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }


        private TextBlock CreateGlobalVariableDescriptionTextBlock(LSLAutoCompleteParser.GlobalVariable v)
        {
            var description = new TextBlock();

            description.Inlines.Add(new Run("Global Variable:" + LSLFormatTools.CreateNewLinesString(2))
            {
                FontWeight = FontWeights.Bold
            });

            description.Inlines.Add(CreateHighlightedRunFromXshd("Type", v.Type));
            description.Inlines.Add(" " + v.Name);
            return description;
        }


        private LSLCompletionData CreateCompletionData_LibraryFunction(string func,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var sigs = LibraryDataProvider.GetLibraryFunctionSignatures(func);

            var allOverloadsDeprecated = sigs.All(x => x.Deprecated);

            var colorBrush = Settings.LibraryFunctionCompleteBrush;

            if (allOverloadsDeprecated)
            {
                colorBrush = Settings.LibraryFunctionDeprecatedCompleteBrush;
            }

            var data = new LSLCompletionData(func, func, 6)
            {
                AppendOnInsert = "()",
                ColorBrush = colorBrush,
                DescriptionFactory = () => CreateDescriptionTextBlock_LibraryFunction(sigs)
            };


            if (sigs.Any(x => x.ParameterCount > 0))
            {
                data.OffsetCaretAfterInsert = true;
                data.CaretOffsetAfterInsert = -1;
            }

            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }


        private LSLCompletionData CreateCompletionData_Constant(LSLLibraryConstantSignature sig)
        {
            var data = new LSLCompletionData(sig.Name, sig.Name, 5)
            {
                ColorBrush = Settings.LibraryConstantCompleteBrush,
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
            description.Inlines.Add(CreateHighlightedRunFromXshd("Constants", sig.Name));
            description.Inlines.Add(" = ");
            description.Inlines.Add(sig.ValueStringAsCodeLiteral + ";");

            if (!string.IsNullOrWhiteSpace(sig.DocumentationString))
            {
                description.Inlines.Add(LSLFormatTools.CreateNewLinesString(2) + sig.DocumentationString);
            }
            return description;
        }


        private LSLCompletionData CreateCompletionData_LocalVariable(LSLAutoCompleteParser.LocalVariable v,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(v.Name, v.Name, 4)
            {
                ColorBrush = Settings.LocalVariableCompleteBrush,
                DescriptionFactory = () => CreateDescriptionTextBlock_LocalVariable(v)
            };




            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }


        private TextBlock CreateDescriptionTextBlock_LocalVariable(LSLAutoCompleteParser.LocalVariable v)
        {
            var description = new TextBlock();

            description.Inlines.Add(new Run("Local Variable:" + LSLFormatTools.CreateNewLinesString(2))
            {
                FontWeight = FontWeights.Bold
            });

            description.Inlines.Add(CreateHighlightedRunFromXshd("Type", v.Type));
            description.Inlines.Add(" " + v.Name);
            return description;
        }


        private LSLCompletionData CreateCompletionData_LocalParameter(LSLAutoCompleteParser.LocalParameter v,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(v.Name, v.Name, 3)
            {
                ColorBrush = Settings.LocalParameterCompleteBrush
            };


            data.DescriptionFactory = () => CreateDescriptionTextBlock_LocalParameter(v);


            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_LocalParameter(LSLAutoCompleteParser.LocalParameter v)
        {
            var description = new TextBlock();

            description.Inlines.Add(new Run("Local Parameter:" + LSLFormatTools.CreateNewLinesString(2))
            {
                FontWeight = FontWeights.Bold
            });

            description.Inlines.Add(CreateHighlightedRunFromXshd("Type", v.Type));
            description.Inlines.Add(" " + v.Name);
            return description;
        }



        private LSLCompletionData CreateCompletionData_GlobalUserFunction(LSLAutoCompleteParser.GlobalFunction func,
            LSLAutoCompleteParser autoCompleteParser)
        {
            var data = new LSLCompletionData(func.Name, func.Name, 2)
            {
                AppendOnInsert = "()",
                ColorBrush = Settings.GlobalFunctionCompleteBrush,
                DescriptionFactory = () => CreateDescriptionTextBlock_GlobalUserFunction(func)
            };


            if (func.Parameters.Count > 0)
            {
                data.OffsetCaretAfterInsert = true;
                data.CaretOffsetAfterInsert = -1;
            }


            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;

            return data;
        }

        private TextBlock CreateDescriptionTextBlock_GlobalUserFunction(LSLAutoCompleteParser.GlobalFunction func)
        {
            var description = new TextBlock();

            var nameRun = new Run(func.Name)
            {
                FontWeight = FontWeights.Bold,
                Foreground = Settings.GlobalFunctionCompleteBrush
            };

            description.Inlines.Add(new Run("Global Function:" + LSLFormatTools.CreateNewLinesString(2))
            {
                FontWeight = FontWeights.Bold
            });

            if (func.HasReturnType)
            {
                description.Inlines.Add(CreateHighlightedRunFromXshd("Type", func.ReturnType + " "));
            }

            description.Inlines.Add(nameRun);
            description.Inlines.Add(new Run("(") {FontWeight = FontWeights.Bold});

            var pIndex = 1;
            foreach (var param in func.Parameters)
            {
                description.Inlines.Add(CreateHighlightedRunFromXshd("Type", param.Type + " "));
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
                        Foreground = Brushes.Red,
                        FontWeight = FontWeights.Bold
                    });

                    nameRun.Foreground = Settings.LibraryFunctionDeprecatedCompleteBrush;
                }
                else
                {
                    nameRun.Foreground = Settings.LibraryFunctionCompleteBrush;
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
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}"),
                ColorBrush = Settings.ControlStatementCompleteBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 4,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters,
                DescriptionFactory = () => CreateDescriptionTextBlock_ForStatement()
            };

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
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}"),
                ColorBrush = Settings.ControlStatementCompleteBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 6,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters,
                DescriptionFactory = () => CreateDescriptionTextBlock_WhileStatement()
            };


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
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}\nwhile()"),
                ColorBrush = Settings.ControlStatementCompleteBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = -1,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters,
                DescriptionFactory = () => CreateDescriptionTextBlock_DoStatement()
            };


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
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}"),
                ColorBrush = Settings.ControlStatementCompleteBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 3,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters,
                DescriptionFactory = () => CreateDescriptionTextBlock_IfStatement()
            };

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
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}"),
                ColorBrush = Settings.ControlStatementCompleteBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 8,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters,
                DescriptionFactory = () => CreateDescriptionTextBlock_ElseIfStatement()
            };

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
                AppendOnInsert = (autoCompleteParser.InSingleStatementCodeScopeTopLevel ? "" : "\n{\n}"),
                ColorBrush = Settings.ControlStatementCompleteBrush,
                ForceIndent = true,
                IndentLevel = scopeLevel,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 6 + scopeLevel,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters,
                DescriptionFactory = () => CreateDescriptionTextBlock_ElseStatement()
            };
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
                ColorBrush = Settings.ControlStatementCompleteBrush,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 6,
                InsertTextAtCaretAfterOffset = false,
                DescriptionFactory = () => CreateDescriptionTextBlock_ReturnStatement()
            };


            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;
            data.CaretOffsetAfterInsert = data.CaretOffsetAfterInsert + 1;

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
                ColorBrush = Settings.ControlStatementCompleteBrush,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 4,
                InsertTextAtCaretAfterOffset = false,
                IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters,
                DescriptionFactory = () => CreateDescriptionTextBlock_JumpStatement()
            };


            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;
            data.CaretOffsetAfterInsert = data.CaretOffsetAfterInsert + 1;

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
                ColorBrush = Settings.ControlStatementCompleteBrush,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 5,
                InsertTextAtCaretAfterOffset = false,
                DescriptionFactory = () => CreateDescriptionTextBlock_StateChangeStatment()
            };

            if (!autoCompleteParser.InSingleStatementCodeScopeTopLevel) return data;

            data.ForceIndent = true;
            data.IndentBreakCharacters = _controlStatementAutocompleteIndentBreakCharacters;
            data.IndentLevel = autoCompleteParser.ScopeAddressAtOffset.ScopeLevel;
            data.CaretOffsetAfterInsert = data.CaretOffsetAfterInsert + 1;

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
                ColorBrush = Settings.BuiltInTypeCompleteBrush,
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
                ColorBrush = Settings.EventHandlerCompleteBrush,
                ForceIndent = true,
                IndentLevel = 1,
                OffsetCaretFromBegining = true,
                OffsetCaretAfterInsert = true,
                CaretOffsetAfterInsert = 1 + eventHandler.Name.Length + parameters.Length + 4,
                InsertTextAtCaretAfterOffset = true,
                CaretOffsetInsertionText = "\t",
                IndentBreakCharacters = _stateAutocompleteIndentBreakCharacters,
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
                Foreground = Settings.EventHandlerCompleteBrush
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

            foreach (var label in fastVarParser.GetLocalJumps(Editor.Text).OrderBy(x => x.Target.Length))
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
            UpdateHighlightingFromDataProvider(LibraryDataProvider);
        }


        public void UpdateHighlightingFromDataProvider(ILSLLibraryDataProvider provider)
        {
            using (var resourceStream = GetType().Assembly.GetManifestResourceStream(GetType().Namespace+".LSL.xshd"))
            {
                if (resourceStream != null)
                {
                    var reader = new XmlTextReader(resourceStream);

                    Editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                    foreach (
                        var funcs in
                            (from s in provider.LibraryFunctions.Where(x => x.Count > 0)
                                orderby s.First().Name.Length descending
                                select s))
                    {
                        var name = funcs.First().Name;

                        var colorName = "Functions";

                        if (funcs.All(f => f.Deprecated))
                        {
                            colorName = "DeprecatedFunctions";
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