#region


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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


#endregion




namespace LSLCCEditor.LSLEditor
{
    /// <summary>
    ///     Interaction logic for LSLEditor.xaml
    /// </summary>
    public partial class LSLEditor : UserControl
    {
        private readonly object _completionLock = new object();
        private readonly ToolTip _hoverToolTip = new ToolTip();

        private readonly HashSet<string> _validSuggestionPrefixes = new HashSet<string>
        {
            "\t",
            "\r",
            "\n",
            " ",
            "{",
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
            "%",
        };


        private CompletionWindow _completionWindow;
        private List<LSLLibraryConstantSignature> _constantSignatures;
        private List<LSLLibraryEventSignature> _eventSignatures;
        private List<string> _libraryFunctionNames;



        public LSLEditor()
        {
            InitializeComponent();

            TextEditor.TextArea.TextEntered += TextArea_TextEntered;
            TextEditor.MouseHover += TextEditor_MouseHover;
            TextEditor.MouseHover += TextEditor_MouseHoverStopped;
            TextEditor.KeyDown += TextEditor_KeyDown;
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



        public ILSLMainLibraryDataProvider LibraryDataProvider { get; private set; }



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
            var pos = TextEditor.GetPositionFromPoint(e.GetPosition(TextEditor));

            if (pos != null)
            {
                var wordHovered = GetIdUnderMouse(TextEditor.Document, pos.Value);
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
                _hoverToolTip.Content = hoverText;
                _hoverToolTip.IsOpen = true;
                e.Handled = true;
            }
        }



        private void TextEditor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            _hoverToolTip.IsOpen = false;
            e.Handled = true;
        }



        private readonly SolidColorBrush _globalFunctionCompleteColor = new SolidColorBrush(Color.FromRgb(153, 0, 204));
        private readonly SolidColorBrush _globalVariableCompleteColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private readonly SolidColorBrush _localParameterCompleteColor = new SolidColorBrush(Color.FromRgb(102, 153, 0));
        private readonly SolidColorBrush _localVariableCompleteColor = new SolidColorBrush(Color.FromRgb(0, 102, 255));
        private readonly SolidColorBrush _libraryFunctionCompleteColor = new SolidColorBrush(Color.FromRgb(127, 0, 38));
        private readonly SolidColorBrush _libraryConstantCompleteColor = new SolidColorBrush(Color.FromRgb(50, 52, 138));
        private readonly SolidColorBrush _builtInTypeCompleteColor = new SolidColorBrush(Color.FromRgb(50, 52, 138));



        private CompletionWindow CreateCompletionWindow()
        {
            var c = new CompletionWindow(this.TextEditor.TextArea);
            c.Width = c.Width + 160;
            return c;
        }



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

            if (TextEditor.TextArea.Caret.Offset > 2)
            {
                behind =
                    TextEditor.Document.GetText(TextEditor.TextArea.Caret.Offset - 3, 3);
            }
            else if (TextEditor.TextArea.Caret.Offset > 1)
            {
                behind =
                    TextEditor.Document.GetText(TextEditor.TextArea.Caret.Offset - 2, 2);
            }

            var onebehind = (behind.Length > 2 ? behind[1] : (behind.Length > 1 ? behind[0] : ' '))
                .ToString(CultureInfo.InvariantCulture);


            lock (_completionLock)
            {
                if (!_validSuggestionPrefixes.Contains(onebehind) || !Regex.Match(e.Text, "[A-Za-z]").Success ||
                    _completionWindow != null) return;


                var textArea = TextEditor.TextArea;
                var caretOffset = textArea.Caret.Offset;

                var commentSkipper = new LSLCommentStringSkipper();

                for (int i = 0; i < caretOffset; i++)
                {
                    commentSkipper.FeedChar(TextEditor.Text, i, caretOffset);
                }

                if (commentSkipper.InComment || commentSkipper.InString)
                {
                    return;
                }

                _completionWindow = CreateCompletionWindow();


                var fastVarParser = new LSLFastEditorParse();
                fastVarParser.Parse(new StringReader(TextEditor.Text), caretOffset);


                var data = _completionWindow.CompletionList.CompletionData;


                bool possibleLibraryFunction = false;
                bool possibleUserDefinedItem = false;
                bool possibleType = false;
                bool possibleConstant = false;


                if (!fastVarParser.InStateOutsideEvent && !fastVarParser.AfterDefaultState)
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

                    foreach (var sig in _constantSignatures.Where(x => x.Name.StartsWith(e.Text)))
                    {
                        data.Add(new LSLCompletionData(sig.Name, sig.Name,
                            sig.SignatureAndDocumentation, 1)
                        {
                            ColorBrush = _libraryConstantCompleteColor
                        });

                        possibleConstant = true;
                    }
                }


