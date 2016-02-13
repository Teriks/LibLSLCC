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

using System;
using System.Reflection;
using LibLSLCC.Settings;

namespace LibLSLCC.Formatter
{
    public class LSLCodeFormatterSettings : SettingsBaseClass<LSLCodeFormatterSettings>
    {
        //helpful VS regex: ({\s(.*?)\s=\svalue;)

        //replace with: { SetField(ref $2,value,"$2");

        //then edit strings.


        private bool _declarationExpressionWrapping = true;
        private int _columnsBeforeDeclarationExpressionWrap = 60;
        private int _minimumExpressionsInDeclarationToWrap = 2;
        private bool _statementExpressionWrapping = true;
        private int _columnsBeforeStatementExpressionWrap = 60;
        private int _minimumExpressionsInStatementToWrap = 2;
        private bool _returnExpressionWrapping = true;
        private int _columnsBeforeReturnExpressionWrap = 60;
        private int _minimumExpressionsInReturnToWrap = 2;
        private bool _ifExpressionWrapping = true;
        private int _columnsBeforeIfExpressionWrap = 60;
        private int _minimumExpressionsInIfToWrap = 2;
        private bool _elseIfExpressionWrapping = true;
        private int _columnsBeforeElseIfExpressionWrap = 60;
        private int _minimumExpressionsInElseIfToWrap = 2;
        private bool _whileExpressionWrapping = true;
        private int _columnsBeforeWhileExpressionWrap = 60;
        private int _minimumExpressionsInWhileToWrap = 2;
        private bool _doWhileExpressionWrapping = true;
        private int _columnsBeforeDoWhileExpressionWrap = 60;
        private int _minimumExpressionsInDoWhileToWrap = 2;
        private bool _ifStatementBracesOnNewLine = true;
        private bool _functionBracesOnNewLine = true;
        private bool _eventBracesOnNewLine = true;
        private bool _stateBracesOnNewLine = true;
        private bool _elseIfStatementBracesOnNewLine = true;
        private bool _elseStatementBracesOnNewLine = true;
        private bool _whileLoopBracesOnNewLine = true;
        private bool _doLoopBracesOnNewLine = true;
        private bool _forLoopBracesOnNewLine = true;
        private int _spacesBeforeOpeningStateBrace;
        private int _spacesBeforeClosingStateBrace;
        private int _spacesBeforeClosingIfBrace;
        private int _spacesBeforeOpeningIfBrace;
        private int _spacesBeforeClosingElseIfBrace;
        private int _spacesBeforeOpeningElseIfBrace;
        private int _spacesBeforeClosingElseBrace;
        private int _spacesBeforeOpeningElseBrace;
        private int _spacesBeforeClosingForBrace;
        private int _spacesBeforeOpeningForBrace;
        private int _spacesBeforeClosingDoLoopBrace;
        private int _spacesBeforeOpeningDoLoopBrace;
        private int _spacesBeforeClosingWhileLoopBrace;
        private int _spacesBeforeOpeningWhileLoopBrace;
        private int _spacesBeforeClosingEventBrace;
        private int _spacesBeforeOpeningEventBrace;
        private int _spacesBeforeClosingFunctionBrace;
        private int _spacesBeforeOpeningFunctionBrace;
        private bool _indentBracelessControlStatements = true;
        private int _spacesBeforeUnbrokenElseIfStatement = 1;
        private int _spacesBeforeUnbrokenElseStatement = 1;
        private bool _elseIfStatementOnNewLine = true;
        private bool _elseStatementOnNewLine = true;
        private int _maximumNewLinesAtEndOfCodeScope = 2;
        private int _maximumNewLinesAtBeginingOfCodeScope = 2;
        private int _minimumNewLinesBetweenDistinctStatements = 2;
        private int _maximumNewLinesAtBeginingOfStateScope = 2;
        private int _minimumNewLinesBetweenEventHandlers = 2;
        private int _maximumNewLinesAtEndOfStateScope = 2;
        private int _minimumNewLinesBetweenEventAndFollowingComment = 3;
        private bool _convertBracelessControlStatements;
        private string _newlineSequence = "\n";
        private string _tabString = "\t";
        private bool _stripComments;
        private bool _addSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningDoLoopBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningForLoopBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningElseBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningElseIfBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningIfBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningFunctionBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningEventBraceAfterCommentBreak;
        private bool _addSpacesBeforeOpeningStateBraceAfterCommentBreak;

