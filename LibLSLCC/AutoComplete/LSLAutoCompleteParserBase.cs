#region FileInfo

// 
// File: LSLAutoCompleteParserBase.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2016, Teriks
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
using System.Text.RegularExpressions;
using LibLSLCC.CodeValidator;
using LibLSLCC.Collections;
using LibLSLCC.Utility;

#endregion

namespace LibLSLCC.AutoComplete
{
    /// <summary>
    ///     An abstract base class with common functionality for auto complete parsers.
    /// </summary>
    public abstract class LSLAutoCompleteParserBase : ILSLAutoCompleteParser
    {
        private static readonly Regex JumpRegex = new Regex("jump\\s*(" + LSLTokenTools.IDRegexString + ")");
        private static readonly Regex LabelRegex = new Regex("@\\s*(" + LSLTokenTools.IDRegexString + ")");

        /// <summary>
        ///     Sets the <see cref="ILSLAutoCompleteParserState" /> implementation.
        /// </summary>
        protected ILSLAutoCompleteParserState ParserState { get; set; }

        /// <summary>
        ///     The offset the <see cref="LSLAutoCompleteParser" /> last parsed to.
        /// </summary>
        public int ParseToOffset
        {
            get { return ParserState.ParseToOffset; }
        }

        /// <summary>
        ///     The name of the state block <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> resides in.
        ///     <c>null</c> if the parse to offset is outside of a state body.
        /// </summary>
        public string CurrentState
        {
            get { return ParserState.CurrentState; }
        }

        /// <summary>
        ///     The name of the function declaration <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> resides in.
        ///     <c>null</c> if the parse to offset is outside of a function body.
        /// </summary>
        public string CurrentFunction
        {
            get { return ParserState.CurrentFunction; }
        }