                if (fastVarParser.InFunctionDeclaration || fastVarParser.InEventHandler)
                {
                    var functionSuggestions = _libraryFunctionNames.Where(x => x.StartsWith(e.Text)).ToList();
                    foreach (var func in functionSuggestions)
                    {
                        var docs = string.Join(Environment.NewLine + Environment.NewLine,
                            LibraryDataProvider.GetLibraryFunctionSignatures(func)
                                .Select(x => x.SignatureAndDocumentation));


                        data.Add(new LSLCompletionData(func, func + "(", docs, 0)
                        {
                            ColorBrush = _libraryFunctionCompleteColor
                        });


                        possibleLibraryFunction = true;
                    }


                    foreach (var v in fastVarParser.LocalVariables
                        .Where(x => x.Name.StartsWith(e.Text)))
                    {
                        string doc = "Local variable:\n" + v.Type + " " + v.Name + ";";

                        data.Add(new LSLCompletionData(v.Name, v.Name, doc, 0)
                        {
                            ColorBrush = _localVariableCompleteColor
                        });

                        possibleUserDefinedItem = true;
                    }


                    foreach (var v in fastVarParser.LocalParameters
                        .Where(x => x.Name.StartsWith(e.Text)))
                    {
                        string doc = "Local parameter:\n" + v.Type + " " + v.Name + ";";

                        data.Add(new LSLCompletionData(v.Name, v.Name, doc, 0)
                        {
                            ColorBrush = _localParameterCompleteColor
                        });
                        possibleUserDefinedItem = true;
                    }


                    foreach (var v in fastVarParser.GlobalVariables.Where(x => x.Name.StartsWith(e.Text)))
                    {
                        string doc = "Global variable:\n" + v.Type + " " + v.Name + ";";
                        data.Add(new LSLCompletionData(v.Name, v.Name,
                            doc, 0)
                        {
                            ColorBrush = _globalVariableCompleteColor
                        });
                        possibleUserDefinedItem = true;
                    }


                    foreach (var func in fastVarParser.GlobalFunctions.Where(x => x.Name.StartsWith(e.Text)))
                    {
                        string doc = "Global function:\n" + func.Signature;

                        data.Add(new LSLCompletionData(func.Name, func.Name + "(", doc, 0)
                        {
                            ColorBrush = _globalFunctionCompleteColor
                        });

                        possibleUserDefinedItem = true;
                    }
                }