        public bool StripComments
        {
            get { return _stripComments; }
            set { SetField(ref _stripComments, value, "StripComments"); }
        }

        public string NewlineSequence
        {
            get { return _newlineSequence; }
            set { SetField(ref _newlineSequence, value, "NewlineSequence"); }
        }

        public string TabString
        {
            get { return _tabString; }
            set { SetField(ref _tabString, value, "TabString"); }
        }

        public bool DeclarationExpressionWrapping
        {
            get { return _declarationExpressionWrapping; }
            set { SetField(ref _declarationExpressionWrapping, value, "DeclarationExpressionWrapping"); }
        }

        public int ColumnsBeforeDeclarationExpressionWrap
        {
            get { return _columnsBeforeDeclarationExpressionWrap; }
            set
            {
                SetField(ref _columnsBeforeDeclarationExpressionWrap, value, "ColumnsBeforeDeclarationExpressionWrap");
            }
        }

        public int MinimumExpressionsInDeclarationToWrap
        {
            get { return _minimumExpressionsInDeclarationToWrap; }
            set
            {
                SetField(ref _minimumExpressionsInDeclarationToWrap, value, "MinimumExpressionsInDeclarationToWrap");
            }
        }

        public bool StatementExpressionWrapping
        {
            get { return _statementExpressionWrapping; }
            set { SetField(ref _statementExpressionWrapping, value, "StatementExpressionWrapping"); }
        }

        public int ColumnsBeforeStatementExpressionWrap
        {
            get { return _columnsBeforeStatementExpressionWrap; }
            set { SetField(ref _columnsBeforeStatementExpressionWrap, value, "ColumnsBeforeStatementExpressionWrap"); }
        }

        public int MinimumExpressionsInStatementToWrap
        {
            get { return _minimumExpressionsInStatementToWrap; }
            set { SetField(ref _minimumExpressionsInStatementToWrap, value, "MinimumExpressionsInStatementToWrap"); }
        }

        public bool ReturnExpressionWrapping
        {
            get { return _returnExpressionWrapping; }
            set { SetField(ref _returnExpressionWrapping, value, "ReturnExpressionWrapping"); }
        }

        public int ColumnsBeforeReturnExpressionWrap
        {
            get { return _columnsBeforeReturnExpressionWrap; }
            set { SetField(ref _columnsBeforeReturnExpressionWrap, value, "ColumnsBeforeReturnExpressionWrap"); }
        }

        public int MinimumExpressionsInReturnToWrap
        {
            get { return _minimumExpressionsInReturnToWrap; }
            set { SetField(ref _minimumExpressionsInReturnToWrap, value, "MinimumExpressionsInReturnToWrap"); }
        }

        public bool IfExpressionWrapping
        {
            get { return _ifExpressionWrapping; }
            set { SetField(ref _ifExpressionWrapping, value, "IfExpressionWrapping"); }
        }

        public int ColumnsBeforeIfExpressionWrap
        {
            get { return _columnsBeforeIfExpressionWrap; }
            set { SetField(ref _columnsBeforeIfExpressionWrap, value, "ColumnsBeforeIfExpressionWrap"); }
        }

        public int MinimumExpressionsInIfToWrap
        {
            get { return _minimumExpressionsInIfToWrap; }
            set { SetField(ref _minimumExpressionsInIfToWrap, value, "MinimumExpressionsInIfToWrap"); }
        }

