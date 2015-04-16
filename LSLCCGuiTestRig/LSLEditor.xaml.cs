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
        private bool _completionWindowOpen = false;
        private List<LSLLibraryConstantSignature> _constantSignatures;
        private List<LSLLibraryEventSignature> _eventSignatures;
        private List<string> _libraryFunctionNames;


        public LSLEditor()
        {
            InitializeComponent();
            Loaded += Control_Loaded;

            TextEditor.TextArea.TextEntering += TextEditor_TextArea_TextEntering;
            TextEditor.TextArea.TextEntered += TextEditor_TextArea_TextEntered;
            TextEditor.MouseHover += TextEditor_MouseHover;
            TextEditor.MouseHover += TextEditor_MouseHoverStopped;
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


        private void TextEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
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
                if (_validSuggestionPrefixes.Contains(onebehind)
                    && Regex.Match(e.Text, "[A-Za-z]").Success && !_completionWindowOpen)
                {
                    _completionWindow = new CompletionWindow(TextEditor.TextArea);

                    _completionWindow.Width = _completionWindow.Width + 160;

                    var data = _completionWindow.CompletionList.CompletionData;

                    double priority = 0;
                    var functionSuggestions = _libraryFunctionNames.Where(x => x.StartsWith(e.Text)).ToList();
                    foreach (var func in functionSuggestions)
                    {
                        var docs = string.Join(Environment.NewLine + Environment.NewLine,
                            LibraryDataProvider.GetLibraryFunctionSignatures(func)
                                .Select(x => x.SignatureAndDocumentation));

                        data.Add(new LSLFunctionCompletionData(func,
                            docs, priority));
                        priority++;
                    }

                    var constantSuggestions = _constantSignatures.Where(x => x.Name.StartsWith(e.Text)).ToList();
                    foreach (var func in constantSuggestions)
                    {
                        data.Add(new LSLConstantCompletionData(func.Name,
                            func.SignatureAndDocumentation, priority));
                        priority++;
                    }

                    if (constantSuggestions.Any() || functionSuggestions.Any())
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


        private void TextEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!(char.IsLetterOrDigit(e.Text[0])||e.Text[0]=='_'))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }


        public void SetLibraryDataProvider(ILSLMainLibraryDataProvider provider)
        {
            LibraryDataProvider = provider;

            var functionNames = new StringBuilder();
            var eventNames = new StringBuilder();
            var constantNames = new StringBuilder();


            _libraryFunctionNames = LibraryDataProvider.LibraryFunctions.Select(x => x.First().Name).ToList();
            _eventSignatures = LibraryDataProvider.SupportedEventHandlers.ToList();
            _constantSignatures = LibraryDataProvider.LibraryConstants.ToList();

            _libraryFunctionNames.Sort(String.CompareOrdinal);
            _eventSignatures.Sort((x, y) => String.CompareOrdinal(x.Name, y.Name));
            _constantSignatures.Sort((x, y) => String.CompareOrdinal(x.Name, y.Name));


            foreach (var func in _libraryFunctionNames)
            {
                functionNames.Append("<Key word=\"" + func + "\"/>");
            }

            foreach (var cnst in _constantSignatures)
            {
                constantNames.Append("<Key word=\"" + cnst.Name + "\"/>");
            }

            foreach (var evnt in _eventSignatures)
            {
                eventNames.Append("<Key word=\"" + evnt.Name + "\"/>");
            }

            using (var resourceStream = GetType().Assembly.GetManifestResourceStream("LSLCCEditor.LSL.xshd"))
            {
                if (resourceStream != null)
                {
                    var streamReader = new StreamReader(resourceStream, Encoding.Default);
                    var builder = new StringBuilder();

                    while (!streamReader.EndOfStream)
                    {
                        builder.Append(streamReader.ReadLine());
                        builder.Replace("{FUNCTION_NAMES}", functionNames.ToString());
                        builder.Replace("{CONSTANT_NAMES}", constantNames.ToString());
                        builder.Replace("{EVENT_NAMES}", eventNames.ToString());
                    }

                    var text = builder.ToString();

                    var memStream = new MemoryStream(Encoding.Default.GetBytes(text));

                    using (var reader = new XmlTextReader(memStream))
                    {
                        TextEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
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