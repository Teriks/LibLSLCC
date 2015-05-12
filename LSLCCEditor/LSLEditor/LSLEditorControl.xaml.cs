using System;
using System.Collections.Generic;
using System.Globalization;
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
using LibLSLCC.CodeValidator.Components;
using LibLSLCC.CodeValidator.Components.Interfaces;
using LibLSLCC.FastEditorParse;

namespace LSLCCEditor.LSLEditor
{
    /// <summary>
    ///     Interaction logic for Test.xaml
    /// </summary>
    public partial class LSLEditorControl : UserControl
    {
        public delegate void TextChangedEventHandler(object sender, EventArgs e);

        private readonly ToolTip _hoverToolTip = new ToolTip();

        private readonly object _propertyChangingLock = new object();
        private readonly object _completionLock = new object();

        private readonly SolidColorBrush _builtInTypeCompleteColor = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private readonly SolidColorBrush _eventHandlerCompleteColor = new SolidColorBrush(Color.FromRgb(0, 76, 127));
        private readonly SolidColorBrush _globalFunctionCompleteColor = new SolidColorBrush(Color.FromRgb(153, 0, 204));
        private readonly SolidColorBrush _globalVariableCompleteColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private readonly SolidColorBrush _libraryConstantCompleteColor = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private readonly SolidColorBrush _libraryFunctionCompleteColor = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private readonly SolidColorBrush _localParameterCompleteColor = new SolidColorBrush(Color.FromRgb(102, 153, 0));
        private readonly SolidColorBrush _localVariableCompleteColor = new SolidColorBrush(Color.FromRgb(0, 102, 255));
        

        private readonly HashSet<char> _stateAutocompleteIndentBreakCharacters = new HashSet<char>
        {
            '{',
            '}'
        };

        private readonly object _userChangingLock = new object();

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
            "<",
            ",",
            ";",
            "=",
            "+",
            "-",
            "*",
            "/",
            "%"
        };

        private CompletionWindow _completionWindow;
        private bool _propertyChanging;
        private bool _userChanging;



        public LSLEditorControl()
        {
            InitializeComponent();

            Editor.TextArea.TextEntered += TextArea_TextEntered;
            Editor.MouseHover += TextEditor_MouseHover;
            Editor.MouseHover += TextEditor_MouseHoverStopped;
            Editor.KeyDown += TextEditor_KeyDown;

        }



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