        public bool ElseIfExpressionWrapping
        {
            get { return _elseIfExpressionWrapping; }
            set { SetField(ref _elseIfExpressionWrapping, value, "ElseIfExpressionWrapping"); }
        }

        public int ColumnsBeforeElseIfExpressionWrap
        {
            get { return _columnsBeforeElseIfExpressionWrap; }
            set { SetField(ref _columnsBeforeElseIfExpressionWrap, value, "ColumnsBeforeElseIfExpressionWrap"); }
        }

        public int MinimumExpressionsInElseIfToWrap
        {
            get { return _minimumExpressionsInElseIfToWrap; }
            set { SetField(ref _minimumExpressionsInElseIfToWrap, value, "MinimumExpressionsInElseIfToWrap"); }
        }

        public bool WhileExpressionWrapping
        {
            get { return _whileExpressionWrapping; }
            set { SetField(ref _whileExpressionWrapping, value, "WhileExpressionWrapping"); }
        }

        public int ColumnsBeforeWhileExpressionWrap
        {
            get { return _columnsBeforeWhileExpressionWrap; }
            set { SetField(ref _columnsBeforeWhileExpressionWrap, value, "ColumnsBeforeWhileExpressionWrap"); }
        }

        public int MinimumExpressionsInWhileToWrap
        {
            get { return _minimumExpressionsInWhileToWrap; }
            set { SetField(ref _minimumExpressionsInWhileToWrap, value, "MinimumExpressionsInWhileToWrap"); }
        }

        public bool DoWhileExpressionWrapping
        {
            get { return _doWhileExpressionWrapping; }
            set { SetField(ref _doWhileExpressionWrapping, value, "DoWhileExpressionWrapping"); }
        }

        public int ColumnsBeforeDoWhileExpressionWrap
        {
            get { return _columnsBeforeDoWhileExpressionWrap; }
            set { SetField(ref _columnsBeforeDoWhileExpressionWrap, value, "ColumnsBeforeDoWhileExpressionWrap"); }
        }

        public int MinimumExpressionsInDoWhileToWrap
        {
            get { return _minimumExpressionsInDoWhileToWrap; }
            set { SetField(ref _minimumExpressionsInDoWhileToWrap, value, "MinimumExpressionsInDoWhileToWrap"); }
        }

        public bool IfStatementBracesOnNewLine
        {
            get { return _ifStatementBracesOnNewLine; }
            set { SetField(ref _ifStatementBracesOnNewLine, value, "IfStatementBracesOnNewLine"); }
        }

        public bool ElseIfStatementBracesOnNewLine
        {
            get { return _elseIfStatementBracesOnNewLine; }
            set { SetField(ref _elseIfStatementBracesOnNewLine, value, "ElseIfStatementBracesOnNewLine"); }
        }

        public bool ElseStatementBracesOnNewLine
        {
            get { return _elseStatementBracesOnNewLine; }
            set { SetField(ref _elseStatementBracesOnNewLine, value, "ElseStatementBracesOnNewLine"); }
        }


        public bool StateBracesOnNewLine
        {
            get { return _stateBracesOnNewLine; }
            set { SetField(ref _stateBracesOnNewLine, value, "StateBracesOnNewLine"); }
        }

        public bool EventBracesOnNewLine
        {
            get { return _eventBracesOnNewLine; }
            set { SetField(ref _eventBracesOnNewLine, value, "EventBracesOnNewLine"); }
        }

        public bool FunctionBracesOnNewLine
        {
            get { return _functionBracesOnNewLine; }
            set { SetField(ref _functionBracesOnNewLine, value, "FunctionBracesOnNewLine"); }
        }

        public bool WhileLoopBracesOnNewLine
        {
            get { return _whileLoopBracesOnNewLine; }
            set { SetField(ref _whileLoopBracesOnNewLine, value, "WhileLoopBracesOnNewLine"); }
        }

