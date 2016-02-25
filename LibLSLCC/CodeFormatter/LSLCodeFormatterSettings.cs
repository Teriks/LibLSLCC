#region FileInfo

// 
// File: LSLCodeFormatterSettings.cs
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

using System.Reflection;
using LibLSLCC.Settings;

#endregion

namespace LibLSLCC.CodeFormatter
{
    /// <summary>
    ///     Settings object for <see cref="LSLCodeFormatter" />.
    /// </summary>
    public class LSLCodeFormatterSettings : SettingsBaseClass<LSLCodeFormatterSettings>
    {
        private static readonly LSLCodeFormatterSettings Defaults = new LSLCodeFormatterSettings();
        private bool _addSpacesBeforeOpeningDoLoopBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningElseBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningElseIfBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningEventBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningForLoopBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningFunctionBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningIfBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningStateBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak;
        private int _columnsBeforeDeclarationExpressionWrap = 60;
        private int _columnsBeforeDoWhileExpressionWrap = 60;
        private int _columnsBeforeElseIfExpressionWrap = 60;
        private int _columnsBeforeIfExpressionWrap = 60;
        private int _columnsBeforeReturnExpressionWrap = 60;
        private int _columnsBeforeStatementExpressionWrap = 60;
        private int _columnsBeforeWhileExpressionWrap = 60;
        private bool _convertBracelessControlStatements;
        //helpful VS regex: ({\s(.*?)\s=\svalue;)

        //replace with: { SetField(ref $2,value,"$2");

        //then edit strings.


        private bool _declarationExpressionWrapping = true;
        private bool _doLoopBracesOnNewLine = true;
        private bool _doWhileExpressionWrapping = true;
        private bool _elseIfExpressionWrapping = true;
        private bool _elseIfStatementBracesOnNewLine = true;
        private bool _elseIfStatementOnNewLine = true;
        private bool _elseStatementBracesOnNewLine = true;
        private bool _elseStatementOnNewLine = true;
        private bool _eventBracesOnNewLine = true;
        private bool _forLoopBracesOnNewLine = true;
        private bool _functionBracesOnNewLine = true;
        private bool _ifExpressionWrapping = true;
        private bool _ifStatementBracesOnNewLine = true;
        private bool _indentBracelessControlStatements = true;
        private int _maximumNewLinesAtBeginingOfCodeScope = 2;
        private int _maximumNewLinesAtBeginingOfStateScope = 2;
        private int _maximumNewLinesAtEndOfCodeScope = 2;
        private int _maximumNewLinesAtEndOfStateScope = 2;
        private int _minimumExpressionsInDeclarationToWrap = 2;
        private int _minimumExpressionsInDoWhileToWrap = 2;
        private int _minimumExpressionsInElseIfToWrap = 2;
        private int _minimumExpressionsInIfToWrap = 2;
        private int _minimumExpressionsInReturnToWrap = 2;
        private int _minimumExpressionsInStatementToWrap = 2;
        private int _minimumExpressionsInWhileToWrap = 2;
        private int _minimumNewLinesBetweenDistinctGlobalStatements = 3;
        private int _minimumNewLinesBetweenDistinctLocalStatements = 2;
        private int _minimumNewLinesBetweenEventHandlers = 2;
        private string _newlineSequence = "\n";
        private bool _removeComments;
        private bool _returnExpressionWrapping = true;
        private int _spacesBeforeClosingDoLoopBrace;
        private int _spacesBeforeClosingElseBrace;
        private int _spacesBeforeClosingElseIfBrace;
        private int _spacesBeforeClosingEventBrace;
        private int _spacesBeforeClosingForLoopBrace;
        private int _spacesBeforeClosingFunctionBrace;
        private int _spacesBeforeClosingIfBrace;
        private int _spacesBeforeClosingStateBrace;
        private int _spacesBeforeClosingWhileLoopBrace;
        private int _spacesBeforeOpeningDoLoopBrace;
        private int _spacesBeforeOpeningElseBrace;
        private int _spacesBeforeOpeningElseIfBrace;
        private int _spacesBeforeOpeningEventBrace;
        private int _spacesBeforeOpeningForLoopBrace;
        private int _spacesBeforeOpeningFunctionBrace;
        private int _spacesBeforeOpeningIfBrace;
        private int _spacesBeforeOpeningStateBrace;
        private int _spacesBeforeOpeningWhileLoopBrace;
        private int _spacesBeforeUnbrokenElseIfStatement = 1;
        private int _spacesBeforeUnbrokenElseStatement = 1;
        private bool _stateBracesOnNewLine = true;
        private bool _statementExpressionWrapping = true;
        private string _tabString = "\t";
        private bool _whileExpressionWrapping = true;
        private bool _whileLoopBracesOnNewLine = true;