        private static void TextPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var t = (LSLEditorControl) dependencyObject;
            if (!t._userChanging && t.Editor.Document != null)
            {
                t._propertyChanging = true;
                t.Editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue == null
                    ? ""
                    : dependencyPropertyChangedEventArgs.NewValue.ToString();
                t._propertyChanging = false;
            }
        }



        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            OnTextChanged();
            lock (_propertyChangingLock)
            {
                if (_propertyChanging)
                {
                    return;
                }
            }
            lock (_userChangingLock)
            {
                var editor = (TextEditor) sender;
                _userChanging = true;
                Text = editor.Text;
                _userChanging = false;
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



        public void CloseCompletionWindow()
        {
            lock (_completionLock)
            {
                if (_completionWindow != null)
                {
                    _completionWindow.Close();
                    _completionWindow = null;
                }
            }
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



        private static string GetIdUnderMouse(TextDocument document, TextViewPosition position)
        {
            try
            {
                return _GetIDUnderMouse(document, position);
            }
            catch (ArgumentOutOfRangeException)
            {
                return "";
            }
        }



        private static string _GetIDUnderMouse(TextDocument document, TextViewPosition position)
        {
            var wordHovered = string.Empty;

            var line = position.Line;
            var column = position.Column;

            var offset = document.GetOffset(line, column);
            if (offset >= document.TextLength)
                offset--;

            var textAtOffset = document.GetText(offset, 1);


            // Get text backward of the mouse position, until the first space
            while (!(string.IsNullOrWhiteSpace(textAtOffset) || !Regex.Match(textAtOffset, "[_a-zA-Z0-9]").Success))
            {
                wordHovered = textAtOffset + wordHovered;

                offset--;

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

                while (!(string.IsNullOrWhiteSpace(textAtOffset) || !Regex.Match(textAtOffset, "[_a-zA-Z0-9]").Success))
                {
                    wordHovered = wordHovered + textAtOffset;

                    offset++;

                    if (offset >= document.TextLength)
                        break;

                    textAtOffset = document.GetText(offset, 1);
                }
            }

            if (Regex.Match(wordHovered, "^[_a-zA-Z]+[_a-zA-Z0-9]*$").Success)
            {
                return wordHovered;
            }
            return "";
        }



        private void TextEditor_MouseHover(object sender, MouseEventArgs e)
        {
            var pos = Editor.GetPositionFromPoint(e.GetPosition(Editor));

            if (pos != null)
            {
                var wordHovered = GetIdUnderMouse(Editor.Document, pos.Value);
                if (string.IsNullOrWhiteSpace(wordHovered))
                {
                    e.Handled = true;
                    _hoverToolTip.IsOpen = false;
                    return;
                }

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
                _hoverToolTip.Content = new TextBlock {MaxWidth = 500, TextWrapping = TextWrapping.Wrap, Text = hoverText};
                _hoverToolTip.IsOpen = true;
                e.Handled = true;
            }
        }



        private void TextEditor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            _hoverToolTip.IsOpen = false;
            e.Handled = true;
        }



        private CompletionWindow CreateCompletionWindow()
        {
            var c = new CompletionWindow(Editor.TextArea);
           
            c.Width = c.Width + 160;
            return c;
        }



        // ReSharper disable once FunctionComplexityOverflow
        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            lock (_completionLock)
            {
                if (_completionWindow != null && _completionWindow.CompletionList.ListBox.Items.Count == 0)
                {
                    _completionWindow.Close();
                    _completionWindow = null;
                }
            }


            var behind = "";

            if (Editor.TextArea.Caret.Offset > 2)
            {
                behind =
                    Editor.Document.GetText(Editor.TextArea.Caret.Offset - 3, 3);
            }
            else if (Editor.TextArea.Caret.Offset > 1)
            {
                behind =
                    Editor.Document.GetText(Editor.TextArea.Caret.Offset - 2, 2);
            }

            var onebehind = (behind.Length > 2 ? behind[1] : (behind.Length > 1 ? behind[0] : ' '))
                .ToString(CultureInfo.InvariantCulture);


            lock (_completionLock)
            {
                if (!_validSuggestionPrefixes.Contains(onebehind) || !Regex.Match(e.Text, "[A-Za-z]").Success ||
                    _completionWindow != null) return;


                var textArea = Editor.TextArea;
                var caretOffset = textArea.Caret.Offset;

                var commentSkipper = new LSLCommentStringSkipper();

                for (int i = 0; i < caretOffset; i++)
                {
                    commentSkipper.FeedChar(Editor.Text, i, caretOffset);
                }

                if (commentSkipper.InComment || commentSkipper.InString)
                {
                    return;
                }

                _completionWindow = CreateCompletionWindow();


                var fastVarParser = new LSLFastEditorParse();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);


                if (fastVarParser.InVariableDeclarationStart) return;

                var data = _completionWindow.CompletionList.CompletionData;


                bool possibleLibraryFunction = false;
                bool possibleUserDefinedItem = false;
                bool possibleType = false;
                bool possibleConstant = false;
                bool possibleEventName = false;


                if (fastVarParser.InStateOutsideEvent)
                {
                    foreach (var eventHandler in EventSignatures.Where(x => x.Name.StartsWith(e.Text)))
                    {
                        var sig = eventHandler.SignatureString + "\n{\n\t\n}";
                        data.Add(new LSLCompletionData(eventHandler.Name, sig, eventHandler.SignatureAndDocumentation, 0)
                        {
                            ColorBrush = _eventHandlerCompleteColor,
                            ForceIndent = true,
                            IndentLevel = 1,
                            TextSubStringStart = 0,
                            OffsetCaretAfterInsert = true,
                            CaretOffsetAfterInsert = -3,
                            InsertTextAtCaretAfterOffset = true,
                            CaretOffsetInsertionText = "\t",
                            IndentBreakCharacters = _stateAutocompleteIndentBreakCharacters
                        });
                        possibleEventName = true;
                    }
                }
                else
                {

                    if (!fastVarParser.AfterDefaultState)
                    {
                        if (e.Text.StartsWith("i"))
                        {
                            data.Add(new LSLCompletionData("integer", "integer",
                                "integer type", 0) {ColorBrush = _builtInTypeCompleteColor});
                            possibleType = true;
                        }
                        else if (e.Text.StartsWith("s"))
                        {
                            data.Add(new LSLCompletionData("string", "string",
                                "string type", 0) {ColorBrush = _builtInTypeCompleteColor});
                            possibleType = true;
                        }
                        else if (e.Text.StartsWith("v"))
                        {
                            data.Add(new LSLCompletionData("vector", "vector",
                                "vector type", 0) {ColorBrush = _builtInTypeCompleteColor});
                            possibleType = true;
                        }
                        else if (e.Text.StartsWith("r"))
                        {
                            data.Add(new LSLCompletionData("rotation", "rotation",
                                "rotation type", 0) {ColorBrush = _builtInTypeCompleteColor});
                            possibleType = true;
                        }
                        else if (e.Text.StartsWith("k"))
                        {
                            data.Add(new LSLCompletionData("key", "key",
                                "key type", 0) {ColorBrush = _builtInTypeCompleteColor});
                            possibleType = true;
                        }
                        else if (e.Text.StartsWith("f"))
                        {
                            data.Add(new LSLCompletionData("float", "float",
                                "float type", 0) {ColorBrush = _builtInTypeCompleteColor});
                            possibleType = true;
                        }
                    }


                    if (fastVarParser.InFunctionDeclaration || fastVarParser.InEventHandler ||
                        fastVarParser.InGlobalScope)
                    {
                        foreach (var v in fastVarParser.GlobalVariables.Where(x => x.Name.StartsWith(e.Text)))
                        {
                            string doc = "Global variable:\n" + v.Type + " " + v.Name + ";";
                            data.Add(new LSLCompletionData(v.Name, v.Name,
                                doc, 1)
                            {
                                ColorBrush = _globalVariableCompleteColor
                            });
                            possibleUserDefinedItem = true;
                        }
                    }


                    if (fastVarParser.InFunctionDeclaration || fastVarParser.InEventHandler)
                    {

                        foreach (var func in fastVarParser.GlobalFunctions.Where(x => x.Name.StartsWith(e.Text)))
                        {
                            string doc = "Global function:\n" + func.Signature;

                            data.Add(new LSLCompletionData(func.Name, func.Name + "(", doc, 2)
                            {
                                ColorBrush = _globalFunctionCompleteColor
                            });

                            possibleUserDefinedItem = true;
                        }


                        foreach (var v in fastVarParser.LocalParameters.Where(x => x.Name.StartsWith(e.Text)))
                        {
                            string doc = "Local parameter:\n" + v.Type + " " + v.Name + ";";

                            data.Add(new LSLCompletionData(v.Name, v.Name, doc, 3)
                            {
                                ColorBrush = _localParameterCompleteColor
                            });
                            possibleUserDefinedItem = true;
                        }

                        foreach (var v in fastVarParser.LocalVariables
                            .Where(x => x.Name.StartsWith(e.Text)))
                        {
                            string doc = "Local variable:\n" + v.Type + " " + v.Name + ";";

                            data.Add(new LSLCompletionData(v.Name, v.Name, doc, 4)
                            {
                                ColorBrush = _localVariableCompleteColor
                            });

                            possibleUserDefinedItem = true;
                        }


                    }

                    if (!fastVarParser.AfterDefaultState)
                    {
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


                    if(fastVarParser.InFunctionDeclaration || fastVarParser.InEventHandler)
                    {

                        var functionSuggestions = LibraryFunctionNames.Where(x => x.StartsWith(e.Text)).ToList();
                        foreach (var func in functionSuggestions)
                        {
                            var docs = string.Join(Environment.NewLine + Environment.NewLine,
                                LibraryDataProvider.GetLibraryFunctionSignatures(func)
                                    .Select(x => x.SignatureAndDocumentation));


                            data.Add(new LSLCompletionData(func, func + "(", docs, 6)
                            {
                                ColorBrush = _libraryFunctionCompleteColor
                            });


                            possibleLibraryFunction = true;
                        }
                    }
                }


                if (!possibleConstant && !possibleLibraryFunction && !possibleType && !possibleUserDefinedItem &&
                    !possibleEventName)
                {
                    _completionWindow = null;
                    return;
                }

                _completionWindow.Show();
                _completionWindow.Closed += (o, args) =>
                {
                    lock (_completionLock)
                    {
                        _completionWindow = null;
                    }
                };
            }
        }



        private void ControlSpace(KeyEventArgs e)
        {
            var textArea = Editor.TextArea;

            lock (_completionLock)
            {
                if (_completionWindow != null)
                {
                    _completionWindow.CompletionList.RequestInsertion(e);
                    e.Handled = true;
                }
                else
                {
                    var caretOffset = textArea.Caret.Offset;

                    var commentSkipper = new LSLCommentStringSkipper();

                    for (int i = 0; i < caretOffset; i++)
                    {
                        commentSkipper.FeedChar(Editor.Text, i, caretOffset);
                    }

                    if (commentSkipper.InComment || commentSkipper.InString)
                    {
                        return;
                    }

                    _completionWindow = CreateCompletionWindow();


                    var fastVarParser = new LSLFastEditorParse();
                    fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);

                    if (fastVarParser.InVariableDeclarationStart) return;

                    var data = _completionWindow.CompletionList.CompletionData;


                    bool possibleUserDefinedItem = false;
                    bool possibleEventName = false;

                    if (fastVarParser.InStateOutsideEvent)
                    {
                        foreach (var eventHandler in EventSignatures)
                        {
                            var sig = eventHandler.SignatureString + "\n{\n\t\n}";
                            data.Add(new LSLCompletionData(eventHandler.Name, sig,
                                eventHandler.SignatureAndDocumentation, 0)
                            {
                                ColorBrush = _eventHandlerCompleteColor,
                                ForceIndent = true,
                                IndentLevel = 1,
                                TextSubStringStart = 0,
                                OffsetCaretAfterInsert = true,
                                CaretOffsetAfterInsert = -3,
                                InsertTextAtCaretAfterOffset = true,
                                CaretOffsetInsertionText = "\t",
                                IndentBreakCharacters = _stateAutocompleteIndentBreakCharacters
                            });
                            possibleEventName = true;
                        }


                    }
                    else
                    {

                        if (fastVarParser.InEventHandler || fastVarParser.InFunctionDeclaration ||
                            fastVarParser.InGlobalScope)
                        {
                            foreach (var i in fastVarParser.GlobalVariables)
                            {
                                data.Add(new LSLCompletionData(i.Name, i.Name,
                                    "Global variable:\n" + i.Type + " " + i.Name, 0)
                                {
                                    TextSubStringStart = 0,
                                    ColorBrush = _globalVariableCompleteColor
                                });
                                possibleUserDefinedItem = true;
                            }
                        }


                        if (fastVarParser.InEventHandler || fastVarParser.InFunctionDeclaration)
                        {

                            foreach (var i in fastVarParser.GlobalFunctions)
                            {
                                data.Add(new LSLCompletionData(
                                    i.Name,
                                    i.Name + "(", "Global function:\n" + i.Signature, 1)
                                {
                                    TextSubStringStart = 0,
                                    ColorBrush = _globalFunctionCompleteColor
                                });
                                possibleUserDefinedItem = true;
                            }

                            foreach (var i in fastVarParser.LocalParameters)
                            {
                                data.Add(new LSLCompletionData(i.Name, i.Name,
                                    "Local parameter:\n" + i.Type + " " + i.Name, 2)
                                {
                                    TextSubStringStart = 0,
                                    ColorBrush = _localParameterCompleteColor
                                });
                                possibleUserDefinedItem = true;
                            }

                            foreach (var i in fastVarParser.LocalVariables)
                            {
                                data.Add(new LSLCompletionData(i.Name, i.Name,
                                    "Local Variable:\n" + i.Type + " " + i.Name, 3)
                                {
                                    TextSubStringStart = 0,
                                    ColorBrush = _localVariableCompleteColor
                                });
                                possibleUserDefinedItem = true;
                            }

                        }
                    }


                    _completionWindow.Closed += (o, args) =>
                    {
                        lock (_completionLock)
                        {
                            _completionWindow = null;
                        }
                    };

                    if (possibleUserDefinedItem || possibleEventName)
                    {
                        _completionWindow.Show();
                    }
                    else
                    {
                        _completionWindow = null;
                    }

                    e.Handled = true;
                }
            }
        }



        private void ControlW(KeyEventArgs e)
        {
            var textArea = Editor.TextArea;

            lock (_completionLock)
            {
                if (_completionWindow != null) return;

                var caretOffset = textArea.Caret.Offset;

                var commentSkipper = new LSLCommentStringSkipper();

                for (int i = 0; i < caretOffset; i++)
                {
                    commentSkipper.FeedChar(Editor.Text, i, caretOffset);
                }

                if (commentSkipper.InComment || commentSkipper.InString)
                {
                    return;
                }


                _completionWindow = CreateCompletionWindow();

                var fastVarParser = new LSLFastEditorParse();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);

                if (fastVarParser.InVariableDeclarationStart) return;

                bool possibleGlobalFunctions = false;
                if (fastVarParser.InEventHandler || fastVarParser.InFunctionDeclaration)
                {
                    foreach (var func in LibraryFunctionNames)
                    {
                        var docs = string.Join(Environment.NewLine + Environment.NewLine,
                            LibraryDataProvider.GetLibraryFunctionSignatures(func)
                                .Select(x => x.SignatureAndDocumentation));


                        _completionWindow.CompletionList.CompletionData.Add(new LSLCompletionData(func, func + "(", docs,
                            0)
                        {
                            ColorBrush = _libraryFunctionCompleteColor,
                            TextSubStringStart = 0
                        });

                        possibleGlobalFunctions = true;
                    }
                }

                _completionWindow.Closed += (o, args) => { _completionWindow = null; };

                if (possibleGlobalFunctions)
                {
                    _completionWindow.Show();
                }
                else
                {
                    _completionWindow = null;
                }
            }
        }



        private void ControlQ(KeyEventArgs e)
        {
            var textArea = Editor.TextArea;

            lock (_completionLock)
            {
                if (_completionWindow != null) return;

                var caretOffset = textArea.Caret.Offset;

                var commentSkipper = new LSLCommentStringSkipper();

                for (int i = 0; i < caretOffset; i++)
                {
                    commentSkipper.FeedChar(Editor.Text, i, caretOffset);
                }

                if (commentSkipper.InComment || commentSkipper.InString)
                {
                    return;
                }


                _completionWindow = CreateCompletionWindow();

                var fastVarParser = new LSLFastEditorParse();
                fastVarParser.Parse(new StringReader(Editor.Text), caretOffset);

                if (fastVarParser.InVariableDeclarationStart) return;

                bool possibleLibraryConstants = false;

                if (fastVarParser.InGlobalScope || fastVarParser.InFunctionDeclaration || fastVarParser.InEventHandler)
                {
                    foreach (var con in ConstantSignatures)
                    {
                        _completionWindow.CompletionList.CompletionData.Add(
                            new LSLCompletionData(con.Name,
                                con.Name,
                                con.SignatureAndDocumentation, 0)
                            {
                                ColorBrush = _libraryConstantCompleteColor,
                                TextSubStringStart = 0
                            });
                        possibleLibraryConstants = true;
                    }
                }

                _completionWindow.Closed += (o, args) => { _completionWindow = null; };

                if (possibleLibraryConstants)
                {
                    _completionWindow.Show();
                }
                else
                {
                    _completionWindow = null;
                }
            }
        }



        private void TextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ControlQ(e);
            }
            else if (e.Key == Key.W && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ControlW(e);
            }
            else if (e.Key == Key.Space && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ControlSpace(e);
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



        public class CustomIndentationStrategy : IIndentationStrategy
        {
            public void IndentLine(TextDocument document, DocumentLine line)
            {
            }



            public void IndentLines(TextDocument document, int beginLine, int endLine)
            {
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof (string), typeof (LSLEditorControl),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                TextPropertyChangedCallback));

        public static readonly DependencyProperty LibraryDataProviderProperty = DependencyProperty.Register(
            "LibraryDataProvider", typeof (ILSLMainLibraryDataProvider), typeof (LSLEditorControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                LibraryDataProviderPropertyChangedCallback));
    }
}