        public bool DoLoopBracesOnNewLine
        {
            get { return _doLoopBracesOnNewLine; }
            set { SetField(ref _doLoopBracesOnNewLine, value, "DoLoopBracesOnNewLine"); }
        }

        public bool ForLoopBracesOnNewLine
        {
            get { return _forLoopBracesOnNewLine; }
            set { SetField(ref _forLoopBracesOnNewLine, value, "ForLoopBracesOnNewLine"); }
        }

        public int SpacesBeforeOpeningStateBrace
        {
            get { return _spacesBeforeOpeningStateBrace; }
            set { SetField(ref _spacesBeforeOpeningStateBrace, value, "SpacesBeforeOpeningStateBrace"); }
        }

        public int SpacesBeforeClosingStateBrace
        {
            get { return _spacesBeforeClosingStateBrace; }
            set { SetField(ref _spacesBeforeClosingStateBrace, value, "SpacesBeforeClosingStateBrace"); }
        }

        public int SpacesBeforeClosingIfBrace
        {
            get { return _spacesBeforeClosingIfBrace; }
            set { SetField(ref _spacesBeforeClosingIfBrace, value, "SpacesBeforeClosingIfBrace"); }
        }

        public int SpacesBeforeOpeningIfBrace
        {
            get { return _spacesBeforeOpeningIfBrace; }
            set { SetField(ref _spacesBeforeOpeningIfBrace, value, "SpacesBeforeOpeningIfBrace"); }
        }

        public int SpacesBeforeClosingElseIfBrace
        {
            get { return _spacesBeforeClosingElseIfBrace; }
            set { SetField(ref _spacesBeforeClosingElseIfBrace, value, "SpacesBeforeClosingElseIfBrace"); }
        }

        public int SpacesBeforeOpeningElseIfBrace
        {
            get { return _spacesBeforeOpeningElseIfBrace; }
            set { SetField(ref _spacesBeforeOpeningElseIfBrace, value, "SpacesBeforeOpeningElseIfBrace"); }
        }

        public int SpacesBeforeClosingElseBrace
        {
            get { return _spacesBeforeClosingElseBrace; }
            set { SetField(ref _spacesBeforeClosingElseBrace, value, "SpacesBeforeClosingElseBrace"); }
        }

        public int SpacesBeforeOpeningElseBrace
        {
            get { return _spacesBeforeOpeningElseBrace; }
            set { SetField(ref _spacesBeforeOpeningElseBrace, value, "SpacesBeforeOpeningElseBrace"); }
        }

        public int SpacesBeforeClosingForBrace
        {
            get { return _spacesBeforeClosingForBrace; }
            set { SetField(ref _spacesBeforeClosingForBrace, value, "SpacesBeforeClosingForBrace"); }
        }

        public int SpacesBeforeOpeningForBrace
        {
            get { return _spacesBeforeOpeningForBrace; }
            set { SetField(ref _spacesBeforeOpeningForBrace, value, "SpacesBeforeOpeningForBrace"); }
        }

        public int SpacesBeforeClosingDoLoopBrace
        {
            get { return _spacesBeforeClosingDoLoopBrace; }
            set { SetField(ref _spacesBeforeClosingDoLoopBrace, value, "SpacesBeforeClosingDoLoopBrace"); }
        }

        public int SpacesBeforeOpeningDoLoopBrace
        {
            get { return _spacesBeforeOpeningDoLoopBrace; }
            set { SetField(ref _spacesBeforeOpeningDoLoopBrace, value, "SpacesBeforeOpeningDoLoopBrace"); }
        }

        public int SpacesBeforeClosingWhileLoopBrace
        {
            get { return _spacesBeforeClosingWhileLoopBrace; }
            set { SetField(ref _spacesBeforeClosingWhileLoopBrace, value, "SpacesBeforeClosingWhileLoopBrace"); }
        }

