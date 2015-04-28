#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
using System.Windows.Media;

#endregion

namespace LSLCCEditor
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
        private bool _completionWindowOpen;
        private List<LSLLibraryConstantSignature> _constantSignatures;
        private List<LSLLibraryEventSignature> _eventSignatures;
        private List<string> _libraryFunctionNames;


        public LSLEditor()
        {
            InitializeComponent();
            Loaded += Control_Loaded;

            //TextEditor.TextArea.TextEntering += TextArea_TextEntering;
            TextEditor.TextArea.TextEntered += TextArea_TextEntered;
            TextEditor.MouseHover += TextEditor_MouseHover;
            TextEditor.MouseHover += TextEditor_MouseHoverStopped;
            TextEditor.TextArea.KeyDown += TextEditor_KeyDown;

            
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
                if ( 
                     _validSuggestionPrefixes.Contains(onebehind) &&
                     Regex.Match(e.Text, "[A-Za-z]").Success && 
                     !_completionWindowOpen
                    )
                {
                    var textArea = TextEditor.TextArea;
                    var caretOffset = textArea.Caret.Offset;
                    

                    var scopeAddress = LSLFastEditorParse.FastParseToOffset(TextEditor.Text, caretOffset);

                    if (scopeAddress.InString || scopeAddress.InComment || (scopeAddress.InState && scopeAddress.ScopeLevel == 1)) return;

                    _completionWindow = new CompletionWindow(textArea);


                    LSLFastEditorParse fastVarParser = new LSLFastEditorParse();
                    fastVarParser.Parse(new StringReader(TextEditor.Text));


                    bool pastDefaultState = false;
                    var beforeDefaultState = true;

                    if(fastVarParser.DefaultState != null){
                        pastDefaultState = fastVarParser.DefaultState.SourceCodeRange.StartIndex < caretOffset;
                        beforeDefaultState = fastVarParser.DefaultState.SourceCodeRange.StartIndex > caretOffset; 
                    }


                    _completionWindow.Width = _completionWindow.Width + 160;

                    var data = _completionWindow.CompletionList.CompletionData;


                    

                    double priority = 0;

                    bool possibleLibraryFunction = false;
                    bool possibleUserDefinedItem = false;


                    if (scopeAddress.ScopeLevel != 0)
                    {
                        var functionSuggestions = _libraryFunctionNames.Where(x => x.StartsWith(e.Text)).ToList();
                        foreach (var func in functionSuggestions)
                        {
                            var docs = string.Join(Environment.NewLine + Environment.NewLine,
                                LibraryDataProvider.GetLibraryFunctionSignatures(func)
                                    .Select(x => x.SignatureAndDocumentation));

                            data.Add(new LSLFunctionCompletionData(func,
                                docs, priority));
                            priority++;

                            possibleLibraryFunction = true;
                        }      
                    }




                    


                    var possibleType = false;
                    var possibleConstant = false;


                    if ((pastDefaultState && scopeAddress.ScopeLevel > 1) || beforeDefaultState)
                    {



                        foreach (var v in fastVarParser.GetLocalVariables(scopeAddress, caretOffset)
                            .Where(x => x.Name.StartsWith(e.Text)))
                        {
                            string doc = "Local variable:\n" + v.Type + " " + v.Name + ";";

                            data.Add(new LSLConstantCompletionData(v.Name, doc, 0)
                            {
                                ColorBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                            });

                            possibleUserDefinedItem = true;
                        }



                        foreach (var v in fastVarParser.GetLocalParameters(scopeAddress, caretOffset)
                            .Where(x => x.Name.StartsWith(e.Text)))
                        {
                            string doc = "Local parameter:\n" + v.Type + " " + v.Name + ";";

                            data.Add(new LSLConstantCompletionData(v.Name, doc, 0)
                            {
                                ColorBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                            });
                            possibleUserDefinedItem = true;
                        }


                        foreach (var v in fastVarParser.GlobalVariables.Where(x => x.Name.StartsWith(e.Text)))
                        {
                            string doc = "Global variable:\n" + v.Type + " " + v.Name + ";";
                            data.Add(new LSLConstantCompletionData(v.Name, doc, 0)
                            {
                                ColorBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                            });
                            possibleUserDefinedItem = true;
                        }


                        foreach (var func in fastVarParser.GlobalFunctions.Where(x => x.Name.StartsWith(e.Text)))
                        {

                            string doc = "Global function:\n" + func.Signature;

                            data.Add(new LSLFunctionCompletionData(func.Name, doc, 0)
                            {
                                ColorBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                            });

                            possibleUserDefinedItem = true;
                        }


                        Color green = Color.FromRgb(25, 76, 25);

                        if (e.Text.StartsWith("i"))
                        {

                            data.Add(new LSLConstantCompletionData("integer",
                                    "integer type", 0) { ColorBrush = new SolidColorBrush(green) });
                            possibleType = true;

                        }
                        else if (e.Text.StartsWith("s"))
                        {
                            data.Add(new LSLConstantCompletionData("string",
                                    "string type", 0) { ColorBrush = new SolidColorBrush(green) });
                            possibleType = true;
                        }
                        else if (e.Text.StartsWith("v"))
                        {
                            data.Add(new LSLConstantCompletionData("vector",
                                    "vector type", 0) { ColorBrush = new SolidColorBrush(green) });
                            possibleType = true;
                        }
                        else if (e.Text.StartsWith("r"))
                        {
                            data.Add(new LSLConstantCompletionData("rotation",
                                    "rotation type", 0) { ColorBrush = new SolidColorBrush(green) });
                            possibleType = true;
                        }
                        else if (e.Text.StartsWith("k"))
                        {
                            data.Add(new LSLConstantCompletionData("key",
                                    "key type", 0) { ColorBrush = new SolidColorBrush(green) });
                            possibleType = true;
                        }
                        else if (e.Text.StartsWith("f"))
                        {

                            data.Add(new LSLConstantCompletionData("float",
                                    "float type", 0) { ColorBrush = new SolidColorBrush(green) });
                            possibleType = true;
                        }

                        foreach (var func in _constantSignatures.Where(x => x.Name.StartsWith(e.Text)))
                        {
                            data.Add(new LSLConstantCompletionData(func.Name, func.SignatureAndDocumentation, priority));
                            possibleConstant = true; 
                            priority++;
                            
                        }

                    }



                    if (possibleConstant || possibleLibraryFunction || possibleType || possibleUserDefinedItem)
                    {
                        _completionWindow.Show();
                        _completionWindow.Closed += delegate
                        {
                            lock (_completionLock)
                            {
                                _completionWindowOpen = false;
                            }
                        };
                    }

                }
            }
        }


        private void TextEditor_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Space && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                lock (_completionLock)
                {
                    if (_completionWindow != null)
                    {
                        _completionWindow.CompletionList.RequestInsertion(e);
                        e.Handled = true;
                    }
                }
            }
        }


        /*
        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            lock (_completionLock)
            {
                if (e.Text.Length > 0 && _completionWindow != null)
                {
                    if (!(char.IsLetterOrDigit(e.Text[0]) || e.Text[0] == '_' || e.Text[0] == '('))
                    {
                        // Whenever a non-letter is typed while the completion window is open,
                        // insert the currently selected element.
                        _completionWindow.CompletionList.RequestInsertion(e);


                    }


                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }*/


        public void SetLibraryDataProvider(ILSLMainLibraryDataProvider provider)
        {
            LibraryDataProvider = provider;

            _libraryFunctionNames = LibraryDataProvider.LibraryFunctions.Select(x => x.First().Name).ToList();
            _eventSignatures = LibraryDataProvider.SupportedEventHandlers.ToList();
            _constantSignatures = LibraryDataProvider.LibraryConstants.ToList();

            _libraryFunctionNames.Sort(String.CompareOrdinal);
            _eventSignatures.Sort((x, y) => String.CompareOrdinal(x.Name, y.Name));
            _constantSignatures.Sort((x, y) => String.CompareOrdinal(x.Name, y.Name));



            using (var resourceStream = GetType().Assembly.GetManifestResourceStream("LSLCCEditor.LSL.xshd"))
            {
                if (resourceStream != null)
                {
                    using (var reader = new XmlTextReader(resourceStream))
                    {
                        TextEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

                    }


                    foreach (var func in _libraryFunctionNames)
                    {
                        var rule = new HighlightingRule
                        {
                            Regex = new Regex(func),
                            Color = TextEditor.SyntaxHighlighting.GetNamedColor("Functions")
                        };
                        TextEditor.SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
                    }

                    foreach (var cnst in _constantSignatures)
                    {
                        var rule = new HighlightingRule
                        {
                            Regex = new Regex(cnst.Name),
                            Color = TextEditor.SyntaxHighlighting.GetNamedColor("Constants")
                        };
                        TextEditor.SyntaxHighlighting.MainRuleSet.Rules.Add(rule);
                    }

                    foreach (var evnt in _eventSignatures)
                    {
                        var rule = new HighlightingRule
                        {
                            Regex = new Regex(evnt.Name),
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

        private void Control_Loaded(object sender, RoutedEventArgs a)
        {
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