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

        private readonly Brush _eventHandlerCompleteColor = new SolidColorBrush(Color.FromRgb(0, 76, 127));
        private readonly Brush _globalFunctionCompleteColor = new SolidColorBrush(Color.FromRgb(153, 0, 204));
        private readonly Brush _globalVariableCompleteColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private readonly ToolTip _hoverToolTip = new ToolTip();
        private readonly Brush _libraryConstantCompleteColor = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private readonly Brush _libraryFunctionCompleteColor = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private readonly Brush _localParameterCompleteColor = new SolidColorBrush(Color.FromRgb(0, 102, 0));
        private readonly Brush _localVariableCompleteColor = new SolidColorBrush(Color.FromRgb(0, 102, 255));

        private readonly Brush _labelNameDefinitionCompleteColor = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private readonly Brush _stateNameCompleteColor = new SolidColorBrush(Colors.Black);
        private readonly Brush _labelNameJumpTargetCompleteColor = new SolidColorBrush(Colors.Black);

        private readonly object _propertyChangingLock = new object();

        private readonly HashSet<char> _stateAutocompleteIndentBreakCharacters = new HashSet<char>
        {
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
                    _hoverToolTip.IsOpen = false;
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
                    _hoverToolTip.IsOpen = false;
                    return;
                }

                _hoverToolTip.PlacementTarget = this; // required for property inheritance
                _hoverToolTip.Content = new TextBlock
                {
                    MaxWidth = 500,
                    TextWrapping = TextWrapping.Wrap,
                    Text = hoverText
                };
                _hoverToolTip.IsOpen = true;
                e.Handled = true;
            }
        }


        private void TextEditor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            _hoverToolTip.IsOpen = false;
            e.Handled = true;
        }


        private bool LSLTypeNameBehindOffset(string text, int caretOffset)
        {
            var behindOffset = caretOffset > 1 ? caretOffset - 1 : -1;

            while (behindOffset > 0)
            {
                var c = text[behindOffset];

                if (char.IsWhiteSpace(c))
                {
                    behindOffset--;
                    continue;
                }


                if ((c == 'r' || c == 'g'))
                {
                    string t;

                    if (behindOffset >= 6)
                    {
                        t = text.Substring(behindOffset - 6, 7);

                        if (t == "integer")
                        {
                            return true;
                        }
                    }
                    if (behindOffset >= 5)
                    {
                        t = text.Substring(behindOffset - 5, 6);
                        if (t == "vector" || t == "string")
                        {
                            return true;
                        }
                    }
                }
                else if (c == 't')
                {
                    string t;
                    if (behindOffset >= 4)
                    {
                        t = text.Substring(behindOffset - 4, 5);
                        if (t == "float")
                        {
                            return true;
                        }
                    }
                    if (behindOffset >= 3)
                    {
                        t = text.Substring(behindOffset - 3, 4);
                        if (t == "list")
                        {
                            return true;
                        }
                    }
                }
                else if (behindOffset >= 7 && c == 'n')
                {
                    var t = text.Substring(behindOffset - 7, 8);
                    if (t == "rotation")
                    {
                        return true;
                    }
                }
                else if (!char.IsWhiteSpace(c))
                {
                    return false;
                }

                behindOffset--;
            }

            return false;
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

        // ReSharper disable once FunctionComplexityOverflow
        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
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


            if (e.Text == "}" && textArea.Document.GetText(caretOffset - 1, 1) == "\t")
            {
                var offset = caretOffset - 1;
                while (offset > 0)
                {
                    var text = textArea.Document.GetText(offset, 1);
                    if (text == ";" || text == "{" || text == "}")
                    {
                        textArea.Document.Remove(caretOffset - 1, 1);
                        return;
                    }
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        break;
                    }

                    offset--;
                }
            }


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



            var behind = "";


            if (caretOffset > 0) behind = Editor.Document.GetText(caretOffset - 1, 1);


            if (!_validSuggestionPrefixes.Contains(behind))
            {
                return;
            }


            lock (_completionLock)
            {
                var commentSkipper = new LSLCommentStringSkipper(Editor.Text, caretOffset);


                if (commentSkipper.InComment || commentSkipper.InString ||
                    LSLTypeNameBehindOffset(Editor.Text, caretOffset))
                {
                    return;
                }


                var fastVarParser = new LSLAutoCompleteParser();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);
                //_debugObjectView.ViewObject("", fastVarParser);

                IList<ICompletionData> data = null;


                if (fastVarParser.CanSuggestEventHandler)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;


                    var possibleEventName = false;
                    foreach (var eventHandler in EventSignatures.Where(x => x.Name.StartsWith(e.Text)))
                    {
                        data.Add(new LSLCompletionData(
                            eventHandler.Name,
                            eventHandler.Name, eventHandler.SignatureAndDocumentation, 0)
                        {
                            AppendOnInsert =
                                eventHandler.SignatureString.Substring(eventHandler.Name.Length) + "\n{\n\t\n}",
                            ColorBrush = _eventHandlerCompleteColor,
                            ForceIndent = true,
                            IndentLevel = 1,
                            OffsetCaretAfterInsert = true,
                            CaretOffsetAfterInsert = -3,
                            InsertTextAtCaretAfterOffset = true,
                            CaretOffsetInsertionText = "\t",
                            IndentBreakCharacters = _stateAutocompleteIndentBreakCharacters
                        });
                        possibleEventName = true;
                    }

                    if (!possibleEventName)
                    {
                        CurrentCompletionWindow = null;
                        return;
                    }

                    CurrentCompletionWindow.Show();
                    return;
                }


                if (fastVarParser.CanSuggestStateName)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;


                    data.Add(new LSLCompletionData("default", "default", "Default script state", 0)
                    {
                        AppendOnInsert = ";",
                        ColorBrush = _stateNameCompleteColor
                    });

                    foreach (var state in fastVarParser.StateBlocks)
                    {
                        data.Add(new LSLCompletionData(state.Name, state.Name, "Script state", 0)
                        {
                            AppendOnInsert = ";",
                            ColorBrush = _stateNameCompleteColor
                        });
                    }

                    CurrentCompletionWindow.Show();
                    return;
                }


                if (fastVarParser.CanSuggestLabelNameJumpTarget)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;


                    var possibleLabelName = false;
                    foreach (var label in fastVarParser.GetLocalLabels(Editor.Text))
                    {
                        data.Add(new LSLCompletionData(label.Name, label.Name, "In scope label", 0)
                        {
                            AppendOnInsert = ";",
                            ColorBrush = _labelNameJumpTargetCompleteColor
                        });
                        possibleLabelName = true;
                    }

                    if (!possibleLabelName)
                    {
                        CurrentCompletionWindow = null;
                        return;
                    }

                    CurrentCompletionWindow.Show();
                    return;
                }


                if (fastVarParser.CanSuggestLabelNameDefinition)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;


                    var possibleLabelName = false;
                    foreach (var label in fastVarParser.GetLocalJumps(Editor.Text))
                    {
                        data.Add(new LSLCompletionData(label.Target, label.Target, "In scope jump", 0)
                        {
                            AppendOnInsert = ";",
                            ColorBrush = _labelNameDefinitionCompleteColor
                        });
                        possibleLabelName = true;
                    }

                    if (!possibleLabelName)
                    {
                        CurrentCompletionWindow = null;
                        return;
                    }

                    CurrentCompletionWindow.Show();
                    return;
                }


                var possibleLibraryFunction = false;
                var possibleUserDefinedItem = false;
                var possibleType = false;
                var possibleConstant = false;


                if (fastVarParser.CanSuggestTypeName)
                {
                    if (e.Text.StartsWith("i"))
                    {
                        CurrentCompletionWindow = LazyInitCompletionWindow();
                        data = CurrentCompletionWindow.CompletionList.CompletionData;

                        data.Add(new LSLCompletionData("integer", "integer",
                            "integer type", 0)
                        {
                            ColorBrush = _builtInTypeCompleteColor
                        });
                        possibleType = true;
                    }
                    else if (e.Text.StartsWith("s"))
                    {
                        CurrentCompletionWindow = LazyInitCompletionWindow();
                        data = CurrentCompletionWindow.CompletionList.CompletionData;

                        data.Add(new LSLCompletionData("string", "string",
                            "string type", 0)
                        {
                            ColorBrush = _builtInTypeCompleteColor
                        });
                        possibleType = true;
                    }
                    else if (e.Text.StartsWith("v"))
                    {
                        CurrentCompletionWindow = LazyInitCompletionWindow();
                        data = CurrentCompletionWindow.CompletionList.CompletionData;

                        data.Add(new LSLCompletionData("vector", "vector",
                            "vector type", 0)
                        {
                            ColorBrush = _builtInTypeCompleteColor
                        });
                        possibleType = true;
                    }
                    else if (e.Text.StartsWith("r"))
                    {
                        CurrentCompletionWindow = LazyInitCompletionWindow();
                        data = CurrentCompletionWindow.CompletionList.CompletionData;

                        data.Add(new LSLCompletionData("rotation", "rotation",
                            "rotation type", 0)
                        {
                            ColorBrush = _builtInTypeCompleteColor
                        });
                        possibleType = true;
                    }
                    else if (e.Text.StartsWith("k"))
                    {
                        CurrentCompletionWindow = LazyInitCompletionWindow();
                        data = CurrentCompletionWindow.CompletionList.CompletionData;

                        data.Add(new LSLCompletionData("key", "key",
                            "key type", 0)
                        {
                            ColorBrush = _builtInTypeCompleteColor
                        });
                        possibleType = true;
                    }
                    else if (e.Text.StartsWith("f"))
                    {
                        CurrentCompletionWindow = LazyInitCompletionWindow();
                        data = CurrentCompletionWindow.CompletionList.CompletionData;

                        data.Add(new LSLCompletionData("float", "float",
                            "float type", 0)
                        {
                            ColorBrush = _builtInTypeCompleteColor
                        });
                        possibleType = true;
                    }
                }


                if (fastVarParser.CanSuggestGlobalVariable)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;

                    foreach (var v in fastVarParser.GlobalVariables.Where(x => x.Name.StartsWith(e.Text)))
                    {
                        var doc = "Global variable:\n" + v.Type + " " + v.Name + ";";
                        data.Add(new LSLCompletionData(v.Name, v.Name,
                            doc, 1)
                        {
                            ColorBrush = _globalVariableCompleteColor
                        });
                        possibleUserDefinedItem = true;
                    }
                }


                if (fastVarParser.CanSuggestFunction)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;


                    foreach (var func in fastVarParser.GlobalFunctions.Where(x => x.Name.StartsWith(e.Text)))
                    {
                        var doc = "Global function:\n" + func.Signature;

                        data.Add(new LSLCompletionData(func.Name, func.Name, doc, 2)
                        {
                            AppendOnInsert = "(",
                            ColorBrush = _globalFunctionCompleteColor
                        });

                        possibleUserDefinedItem = true;
                    }
                }


                if (fastVarParser.CanSuggestLocalVariableOrParameter)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;


                    foreach (var v in fastVarParser.LocalParameters.Where(x => x.Name.StartsWith(e.Text)))
                    {
                        var doc = "Local parameter:\n" + v.Type + " " + v.Name + ";";

                        data.Add(new LSLCompletionData(v.Name, v.Name, doc, 3)
                        {
                            ColorBrush = _localParameterCompleteColor
                        });
                        possibleUserDefinedItem = true;
                    }

                    foreach (var v in fastVarParser.LocalVariables
                        .Where(x => x.Name.StartsWith(e.Text)))
                    {
                        var doc = "Local variable:\n" + v.Type + " " + v.Name + ";";

                        data.Add(new LSLCompletionData(v.Name, v.Name, doc, 4)
                        {
                            ColorBrush = _localVariableCompleteColor
                        });

                        possibleUserDefinedItem = true;
                    }
                }

                if (fastVarParser.CanSuggestLibraryConstant)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;


                    foreach (var sig in ConstantSignatures.Where(x => x.Name.StartsWith(e.Text)))
                    {
                        data.Add(new LSLCompletionData(sig.Name, sig.Name,
                            sig.SignatureAndDocumentation, 5)
                        {
                            ColorBrush = _libraryConstantCompleteColor
                        });

                        possibleConstant = true;
                    }
                }


                if (fastVarParser.CanSuggestFunction)
                {
                    CurrentCompletionWindow = LazyInitCompletionWindow();
                    data = CurrentCompletionWindow.CompletionList.CompletionData;


                    var functionSuggestions = LibraryFunctionNames.Where(x => x.StartsWith(e.Text));

                    foreach (var func in functionSuggestions)
                    {
                        var docs = string.Join(Environment.NewLine + Environment.NewLine,
                            LibraryDataProvider.GetLibraryFunctionSignatures(func)
                                .Select(x => x.SignatureAndDocumentation));


                        data.Add(new LSLCompletionData(func, func, docs, 6)
                        {
                            AppendOnInsert = "(",
                            ColorBrush = _libraryFunctionCompleteColor
                        });


                        possibleLibraryFunction = true;
                    }
                }


                if (!possibleConstant && !possibleLibraryFunction && !possibleType && !possibleUserDefinedItem)
                {
                    CurrentCompletionWindow = null;
                    return;
                }

                CurrentCompletionWindow.Show();
            }
        }


        private void SuggestUserDefinedOrEvent()
        {
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


                var behind = caretOffset > 0 ? Editor.Document.GetText(caretOffset - 1, 1) : "";


                if (!_validSuggestionPrefixes.Contains(behind)) return;


                var commentSkipper = new LSLCommentStringSkipper(Editor.Text, caretOffset);


                if (commentSkipper.InComment || commentSkipper.InString ||
                    LSLTypeNameBehindOffset(Editor.Text, caretOffset)) return;


                CurrentCompletionWindow = CreateNewCompletionWindow();


                var fastVarParser = new LSLAutoCompleteParser();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);

                var data = CurrentCompletionWindow.CompletionList.CompletionData;


                if (fastVarParser.CanSuggestEventHandler)
                {
                    var possibleEventName = false;

                    foreach (var eventHandler in EventSignatures)
                    {
                        data.Add(new LSLCompletionData(
                            eventHandler.Name,
                            eventHandler.Name,
                            eventHandler.SignatureAndDocumentation, 0)
                        {
                            AppendOnInsert =
                                eventHandler.SignatureString.Substring(eventHandler.Name.Length) + "\n{\n\t\n}",
                            ColorBrush = _eventHandlerCompleteColor,
                            ForceIndent = true,
                            IndentLevel = 1,
                            OffsetCaretAfterInsert = true,
                            CaretOffsetAfterInsert = -3,
                            InsertTextAtCaretAfterOffset = true,
                            CaretOffsetInsertionText = "\t",
                            IndentBreakCharacters = _stateAutocompleteIndentBreakCharacters
                        });
                        possibleEventName = true;
                    }

                    if (!possibleEventName)
                    {
                        CurrentCompletionWindow = null;
                        return;
                    }

                    CurrentCompletionWindow.Show();
                }


                if (fastVarParser.CanSuggestStateName)
                {
                    data.Add(new LSLCompletionData("default", "default", "Default script state", 0)
                    {
                        AppendOnInsert = ";",
                        ColorBrush = _stateNameCompleteColor
                    });

                    foreach (var state in fastVarParser.StateBlocks)
                    {
                        data.Add(new LSLCompletionData(state.Name, state.Name, "Script state", 0)
                        {
                            AppendOnInsert = ";",
                            ColorBrush = _stateNameCompleteColor
                        });
                    }

                    CurrentCompletionWindow.Show();
                    return;
                }


                if (fastVarParser.CanSuggestLabelNameJumpTarget)
                {
                    var possibleLabelName = false;
                    foreach (var label in fastVarParser.GetLocalLabels(Editor.Text))
                    {
                        data.Add(new LSLCompletionData(label.Name, label.Name, "In scope label", 0)
                        {
                            AppendOnInsert = ";",
                            ColorBrush = _labelNameJumpTargetCompleteColor
                        });
                        possibleLabelName = true;
                    }

                    if (!possibleLabelName)
                    {
                        CurrentCompletionWindow = null;
                        return;
                    }

                    CurrentCompletionWindow.Show();
                    return;
                }


                if (fastVarParser.CanSuggestLabelNameDefinition)
                {
                    var possibleLabelName = false;
                    foreach (var label in fastVarParser.GetLocalJumps(Editor.Text))
                    {
                        data.Add(new LSLCompletionData(label.Target, label.Target, "In scope jump", 0)
                        {
                            AppendOnInsert = ";",
                            ColorBrush = _labelNameDefinitionCompleteColor
                        });
                        possibleLabelName = true;
                    }

                    if (!possibleLabelName)
                    {
                        CurrentCompletionWindow = null;
                        return;
                    }

                    CurrentCompletionWindow.Show();
                    return;
                }


                var possibleUserDefinedItem = false;

                if (fastVarParser.CanSuggestGlobalVariable)
                {
                    foreach (var i in fastVarParser.GlobalVariables)
                    {
                        data.Add(new LSLCompletionData(i.Name, i.Name,
                            "Global variable:\n" + i.Type + " " + i.Name, 0)
                        {
                            ColorBrush = _globalVariableCompleteColor
                        });
                        possibleUserDefinedItem = true;
                    }
                }


                if (fastVarParser.CanSuggestFunction)
                {
                    foreach (var i in fastVarParser.GlobalFunctions)
                    {
                        data.Add(new LSLCompletionData(
                            i.Name,
                            i.Name, "Global function:\n" + i.Signature, 1)
                        {
                            AppendOnInsert = "(",
                            ColorBrush = _globalFunctionCompleteColor
                        });
                        possibleUserDefinedItem = true;
                    }
                }

                if (fastVarParser.CanSuggestLocalVariableOrParameter)
                {
                    foreach (var i in fastVarParser.LocalParameters)
                    {
                        data.Add(new LSLCompletionData(i.Name, i.Name,
                            "Local parameter:\n" + i.Type + " " + i.Name, 2)
                        {
                            ColorBrush = _localParameterCompleteColor
                        });
                        possibleUserDefinedItem = true;
                    }

                    foreach (var i in fastVarParser.LocalVariables)
                    {
                        data.Add(new LSLCompletionData(i.Name, i.Name,
                            "Local Variable:\n" + i.Type + " " + i.Name, 3)
                        {
                            ColorBrush = _localVariableCompleteColor
                        });
                        possibleUserDefinedItem = true;
                    }
                }


                if (!possibleUserDefinedItem)
                {
                    CurrentCompletionWindow = null;
                    return;
                }

                CurrentCompletionWindow.Show();
            }
        }


        private void SuggestLibraryFunctions()
        {
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


                var lookBehind = caretOffset > 0 ? Editor.Document.GetText(caretOffset - 1, 1) : "";


                if (!_validSuggestionPrefixes.Contains(lookBehind)) return;


                var commentSkipper = new LSLCommentStringSkipper(Editor.Text, caretOffset);


                if (commentSkipper.InComment || commentSkipper.InString ||
                    LSLTypeNameBehindOffset(Editor.Text, caretOffset)) return;


                CurrentCompletionWindow = CreateNewCompletionWindow();

                var fastVarParser = new LSLAutoCompleteParser();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);


                var possibleGlobalFunctions = false;


                if (fastVarParser.CanSuggestFunction)
                {
                    foreach (var func in LibraryFunctionNames)
                    {
                        var docs = string.Join(Environment.NewLine + Environment.NewLine,
                            LibraryDataProvider.GetLibraryFunctionSignatures(func)
                                .Select(x => x.SignatureAndDocumentation));


                        CurrentCompletionWindow.CompletionList.CompletionData.Add(new LSLCompletionData(func, func, docs,
                            0)
                        {
                            AppendOnInsert = "(",
                            ColorBrush = _libraryFunctionCompleteColor
                        });

                        possibleGlobalFunctions = true;
                    }
                }


                if (!possibleGlobalFunctions)
                {
                    CurrentCompletionWindow = null;
                    return;
                }

                CurrentCompletionWindow.Show();
            }
        }


        private void SuggestLibraryConstants()
        {
            var textArea = Editor.TextArea;
            var caretOffset = textArea.Caret.Offset;

#if DEBUG_FASTPARSER
            var P = new LSLFastEditorParse();
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

                var lookBehind = caretOffset > 0 ? Editor.Document.GetText(caretOffset - 1, 1) : "";


                if (!_validSuggestionPrefixes.Contains(lookBehind)) return;


                var commentSkipper = new LSLCommentStringSkipper(Editor.Text, caretOffset);


                if (commentSkipper.InComment || commentSkipper.InString ||
                    LSLTypeNameBehindOffset(Editor.Text, caretOffset)) return;


                CurrentCompletionWindow = CreateNewCompletionWindow();


                var fastVarParser = new LSLAutoCompleteParser();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);


                var possibleLibraryConstants = false;


                if (fastVarParser.CanSuggestLibraryConstant)
                {
                    foreach (var con in ConstantSignatures)
                    {
                        CurrentCompletionWindow.CompletionList.CompletionData.Add(
                            new LSLCompletionData(con.Name,
                                con.Name,
                                con.SignatureAndDocumentation, 0)
                            {
                                ColorBrush = _libraryConstantCompleteColor
                            });

                        possibleLibraryConstants = true;
                    }
                }


                if (!possibleLibraryConstants)
                {
                    CurrentCompletionWindow = null;
                    return;
                }

                CurrentCompletionWindow.Show();
            }
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
            _hoverToolTip.IsOpen = false;
        }


        private void TextEditor_OnMouseLeave(object sender, MouseEventArgs e)
        {
            _hoverToolTip.IsOpen = false;
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