        public int SpacesBeforeOpeningWhileLoopBrace
        {
            get { return _spacesBeforeOpeningWhileLoopBrace; }
            set { SetField(ref _spacesBeforeOpeningWhileLoopBrace, value, "SpacesBeforeOpeningWhileLoopBrace"); }
        }

        public int SpacesBeforeClosingEventBrace
        {
            get { return _spacesBeforeClosingEventBrace; }
            set { SetField(ref _spacesBeforeClosingEventBrace, value, "SpacesBeforeClosingEventBrace"); }
        }

        public int SpacesBeforeOpeningEventBrace
        {
            get { return _spacesBeforeOpeningEventBrace; }
            set { SetField(ref _spacesBeforeOpeningEventBrace, value, "SpacesBeforeOpeningEventBrace"); }
        }

        public int SpacesBeforeClosingFunctionBrace
        {
            get { return _spacesBeforeClosingFunctionBrace; }
            set { SetField(ref _spacesBeforeClosingFunctionBrace, value, "SpacesBeforeClosingFunctionBrace"); }
        }

        public int SpacesBeforeOpeningFunctionBrace
        {
            get { return _spacesBeforeOpeningFunctionBrace; }
            set { SetField(ref _spacesBeforeOpeningFunctionBrace, value, "SpacesBeforeOpeningFunctionBrace"); }
        }

        public bool IndentBracelessControlStatements
        {
            get { return _indentBracelessControlStatements; }
            set
            {
                if (value) ConvertBracelessControlStatements = false;
                SetField(ref _indentBracelessControlStatements, value, "IndentBracelessControlStatements");
            }
        }

        public bool ConvertBracelessControlStatements
        {
            get { return _convertBracelessControlStatements; }
            set
            {
                if (value) IndentBracelessControlStatements = false;
                SetField(ref _convertBracelessControlStatements, value, "ConvertBracelessControlStatements");
            }
        }

        public int SpacesBeforeUnbrokenElseIfStatement
        {
            get { return _spacesBeforeUnbrokenElseIfStatement; }
            set { SetField(ref _spacesBeforeUnbrokenElseIfStatement, value, "SpacesBeforeUnbrokenElseIfStatement"); }
        }

        public bool ElseIfStatementOnNewLine
        {
            get { return _elseIfStatementOnNewLine; }
            set { SetField(ref _elseIfStatementOnNewLine, value, "ElseIfStatementOnNewLine"); }
        }

        public bool ElseStatementOnNewLine
        {
            get { return _elseStatementOnNewLine; }
            set { SetField(ref _elseStatementOnNewLine, value, "ElseStatementOnNewLine"); }
        }

        public int SpacesBeforeUnbrokenElseStatement
        {
            get { return _spacesBeforeUnbrokenElseStatement; }
            set { SetField(ref _spacesBeforeUnbrokenElseStatement, value, "SpacesBeforeUnbrokenElseStatement"); }
        }

        public int MaximumNewLinesAtEndOfCodeScope
        {
            get { return _maximumNewLinesAtEndOfCodeScope; }
            set { SetField(ref _maximumNewLinesAtEndOfCodeScope, value, "MaximumNewLinesAtEndOfCodeScope"); }
        }

        public int MaximumNewLinesAtBeginingOfCodeScope
        {
            get { return _maximumNewLinesAtBeginingOfCodeScope; }
            set { SetField(ref _maximumNewLinesAtBeginingOfCodeScope, value, "MaximumNewLinesAtBeginingOfCodeScope"); }
        }

        public int MinimumNewLinesBetweenDistinctStatements
        {
            get { return _minimumNewLinesBetweenDistinctStatements; }
            set
            {
                SetField(ref _minimumNewLinesBetweenDistinctStatements, value,
                    "MinimumNewLinesBetweenDistinctStatements");
            }
        }