        /// <summary>
        ///     The name of the event handler declaration <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> offset resides
        ///     in.
        ///     <c>null</c> if the parse to offset is outside of an event body.
        /// </summary>
        public string CurrentEvent
        {
            get { return ParserState.CurrentEvent; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a state block, but outside of
        ///     an
        ///     event handler declaration.
        /// </summary>
        public bool InStateScope
        {
            get { return ParserState.InStateScope; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the source code range of an event
        ///     handler.
        ///     This includes being within the name or parameter definitions.
        /// </summary>
        public bool InEventSourceRange
        {
            get { return ParserState.InEventSourceRange; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset"/> is inside of a string literal.
        /// </summary>
        public bool InString
        {
            get { return ParserState.InString; }
        }

        /// <summary>
        ///     <see cref="ILSLAutoCompleteParserState.InLineComment"/> or <see cref="ILSLAutoCompleteParserState.InBlockComment"/>
        /// </summary>
        public bool InComment
        {
            get { return ParserState.InComment; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset"/> is inside of a line style comment.
        /// </summary>
        public bool InLineComment
        {
            get { return ParserState.InLineComment; }
        }

        /// <summary>
        ///     <c>true</c> if auto complete could not take place because the character preceding the given <see cref="ILSLAutoCompleteParserState.ParseToOffset"/>
        ///     was not a valid suggestion prefix.
        /// </summary>
        public bool InvalidPrefix
        {
            get { return ParserState.InvalidPrefix; }
        }

        /// <summary>
        ///     <c>true</c> if auto complete could not take place because the keyword preceding the given <see cref="ILSLAutoCompleteParserState.ParseToOffset"/>
        ///     prevented a suggestion.
        /// </summary>
        public bool InvalidKeywordPrefix
        {
            get { return ParserState.InvalidKeywordPrefix; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset"/> is inside of a multi line block style comment.
        /// </summary>
        public bool InBlockComment
        {
            get { return ParserState.InBlockComment; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is anywhere inside the code body of an
        ///     event handler.
        /// </summary>
        public bool InEventCodeBody
        {
            get { return ParserState.InEventCodeBody; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is anywhere inside the code body of a
        ///     function
        ///     declaration.
        /// </summary>
        public bool InFunctionCodeBody
        {
            get { return ParserState.InFunctionCodeBody; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the global scope.
        /// </summary>
        public bool InGlobalScope
        {
            get { return ParserState.InGlobalScope; }
        }

        /// <summary>
        ///     Gets a list of <see cref="LSLAutoCompleteStateBlock" /> objects representing user defined script state blocks.
        /// </summary>
        public IReadOnlyGenericArray<LSLAutoCompleteStateBlock> StateBlocks
        {
            get { return ParserState.StateBlocks; }
        }

        /// <summary>
        ///     Gets a <see cref="LSLAutoCompleteStateBlock" /> object representing the scripts default state.
        /// </summary>
        public LSLAutoCompleteStateBlock DefaultState
        {
            get { return ParserState.DefaultState; }
        }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalVariable" /> objects representing global variables
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteGlobalVariable> GlobalVariables
        {
            get { return ParserState.GlobalVariables; }
        }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteLocalVariable" /> objects representing local variables
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteLocalVariable> LocalVariables
        {
            get { return ParserState.LocalVariables; }
        }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing global function
        ///     declarations
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.  The functions are keyed in the
        ///     hash
        ///     map by name.
        /// </summary>
        public IReadOnlyHashMap<string, LSLAutoCompleteGlobalFunction> GlobalFunctionsDictionary
        {
            get { return ParserState.GlobalFunctionsDictionary; }
        }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteGlobalVariable" /> objects representing global variable
        ///     declarations
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.  The declarations are keyed in the
        ///     hash
        ///     map by name.
        /// </summary>
        public IReadOnlyHashMap<string, LSLAutoCompleteGlobalVariable> GlobalVariablesDictionary
        {
            get { return ParserState.GlobalVariablesDictionary; }
        }

        /// <summary>
        ///     Gets a read only hash map of <see cref="LSLAutoCompleteLocalParameter" /> objects representing local parameter
        ///     declarations
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.  The declarations are keyed in the
        ///     hash
        ///     map by name.
        /// </summary>
        public IReadOnlyHashMap<string, LSLAutoCompleteLocalParameter> LocalParametersDictionary
        {
            get { return ParserState.LocalParametersDictionary; }
        }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing global functions
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteGlobalFunction> GlobalFunctions
        {
            get { return ParserState.GlobalFunctions; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in an area where a code statement can
        ///     exist.
        ///     (<see cref="ILSLAutoCompleteParserState.InMultiCodeStatementArea" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InBracelessCodeStatementArea" />)
        /// </summary>
        public bool InCodeStatementArea
        {
            get { return ParserState.InCodeStatementArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in a multi statement area where a code
        ///     statement can exist.
        /// </summary>
        public bool InMultiCodeStatementArea
        {
            get { return ParserState.InMultiCodeStatementArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in an area where a brace-less code scope
        ///     (single statement) can exist.
        /// </summary>
        public bool InBracelessCodeStatementArea
        {
            get { return ParserState.InBracelessCodeStatementArea; }
        }

        /// <summary>
        ///     Gets an enumerable of <see cref="LSLAutoCompleteGlobalFunction" /> objects representing local parameters
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public IEnumerable<LSLAutoCompleteLocalParameter> LocalParameters
        {
            get { return ParserState.LocalParameters; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a binary expression/prefix
        ///     expression/postfix expression or parenthesized expression.
        /// </summary>
        public bool InExpressionTree
        {
            get { return ParserState.InExpressionTree; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is to the right of the dot in a dot member
        ///     accessor expression.
        /// </summary>
        public bool RightOfDotAccessor
        {
            get { return ParserState.RightOfDotAccessor; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of the expression used to declare
        ///     a
        ///     local variable.
        /// </summary>
        public bool InLocalVariableDeclarationExpression
        {
            get { return ParserState.InLocalVariableDeclarationExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of the expression used to declare
        ///     a
        ///     global variable.
        /// </summary>
        public bool InGlobalVariableDeclarationExpression
        {
            get { return ParserState.InGlobalVariableDeclarationExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a function declarations
        ///     parameter
        ///     declaration list.
        /// </summary>
        public bool InFunctionDeclarationParameterList
        {
            get { return ParserState.InFunctionDeclarationParameterList; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of an event declarations
        ///     parameter
        ///     declaration list.
        /// </summary>
        public bool InEventDeclarationParameterList
        {
            get { return ParserState.InEventDeclarationParameterList; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of an 'if' statements condition
        ///     expression area.
        /// </summary>
        public bool InIfConditionExpression
        {
            get { return ParserState.InIfConditionExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of an 'else if' statements
        ///     condition
        ///     expression area.
        /// </summary>
        public bool InElseIfConditionExpression
        {
            get { return ParserState.InElseIfConditionExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a function declaration or
        ///     event
        ///     declaration code body.
        /// </summary>
        public bool InCodeBody
        {
            get { return ParserState.InCodeBody; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in a local expression area, such as a
        ///     condition area or function call arguments. etc..
        /// </summary>
        public bool InLocalExpressionArea
        {
            get { return ParserState.InLocalExpressionArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in a global area. currently only when
        ///     <see cref="ILSLAutoCompleteParserState.InGlobalVariableDeclarationExpression" /> is <c>true</c>.
        /// </summary>
        public bool InGlobalExpressionArea
        {
            get { return ParserState.InGlobalExpressionArea; }
        }

        /// <summary>
        ///     <see cref="ILSLAutoCompleteParserState.InGlobalExpressionArea" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InLocalExpressionArea" />.
        ///     If the offset is in a global variable declaration expression, or the start of one.  Or
        ///     a local expression area such as an expression statement, loop condition, function call parameters, for loop clauses
        ///     etc..
        /// </summary>
        public bool InExpressionArea
        {
            get { return ParserState.InExpressionArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the expression area of a return
        ///     statement
        ///     inside of a function.
        /// </summary>
        public bool InFunctionReturnExpression
        {
            get { return ParserState.InFunctionReturnExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the expression area right of a
        ///     compound
        ///     operation/assignment to a variable, such as after var += (here).
        /// </summary>
        public bool InModifyingVariableAssignmentExpression
        {
            get { return ParserState.InModifyingVariableAssignmentExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the expression area right of a
        ///     compound
        ///     operation/assignment to a member of a variable, such as after var.x += (here).
        /// </summary>
        public bool InModifyingComponentAssignmentExpression
        {
            get { return ParserState.InModifyingComponentAssignmentExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the expression area right of an
        ///     assignment
        ///     to a variable, such as after var = (here).
        /// </summary>
        public bool InPlainVariableAssignmentExpression
        {
            get { return ParserState.InPlainVariableAssignmentExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the expression area right of an
        ///     assignment
        ///     to a member of a variable, such as after var.x = (here).
        /// </summary>
        public bool InPlainComponentAssignmentExpression
        {
            get { return ParserState.InPlainComponentAssignmentExpression; }
        }

        /// <summary>
        ///     <see cref="ILSLAutoCompleteParserState.InPlainVariableAssignmentExpression" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InModifyingVariableAssignmentExpression" />
        /// </summary>
        public bool InVariableAssignmentExpression
        {
            get { return ParserState.InVariableAssignmentExpression; }
        }

        /// <summary>
        ///     <see cref="ILSLAutoCompleteParserState.InPlainComponentAssignmentExpression" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InModifyingComponentAssignmentExpression" />
        /// </summary>
        public bool InComponentAssignmentExpression
        {
            get { return ParserState.InComponentAssignmentExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in an area where you could start typing
        ///     the
        ///     name of the state in a state change statement.
        /// </summary>
        public bool InStateChangeStatementStateNameArea
        {
            get { return ParserState.InStateChangeStatementStateNameArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in an area where you could start typing
        ///     the
        ///     name of the label in a jump statement.
        /// </summary>
        public bool InJumpStatementLabelNameArea
        {
            get { return ParserState.InJumpStatementLabelNameArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in an area where you could start typing
        ///     the
        ///     name of a label.
        /// </summary>
        public bool InLabelDefinitionNameArea
        {
            get { return ParserState.InLabelDefinitionNameArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is anywhere in a for loops clauses area.
        /// </summary>
        public bool InForLoopClausesArea
        {
            get { return ParserState.InForLoopClausesArea; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in a do while statements condition area.
        /// </summary>
        public bool InDoWhileConditionExpression
        {
            get { return ParserState.InDoWhileConditionExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in a while statements condition area.
        /// </summary>
        public bool InWhileConditionExpression
        {
            get { return ParserState.InWhileConditionExpression; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is in the source code range of a control
        ///     statement.
        /// </summary>
        public bool InControlStatementSourceRange
        {
            get { return ParserState.InControlStatementSourceRange; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a function calls parameter
        ///     expression list.
        /// </summary>
        public bool InFunctionCallParameterList
        {
            get { return ParserState.InFunctionCallParameterList; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a list literals initializer
        ///     expression list.
        /// </summary>
        public bool InListLiteralInitializer
        {
            get { return ParserState.InListLiteralInitializer; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a vector literals initializer
        ///     expression list.
        /// </summary>
        public bool InVectorLiteralInitializer
        {
            get { return ParserState.InVectorLiteralInitializer; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is inside of a rotation literals
        ///     initializer
        ///     expression list.
        /// </summary>
        public bool InRotationLiteralInitializer
        {
            get { return ParserState.InRotationLiteralInitializer; }
        }

        /// <summary>
        ///     <c>true</c> if a library constant can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InExpressionArea" />
        /// </summary>
        public bool CanSuggestLibraryConstant
        {
            get { return ParserState.CanSuggestLibraryConstant; }
        }

        /// <summary>
        ///     <c>true</c> if a function call can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     (<see cref="ILSLAutoCompleteParserState.InLocalExpressionArea" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />)
        /// </summary>
        public bool CanSuggestFunction
        {
            get { return ParserState.CanSuggestFunction; }
        }

        /// <summary>
        ///     <c>true</c> if a local variable or parameter name can be suggested at
        ///     <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     (<see cref="ILSLAutoCompleteParserState.InLocalExpressionArea" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />)
        /// </summary>
        public bool CanSuggestLocalVariableOrParameter
        {
            get { return ParserState.CanSuggestLocalVariableOrParameter; }
        }

        /// <summary>
        ///     <c>true</c> if a global variable can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     (<see cref="ILSLAutoCompleteParserState.InLocalExpressionArea" /> ||
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />)
        /// </summary>
        public bool CanSuggestGlobalVariable
        {
            get { return ParserState.CanSuggestGlobalVariable; }
        }

        /// <summary>
        ///     <c>true</c> if an event handler can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InStateScope" />
        /// </summary>
        public bool CanSuggestEventHandler
        {
            get { return ParserState.CanSuggestEventHandler; }
        }

        /// <summary>
        ///     <c>true</c> if a state name can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InStateChangeStatementStateNameArea" />
        /// </summary>
        public bool CanSuggestStateName
        {
            get { return ParserState.CanSuggestStateName; }
        }

        /// <summary>
        ///     <c>true</c> if a label name for a jump target can be suggested at
        ///     <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InJumpStatementLabelNameArea" />
        /// </summary>
        public bool CanSuggestLabelNameJumpTarget
        {
            get { return ParserState.CanSuggestLabelNameJumpTarget; }
        }

        /// <summary>
        ///     <c>true</c> if a label definitions name can be suggested at
        ///     <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InLabelDefinitionNameArea" />
        /// </summary>
        public bool CanSuggestLabelNameDefinition
        {
            get { return ParserState.CanSuggestLabelNameDefinition; }
        }

        /// <summary>
        ///     <c>true</c> if an LSL type name can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public bool CanSuggestTypeName
        {
            get { return ParserState.CanSuggestTypeName; }
        }

        /// <summary>
        ///     Gets the computed scope address at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        public LSLAutoCompleteScopeAddress ScopeAddressAtOffset
        {
            get { return ParserState.ScopeAddressAtOffset; }
        }

        /// <summary>
        ///     Gets the source code range of the code body <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> exists inside
        ///     of.
        /// </summary>
        public LSLSourceCodeRange CurrentCodeAreaRange
        {
            get { return ParserState.CurrentCodeAreaRange; }
        }

        /// <summary>
        ///     <c>true</c> if <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is after an 'if' or 'else if' statements
        ///     code
        ///     body.
        /// </summary>
        public bool AfterIfOrElseIfStatement
        {
            get { return ParserState.AfterIfOrElseIfStatement; }
        }

        /// <summary>
        ///     <c>true</c> if a control statement chain can be suggested at
        ///     <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestControlStatement
        {
            get { return ParserState.CanSuggestControlStatement; }
        }

        /// <summary>
        ///     <c>true</c> if a state change statement can be suggested at
        ///     <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestStateChangeStatement
        {
            get { return ParserState.CanSuggestStateChangeStatement; }
        }

        /// <summary>
        ///     <c>true</c> if a return statement can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestReturnStatement
        {
            get { return ParserState.CanSuggestReturnStatement; }
        }

        /// <summary>
        ///     <c>true</c> if a jump statement can be suggested at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        ///     <see cref="ILSLAutoCompleteParserState.InCodeStatementArea" />
        /// </summary>
        public bool CanSuggestJumpStatement
        {
            get { return ParserState.CanSuggestJumpStatement; }
        }

        /// <summary>
        ///     Gets the return type of the function declaration that <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is
        ///     currently in the code body of.
        /// </summary>
        public LSLType CurrentFunctionReturnType
        {
            get { return ParserState.CurrentFunctionReturnType; }
        }


        /// <summary>
        ///     Get an enumerable of <see cref="LSLAutoCompleteLocalLabel" /> objects representing local labels
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.
        /// </summary>
        /// <param name="sourceCode">The source code of the entire script.</param>
        /// <returns>An enumerable of <see cref="LSLAutoCompleteLocalLabel" /> objects that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.</returns>
        /// <exception cref="InvalidOperationException">If <see cref="Parse" /> has not been called first.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="sourceCode" /> is <c>null</c>.</exception>
        public IEnumerable<LSLAutoCompleteLocalLabel> GetLocalLabels(string sourceCode)
        {
            if (sourceCode == null)
            {
                throw new ArgumentNullException("sourceCode");
            }
            if (ParserState == null)
            {
                throw new InvalidOperationException("Parse state does not exist, Parse has not been called.");
            }


            var len = CurrentCodeAreaRange.StopIndex - CurrentCodeAreaRange.StartIndex;


            var match = LabelRegex.Match(sourceCode, CurrentCodeAreaRange.StartIndex, len);

            var names = new HashSet<string>();

            while (match.Success)
            {
                var name = match.Groups[1].ToString();

                match = match.NextMatch();

                if (names.Contains(name))
                {
                    continue;
                }
                names.Add(name);
                yield return new LSLAutoCompleteLocalLabel(name);
            }
        }


        /// <summary>
        ///     Get an enumerable of <see cref="LSLAutoCompleteLocalJump" /> objects representing local jump statements
        ///     that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />. 
        /// </summary>
        /// <param name="sourceCode">The source code of the entire script.</param>
        /// <returns>An enumerable of <see cref="LSLAutoCompleteLocalJump" /> objects that are accessible at <see cref="ILSLAutoCompleteParserState.ParseToOffset" />.</returns>
        /// <exception cref="InvalidOperationException">If <see cref="Parse" /> has not been called first.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="sourceCode" /> is <c>null</c>.</exception>
        public IEnumerable<LSLAutoCompleteLocalJump> GetLocalJumps(string sourceCode)
        {
            if (sourceCode == null)
            {
                throw new ArgumentNullException("sourceCode");
            }
            if (ParserState == null)
            {
                throw new InvalidOperationException("Parse state does not exist, Parse has not been called.");
            }

            var len = CurrentCodeAreaRange.StopIndex - CurrentCodeAreaRange.StartIndex;


            var match = JumpRegex.Match(sourceCode, CurrentCodeAreaRange.StartIndex, len);

            var names = new HashSet<string>();

            while (match.Success)
            {
                var name = match.Groups[1].ToString();

                match = match.NextMatch();

                if (names.Contains(name))
                {
                    continue;
                }
                names.Add(name);
                yield return new LSLAutoCompleteLocalJump(name);
            }
        }



        /// <summary>
        ///     Preforms an auto-complete parse on the specified stream of LSL source code, up to an arbitrary offset.
        /// </summary>
        /// <param name="code">The input source code.</param>
        /// <param name="toOffset">To offset to parse up to (the cursor offset).</param>
        /// <param name="options">Parse options.</param>
        public abstract void Parse(string code, int toOffset, LSLAutoCompleteParseOptions options);


        /// <summary>
        /// Determine if autocomplete should be blocked if the only thing separating a given keyword from <see cref="ILSLAutoCompleteParserState.ParseToOffset" /> is whitespace. <para/>
        /// In other words, autocomplete cannot continue if <paramref name="keyword"/> comes before the cursor with only whitespace inbetween.
        /// </summary>
        /// <param name="keyword">The keyword or character sequence to test.</param>
        /// <returns><c>true</c> if the keyword/sequence blocks autocomplete.</returns>
        /// <seealso cref="LSLAutoCompleteParseOptions.BlockOnInvalidKeywordPrefix"/>
        /// <seealso cref="ILSLAutoCompleteParserState.InvalidKeywordPrefix"/>
        /// <exception cref="ArgumentNullException"><paramref name="keyword"/> is <see langword="null" />.</exception>
        public abstract bool IsInvalidSuggestionKeywordPrefix(string keyword);


        /// <summary>
        /// Determine if a given character can come immediately before an autocomplete suggestion.  An empty string represents the begining of the source code.
        /// </summary>
        /// <param name="character">The character to test, or an empty string.</param>
        /// <returns><c>true</c> if the given character can appear before a suggestion.</returns>
        /// <seealso cref="LSLAutoCompleteParseOptions.BlockOnInvalidPrefix"/>
        /// <seealso cref="ILSLAutoCompleteParserState.InvalidPrefix"/>
        /// <exception cref="ArgumentNullException"><paramref name="character"/> is <see langword="null" />.</exception>
        public abstract bool IsValidSuggestionPrefix(string character);


        /// <summary>
        /// Determine if a given character can come immediately after an autocomplete suggestion.  An empty string represents the end of the source code.
        /// </summary>
        /// <param name="character">The character to test, or an empty string.</param>
        /// <returns><c>true</c> if the given character can appear after a suggestion.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="character"/> is <see langword="null" />.</exception>
        public abstract bool IsValidSuggestionSuffix(string character);


    }
}