        /// <summary>
        ///     Whether or not the formatter should strip all comments from the source code.
        /// </summary>
        public bool RemoveComments
        {
            get { return _removeComments; }
            set { SetField(ref _removeComments, value, "RemoveComments"); }
        }

        /// <summary>
        ///     The newline sequence the formatter should use to write newlines with.
        /// </summary>
        public string NewlineSequence
        {
            get { return _newlineSequence; }
            set { SetField(ref _newlineSequence, value, "NewlineSequence"); }
        }

        /// <summary>
        ///     The tab string the formatter should use to indent with.
        /// </summary>
        public string TabString
        {
            get { return _tabString; }
            set { SetField(ref _tabString, value, "TabString"); }
        }

        /// <summary>
        ///     True if the formatter should wrap declaration expressions that exceed wrapping limits.
        /// </summary>
        /// <seealso cref="ColumnsBeforeDeclarationExpressionWrap" />
        /// <seealso cref="MinimumExpressionsInDeclarationToWrap" />
        public bool DeclarationExpressionWrapping
        {
            get { return _declarationExpressionWrapping; }
            set { SetField(ref _declarationExpressionWrapping, value, "DeclarationExpressionWrapping"); }
        }

        /// <summary>
        ///     The number of character columns before the formatter should consider wrapping a declaration expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int ColumnsBeforeDeclarationExpressionWrap
        {
            get { return _columnsBeforeDeclarationExpressionWrap; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _columnsBeforeDeclarationExpressionWrap, value, "ColumnsBeforeDeclarationExpressionWrap");
            }
        }