        public int MaximumNewLinesAtBeginingOfStateScope
        {
            get { return _maximumNewLinesAtBeginingOfStateScope; }
            set
            {
                SetField(ref _maximumNewLinesAtBeginingOfStateScope, value, "MaximumNewLinesAtBeginingOfStateScope");
            }
        }

        public int MinimumNewLinesBetweenEventHandlers
        {
            get { return _minimumNewLinesBetweenEventHandlers; }
            set { SetField(ref _minimumNewLinesBetweenEventHandlers, value, "MinimumNewLinesBetweenEventHandlers"); }
        }

        public int MaximumNewLinesAtEndOfStateScope
        {
            get { return _maximumNewLinesAtEndOfStateScope; }
            set { SetField(ref _maximumNewLinesAtEndOfStateScope, value, "MaximumNewLinesAtEndOfStateScope"); }
        }

        public int MinimumNewLinesBetweenEventAndFollowingComment
        {
            get { return _minimumNewLinesBetweenEventAndFollowingComment; }
            set
            {
                SetField(ref _minimumNewLinesBetweenEventAndFollowingComment, value,
                    "MinimumNewLinesBetweenEventAndFollowingComment");
            }
        }

        public bool AddSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak; }
            set { SetField(ref _addSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak,value,"AddSpacesBeforeOpeningWhileLoopBraceAfterCommentBreak"); }
        }


        public bool AddSpacesBeforeOpeningDoLoopBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningDoLoopBraceAfterCommentBreak; }
            set { SetField(ref _addSpacesBeforeOpeningDoLoopBraceAfterCommentBreak,value,"AddSpacesBeforeOpeningDoLoopBraceAfterCommentBreak"); }
        }


        public bool AddSpacesBeforeOpeningForLoopBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningForLoopBraceAfterCommentBreak; }
            set { SetField(ref _addSpacesBeforeOpeningForLoopBraceAfterCommentBreak,value,"AddSpacesBeforeOpeningForLoopBraceAfterCommentBreak"); }
        }

        public bool AddSpacesBeforeOpeningElseBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningElseBraceAfterCommentBreak; }
            set { SetField(ref _addSpacesBeforeOpeningElseBraceAfterCommentBreak,value,"AddSpacesBeforeOpeningElseBraceAfterCommentBreak"); }
        }

        public bool AddSpacesBeforeOpeningElseIfBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningElseIfBraceAfterCommentBreak; }
            set { SetField(ref _addSpacesBeforeOpeningElseIfBraceAfterCommentBreak,value,"AddSpacesBeforeOpeningElseIfBraceAfterCommentBreak"); }
        }

        public bool AddSpacesBeforeOpeningIfBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningIfBraceAfterCommentBreak; }
            set { SetField(ref _addSpacesBeforeOpeningIfBraceAfterCommentBreak,value,"AddSpacesBeforeOpeningIfBraceAfterCommentBreak"); }
        }

        public bool AddSpacesBeforeOpeningFunctionBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningFunctionBraceAfterCommentBreak; }
            set { SetField(ref _addSpacesBeforeOpeningFunctionBraceAfterCommentBreak,value,"AddSpacesBeforeOpeningFunctionBraceAfterCommentBreak"); }
        }

        public bool AddSpacesBeforeOpeningEventBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningEventBraceAfterCommentBreak; }
            set { SetField(ref _addSpacesBeforeOpeningEventBraceAfterCommentBreak,value,"AddSpacesBeforeOpeningEventBraceAfterCommentBreak"); }
        }

        public bool AddSpacesBeforeOpeningStateBraceAfterCommentBreak
        {
            get { return _addSpacesBeforeOpeningStateBraceAfterCommentBreak; }
            set { SetField(ref _addSpacesBeforeOpeningStateBraceAfterCommentBreak,value,"AddSpacesBeforeOpeningStateBraceAfterCommentBreak"); }
        }
    }
}