                if (!possibleConstant && !possibleLibraryFunction && !possibleType && !possibleUserDefinedItem)
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
            var textArea = TextEditor.TextArea;

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
                        commentSkipper.FeedChar(TextEditor.Text, i, caretOffset);
                    }

                    if (commentSkipper.InComment || commentSkipper.InString)
                    {
                        return;
                    }

                    _completionWindow = CreateCompletionWindow();


                    var fastVarParser = new LSLFastEditorParse();
                    fastVarParser.Parse(new StringReader(TextEditor.Text), caretOffset);


                    var data = _completionWindow.CompletionList.CompletionData;


                    foreach (var i in fastVarParser.GlobalFunctions)
                    {
                        data.Add(new LSLCompletionData(
                            i.Name,
                            i.Name + "(", "Global function:\n" + i.Signature, 0)
                        {
                            TextSubStringStart = 0,
                            ColorBrush = _globalFunctionCompleteColor
                        });
                    }

                    foreach (var i in fastVarParser.GlobalVariables)
                    {
                        data.Add(new LSLCompletionData(i.Name, i.Name,
                            "Global variable:\n" + i.Type + " " + i.Name, 1)
                        {
                            TextSubStringStart = 0,
                            ColorBrush = _globalVariableCompleteColor
                        });
                    }

                    foreach (var i in fastVarParser.LocalParameters)
                    {
                        data.Add(new LSLCompletionData(i.Name, i.Name,
                            "Local parameter:\n" + i.Type + " " + i.Name, 2)
                        {
                            TextSubStringStart = 0,
                            ColorBrush = _localParameterCompleteColor
                        });
                    }

                    foreach (var i in fastVarParser.LocalVariables)
                    {
                        data.Add(new LSLCompletionData(i.Name, i.Name,
                            "Local Variable:\n" + i.Type + " " + i.Name, 3)
                        {
                            TextSubStringStart = 0,
                            ColorBrush = _localVariableCompleteColor
                        });
                    }


                    _completionWindow.Loaded += (o, args) =>
                    {
                        lock (_completionWindow)
                        {
                            if (_completionWindow.CompletionList.ListBox.Items.Count == 0)
                            {
                                _completionWindow.Close();
                            }
                        }
                    };


                    _completionWindow.Closed += (o, args) =>
                    {
                        lock (_completionLock)
                        {
                            _completionWindow = null;
                        }
                    };

                    _completionWindow.Show();

                    e.Handled = true;
                }
            }
        }



        private void ControlW(KeyEventArgs e)
        {
            var textArea = TextEditor.TextArea;

            lock (_completionLock)
            {
                if (_completionWindow != null) return;

                _completionWindow = CreateCompletionWindow();


                foreach (var func in _libraryFunctionNames)
                {
                    var docs = string.Join(Environment.NewLine + Environment.NewLine,
                        LibraryDataProvider.GetLibraryFunctionSignatures(func)
                            .Select(x => x.SignatureAndDocumentation));


                    _completionWindow.CompletionList.CompletionData.Add(new LSLCompletionData(func, func + "(", docs, 0)
                    {
                        ColorBrush = _libraryFunctionCompleteColor,
                        TextSubStringStart = 0
                    });
                }

                _completionWindow.Closed += (o, args) => { _completionWindow = null; };

                _completionWindow.Show();
            }
        }



        private void ControlQ(KeyEventArgs e)
        {
            var textArea = TextEditor.TextArea;

            lock (_completionLock)
            {
                if (_completionWindow != null) return;

                _completionWindow = CreateCompletionWindow();


                foreach (var con in _constantSignatures)
                {
                    _completionWindow.CompletionList.CompletionData.Add(
                        new LSLCompletionData(con.Name,
                            con.Name,
                            con.SignatureAndDocumentation, 0)
                        {
                            ColorBrush = _libraryConstantCompleteColor,
                            TextSubStringStart = 0
                        });
                }

                _completionWindow.Closed += (o, args) => { _completionWindow = null; };

                _completionWindow.Show();
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



        public void SetLibraryDataProvider(ILSLMainLibraryDataProvider provider)
        {
            LibraryDataProvider = provider;

            _libraryFunctionNames = LibraryDataProvider.LibraryFunctions.Select(x => x.First().Name).ToList();
            _eventSignatures = LibraryDataProvider.SupportedEventHandlers.ToList();
            _constantSignatures = LibraryDataProvider.LibraryConstants.ToList();

            _libraryFunctionNames.Sort(String.CompareOrdinal);
            _eventSignatures.Sort((x, y) => String.CompareOrdinal(x.Name, y.Name));
            _constantSignatures.Sort((x, y) => String.CompareOrdinal(x.Name, y.Name));


            using (var resourceStream = GetType().Assembly.GetManifestResourceStream("LSLCCEditor.LSLEditor.LSL.xshd"))
            {
                if (resourceStream != null)
                {
                    var reader = new XmlTextReader(resourceStream);

                    TextEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);


                    foreach (var func in (from s in _libraryFunctionNames orderby s.Length descending select s))
                    {
                        var rule = new HighlightingRule
                        {
                            Regex = new Regex("\\b" + func + "\\b"),
                            Color = TextEditor.SyntaxHighlighting.GetNamedColor("Functions")
                        };
                        TextEditor.SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
                    }

                    foreach (var cnst in (from s in _constantSignatures orderby s.Name.Length descending select s))
                    {
                        var rule = new HighlightingRule
                        {
                            Regex = new Regex("\\b" + cnst.Name + "\\b"),
                            Color = TextEditor.SyntaxHighlighting.GetNamedColor("Constants")
                        };
                        TextEditor.SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
                    }

                    foreach (var evnt in (from s in _eventSignatures orderby s.Name.Length descending select s))
                    {
                        var rule = new HighlightingRule
                        {
                            Regex = new Regex("\\b" + evnt.Name + "\\b"),
                            Color = TextEditor.SyntaxHighlighting.GetNamedColor("Events")
                        };
                        TextEditor.SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
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
    }
}