        /// <summary>
        ///     The minimum number of binary expressions before the formatter should consider wrapping a declaration expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MinimumExpressionsInDeclarationToWrap
        {
            get { return _minimumExpressionsInDeclarationToWrap; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _minimumExpressionsInDeclarationToWrap, value, "MinimumExpressionsInDeclarationToWrap");
            }
        }

        /// <summary>
        ///     True if the formatter should wrap statement expressions that exceed wrapping limits.
        /// </summary>
        /// <seealso cref="ColumnsBeforeStatementExpressionWrap" />
        /// <seealso cref="MinimumExpressionsInStatementToWrap" />
        public bool StatementExpressionWrapping
        {
            get { return _statementExpressionWrapping; }
            set { SetField(ref _statementExpressionWrapping, value, "StatementExpressionWrapping"); }
        }

        /// <summary>
        ///     The number of character columns before the formatter should consider wrapping a statement expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int ColumnsBeforeStatementExpressionWrap
        {
            get { return _columnsBeforeStatementExpressionWrap; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _columnsBeforeStatementExpressionWrap, value, "ColumnsBeforeStatementExpressionWrap");
            }
        }

        /// <summary>
        ///     The minimum number of binary expressions before the formatter should consider wrapping a statement expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MinimumExpressionsInStatementToWrap
        {
            get { return _minimumExpressionsInStatementToWrap; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _minimumExpressionsInStatementToWrap, value, "MinimumExpressionsInStatementToWrap");
            }
        }

        /// <summary>
        ///     True if the formatter should wrap return expressions that exceed wrapping limits.
        /// </summary>
        /// <seealso cref="ColumnsBeforeReturnExpressionWrap" />
        /// <seealso cref="MinimumExpressionsInReturnToWrap" />
        public bool ReturnExpressionWrapping
        {
            get { return _returnExpressionWrapping; }
            set { SetField(ref _returnExpressionWrapping, value, "ReturnExpressionWrapping"); }
        }

        /// <summary>
        ///     The number of character columns before the formatter should consider wrapping a return expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int ColumnsBeforeReturnExpressionWrap
        {
            get { return _columnsBeforeReturnExpressionWrap; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _columnsBeforeReturnExpressionWrap, value, "ColumnsBeforeReturnExpressionWrap");
            }
        }

        /// <summary>
        ///     The minimum number of binary expressions before the formatter should consider wrapping a return expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MinimumExpressionsInReturnToWrap
        {
            get { return _minimumExpressionsInReturnToWrap; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _minimumExpressionsInReturnToWrap, value, "MinimumExpressionsInReturnToWrap");
            }
        }

        /// <summary>
        ///     True if the formatter should wrap if condition expressions that exceed wrapping limits.
        /// </summary>
        /// <seealso cref="ColumnsBeforeIfExpressionWrap" />
        /// <seealso cref="MinimumExpressionsInIfToWrap" />
        public bool IfExpressionWrapping
        {
            get { return _ifExpressionWrapping; }
            set { SetField(ref _ifExpressionWrapping, value, "IfExpressionWrapping"); }
        }

        /// <summary>
        ///     The number of character columns before the formatter should consider wrapping an if condition expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int ColumnsBeforeIfExpressionWrap
        {
            get { return _columnsBeforeIfExpressionWrap; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _columnsBeforeIfExpressionWrap, value, "ColumnsBeforeIfExpressionWrap");
            }
        }

        /// <summary>
        ///     The minimum number of binary expressions before the formatter should consider wrapping an if condition expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MinimumExpressionsInIfToWrap
        {
            get { return _minimumExpressionsInIfToWrap; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _minimumExpressionsInIfToWrap, value, "MinimumExpressionsInIfToWrap");
            }
        }

        /// <summary>
        ///     True if the formatter should wrap 'if else' condition expressions that exceed wrapping limits.
        /// </summary>
        /// <seealso cref="ColumnsBeforeElseIfExpressionWrap" />
        /// <seealso cref="MinimumExpressionsInElseIfToWrap" />
        public bool ElseIfExpressionWrapping
        {
            get { return _elseIfExpressionWrapping; }
            set { SetField(ref _elseIfExpressionWrapping, value, "ElseIfExpressionWrapping"); }
        }

        /// <summary>
        ///     The number of character columns before the formatter should consider wrapping an 'if else' condition expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int ColumnsBeforeElseIfExpressionWrap
        {
            get { return _columnsBeforeElseIfExpressionWrap; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _columnsBeforeElseIfExpressionWrap, value, "ColumnsBeforeElseIfExpressionWrap");
            }
        }

        /// <summary>
        ///     The minimum number of binary expressions before the formatter should consider wrapping an 'if else' condition
        ///     expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MinimumExpressionsInElseIfToWrap
        {
            get { return _minimumExpressionsInElseIfToWrap; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _minimumExpressionsInElseIfToWrap, value, "MinimumExpressionsInElseIfToWrap");
            }
        }

        /// <summary>
        ///     True if the formatter should wrap while condition expressions that exceed wrapping limits.
        /// </summary>
        /// <seealso cref="ColumnsBeforeWhileExpressionWrap" />
        /// <seealso cref="MinimumExpressionsInWhileToWrap" />
        public bool WhileExpressionWrapping
        {
            get { return _whileExpressionWrapping; }
            set { SetField(ref _whileExpressionWrapping, value, "WhileExpressionWrapping"); }
        }

        /// <summary>
        ///     The number of character columns before the formatter should consider wrapping a while condition expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int ColumnsBeforeWhileExpressionWrap
        {
            get { return _columnsBeforeWhileExpressionWrap; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _columnsBeforeWhileExpressionWrap, value, "ColumnsBeforeWhileExpressionWrap");
            }
        }

        /// <summary>
        ///     The minimum number of binary expressions before the formatter should consider wrapping a while condition
        ///     expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MinimumExpressionsInWhileToWrap
        {
            get { return _minimumExpressionsInWhileToWrap; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _minimumExpressionsInWhileToWrap, value, "MinimumExpressionsInWhileToWrap");
            }
        }

        /// <summary>
        ///     True if the formatter should wrap 'do while' condition expressions that exceed wrapping limits.
        /// </summary>
        /// <seealso cref="ColumnsBeforeDoWhileExpressionWrap" />
        /// <seealso cref="MinimumExpressionsInDoWhileToWrap" />
        public bool DoWhileExpressionWrapping
        {
            get { return _doWhileExpressionWrapping; }
            set { SetField(ref _doWhileExpressionWrapping, value, "DoWhileExpressionWrapping"); }
        }

        /// <summary>
        ///     The number of character columns before the formatter should consider wrapping a while condition expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int ColumnsBeforeDoWhileExpressionWrap
        {
            get { return _columnsBeforeDoWhileExpressionWrap; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _columnsBeforeDoWhileExpressionWrap, value, "ColumnsBeforeDoWhileExpressionWrap");
            }
        }

        /// <summary>
        ///     The minimum number of binary expressions before the formatter should consider wrapping a 'do while' condition
        ///     expression.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MinimumExpressionsInDoWhileToWrap
        {
            get { return _minimumExpressionsInDoWhileToWrap; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _minimumExpressionsInDoWhileToWrap, value, "MinimumExpressionsInDoWhileToWrap");
            }
        }

        /// <summary>
        ///     whether or not an if statements opening brace should be placed on a new line.
        /// </summary>
        public bool IfStatementBracesOnNewLine
        {
            get { return _ifStatementBracesOnNewLine; }
            set { SetField(ref _ifStatementBracesOnNewLine, value, "IfStatementBracesOnNewLine"); }
        }

        /// <summary>
        ///     whether or not an 'else if' statements opening brace should be placed on a new line.
        /// </summary>
        public bool ElseIfStatementBracesOnNewLine
        {
            get { return _elseIfStatementBracesOnNewLine; }
            set { SetField(ref _elseIfStatementBracesOnNewLine, value, "ElseIfStatementBracesOnNewLine"); }
        }

        /// <summary>
        ///     whether or not an 'else' statements opening brace should be placed on a new line.
        /// </summary>
        public bool ElseStatementBracesOnNewLine
        {
            get { return _elseStatementBracesOnNewLine; }
            set { SetField(ref _elseStatementBracesOnNewLine, value, "ElseStatementBracesOnNewLine"); }
        }

        /// <summary>
        ///     Whether or not a state blocks opening brace should be placed on a new line.
        /// </summary>
        public bool StateBracesOnNewLine
        {
            get { return _stateBracesOnNewLine; }
            set { SetField(ref _stateBracesOnNewLine, value, "StateBracesOnNewLine"); }
        }

        /// <summary>
        ///     Whether or not an event handlers opening brace should be placed on a new line.
        /// </summary>
        public bool EventBracesOnNewLine
        {
            get { return _eventBracesOnNewLine; }
            set { SetField(ref _eventBracesOnNewLine, value, "EventBracesOnNewLine"); }
        }

        /// <summary>
        ///     Whether or not a function declarations opening brace should be placed on a new line.
        /// </summary>
        public bool FunctionBracesOnNewLine
        {
            get { return _functionBracesOnNewLine; }
            set { SetField(ref _functionBracesOnNewLine, value, "FunctionBracesOnNewLine"); }
        }

        /// <summary>
        ///     Whether or not a while loops opening brace should be placed on a new line.
        /// </summary>
        public bool WhileLoopBracesOnNewLine
        {
            get { return _whileLoopBracesOnNewLine; }
            set { SetField(ref _whileLoopBracesOnNewLine, value, "WhileLoopBracesOnNewLine"); }
        }

        /// <summary>
        ///     Whether or not a 'do while' loops opening brace should be placed on a new line.
        /// </summary>
        public bool DoLoopBracesOnNewLine
        {
            get { return _doLoopBracesOnNewLine; }
            set { SetField(ref _doLoopBracesOnNewLine, value, "DoLoopBracesOnNewLine"); }
        }

        /// <summary>
        ///     Whether or not a for loops opening brace should be placed on a new line.
        /// </summary>
        public bool ForLoopBracesOnNewLine
        {
            get { return _forLoopBracesOnNewLine; }
            set { SetField(ref _forLoopBracesOnNewLine, value, "ForLoopBracesOnNewLine"); }
        }

        /// <summary>
        ///     The number of spaces to place before a state blocks opening brace.
        /// </summary>
        /// <seealso cref="AddSpacesBeforeOpeningStateBraceAfterCommentBreak" />
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeOpeningStateBrace
        {
            get { return _spacesBeforeOpeningStateBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeOpeningStateBrace, value, "SpacesBeforeOpeningStateBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before a state blocks closing brace.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeClosingStateBrace
        {
            get { return _spacesBeforeClosingStateBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeClosingStateBrace, value, "SpacesBeforeClosingStateBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before an if statements opening brace.
        /// </summary>
        /// <seealso cref="AddSpacesBeforeOpeningIfBraceAfterCommentBreak" />
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeOpeningIfBrace
        {
            get { return _spacesBeforeOpeningIfBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeOpeningIfBrace, value, "SpacesBeforeOpeningIfBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before an if statements closing brace.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeClosingIfBrace
        {
            get { return _spacesBeforeClosingIfBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeClosingIfBrace, value, "SpacesBeforeClosingIfBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before an 'else if' statements opening brace.
        /// </summary>
        /// <seealso cref="AddSpacesBeforeOpeningElseIfBraceAfterCommentBreak" />
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeOpeningElseIfBrace
        {
            get { return _spacesBeforeOpeningElseIfBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeOpeningElseIfBrace, value, "SpacesBeforeOpeningElseIfBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before an 'else if' statements closing brace.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeClosingElseIfBrace
        {
            get { return _spacesBeforeClosingElseIfBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeClosingElseIfBrace, value, "SpacesBeforeClosingElseIfBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before an else statements opening brace.
        /// </summary>
        /// <seealso cref="AddSpacesBeforeOpeningElseBraceAfterCommentBreak" />
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeOpeningElseBrace
        {
            get { return _spacesBeforeOpeningElseBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeOpeningElseBrace, value, "SpacesBeforeOpeningElseBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before an else statements closing brace.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeClosingElseBrace
        {
            get { return _spacesBeforeClosingElseBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeClosingElseBrace, value, "SpacesBeforeClosingElseBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before a for loop statements opening brace.
        /// </summary>
        /// <seealso cref="AddSpacesBeforeOpeningForLoopBraceAfterCommentBreak" />
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeOpeningForLoopBrace
        {
            get { return _spacesBeforeOpeningForLoopBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeOpeningForLoopBrace, value, "SpacesBeforeOpeningForLoopBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before a for loop statements closing brace.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeClosingForLoopBrace
        {
            get { return _spacesBeforeClosingForLoopBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeClosingForLoopBrace, value, "SpacesBeforeClosingForLoopBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before a do loop statements opening brace.
        /// </summary>
        /// <seealso cref="AddSpacesBeforeOpeningDoLoopBraceAfterCommentBreak" />
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeOpeningDoLoopBrace
        {
            get { return _spacesBeforeOpeningDoLoopBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeOpeningDoLoopBrace, value, "SpacesBeforeOpeningDoLoopBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before a do loop statements closing brace.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeClosingDoLoopBrace
        {
            get { return _spacesBeforeClosingDoLoopBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeClosingDoLoopBrace, value, "SpacesBeforeClosingDoLoopBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before a while loop statements opening brace.
        /// </summary>
        /// <seealso cref="AddSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak" />
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeOpeningWhileLoopBrace
        {
            get { return _spacesBeforeOpeningWhileLoopBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeOpeningWhileLoopBrace, value, "SpacesBeforeOpeningWhileLoopBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before a while loop statements closing brace.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeClosingWhileLoopBrace
        {
            get { return _spacesBeforeClosingWhileLoopBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeClosingWhileLoopBrace, value, "SpacesBeforeClosingWhileLoopBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before an event handlers opening brace.
        /// </summary>
        /// <seealso cref="AddSpacesBeforeOpeningEventBraceAfterCommentBreak" />
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeOpeningEventBrace
        {
            get { return _spacesBeforeOpeningEventBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeOpeningEventBrace, value, "SpacesBeforeOpeningEventBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before an event handlers closing brace.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeClosingEventBrace
        {
            get { return _spacesBeforeClosingEventBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeClosingEventBrace, value, "SpacesBeforeClosingEventBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before a function declarations opening brace.
        /// </summary>
        /// <seealso cref="AddSpacesBeforeOpeningFunctionBraceAfterCommentBreak" />
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeOpeningFunctionBrace
        {
            get { return _spacesBeforeOpeningFunctionBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeOpeningFunctionBrace, value, "SpacesBeforeOpeningFunctionBrace");
            }
        }

        /// <summary>
        ///     The number of spaces to place before a function declarations closing brace.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeClosingFunctionBrace
        {
            get { return _spacesBeforeClosingFunctionBrace; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeClosingFunctionBrace, value, "SpacesBeforeClosingFunctionBrace");
            }
        }

        /// <summary>
        ///     Whether or not to indent control statements (branches and loops) that do not use braces.
        ///     Setting this to <c>true</c> will make <see cref="ConvertBracelessControlStatements" /> <c>false</c>.
        /// </summary>
        /// <seealso cref="ConvertBracelessControlStatements" />
        public bool IndentBracelessControlStatements
        {
            get { return _indentBracelessControlStatements; }
            set
            {
                if (value) ConvertBracelessControlStatements = false;
                SetField(ref _indentBracelessControlStatements, value, "IndentBracelessControlStatements");
            }
        }

        /// <summary>
        ///     Whether or not to add braces to control statements (branches and loops) that were not previously using braces.
        ///     Setting this to <c>true</c> will make <see cref="IndentBracelessControlStatements" /> <c>false</c>.
        /// </summary>
        /// <seealso cref="IndentBracelessControlStatements" />
        public bool ConvertBracelessControlStatements
        {
            get { return _convertBracelessControlStatements; }
            set
            {
                if (value) IndentBracelessControlStatements = false;
                SetField(ref _convertBracelessControlStatements, value, "ConvertBracelessControlStatements");
            }
        }

        /// <summary>
        ///     The number of spaces to place before an unbroken 'else if' statement keyword.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeUnbrokenElseIfStatement
        {
            get { return _spacesBeforeUnbrokenElseIfStatement; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeUnbrokenElseIfStatement, value, "SpacesBeforeUnbrokenElseIfStatement");
            }
        }

        /// <summary>
        ///     Whether or not to place 'else if' on a new line in branch statements.
        /// </summary>
        public bool ElseIfStatementOnNewLine
        {
            get { return _elseIfStatementOnNewLine; }
            set { SetField(ref _elseIfStatementOnNewLine, value, "ElseIfStatementOnNewLine"); }
        }

        /// <summary>
        ///     Whether or not to place 'else' on a new line in branch statements.
        /// </summary>
        public bool ElseStatementOnNewLine
        {
            get { return _elseStatementOnNewLine; }
            set { SetField(ref _elseStatementOnNewLine, value, "ElseStatementOnNewLine"); }
        }

        /// <summary>
        ///     The number of spaces to place before an unbroken 'else' statement keyword.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanZero))]
        public int SpacesBeforeUnbrokenElseStatement
        {
            get { return _spacesBeforeUnbrokenElseStatement; }
            set
            {
                if (value < 0) value = 0;
                SetField(ref _spacesBeforeUnbrokenElseStatement, value, "SpacesBeforeUnbrokenElseStatement");
            }
        }

        /// <summary>
        ///     The maximum amount of new lines that can appear at the end of a code scope, the lowest possible value is 1.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MaximumNewLinesAtEndOfCodeScope
        {
            get { return _maximumNewLinesAtEndOfCodeScope; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _maximumNewLinesAtEndOfCodeScope, value, "MaximumNewLinesAtEndOfCodeScope");
            }
        }

        /// <summary>
        ///     The maximum amount of new lines that can appear at the beginning of a code scope, the lowest possible value is 1.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MaximumNewLinesAtBeginingOfCodeScope
        {
            get { return _maximumNewLinesAtBeginingOfCodeScope; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _maximumNewLinesAtBeginingOfCodeScope, value, "MaximumNewLinesAtBeginingOfCodeScope");
            }
        }

        /// <summary>
        ///     The minimum amount of new lines between distinct types of code statements inside of a code scope, the lowest
        ///     possible value is 1.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MinimumNewLinesBetweenDistinctLocalStatements
        {
            get { return _minimumNewLinesBetweenDistinctLocalStatements; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _minimumNewLinesBetweenDistinctLocalStatements, value,
                    "MinimumNewLinesBetweenDistinctLocalStatements");
            }
        }

        /// <summary>
        ///     The minimum amount of new lines between distinct types of code statements in the global scope, the lowest possible
        ///     value is 1.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MinimumNewLinesBetweenDistinctGlobalStatements
        {
            get { return _minimumNewLinesBetweenDistinctGlobalStatements; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _minimumNewLinesBetweenDistinctGlobalStatements, value,
                    "MinimumNewLinesBetweenDistinctGlobalStatements");
            }
        }

        /// <summary>
        ///     The maximum amount of new lines that can appear at the beginning of a state scope, the lowest possible value is 1.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MaximumNewLinesAtBeginingOfStateScope
        {
            get { return _maximumNewLinesAtBeginingOfStateScope; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _maximumNewLinesAtBeginingOfStateScope, value, "MaximumNewLinesAtBeginingOfStateScope");
            }
        }

        /// <summary>
        ///     The minimum amount of new lines between distinct types of code statement inside of a code scope, the lowest
        ///     possible value is 1.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MinimumNewLinesBetweenEventHandlers
        {
            get { return _minimumNewLinesBetweenEventHandlers; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _minimumNewLinesBetweenEventHandlers, value, "MinimumNewLinesBetweenEventHandlers");
            }
        }

        /// <summary>
        ///     The maximum amount of new lines that can appear at the end of a state scope, the lowest possible value is 1.
        /// </summary>
        [DefaultValueFactory(typeof (ResetIfLessThanOne))]
        public int MaximumNewLinesAtEndOfStateScope
        {
            get { return _maximumNewLinesAtEndOfStateScope; }
            set
            {
                if (value < 1) value = 1;
                SetField(ref _maximumNewLinesAtEndOfStateScope, value, "MaximumNewLinesAtEndOfStateScope");
            }
        }

        /// <summary>
        ///     Whether or not to add <see cref="SpacesBeforeOpeningWhileLoopBrace" /> before an opening while loop brace
        ///     that has been forcefully put on a new line, due to a comment appearing immediately before the brace.
        /// </summary>
        public bool AddSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak; }
            set
            {
                SetField(ref _addSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak, value,
                    "AddSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak");
            }
        }

        /// <summary>
        ///     Whether or not to add <see cref="SpacesBeforeOpeningDoLoopBrace" /> before an opening do loop brace
        ///     that has been forcefully put on a new line, due to a comment appearing immediately before the brace.
        /// </summary>
        public bool AddSpacesBeforeOpeningDoLoopBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningDoLoopBraceAfterCommentBreak; }
            set
            {
                SetField(ref _addSpacesBeforeOpeningDoLoopBraceAfterCommentBreak, value,
                    "AddSpacesBeforeOpeningDoLoopBraceAfterCommentBreak");
            }
        }

        /// <summary>
        ///     Whether or not to add <see cref="SpacesBeforeOpeningForLoopBrace" /> before an opening for loop brace
        ///     that has been forcefully put on a new line, due to a comment appearing immediately before the brace.
        /// </summary>
        public bool AddSpacesBeforeOpeningForLoopBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningForLoopBraceAfterCommentBreak; }
            set
            {
                SetField(ref _addSpacesBeforeOpeningForLoopBraceAfterCommentBreak, value,
                    "AddSpacesBeforeOpeningForLoopBraceAfterCommentBreak");
            }
        }

        /// <summary>
        ///     Whether or not to add <see cref="SpacesBeforeOpeningElseBrace" /> before an opening 'else' statement brace
        ///     that has been forcefully put on a new line, due to a comment appearing immediately before the brace.
        /// </summary>
        public bool AddSpacesBeforeOpeningElseBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningElseBraceAfterCommentBreak; }
            set
            {
                SetField(ref _addSpacesBeforeOpeningElseBraceAfterCommentBreak, value,
                    "AddSpacesBeforeOpeningElseBraceAfterCommentBreak");
            }
        }

        /// <summary>
        ///     Whether or not to add <see cref="SpacesBeforeOpeningElseIfBrace" /> before an opening 'else if' statement brace
        ///     that has been forcefully put on a new line, due to a comment appearing immediately before the brace.
        /// </summary>
        public bool AddSpacesBeforeOpeningElseIfBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningElseIfBraceAfterCommentBreak; }
            set
            {
                SetField(ref _addSpacesBeforeOpeningElseIfBraceAfterCommentBreak, value,
                    "AddSpacesBeforeOpeningElseIfBraceAfterCommentBreak");
            }
        }

        /// <summary>
        ///     Whether or not to add <see cref="SpacesBeforeOpeningIfBrace" /> before an opening 'if' statement brace
        ///     that has been forcefully put on a new line, due to a comment appearing immediately before the brace.
        /// </summary>
        public bool AddSpacesBeforeOpeningIfBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningIfBraceAfterCommentBreak; }
            set
            {
                SetField(ref _addSpacesBeforeOpeningIfBraceAfterCommentBreak, value,
                    "AddSpacesBeforeOpeningIfBraceAfterCommentBreak");
            }
        }

        /// <summary>
        ///     Whether or not to add <see cref="SpacesBeforeOpeningFunctionBrace" /> before an opening function declaration brace
        ///     that has been forcefully put on a new line, due to a comment appearing immediately before the brace.
        /// </summary>
        public bool AddSpacesBeforeOpeningFunctionBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningFunctionBraceAfterCommentBreak; }
            set
            {
                SetField(ref _addSpacesBeforeOpeningFunctionBraceAfterCommentBreak, value,
                    "AddSpacesBeforeOpeningFunctionBraceAfterCommentBreak");
            }
        }

        /// <summary>
        ///     Whether or not to add <see cref="SpacesBeforeOpeningEventBrace" /> before an opening event handler brace
        ///     that has been forcefully put on a new line, due to a comment appearing immediately before the brace.
        /// </summary>
        public bool AddSpacesBeforeOpeningEventBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningEventBraceAfterCommentBreak; }
            set
            {
                SetField(ref _addSpacesBeforeOpeningEventBraceAfterCommentBreak, value,
                    "AddSpacesBeforeOpeningEventBraceAfterCommentBreak");
            }
        }

        /// <summary>
        ///     Whether or not to add <see cref="SpacesBeforeOpeningStateBrace" /> before an opening state block brace
        ///     that has been forcefully put on a new line, due to a comment appearing immediately before the brace.
        /// </summary>
        public bool AddSpacesBeforeOpeningStateBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningStateBraceAfterCommentBreak; }
            set
            {
                SetField(ref _addSpacesBeforeOpeningStateBraceAfterCommentBreak, value,
                    "AddSpacesBeforeOpeningStateBraceAfterCommentBreak");
            }
        }

        private class ResetIfLessThanOne : IDefaultSettingsValueFactory
        {
            public bool CheckForNecessaryResets(MemberInfo member, object objectInstance, object settingValue)
            {
                if (settingValue is int)
                {
                    return (int) settingValue < 1;
                }
                return false;
            }


            public object GetDefaultValue(MemberInfo member, object objectInstance)
            {
                return ((PropertyInfo) member).GetValue(Defaults, null);
            }
        }

        private class ResetIfLessThanZero : IDefaultSettingsValueFactory
        {
            public bool CheckForNecessaryResets(MemberInfo member, object objectInstance, object settingValue)
            {
                if (settingValue is int)
                {
                    return (int) settingValue < 1;
                }
                return false;
            }


            public object GetDefaultValue(MemberInfo member, object objectInstance)
            {
                return ((PropertyInfo) member).GetValue(Defaults, null);
            }
        }
    }
}